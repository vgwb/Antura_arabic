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

        private string key;
        public string Key {
            get { return key; }
            set { key = value; }
        }

        public Db.WordData Data;

        public LL_WordData(string _keyRow, Db.WordData _wordRow)
        {
            Key = _keyRow;
            Data = _wordRow;
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
            get { return ArabicAlphabetHelper.ParseWord(Data.Arabic, AppManager.Instance.Letters); }
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