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
                
                
            });

        }




        public enum QuestionPackType {
            type_1, // Word, letters (contained in question), letters (not contained in question)
            type_2,
            type_3,
        }


        //List<FindRightDataQuestionPack> CreateQuestionPacks(int _dimensionPack) {

        //    List<FindRightDataQuestionPack> questions = new List<FindRightDataQuestionPack>();

        //    // 10 QuestionPacks
        //    for (int i = 0; i < _dimensionPack; i++) {
        //        List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
        //        List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

        //        WordData newWordData = AppManager.I.Teacher.GimmeAGoodWordData();
        //        foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(newWordData.Word, AppManager.I.Letters)) {
        //            correctAnswers.Add(letterData);
        //        }

        //        correctAnswers = correctAnswers.Distinct().ToList();

        //        // At least 4 wrong letters
        //        while (wrongAnswers.Count < 4) {
        //            var letter = AppManager.I.Teacher.GimmeARandomLetter();

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