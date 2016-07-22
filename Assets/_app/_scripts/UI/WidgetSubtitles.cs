using UnityEngine;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;
using Google2u;

namespace EA4S
{
    public class WidgetSubtitles : MonoBehaviour
    {
        public static WidgetSubtitles I;

        public GameObject Background;
        public GameObject TextGO;

        TextMeshProUGUI TextUI;
        System.Action currentCallback;

        int index;

        void Awake() {
            I = this;
        }

        void Start() {
            TextUI = TextGO.GetComponent<TextMeshProUGUI>();
            DisplayText("");
        }

        void OnEnable() {

        }

        public void DisplaySentence(string SentenceId) {
            currentCallback = null;
            DisplayText(SentenceId);
        }

        public void DisplaySentence(string SentenceId, System.Action callback) {
            index = 0;
            currentCallback = callback;
            DisplayText(SentenceId);
        }

        public void DisplaySentence(string[] SentenceIdList, System.Action callback) {
            index = 0;
            currentCallback = callback;
            DisplayText(SentenceIdList[index]);
        }

        public void ShowNext() {
        
        }

        void DisplayText(string textID) {
            if (textID != "") {
                Background.SetActive(true);

                LocalizationDataRow row = LocalizationData.Instance.GetRow(textID);
                if (row == null) {
                    TextUI.text = textID;
                } else {
                    TextUI.text = ArabicFixer.Fix(row.GetStringData("Arabic"), false, false);
                }
            } else {
                TextUI.text = "";
                Background.SetActive(false);
            }

        }
    }
}