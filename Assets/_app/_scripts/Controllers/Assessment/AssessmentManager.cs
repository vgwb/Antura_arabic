using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ModularFramework.Helpers;

namespace EA4S
{
    public class AssessmentManager : MonoBehaviour
    {

        public List<Color> Colors;

        public List<AssessmentObject> Draws;
        public List<AssessmentObject> Words;

        public AssessmentObject startObj, endObj;
        public PopupTmp Popup;

        void Start() {
            AppManager.Instance.InitDataAI();
            Colors.Shuffle();
            List<ILivingLetterData> newDatas = new List<ILivingLetterData>(); // list to be shuffled
            for (int i = 0; i < Draws.Count; i++) {
                ILivingLetterData newData = AppManager.Instance.Teacher.GimmeAGoodWordData();
                newDatas.Add(newData);
                Draws[i].Init(newData, false, Colors[i]);
                Draws[i].InjectManager(this);
            }
            newDatas.Shuffle();
            for (int i = 0; i < newDatas.Count; i++) {
                Words[i].Init(newDatas[i], true, Colors[i]);
                Words[i].InjectManager(this);
            }


            WidgetSubtitles.I.DisplaySentence("assessment_start_A1", 2, true);
        }

        public void OnReleaseOnWord(AssessmentObject _objDrag, AssessmentObject _objDrop) {
            if (Draws.FindAll(o => !o.IsLocked).Count == 0)
                CalculateResult();
        }

        void CalculateResult() {
            int rightCounter = 0;
            foreach (var c in Colors) {
                ILivingLetterData d, w;
                d = Draws.Find(o => o.Color == c).data;
                w = Words.Find(o => o.Color == c).data;
                if (d.Key == w.Key) {
                    rightCounter++;
                }
            }

            //Debug.LogFormat("Result : {0}/{1}", rightCounter, Draws.Count);
            Popup.Show(true, string.Format("Result : {0}/{1}", rightCounter, Draws.Count));
        }

    }

}