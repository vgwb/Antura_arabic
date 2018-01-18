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

        public TextRender WordArabicText;
        public TextRender WordSpellingArabicText;
        public TextRender WordDrawingText;

        private WordInfo currentWordInfo;
        private WordData currentWordData;
        private WordDataCategory currentWordCategory;
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
                    btnGO.GetComponent<ItemWord>().Init(this, info_item, false);
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
            currentWordInfo = _currentWord;
            currentWordData = currentWordInfo.data;

            DetailPanel.SetActive(true);

            HighlightWordItem(currentWordInfo.data.Id);

            Debug.Log("Detail Word :" + currentWordInfo.data.Id);
            AudioManager.I.PlayWord(currentWordInfo.data);

            var spellingString = "";
            var splittedLetters = ArabicAlphabetHelper.SplitWord(AppManager.I.DB, currentWordInfo.data, false, true);
            foreach (var letter in splittedLetters) {
                spellingString += letter.letter.GetStringForDisplay() + " ";
            }

            WordArabicText.text = currentWordInfo.data.Arabic;
            WordSpellingArabicText.text = spellingString;

            if (currentWordInfo.data.Drawing != "") {
                WordDrawingText.text = AppManager.I.VocabularyHelper.GetWordDrawing(currentWordInfo.data);
                if (currentWordInfo.data.Category == WordDataCategory.Color) {
                    WordDrawingText.SetColor(GenericHelper.GetColorFromString(currentWordInfo.data.Value));
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

        void HighlightWordItem(string id)
        {
            foreach (Transform t in ListContainer.transform) {
                t.GetComponent<ItemWord>().Select(id);
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