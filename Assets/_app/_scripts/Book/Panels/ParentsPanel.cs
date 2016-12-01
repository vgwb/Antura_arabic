using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{
    public class ParentsPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        //public GameObject LearningBlockItemPrefab;

        [Header("References")]
        //public GameObject ElementsContainer;
        public TextRender ScoreText;

        void OnEnable()
        {
            InitUI();
        }

        void InitUI()
        {


        }

        public void OnSuperDogMode()
        {
            GlobalUI.ShowPrompt(true, "go super dog?", GoSuperDogMode);
        }

        void GoSuperDogMode()
        {
            Debug.Log("Super Dog Mode enabled");
            SetJourneyAtMaximum();
            StartCoroutine(PopulateDatabaseWithUsefulDataCO());
        }

        public void OnDeleteProfile()
        {
            GlobalUI.ShowPrompt(true, "delete this profile?", GoDeleteProfile);
        }

        void GoDeleteProfile()
        {
            Debug.Log("YEAH!");
        }

        public void OnExportData()
        {
            GlobalUI.ShowPrompt(true, "Export alla Database?", GoExportData);
        }

        void GoExportData()
        {
            Debug.Log("YEAH!");
        }


        #region Super Dog Helpers

        private void SetJourneyAtMaximum()
        {
            var allPlaySessionInfos = AppManager.I.Teacher.scoreHelper.GetAllPlaySessionInfo();
            var secondToLastPS = allPlaySessionInfos[allPlaySessionInfos.Count - 1].data.Id;    // @note: we use the second-to-last because the last gives an error with the map logic
            var maxPossibleJourneyPosition = AppManager.I.Teacher.journeyHelper.PlaySessionIdToJourneyPosition(secondToLastPS);
            AppManager.I.Player.SetMaxJourneyPosition(maxPossibleJourneyPosition, true);
        }

        private System.Collections.IEnumerator PopulateDatabaseWithUsefulDataCO()
        {
            var logAi = AppManager.I.Teacher.logAI;
            var fakeAppSession = LogManager.I.Session;

            // Enable cheat mode
            AppManager.I.GameSettings.CheatSuperDogMode = true;

            // Add some mood data
            int nMoodData = 15;
            for (int i = 0; i < nMoodData; i++)
            {
                logAi.LogMood(Random.Range(AppConstants.minimumMoodValue, AppConstants.maximumMoodValue + 1));
                Debug.Log("Add mood " + i);
                yield return null;
            }

            // Add scores for all play sessions
            var allPlaySessionInfos = AppManager.I.Teacher.scoreHelper.GetAllPlaySessionInfo();
            for (int i = 0; i < allPlaySessionInfos.Count; i++)
            {
                logAi.LogPlaySessionScore(allPlaySessionInfos[i].data.Id, Random.Range(1, 4));
                Debug.Log("Add play session score " + i);
                yield return null;
            }

            // Add scores for all minigames
            var allMiniGameInfo = AppManager.I.Teacher.scoreHelper.GetAllMiniGameInfo();
            for (int i = 0; i < allMiniGameInfo.Count; i++)
            {
                logAi.LogMiniGameScore(allMiniGameInfo[i].data.Code, Random.Range(1, 4));
                Debug.Log("Add minigame score " + i);
                yield return null;
            }

            // Add scores for some learning data (words/letters/phrases)
            /*var maxPlaySession = AppManager.I.Player.MaxJourneyPosition.ToString();
            var allWordInfo = AppManager.I.Teacher.scoreHelper.GetAllWordInfo();
            for (int i = 0; i < allWordInfo.Count; i++)
            {
                if (Random.value < 0.3f)
                {
                    var resultsList = new List<Teacher.LogAI.LearnResultParameters>();
                    var newResult = new Teacher.LogAI.LearnResultParameters();
                    newResult.elementId = allWordInfo[i].data.Id;
                    newResult.table = DbTables.Words;
                    newResult.nCorrect = Random.Range(1,5);
                    newResult.nWrong = Random.Range(1, 5);
                    resultsList.Add(newResult);
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterShape, resultsList);
                }
            }
            var allLetterInfo = AppManager.I.Teacher.scoreHelper.GetAllLetterInfo();
            for (int i = 0; i < allLetterInfo.Count; i++)
            {
                if (Random.value < 0.3f)
                {
                    var resultsList = new List<Teacher.LogAI.LearnResultParameters>();
                    var newResult = new Teacher.LogAI.LearnResultParameters();
                    newResult.elementId = allLetterInfo[i].data.Id;
                    newResult.table = DbTables.Letters;
                    newResult.nCorrect = Random.Range(1, 5);
                    newResult.nWrong = Random.Range(1, 5);
                    resultsList.Add(newResult);
                    logAi.LogLearn(fakeAppSession, maxPlaySession, MiniGameCode.Assessment_LetterShape, resultsList);
                }
            }*/

            // Force update of graphs
            FindObjectOfType<GraphMood>().OnEnable();

            Debug.Log("Finished dog mode additions");
        }


        #endregion
    }
}