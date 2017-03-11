using UnityEngine;
using System.Collections.Generic;
using EA4S.Audio;
using EA4S.Core;
using EA4S.Database;
using EA4S.Helpers;
using EA4S.LivingLetters;
using EA4S.MinigamesAPI;
using EA4S.UI;
using EA4S.Utilities;

namespace EA4S.Book
{

    public struct GenericCategoryData
    {
        public VocabularyChapter area;
        public string Id;
        public string Title;
        public string TitleEn;
        public WordDataCategory wordCategory;
        public PhraseDataCategory phraseCategory;
    }

    public enum VocabularyChapter
    {
        None,
        Letters,
        Words,
        Phrases,
        LearningBlock
    }

    /// <summary>
    /// Displays information on all learning items the player has unlocked.
    /// </summary>
    public class VocabularyPanel : MonoBehaviour, IBookPanel
    {
        [Header("Prefabs")]
        public GameObject WordItemPrefab;
        public GameObject LetterItemPrefab;
        public GameObject PhraseItemPrefab;
        public GameObject CategoryItemPrefab;

        [Header("References")]
        public GameObject DetailPanel;
        public GameObject Submenu;
        public GameObject SubmenuContainer;
        public GameObject ListPanel;
        public GameObject ElementsContainer;

        public UIButton BtnLetters;
        public UIButton BtnWords;
        public UIButton BtnPhrases;

        public GameObject MoreInfoLetterPanel;
        public GameObject MoreInfoWordPanel;
        public TextRender ArabicText;

        public TextRender LetterTextIsolated;
        public TextRender LetterTextInitial;
        public TextRender LetterTextMedial;
        public TextRender LetterTextFinal;
        public TextRender WordDrawingText;

        public TextRender ScoreText;

        VocabularyChapter currentChapter = VocabularyChapter.None;
        GameObject btnGO;
        string currentCategory;
        LocalizationData CategoryData;
        WordDataCategory currentWordCategory;
        PhraseDataCategory currentPhraseCategory;
        LetterInfo currentLetter;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(VocabularyChapter.Letters);
        }

        void OpenArea(VocabularyChapter newArea)
        {
            if (newArea != currentChapter) {
                currentChapter = newArea;
                activatePanel(currentChapter, true);
                ResetMenuButtons();
            }
        }

        void activatePanel(VocabularyChapter panel, bool status)
        {
            DetailPanel.SetActive(false);
            switch (panel) {
                case VocabularyChapter.Letters:
                    AudioManager.I.PlayDialogue(LocalizationDataId.UI_Letters);
                    LettersPanel("letters");
                    break;
                case VocabularyChapter.Words:
                    AudioManager.I.PlayDialogue(LocalizationDataId.UI_Words);
                    WordsPanel(WordDataCategory.Adjectives);
                    break;
                case VocabularyChapter.Phrases:
                    AudioManager.I.PlayDialogue(LocalizationDataId.UI_Phrases);
                    PhrasesPanel(PhraseDataCategory.Expression);
                    break;
            }
        }

        void ResetMenuButtons()
        {
            BtnLetters.Lock(currentChapter == VocabularyChapter.Letters);
            BtnWords.Lock(currentChapter == VocabularyChapter.Words);
            BtnPhrases.Lock(currentChapter == VocabularyChapter.Phrases);
        }

        #region Letters

