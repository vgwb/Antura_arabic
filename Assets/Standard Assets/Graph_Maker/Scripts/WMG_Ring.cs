using UnityEngine;
using System.Collections;

public class WMG_Ring : WMG_GUI_Functions {
	
	public GameObject ring;
	public GameObject band;
	public GameObject label;
	public GameObject textLine;
	public GameObject labelText;
	public GameObject lowerLabelText;
	public GameObject labelPoint;
	public GameObject labelBackground;
	public GameObject lowerLabelBackground;
	public GameObject line;
	public GameObject interactibleObj;

	public WMG_Ring_Graph graph { get; private set; }
	public int ringIndex { get; private set; }

	public Sprite ringSprite { get; private set; }
	public Sprite bandSprite { get; private set; }
	private int texSize;

	float animTimeline;

	public void initialize(WMG_Ring_Graph graph) {
		this.graph = graph;
		texSize = graph.textureResolution;
		ringSprite = WMG_Util.createSprite(texSize, texSize);
		bandSprite = WMG_Util.createSprite(texSize, texSize);
		setTexture(ring, ringSprite);
		setTexture(band, bandSprite);
		changeSpriteParent(label, graph.ringLabelsParent);
		graph.addRingClickEvent (interactibleObj);
		graph.addRingMouseEnterEvent (interactibleObj);
		ringIndex = graph.rings.Count;
	}

	public void updateRingTexture(int ringNum, float ringFill, float bandFill) {
		float ringRadius = graph.getRingRadius(graph.pieMode ? 0 : ringNum);
		// rings
		graph.textureChanger(ring, ringSprite, (2*ringNum + 1), graph.outerRadius*2, ringRadius - graph.ringWidth, ringRadius, graph.antiAliasing, graph.antiAliasingStrength, ringFill);
		// bands
		if (graph.bandMode) {
			graph.textureChanger(band, bandSprite, (2*ringNum + 1) + 1, graph.outerRadius*2, ringRadius + graph.bandPadding, 
			                     (graph.pieMode ? graph.outerRadius : (graph.getRingRadius(ringNum + 1))) - graph.ringWidth - graph.bandPadding, graph.antiAliasing, graph.antiAliasingStrength, bandFill);
		}
	}

	public void animBandFill(int ringNum, float endFill) {
		if (!graph.bandMode) return;
		if (!graph.useComputeShader) {
			WMG_Anim.animFill(band, graph.animDuration, graph.animEaseType, endFill);
		}
		else {
			animTimeline = 0;
			WMG_Anim.animFloatCallbackU(() => animTimeline, x=> animTimeline = x, graph.animDuration, 1, 
			                            () => onUpdateAnimateBandFill(ringNum, endFill), graph.animEaseType);
		}
		if (graph.pieMode) {
			WMG_Anim.animFill (interactibleObj, graph.animDuration, graph.animEaseType, endFill);
		}
	}

	void onUpdateAnimateBandFill(int ringNum, float endFill) {
		float ringRadius = graph.getRingRadius(graph.pieMode ? 0 : ringNum);
		float newFill = WMG_Util.RemapFloat(animTimeline, 0, 1, 0, endFill);
		graph.textureChanger(band, bandSprite, (2*ringNum + 1) + 1, graph.outerRadius*2, ringRadius + graph.bandPadding, 
		                     (graph.pieMode ? graph.outerRadius : (graph.getRingRadius(ringNum + 1))) - graph.ringWidth - graph.bandPadding, graph.antiAliasing, graph.antiAliasingStrength, newFill);
	}
}
