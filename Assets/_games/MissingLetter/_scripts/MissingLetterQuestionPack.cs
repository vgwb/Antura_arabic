using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EA4S.MissingLetter
{
    public class MissingLetterQuestionPack : IQuestionPack
    {
        //ILivingLetterData questionSentence;
        IEnumerable<ILivingLetterData> questionSentence;
        IEnumerable<ILivingLetterData> wrongAnswersSentence;
        IEnumerable<ILivingLetterData> correctAnswersSentence;

        public MissingLetterQuestionPack()
        {

        }
        public MissingLetterQuestionPack(IEnumerable<ILivingLetterData> questionSentence, IEnumerable<ILivingLetterData> wrongAnswersSentence, IEnumerable<ILivingLetterData> correctAnswersSentence)
        {
            this.questionSentence = questionSentence;
            this.wrongAnswersSentence = wrongAnswersSentence;
            this.correctAnswersSentence = correctAnswersSentence;
        }

        //TODO how to return a list of word (sentence) using this interface?
        IEnumerable<ILivingLetterData> GetQuestion()
        {
            return questionSentence;
        }

        ILivingLetterData IQuestionPack.GetQuestion()
        {
            return questionSentence.ToList()[0];
        }

        IEnumerable<ILivingLetterData> IQuestionPack.GetWrongAnswers()
        {
            return wrongAnswersSentence;
        }

        IEnumerable<ILivingLetterData> IQuestionPack.GetCorrectAnswers()
        {
            return correctAnswersSentence;
        }
    }
    }
