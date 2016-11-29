using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EA4S.HideAndSeek
{
    public class HideAndSeekTutorialManager : MonoBehaviour
    {

        void OnEnable()
        {
            HideAndSeekTreeController.onTreeTouched += MoveObject;
            HideAndSeekLetterController.onLetterTouched += CheckResult;

            SetupTutorial();

            phase = 0;
            
        }
        void OnDisable()
        {
            HideAndSeekTreeController.onTreeTouched -= MoveObject;
            HideAndSeekLetterController.onLetterTouched -= CheckResult;
        }

        void Update()
        {
            if (timeFinger > 0 && Time.time > timeFinger)
                ShowFinger();
        }

        private LL_LetterData GetCorrectAnswer()
        {
            return (LL_LetterData)currentQuestion.GetCorrectAnswers().ToList()[0];
        }

        void SetupTutorial()
        {
            currentQuestion = HideAndSeekConfiguration.Instance.Questions.GetNextQuestion();
            List<ILivingLetterData> letterList = new List<ILivingLetterData>();

            ILivingLetterData right = currentQuestion.GetCorrectAnswers().ToList()[0];
            ILivingLetterData wrong = currentQuestion.GetCorrectAnswers().ToList()[1];
            letterList.Add(right);
            letterList.Add(wrong);

            // Set the wrong answer
            ArrayLetters[0].transform.position = ArrayPlaceholder[0].transform.position;
            ArrayLetters[0].GetComponent<HideAndSeekLetterController>().id = 3;

            ArrayLetters[0].GetComponentInChildren<LetterObjectView>().Init(wrong);
            ArrayTrees[0].GetComponent<CapsuleCollider>().enabled = true;


            // Set the correct answer
            ArrayLetters[1].transform.position = ArrayPlaceholder[1].transform.position;
            ArrayLetters[1].GetComponent<HideAndSeekLetterController>().id = 5;

            ArrayLetters[1].GetComponentInChildren<LetterObjectView>().Init(right);

            StartCoroutine(WaitTutorial());
        }

        private IEnumerator WaitTutorial()
        {
            var winInitialDelay = 1f;
            yield return new WaitForSeconds(winInitialDelay);

            AudioManager.I.PlayLetter(GetCorrectAnswer().Id);

            buttonRepeater.SetActive(true);

            ShowFinger();
        }

        void ShowFinger()
        {
            Vector3 offset = new Vector3(0f, 3f, -1.5f);
            Vector3 offsetCentral = new Vector3(0f, 2.5f, -2f);
            Vector3 offsetFirst = new Vector3(0.5f, 2f, -2f);


            switch (phase)
            {
                case 0:
                    TutorialUI.ClickRepeat(ArrayTrees[0].transform.position + offsetFirst, animDuration, 1);
                    break;
                case 1:
                    TutorialUI.ClickRepeat(ArrayLetters[0].transform.position + offset, animDuration, 1);
                    break;
                case 2:
                    TutorialUI.ClickRepeat(ArrayTrees[1].transform.position + offsetCentral, animDuration, 1);
                    break;
                case 3:
                    TutorialUI.ClickRepeat(ArrayLetters[1].transform.position + offset, animDuration, 1);
                    break;

            }

            timeFinger = Time.time + animDuration + timeToWait;
        }

        void MoveObject(int id)
        {
            if (ArrayLetters.Length > 0)
            {
                script = ArrayLetters[GetIdFromPosition(id)].GetComponent<HideAndSeekLetterController>();
                script.MoveTutorial();
            }

            if(GetIdFromPosition(id) == 0)
            {
                ArrayTrees[0].GetComponent<CapsuleCollider>().enabled = false;
                phase = 1;
                TutorialUI.Clear(false);
                timeFinger = Time.time + animDuration + timeToWait;
            }
                

            if (GetIdFromPosition(id) == 1)
            {
                ArrayTrees[1].GetComponent<CapsuleCollider>().enabled = false;
                phase = 3;
                TutorialUI.Clear(false);
                timeFinger = Time.time + animDuration + timeToWait;
            }
                
        }

        void CheckResult(int id)
        {
            letterInAnimation = GetIdFromPosition(id);
            HideAndSeekLetterController script = ArrayLetters[letterInAnimation].GetComponent<HideAndSeekLetterController>();
            if (script.view.Data.Id == GetCorrectAnswer().Id)
            {
                script.resultAnimation(true);
                AudioManager.I.PlaySfx(Sfx.Win);
                game.Context.GetCheckmarkWidget().Show(true);
                StartCoroutine(GoToPlay());
                phase = -1;
                buttonRepeater.SetActive(false);
            }
            else
            {
                script.resultAnimation(false);
                ArrayTrees[1].GetComponent<CapsuleCollider>().enabled = true;
                phase = 2;
                TutorialUI.Clear(false);
                AudioManager.I.PlaySfx(Sfx.Lose);
                game.Context.GetCheckmarkWidget().Show(false);
                timeFinger = Time.time + animDuration + timeToWait;
            }

        }

        private IEnumerator GoToPlay()
        {
            var winInitialDelay = 3f;
            yield return new WaitForSeconds(winInitialDelay);

            foreach(GameObject x in ArrayLetters)
            {
                x.GetComponent<LetterObjectView>().Poof();
                AudioManager.I.PlaySfx(Sfx.Poof);
                x.SetActive(false);
            }

            var delay = 1f;
            yield return new WaitForSeconds(delay);

            game.SetCurrentState(game.PlayState);
            
        }

        int GetIdFromPosition(int index)
        {
            for (int i = 0; i < ArrayLetters.Length; ++i)
            {
                if (ArrayLetters[i].GetComponent<HideAndSeekLetterController>().id == index)
                    return i;
            }
            return -1;
        }

        public void RepeatAudio()
        {
            AudioManager.I.PlayLetter(GetCorrectAnswer().Id);
        }

        public GameObject[] ArrayTrees;
        public GameObject[] ArrayLetters;
        public Transform[] ArrayPlaceholder;

        private HideAndSeekLetterController script;

        private int letterInAnimation = -1;

        private IQuestionPack currentQuestion;

        public HideAndSeekGame game;

        int phase;

        public float timeToWait = 0f;//
        float timeFinger = -1f;

        float animDuration = 1f;//

        public GameObject buttonRepeater;
    }
}
