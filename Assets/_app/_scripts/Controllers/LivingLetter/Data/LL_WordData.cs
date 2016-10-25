using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class LL_WordData : ILivingLetterData
    {

        public LivingLetterDataType DataType {
            get { return LivingLetterDataType.Word; }
        }

        public string Key {
            get { return key; }
            set { key = value; }
        }

        public string Kind;
        public string Category;
        public string English;
        public string Word;
        public string Letters;
        public string Transliteration;
        public string DifficultyLevel;
        public string NumberOfLetters;
        public string Group;

        private string key;

        public LL_WordData(string _keyRow, Db.WordData _wordRow)
        {
            Key = _keyRow;
            Kind = _wordRow.Kind.ToString();
            Category = _wordRow.Category.ToString();
            English = _keyRow;
            Word = _wordRow.Arabic;
            Letters = _wordRow.Letters.ToString();
            Transliteration = _wordRow.Transliteration;
            DifficultyLevel = _wordRow.Difficulty.ToString();
            NumberOfLetters = _wordRow.NumberOfLetters.ToString();
        }

        #region API

        /// <summary>
        /// Get WordData by keyRow.
        /// </summary>
        /// <param name="_keyRow"></param>
        /// <returns></returns>
        public static LL_WordData GetWordDataByKeyRow(string _keyRow)
        {
            // wordsRow wr = words.Instance.GetRow(_keyRow);

            Db.WordData wdata = AppManager.Instance.DB.GetWordDataById(_keyRow);
            LL_WordData wd = new LL_WordData(_keyRow, wdata);
            return wd;
        }


        public static LL_WordData GetRandomWord()
        {
            // wordsRow wr = words.Instance.GetRow(_keyRow);

            Db.WordData wdata = AppManager.Instance.DB.GetWordDataByRandom();
            LL_WordData wd = new LL_WordData(wdata.GetId(), wdata);
            return wd;
        }


        #region API

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter {
            get { return ArabicAlphabetHelper.ParseWord(Word, AppManager.Instance.Letters); }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        public Sprite DrawForLivingLetter {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Key); }
        }

        #endregion

        #endregion
    }
}