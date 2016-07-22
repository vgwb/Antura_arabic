using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S
{
    public class WidgetPopupWindow : MonoBehaviour
    {

        public static WidgetPopupWindow I;

        public GameObject Window;
        public GameObject TitleGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;

        Action currentCallback;

        void Awake() {
            I = this;
        }

        public void Init(string introText, string wordCode, string arabicWord) {
            Init(null, introText, wordCode, arabicWord);
        }

        public void Init(Action callback, string introText, string wordCode, string arabicWord) {
            currentCallback = callback;
            ButtonGO.SetActive(callback != null);

            AudioManager.I.PlaySfx(Sfx.UIPopup);
            SetTitle(introText);
            SetWord(wordCode, arabicWord);
            Window.SetActive(true);
        }

        public void Close() {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            Window.SetActive(false);
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
            Close();
            currentCallback();
        }
    }
}