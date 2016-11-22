using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using DG.Tweening;

namespace EA4S
{
    public class AssessmentManager : MonoBehaviour
    {
        [Header("References")]
        public GameObject PanelTestGO;

        public List<Color> Colors;
        public List<ColorSet> AvailableColors = new List<ColorSet>();

        public List<AssessmentObject> Draws;
        public List<AssessmentObject> Words;
        public List<SpriteLineRenderer> Lines;

        int currentResult;

        public struct ColorSet
        {
            public Color Color;
            public bool Available;
        }

        void Start()
        {
            currentResult = 0;

            PanelTestGO.SetActive(false);
            AppManager.Instance.InitDataAI();

            SceneTransitioner.Close();
            WidgetSubtitles.I.DisplaySentence("assessment_start_A1", 2, true, NextSentence);
            // StartTest();
        }

        #region Tutorial

        public void NextSentence()
        {
            WidgetSubtitles.I.DisplaySentence("assessment_start_A2", 3, true, NextSentence2);
        }

        public void NextSentence2()
        {
            WidgetSubtitles.I.DisplaySentence("assessment_start_A3", 3, true, ReadyToTest);
        }

        public void ReadyToTest()
        {
            ContinueScreen.Show(StartTest, ContinueScreenMode.Button);
        }

        #endregion

        void StartTest()
        {
            WidgetSubtitles.I.Close();
            Colors.Shuffle();
            int counter = 0;
            foreach (Color c in Colors) {
                AvailableColors.Add(new ColorSet() { Color = c, Available = true });
                Lines[counter].SetColor(c);
                counter++;
            }
            string serializedWordsForLog = string.Empty;
            List<ILivingLetterData> newDatas = new List<ILivingLetterData>(); // list to be shuffled
            for (int i = 0; i < Draws.Count; i++) {
                ILivingLetterData newData = AppManager.Instance.Teacher.GetRandomTestWordDataLL();
                if (newDatas.Contains(newData)) {
                    i--;
                    continue;
                }
                newDatas.Add(newData);
                Draws[i].Init(newData, false);
                Draws[i].InjectManager(this);
                serializedWordsForLog += string.Format(" - {0}", newData.Id);
            }
            newDatas.Shuffle();
            for (int i = 0; i < newDatas.Count; i++) {
                Words[i].Init(newDatas[i], true);
                Words[i].InjectManager(this);
            }
            LoggerEA4S.Log("app", "assessment", "start", serializedWordsForLog);
            PanelTestGO.SetActive(true);
        }

        /// <summary>
        /// Check if assessment is completed.
        /// </summary>
        /// <param name="_objDrag"></param>
        /// <param name="_objDrop"></param>
        public void OnReleaseOnWord(AssessmentObject _objDrag, AssessmentObject _objDrop)
        {

            if (Draws.FindAll(o => !o.IsLocked).Count == 0 || Words.FindAll(o => !o.IsLocked).Count == 0) {
                CalculateResult();

                foreach (var obj in Draws) {
                    obj.IsInteractable = false;
                }
                foreach (var obj in Words) {
                    obj.IsInteractable = false;
                }
            }
        }

        void CalculateResult()
        {
            int rightCounter = 0;
            foreach (var c in Colors) {
                ILivingLetterData d, w;
                d = Draws.Find(o => o.Color == c).data;
                w = Words.Find(o => o.Color == c).data;
                if (d.Id == w.Id) {
                    rightCounter++;
                    Words.Find(o => o.Color == c).ShowResult(true);
                } else {
                    Words.Find(o => o.Color == c).ShowResult(false);
                }
            }
            currentResult = rightCounter;

            LoggerEA4S.Log("app", "assessment", "result", rightCounter.ToString());
            // MiniGameDone refactoring
            // AppManager.Instance.MiniGameDone("assessment");

            WidgetSubtitles.I.DisplaySentence("assessment_result_intro", 3, true, ShowResults);
            //Debug.LogFormat("Result : {0}/{1}", rightCounter, Draws.Count);
        }

        public void ShowResults()
        {
            //WidgetPopupWindow.I.ShowTextDirect(AllFinished, string.Format("Result : {0}/{1}", currentResult, Draws.Count));

            //            var sentenceId = "assessment_result";
            //            var row = LocalizationData.Instance.GetRow(sentenceId);
            //            var arabicText = string.Format("{0} : {1}/{2}", row.GetStringData("Arabic"), currentResult, Draws.Count);
            //
            //           WidgetPopupWindow.I.ShowArabicTextDirect(AllFinished, arabicText);

            if (currentResult >= 5) {
                WidgetSubtitles.I.DisplaySentence("assessment_result_verygood", 3, true);
            } else if (currentResult >= 3) {
                WidgetSubtitles.I.DisplaySentence("assessment_result_good", 3, true);
            } else {
                WidgetSubtitles.I.DisplaySentence("assessment_result_retry", 3, true);
            }

            ContinueScreen.Show(AllFinished, ContinueScreenMode.Button);
        }

        public void AllFinished()
        {
            WidgetSubtitles.I.Close();
            WidgetPopupWindow.I.Close();
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Rewards");
        }

        public void UnlockObjects(Color _color)
        {
            foreach (var obj in Draws.FindAll(o => o.Color == _color)) {
                obj.IsLocked = false;
                obj.HideCircle(0.5f);
            }
            foreach (var obj in Words.FindAll(o => o.Color == _color)) {
                obj.IsLocked = false;
                obj.HideCircle(0.5f);
            }
            ReleaseColor(_color);
        }

        public SpriteLineRenderer GetLine(Color _color)
        {
            SpriteLineRenderer returnLine = Lines.Find(l => l.Color == _color);
            returnLine.GetComponent<Image>().DOFade(1, 0.3f);
            return returnLine;
        }

        #region colors

        public Color GetAvailableColor()
        {
            AvailableColors.Shuffle();
            int index = AvailableColors.FindIndex(c => c.Available == true);
            AvailableColors[index] = new ColorSet() { Color = AvailableColors[index].Color, Available = false };
            return AvailableColors[index].Color;
        }

        public void ReleaseColor(Color _color)
        {
            int index = AvailableColors.FindIndex(c => c.Color == _color);
            AvailableColors[index] = new ColorSet() { Color = AvailableColors[index].Color, Available = true };
            Lines.Find(l => l.Color == _color).GetComponent<Image>().DOFade(0, 0.3f);
        }

        #endregion

    }

}