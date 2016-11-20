using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using EA4S.TestE;

namespace EA4S
{
    public class MiniMap : MonoBehaviour
    {
        [Header("Letter")]
        public GameObject letter;

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
        Quaternion rot;
        int numDot = 0;
        int numLearningBlock;
        void Awake()
        {
            posDots = new GameObject[28];
            for (numLearningBlock = 0; numLearningBlock < (posPines.Length - 1); numLearningBlock++) {
                pinLeft = posPines[numLearningBlock].position;
                pinRight = posPines[numLearningBlock + 1].position;

                CalculateStepsBetweenPines(pinLeft, pinRight);
            }
            pinLeft = posPines[0].position;
            pinRight = posPines[1].position;
        }

        List<Db.PlaySessionData> myList = new List<Db.PlaySessionData>();
        List<PlaySessionState> myListPlaySessionState = new List<PlaySessionState>();

        void Start()
        {
            /* FIRST CONTACT FEATURE */
            if (AppManager.Instance.Player.IsFirstContact()) {
                FirstContactBehaviour();
            }
            /* --------------------- */

            Debug.Log("MapManager PlaySession " + AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);
            Debug.Log("Learning Block " + AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock);
            //Debug.Log("LBlock " + AppManager.Instance.Teacher.GetMiniGamesForCurrentPlaySession());
            //Debug.Log("LBlock " + AppManager.Instance.GameSettin);

            myList = GetAllPlaySessionDataForStage(1);
            Debug.Log(myList.Count);
            //myListPlaySessionState = GetAllPlaySessionStateForStage(1);
            //Debug.Log(myListPlaySessionState[0].data.Description);
        }

        void Update()
        {
            // Remove this with First Contact Temp Behaviour
            UpdateTimer();
            //Debug.Log("MapManager PlaySession " + AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);
            //Debug.Log("Learning Block " + AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock);
        }

        #region First Contact Session        
        /// <summary>
        /// Firsts the contact behaviour.
        /// Put Here logic for first contact only situations.
        /// </summary>
        void FirstContactBehaviour()
        {

            if (AppManager.Instance.Player.IsFirstContact(1)) {
                // First contact step 1:
                #region Temp Behaviour (to be deleted)
                countDown.Start();
                #endregion
                // ..and set first contact done.
                AppManager.Instance.Player.FirstContactPassed();
                Debug.Log("First Contact Step1 finished! Go to Antura Space!");
            } else if (AppManager.Instance.Player.IsFirstContact(2)) {
                // First contact step 2:

                // ..and set first contact done.
                AppManager.Instance.Player.FirstContactPassed(2);
                Debug.Log("First Contact Step2 finished! Good Luck!");
            }

        }
        #region Temp Behaviour (to be deleted)
        CountdownTimer countDown = new CountdownTimer(5);
        void OnEnable() { countDown.onTimesUp += CountDown_onTimesUp; }
        void OnDisable() { countDown.onTimesUp -= CountDown_onTimesUp; }
        private void CountDown_onTimesUp() { AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Rewards"); }
        private void UpdateTimer() { countDown.Update(Time.deltaTime); }
        #endregion

        #endregion

        void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2)
        {
            float step = 1f / (numStepsBetweenPines + 1);
            for (float perc = step; perc < 1f; perc += step) {
                Vector3 v = Vector3.Lerp(p1, p2, perc);

                rot.eulerAngles = new Vector3(90, 0, 0);
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
                posDots[numDot] = dotGo;
                numDot++;
            }
        }

        private class PlaySessionState
        {
            public Db.PlaySessionData data;
            public float score;

            public PlaySessionState(Db.PlaySessionData _data, float _score)
            {
                this.data = _data;
                this.score = _score;
            }
        }

        /// <summary>
        /// Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            List<Db.ScoreData> scoreData_list = AppManager.Instance.Teacher.scoreHelper.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            List<PlaySessionState> playSessionState_list = new List<PlaySessionState>();
            for (int i = 0; i < scoreData_list.Count; i++) {
                var data = AppManager.Instance.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Score));
            }

            return playSessionState_list;
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<Db.PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.Instance.DB.FindPlaySessionData(x => x.Stage == _stage);
        }
        public void Play()
        {
            AppManager.Instance.Teacher.InitialiseCurrentPlaySession();   // This must becalled before the games selector is loaded

            if (AppManager.Instance.IsAssessmentTime)
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Assessment");
            else
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_GamesSelector");
        }
    }
}
