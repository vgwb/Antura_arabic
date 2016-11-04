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
            var packsCount = 0;

            switch (miniGameCodeSelected) {
                case MiniGameCode.Egg:
                    packsCount = 4;
                    break;
                case MiniGameCode.FastCrowd_alphabet:
                    packsCount = 1;
                    break;
                case MiniGameCode.DancingDots:
                case MiniGameCode.ThrowBalls_letters:
                case MiniGameCode.ThrowBalls_words:
                case MiniGameCode.Maze:
                case MiniGameCode.Balloons_counting:
                case MiniGameCode.Balloons_letter:
                case MiniGameCode.Balloons_spelling:
                case MiniGameCode.Balloons_words:
                case MiniGameCode.FastCrowd_letter:
                case MiniGameCode.FastCrowd_spelling:
                case MiniGameCode.FastCrowd_words:
                case MiniGameCode.FastCrowd_counting:
                case MiniGameCode.Tobogan_letters:
                case MiniGameCode.Tobogan_words:
                    packsCount = 10;
                    break;
                case MiniGameCode.Assessment_Letters:
                case MiniGameCode.Assessment_LettersMatchShape:
                case MiniGameCode.AlphabetSong:
                case MiniGameCode.ColorTickle:
                case MiniGameCode.DontWakeUp:
                case MiniGameCode.HiddenSource:
                case MiniGameCode.HideSeek:
                case MiniGameCode.MakeFriends:
                case MiniGameCode.MissingLetter:
                case MiniGameCode.MissingLetter_phrases:
                case MiniGameCode.MixedLetters_alphabet:
                case MiniGameCode.MixedLetters_spelling:
                case MiniGameCode.SickLetter:
                case MiniGameCode.ReadingGame:
                case MiniGameCode.Scanner:
                case MiniGameCode.Scanner_phrase:

                    break;

                case MiniGameCode.ThrowBalls_letterinword:
                    break;
                case MiniGameCode.TakeMeHome:
                    break;
                case MiniGameCode.Assessment_3:
                    break;
                case MiniGameCode.Assessment_4:
                    break;
                case MiniGameCode.Assessment_5:
                    break;
                case MiniGameCode.Assessment_Alphabet:
                    break;
                case MiniGameCode.Assessment_7:
                    break;
                case MiniGameCode.Assessment_8:
                    break;
                case MiniGameCode.Assessment_9:
                    break;
                case MiniGameCode.Assessment_10:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("miniGameCodeSelected", miniGameCodeSelected, null);
            }
            // Call start game with parameters
            MiniGameAPI.Instance.StartGame(
                miniGameCodeSelected,
                CreateQuestionPacksDummyAI(miniGameCodeSelected, packsCount),
                new GameConfiguration(Difficulty)
            );

        }

        List<IQuestionPack> CreateQuestionPacksDummyAI(MiniGameCode _miniGameCode, int _questionNumber)
        {
            var questionPackList = new List<IQuestionPack>();
            for (var i = 0; i < _questionNumber; i++) {
                questionPackList.Add(CreateQuestionPack(_miniGameCode));
            }
            return questionPackList;
        }

        private IQuestionPack CreateQuestionPack(MiniGameCode _miniGameCode)
        {
            IQuestionPack questionPack = null;
            ILivingLetterData question;
            var correctAnswers = new List<ILivingLetterData>();
            var wrongAnswers = new List<ILivingLetterData>();
            var letters = new List<LL_LetterData>();

            switch (_miniGameCode) {
                case MiniGameCode.Assessment_Letters:
                case MiniGameCode.Assessment_LettersMatchShape:
                    break;
                case MiniGameCode.AlphabetSong:
                    break;
                case MiniGameCode.Balloons_counting:
                    // Dummy logic for question creation
                    foreach (var w in AppManager.Instance.DB.GetAllWordData().Where(w => w.Category == Db.WordDataCategory.Number)) {
                        var w_ll = new LL_WordData(w.Id, w);
                        correctAnswers.Add(w_ll);
                        if (correctAnswers.Count > 10)
                            break;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(null, null, correctAnswers);
                    break;
                case MiniGameCode.Balloons_letter:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeARandomLetter();

                    for (var i = 0; i < 1; i++) {
                        var correctWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (CheckIfContains(GetLettersFromWord(correctWord), question))
                            correctAnswers.Add(correctWord);
                        else
                            i--;
                    }

                    for (var i = 0; i < 10; i++) {
                        var wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(GetLettersFromWord(wrongWord), question))
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.Balloons_spelling:
                    // Dummy logic for question creation
                    letters = GetLettersFromWord(AppManager.Instance.Teacher.GimmeAGoodWordData());
                    foreach (var l in letters) {
                        correctAnswers.Add(l);
                    }
                    letters = GetLettersNotContained(letters, 8);
                    foreach (var l in letters) {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(null, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.Balloons_words:
                    // Dummy logic for question creation
                    LL_WordData balloonWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    correctAnswers.Add(balloonWord);
                    for (int i = 0; i < 10; i++) {
                        LL_WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!correctAnswers.Contains(wrongWord))
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(balloonWord, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.ColorTickle:
                    break;
                case MiniGameCode.DancingDots:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeARandomLetter();
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, null, null);
                    break;
                case MiniGameCode.DontWakeUp:
                    break;
                case MiniGameCode.Egg:
                    //
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as LL_WordData);
                    foreach (var l in letters) {
                        correctAnswers.Add(l);
                    }
                    // The AI in definitive version must check the difficulty threshold (0.5f in example) to determine gameplayType without passing wrongAnswers
                    if (Difficulty < 0.5f) {
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
                    foreach (var letter in AppManager.Instance.DB.GetAllLetterData())
                        correctAnswers.Add(new LL_LetterData(letter.GetId()));

                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(null, null, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_counting:
                    // Dummy logic for question creation
                    var word = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    correctAnswers.Add(word);
                    for (var i = 0; i < 10; i++) {
                        var wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
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
                    letters = GetLettersFromWord((LL_WordData) question);
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
                    for (var i = 0; i < 4; i++) {
                        var correctWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(correctAnswers, correctWord))
                            correctAnswers.Add(correctWord);
                        else
                            i--;
                    }
                    for (var i = 0; i < 2; i++) {
                        var wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(correctAnswers, wrongWord))
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
                    question = AppManager.Instance.Teacher.GimmeARandomLetter();
                    for (var i = 0; i < 2; i++) {
                        var correctWordMakeFriends = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(correctAnswers, correctWordMakeFriends) && CheckIfContains(GetLettersFromWord(correctWordMakeFriends), question))
                            // if not already in list and contain question letterData
                            correctAnswers.Add(correctWordMakeFriends);
                        else
                            i--;
                    }
                    for (var i = 0; i < 8; i++) {
                        var wrongWordMakeFriends = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(GetLettersFromWord(wrongWordMakeFriends), question))
                            // if not contain quest letter
                            wrongAnswers.Add(wrongWordMakeFriends);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.Maze:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, null, null);
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
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();

                    for (int i = 0; i < 8; i++) {
                        LL_WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (wrongWord != question)
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, null);
                    break;
                case MiniGameCode.Scanner_phrase:
                    // Wait Design info
                    break;
                case MiniGameCode.ThrowBalls_letters:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeARandomLetter();
                    letters = GetLettersNotContained(letters, 3);
                    foreach (var l in letters) {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, null);
                    break;
                case MiniGameCode.ThrowBalls_words:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();

                    for (int i = 0; i < 8; i++) {
                        LL_WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (wrongWord != question)
                            wrongAnswers.Add(wrongWord);
                        else
                            i--;
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, null);
                    break;
                case MiniGameCode.Tobogan_letters:
                    // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as LL_WordData);
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
                case MiniGameCode.ThrowBalls_letterinword:
                    break;
                case MiniGameCode.TakeMeHome:
                    break;
                case MiniGameCode.Assessment_3:
                    break;
                case MiniGameCode.Assessment_4:
                    break;
                case MiniGameCode.Assessment_5:
                    break;
                case MiniGameCode.Assessment_Alphabet:
                    break;
                case MiniGameCode.Assessment_7:
                    break;
                case MiniGameCode.Assessment_8:
                    break;
                case MiniGameCode.Assessment_9:
                    break;
                case MiniGameCode.Assessment_10:
                    break;
                default:
                    break;
            }
            return questionPack;
        }

        #region Test Helpers

        private static List<LL_LetterData> GetLettersFromWord(LL_WordData _word)
        {
            var letters = new List<LL_LetterData>();
            foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(_word.Data.Arabic, AppManager.Instance.Letters)) {
                letters.Add(letterData);
            }
            return letters;
        }

        private static List<LL_LetterData> GetLettersNotContained(List<LL_LetterData> _lettersToAvoid, int _count)
        {
            var letterListToReturn = new List<LL_LetterData>();
            for (var i = 0; i < _count; i++) {
                var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

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
