using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using EA4S.API;

namespace EA4S
{
    public enum DifficulyLevels
    {
        VeryEasy = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3,
        VeryHard = 4
    }

    public class DebugManager : MonoBehaviour
    {
        public static DebugManager I;

        private DifficulyLevels _difficultyLevel = DifficulyLevels.Normal;
        public DifficulyLevels DifficultyLevel {
            get { return _difficultyLevel; }
            set {
                _difficultyLevel = value;
                switch (_difficultyLevel) {
                    case DifficulyLevels.VeryEasy:
                        Difficulty = 0.1f;
                        break;
                    case DifficulyLevels.Easy:
                        Difficulty = 0.3f;
                        break;
                    case DifficulyLevels.Normal:
                        Difficulty = 0.5f;
                        break;
                    case DifficulyLevels.Hard:
                        Difficulty = 0.7f;
                        break;
                    case DifficulyLevels.VeryHard:
                        Difficulty = 1.0f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public float Difficulty = 0.5f;
        public int Stage = 1;
        public int LearningBlock = 1;

        void Awake()
        {
            I = this;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("DEBUG - SPACE : skip");
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
                Debug.Log("DEBUG - 0");
            }

            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) {
                Debug.Log("DEBUG - 1");
            }

            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) {
                Debug.Log("DEBUG - 2");
            }

            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) {
                Debug.Log("DEBUG - 3");
            }
        }



        public void LaunchMinigGame(MiniGameCode miniGameCodeSelected)
        {
            // Call start game with parameters
            MiniGameAPI.Instance.StartGame(
                miniGameCodeSelected,
                new GameConfiguration(Difficulty)
            );

        }


        #region Test Helpers

        private static List<LL_LetterData> GetLettersFromWord(LL_WordData _word)
        {
            var letters = new List<LL_LetterData>();
            foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(_word.Data.Arabic, AppManager.Instance.Teacher.GetAllTestLetterDataLL())) {
                letters.Add(letterData);
            }
            return letters;
        }

        private static List<LL_LetterData> GetLettersNotContained(List<LL_LetterData> _lettersToAvoid, int _count)
        {
            var letterListToReturn = new List<LL_LetterData>();
            for (var i = 0; i < _count; i++) {
                var letter = AppManager.Instance.Teacher.GetRandomTestLetterLL();

                if (!CheckIfContains(_lettersToAvoid, letter) && !CheckIfContains(letterListToReturn, letter)) {
                    letterListToReturn.Add(letter);
                }
            }
            return letterListToReturn;
        }


        private static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }


        private static bool CheckIfContains(List<LL_LetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        private static bool CheckIfContains(List<LL_WordData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        #endregion
    }
}
