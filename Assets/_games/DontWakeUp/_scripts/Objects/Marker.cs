using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;

namespace EA4S.DontWakeUp
{
    public enum MarkerType {
        Start,
        Goal
    }

    public class Marker : MonoBehaviour
    {
        public MarkerType Type;
        public GameObject DrawingGO;
        public GameObject TextGO;
        TextMeshProUGUI TextWord;

        void Start() {
	
        }

        public void Init(string wordCode) {
            //Debug.Log(wordCode + " " + GameDontWakeUp.Instance.currentWord._word);
            switch (Type) {
                case MarkerType.Start:
                    GetComponent<BoxCollider>().enabled = false;
                    DrawingGO.SetActive(true);
                    DrawingGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
                    TextGO.SetActive(false);
                    break;
                case MarkerType.Goal:
                    GetComponent<BoxCollider>().enabled = true;
                    TextGO.SetActive(true);
                    TextGO.GetComponent<TextMeshPro>().text = ArabicFixer.Fix(GameDontWakeUp.Instance.currentWord._word, false, false);
                    DrawingGO.SetActive(false);
                    break;
            }

        }

    }
}