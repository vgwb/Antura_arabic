using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using EA4S;
using EA4S.API;
using System.Linq;

namespace EA4S.Test {

    public class MiniGamesDropDownList : Dropdown {


        new void Start() {
            options.Clear();
            options.AddRange(addOptionsFromEnum<MiniGameCode>());
            onValueChanged.AddListener(delegate {
                // Example minigame call
                MiniGameCode miniGameCodeSelected = (MiniGameCode)Enum.Parse(typeof(MiniGameCode), options[value].text);

                float difficulty = float.Parse(FindObjectsOfType<InputField>().First(n => n.name == "Difficulty").text);


                MiniGameAPI.Instance.StartGame(
                    miniGameCodeSelected,
                    CreateQuestionPack(9),
                    new GameConfiguration(difficulty)
                    );
                
            });

        }

        List<FindRightDataQuestionPack> CreateQuestionPack(int _dimensionPack) {

            List<FindRightDataQuestionPack> questions = new List<FindRightDataQuestionPack>();

            // 10 QuestionPacks
            for (int i = 0; i < _dimensionPack; i++) {
                List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
                List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

                WordData newWordData = AppManager.Instance.Teacher.GimmeAGoodWordData();
                foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData.Word, AppManager.Instance.Letters)) {
                    correctAnswers.Add(letterData);
                }

                correctAnswers = correctAnswers.Distinct().ToList();

                // At least 4 wrong letters
                while (wrongAnswers.Count < 4) {
                    var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                    if (!correctAnswers.Contains(letter) && !wrongAnswers.Contains(letter)) {
                        wrongAnswers.Add(letter);
                    }
                }

                var currentPack = new FindRightDataQuestionPack( newWordData, wrongAnswers, correctAnswers);
                
                questions.Add(currentPack);
            }
            return questions;
        }

        List<OptionData> addOptionsFromEnum<T>() {
            List<OptionData> optionsToAdd = new List<OptionData>();
            foreach (var val in Enum.GetValues(typeof(T))) {
                optionsToAdd.Add(new OptionData() { text = val.ToString() });
            }
            return optionsToAdd;
        }

        void OnDisable() {
            onValueChanged.RemoveAllListeners();
        }
    }
}