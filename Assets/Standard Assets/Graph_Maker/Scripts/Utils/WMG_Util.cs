using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Static utility class for performing miscellaneous functions such as data generation functions.
/// </summary>
public static class WMG_Util {

	/// <summary>
	/// Remaps the value of a float from one range to another range.
	/// </summary>
	/// <returns>The float.</returns>
	/// <param name="val">Value.</param>
	/// <param name="start1">Start1.</param>
	/// <param name="end1">End1.</param>
	/// <param name="start2">Start2.</param>
	/// <param name="end2">End2.</param>
	public static float RemapFloat(float val, float start1, float end1, float start2, float end2) {
		return start2 + ((val - start1) / (end1 - start1)) * (end2 - start2);
	}

	/// <summary>
	/// Remaps a float value to a Vector2 using a source float range and target Vector2 range. 
	/// </summary>
	/// <returns>The vec2.</returns>
	/// <param name="val">Value.</param>
	/// <param name="start1">Start1.</param>
	/// <param name="end1">End1.</param>
	/// <param name="start2">Start2.</param>
	/// <param name="end2">End2.</param>
	public static Vector2 RemapVec2(float val, float start1, float end1, Vector2 start2, Vector2 end2) {
		float valPercent = (val - start1) / (end1 - start1);
		return new Vector2(
			start2.x + valPercent * (end2.x - start2.x),
			start2.y + valPercent * (end2.y - start2.y));
	}

	public static Sprite createSprite(int x, int y) {
		Texture2D newTex = new Texture2D(x, y, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Bilinear;
		return Sprite.Create(newTex, new Rect(0, 0, x, y), new Vector2(0.5f, 0.5f), 100);
	}

	public static Texture2D createTexture(int x, int y) {
		Texture2D newTex = new Texture2D(x, y, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;
		return newTex;
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

	/// <summary>
	/// Update the value of a cached value, and set bool flag to true if the cache was different.
	/// </summary>
	/// <param name="cache">Cache.</param>
	/// <param name="val">Value.</param>
	/// <param name="flag">Flag.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void updateCacheAndFlag<T>(ref T cache, T val, ref bool flag) {
		if (!EqualityComparer<T>.Default.Equals(cache, val)) {
			cache = val;
			flag = true;
		}
	}

	/// <summary>
	/// Cache the values of a list, and set bool flag to true if the cache was different.
	/// </summary>
	/// <param name="cache">Cache.</param>
	/// <param name="val">Value.</param>
	/// <param name="flag">Flag.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void updateCacheAndFlagList<T>(ref List<T> cache, List<T> val, ref bool flag) {
		if (cache.Count != val.Count) {
			cache = new List<T>(val);
			flag = true;
		}
		else {
			for (int i = 0; i < val.Count; i++) {
				if (!EqualityComparer<T>.Default.Equals(val[i], cache[i])) {
					cache = new List<T>(val);
					flag = true;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Swaps one value with another value.
	/// </summary>
	/// <param name="val1">Val1.</param>
	/// <param name="val2">Val2.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SwapVals<T>(ref T val1, ref T val2) {
		T tmp = val1;
		val1 = val2;
		val2 = tmp;
	}

	/// <summary>
	/// Swaps one list with another list.
	/// </summary>
	/// <param name="val1">Val1.</param>
	/// <param name="val2">Val2.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void SwapValsList<T>(ref List<T> val1, ref List<T> val2) {
		List<T> tmp = new List<T>(val1);
		val1 = val2;
		val2 = tmp;
	}

	/// <summary>
	/// Generate data of the form Y = aX + b
	/// </summary>
	/// <returns>The linear.</returns>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	public static List<Vector2> GenLinear(int numPoints, float minX, float maxX, float a, float b) {
		List<Vector2> results = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX) return results;
		float step = (maxX - minX) / (numPoints - 1);
		for (int i = 0; i < numPoints; i++) {
			float x = i*step + minX;
			results.Add(new Vector2(x, a*x + b));
		}
		return results;
	}
	
	/// <summary>
	/// Generate data of the form Y = aX^2 + bX + c
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="c">C.</param>
	public static List<Vector2> GenQuadratic(int numPoints, float minX, float maxX, float a, float b, float c) {
		List<Vector2> results = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX) return results;
		float step = (maxX - minX) / (numPoints - 1);
		for (int i = 0; i < numPoints; i++) {
			float x = i*step + minX;
			results.Add(new Vector2(x, a*x*x + b*x + c));
		}
		return results;
	}
	
