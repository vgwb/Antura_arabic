using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EA4S.Db.Loader
{
    public class DatabaseLoader : MonoBehaviour
    {
        #region Utility Classes
        // Utility classes needed to load json arrays as root (as created by google2u)
        [System.Serializable]
        private struct MiniGameDataArray { public MiniGameData[] objects; }
        [System.Serializable]
        private struct LetterDataArray { public LetterData[] objects; }
        [System.Serializable]
        private struct WordDataArray { public WordData[] objects; }
        [System.Serializable]
        private struct PlaySessionDataArray { public PlaySessionData[] objects; }
        [System.Serializable]
        private struct AssessmentDataArray { public AssessmentData[] objects; }
        [System.Serializable]
        private struct LocalizationDataArray { public LocalizationData[] objects; }
        [System.Serializable]
        private struct PhraseDataArray { public PhraseData[] objects; }
        [System.Serializable]
        private struct StageDataArray { public StageData[] objects; }
        [System.Serializable]
        private struct RewardDataArray { public RewardData[] objects; }
        #endregion

        // Fields
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

            if (verbose) Debug.Log("Validating data..");
            if (!ValidateAllData())
            {
                Debug.LogError("Data did not pass validation! Check previous errors!");
            }

            if (verbose) Debug.Log("Finished loading!");
        }

        /// <summary>
        /// Load input data and place it inside the database.
        /// </summary>
        /// <param name="inputData"></param>
        private void LoadDataFrom(DatabaseInputData inputData)
        {
            // @note: cannot use Linq to move from array to dict as ToDictionary will not convert to our own serializable dicts

            var minigameArray = LoadJson<MiniGameDataArray>(inputData.minigameDataAsset.text);
            foreach (var obj in minigameArray.objects)
                database.minigameTable[obj.GetID()] = obj;

            var letterArray = LoadJson<LetterDataArray>(inputData.letterDataAsset.text);
            foreach (var obj in letterArray.objects)
                database.letterTable[obj.GetID()] = obj;

            var wordArray = LoadJson<WordDataArray>(inputData.wordDataAsset.text);
            foreach (var obj in wordArray.objects)
                database.wordTable[obj.GetID()] = obj;

            var playSessionArray = LoadJson<PlaySessionDataArray>(inputData.playSessionDataAsset.text);
            foreach (var obj in playSessionArray.objects)
            {
                if (database.playSessionTable.ContainsKey(obj.GetID()))
                {
                    Debug.LogError("PlaySessionData: multiple data with ID " + obj.GetID() + " found");
                }
                database.playSessionTable[obj.GetID()] = obj;
            }

            var localizationArray = LoadJson<LocalizationDataArray>(inputData.localizationDataAsset.text);
            foreach (var obj in localizationArray.objects)
                database.localizationTable[obj.GetID()] = obj;

            var phraseArray = LoadJson<PhraseDataArray>(inputData.phraseDataAsset.text);
            foreach (var obj in phraseArray.objects)
                database.phraseTable[obj.GetID()] = obj;

            var stageArray = LoadJson<StageDataArray>(inputData.stageDataAsset.text);
            foreach (var obj in stageArray.objects)
                database.stageTable[obj.GetID()] = obj;

            var rewardArray = LoadJson<RewardDataArray>(inputData.rewardDataAsset.text);
            foreach (var obj in rewardArray.objects)
                database.rewardTable[obj.GetID()] = obj;

            // Save database modifications
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region Validation

        private bool ValidateAllData()
        {
            bool passedValidation = true;

            foreach (var data in database.minigameTable.Values)
            {
                if (!ValidateMiniGame(data))
                {
                    passedValidation = false;
                }
            }

            foreach (var data in database.letterTable.Values)
            {
                if (!ValidateLetter(data))
                {
                    passedValidation = false;
                }
            }

            return passedValidation;
        }

        private bool ValidateMiniGame(MiniGameData data)
        {
            // Validate Id
            try
            {
                MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Id);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError("MiniGameData (ID ?): " + "field Id is '" + data.Id + "', not available in the enum values.");
                return false;
            }

            // Validate Parent
            try
            {
                if (data.Parent != "")
                {
                    MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Parent);
                }
            }
            catch (System.ArgumentException)
            {
                Debug.LogError("MiniGameData (ID " + data.GetID() + "): " + "field Parent is '" + data.Parent + "', not available in the enum values.");
                return false;
            }

            // Set derived values too
            data.Available = data.Status == "active";
            data.MiniGameCode = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Id);
            return true;
        }

        private bool ValidateLetter(LetterData data)
        {
            // Nothing to validate for letters
            return true;
        }

        #endregion

        #region Json Utilities

        private T LoadJson<T>(string json) {
            // @note: the wrapper is required for google2u array roots
            // @note:  direct array Json serialization is not supported yet by JsonUtility: https://docs.unity3d.com/Manual/JSONSerialization.html (see Supported Types)
            var wrapped_json = inputData.minigameDataAsset.text;
            wrapped_json = "{\"objects\":" + wrapped_json + "}";
            T dataArray = JsonUtility.FromJson<T>(wrapped_json);
            return dataArray;
        }
        
        #endregion
    }
}
