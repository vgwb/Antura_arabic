using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S
{
    public enum BookPanelArea
    {
        None,
        Letters,
        Words,
        Phrases,
        Minigames
    }

    public struct GenericCategoryData
    {
        public BookPanelArea area;
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
        public TMPro.TextMeshProUGUI Drawing;

        public LetterObjectView LLText;
        public LetterObjectView LLDrawing;

        BookPanelArea currentArea = BookPanelArea.None;
        GameObject btnGO;
        string currentCategory;
        WordDataCategory currentWordCategory;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(BookPanelArea.Letters);
        }

        void OpenArea(BookPanelArea newArea)
        {
            if (newArea != currentArea) {
                currentArea = newArea;
                activatePanel(currentArea, true);
            }
        }

        void activatePanel(BookPanelArea panel, bool status)
        {
            switch (panel) {
                case BookPanelArea.Letters:
                    AudioManager.I.PlayDialog("Book_Letters");
                    LettersPanel();
                    break;
                case BookPanelArea.Words:
                    AudioManager.I.PlayDialog("Book_Words");
                    WordsPanel();
                    break;
                case BookPanelArea.Phrases:
                    AudioManager.I.PlayDialog("Book_Phrases");
                    PhrasesPanel();
                    break;
                case BookPanelArea.Minigames:
                    AudioManager.I.PlayDialog("Book_Games");
                    MinigamesPanel();
                    break;
            }
        }

        void LettersPanel(string _category = "")
        {
            currentCategory = _category;
            List<LetterData> list;
            switch (currentCategory) {
                case "combo":
                    list = AppManager.Instance.DB.FindLetterData((x) => (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation));
                    break;
                case "symbol":
                    list = AppManager.Instance.DB.FindLetterData((x) => (x.Kind == LetterDataKind.Symbol));
                    break;
                default:
                    list = AppManager.Instance.DB.GetAllLetterData();
                    break;
            }

            emptyListContainers();
            foreach (LetterData item in list) {
                btnGO = Instantiate(LetterItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemLetter>().Init(this, item);
            }

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new CategoryData { Id = "all", Title = "All" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = BookPanelArea.Letters, Id = "letter", Title = "Letters" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = BookPanelArea.Letters, Id = "symbol", Title = "Symbols" });

            btnGO = Instantiate(CategoryItemPrefab);
            btnGO.transform.SetParent(SubmenuContainer.transform, false);
            btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = BookPanelArea.Letters, Id = "combo", Title = "Combinations" });

        }

        void WordsPanel(WordDataCategory _category = WordDataCategory.None)
        {
            currentWordCategory = _category;
            List<WordData> list;
            switch (currentWordCategory) {

                case WordDataCategory.None:
                    //list = AppManager.Instance.DB.GetAllWordData();
                    list = new List<WordData>();
                    break;
                default:
                    list = AppManager.Instance.DB.FindWordDataByCategory(currentWordCategory);
                    break;
            }
            emptyListContainers();

            foreach (WordData item in list) {
                btnGO = Instantiate(WordItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemWord>().Init(this, item);
            }
            Drawing.text = "";

            //btnGO = Instantiate(CategoryItemPrefab);
            //btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { Id = WordDataCategory.None.ToString(), Title = "All" });

            foreach (WordDataCategory cat in GenericUtilities.SortEnums<WordDataCategory>()) {
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                btnGO.GetComponent<MenuItemCategory>().Init(this, new GenericCategoryData { area = BookPanelArea.Words, wordCategory = cat, Title = cat.ToString() });
            }

        }

        void PhrasesPanel()
        {
            emptyListContainers();

            foreach (PhraseData item in AppManager.Instance.DB.GetAllPhraseData()) {
                btnGO = Instantiate(PhraseItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemPhrase>().Init(this, item);
            }
        }

        void MinigamesPanel()
        {
            emptyListContainers();

            foreach (MiniGameData item in AppManager.Instance.DB.GetActiveMinigames()) {
                btnGO = Instantiate(MinigameItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemMiniGame>().Init(this, item);
            }
        }

        public void SelectSubCategory(GenericCategoryData _category)
        {
            switch (_category.area) {
                case BookPanelArea.Letters:
                    LettersPanel(_category.Id);
                    break;
                case BookPanelArea.Words:
                    WordsPanel(_category.wordCategory);
                    break;
            }
        }

        public void DetailWord(WordData word)
        {
            Debug.Log("Detail Word :" + word.Id);
            AudioManager.I.PlayWord(word.Id);
            ArabicText.text = word.Arabic;

            LLText.Init(new LL_WordData(word));
            //LLText.Label.text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(word.Arabic);

            if (word.Drawing != "") {
                var drawingChar = AppManager.Instance.Teacher.wordHelper.GetWordDrawing(word);
                Drawing.text = drawingChar;
                //LLDrawing.Lable.text = drawingChar;
                LLDrawing.Init(new LL_ImageData(word));
                Debug.Log("Drawing: " + word.Drawing);
            } else {
                Drawing.text = "";
                LLDrawing.Init(new LL_ImageData(word));
            }
        }

        public void DetailLetter(LetterData letter)
        {
            Debug.Log("Detail Letter :" + letter.Id);
            AudioManager.I.PlayLetter(letter.Id);

            ArabicText.text = "iso: " + letter.GetChar(LetterPosition.Isolated);
            ArabicText.text += "\nfin: " + letter.GetChar(LetterPosition.Final);
            ArabicText.text += "\nmed: " + letter.GetChar(LetterPosition.Medial);
            ArabicText.text += "\nini: " + letter.GetChar(LetterPosition.Initial);

            LLText.Init(new LL_LetterData(letter));
        }

        public void DetailPhrase(PhraseData phrase)
        {
            Debug.Log("Detail Phrase :" + phrase.Id);
            AudioManager.I.PlayPhrase(phrase.Id);
        }

        public void DetailMiniGame(MiniGameData data)
        {
            AudioManager.I.PlayDialog(data.GetTitleSoundFilename());
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
            OpenArea(BookPanelArea.Letters);
        }

        public void BtnOpenWords()
        {
            OpenArea(BookPanelArea.Words);
        }

        public void BtnOpenPhrases()
        {
            OpenArea(BookPanelArea.Phrases);
        }

        public void BtnOpenMinigames()
        {
            OpenArea(BookPanelArea.Minigames);
        }
    }
}