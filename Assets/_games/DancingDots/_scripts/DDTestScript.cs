using UnityEngine;
using System.Collections;
using TMPro;


namespace EA4S.DancingDots
{
	public class DDTestScript : MonoBehaviour {

		// Use this for initialization
		void Start () {
			GetComponent<TextMeshPro>().text = ArabicSupport.ArabicFixer.Fix("ضَ",true,true);
		}

		// Update is called once per frame
		void Update () {

		}
	}
}
