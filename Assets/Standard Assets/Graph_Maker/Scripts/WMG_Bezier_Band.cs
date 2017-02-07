using UnityEngine;
using System.Collections;

public class WMG_Bezier_Band : WMG_GUI_Functions {
	
	public GameObject bandFillSpriteGO;
	public GameObject bandLineSpriteGO;
	public GameObject labelParent;
	public GameObject percentLabel;
	public GameObject label;
//	public GameObject bandBotLineSpriteGO;

	private Sprite bandFillSprite;
	private Sprite bandLineSprite;
	private Material bandFillMat;
	private Material bandLineMat;
//	private Sprite bandBotLineSprite;
	private Color[] bandFillColors;
	private Color[] bandLineColors;
//	private Color[] bandBotLineColors;
	private int texSize;
	private WMG_Bezier_Band_Graph graph;
	public float cumulativePercent;
	public float prevCumulativePercent;
	private int size;
	private int maxS;
	private int superSamplingRate = 3;
	private int xPad = 2;

	public void Init(WMG_Bezier_Band_Graph graph) {
		this.graph = graph;
//		theGraph.setTextureMaterial(areaShadingRects[i], matToUse);
		bandFillSprite = WMG_Util.createSprite(getTexture(bandFillSpriteGO));
		bandLineSprite = WMG_Util.createSprite(getTexture(bandLineSpriteGO));
//		bandFillSprite = WMG_Util.createAlphaSprite(1024);
//		bandLineSprite = WMG_Util.createAlphaSprite(1024);
//		bandBotLineSprite = WMG_Util.createSprite(getTexture(bandBotLineSpriteGO));
		texSize = bandFillSprite.texture.width;
//		texSize = 1024;
		bandFillColors = new Color[texSize * texSize];
		bandLineColors = new Color[texSize * texSize];
//		bandBotLineColors = new Color[texSize * texSize];
		setTexture(bandFillSpriteGO, bandFillSprite);
		setTexture(bandLineSpriteGO, bandLineSprite);
//		setTexture(bandBotLineSpriteGO, bandBotLineSprite);
		size = Mathf.RoundToInt(Mathf.Sqrt(bandFillColors.Length));
//		size = 1024;
		maxS = size - 1;
//		setTextureMaterial(bandFillSpriteGO, graph.uiGradient);
//		setTextureMaterial(bandLineSpriteGO, graph.uiGradient);
//		bandFillMat = getTextureMaterial(bandFillSpriteGO);
//		bandLineMat = getTextureMaterial(bandLineSpriteGO);
	}

	public void setCumulativePercents(float val, float prev) {
		cumulativePercent = val / graph.TotalVal;
		prevCumulativePercent = prev / graph.TotalVal;
	}

	public void setFillColor(Color color) {
		changeSpriteColor(bandFillSpriteGO, color);
//		bandFillMat.SetColor("_color", color);
	}

	public void setLineColor(Color color) {
		changeSpriteColor(bandLineSpriteGO, color);
//		bandLineMat.SetColor("_color", color);
	}

	public void UpdateBand() {
//		byte[] bandFillColors = new byte[1024*1024];
//		byte[] bandLineColors = new byte[1024*1024];
		updateColors (ref bandFillColors, ref bandLineColors);
		bandFillSprite.texture.SetPixels(bandFillColors);
//		bandFillSprite.texture.LoadRawTextureData(bandFillColors);
		bandFillSprite.texture.Apply();
		bandLineSprite.texture.SetPixels(bandLineColors);
//		bandLineSprite.texture.LoadRawTextureData(bandLineColors);
		bandLineSprite.texture.Apply();
//		bandBotLineSprite.texture.SetPixels(bandBotLineColors);
//		bandBotLineSprite.texture.Apply();
//		bandFillMat.SetTexture("_alphaGradient", bandFillSprite.texture);
//		bandLineMat.SetTexture("_alphaGradient", bandLineSprite.texture);
//		bandFillMat.SetTexture("_alphaGradient", testTex);
//		bandLineMat.SetTexture("_alphaGradient", testTex);
	}
	
