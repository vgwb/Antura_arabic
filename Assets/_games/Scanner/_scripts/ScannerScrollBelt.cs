using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{
	public class ScannerScrollBelt : MonoBehaviour {

		const float BELT_FACTOR = -0.4f;

		private Renderer rend;

		void Start() {
			rend = GetComponent<Renderer>();
		}

		void Update() {
			float offset = Time.time * BELT_FACTOR * ScannerConfiguration.Instance.beltSpeed;
			rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		}

	}

}