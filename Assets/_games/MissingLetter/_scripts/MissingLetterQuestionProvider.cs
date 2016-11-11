using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using System.Linq;

namespace EA4S.MissingLetter
{

    public class MissingLetterQuestionProvider : IQuestionProvider
    {

        static string msDescription = "Missing Letter Question Provider";
        int mcurrentQuestion;
        RoundType mQuestionType = RoundType.WORD;
        LL_WordData mDataToRestore;
        string mRemovedElement;
        
        public void SetType(RoundType type)
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
            return msDescription;
        }

        IQuestionPack IQuestionProvider.GetNextQuestion()
        {
            if(mQuestionType == RoundType.WORD)
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
            LL_WordData word = AppManager.Instance.Teacher.GimmeAGoodWordData();
            mDataToRestore = word;

            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            var Letters = ArabicAlphabetHelper.LetterDataListFromWord(word.Data.Arabic, AppManager.Instance.Letters);
            int index = UnityEngine.Random.Range(0, Letters.Count);

            Debug.Log("Orginal Word :" + word.TextForLivingLetter);
            //add correct letter answer
            correctAnswers.Add(Letters[index]);
            //save original word
            mRemovedElement = word.Data.Arabic;
            //remove correct letter
            string sQuestion = word.Data.Arabic;
            sQuestion = sQuestion.Remove(index,1);
            //sQuestion = sQuestion.Insert(index, "\u25A0");
            sQuestion = sQuestion.Insert(index, "_");
            word.Data.Arabic = sQuestion;

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

            IQuestionPack questionPack = new SampleQuestionPack(word, wrongAnswers, correctAnswers);
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

            IQuestionPack questionPack = new SampleQuestionPack(question[0], wrongAnswers, correctAnswers);
            return questionPack;
        }

        public void Restore()
        {
            //restore last letter removed
            if(mDataToRestore != null)
                mDataToRestore.Data.Arabic = mRemovedElement;
        }

    }
}