	/// <summary>
	/// Generate data of the form Y = ab^X + c
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="c">C.</param>
	public static List<Vector2> GenExponential(int numPoints, float minX, float maxX, float a, float b, float c) {
		List<Vector2> results = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX || b <= 0) return results;
		float step = (maxX - minX) / (numPoints - 1);
		for (int i = 0; i < numPoints; i++) {
			float x = i*step + minX;
			results.Add(new Vector2(x, a*Mathf.Pow(b,x) + c));
		}
		return results;
	}
	
	/// <summary>
	/// Generate data of the form Y = a * log base b of X + c
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="c">C.</param>
	public static List<Vector2> GenLogarithmic(int numPoints, float minX, float maxX, float a, float b, float c) {
		List<Vector2> results = new List<Vector2>();
		if (numPoints < 2 || maxX <= minX || b <= 0 || b == 1) return results;
		float step = (maxX - minX) / (numPoints - 1);
		for (int i = 0; i < numPoints; i++) {
			float x = i*step + minX;
			if (x <= 0) continue;
			results.Add(new Vector2(x, a*Mathf.Log(x,b) + c));
		}
		return results;
	}
	
	/// <summary>
	/// Generate data of the form c^2 = (X - a)^2 * (Y - b)^2. Can be used to generate shapes as well (e.g. 3 points for triangle).
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="c">C.</param>
	/// <param name="degreeOffset">Degree offset.</param>
	public static List<Vector2> GenCircular(int numPoints, float a, float b, float c, float degreeOffset = 0) {
		List<Vector2> results = new List<Vector2>();
		if (numPoints < 2) return results;
		float step = 360f / numPoints;
		for (int i = 0; i < numPoints; i++) {
			float deg = i*step + degreeOffset;
			float x = c * Mathf.Cos(Mathf.Deg2Rad*deg);
			float y = c * Mathf.Sin(Mathf.Deg2Rad*deg);
			results.Add(new Vector2(x + a, y + b));
		}
		return results;
	}
	
	/// <summary>
	/// Generates a list of Vector2 used in a Radar graph.
	/// </summary>
	/// <param name="data">Data.</param>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="degreeOffset">Degree offset.</param>
	public static List<Vector2> GenRadar(List<float> data, float a, float b, float degreeOffset) {
		List<Vector2> results = new List<Vector2>();
		if (data.Count < 2) return results;
		
		float step = 360f / data.Count;
		for (int i = 0; i < data.Count; i++) {
			float deg = i*step + degreeOffset;
			float x = data[i] * Mathf.Cos(Mathf.Deg2Rad*deg);
			float y = data[i] * Mathf.Sin(Mathf.Deg2Rad*deg);
			results.Add(new Vector2(x + a, y + b));
		}
		return results;
	}
	
	/// <summary>
	/// Generates a list of Vector2 where both x and y are random.
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="minY">Minimum y.</param>
	/// <param name="maxY">Max y.</param>
	public static List<Vector2> GenRandomXY(int numPoints, float minX, float maxX, float minY, float maxY) {
		List<Vector2> results = new List<Vector2>();
		if (maxY <= minY || maxX <= minX) return results;
		for (int i = 0; i < numPoints; i++) {
			results.Add(new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY)));
		}
		return results;
	}
	
	/// <summary>
	/// Generates a list of Vector2 where x increases uniformly and y is random.
	/// </summary>
	/// <param name="numPoints">Number points.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	/// <param name="minY">Minimum y.</param>
	/// <param name="maxY">Max y.</param>
	public static List<Vector2> GenRandomY(int numPoints, float minX, float maxX, float minY, float maxY) {
		List<Vector2> results = new List<Vector2>();
		if (maxY <= minY || maxX <= minX) return results;
		float step = (maxX - minX) / (numPoints - 1);
		for (int i = 0; i < numPoints; i++) {
			results.Add(new Vector2(i*step + minX, Random.Range(minY,maxY)));
		}
		return results;
	}
	
	/// <summary>
	/// Generates a list of random values.
	/// </summary>
	/// <returns>The random list.</returns>
	/// <param name="numPoints">Number points.</param>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
	public static List<float> GenRandomList(int numPoints, float min, float max) {
		List<float> results = new List<float>();
		if (max <= min) return results;
		for (int i = 0; i < numPoints; i++) {
			results.Add(Random.Range(min,max));
		}
		return results;
	}


	/// <summary>
	/// For filling texture based on a circular band (e.g. doughnut defined by inner and outer radii)
	/// </summary>
	/// <param name="colors">Colors.</param>
	/// <param name="maxSize">Max size.</param>
	/// <param name="inner">Inner.</param>
	/// <param name="outer">Outer.</param>
	/// <param name="antiAliasing">If set to <c>true</c> anti aliasing.</param>
	/// <param name="antiAliasingStrength">Anti aliasing strength.</param>
	/// <param name="orig">Original.</param>
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

	/// <summary>
	/// Returns orientation of point 3 relative to line segment formed by points 1 and 2. If positive then clockwise, if negative then counterclockwise.
	/// </summary>
	/// <returns>The inter area.</returns>
	/// <param name="p1x">P1x.</param>
	/// <param name="p1y">P1y.</param>
	/// <param name="p2x">P2x.</param>
	/// <param name="p2y">P2y.</param>
	/// <param name="p3x">P3x.</param>
	/// <param name="p3y">P3y.</param>
	static float PointInterArea(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y) {
		return (p2y - p1y) * (p3x - p2x) - (p2x - p1x) * (p3y - p2y);
	}

	/// <summary>
	/// Returns a string given an input string / input float and some formatting options.
	/// </summary>
	/// <returns>The value label.</returns>
	/// <param name="text">Text.</param>
	/// <param name="labelType">Label type.</param>
	/// <param name="value">Value.</param>
	/// <param name="percent">Percent.</param>
	/// <param name="numDecimals">Number decimals.</param>
	public static string FormatValueLabel(string text, WMG_Enums.labelTypes labelType, float value, float percent, int numDecimals) {
		string theText = text;
		float multiplier = Mathf.Pow(10f, numDecimals+2);
		
		if (labelType == WMG_Enums.labelTypes.None) {
			theText = "";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Percents) {
			theText += " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Values) {
			theText += " (" + Mathf.Round(value).ToString() + ")";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Values_Percents) {
			theText += " - " + Mathf.Round(value).ToString() + " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		else if (labelType == WMG_Enums.labelTypes.Values_Only) {
			theText = Mathf.Round(value).ToString();
		}
		else if (labelType == WMG_Enums.labelTypes.Percents_Only) {
			theText = (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%";
		}
		else if (labelType == WMG_Enums.labelTypes.Values_Percents) {
			theText = Mathf.Round(value).ToString() + " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		return theText;
	}

	/// <summary>
	/// Evaluate arbitrary expression of the form (y = ...) for a given input value of x (expression doesn't have to have an x).
	/// Input expression must be in reverse polish notation with each token of the expression put in a List<string>
	/// </summary>
	/// <returns>The result as the point (x, y).</returns>
	/// <param name="rpnString">Reverse Polish Notation (RPN) string.</param>
	/// <param name="x">The x coordinate.</param>
	public static Vector2 ExpressionEvaluator(List<string> rpnString, float x) {
		Stack<float> outputStack = new Stack<float> ();
		for (int i = 0; i < rpnString.Count; i++) {
			if (IsOperator(rpnString[i])) { // is an operator, pop values off stack, and evaluate those values using the operator
				if (OperatorIsUnary(rpnString[i])) { // unary
					float value = outputStack.Pop();
					if (rpnString[i] == "#") {
						value = -value;
					}
					else if (rpnString[i] == "sqrt") {
						value = Mathf.Sqrt(value);
					}
					else if (rpnString[i] == "abs") {
						value = Mathf.Abs(value);
					}
					else if (rpnString[i] == "sin") {
						value = Mathf.Sin(value);
					}
					else if (rpnString[i] == "cos") {
						value = Mathf.Cos(value);
					}
					else if (rpnString[i] == "tan") {
						value = Mathf.Tan(value);
					}
					else if (rpnString[i] == "log") {
						if (value <= 0) {
							return new Vector2(x, float.NaN);
						}
						value = Mathf.Log10(value);
					}
					else if (rpnString[i] == "ln") {
						if (value <= 0) {
							return new Vector2(x, float.NaN);
						}
						value = Mathf.Log(value);
					}
					outputStack.Push(value);
				}
				else { // binary
					float valueRight = outputStack.Pop();
					float valueLeft = outputStack.Pop();
					if (rpnString[i] == "+") {
						outputStack.Push(valueLeft + valueRight);
					}
					else if (rpnString[i] == "-") {
						outputStack.Push(valueLeft - valueRight);
					}
					else if (rpnString[i] == "*") {
						outputStack.Push(valueLeft * valueRight);
					}
					else if (rpnString[i] == "/") {
						if (valueRight == 0) {
							return new Vector2(x, float.NaN);
						}
						outputStack.Push(valueLeft / valueRight);
					}
					else if (rpnString[i] == "^") {
						outputStack.Push(Mathf.Pow(valueLeft, valueRight));
					}
				}
			}
			else { // is a value, or the variable x, push it onto the stack
				float value = float.NaN;
				if (rpnString[i] == "x") {
					value = x;
				} // check for math constants
				else if (rpnString[i] == "e") {
					value = (float)System.Math.E;
				}
				else if (rpnString[i] == "π" || rpnString[i] == "PI") {
					value = (float)System.Math.PI;
				} 
				else { // should be a number string now
					bool successfullyParsedToFloat = float.TryParse(rpnString[i], out value);
					if (!successfullyParsedToFloat) {
						Debug.LogError("Expected string that can parse to float, but found: " + rpnString[i]);
						return Vector2.zero;
					}
				}
				outputStack.Push(value);
			}
		}
		if (outputStack.Count > 0) {
			return new Vector2(x, outputStack.Peek());
		} else {
			Debug.LogError("Expected a result from expression evaluation, but found nothing");
			return Vector2.zero;
		}
	}
	
	/// <summary>
	/// Convert space delimited infix expression string to reverse polish notation
	/// </summary>
	/// <returns>The yard algorithm.</returns>
	/// <param name="input">Input.</param>
	public static List<string> ShuntingYardAlgorithm(string input) {
		List<string> result = new List<string> ();
		string[] expr = input.Split(' ');
		bool expectingOperator = false;
		Stack<string> operators = new Stack<string> ();
		for (int i = 0; i < expr.Length; i++) {
			/* Case 1: Token is an operator. */
			if (IsOperator(expr[i])) {
				string opToken = expr[i];
				/* Confirm we were expecting one. */
				if (!expectingOperator) {
					if (expr[i] == "-") {
						opToken = "#"; // change unary minus
					}
					if (!OperatorIsUnary(opToken)) {
						Debug.LogError("Unexpected operator: " + expr[i]);
					}
				}
				
				/* Next, move all operators off the stack until this operator has the
				* highest precedence of them all.
				*/
				while (operators.Count > 0 && IsOperator(operators.Peek()) &&
				       PrecedenceOf(opToken) >= PrecedenceOf(operators.Peek())) {
					result.Add(operators.Pop());
				}
				
				/* Add the operator to the operator stack. */
				operators.Push(opToken);
				
				/* We're no longer expecting an operator; we just found one. */
				expectingOperator = false;
			}
			
			/* Case 2: Found an open parenthesis. */
			else if (expr[i] == "(") {
				/* We had better not have been expecting an operator here! */
				if (expectingOperator) {
					Debug.LogError("Expected operator, found (.");
				}
				else {
					operators.Push(expr[i]);
				}
			}
			/* Case 3: Found a close parenthesis. */
			else if (expr[i] == ")") {
				/* This should appear after parsing a number, which would be when an
				* operator is expected.
				*/
				if (!expectingOperator)
					Debug.LogError("Expected value, found )");
				
				/* Keep moving tokens over from the operator stack to the result until
				* we find a matching open parenthesis.  If we run out of tokens, then
				* there must be a syntax error.
				*/
				while (operators.Count > 0 && operators.Peek() != "(") {
					result.Add(operators.Pop());
				}
				
				/* We've now moved everything, so check whether we found a matching open
				* parenthesis.
				*/
				if (operators.Count == 0)
					Debug.LogError("Imbalanced parentheses.");
				
				/* Remove the parenthesis we matched. */
				operators.Pop();
				
				/* We are now expecting an operator, since we just read a value. */
				expectingOperator = true;
			}
			/* Case 4: Found a number or variable. */
			else {
				/* If we're expecting an operator, we're very disappointed. */
				if (expectingOperator)
					Debug.LogError("Expecting operator, found " + expr[i]);
				
				/* Shift the number, then say we're looking for an operator. */
				result.Add(expr[i]);
				expectingOperator = true;
			}
		}
		
		/* At this point we've parsed everything.  We should be expecting an
		* operator, since the last thing we read should have been a value.
		*/
		if (!expectingOperator)
			Debug.LogError("Expected value, didn't find one.");
		
		/* Keep shifting all the operators back off the operator stack onto the end
		* of the input.  If we find an open parenthesis, then the input is
		* malformed.
		*/
		while (operators.Count > 0) {
			if (operators.Peek() == "(")
				Debug.LogError("Imbalanced parentheses.");
			result.Add(operators.Pop());
		}
		
		return result;
	}

	static bool IsOperator(string token) {
		return token == "#" || // internal use only, means unary minus (negate) instead of binary minus (subtract) 
			token == "+" ||
				token == "-" ||
				token == "*" ||
				token == "/" ||
				token == "^" ||
				token == "sqrt" ||
				token == "abs" ||
				token == "sin" ||
				token == "cos" ||
				token == "tan" ||
				token == "ln" ||
				token == "log";
	}
	
	static bool OperatorIsUnary(string token) {
		return token == "#" ||
			token == "sqrt" ||
				token == "abs" ||
				token == "sin" ||
				token == "cos" ||
				token == "tan" ||
				token == "ln" ||
				token == "log";
	}
	
	static int PrecedenceOf(string token) {
		if (token == "+" || token == "-") return 6;
		if (token == "*" || token == "/") return 5;
		if (token == "^" || token == "#" || token == "sqrt" || token == "abs" || token == "sin" 
		    || token == "cos" || token == "tan" || token == "ln" || token == "log") return 3;
		Debug.LogError("Unknown operator: " + token);
		return -1;
	}

}
