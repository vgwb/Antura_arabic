using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class WMG_Util {

	public static float RemapFloat(float val, float start1, float end1, float start2, float end2) {
		return start2 + ((val - start1) / (end1 - start1)) * (end2 - start2);
	}

	public static Vector2 RemapVec2(float val, float start1, float end1, Vector2 start2, Vector2 end2) {
		float valPercent = (val - start1) / (end1 - start1);
		return new Vector2(
			start2.x + valPercent * (end2.x - start2.x),
			start2.y + valPercent * (end2.y - start2.y));
	}

	public static Sprite createSprite(Texture2D tex) {
		Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Bilinear;
		return Sprite.Create(newTex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
	}

	public static Texture2D createTexture(int resolution) {
		Texture2D newTex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;
		return newTex;
	}

	public static Texture2D createTexture(int x, int y) {
		Texture2D newTex = new Texture2D(x, y, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;
		return newTex;
	}

	public static Sprite createAlphaSprite(int resolution) {
		Texture2D newTex = new Texture2D(resolution, resolution, TextureFormat.Alpha8, false);
		return Sprite.Create(newTex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100);
	}

	public static void listChanged<T>(bool editorChange, ref WMG_List<T> w_list, ref List<T> list, bool oneValChanged, int index) {
		if (editorChange) {
			if (oneValChanged) w_list.SetValViaEditor(index, list[index]);
			else w_list.SetListViaEditor(list);
		} else {
			if (oneValChanged) list[index] = w_list[index];
			else list = new List<T> (w_list);
		}
	}

	public static void updateBandColors(ref Color[] colors, float maxSize, float inner, float outer, bool antiAliasing, float antiAliasingStrength, Color[] orig = null) {
		int size = Mathf.RoundToInt(Mathf.Sqrt(colors.Length));
		float texFactor = maxSize / size;
		inner = inner / texFactor;
		outer = outer / texFactor;
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				int colorIndex = i + size * j;
				Color newColor = (orig == null ? new Color(1, 1, 1, 1) : orig[colorIndex]);
				int centerX = i - size / 2;
				int centerY = j - size / 2;
				float dist = Mathf.Sqrt(centerX * centerX + centerY * centerY);
				if (dist >= inner && dist < outer) {
					if (antiAliasing) {
						if (dist >= inner + antiAliasingStrength && dist < outer - antiAliasingStrength) {
							colors[colorIndex] = newColor;
						}
						else {
							if (dist > inner + antiAliasingStrength) {
								colors[colorIndex] = new Color(newColor.r, newColor.g, newColor.b, (outer - dist) / antiAliasingStrength);
							}
							else {
								colors[colorIndex] = new Color(newColor.r, newColor.g, newColor.b, (dist - inner) / antiAliasingStrength);
							}
						}
					}
					else {
						colors[colorIndex] = newColor;
					}
				}
				else {
					colors[colorIndex] = new Color(1, 1, 1, 0);
				}
			}
		}
	}

	/// <summary>
	/// Returns true if a line segment intersect a circle.
	/// Line segment is (x0, y0) to (x1, y1). Circle is centered at (x2, y2) with radius r
	/// </summary>
	/// <returns><c>true</c>, if circle and line intersect, <c>false</c> otherwise.</returns>
	/// <param name="x0">X0.</param>
	/// <param name="y0">Y0.</param>
	/// <param name="x1">The first x value.</param>
	/// <param name="y1">The first y value.</param>
	/// <param name="x2">The second x value.</param>
	/// <param name="y2">The second y value.</param>
	/// <param name="r">The red component.</param>
	public static bool LineIntersectsCircle(float x0, float y0, float x1, float y1, float x2, float y2, float r) {
		// Translate everything so that line segment start point to (0, 0)
		float a = x1-x0; // Line segment end point horizontal coordinate
		float b = y1-y0; // Line segment end point vertical coordinate
		float c = x2-x0; // Circle center horizontal coordinate
		float d = y2-y0; // Circle center vertical coordinate
		bool startInside = false;
		bool endInside = false;
		bool middleInside = false;
		if (r*r*(a*a + b*b) - (d*a - c*b)*(d*a - c*b) >= 0) {
			// Collision is possible, discriminant is greater than or equal to 0
			if (c*c + d*d <= r*r) {
				// Line segment start point is inside the circle, simply equation of circle start point (x0, y0) was translated by circle origin
				startInside = true;
			}
			if ((a-c)*(a-c) + (b-d)*(b-d) <= r*r) {
				// Line segment end point is inside the circle
				endInside = true;
			}
			if (!startInside && !endInside && c*a + d*b >= 0 && c*a + d*b <= a*a + b*b) {
				// Middle section only
				middleInside = true;
			}
		}
		return startInside || endInside || middleInside;
	}

	/// <summary>
	/// Returns true if line segment (p1x,p1y) - (p2x,p2y) intersects line segment (p3x,p3y) - (p4x,p4y) 
	/// </summary>
	/// <returns><c>true</c>, if line and line intersect, <c>false</c> otherwise.</returns>
	/// <param name="p1x">P1x.</param>
	/// <param name="p1y">P1y.</param>
	/// <param name="p2x">P2x.</param>
	/// <param name="p2y">P2y.</param>
	/// <param name="p3x">P3x.</param>
	/// <param name="p3y">P3y.</param>
	/// <param name="p4x">P4x.</param>
	/// <param name="p4y">P4y.</param>
	public static bool LineSegmentsIntersect(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y, float p4x, float p4y) {
		if (PointInterArea (p1x, p1y, p2x, p2y, p3x, p3y) * PointInterArea (p1x, p1y, p2x, p2y, p4x, p4y) < 0 &&
			PointInterArea (p3x, p3y, p4x, p4y, p1x, p1y) * PointInterArea (p3x, p3y, p4x, p4y, p2x, p2y) < 0) {
			return true;
		}
		return false;
	}

	static float PointInterArea(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y) {
		// Returns orientation of point 3 relative to line segment formed by points 1 and 2
		// If positive then clockwise, if negative then counterclockwise
		return (p2y - p1y) * (p3x - p2x) - (p2x - p1x) * (p3y - p2y);
	}
	
}
