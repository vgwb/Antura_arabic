using UnityEngine;
using System.Collections;

public class WMG_Bezier_Band : WMG_GUI_Functions {
	
	public GameObject bandFillSpriteGO;
	public GameObject bandLineSpriteGO;
	public GameObject labelParent;
	public GameObject percentLabel;
	public GameObject label;

	private Sprite bandFillSprite;
	private Sprite bandLineSprite;
	private Material bandFillMat;
	private Material bandLineMat;
	private Color[] bandFillColors;
	private Color[] bandLineColors;
	private int texSize;
	private WMG_Bezier_Band_Graph graph;
	public float cumulativePercent;
	public float prevCumulativePercent;
	private int superSamplingRate = 3;
	private int xPad = 2;

	public void Init(WMG_Bezier_Band_Graph graph) {
		this.graph = graph;
		texSize = graph.textureResolution;
		bandFillSprite = WMG_Util.createSprite(texSize, texSize);
		bandLineSprite = WMG_Util.createSprite(texSize, texSize);
		bandFillColors = new Color[texSize * texSize];
		bandLineColors = new Color[texSize * texSize];
		setTexture(bandFillSpriteGO, bandFillSprite);
		setTexture(bandLineSpriteGO, bandLineSprite);
	}

	public void setCumulativePercents(float val, float prev) {
		cumulativePercent = val / graph.TotalVal;
		prevCumulativePercent = prev / graph.TotalVal;
	}

	public void setFillColor(Color color) {
		changeSpriteColor(bandFillSpriteGO, color);
	}

	public void setLineColor(Color color) {
		changeSpriteColor(bandLineSpriteGO, color);
	}

	public void UpdateBand() {
		updateColors (ref bandFillColors, ref bandLineColors);
		bandFillSprite.texture.SetPixels(bandFillColors);
		bandFillSprite.texture.Apply();
		bandLineSprite.texture.SetPixels(bandLineColors);
		bandLineSprite.texture.Apply();
	}
	
