using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using EA4S.Helpers;
using EA4S.MinigamesAPI;

namespace EA4S.Test
{
    /// <summary>
    /// Helper class to test the launch of specific minigames
    /// </summary>
    // refactor: should use the actual MiniGameLauncher
    // refactor: rename to TestMiniGameLauncher
    public class MiniGameLauncher : MonoBehaviour
    {

        public MiniGamesDropDownList MiniGamesDropDownList;
        public Button LaunchButton;

        public void LaunchGame()
        {
            // Example minigame call
            MiniGameCode miniGameCodeSelected = (MiniGameCode)Enum.Parse(typeof(MiniGameCode), MiniGamesDropDownList.options[MiniGamesDropDownList.value].text);
            float difficulty = float.Parse(FindObjectsOfType<InputField>().First(n => n.name == "Difficulty").text);

            // Call start game with parameters
            AppManager.I.GameLauncher.LaunchGame(miniGameCodeSelected, new MinigameLaunchConfiguration(difficulty));
        }

        #region Test Helpers

        LL_WordData GetWord()
        {
            return AppManager.I.Teacher.GetRandomTestWordDataLL();
        }

        List<LL_WordData> GetWordsNotContained(List<LL_WordData> _WordsToAvoid, int _count)
        {
            List<LL_WordData> wordListToReturn = new List<LL_WordData>();
            for (int i = 0; i < _count; i++)
            {
                var word = AppManager.I.Teacher.GetRandomTestWordDataLL();

                if (!CheckIfContains(_WordsToAvoid, word) && !CheckIfContains(wordListToReturn, word))
                {
                    wordListToReturn.Add(word);
                }
            }
            return wordListToReturn;
        }

        List<LL_LetterData> GetLettersFromWord(LL_WordData _word)
        {
            List<LL_LetterData> letters = new List<LL_LetterData>();
            foreach (var letterData in ArabicAlphabetHelper.AnalyzeData(AppManager.I.DB, _word.Data))
            {
                letters.Add(new LL_LetterData(letterData.letter));
            }
            return letters;
        }

        List<LL_LetterData> GetLettersNotContained(List<LL_LetterData> _lettersToAvoid, int _count)
        {
            List<LL_LetterData> letterListToReturn = new List<LL_LetterData>();
            for (int i = 0; i < _count; i++)
            {
                var letter = AppManager.I.Teacher.GetRandomTestLetterLL();

                if (!CheckIfContains(_lettersToAvoid, letter) && !CheckIfContains(letterListToReturn, letter))
                {
                    letterListToReturn.Add(letter);
                }
            }
            return letterListToReturn;
        }


        static bool CheckIfContains(List<ILivingLetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Id == letter.Id)
                    return true;
            return false;
        }


        static bool CheckIfContains(List<LL_LetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Id == letter.Id)
                    return true;
            return false;
        }

        static bool CheckIfContains(List<LL_WordData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Id == letter.Id)
                    return true;
            return false;
        }

        #endregion
    }
}
