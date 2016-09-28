using UnityEngine;
using System.Collections.Generic;
using ModularFramework.Core;
using EA4S;

namespace EA4S.API {

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ModularFramework.Core.Singleton{EA4S.API.MiniGameAPI}" />
    public class MiniGameAPI : Singleton<MiniGameAPI> {

        #region Learning course 

        #endregion

        #region Teacher AI
        /// TODO:
        /// - Game To Show in the wheel
        /// - Is it time for an Assessment?
        /// - Is it time for an extra (outbound) activity?


        /// <summary>
        /// Return all information needed from minigame to setup gameplay session.
        /// </summary>
        /// <returns></returns>
        public PlaySessionInfo GetPlaySessionInfo() {
            PlaySessionInfo playSessionInfo = new PlaySessionInfo();
            // TODO
            return playSessionInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_result"></param>
        public void PushPlaySessionResult(PlaySessionResult _result) {
            // TODO
        }

        /// <summary>
        /// Get corresponding arabic letter with _letterId param.
        /// </summary>
        /// <param name="_letterId"></param>
        /// <returns></returns>
        public string GetArabicLetter(string _letterId) {
            // TODO
            return "";
        }

        /// <summary>
        /// Return List of Letter Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <returns></returns>
        public List<LetterData> GetValidLetters(int _amount = -1) {
            // TODO
            return new List<LetterData>();
        }

        /// <summary>
        /// Return List of Word Data valid for this gameplay and actual profile data.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
        public List<WordData> GetValidWorlds(int _amount = -1) {
            // TODO
            return new List<WordData>();
        }

        #endregion

        #region Log System

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_area"></param>
        /// <param name="_context"></param>
        /// <param name="_action"></param>
        /// <param name="_data"></param>
        public void Log(string _area, string _context, string _action, string _data) {
            LoggerEA4S.Log(_area, _context, _action, _data);
        }

        #endregion

        #region Player profile

        #endregion

        #region Audio

        #endregion

        #region Localization Data
        /// TODO: 
        /// - Create localization interface for data structure for single language.
        #endregion

        #region AssetManager
        /// TODO:
        /// - Living letters base
        /// -- base init by ILivingLetterData
        /// -- animation controller
        /// - Input Controller
        /// - AI controller

        /// <summary>
        /// Return asset with specific generic asset Id and theme.
        /// </summary>
        /// <param name="_prefId"></param>
        /// <param name="_themeId"></param>
        /// <returns></returns>
        public GameObject GetAsset(string _prefId, int _themeId) {
            // TODO
            return null;
        }

        #endregion

    }

    #region Data Structures

    /// <summary>
    /// Information needed from minigame to setup gameplay session.
    /// </summary>
    public class PlaySessionInfo {

        /// <summary>
        /// List of learning objectives to use in the game (letters, words, etc).
        /// </summary>
        public List<ILivingLetterData> LivingLetterData;

        /// TODO:
        /// - AbilityType1, AbilityType2, etc...
        /// - level
        /// - difficulty
        /// - parameters

    }


    /// <summary>
    /// Information used to comunicate result of minigame gameplay session.
    /// </summary>
    public class PlaySessionResult {
        /// <summary>
        /// Number of stars based on playsession result.
        /// 0 = negative result.
        /// 1, 2, 3 = positive result.
        /// </summary>
        public int StarResult;


    }

    public class ProfileInfoForMinigame {

    }

    #endregion
}