	// \mathbf{B}(t)=(1-t)^3\mathbf{P}_0+3(1-t)^2t\mathbf{P}_1+3(1-t)t^2\mathbf{P}_2+t^3\mathbf{P}_3 \mbox{ , } 0 \le t \le 1.
	void updateColors(ref Color[] bandFillColors, ref Color[] bandLineColors) {

		bandFillColors = new Color[texSize * texSize];
		bandLineColors = new Color[texSize * texSize];
//		bandBotLineColors = new Color[texSize * texSize];
		int bandWidth = Mathf.Max(0,graph.bandLineWidth-1);

		int[] fillStartPoints = new int[size];

		for (int k = 0; k <= 1; k++) {

			float offset = graph.startHeightPercent * maxS;
			float valOffset = prevCumulativePercent;
			if (k == 1) {
				valOffset = cumulativePercent;
			}

			Vector2 start = new Vector2(0, maxS/2f + (0.5f - valOffset) * offset);
			Vector2 end = new Vector2(maxS, (1 - valOffset) * maxS - (k == 1 ? -graph.bandSpacing : graph.bandSpacing));
			if (k == 1 && cumulativePercent == 1 && graph.bandSpacing < graph.bandLineWidth) {
				end = new Vector2(end.x, end.y + (graph.bandLineWidth - graph.bandSpacing));
			}
			Vector2 p1 = new Vector2(graph.cubicBezierP1.x * end.x, start.y + graph.cubicBezierP1.y * (end.y - start.y));
			Vector2 p2 = new Vector2((1- graph.cubicBezierP2.x) * end.x, end.y - graph.cubicBezierP2.y * (end.y - start.y));

			int[] startPoints = new int[size];
//			float[] slopes = new float[size];
			for (int i = 0; i < size*superSamplingRate; i++) {
				Vector2 pos = cubicBezier(start, p1, p2, end, i / (size*superSamplingRate-1f));
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

			int[] endPoints = new int[size];
			for (int i = 0; i < size; i++) {
				endPoints[i] = maxS;
			}
			for (int i = 0; i < size*superSamplingRate; i++) {
				Vector2 pos = cubicBezier(start, p1, p2, end, i / (size*superSamplingRate-1f));
				int posX = Mathf.RoundToInt(pos.x);
				int posY = Mathf.RoundToInt(pos.y);
				endPoints[posX] = Mathf.Min(posY, endPoints[posX]);

			}

			for (int i = xPad; i < (size-xPad); i++) {
				int width = (startPoints[i] - endPoints[i]);
				width++;

				// get fill start points
				if (k == 0) {
					fillStartPoints[i] = endPoints[i]-1;
				}

				for (int j = 0; j < width; j++) {
					int y = (startPoints[i] -j);
					float alpha = 1f - (Mathf.Abs((j - width/2f)) / (width/2f));
//					if (k == 0) {
					bandLineColors[i + size * y] = new Color(1, 1, 1, alpha * alpha);
//					bandLineColors[i + size * y] = (byte)Mathf.RoundToInt(alpha * alpha * 255);
//					}
//					else {
//						bandBotLineColors[i + size * y] = new Color(1, 1, 1, alpha * alpha);
//					}
				}

				// Fill
				if (k == 1) {
					int bandWidth2 = (fillStartPoints[i] - startPoints[i]);
					
					for (int j = 0; j < bandWidth2; j++) {
						int y = (fillStartPoints[i] -j);
						bandFillColors[i + size * y] = Color.white;
//						bandFillColors[i + size * y] = (byte)255;
					}

					// Alias top edge of band fill
					int alphaFadeWidth = Mathf.Min(bandWidth/2, bandWidth2/2);
					for (int j = 0; j < alphaFadeWidth; j++) {
						float alpha = (j+1f) / alphaFadeWidth;
						int y = (fillStartPoints[i] -j);
						bandFillColors[i + size * y] = new Color(1, 1, 1, alpha);
//						bandFillColors[i + size * y] = (byte)Mathf.RoundToInt(alpha*255);
					}

					for (int j = 0; j < alphaFadeWidth; j++) {
						float alpha = (j+1f) / alphaFadeWidth;
						int y = (startPoints[i] +j);
						bandFillColors[i + size * y] = new Color(1, 1, 1, alpha);
//						bandFillColors[i + size * y] = (byte)Mathf.RoundToInt(alpha*255);
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
