using UnityEngine;
using System.Collections.Generic;
using EA4S.Core;

namespace EA4S.Map
{
    /// <summary>
    /// Controls the generation of the Map's pins and dots based on the journey's progression data.
    /// </summary>
    // refactor: rename to StageMap or similar, as there is no mini-map in the game.
    public class MiniMap : MonoBehaviour
    {
        [Header("Letter")]
        public GameObject letter;

        [Header("NumberStage")]
        public int numberStage;

        [Header("Pines")]
        public Transform[] posPines;
        public int numStepsBetweenPines;

        [Header("Ropes")]
        public GameObject[] ropes;

        [Header("Steps")]
        public GameObject dot;
        public GameObject[] posDots;
        public GameObject stepsParent;
        public Vector3 pinLeft, pinRight;
        public int posMax;
        public bool isAvailableTheWholeMap;
        int numDot = 0;
        int numLearningBlock;

        public void CalculateSettingsStageMap()
        {
            //  posDots = new GameObject[28];
            for (numLearningBlock = 0; numLearningBlock < (posPines.Length - 1); numLearningBlock++) {
                pinLeft = posPines[numLearningBlock].transform.position;
                pinRight = posPines[numLearningBlock + 1].transform.position;
                CalculateStepsBetweenPines(pinLeft, pinRight);
            }
            pinLeft = posPines[0].position;
            pinRight = posPines[1].position;

            if (!isAvailableTheWholeMap) CalculatePlaySessionAvailables();
            CalculatePin_RopeAvailable();
        }
        int p;
        Vector3 v;
        void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2)
        {
            // Debug.Log("DISTANCE "+Vector3.Distance(p1, p2));
            //float step = 1f / (numStepsBetweenPines + 1);
            //for (float perc = step; perc < 1f; perc += step) {
            // Vector3 v = Vector3.Lerp(p1, p2, perc);
            float d = Vector3.Distance(p1, p2);
            float x = ((d - 5) - 2) / 3;
            for (p = 1; p < 3; p++) {
                if (p == 1) {
                    v = (x + 0.5f + 2.5f) * Vector3.Normalize(p2 - p1) + p1;
                } else v = (2 * x + 1.5f + 2.5f) * Vector3.Normalize(p2 - p1) + p1;


                var rot = Quaternion.Euler(90, 0, 0);
                GameObject dotGo;
                dotGo = Instantiate(dot, v, rot) as GameObject;
                dotGo.GetComponent<Dot>().learningBlockActual = numLearningBlock + 1;
                dotGo.GetComponent<Dot>().pos = numDot;
                if (numLearningBlock < posPines.Length - 1) {
                    ropes[numLearningBlock].GetComponent<Rope>().dots.Add(dotGo);
                    ropes[numLearningBlock].GetComponent<Rope>().learningBlockRope = numLearningBlock + 1;
                }

                if (numDot % 2 == 0)
                    dotGo.GetComponent<Dot>().playSessionActual = 1;
                else
                    dotGo.GetComponent<Dot>().playSessionActual = 2;
                dotGo.transform.parent = stepsParent.transform;
                if (!isAvailableTheWholeMap) dotGo.SetActive(false);
                posDots[numDot] = dotGo;
                numDot++;
            }
        }
        void CalculatePlaySessionAvailables()
        {
            int l = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
            int p = AppManager.I.Player.MaxJourneyPosition.PlaySession;
            int m;
            if (p == 1) m = (l * 2) - 1;
            else m = l * 2;
            posMax = m;
            for (int i = 0; i < m; i++) {
                posDots[i].SetActive(true);
            }
        }
        void CalculatePin_RopeAvailable()
        {
            if (isAvailableTheWholeMap) {
                posMax = posDots.Length;
                for (int i = 1; i < (posPines.Length - 1); i++) {
                    posPines[i].tag = "Pin";
                    posPines[i].GetComponent<MapPin>().unlocked = true;
                    ropes[i].transform.GetChild(0).tag = "Rope";
                }
                posPines[posPines.Length - 1].tag = "Pin";
                posPines[posPines.Length - 1].GetComponent<MapPin>().unlocked = true;
            } else {
                int l = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                int p = AppManager.I.Player.MaxJourneyPosition.PlaySession;
                int m;
                if (p == 100) m = l;
                else m = l - 1;
                for (int i = 1; i < (m + 1); i++) {
                    posPines[i].tag = "Pin";
                    posPines[i].GetComponent<MapPin>().unlocked = true;
                    if ((i == m) && (p == 100))
                        return;
                    else
                        ropes[i].transform.GetChild(0).tag = "Rope";
                }
            }
        }

        private class PlaySessionState
        {
            public Database.PlaySessionData data;
            public float score;

            public PlaySessionState(Database.PlaySessionData _data, float _score)
            {
                this.data = _data;
                this.score = _score;
            }
        }

        // refactor: these methods are not called at all, but are needed by the Map to show the state of a play session. The map should be improved to use these.

        /// <summary>
        /// Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            List<Database.JourneyScoreData> scoreData_list = AppManager.I.ScoreHelper.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            List<PlaySessionState> playSessionState_list = new List<PlaySessionState>();
            for (int i = 0; i < scoreData_list.Count; i++) {
                var data = AppManager.I.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Stars));
            }

            return playSessionState_list;
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<Database.PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.I.DB.FindPlaySessionData(x => x.Stage == _stage);
        }
        public void Play()
        {
            // refactor: move this initalisation to a better place, maybe inside the MiniGameLauncher.
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}