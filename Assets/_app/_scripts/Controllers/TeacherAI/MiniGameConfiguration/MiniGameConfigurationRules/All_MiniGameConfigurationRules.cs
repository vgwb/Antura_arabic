using EA4S.API;
using System.Collections.Generic;

namespace EA4S.MiniGameConfiguration
{
    /// <summary>
    /// The old minigame configuration rules that uses a big switch to define the rules
    /// </summary>
    public class All_MiniGameConfigurationRules : IMiniGameConfigurationRules
    {
        MiniGameCode code;

        public All_MiniGameConfigurationRules(MiniGameCode _code)
        {
            code = _code;
        }

        public int GetQuestionPackCount()
        {
            MiniGameCode _miniGameCode = this.code;
            int packsCount = 0;
            switch (_miniGameCode)
            {
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
                case MiniGameCode.Assessment_Letters:
                case MiniGameCode.Assessment_LettersMatchShape:
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
            return packsCount;
        }

        public IQuestionPack CreateQuestionPack()
        {
            MiniGameCode _miniGameCode = this.code;
            IQuestionPack questionPack = null;
            ILivingLetterData question;
            List<ILivingLetterData> correctAnswers = new List<ILivingLetterData>();
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();
            List<LL_LetterData> letters = new List<LL_LetterData>();

            switch (_miniGameCode)
            {
                case MiniGameCode.Assessment_Letters:
                case MiniGameCode.Assessment_LettersMatchShape:
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
                    letters = GetLettersFromWord(question as LL_WordData);
                    foreach (var l in letters)
                    {
                        correctAnswers.Add(l);
                    }
                    // The AI in definitive version must check the difficulty threshold (0.5f in example) to determine gameplayType without passing wrongAnswers
                    //if (difficulty < 0.5f) {
                    letters = GetLettersNotContained(letters, 7);
                    foreach (var l in letters)
                    {
                        wrongAnswers.Add(l);
                    }
                    //}
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
                    LL_WordData word = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    correctAnswers.Add(word);
                    for (int i = 0; i < 10; i++)
                    {
                        LL_WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
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
                    foreach (var l in letters)
                    {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_spelling: // var 1
                                                      // Dummy logic for question creation
                    question = AppManager.Instance.Teacher.GimmeAGoodWordData();
                    letters = GetLettersFromWord(question as LL_WordData);
                    foreach (var l in letters)
                    {
                        correctAnswers.Add(l);
                    }
                    letters = GetLettersNotContained(letters, 8);
                    foreach (var l in letters)
                    {
                        wrongAnswers.Add(l);
                    }
                    // QuestionPack creation
                    questionPack = new FindRightDataQuestionPack(question, wrongAnswers, correctAnswers);
                    break;
                case MiniGameCode.FastCrowd_words:
                    // Dummy logic for question creation
                    for (int i = 0; i < 4; i++)
                    {
                        LL_WordData correctWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
                        if (!CheckIfContains(correctAnswers, correctWord))
                            correctAnswers.Add(correctWord);
                        else
                            i--;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        LL_WordData wrongWord = AppManager.Instance.Teacher.GimmeAGoodWordData();
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
                    letters = GetLettersFromWord(question as LL_WordData);
                    foreach (var l in letters)
                    {
                        correctAnswers.Add(l);
                        break; //get alway first (only for test)
                    }
                    letters = GetLettersNotContained(letters, 3);
                    foreach (var l in letters)
                    {
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

        LL_WordData GetWord()
        {
            return AppManager.Instance.Teacher.GimmeAGoodWordData();
        }

        List<LL_WordData> GetWordsNotContained(List<LL_WordData> _WordsToAvoid, int _count)
        {
            List<LL_WordData> wordListToReturn = new List<LL_WordData>();
            for (int i = 0; i < _count; i++)
            {
                var word = AppManager.Instance.Teacher.GimmeAGoodWordData();

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
            foreach (var letterData in ArabicAlphabetHelper.LetterDataListFromWord(_word.Data.Arabic, AppManager.Instance.Letters))
            {
                letters.Add(letterData);
            }
            return letters;
        }

        List<LL_LetterData> GetLettersNotContained(List<LL_LetterData> _lettersToAvoid, int _count)
        {
            List<LL_LetterData> letterListToReturn = new List<LL_LetterData>();
            for (int i = 0; i < _count; i++)
            {
                var letter = AppManager.Instance.Teacher.GimmeARandomLetter();

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
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }


        static bool CheckIfContains(List<LL_LetterData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        static bool CheckIfContains(List<LL_WordData> list, ILivingLetterData letter)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
                if (list[i].Key == letter.Key)
                    return true;
            return false;
        }

        #endregion
    }
}