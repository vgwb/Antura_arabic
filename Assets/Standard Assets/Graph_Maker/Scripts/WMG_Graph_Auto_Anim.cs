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
				aSeries.SeriesDataChanged += SeriesDataChangedMethod;
			}
			else {
				aSeries.SeriesDataChanged -= SeriesDataChangedMethod;
			}
		}
	}
	
	public void addSeriesForAutoAnim(WMG_Series aSeries) {
		aSeries.SeriesDataChanged += SeriesDataChangedMethod;
	}
	
	private void SeriesDataChangedMethod(WMG_Series aSeries) {
		// Animate the points, links, and bars
		List<GameObject> objects = aSeries.getPoints();
		for (int i = 0; i < objects.Count; i++) {
			if (aSeries.seriesIsLine) {

				// For line graphs, need to animate links as well via callback functions
				GameObject go = objects[i]; // otherwise causes warnings when used in lambda expression for DOTween callback

				string tweenID = aSeries.GetHashCode() + "autoAnim" + i;
				bool isLast = (i == objects.Count-1);
				if (aSeries.currentlyAnimating) {
					DOTween.Kill(tweenID);
					animateLinkCallback(aSeries, go, isLast);
				}
				WMG_Anim.animPositionCallbacks(go, theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, 
				                               new Vector3(aSeries.AfterPositions()[i].x, aSeries.AfterPositions()[i].y),
				                               ()=> animateLinkCallback(aSeries, go, isLast), ()=> animateLinkCallbackEnd(aSeries, isLast), tweenID);
			}
			else {
				// For bar graphs, animate widths and heights in addition to position. Depending on pivot / GUI system, animating width / height also affects position
				Vector2 newPos = theGraph.getChangeSpritePositionTo(objects[i], new Vector2(aSeries.AfterPositions()[i].x, aSeries.AfterPositions()[i].y));

				WMG_Anim.animPosition(objects[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype,
				                      new Vector3(newPos.x, newPos.y));

				WMG_Anim.animSize(objects[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype,
				                  new Vector2(aSeries.AfterWidths()[i], aSeries.AfterHeights()[i]));
			}
		}
		// Animate the data point labels
		List<GameObject> dataLabels = aSeries.getDataLabels();
		for (int i = 0; i < dataLabels.Count; i++) {
			if (aSeries.seriesIsLine) {
				float newX = aSeries.dataLabelsOffset.x;
				float newY = aSeries.dataLabelsOffset.y;
				Vector2 newPos = theGraph.getChangeSpritePositionTo(dataLabels[i], new Vector2(newX, newY));
				newPos = new Vector2(newPos.x + aSeries.AfterPositions()[i].x, newPos.y + aSeries.AfterPositions()[i].y);
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					newPos = new Vector2 (newPos.x, newPos.y + aSeries.AfterHeights()[i] / 2f);
				}
				else {
					newPos = new Vector2 (newPos.x + aSeries.AfterHeights()[i] / 2f, newPos.y);
				}
				WMG_Anim.animPosition(dataLabels[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype,
				                      new Vector3(newPos.x, newPos.y));
			}
			else {
				float newY = aSeries.dataLabelsOffset.y + aSeries.AfterPositions()[i].y + theGraph.barWidth / 2;
				float newX = aSeries.dataLabelsOffset.x + aSeries.AfterPositions()[i].x + aSeries.AfterWidths()[i];
				if (aSeries.getBarIsNegative(i)) {
					newX = -aSeries.dataLabelsOffset.x - aSeries.AfterWidths()[i] + Mathf.RoundToInt((theGraph.barAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue) * theGraph.xAxisLength);
				}
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					newY = aSeries.dataLabelsOffset.y + aSeries.AfterPositions()[i].y + aSeries.AfterHeights()[i];
					newX = aSeries.dataLabelsOffset.x + aSeries.AfterPositions()[i].x + theGraph.barWidth / 2;
					if (aSeries.getBarIsNegative(i)) {
						newY = -aSeries.dataLabelsOffset.y - aSeries.AfterHeights()[i] + Mathf.RoundToInt((theGraph.barAxisValue - aSeries.yAxis.AxisMinValue) / (aSeries.yAxis.AxisMaxValue - aSeries.yAxis.AxisMinValue) * theGraph.yAxisLength);
					}
				}
				Vector2 newPos = theGraph.getChangeSpritePositionTo(dataLabels[i], new Vector2(newX, newY));
				WMG_Anim.animPosition(dataLabels[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype,
				                      new Vector3(newPos.x, newPos.y));
			}
		}

		if (!aSeries.currentlyAnimating) {
			aSeries.currentlyAnimating = true;
		}

	}

	private void animateLinkCallback(WMG_Series aSeries, GameObject aGO, bool isLast) {
		WMG_Node aNode = aGO.GetComponent<WMG_Node>();
		if (aNode.links.Count != 0) {
			WMG_Link theLine = aNode.links[aNode.links.Count-1].GetComponent<WMG_Link>();
			theLine.Reposition();
		}
		if (isLast) {
			aSeries.updateAreaShading(null);
		}
		if (aSeries.connectFirstToLast) { // One extra link to animate for circles / close loop series
			aNode = aSeries.getPoints()[0].GetComponent<WMG_Node>();
			WMG_Link theLine = aNode.links[0].GetComponent<WMG_Link>();
			theLine.Reposition();
		}
	}

	private void animateLinkCallbackEnd(WMG_Series aSeries, bool isLast) {
		aSeries.RepositionLines();
		if (isLast) {
			aSeries.updateAreaShading(null);
		}
		aSeries.currentlyAnimating = false;
	}
}