	// \mathbf{B}(t)=(1-t)^3\mathbf{P}_0+3(1-t)^2t\mathbf{P}_1+3(1-t)t^2\mathbf{P}_2+t^3\mathbf{P}_3 \mbox{ , } 0 \le t \le 1.
	void updateColors(ref Color[] bandFillColors, ref Color[] bandLineColors) {

		// reset the colors
		for (int i = 0; i < bandFillColors.Length; i++) {
			bandFillColors[i].a = 0;
			bandLineColors[i].a = 0;
		}

		int bandWidth = Mathf.Max(0,graph.bandLineWidth-1);

		int[] fillStartPoints = new int[texSize];

		for (int k = 0; k <= 1; k++) { // k == 0 is for the band's borders "bandLine", and k == 1 is for the band itself "bandFill"

			float offset = graph.startHeightPercent * (texSize - 1);
			float valOffset = prevCumulativePercent;
			if (k == 1) {
				valOffset = cumulativePercent;
			}

			Vector2 start = new Vector2(0, (texSize - 1)/2f + (0.5f - valOffset) * offset);
			Vector2 end = new Vector2((texSize - 1), (1 - valOffset) * (texSize - 1) - (k == 1 ? -graph.bandSpacing : graph.bandSpacing));
			if (k == 1 && cumulativePercent == 1 && graph.bandSpacing < graph.bandLineWidth) {
				end = new Vector2(end.x, end.y + (graph.bandLineWidth - graph.bandSpacing));
			}
			Vector2 p1 = new Vector2(graph.cubicBezierP1.x * end.x, start.y + graph.cubicBezierP1.y * (end.y - start.y));
			Vector2 p2 = new Vector2((1- graph.cubicBezierP2.x) * end.x, end.y - graph.cubicBezierP2.y * (end.y - start.y));

			int[] startPoints = new int[texSize];
//			float[] slopes = new float[size];
			for (int i = 0; i < texSize*superSamplingRate; i++) {
				Vector2 pos = cubicBezier(start, p1, p2, end, i / (texSize*superSamplingRate-1f));
				int posX = Mathf.RoundToInt(pos.x);
				int posY = Mathf.RoundToInt(pos.y);
				startPoints[posX] = Mathf.Max(posY, startPoints[posX]);
//				Vector2 slope = cubicBezierSlope(start, p1, p2, end, i / (size*superSamplingRate-1f));
//				slopes[posX] = Mathf.Max(slopes[posX], slope.y / slope.x);
			}




			start = new Vector2(start.x, start.y - bandWidth);
			end = new Vector2(end.x, end.y - bandWidth);
			p1 = new Vector2(graph.cubicBezierP1.x * end.x, start.y + graph.cubicBezierP1.y * (end.y - start.y));
			p2 = new Vector2((1- graph.cubicBezierP2.x) * end.x, end.y - graph.cubicBezierP2.y * (end.y - start.y));

			int[] endPoints = new int[texSize];
			for (int i = 0; i < texSize; i++) {
				endPoints[i] = (texSize - 1);
			}
			for (int i = 0; i < texSize*superSamplingRate; i++) {
				Vector2 pos = cubicBezier(start, p1, p2, end, i / (texSize*superSamplingRate-1f));
				int posX = Mathf.RoundToInt(pos.x);
				int posY = Mathf.RoundToInt(pos.y);
				endPoints[posX] = Mathf.Min(posY, endPoints[posX]);

			}

			for (int i = xPad; i < (texSize-xPad); i++) {
				int width = (startPoints[i] - endPoints[i]);
				width++;

				// get fill start points
				if (k == 0) {
					fillStartPoints[i] = endPoints[i]-1;
				}

				for (int j = 0; j < width; j++) {
					int y = (startPoints[i] -j);
					float alpha = 1f - (Mathf.Abs((j - width/2f)) / (width/2f));
					bandLineColors[i + texSize * y] = new Color(1, 1, 1, alpha * alpha);
				}

				// Fill
				if (k == 1) {
					int bandWidth2 = (fillStartPoints[i] - startPoints[i]);
					
					for (int j = 0; j < bandWidth2; j++) {
						int y = (fillStartPoints[i] -j);
						bandFillColors[i + texSize * y] = Color.white;
					}

					// Alias top edge of band fill
					int alphaFadeWidth = Mathf.Min(bandWidth/2, bandWidth2/2);
					for (int j = 0; j < alphaFadeWidth; j++) {
						float alpha = (j+1f) / alphaFadeWidth;
						int y = (fillStartPoints[i] -j);
						bandFillColors[i + texSize * y] = new Color(1, 1, 1, alpha);
					}

					for (int j = 0; j < alphaFadeWidth; j++) {
						float alpha = (j+1f) / alphaFadeWidth;
						int y = (startPoints[i] +j);
						bandFillColors[i + texSize * y] = new Color(1, 1, 1, alpha);
					}
				}
			}

		}
	}
	
	// B(t) = (1-t)^3 P0 + 3((1-t)^2)t P1 + 3(1-t)t^2 P2 + t^3 P3 for 0 <= t <= 1
	Vector2 cubicBezier (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		float r = 1f - t;
		float f0 = r * r * r;
		float f1 = r * r * t * 3;
		float f2 = r * t * t * 3;
		float f3 = t * t * t;
		return f0*p0 + f1*p1 + f2*p2 + f3*p3;
	}

//	// B(t) = 3(1-t)^2 (P1-P0) + 6(1-t)t (P2-P1) + 3t^2 (P3-P2) for 0 <= t <= 1
//	Vector2 cubicBezierSlope (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
//		float r = 1f - t;
//		float f0 = r * r * 3;
//		float f1 = r * t * 6;
//		float f2 = t * t * 3;
//		return f0*(p1-p0) + f1*(p2-p1) + f2*(p3-p2);
//	}

}
