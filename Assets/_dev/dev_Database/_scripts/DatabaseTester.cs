using UnityEngine;
using System.Collections;

namespace EA4S.Db.Loader
{
    public class DatabaseTester : MonoBehaviour
    {
        public Database db;

        void Start()
        {
            LogDataCounts();
            LogAllLetters();
            TestAccess();
        }

        public void LogDataCounts()
        {
            string s = "";
            s += ("N minigames: " + db.minigameTable.Count);
            s += ("\nN letters: " + db.letterTable.Count);
            s += ("\nN words: " + db.wordTable.Count);
            s += ("\nN play sessions: " + db.playSessionTable.Count);
            s += ("\nN assessments: " + db.assessmentTable.Count);
            s += ("\nN localizations: " + db.localizationTable.Count);
            s += ("\nN phrases: " + db.phraseTable.Count);
            s += ("\nN stages: " + db.stageTable.Count);
            s += ("\nN rewards: " + db.rewardTable.Count);
            Debug.Log(s);
        }

        public void LogAllLetters()
        {
            string s = "";
            foreach (var letter in db.letterTable.Values)
            {
                s +=  (letter.Id) + "\n";
            }
            Debug.Log(s);
        }

        public void TestAccess()
        {
            string s = "";
            s += ("AlphabetSong has description: ");
            s += ("\n" + db.minigameTable["AlphabetSong"].Description);
            Debug.Log(s);
        }

    }
}
