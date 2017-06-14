using System;
using System.Collections;
using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using EA4S.UI;
using UnityEngine;
using UnityEngine.UI;
using EA4S.Assessment;
using EA4S.Audio;
using EA4S.Database;
using System.Linq;
using EA4S.Core;
using EA4S.Helpers;

namespace EA4S.Teacher.Test
{
    /// <summary>
    /// Helper class to test DataBase contents, useful to pinpoint critical data.
    /// </summary>
    public class DataStatisticsTester : MonoBehaviour
    {
        private VocabularyHelper _vocabularyHelper;
        private DatabaseManager _databaseManager;
        private List<PlaySessionData> _playSessionDatas;
        private List<LetterData> _letterDatas;
        private List<WordData> _wordDatas;
        private List<PhraseData> _phraseDatas;

        //private LetterFilters _letterFilters;
        private WordFilters _wordFilters;

        void Awake()
        {
            _databaseManager = AppManager.Instance.DB;
            _vocabularyHelper = AppManager.Instance.VocabularyHelper;

            _playSessionDatas = _databaseManager.GetAllPlaySessionData();
            _letterDatas = _databaseManager.GetAllLetterData();
            _wordDatas = _databaseManager.GetAllWordData();
            _phraseDatas = _databaseManager.GetAllPhraseData();

            //_letterFilters = new LetterFilters();
            _wordFilters = new WordFilters();
        }


        [DeMethodButton("Letters Frequency")]
        public void DoLettersFrequency()
        {
            int threshold = 3;

            DoStatsList("Frequency of letters in words", _letterDatas,
                data => _vocabularyHelper.GetWordsWithLetter(_wordFilters, data.Id).Count < threshold,
                data => _vocabularyHelper.GetWordsWithLetter(_wordFilters, data.Id).Count.ToString());
        }
        

        [DeMethodButton("Word Length")]
        public void DoWordLength()
        {
            DoStatsList("Frequency of word length", _wordDatas,
                data => false,
                data => data.Letters.Length.ToString());
        }

        [DeMethodButton("Letter Audio")]
        public void DoLetterAudio()
        {
            DoStatsList("Letters with audio", _letterDatas,
                data => AudioManager.I.GetAudioClip(data) == null,
                data => AudioManager.I.GetAudioClip(data) == null ? "NO" : "ok");
        }

        [DeMethodButton("Word Audio")]
        public void DoWordAudio()
        {
            DoStatsList("Words with audio", _wordDatas,
                data => AudioManager.I.GetAudioClip(data) == null,
                data => AudioManager.I.GetAudioClip(data) == null ? "NO" : "ok");
        }

        [DeMethodButton("Phrase Audio")]
        public void DoPhraseAudio()
        {
            DoStatsList("Phrases with audio", _phraseDatas,
                data => AudioManager.I.GetAudioClip(data) == null,
                data => AudioManager.I.GetAudioClip(data) == null ? "NO" : "ok");
        }

        [DeMethodButton("Data matching in PS")]
        public void DoCheckWordsByPS()
        {
            string final_s = "Word & Letters matching in PS";

            List<WordData> observedWords = new List<WordData>();
            List<LetterData> observedLetters = new List<LetterData>();

            final_s += "\n\n Words without matching letters in their PS:";
            foreach (var playSessionData in _playSessionDatas)
            {
                // Get the letters & words in this PS
                var contents = AppManager.Instance.Teacher.VocabularyAi.GetContentsUpToJourneyPosition(playSessionData.GetJourneyPosition());
                var letters = contents.GetHashSet<LetterData>();
                var letterIds = letters.ToList().ConvertAll(x => x.Id);
                var words = contents.GetHashSet<WordData>();
                var wordIds = words.ToList().ConvertAll(x => x.Id);

                // Check whether there are words with letters that are not in the PS
                bool somethingWrong = false;
                string ps_s = "\n\nPS " + playSessionData.GetJourneyPosition();
                foreach (var word in words)
                {
                    if (observedWords.Contains(word)) continue;

                    if (!_vocabularyHelper.WordContainsAnyLetter(word, letterIds))
                    {
                        observedWords.Add(word);
                        ps_s += "\n" + word.Id + " has no matching letters!";
                        somethingWrong = true;
                    }
                }
                foreach (var letter in letters)
                {
                    if (observedLetters.Contains(letter)) continue;

                    if (!_vocabularyHelper.AnyWordContainsLetter(letter, wordIds))
                    {
                        observedLetters.Add(letter);
                        ps_s += "\n" + letter.Id + " has no matching words!";
                        somethingWrong = true;
                    }
                }
                if (somethingWrong)
                    final_s += ps_s;
            }

            Debug.Log(final_s);
        }

