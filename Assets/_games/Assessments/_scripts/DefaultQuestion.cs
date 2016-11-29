using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestion : IQuestion
    {
        private LetterObjectView view;
        private int placeholdersCount;

        public DefaultQuestion( LetterObjectView letter, int placeholders)
        {
            view = letter;
            placeholdersCount = placeholders;
            placeholdersSet = new List< GameObject>();
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
            throw new NotImplementedException( "Not implemented (on purpose)");
        }

        public ILivingLetterData LetterData()
        {
            return view.Data;
        }

        public int PlaceholdersCount()
        {
            return placeholdersCount;
        }

        private List< GameObject> placeholdersSet;

        public void TrackPlaceholder( GameObject gameObject)
        {
            placeholdersSet.Add( gameObject);
        }

        public IEnumerable< GameObject> GetPlaceholders()
        {
            if (placeholdersSet.Count != placeholdersCount)
                throw new InvalidOperationException( "Something wrong. Check Question placer");

            return placeholdersSet;
        }

        private AnswerSet answerSet;
        public void SetAnswerSet( AnswerSet answerSet)
        {
            this.answerSet = answerSet;
        }

        public AnswerSet GetAnswerSet()
        {
            return answerSet;
        }
    }
}
