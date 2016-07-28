using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;
using DG.Tweening;

namespace EA4S
{
    public class WidgetPopupWindow : MonoBehaviour
    {

        public static WidgetPopupWindow I;
        public static bool IsShown { get; private set; }

        public GameObject Window;
        public GameObject TitleGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;

        Action currentCallback;
        Tween showTween;

        void Awake()
        {
            I = this;

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.5f).From()
                .SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnPlay(()=> this.gameObject.SetActive(true))
                .OnRewind(()=> this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public static void Show(bool _doShow)
        {
            GlobalUI.Init();

            IsShown = _doShow;
            if (_doShow) I.showTween.PlayForward();
            else I.showTween.PlayBackwards();
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
//            Window.SetActive(true);
        }

        public void Close() {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
//            Window.SetActive(false);
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