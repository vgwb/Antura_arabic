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

namespace EA4S.PlayerBook
{

    public struct GenericCategoryData
    {
        public VocabularyChapter area;
        public string Id;
        public string Title;
        public WordDataCategory wordCategory;
    }

    public enum VocabularyChapter
    {
        None,
        Letters,
        Words,
        Phrases
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
        public GameObject ListWidePanel;
        public GameObject ElementsContainerWide;

        public UIButton BtnLetters;
        public UIButton BtnWords;
        public UIButton BtnPhrases;

        public GameObject MoreInfoPanel;
        public TextRender ArabicText;

        public TextRender LetterTextIsolated;
        public TextRender LetterTextInitial;
        public TextRender LetterTextMedial;
        public TextRender LetterTextFinal;

        public TextRender ScoreText;

        public LetterObjectView LL_Isolated;
        public LetterObjectView LL_Initial;
        public LetterObjectView LL_Medial;
        public LetterObjectView LL_Final;

        VocabularyChapter currentChapter = VocabularyChapter.None;
        GameObject btnGO;
        string currentCategory;
        WordDataCategory currentWordCategory;
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
                    LettersPanel("letter");
                    break;
                case VocabularyChapter.Words:
                    AudioManager.I.PlayDialogue(LocalizationDataId.UI_Words);
                    WordsPanel();
                    break;
                case VocabularyChapter.Phrases:
                    AudioManager.I.PlayDialogue(LocalizationDataId.UI_Phrases);
                    PhrasesPanel();
                    break;
            }
        }

        void ResetMenuButtons()
        {
            BtnLetters.Lock(currentChapter == VocabularyChapter.Letters);
            BtnWords.Lock(currentChapter == VocabularyChapter.Words);
            BtnPhrases.Lock(currentChapter == VocabularyChapter.Phrases);
        }

        void LettersPanel(string _category = "")
        {
            ListPanel.SetActive(true);
            Submenu.SetActive(true);
            ListWidePanel.SetActive(false);

            currentCategory = _category;
            List<LetterData> list;
            switch (currentCategory) {
                case "combo":
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation));
                    break;
                case "symbol":
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Symbol));
                    break;
                default:
                    list = AppManager.I.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Letter));
                    break;
            }
            emptyListContainers();

            List<LetterInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllLetterInfo();
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
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = VocabularyChapter.Letters, Id = "letter", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Letters) },
                currentCategory == "letter"
            );

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = VocabularyChapter.Letters, Id = "symbol", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Symbols) },
                currentCategory == "symbol"
            );

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = VocabularyChapter.Letters, Id = "combo", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Combinations) },
                currentCategory == "combo"
            );

            //HighlightMenutCategory(currentCategory);
            // HighlightLetterItem("");
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


        void WordsPanel(WordDataCategory _category = WordDataCategory.None)
        {
            ListPanel.SetActive(true);
            Submenu.SetActive(true);
            ListWidePanel.SetActive(false);
            currentWordCategory = _category;

            List<WordData> list;
            switch (currentWordCategory) {

                case WordDataCategory.None:
                    //list = AppManager.I.DB.GetAllWordData();
                    list = new List<WordData>();
                    break;
                default:
                    list = AppManager.I.DB.FindWordData((x) => (x.Category == currentWordCategory && x.Article == WordDataArticle.None && x.Kind == WordDataKind.Noun));
                    break;
            }
            emptyListContainers();

            List<WordInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllWordInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(WordItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemWord>().Init(this, info_item);
                }
            }

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { Id = WordDataCategory.None.ToString(), Title = "All" });

            foreach (WordDataCategory cat in GenericHelper.SortEnums<WordDataCategory>()) {
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData {
                        area = VocabularyChapter.Words,
                        wordCategory = cat,
                        Id = cat.ToString(),
                        Title = LocalizationManager.GetWordCategoryTitle(cat)
                    },
                    currentWordCategory == cat
                );
            }
        }

        void PhrasesPanel()
        {
            ListPanel.SetActive(false);
            Submenu.SetActive(false);
            ListWidePanel.SetActive(true);
            emptyListContainers();

            List<PhraseInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllPhraseInfo();
            foreach (var info_item in info_list) {
                btnGO = Instantiate(PhraseItemPrefab);
                btnGO.transform.SetParent(ElementsContainerWide.transform, false);
                btnGO.GetComponent<ItemPhrase>().Init(this, info_item);
            }
        }

        public void SelectSubCategory(GenericCategoryData _category)
        {
            switch (_category.area) {
                case VocabularyChapter.Letters:
                    LettersPanel(_category.Id);
                    break;
                case VocabularyChapter.Words:
                    WordsPanel(_category.wordCategory);
                    break;
            }
        }

        public void DetailWord(WordInfo info)
        {
            DetailPanel.SetActive(true);
            Debug.Log("Detail Word :" + info.data.Id);
            AudioManager.I.PlayWord(info.data);
            MoreInfoPanel.SetActive(false);
            ScoreText.text = "Score: " + info.score;

            var output = "";

            var splittedLetters = ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB, info.data);
            foreach (var letter in splittedLetters) {
                output += letter.letter.GetChar() + " ";
            }
            output += "\n";
            output += info.data.Arabic;

            //output += "\n";

            //foreach (var letter in splittedLetters) {
            //    output += letter.GetChar();
            //}

            ArabicText.text = output;

            LL_Isolated.Initialize(new LL_WordData(info.data));
            LL_Initial.gameObject.SetActive(false);
            LL_Final.gameObject.SetActive(false);

            if (info.data.Drawing != "") {
                //var drawingChar = AppManager.I.VocabularyHelper.GetWordDrawing(info.data);
                //Drawing.text = drawingChar;
                //LL_Medial.gameObject.SetActive(true);
                LL_Medial.Initialize(new LL_ImageData(info.data));
                Debug.Log("Drawing: " + info.data.Drawing + " / " + ArabicAlphabetHelper.GetLetterFromUnicode(info.data.Drawing));
            } else {
                //Drawing.text = "";
                LL_Medial.gameObject.SetActive(false);
            }
        }

        public void DetailLetter(LetterInfo info)
        {
            currentLetter = info;
            HighlightLetterItem(info.data.Id);

            DetailPanel.SetActive(true);
            string positionsString = "";
            foreach (var p in info.data.GetAvailableForms()) {
                positionsString = positionsString + " " + p;
            }
            Debug.Log("Detail Letter :" + info.data.Id + " [" + positionsString + " ]");
            AudioManager.I.PlayLetter(info.data);
            MoreInfoPanel.SetActive(true);
            ArabicText.text = "";
            ScoreText.text = "Score: " + info.score;

            var isolatedChar = info.data.GetCharFixedForDisplay(LetterForm.Isolated);
            LL_Isolated.Initialize(new LL_LetterData(info.data));
            LL_Isolated.Label.text = isolatedChar;

            var InitialChar = info.data.GetCharFixedForDisplay(LetterForm.Initial);
            if (InitialChar != "") {
                LL_Initial.gameObject.SetActive(true);
                LL_Initial.Initialize(new LL_LetterData(info.data));
                LL_Initial.Label.text = InitialChar;
            } else {
                LL_Initial.gameObject.SetActive(false);
            }

            var MedialChar = info.data.GetCharFixedForDisplay(LetterForm.Medial);
            if (MedialChar != "") {
                LL_Medial.gameObject.SetActive(true);
                LL_Medial.Initialize(new LL_LetterData(info.data));
                LL_Medial.Label.text = MedialChar;
            } else {
                LL_Medial.gameObject.SetActive(false);
            }

            var FinalChar = info.data.GetCharFixedForDisplay(LetterForm.Final);
            if (FinalChar != "") {
                LL_Final.gameObject.SetActive(true);
                LL_Final.Initialize(new LL_LetterData(info.data));
                LL_Final.Label.text = FinalChar;
            } else {
                LL_Final.gameObject.SetActive(false);
            }

            LetterTextIsolated.SetTextUnfiltered(isolatedChar);
            LetterTextInitial.SetTextUnfiltered(InitialChar);
            LetterTextMedial.SetTextUnfiltered(MedialChar);
            LetterTextFinal.SetTextUnfiltered(FinalChar);
        }

        public void DetailPhrase(PhraseInfo info)
        {
            DetailPanel.SetActive(true);
            Debug.Log("Detail Phrase :" + info.data.Id);
            AudioManager.I.PlayPhrase(info.data);
            MoreInfoPanel.SetActive(false);
            ScoreText.text = "Score: " + info.score;

            ArabicText.text = info.data.Arabic;

            LL_Isolated.gameObject.SetActive(false);
            LL_Initial.gameObject.SetActive(false);
            LL_Medial.gameObject.SetActive(false);
            LL_Final.gameObject.SetActive(false);
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }
            foreach (Transform t in ElementsContainerWide.transform) {
                Destroy(t.gameObject);
            }
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