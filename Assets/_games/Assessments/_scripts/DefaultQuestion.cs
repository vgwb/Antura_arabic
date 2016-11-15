using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestion : IQuestion
    {
        private LetterObjectView view;
        private int placeholders;
        private QuestionType type;

        public DefaultQuestion( LetterObjectView letter, int placeholders, QuestionType type)
        {
            view = letter;
            this.placeholders = placeholders;
            this.type = type;
            var question = letter.gameObject.AddComponent< QuestionBehaviour>();
            question.SetQuestion( this);
        }

        public GameObject gameObject
        {
            get
            {
                return view.gameObject;
            }
        }

        public ILivingLetterData Image()
        {
            throw new NotImplementedException("Not implemented");
        }

        public ILivingLetterData LetterData()
        {
            return view.Data;
        }

        public int PlaceholdersCount()
        {
            return placeholders;
        }

        public QuestionType Type()
        {
            return type;
        }
    }
}
