using UnityEngine;
using UnityEditor;

namespace EA4S.Db.Loader
{
    public class DatabaseLoader : MonoBehaviour
    {
        public DatabaseInputData inputData;
        public Database database;
        public bool verbose;

        #region Loading

        /// <summary>
        /// Load all database values from scriptable objects
        /// </summary>
        public void LoadDatabase()
        {
            if (verbose) Debug.Log("Loading data from JSON files...");
            LoadDataFrom(inputData);

            if (verbose) Debug.Log("Finished loading!");
        }

        /// <summary>
        /// Load input data and place it inside the database.
        /// </summary>
        /// <param name="inputData"></param>
        private void LoadDataFrom(DatabaseInputData inputData)
        {
            {
                Debug.Log("Loading Letters...");
                var adapter = new LetterParser();
                adapter.Parse(inputData.letterDataAsset.text, database, database.letterTable);
            }

            {
                // @note: depends on Letter
                Debug.Log("Loading Words...");
                var adapter = new WordParser();
                adapter.Parse(inputData.wordDataAsset.text, database, database.wordTable);
            }

            {
                Debug.Log("Loading Phrases...");
                var adapter = new PhraseParser();
                adapter.Parse(inputData.phraseDataAsset.text, database, database.phraseTable);
            }

            {
                Debug.Log("Loading Minigames...");
                var adapter = new MiniGameParser();
                adapter.Parse(inputData.minigameDataAsset.text, database, database.minigameTable);
            }

            {
                // @note: depends on Letter, Word, Phrase, Minigame
                Debug.Log("Loading PlaySessions...");
                var adapter = new PlaySessionParser();
                adapter.Parse(inputData.playSessionDataAsset.text, database, database.playSessionTable);
            }

            {
                Debug.Log("Loading Localization...");
                var adapter = new LocalizationParser();
                adapter.Parse(inputData.localizationDataAsset.text, database, database.localizationTable);
            }

            {
                Debug.Log("Loading Stages...");
                var adapter = new StageParser();
                adapter.Parse(inputData.stageDataAsset.text, database, database.stageTable);
            }

            // TODO: reward

            // Save database modifications
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
        #endregion

    }
}
