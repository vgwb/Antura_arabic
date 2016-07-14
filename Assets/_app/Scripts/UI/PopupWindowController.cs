using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S
{
    public class PopupWindowController : MonoBehaviour
    {

        public static PopupWindowController I;

        public GameObject Manager;

        public GameObject TitleGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;

        void Start() {
            I = this;
        }

        public void Init(string introText, string word) {
            SetTitle(introText);
            SetWord(word);
            
        }

        public void SetTitle(string text) {
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
        }

        public void SetWord(string word) {
            // here set both word and drawing 

            WordTextGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(word, false, false);

        }

        public void OnPressButton() {
            Manager.SendMessage("PopupPressedContinue");
            gameObject.SetActive(false);
        }
    }
}