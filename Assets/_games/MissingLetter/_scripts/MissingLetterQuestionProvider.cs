using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using System.Linq;

namespace EA4S.MissingLetter
{

    public class MissingLetterQuestionProvider : IQuestionProvider
    {

        List<MissingLetterQuestionPack> questions = new List<MissingLetterQuestionPack>();
        string description;
        int currentQuestion;
        RoundManager.RoundType mQuestionType = RoundManager.RoundType.WORD;
        
        public void SetType(RoundManager.RoundType type)
        {
            mQuestionType = type;
        }

        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        public string GetDescription()
        {
            return description;
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            if(mQuestionType == RoundManager.RoundType.WORD)
            {
                return MakeWordQuestion();
            }
            else
            {
                return MakeSentenceQuestion();
            }
        }

        IQuestionPack MakeWordQuestion()
        {
            List<ILivingLetterData> question = new List<ILivingLetterData>();
            LL_WordData word = AppManager.Instance.Teacher.GimmeAGoodWordData();

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            var Letters = ArabicAlphabetHelper.LetterDataListFromWord(word.Data.Arabic, AppManager.Instance.Letters);
            int index = UnityEngine.Random.Range(0, Letters.Count);

            //TODO 
            //WORD void sometimes ???????? fix
            if(Letters.Count > 0)
            {
                correctAnswers.Add(Letters[index]);

                string sQuestion = word.Data.Arabic;
                sQuestion = sQuestion.Remove(index,1);
                sQuestion = sQuestion.Insert(index, " ");
                word.Data.Arabic = sQuestion;
                question.Add(word);
            }
            else
            {
                question.Add(word);
                correctAnswers.Add(AppManager.Instance.Teacher.GimmeARandomLetter());
            }

            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            for(int i=0; i< Letters.Count; ++i)
            {
                if(i != index)
                {
                    wrongAnswers.Add(Letters[i]);
                }
            }
            while (wrongAnswers.Count < 8)
            {
                var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                if (!CheckIfContains(correctAnswers, letter) && !CheckIfContains(wrongAnswers, letter))
                {
                    wrongAnswers.Add(letter);
                }
            }

            IQuestionPack questionPack = new MissingLetterQuestionPack(question, correctAnswers, wrongAnswers);
            return questionPack;
        }

        //TODO make for sentences
        IQuestionPack MakeSentenceQuestion()
        {
            List<ILivingLetterData> question = new List<ILivingLetterData>();
            for (int i = 0; i < 5; ++i)
            {
                LL_WordData word = AppManager.Instance.Teacher.GimmeAGoodWordData();
                question.Add(word);
            }

            int index = UnityEngine.Random.Range(0, question.Count);
            LL_WordData result = (LL_WordData)question.ElementAt(index);
            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            correctAnswers.Add(result);

            ((LL_WordData)question[index]).Data.Arabic = "";

            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            while (wrongAnswers.Count < 8)
            {
                var letter = AppManager.Instance.Teacher.GimmeAGoodWordData();

                if (!CheckIfContains(correctAnswers, letter) && !CheckIfContains(wrongAnswers, letter))
                {
                    wrongAnswers.Add(letter);
                }
            }

            IQuestionPack questionPack = new MissingLetterQuestionPack(question, correctAnswers, wrongAnswers);
            return questionPack;
        }

    }
}