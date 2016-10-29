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
            TextGO.SetActive(false);
            DrawingGO.SetActive(false);
        }

        public void Init(string wordCode) {
            //Debug.Log(wordCode + " " + GameDontWakeUp.Instance.currentWord._word);
            switch (Type) {
                case MarkerType.Start:
                    GetComponent<BoxCollider>().enabled = false;
                    TextGO.SetActive(true);
                    TextGO.GetComponent<TextMeshPro>().text = ArabicFixer.Fix(DontWakeUpManager.Instance.currentWord.Data.Arabic, false, false);
                    DrawingGO.SetActive(false);
                    break;
                case MarkerType.Goal:
                    GetComponent<BoxCollider>().enabled = true;
                    DrawingGO.SetActive(true);
                    DrawingGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
                    TextGO.SetActive(false);
                    break;
            }

        }

    }
}