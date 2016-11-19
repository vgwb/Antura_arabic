using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;

namespace EA4S
{
    public class BookPanel : MonoBehaviour
    {
        enum BookPanelArea
        {
            None,
            Letters,
            Words,
            Phrases,
            Minigames
        }

        [Header("Prefabs")]
        public GameObject WordItemPrefab;
        public GameObject LetterItemPrefab;
        public GameObject MinigameItemPrefab;
        public GameObject PhraseItemPrefab;

        [Header("References")]
        public GameObject SubmenuContainer;
        public GameObject ElementsContainer;
        public TextRender ArabicText;
        public TMPro.TextMeshProUGUI Drawing;

        public LetterObjectView LLText;
        public LetterObjectView LLDrawing;

        BookPanelArea currentArea = BookPanelArea.None;
        GameObject btnGO;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea(BookPanelArea.Words);
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
                    LettersPanel();
                    break;
                case BookPanelArea.Words:
                    WordsPanel();
                    break;
                case BookPanelArea.Phrases:
                    PhrasesPanel();
                    break;
                case BookPanelArea.Minigames:
                    MinigamesPanel();
                    break;
            }
        }

        void LettersPanel()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (LetterData item in AppManager.Instance.DB.GetAllLetterData()) {
                btnGO = Instantiate(LetterItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = item.Id;
            }
        }

        void PhrasesPanel()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (PhraseData item in AppManager.Instance.DB.GetAllPhraseData()) {
                btnGO = Instantiate(PhraseItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = item.Id;
            }
        }

        void MinigamesPanel()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (MiniGameData item in AppManager.Instance.DB.GetAllMiniGameData()) {
                btnGO = Instantiate(MinigameItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = item.GetId();
            }
        }

        void WordsPanel()
        {

            //// Words
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (WordData item in AppManager.Instance.DB.GetAllWordData()) {
                btnGO = Instantiate(WordItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = item.Id;
                if (item.Drawing != "") {
                    btnGO.GetComponent<Image>().color = Color.green;
                }
                AddListenerWord(btnGO.GetComponent<Button>(), item);
                //btnGO.GetComponent<Button>().onClick.AddListener(() => PlayWord(word));
            }

            Drawing.text = "";
        }

        void AddListenerWord(Button b, WordData word)
        {
            b.onClick.AddListener(() => PlayWord(word));
        }

        void PlayWord(WordData word)
        {
            Debug.Log("playing word :" + word.Id);
            AudioManager.I.PlayWord(word.Id);
            ArabicText.text = word.Arabic;

            LLText.Init(new LL_WordData(word.GetId(), word));
            //LLText.Label.text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(word.Arabic);

            if (word.Drawing != "") {
                var drawingChar = AppManager.Instance.Teacher.wordHelper.GetWordDrawing(word);
                Drawing.text = drawingChar;
                //LLDrawing.Lable.text = drawingChar;
                LLDrawing.Init(new LL_ImageData(word.GetId(), word));
                Debug.Log("Drawing: " + word.Drawing);
            } else {
                Drawing.text = "";
                LLDrawing.Label.text = "";
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