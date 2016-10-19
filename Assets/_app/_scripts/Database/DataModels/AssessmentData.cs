using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class AssessmentData : IData
    {
        public AssessmentType Id;
        public string Title;
        public string Description;

        public string GetId()
        {
            return Id.ToString();
        }
    }

    public enum AssessmentType
    {
        Letters = 1,
        LettersMatchShape = 2,
        Alphabet = 3
    }
}