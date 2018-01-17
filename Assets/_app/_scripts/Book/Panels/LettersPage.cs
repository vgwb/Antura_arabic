using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class LettersPage : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject LetterItemPrefab;
        public GameObject DiacriticSymbolItemPrefab;

        [Header("References")]
        public GameObject DetailPanel;
        public GameObject ListPanel;
        public GameObject ListContainer;

        public LetterAllForms MainLetterDisplay;
        public GameObject DiacriticsContainer;

        private LetterInfo myLetterInfo;
        private LetterData myLetterData;
        private GameObject btnGO;

        #region Letters

        private void OnEnable()
        {
            LettersPanel();
        }

        void LettersPanel()
        {
            ListPanel.SetActive(true);
            DetailPanel.SetActive(false);

            //switch (currentCategory) {
            //    case "combinations":
            //        letters = AppManager.I.DB.FindLetterData((x) =>
            //            (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation));
            //        break;
            //    case "symbols":
            //        letters = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Symbol));

            //}
            List<LetterData> letters = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Letter));

            emptyListContainers();

            List<LetterInfo> info_list = AppManager.I.ScoreHelper.GetAllLetterInfo();
            info_list.Sort((x, y) => x.data.Number.CompareTo(y.data.Number));
            foreach (var info_item in info_list) {
                if (letters.Contains(info_item.data)) {
                    btnGO = Instantiate(LetterItemPrefab);
                    btnGO.transform.SetParent(ListContainer.transform, false);
                    btnGO.transform.SetAsFirstSibling();
                    btnGO.GetComponent<ItemLetter>().Init(this, info_item, false);
                }
            }

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new CategoryData { Id = "all", Title = "All" });

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Letters);
            //btnGO.GetComponent<MenuItemCategory>().Init(
            //    this,
            //    new GenericCategoryData
            //    {
            //        area = VocabularyChapter.Letters,
            //        Id = "letters",
            //        Title = CategoryData.Arabic,
            //        TitleEn = CategoryData.English
            //    },
            //    currentCategory == "letters"
            //);

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Symbols);
            //btnGO.GetComponent<MenuItemCategory>().Init(
            //    this,
            //    new GenericCategoryData
            //    {
            //        area = VocabularyChapter.Letters,
            //        Id = "symbols",
            //        Title = CategoryData.Arabic,
            //        TitleEn = CategoryData.English
            //    },
            //    currentCategory == "symbols"
            //);

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Combinations);
            //btnGO.GetComponent<MenuItemCategory>().Init(
            //    this,
            //    new GenericCategoryData
            //    {
            //        area = VocabularyChapter.Letters,
            //        Id = "combinations",
            //        Title = CategoryData.Arabic,
            //        TitleEn = CategoryData.English
            //    },
            //    currentCategory == "combinations"
            //);

            //HighlightMenutCategory(currentCategory);
            // HighlightLetterItem("");
        }

        #endregion
        public void DetailLetter(LetterInfo letterInfo)
        {
            DetailPanel.SetActive(true);
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
            MainLetterDisplay.Init(myLetterData);
            //LetterScoreText.text = "Score: " + myLetterInfo.score;

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
        }

        public void ShowDiacriticCombo(LetterInfo newLetterInfo)
        {
            ShowLetter(newLetterInfo);
        }

        void HighlightLetterItem(string id)
        {
            foreach (Transform t in ListContainer.transform) {
                t.GetComponent<ItemLetter>().Select(id);
            }
        }

        void HighlightDiacriticItem(string id)
        {
            foreach (Transform t in DiacriticsContainer.transform) {
                t.GetComponent<ItemDiacriticSymbol>().Select(id);
            }
        }

        void emptyListContainers()
        {
            foreach (Transform t in ListContainer.transform) {
                Destroy(t.gameObject);
            }
            // reset vertical position
            //ListPanel.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1.0f;
        }
    }
}