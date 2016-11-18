using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;

namespace EA4S
{
    public class BookPanel : MonoBehaviour
    {

        [Header("References")]
        public GameObject ButtonPrefab;
        public GameObject WordsContainer;
        public TextRender ArabicText;
        public TMPro.TextMeshProUGUI Drawing;

        public LetterObjectView LLText;
        public LetterObjectView LLDrawing;

        void Start()
        {
            InitUI();
            Drawing.text = "";
        }

        void InitUI()
        {
            GameObject btnGO;

            //// Words
            foreach (Transform t in WordsContainer.transform) {
                Destroy(t.gameObject);
            }

            foreach (WordData word in AppManager.Instance.DB.GetAllWordData()) {
                btnGO = Instantiate(ButtonPrefab);
                btnGO.transform.SetParent(WordsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = word.Id;
                if (word.Drawing != "") {
                    btnGO.GetComponent<Image>().color = Color.green;
                }
                AddListenerWord(btnGO.GetComponent<Button>(), word);
            }
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

    }
}