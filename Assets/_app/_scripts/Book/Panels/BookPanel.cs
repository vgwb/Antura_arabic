using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{

    public struct GenericCategoryData
    {
        public PlayerBookPanel area;
        public string Id;
        public string Title;
        public WordDataCategory wordCategory;
    }

    public class BookPanel : MonoBehaviour
    {

        [Header("Prefabs")]
        public GameObject WordItemPrefab;
        public GameObject LetterItemPrefab;
        public GameObject PhraseItemPrefab;
        public GameObject CategoryItemPrefab;
        public GameObject LearningBlockItemPrefab;

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
        public UIButton BtnLearningBlocks;

        public GameObject MoreInfoPanel;
        public TextRender ArabicText;

        public TextRender LetterTextIsolated;
        public TextRender LetterTextInitial;
        public TextRender LetterTextMedial;
        public TextRender LetterTextFinal;

        public TextRender ScoreText;
        public TMPro.TextMeshProUGUI Drawing;

        public LetterObjectView LL_Isolated;
        public LetterObjectView LL_Initial;
        public LetterObjectView LL_Medial;
        public LetterObjectView LL_Final;

        PlayerBookPanel currentArea = PlayerBookPanel.None;
        GameObject btnGO;
        string currentCategory;
        WordDataCategory currentWordCategory;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(PlayerBookPanel.BookLetters);
        }

        void OpenArea(PlayerBookPanel newArea)
        {
            if (newArea != currentArea) {
                currentArea = newArea;
                activatePanel(currentArea, true);
                ResetMenuButtons();
            }
        }

        void activatePanel(PlayerBookPanel panel, bool status)
        {
            DetailPanel.SetActive(false);
            switch (panel) {
                case PlayerBookPanel.BookLetters:
                    AudioManager.I.PlayDialog(LocalizationDataId.UI_Letters);
                    LettersPanel();
                    break;
                case PlayerBookPanel.BookWords:
                    AudioManager.I.PlayDialog(LocalizationDataId.UI_Words);
                    WordsPanel();
                    break;
                case PlayerBookPanel.BookPhrases:
                    AudioManager.I.PlayDialog(LocalizationDataId.UI_Phrases);
                    PhrasesPanel();
                    break;
                case PlayerBookPanel.BookLearningBlocks:
                    //AudioManager.I.PlayDialog(LocalizationDataId.UI);
                    LearningBlockPanel();
                    break;
            }
        }

        void ResetMenuButtons()
        {
            BtnLetters.Lock(currentArea == PlayerBookPanel.BookLetters);
            BtnWords.Lock(currentArea == PlayerBookPanel.BookWords);
            BtnPhrases.Lock(currentArea == PlayerBookPanel.BookPhrases);
            BtnLearningBlocks.Lock(currentArea == PlayerBookPanel.BookLearningBlocks);
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
                    list = AppManager.I.DB.GetAllLetterData();
                    break;
            }
            emptyListContainers();

            List<LetterInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllLetterInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(LetterItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemLetter>().Init(this, info_item);
                }
            }

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new CategoryData { Id = "all", Title = "All" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "letter", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Letters) });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "symbol", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Symbols) });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(
                this,
                new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "combo", Title = LocalizationManager.GetTranslation(LocalizationDataId.UI_Combinations) });

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
            Drawing.text = "";

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { Id = WordDataCategory.None.ToString(), Title = "All" });

            foreach (WordDataCategory cat in GenericUtilities.SortEnums<WordDataCategory>()) {
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData {
                        area = PlayerBookPanel.BookWords,
                        wordCategory = cat,
                        Id = cat.ToString(),
                        Title = LocalizationManager.GetWordCategoryTitle(cat)
                    });
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

        void LearningBlockPanel()
        {
            ListPanel.SetActive(false);
            Submenu.SetActive(false);
            ListWidePanel.SetActive(true);
            emptyListContainers();

            List<LearningBlockInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllLearningBlockInfo();
            foreach (var item_info in info_list) {
                btnGO = Instantiate(LearningBlockItemPrefab);
                btnGO.transform.SetParent(ElementsContainerWide.transform, false);
                btnGO.GetComponent<ItemLearningBlock>().Init(this, item_info);
            }
        }

        public void SelectSubCategory(GenericCategoryData _category)
        {
            switch (_category.area) {
                case PlayerBookPanel.BookLetters:
                    LettersPanel(_category.Id);
                    break;
                case PlayerBookPanel.BookWords:
                    WordsPanel(_category.wordCategory);
                    break;
            }
        }

        public void DetailWord(WordInfo info)
        {
            DetailPanel.SetActive(true);
            Debug.Log("Detail Word :" + info.data.Id);
            AudioManager.I.PlayWord(info.data.Id);
            MoreInfoPanel.SetActive(false);
            ScoreText.text = "Score: " + info.score;

            var output = "";

            var splittedLetters = ArabicAlphabetHelper.SplitWordIntoLetters(info.data.Arabic);
            foreach (var letter in splittedLetters) {
                output += letter.GetChar() + " ";
            }
            output += "\n";
            output += info.data.Arabic;

            //output += "\n";

            //foreach (var letter in splittedLetters) {
            //    output += letter.GetChar();
            //}

            ArabicText.text = output;

            LL_Isolated.Init(new LL_WordData(info.data));
            LL_Initial.gameObject.SetActive(false);
            LL_Final.gameObject.SetActive(false);

            if (info.data.Drawing != "") {
                var drawingChar = AppManager.I.Teacher.wordHelper.GetWordDrawing(info.data);
                Drawing.text = drawingChar;
                //LL_Medial.gameObject.SetActive(true);
                LL_Medial.Init(new LL_ImageData(info.data));
                Debug.Log("Drawing: " + info.data.Drawing + " / " + ArabicAlphabetHelper.GetLetterFromUnicode(info.data.Drawing));
            } else {
                Drawing.text = "";
                LL_Medial.gameObject.SetActive(false);
            }
        }

        public void DetailLetter(LetterInfo info)
        {
            DetailPanel.SetActive(true);
            Debug.Log("Detail Letter :" + info.data.Id + " [" + info.data.GetAvailablePositions() + "]");
            AudioManager.I.PlayLetter(info.data.Id);
            MoreInfoPanel.SetActive(true);
            ArabicText.text = "";
            ScoreText.text = "Score: " + info.score;

            var isolatedChar = info.data.GetCharFixedForDisplay(LetterPosition.Isolated);
            LL_Isolated.Init(new LL_LetterData(info.data));
            LL_Isolated.Label.text = isolatedChar;

            var InitialChar = info.data.GetCharFixedForDisplay(LetterPosition.Initial);
            if (InitialChar != "") {
                LL_Initial.gameObject.SetActive(true);
                LL_Initial.Init(new LL_LetterData(info.data));
                LL_Initial.Label.text = InitialChar;
            } else {
                LL_Initial.gameObject.SetActive(false);
            }

            var MedialChar = info.data.GetCharFixedForDisplay(LetterPosition.Medial);
            if (MedialChar != "") {
                LL_Medial.gameObject.SetActive(true);
                LL_Medial.Init(new LL_LetterData(info.data));
                LL_Medial.Label.text = MedialChar;
            } else {
                LL_Medial.gameObject.SetActive(false);
            }

            var FinalChar = info.data.GetCharFixedForDisplay(LetterPosition.Final);
            if (FinalChar != "") {
                LL_Final.gameObject.SetActive(true);
                LL_Final.Init(new LL_LetterData(info.data));
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
            AudioManager.I.PlayPhrase(info.data.Id);
            MoreInfoPanel.SetActive(false);
            ScoreText.text = "Score: " + info.score;

            ArabicText.text = info.data.Arabic;

            LL_Isolated.gameObject.SetActive(false);
            LL_Initial.gameObject.SetActive(false);
            LL_Medial.gameObject.SetActive(false);
            LL_Final.gameObject.SetActive(false);
        }

        public void DetailLearningBlock(LearningBlockInfo info)
        {
            DetailPanel.SetActive(true);
            AudioManager.I.PlayDialog(info.data.GetTitleSoundFilename());
            ScoreText.text = "Score: " + info.score;
            MoreInfoPanel.SetActive(false);

            ArabicText.text = info.data.Title_Ar;

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
            OpenArea(PlayerBookPanel.BookLetters);
        }

        public void BtnOpenWords()
        {
            OpenArea(PlayerBookPanel.BookWords);
        }

        public void BtnOpenPhrases()
        {
            OpenArea(PlayerBookPanel.BookPhrases);
        }

        public void BtnOpenLearningBlocks()
        {
            OpenArea(PlayerBookPanel.BookLearningBlocks);
        }

        void ResetLL()
        {

        }
    }
}