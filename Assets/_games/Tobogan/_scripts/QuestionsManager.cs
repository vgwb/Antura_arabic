using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.Tobogan
{
    public class QuestionsManager
    {
        ToboganGame game;

        bool initialized = false;

        QuestionLivingLetter questionLivingLetter;
        List<QuestionLivingLetter> standbyLivingLetters = new List<QuestionLivingLetter>();
        public event Action<bool> onAnswered;

        public QuestionsManager(ToboganGame game)
        {
            this.game = game;
        }

        public void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                CreateQuestionLivingLetters();
            }
        }

        public void NewQuestion()
        {
            var nextQuestionPack = ToboganConfiguration.Instance.PipeQuestions.GetNextQuestion();

            UpdateQuestion(nextQuestionPack);
        }

        void UpdateQuestion(IQuestionPack questionPack)
        {
            ResetStandbyLetters();

            questionLivingLetter.SetQuestionText(questionPack.GetQuestion());
            questionLivingLetter.PlayIdleAnimation();

            ILivingLetterData corretcAnswer = null;

            foreach(ILivingLetterData correct in questionPack.GetCorrectAnswers())
            {
                corretcAnswer = correct;
            }

            game.pipesAnswerController.SetPipeAnswers(questionPack.GetWrongAnswers(), corretcAnswer);
        }

        void CreateQuestionLivingLetters()
        {
            questionLivingLetter = GetQuestionLivingLetter();

            questionLivingLetter.transform.localPosition = game.questionLivingLetterBox.letterEndPosition.localPosition;
            questionLivingLetter.transform.rotation = game.questionLivingLetterBox.letterEndPosition.rotation;

            standbyLivingLetters.Clear();
            Transform[] lettersPosition = game.questionLivingLetterBox.lettersPosition;

            QuestionLivingLetter startQuestionLetter = GetQuestionLivingLetter();

            startQuestionLetter.transform.localPosition = game.questionLivingLetterBox.letterStartPosition.localPosition;
            startQuestionLetter.transform.rotation = game.questionLivingLetterBox.letterStartPosition.rotation;

            standbyLivingLetters.Add(startQuestionLetter);

            for (int i = 0; i < lettersPosition.Length; i++)
            {
                QuestionLivingLetter questionLetter = GetQuestionLivingLetter();

                questionLetter.transform.localPosition = lettersPosition[i].localPosition;
                questionLetter.transform.rotation = lettersPosition[i].rotation;

                standbyLivingLetters.Add(questionLetter);
            }
        }

        QuestionLivingLetter GetQuestionLivingLetter()
        {
            QuestionLivingLetter newQuestionLivingLetter = GameObject.Instantiate(game.questionLivingLetterPrefab).GetComponent<QuestionLivingLetter>();
            newQuestionLivingLetter.transform.SetParent(game.questionLivingLetterBox.transform);
            return newQuestionLivingLetter;
        }

        void ResetStandbyLetters()
        {
            for (int i = 0; i < standbyLivingLetters.Count; i++)
            {
                standbyLivingLetters[i].ClearQuestionText();
                standbyLivingLetters[i].PlayIdleAnimation();
            }
        }
    }
}
