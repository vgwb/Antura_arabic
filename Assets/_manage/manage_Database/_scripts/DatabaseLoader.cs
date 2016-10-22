#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace EA4S.Db.Management
{
    public class DatabaseLoader : MonoBehaviour
    {
        public DatabaseInputData inputData;
        private Database database;
        public bool verbose;

        #region Loading

        /// <summary>
        /// Load all database values from scriptable objects
        /// </summary>
        public void LoadDatabase()
        {
            if (verbose) Debug.Log("Loading data from JSON files...");
            this.database = Resources.Load<Database>("EA4S.Database");

            LoadDataFrom(inputData);

            if (verbose) Debug.Log("Finished loading!");
        }

        /// <summary>
        /// Load input data and place it inside the database.
        /// </summary>
        /// <param name="DBInputData"></param>
        private void LoadDataFrom(DatabaseInputData DBInputData)
        {
            {
                Debug.Log("Loading Letters...");
                var adapter = new LetterParser();
                adapter.Parse(DBInputData.letterDataAsset.text, database, database.GetLetterTable());
            }

            {
                // @note: depends on Letter
                Debug.Log("Loading Words...");
                var adapter = new WordParser();
                adapter.Parse(DBInputData.wordDataAsset.text, database, database.GetWordTable());
            }

            {
                Debug.Log("Loading Phrases...");
                var adapter = new PhraseParser();
                adapter.Parse(DBInputData.phraseDataAsset.text, database, database.GetPhraseTable());
            }

            {
                Debug.Log("Loading Minigames...");
                var adapter = new MiniGameParser();
                adapter.Parse(DBInputData.minigameDataAsset.text, database, database.GetMiniGameTable());
            }

            {
                // @note: depends on Letter, Word, Phrase, Minigame
                Debug.Log("Loading PlaySessions...");
                var adapter = new PlaySessionParser();
                adapter.Parse(DBInputData.playSessionDataAsset.text, database, database.GetPlaySessionTable());
            }

            {
                Debug.Log("Loading Localization...");
                var adapter = new LocalizationParser();
                adapter.Parse(DBInputData.localizationDataAsset.text, database, database.GetLocalizationTable());
            }

            {
                Debug.Log("Loading Stages...");
                var adapter = new StageParser();
                adapter.Parse(DBInputData.stageDataAsset.text, database, database.GetStageTable());
            }

            {
                Debug.Log("Loading Rewards...");
                var adapter = new RewardParser();
                adapter.Parse(DBInputData.rewardDataAsset.text, database, database.GetRewardTable());
            }

            {
                Debug.Log("Loading Localization...");
                var adapter = new LocalizationParser();
                adapter.Parse(DBInputData.localizationDataAsset.text, database, database.GetLocalizationTable());
            }

            // Save database modifications
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
        #endregion

    }
}
#endif