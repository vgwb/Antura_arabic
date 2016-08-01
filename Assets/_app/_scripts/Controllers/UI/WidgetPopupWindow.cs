using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;
using DG.Tweening;
using Google2u;

namespace EA4S
{
    public class WidgetPopupWindow : MonoBehaviour
    {

        public static WidgetPopupWindow I;

        public static bool IsShown { get; private set; }

        [Header("Options")]
        public bool timeIndependent = true;
        [Header("References")]
        public GameObject Window;
        public GameObject TitleGO;
        public GameObject TitleEnglishGO;
        public GameObject DrawingImageGO;
        public GameObject WordTextGO;
        public GameObject ButtonGO;
        public GameObject TutorialImageGO;
        public GameObject MarkOK;
        public GameObject MarkKO;

        Action currentCallback;
        Tween showTween;

        void Awake()
        {
            I = this;

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.5f).From().SetUpdate(timeIndependent)
                .SetEase(Ease.OutBack).SetAutoKill(false).Pause() 
                .OnPlay(() => this.gameObject.SetActive(true))
                .OnRewind(() => this.gameObject.SetActive(false));

            this.gameObject.SetActive(false);
        }

        void ResetContents()
        {
            TutorialImageGO.SetActive(false);
            SetTitle("");
            SetWord("", "");
            MarkOK.SetActive(false);
            MarkKO.SetActive(false);
        }

        public static void Close(bool _immediate = false)
        {
            if (IsShown || _immediate)
                Show(false, _immediate);
        }

        public static void Show(bool _doShow, bool _immediate = false)
        {
            GlobalUI.Init();

            IsShown = _doShow;
            if (_doShow) {
                if (_immediate)
                    I.showTween.Complete();
                else
                    I.showTween.PlayForward();
            } else {
                if (_immediate)
                    I.showTween.Rewind();
                else
                    I.showTween.PlayBackwards();
            }
        }


        public void ShowTextDirect(Action callback, string myText)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);

            TitleGO.GetComponent<TextMeshProUGUI>().text = myText;
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = "";

            Show(true);
        }

       
        public void ShowSentence(Action callback, string SentenceId)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);

            LocalizationDataRow row = LocalizationData.Instance.GetRow(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.GetStringData("Arabic"), false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.GetStringData("English");

            AudioManager.I.PlayDialog(SentenceId);

            Show(true);
        }

        public void ShowSentenceWithMark(Action callback, string SentenceId, bool result)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);

            MarkOK.SetActive(result);
            MarkKO.SetActive(!result);

            LocalizationDataRow row = LocalizationData.Instance.GetRow(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.GetStringData("Arabic"), false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.GetStringData("English");

            AudioManager.I.PlayDialog(SentenceId);

            Show(true);
        }

        public void ShowSentenceAndWord(Action callback, string SentenceId, WordData wordData)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);

            LocalizationDataRow row = LocalizationData.Instance.GetRow(SentenceId);
            TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(row.GetStringData("Arabic"), false, false);
            TitleEnglishGO.GetComponent<TextMeshProUGUI>().text = row.GetStringData("English");

            AudioManager.I.PlayDialog(SentenceId);

            SetWord(wordData.Key, wordData.Word);

            Show(true);
        }



        public void Init(string introText, string wordCode, string arabicWord)
        {
            Init(null, introText, wordCode, arabicWord);
        }

        public void ShowTutorial(Action callback, Sprite tutorialImage)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);
            TutorialImageGO.GetComponent<Image>().sprite = tutorialImage;
            TutorialImageGO.SetActive(true);

            AudioManager.I.PlaySfx(Sfx.UIPopup);
            Show(true);
        }

        public void Init(Action callback, string introText, string wordCode, string arabicWord)
        {
            ResetContents();

            currentCallback = callback;
            ButtonGO.SetActive(callback != null);
            TutorialImageGO.SetActive(false);

            SetTitle(introText);
            SetWord(wordCode, arabicWord);
//            Window.SetActive(true);
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
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            currentCallback();
        }
    }
}