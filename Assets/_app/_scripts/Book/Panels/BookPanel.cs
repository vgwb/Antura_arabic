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
        public GameObject MinigameItemPrefab;
        public GameObject PhraseItemPrefab;
        public GameObject CategoryItemPrefab;

        [Header("References")]
        public GameObject SubmenuContainer;
        public GameObject ElementsContainer;
        public TextRender ArabicText;
        public TextRender ScoreText;
        public TMPro.TextMeshProUGUI Drawing;

        public LetterObjectView LLText;
        public LetterObjectView LLDrawing;

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
            }
        }

        void activatePanel(PlayerBookPanel panel, bool status)
        {
            switch (panel) {
                case PlayerBookPanel.BookLetters:
                    AudioManager.I.PlayDialog("Book_Letters");
                    LettersPanel();
                    break;
                case PlayerBookPanel.BookWords:
                    AudioManager.I.PlayDialog("Book_Words");
                    WordsPanel();
                    break;
                case PlayerBookPanel.BookPhrases:
                    AudioManager.I.PlayDialog("Book_Phrases");
                    PhrasesPanel();
                    break;
            }
        }

        void LettersPanel(string _category = "")
        {
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
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "letter", Title = "Letters" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "symbol", Title = "Symbols" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = PlayerBookPanel.BookLetters, Id = "combo", Title = "Combinations" });

        }

        void WordsPanel(WordDataCategory _category = WordDataCategory.None)
        {
            currentWordCategory = _category;

            List<WordData> list;
            switch (currentWordCategory) {

                case WordDataCategory.None:
                    //list = AppManager.I.DB.GetAllWordData();
                    list = new List<WordData>();
                    break;
                default:
                    list = AppManager.I.DB.FindWordDataByCategory(currentWordCategory);
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
                btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = PlayerBookPanel.BookWords, wordCategory = cat, Title = cat.ToString() });
            }

        }

        void PhrasesPanel()
        {
            emptyListContainers();

            List<PhraseInfo> info_list = AppManager.I.Teacher.scoreHelper.GetAllPhraseInfo();
            foreach (var info_item in info_list) {
                btnGO = Instantiate(PhraseItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemPhrase>().Init(this, info_item);
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
            Debug.Log("Detail Word :" + info.data.Id);
            AudioManager.I.PlayWord(info.data.Id);

            ScoreText.text = "Score: " + info.score;

            var output = "";

            var splittedLetters = ArabicAlphabetHelper.SplitWordIntoLetters(info.data.Arabic);
            foreach (var letter in splittedLetters) {
                output += letter.GetChar() + " ";
            }
            output += "\n";
            output += info.data.Arabic;

            output += "\n";

            foreach (var letter in splittedLetters) {
                output += letter.GetChar();
            }

            ArabicText.text = output;

            LLText.Init(new LL_WordData(info.data));

            if (info.data.Drawing != "") {
                var drawingChar = AppManager.I.Teacher.wordHelper.GetWordDrawing(info.data);
                Drawing.text = drawingChar;
                LLDrawing.Init(new LL_ImageData(info.data));
                Debug.Log("Drawing: " + info.data.Drawing + " / " + ArabicAlphabetHelper.GetLetterFromUnicode(info.data.Drawing));
            } else {
                Drawing.text = "";
                LLDrawing.Init(new LL_ImageData(info.data));
            }
        }

        void ResetLL()
        {

        }

        public void DetailLetter(LetterInfo info)
        {
            Debug.Log("Detail Letter :" + info.data.Id + " [" + info.data.GetAvailablePositions() + "]");
            AudioManager.I.PlayLetter(info.data.Id);

            ScoreText.text = "Score: " + info.score;

            ArabicText.text = info.data.GetChar(LetterPosition.Isolated);
            ArabicText.text += " " + info.data.GetChar(LetterPosition.Final);
            ArabicText.text += " " + info.data.GetChar(LetterPosition.Medial);
            ArabicText.text += " " + info.data.GetChar(LetterPosition.Initial);

            LLText.Init(new LL_LetterData(info.data));
        }

        public void DetailPhrase(PhraseInfo info)
        {
            Debug.Log("Detail Phrase :" + info.data.Id);
            AudioManager.I.PlayPhrase(info.data.Id);

            ScoreText.text = "Score: " + info.score;
        }


        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
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


    }
}