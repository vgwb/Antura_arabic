using System;
using UnityEngine;

namespace EA4S
{
    public class StringTestData : ILivingLetterData
    {
        string text;
        public StringTestData(string text)
        {
            this.text = text;
        }

        public LivingLetterDataType DataType
        {
            get
            {
                return LivingLetterDataType.Word;
            }
        }

        public Sprite DrawForLivingLetter
        {
            get
            {
                return null;
            }
        }

        public string Key        {            get; set;        }

        public string TextForLivingLetter
        {
            get
            {
                return text;
            }
        }
    }
}