        [DeMethodButton("Data frequency in PS")]
        public void DoCheckDataFrequencyByPS()
        {
            string final_s = "Data frequency in PS";

            Dictionary<LetterData, int> observedLetters = new Dictionary<LetterData, int>();
            Dictionary<WordData, int> observedWords = new Dictionary<WordData, int>();
            Dictionary<PhraseData, int> observedPhrases = new Dictionary<PhraseData, int>();

            foreach (var d in AppManager.Instance.DB.GetAllLetterData())  observedLetters[d] = 0;
            foreach (var d in AppManager.Instance.DB.GetAllWordData()) observedWords[d] = 0;
            foreach (var d in AppManager.Instance.DB.GetAllPhraseData())  observedPhrases[d] = 0;

            foreach (var playSessionData in _playSessionDatas)
            {
                // Get the letters & words in this PS
                var contents = AppManager.Instance.Teacher.VocabularyAi.GetContentsUpToJourneyPosition(playSessionData.GetJourneyPosition());
                var letters = contents.GetHashSet<LetterData>();
                var words = contents.GetHashSet<WordData>();
                var phrases = contents.GetHashSet<PhraseData>();

                // Check whether there are words with letters that are not in the PS
                //string ps_s = "\n\nPS " + playSessionData.GetJourneyPosition();
                foreach (var d in words)
                    observedWords[d]++;
                foreach (var d in letters)
                    observedLetters[d]++;
                foreach (var d in phrases)
                    observedPhrases[d]++;
            }

            final_s += "\n\n Letters:";
            foreach (var d in AppManager.Instance.DB.GetAllLetterData()) if (observedLetters[d] == 0) final_s +=  "\n" + d.Id + ": " +  observedLetters[d];

            final_s += "\n\n Words:";
            foreach (var d in AppManager.Instance.DB.GetAllWordData()) if (observedWords[d] == 0) final_s += "\n" + d.Id + ": " + observedWords[d];

            final_s += "\n\n Phrases:";
            foreach (var d in AppManager.Instance.DB.GetAllPhraseData()) if (observedPhrases[d] == 0) final_s += "\n" + d.Id + ": " + observedPhrases[d];

            Debug.Log(final_s);
        }

        [DeMethodButton("Letters and words")]
        public void DoLettersAndWords()
        {
            DoStatsList("Letters & words", _letterDatas,
                data => true,
                data =>
                {
                    string s = "";
                    var words = _vocabularyHelper.GetWordsWithLetter(_wordFilters, data.Id);
                    foreach (var word in words)
                    {
                        s += word.Id +", ";
                    }
                    return s;
                });
        }


        #region Internals

        void DoStatsList<T>(string title, List<T> dataList, Predicate<T> problematicCheck, Func<T, string> valueFunc)
        {
            var problematicEntries = new List<string>();

            string data_s = "\n\n";
            foreach (var data in dataList)
            {
                bool isProblematic = problematicCheck(data);

                string entryS = string.Format("{0}: \t{1}", data, valueFunc(data));
                if (isProblematic)
                {
                    data_s += "\n" + "<color=red>" + entryS + "</color>";
                    problematicEntries.Add(data.ToString());
                }
                else
                {
                    data_s += "\n" + entryS;
                }
            }

            string final_s = "---- " + title;
            if (problematicEntries.Count == 0)
            {
                final_s += "\nAll is fine!\n";
            }
            else
            {
                final_s += "\nProblematic: (" + problematicEntries.Count + ") \n";
                foreach (var entry in problematicEntries)
                    final_s += "\n" + entry;
            }

            final_s += data_s;
            PrintReport(final_s);
        }

        void PrintReport(string s)
        {
            Debug.Log(s);
        }

        #endregion

    }

}
