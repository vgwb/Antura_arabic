using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;

namespace EA4S.DontWakeUp
{
    public class Destinationmarker : MonoBehaviour
    {

        public GameObject TextGO;
        TextMeshProUGUI TextWord;

        // Use this for initialization
        void Start() {
	
        }

        public void Init(string wordCode) {
            Debug.Log(wordCode + " " + GameDontWakeUp.Instance.currentWord._word);
            TextGO.GetComponent<TextMeshPro>().text = ArabicFixer.Fix(GameDontWakeUp.Instance.currentWord._word, false, false);
            //TextWord.text = "URRO";
        }

    }
}