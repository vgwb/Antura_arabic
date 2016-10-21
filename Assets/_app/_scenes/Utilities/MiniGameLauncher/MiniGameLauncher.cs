using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using EA4S.API;
using System.Collections.Generic;

namespace EA4S.Test {

    public class MiniGameLauncher : MonoBehaviour {

        public MiniGamesDropDownList MiniGamesDropDownList;
        public Button LaunchButton;

        public void LaunchGame() {
            // Example minigame call
            MiniGameCode miniGameCodeSelected = (MiniGameCode)Enum.Parse(typeof(MiniGameCode), MiniGamesDropDownList.options[MiniGamesDropDownList.value].text);
            float difficulty = float.Parse(FindObjectsOfType<InputField>().First(n => n.name == "Difficulty").text);

            int packsCount = 0;

            switch (miniGameCodeSelected) {
                case MiniGameCode.Egg:
                    packsCount = 4;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    packsCount = 1;
                    break;
                case MiniGameCode.FastCrowd_letter:
                case MiniGameCode.FastCrowd_spelling:
                case MiniGameCode.FastCrowd_words:
                case MiniGameCode.FastCrowd_counting:
                case MiniGameCode.Tobogan_letters:
                case MiniGameCode.Tobogan_words:
                    packsCount = 10;
                    break;
                case MiniGameCode.Assessment:
                case MiniGameCode.AlphabetSong:
                case MiniGameCode.Balloons_counting:
                case MiniGameCode.Balloons_letter:
                case MiniGameCode.Balloons_spelling:
                case MiniGameCode.Balloons_words:
                case MiniGameCode.ColorTickle:
                case MiniGameCode.DancingDots:
                case MiniGameCode.DontWakeUp:
                case MiniGameCode.HiddenSource:
                case MiniGameCode.HideSeek:
                case MiniGameCode.MakeFriends:
                case MiniGameCode.Maze:
                case MiniGameCode.MissingLetter:
                case MiniGameCode.MissingLetter_phrases:
                case MiniGameCode.MixedLetters_alphabet:
                case MiniGameCode.MixedLetters_spelling:
                case MiniGameCode.SickLetter:
                case MiniGameCode.ReadingGame:
                case MiniGameCode.Scanner:
                case MiniGameCode.Scanner_phrase:
                case MiniGameCode.ThrowBalls_letters:
                case MiniGameCode.ThrowBalls_words:
                    break;

            }
            // Call start game with parameters
            MiniGameAPI.Instance.StartGame(
                miniGameCodeSelected,
                CreateQuestionPacksDummyAI(miniGameCodeSelected, packsCount),
                new GameConfiguration(difficulty)
            );

        }

        List<IQuestionPack> CreateQuestionPacksDummyAI(MiniGameCode _miniGameCode, int _questionNumber) {
            List<IQuestionPack> questionPackList = new List<IQuestionPack>();
            for (int i = 0; i < _questionNumber; i++) {
                questionPackList.Add(CreateQuestionPack(_miniGameCode));
            }
            return questionPackList;
        }

        IQuestionPack CreateQuestionPack(MiniGameCode _miniGameCode) {
            IQuestionPack questionPack = null;
            ILivingLetterData question;
            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            List<LetterData> letters = new List<LetterData>();

            switch (_miniGameCode) {
                case MiniGameCode.Assessment:
                    break;
                case MiniGameCode.AlphabetSong:
                    break;
                case MiniGameCode.Balloons_counting:
                    break;
                case MiniGameCode.Balloons_letter:
                    break;
                case MiniGameCode.Balloons_spelling:
                    break;
                case MiniGameCode.Balloons_words:
                    break;
                case MiniGameCode.ColorTickle:
                    break;
                case MiniGameCode.DancingDots:
                    break;
                case MiniGameCode.DontWakeUp:
                    break;
                case MiniGameCode.Egg:
                    //
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as WordData);
                    foreach (var l in letters) {
                        correctAnswers.Add(l);
                    }
                    // The AI in definitive version must check the difficulty threshold (0.5f in example) to determine gameplayType without passing wrongAnswers
                    if (float.Parse(FindObjectsOfType<InputField>().First(n => n.name == "Difficulty").text) < 0.5f) {
                        letters = GetLettersNotContained(letters, 7);
                        foreach (var l in letters) {
                            wrongAnswers.Add(l);
                        }
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    // Dummy logic for get fake full ordered alphabet.
                    for (int i = 0; i < 28; i++) {
                        correctAnswers.Add(AppManager.Instance.Teacher.GimmeARandomLetter());
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(null, null, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_counting:
                    // Dummy logic for question creation
                    WordData word = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    correctAnswers.Add(word);
                    for (int i = 0; i < 10; i++) {
                        WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!correctAnswers.Contains(wrongWord))
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(word, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_letter:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeARandomLetter();
                    letters = GetLettersNotContained(letters, 3); // TODO: auto generation in game
                    foreach (var l in letters) {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_spelling: // var 1
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
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_words:
                    // Dummy logic for question creation
                    for (int i = 0; i < 4; i++) {
                        WordData correctWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!correctAnswers.Contains(correctWord))
                            correctAnswers.Add(correctWord);
                        else
                            i--;
                    }
                    for (int i = 0; i < 2; i++) {
                        WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!correctAnswers.Contains(wrongWord))
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(null, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.HiddenSource:
                    break;
                case MiniGameCode.HideSeek:
                    break;
                case MiniGameCode.MakeFriends:
                    break;
                case MiniGameCode.Maze:
                    break;
                case MiniGameCode.MissingLetter:
                    break;
                case MiniGameCode.MissingLetter_phrases:
                    break;
                case MiniGameCode.MixedLetters_alphabet:
                    break;
                case MiniGameCode.MixedLetters_spelling:
                    break;
                case MiniGameCode.SickLetter:
                    break;
                case MiniGameCode.ReadingGame:
                    break;
                case MiniGameCode.Scanner:
                    break;
                case MiniGameCode.Scanner_phrase:
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    break;
                case MiniGameCode.ThrowBalls_words:
                    break;
                case MiniGameCode.Tobogan_letters:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as WordData);
                    foreach (var l in letters) {
                        correctAnswers.Add(l);
                        break; //get alway first (only for test)
                    }
                    letters = GetLettersNotContained(letters, 3);
                    foreach (var l in letters) {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.Tobogan_words:
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

        List<WordData> GetWordsNotContained(List<WordData> _WordsToAvoid, int _count) {
            List<WordData> wordListToReturn = new List<WordData>();
            for (int i = 0; i < _count; i++) {
                var word = AppManager.Instance.Teacher.GimmeAGoodWordData();

                if (!_WordsToAvoid.Contains(word) && !wordListToReturn.Contains(word)) {
                    wordListToReturn.Add(word);
                }
            }
            return wordListToReturn;
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
    }
}
