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

        [Header("References")]
        public GameObject Window;
        public GameObject TitleGO;
        public GameObject TitleEnglishGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;
        public GameObject TutorialImageGO;

        Action currentCallback;
        Tween showTween;

        void Awake()
        {
            I = this;

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.5f).From()
                .SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(() => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        public static void Show(bool _doShow)
        {
            GlobalUI.Init();

            IsShown = _doShow;
            if (_doShow)
                I.showTween.PlayForward();
            else
                I.showTween.PlayBackwards();
        }

        public void Init(string introText, string wordCode, string arabicWord)
        {
            Init(null, introText, wordCode, arabicWord);
        }

        public void InitTutorial(Action callback, Sprite tutorialImage)
        {
            currentCallback = callback;
            ButtonGO.SetActive(callback != null);
            TutorialImageGO.GetComponent<Image>().sprite = tutorialImage;
            TutorialImageGO.SetActive(true);
            SetTitle("");
            SetWord("", "");
            AudioManager.I.PlaySfx(Sfx.UIPopup);
        }

        public void Init(Action callback, string introText, string wordCode, string arabicWord)
        {
            currentCallback = callback;
            ButtonGO.SetActive(callback != null);
            TutorialImageGO.SetActive(false);
            AudioManager.I.PlaySfx(Sfx.UIPopup);
            SetTitle(introText);
            SetWord(wordCode, arabicWord);
//            Window.SetActive(true);
        }

        public void Close()
        {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
//            Window.SetActive(false);
        }

        public void SetTitle(string text)
        {
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void SetWord(string wordCode, string arabicWord)
        {
            if (wordCode != "") {
                WordTextGO.SetActive(true);
                DrawingImageGO.SetActive(true);
                // here set both word and drawing 
                WordTextGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(arabicWord, false, false);
                DrawingImageGO.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
            } else {
                WordTextGO.SetActive(false);
                DrawingImageGO.SetActive(false);
            }
        }

        public void OnPressButton()
        {
            Close();
            currentCallback();
        }
    }
}