using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S.Db.Loader
{
    public class DatabaseManager : MonoBehaviour
    {
        public Database db;
        public DatabaseLoader DBLoader;

        public Text OutputText;
        public TextMeshProUGUI OutputTextArabic;

        public void ImportAll()
        {
            DBLoader.LoadDatabase();
            LogDataCounts();
        }

        public void LogDataCounts()
        {
            string output = "";
            output += ("N minigames: " + db.minigameTable.Count);
            output += ("\nN letters: " + db.letterTable.Count);
            output += ("\nN words: " + db.wordTable.Count);
            output += ("\nN play sessions: " + db.playSessionTable.Count);
            output += ("\nN assessments: " + db.assessmentTable.Count);
            output += ("\nN localizations: " + db.localizationTable.Count);
            output += ("\nN phrases: " + db.phraseTable.Count);
            output += ("\nN stages: " + db.stageTable.Count);
            output += ("\nN rewards: " + db.rewardTable.Count);
            Debug.Log(output);
            OutputText.text = output;
        }

        public void LogWord()
        {
            string output = db.localizationTable["assessment_start_A1"].Arabic;
            OutputTextArabic.text = ReverseText(ArabicFixer.Fix(output)); ;
            Debug.Log(output);
        }

        public void LogAllLetters()
        {
            string output = "";
            foreach (var letter in db.letterTable.Values) {
                output += (letter.Id) + "\n";
            }
            Debug.Log(output);
        }

        public void LogAllPlaySessions()
        {
            string output = "";
            foreach (var playsession in db.playSessionTable.Values) {
                output += (playsession.GetID()) + "\n";
            }
            Debug.Log(output);
        }

        public void LogAllLocalizations()
        {
            string output = "";
            foreach (var localization in db.localizationTable.Values) {
                output += localization.Id + ": " + localization.Arabic + "\n";
            }
            Debug.Log(output);
        }

        public void LogMiniGames()
        {
            string output = "";
            foreach (var data in db.minigameTable.Values) {
                if (data.Available) {
                    output += (data.ToString()) + "\n";
                }
            }
            Debug.Log(output);
            OutputText.text = output;
        }

        public void TestAccess()
        {
            string output = "";
            output += ("AlphabetSong has description: ");
            output += ("\n" + db.minigameTable["AlphabetSong"].Description);
            Debug.Log(output);
        }

        string ReverseText(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--) {
                reverse += cArray[i];
            }
            return reverse;
        }
    }
}
