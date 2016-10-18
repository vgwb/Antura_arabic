using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class AssessmentData :IData
    {
        public AssessmentType Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

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