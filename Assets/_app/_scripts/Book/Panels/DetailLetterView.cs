using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class DetailLetterView : MonoBehaviour
    {
        [Header("References")]
        public LetterAllForms MainLetterDisplay;
        public TextRender LetterScoreText;
        public GameObject DiacriticsContainer;
        public GameObject LettersContainer;

        [Header("Prefabs")]
        public GameObject DiacriticSymbolItemPrefab;

        private LetterInfo myLetterInfo;
        private LetterData myLetterData;
        private GameObject btnGO;

        public void Init(LetterInfo letterInfo)
        {
            myLetterInfo = letterInfo;
            myLetterData = letterInfo.data;

            HighlightLetterItem(myLetterInfo.data.Id);

            foreach (Transform t in DiacriticsContainer.transform) {
                Destroy(t.gameObject);
            }
            var letterbase = myLetterInfo.data.Id;
            var variationsletters = AppManager.I.DB.FindLetterData(
                (x) => (x.BaseLetter == letterbase && (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation))
            );

            var letterGO = Instantiate(DiacriticSymbolItemPrefab);
            letterGO.transform.SetParent(DiacriticsContainer.transform, false);
            letterGO.GetComponent<ItemDiacriticSymbol>().Init(this, myLetterInfo, true);

            List<LetterInfo> info_list = AppManager.I.ScoreHelper.GetAllLetterInfo();
            info_list.Sort((x, y) => x.data.Number.CompareTo(y.data.Number));
            foreach (var info_item in info_list) {
                if (variationsletters.Contains(info_item.data)) {
                    btnGO = Instantiate(DiacriticSymbolItemPrefab);
                    btnGO.transform.SetParent(DiacriticsContainer.transform, false);
                    //btnGO.transform.SetAsFirstSibling();
                    btnGO.GetComponent<ItemDiacriticSymbol>().Init(this, info_item, false);
                }
            }

            //foreach (var letter in variationsletters) {
            //    letterGO = Instantiate(DiacriticSymbolItemPrefab);
            //    letterGO.transform.SetParent(DiacriticsContainer.transform, false);
            //    letterGO.GetComponent<ItemDiacriticSymbol>().Init(this, letter);
            //}
            ShowLetter(myLetterInfo);
        }

        public void ShowLetter(LetterInfo letterInfo)
        {
            myLetterInfo = letterInfo;
            myLetterData = letterInfo.data;

            Debug.Log("ShowLetter " + myLetterData.Id);

            string positionsString = "";
            foreach (var p in letterInfo.data.GetAvailableForms()) {
                positionsString = positionsString + " " + p;
            }
            MainLetterDisplay.Init(myLetterInfo.data);
            LetterScoreText.text = "Score: " + myLetterInfo.score;

            HighlightDiacriticItem(myLetterData.Id);
            playSound();
        }

        void playSound()
        {
            if (myLetterData.Kind == LetterDataKind.DiacriticCombo) {
                AudioManager.I.PlayLetter(myLetterData, true, LetterDataSoundType.Phoneme);
            } else {
                AudioManager.I.PlayLetter(myLetterData, true, LetterDataSoundType.Name);
            }

            //AudioManager.I.GetAudioClip(myLetterInfo.data, LetterDataSoundType.Phoneme) != null);
            //AudioManager.I.PlayLetter(currentLetter.data, true, LetterDataSoundType.Phoneme);
        }

        public void ShowDiacriticCombo(LetterInfo newLetterInfo)
        {
            ShowLetter(newLetterInfo);
        }

        void HighlightLetterItem(string id)
        {
            foreach (Transform t in LettersContainer.transform) {
                t.GetComponent<ItemLetter>().Select(id);
            }
        }

        void HighlightDiacriticItem(string id)
        {
            foreach (Transform t in DiacriticsContainer.transform) {
                t.GetComponent<ItemDiacriticSymbol>().Select(id);
            }
        }
    }
}