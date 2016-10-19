using UnityEngine;
using Google2u;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class WordData : ILivingLetterData
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
        public string Stage;
        public string English;
        public string Word;
        public string Letters;
        public string Transliteration;
        public string DifficultyLevel;
        public string NumberOfLetters;
        public string Group;

        private string key;

        public WordData(string _keyRow, Db.WordData _wordRow)
        {
            Key = _keyRow;
            Kind = _wordRow.Kind;
            Category = _wordRow.Category;
            English = _wordRow.English;
            Word = _wordRow.Arabic;
            Letters = _wordRow.Letters.ToString();
            Transliteration = _wordRow.Transliteration;
            DifficultyLevel = _wordRow.Difficulty.ToString();
            NumberOfLetters = _wordRow.NumberOfLetters.ToString();
            Group = _wordRow.Group;
        }

        #region API

        /// <summary>
        /// Get WordData by keyRow.
        /// </summary>
        /// <param name="_keyRow"></param>
        /// <returns></returns>
        public static WordData GetWordDataByKeyRow(string _keyRow)
        {
            // wordsRow wr = words.Instance.GetRow(_keyRow);

            Db.WordData wdata = AppManager.Instance.DB.GetWordDataById(_keyRow);
            WordData wd = new WordData(_keyRow, wdata);
            return wd;
        }


        public static WordData GetRandomWord()
        {
            // wordsRow wr = words.Instance.GetRow(_keyRow);

            Db.WordData wdata = AppManager.Instance.DB.GetWordDataByRandom();
            WordData wd = new WordData(wdata.GetId(), wdata);
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