        void LettersPanel(string _category = "")
        {
            ListPanel.SetActive(true);
            Submenu.SetActive(true);

            currentCategory = _category;
            List<LetterData> list;
            switch (currentCategory) {
                case "combinations":
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation));
                    break;
                case "symbols":
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Symbol));
                    break;
                default:
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Letter));
                    break;
            }
            emptyListContainers();

            List<LetterInfo> info_list = AppManager.I.ScoreHelper.GetAllLetterInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(LetterItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemLetter>().Init(this, info_item, false);
                }
            }

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new CategoryData { Id = "all", Title = "All" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Letters);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData {
                    area = VocabularyChapter.Letters,
                    Id = "letters",
                    Title = CategoryData.Arabic,
                    TitleEn = CategoryData.English
                },
                currentCategory == "letters"
            );

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Symbols);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData {
                    area = VocabularyChapter.Letters,
                    Id = "symbols",
                    Title = CategoryData.Arabic,
                    TitleEn = CategoryData.English
                },
                currentCategory == "symbols"
            );

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            CategoryData = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Combinations);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData {
                    area = VocabularyChapter.Letters,
                    Id = "combinations",
                    Title = CategoryData.Arabic,
                    TitleEn = CategoryData.English
                },
                currentCategory == "combinations"
            );

            //HighlightMenutCategory(currentCategory);
            // HighlightLetterItem("");
        }

        public void DetailLetter(LetterInfo _currentLetter)
        {
            currentLetter = _currentLetter;
            HighlightLetterItem(currentLetter.data.Id);

            DetailPanel.SetActive(true);
            MoreInfoLetterPanel.SetActive(true);
            MoreInfoWordPanel.SetActive(false);

            string positionsString = "";
            foreach (var p in currentLetter.data.GetAvailableForms()) {
                positionsString = positionsString + " " + p;
            }
            Debug.Log("Detail Letter :" + currentLetter.data.Id + " [" + positionsString + " ]");
            AudioManager.I.PlayLetter(currentLetter.data);

            ArabicText.text = "";
            // ScoreText.text = "Score: " + info.score;

            var isolatedChar = currentLetter.data.GetCharFixedForDisplay(LetterForm.Isolated);
            var InitialChar = currentLetter.data.GetCharFixedForDisplay(LetterForm.Initial);
            var MedialChar = currentLetter.data.GetCharFixedForDisplay(LetterForm.Medial);
            var FinalChar = currentLetter.data.GetCharFixedForDisplay(LetterForm.Final);

            LetterTextIsolated.SetTextUnfiltered(isolatedChar);
            LetterTextInitial.SetTextUnfiltered(InitialChar);
            LetterTextMedial.SetTextUnfiltered(MedialChar);
            LetterTextFinal.SetTextUnfiltered(FinalChar);

            LetterTextInitial.gameObject.SetActive(InitialChar != isolatedChar);
            LetterTextMedial.gameObject.SetActive(MedialChar != isolatedChar);
            LetterTextFinal.gameObject.SetActive(FinalChar != isolatedChar);
        }

        #endregion

        #region Words

        void WordsPanel(WordDataCategory _category = WordDataCategory.None)
        {
            ListPanel.SetActive(true);
            Submenu.SetActive(true);
            currentWordCategory = _category;

            Debug.Log("current wor cat: " + _category);


            List<WordData> list;
            switch (currentWordCategory) {

                case WordDataCategory.None:
                    //list = AppManager.I.DB.GetAllWordData();
                    list = new List<WordData>();
                    break;
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
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemWord>().Init(this, info_item);
                }
            }

            foreach (WordDataCategory cat in GenericHelper.SortEnums<WordDataCategory>()) {
                if (cat == WordDataCategory.None) continue;
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                CategoryData = LocalizationManager.GetWordCategoryData(cat);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData {
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

        public void DetailWord(WordInfo info)
        {
            DetailPanel.SetActive(true);
            MoreInfoLetterPanel.SetActive(false);
            MoreInfoWordPanel.SetActive(true);
            Debug.Log("Detail Word :" + info.data.Id);
            AudioManager.I.PlayWord(info.data);

            // ScoreText.text = "Score: " + info.score;

            var output = "";
            output += info.data.Arabic;
            output += "\n";
            var splittedLetters = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB, info.data);
            foreach (var letter in splittedLetters) {
                output += letter.letter.GetChar() + " ";
            }

            ArabicText.text = output;
            if (info.data.Drawing != "") {
                WordDrawingText.text = AppManager.I.VocabularyHelper.GetWordDrawing(info.data);
                if (info.data.Category == Database.WordDataCategory.Color) {
                    WordDrawingText.SetColor(GenericHelper.GetColorFromString(info.data.Value));
                }
            } else {
                WordDrawingText.text = "";
            }

        }

        #endregion

        #region Phrases

        void PhrasesPanel(PhraseDataCategory _category = PhraseDataCategory.None)
        {
            ListPanel.SetActive(true);
            Submenu.SetActive(true);
            currentPhraseCategory = _category;

            List<PhraseData> list;
            switch (currentPhraseCategory) {

                case PhraseDataCategory.None:
                    list = new List<PhraseData>();
                    break;
                default:
                    list = AppManager.I.DB.FindPhraseData((x) => (x.Category == currentPhraseCategory));
                    break;
            }
            emptyListContainers();

            List<PhraseInfo> info_list = AppManager.I.ScoreHelper.GetAllPhraseInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(PhraseItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemPhrase>().Init(this, info_item);
                }
            }

            foreach (PhraseDataCategory cat in GenericHelper.SortEnums<PhraseDataCategory>()) {
                if (cat == PhraseDataCategory.None) continue;
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                CategoryData = LocalizationManager.GetPhraseCategoryData(cat);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData {
                        area = VocabularyChapter.Phrases,
                        phraseCategory = cat,
                        Id = cat.ToString(),
                        Title = CategoryData.Arabic,
                        TitleEn = CategoryData.English
                    },
                    currentPhraseCategory == cat
                );

            }
        }

        public void DetailPhrase(PhraseInfo info)
        {
            DetailPanel.SetActive(true);
            MoreInfoLetterPanel.SetActive(false);
            MoreInfoWordPanel.SetActive(false);

            Debug.Log("Detail Phrase :" + info.data.Id);
            AudioManager.I.PlayPhrase(info.data);
            //ScoreText.text = "Score: " + info.score;

            ArabicText.text = info.data.Arabic;
        }

        #endregion

        public void SelectSubCategory(GenericCategoryData _category)
        {
            switch (_category.area) {
                case VocabularyChapter.Letters:
                    LettersPanel(_category.Id);
                    break;
                case VocabularyChapter.Words:
                    WordsPanel(_category.wordCategory);
                    break;
                case VocabularyChapter.Phrases:
                    PhrasesPanel(_category.phraseCategory);
                    break;
            }
        }

        void HighlightLetterItem(string id)
        {
            foreach (Transform t in ElementsContainer.transform) {
                t.GetComponent<ItemLetter>().Select(id);
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
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }
            // reset vertical position
            ListPanel.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1.0f;

            foreach (Transform t in SubmenuContainer.transform) {
                Destroy(t.gameObject);
            }
        }

        public void BtnOpenLetters()
        {
            OpenArea(VocabularyChapter.Letters);
        }

        public void BtnOpenWords()
        {
            OpenArea(VocabularyChapter.Words);
        }

        public void BtnOpenPhrases()
        {
            OpenArea(VocabularyChapter.Phrases);
        }

        void ResetLL()
        {

        }
    }
}