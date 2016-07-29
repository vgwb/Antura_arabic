using UnityEngine;
using Google2u;
using System.Collections.Generic;
using System;

namespace EA4S
{

    public class WordData : ILivingLetterData
    {

        public LivingLetterDataType DataType
        {
            get { return LivingLetterDataType.Word; }
        }

        public string Key
        {
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

        public WordData(string _keyRow, wordsRow _wordRow) {
            Key = _keyRow;
            Kind = _wordRow._kind;
            Category = _wordRow._kind;
            Stage = _wordRow._stage;
            English = _wordRow._english;
            Word = _wordRow._word;
            Letters = _wordRow._letters;
            Transliteration = _wordRow._transliteration;
            DifficultyLevel = _wordRow._difficulty_level;
            NumberOfLetters = _wordRow._number_of_letter;
            Group = _wordRow._group;
        }

        #region API

        static List<WordData> vocabulary = null;

        /// <summary>
        /// Contain entire cached vocabolary.
        /// </summary>
        /// <returns></returns>
        protected static List<WordData> Vocabulary() {
            if (vocabulary == null) {
                vocabulary = new List<WordData>();
                foreach (string wordKey in words.Instance.rowNames) {
                    vocabulary.Add(new WordData(wordKey, words.Instance.GetRow(wordKey)));
                }
            }
            return vocabulary;
        }

        /// <summary>
        /// Get WordData by keyRow.
        /// </summary>
        /// <param name="_keyRow"></param>
        /// <returns></returns>
        public static WordData GetWordDataByKeyRow(string _keyRow) {
            // wordsRow wr = words.Instance.GetRow(_keyRow);
            WordData wd = Vocabulary().Find(w => w.Key == _keyRow);
            return wd;
        }

        /// <summary>
        /// Get word collaction entaire or filtered by category.
        /// </summary>
        /// <param name="_category"></param>
        /// <returns></returns>
        public static List<WordData> GetWordCollection(string _category = "") {
            List<WordData> returnList = Vocabulary();
            if (_category != "")
                return returnList.FindAll(w => w.Category == _category);
            return returnList;
        }

        #region API

        /// <summary>
        /// Living Letter Text To Display.
        /// </summary>
        public string TextForLivingLetter
        {
            get { return ArabicAlphabetHelper.ParseWord(Word, AppManager.Instance.Letters); }
        }

        /// <summary>
        /// Return draw of word.
        /// </summary>
        public Sprite DrawForLivingLetter
        {
            get { return Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + Key); }
        }

        #endregion

        #endregion
    }
}