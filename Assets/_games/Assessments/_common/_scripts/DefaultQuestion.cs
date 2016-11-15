using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class DefaultQuestion : IQuestion
    {
        public GameObject gameObject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ILivingLetterData Image()
        {
            throw new NotImplementedException();
        }

        public ILivingLetterData LetterData()
        {
            throw new NotImplementedException();
        }

        public int PlaceholdersCount()
        {
            throw new NotImplementedException();
        }

        public void SetGameObject(GameObject gameObject)
        {
            throw new NotImplementedException();
        }

        public QuestionType Type()
        {
            throw new NotImplementedException();
        }
    }
}
