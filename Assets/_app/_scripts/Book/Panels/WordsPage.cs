using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.Helpers;
using Antura.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class WordsPage : MonoBehaviour, IBookPanel
    {
        [Header("Prefabs")]
        public GameObject WordItemPrefab;
        public GameObject CategoryItemPrefab;

        [Header("References")]
        public GameObject DetailPanel;
        public GameObject ListPanel;
        public GameObject ListContainer;
        public GameObject Submenu;
        public GameObject SubmenuContainer;

        public TextRender ArabicText;
        public TextRender WordDrawingText;

        private WordDataCategory currentWordCategory;
        private WordInfo currentWord;
        private string currentCategory;
        private LocalizationData CategoryData;
        private GameObject btnGO;

        private void OnEnable()
        {
            WordsPanel();
        }

        void WordsPanel(WordDataCategory _category = WordDataCategory.None)
        {
            ListPanel.SetActive(true);
            DetailPanel.SetActive(false);
            currentWordCategory = _category;

            Debug.Log("current word cat: " + _category);

            List<WordData> list;
            switch (currentWordCategory) {
                //case WordDataCategory.None:
                ////list = AppManager.I.DB.GetAllWordData();
                //list = new List<WordData>();
                //break;
                default:
                    //list = AppManager.I.DB.FindWordData((x) => (x.Category == currentWordCategory && x.Article == WordDataArticle.None && x.Kind == WordDataKind.Noun));
                    list = AppManager.I.DB.FindWordData((x) => (x.Category == currentWordCategory));
                    break;
            }
            emptyListContainers();

            List<WordInfo> info_list = AppManager.I.ScoreHelper.GetAllWordInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(WordItemPrefab);
                    btnGO.transform.SetParent(ListContainer.transform, false);
                    btnGO.GetComponent<ItemWord>().Init(this, info_item);
                }
            }

            foreach (WordDataCategory cat in GenericHelper.SortEnums<WordDataCategory>()) {
                //if (cat == WordDataCategory.None) continue;
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                CategoryData = LocalizationManager.GetWordCategoryData(cat);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData
                    {
                        area = VocabularyChapter.Words,
                        wordCategory = cat,
                        Id = cat.ToString(),
                        Title = CategoryData.Arabic,
                        TitleEn = CategoryData.English
                    },
                    currentWordCategory == cat
                );
            }
        }

        public void DetailWord(WordInfo _currentWord)
        {
            currentWord = _currentWord;
            DetailPanel.SetActive(true);

            Debug.Log("Detail Word :" + currentWord.data.Id);
            AudioManager.I.PlayWord(currentWord.data);

            var output = "";
            output += currentWord.data.Arabic;
            output += "\n";
            var splittedLetters = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB, currentWord.data);
            foreach (var letter in splittedLetters) {
                output += letter.letter.GetStringForDisplay() + " ";
            }

            ArabicText.text = output;
            if (currentWord.data.Drawing != "") {
                WordDrawingText.text = AppManager.I.VocabularyHelper.GetWordDrawing(currentWord.data);
                if (currentWord.data.Category == WordDataCategory.Color) {
                    WordDrawingText.SetColor(GenericHelper.GetColorFromString(currentWord.data.Value));
                }
            } else {
                WordDrawingText.text = "";
            }

            //ScoreText.text = "Score: " + currentWord.score;
        }


        public void SelectSubCategory(GenericCategoryData _category)
        {
            switch (_category.area) {
                case VocabularyChapter.Words:
                    WordsPanel(_category.wordCategory);
                    break;
            }
        }

        void HighlightMenutCategory(string id)
        {
            foreach (Transform t in SubmenuContainer.transform) {
                t.GetComponent<MenuItemCategory>().Select(id);
            }
        }

        void emptyListContainers()
        {
            foreach (Transform t in ListContainer.transform) {
                Destroy(t.gameObject);
            }
            // reset vertical position
            ListPanel.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1.0f;

            foreach (Transform t in SubmenuContainer.transform) {
                Destroy(t.gameObject);
            }
        }

    }
}