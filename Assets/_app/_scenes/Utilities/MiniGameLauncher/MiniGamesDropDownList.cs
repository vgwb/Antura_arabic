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
                    CreateQuestionPacksDummyAI(10, QuestionPackType.type_1),
                    new GameConfiguration(difficulty)
                );
                
            });

        }

        List<IQuestionPack> CreateQuestionPacksDummyAI(int _questionNumber, QuestionPackType _packType) {
            List<IQuestionPack> questionPackList = new List<IQuestionPack>();
            for (int i = 0; i < _questionNumber; i++) {
                questionPackList.Add(CreateQuestionPack(_packType));
            }
            return questionPackList;
        }

        IQuestionPack CreateQuestionPack(QuestionPackType _packType) {
            IQuestionPack questionPack = null;
            ILivingLetterData question;
            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            List<LetterData> letters;
            //var wrongAnswers;
            switch (_packType) {
                case QuestionPackType.type_1:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as WordData);
                    foreach (var l in letters) {
                        correctAnswers.Add(l);
                    }
                    letters = GetLettersNotContained(letters, 8);
                    foreach (var l in letters) {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, wrongAnswers);
                    break;
                case QuestionPackType.type_2:
                    break;
                case QuestionPackType.type_3:
                    break;
                default:
                    break;
            }

            return questionPack;

            
        }
        #region Test Helpers

        WordData GetWord() {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }

        List<LetterData> GetLettersFromWord(WordData _word) {
            List<LetterData> letters = new List<LetterData>();
            foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(_word.Word, AppManager.Instance.Letters)) {
                letters.Add(letterData);
            }
            return letters;
        }

        List<LetterData> GetLettersNotContained(List<LetterData> _lettersToAvoid, int _count) {
            List<LetterData> letterListToReturn = new List<LetterData>();
            for (int i = 0; i < _count; i++) {
                var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

                if (!_lettersToAvoid.Contains(letter) && !letterListToReturn.Contains(letter)) {
                    letterListToReturn.Add(letter);
                }
            }
            return letterListToReturn;
        }

        #endregion


        public enum QuestionPackType {
            type_1, // Word, letters, letters
            type_2,
            type_3,
        }


        //List<FindRightDataQuestionPack> CreateQuestionPacks(int _dimensionPack) {

        //    List<FindRightDataQuestionPack> questions = new List<FindRightDataQuestionPack>();

        //    // 10 QuestionPacks
        //    for (int i = 0; i < _dimensionPack; i++) {
        //        List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
        //        List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

        //        WordData newWordData = AppManager.Instance.Teacher.GimmeAGoodWordData();
        //        foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData.Word, AppManager.Instance.Letters)) {
        //            correctAnswers.Add(letterData);
        //        }

        //        correctAnswers = correctAnswers.Distinct().ToList();

        //        // At least 4 wrong letters
        //        while (wrongAnswers.Count < 4) {
        //            var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

        //            if (!correctAnswers.Contains(letter) && !wrongAnswers.Contains(letter)) {
        //                wrongAnswers.Add(letter);
        //            }
        //        }

        //        var currentPack = new FindRightDataQuestionPack( newWordData, wrongAnswers, correctAnswers);
                
        //        questions.Add(currentPack);
        //    }
        //    return questions;
        //}

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