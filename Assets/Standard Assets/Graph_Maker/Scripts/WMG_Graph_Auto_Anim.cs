using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WMG_Graph_Auto_Anim : MonoBehaviour {
	
	public WMG_Axis_Graph theGraph;
	
	public void subscribeToEvents(bool val) {
		for (int j = 0; j < theGraph.lineSeries.Count; j++) {
			if (!theGraph.activeInHierarchy(theGraph.lineSeries[j])) continue;
			WMG_Series aSeries = theGraph.lineSeries[j].GetComponent<WMG_Series>();
			if (val) {
				aSeries.SeriesAutoAnimStarted += SeriesAutoAnimStartedMethod;
			}
			else {
				aSeries.SeriesAutoAnimStarted -= SeriesAutoAnimStartedMethod;
			}
		}
	}
	
	public void addSeriesForAutoAnim(WMG_Series aSeries) {
		aSeries.SeriesAutoAnimStarted += SeriesAutoAnimStartedMethod;
	}
	
	private void SeriesAutoAnimStartedMethod(WMG_Series aSeries) {
		if (aSeries.currentlyAnimating) {
			DOTween.Kill (aSeries.autoAnimTweenId); // stop existing animation, if previously already animating
		}
		DOTween.To (x => aSeries.autoAnimationTimeline = x, 0, 1, theGraph.autoAnimationsDuration).SetEase(theGraph.autoAnimationsEasetype)
			.OnUpdate(() => onAutoAnimUpdate(aSeries)).OnComplete(() => onAutoAnimComplete(aSeries)).SetId(aSeries.autoAnimTweenId);

		aSeries.currentlyAnimating = true;
	}

	private void onAutoAnimUpdate(WMG_Series aSeries) {
		List<Vector2> newPositions = new List<Vector2>(); 
		List<int> newWidths = new List<int>(); 
		List<int> newHeights = new List<int>();

		for (int i = 0; i < aSeries.AfterPositions.Count; i++) {
			newPositions.Add(WMG_Util.RemapVec2(aSeries.autoAnimationTimeline, 0, 1, aSeries.OrigPositions[i], aSeries.AfterPositions[i]));
			newWidths.Add(Mathf.RoundToInt(WMG_Util.RemapFloat(aSeries.autoAnimationTimeline, 0, 1, aSeries.OrigWidths[i], aSeries.AfterWidths[i])));
			newHeights.Add(Mathf.RoundToInt(WMG_Util.RemapFloat(aSeries.autoAnimationTimeline, 0, 1, aSeries.OrigHeights[i], aSeries.AfterHeights[i])));
		}

		aSeries.UpdateVisuals (newPositions, newWidths, newHeights);
	}

	private void onAutoAnimComplete(WMG_Series aSeries) {
		aSeries.currentlyAnimating = false;
	}
}
