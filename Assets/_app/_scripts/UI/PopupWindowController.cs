using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S
{
    public class PopupWindowController : MonoBehaviour
    {

        public static PopupWindowController I;

        public GameObject TitleGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;

        Action currentCallback;

        void Start() {
            I = this;
        }

        public void Init(string introText, string wordCode, string arabicWord) {
            Init(null, introText, wordCode, arabicWord);
        }

        public void Init(Action callback, string introText, string wordCode, string arabicWord) {
            currentCallback = callback;
            AudioManager.I.PlaySfx(Sfx.UIPopup);
            SetTitle(introText);
            SetWord(wordCode, arabicWord);
        }

        public void SetTitle(string text) {
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
        }

        public void SetWord(string wordCode, string arabicWord) {
            // here set both word and drawing 
            WordTextGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(arabicWord, false, false);
            DrawingImageGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);

        }

        public void OnPressButton() {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            gameObject.SetActive(false);
            currentCallback();
        }
    }
}