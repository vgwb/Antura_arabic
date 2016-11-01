using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using EA4S;
using EA4S.Db;

namespace EA4S.Test
{
    public class ArabicTester : MonoBehaviour
    {
        public GameObject ButtonPrefab;
        public GameObject WordsContainer;

        private TeacherAI teacherAI;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            InitUI();
        }

        void InitUI()
        {
            GameObject btnGO;

            //// Words
            foreach (Transform t in WordsContainer.transform) {
                Destroy(t.gameObject);
            }

            //btnGO = Instantiate(ButtonPrefab);
            //btnGO.transform.SetParent(WordsContainer.transform, false);
            //btnGO.GetComponentInChildren<Text>().text = "Stop Music";
            //btnGO.GetComponent<Button>().onClick.AddListener(StopMusic);

            foreach (WordData word in AppManager.Instance.DB.GetAllWordData()) {
                //Debug.Log(mus.ToString());
                btnGO = Instantiate(ButtonPrefab);
                btnGO.transform.SetParent(WordsContainer.transform, false);
                btnGO.GetComponentInChildren<Text>().text = word.Id;
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
            //AudioManager.I.PlayMusic(music);
        }
    }
}