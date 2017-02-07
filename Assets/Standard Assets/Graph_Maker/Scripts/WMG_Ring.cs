using UnityEngine;
using System.Collections;

public class WMG_Ring : WMG_GUI_Functions {
	
	public GameObject ring;
	public GameObject band;
	public GameObject label;
	public GameObject textLine;
	public GameObject labelText;
	public GameObject labelPoint;
	public GameObject labelBackground;
	public GameObject line;

	private Sprite ringSprite;
	private Sprite bandSprite;
	private WMG_Ring_Graph graph;
	private int ringTexSize;
	private int bandTexSize;

	public void initialize(WMG_Ring_Graph graph) {
		ringSprite = WMG_Util.createSprite(getTexture(ring));
		bandSprite = WMG_Util.createSprite(getTexture(band));
		setTexture(ring, ringSprite);
		setTexture(band, bandSprite);
		this.graph = graph;
		changeSpriteParent(label, graph.ringLabelsParent);
	}

	public void updateRing(int ringNum) {
		float ringRadius = graph.getRingRadius(ringNum);
		// rings
		graph.textureChanger(ring, ringSprite, (2*ringNum + 1), graph.outerRadius*2, ringRadius - graph.ringWidth, ringRadius, graph.antiAliasing, graph.antiAliasingStrength);
		// bands
		if (graph.bandMode) {
			SetActive(band, true);
			graph.textureChanger(band, bandSprite, (2*ringNum + 1) + 1, graph.outerRadius*2, ringRadius + graph.bandPadding, 
			                     graph.getRingRadius(ringNum + 1) - graph.ringWidth - graph.bandPadding, graph.antiAliasing, graph.antiAliasingStrength);
		}
		else {
			SetActive(band, false);
		}
	}

	public void updateRingPoint(int ringNum) {
		float ringRadius = graph.getRingRadius(ringNum);
		// label points
		if (graph.bandMode && graph.ringColor.a == 0) { // center on bands
			float nextRingRadius = graph.getRingRadius(ringNum+1);
			changeSpritePositionToY (labelPoint, -(ringRadius + (nextRingRadius - ringRadius) / 2 - graph.RingWidthFactor * graph.ringWidth / 2));
		} else { // center on rings
			changeSpritePositionToY (labelPoint, -(ringRadius - graph.RingWidthFactor * graph.ringWidth / 2));
		}
		int pointWidthHeight = Mathf.RoundToInt (graph.RingWidthFactor * graph.ringWidth + graph.RingWidthFactor * graph.ringPointWidthFactor);
		changeSpriteSize (labelPoint, pointWidthHeight, pointWidthHeight);
	}

}
