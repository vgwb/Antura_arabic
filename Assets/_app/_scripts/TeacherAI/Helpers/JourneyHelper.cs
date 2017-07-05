using System.Collections.Generic;
using Antura.Core;
using Antura.Database;

namespace Antura.Teacher
{   
    // TODO refactor: merge JourneyPosition, JourneyHelper
    public class JourneyHelper
    {
        private DatabaseManager dbManager;

        public JourneyHelper(DatabaseManager _dbManager)
        {
            this.dbManager = _dbManager;
        }

        #region Utilities

        public bool IsAssessmentTime(JourneyPosition journeyPosition)
        {
            return journeyPosition.PlaySession == 100;
        }

        public PlaySessionData GetCurrentPlaySessionData()
        {
            var currentJourneyPosition = AppManager.I.Player.CurrentJourneyPosition;
            var currentPlaySessionId = AppManager.I.JourneyHelper.JourneyPositionToPlaySessionId(currentJourneyPosition);
            var playSessionData = AppManager.I.DB.GetPlaySessionDataById(currentPlaySessionId);
            return playSessionData;
        }

        #endregion

        #region JourneyPosition

        public string JourneyPositionToPlaySessionId(JourneyPosition journeyPosition)
        {
            return journeyPosition.Stage + "." + journeyPosition.LearningBlock + "." + journeyPosition.PlaySession;
        }

        public JourneyPosition PlaySessionIdToJourneyPosition(string psId)
        {
            var parts = psId.Split('.');
            return new JourneyPosition(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }

        public JourneyPosition FindNextJourneyPosition(JourneyPosition currentPosition)
        {
            var id = JourneyPositionToPlaySessionId(currentPosition);

            var allPlaySessions = dbManager.GetAllPlaySessionData();
            int next_id = -1;
            for (int ps_i = 0; ps_i < allPlaySessions.Count; ps_i++)
            {
                if (allPlaySessions[ps_i].Id == id)
                {
                    next_id = ps_i + 1;
                    break;
                }
            }

            // Check for the last session
            if (next_id == allPlaySessions.Count)
            {
                return null;
            }
            else
            {
                return PlaySessionIdToJourneyPosition(allPlaySessions[next_id].Id);
            }
        }

        public JourneyPosition GetInitialJourneyPosition()
        {
            var allPlaySessions = dbManager.GetAllPlaySessionData();
            return PlaySessionIdToJourneyPosition(allPlaySessions[0].Id);
        }

        public JourneyPosition GetFinalJourneyPosition()
        {
            var allPlaySessions = dbManager.GetAllPlaySessionData();
            return PlaySessionIdToJourneyPosition(allPlaySessions[allPlaySessions.Count - 1].Id);
        }

        public bool PlayerIsAtFinalJourneyPosition()
        {
            return AppManager.I.Player.CurrentJourneyPosition.Equals(GetFinalJourneyPosition());
        }

        public JourneyPosition GetMinimumJourneyPositionForMiniGame(MiniGameCode minigameCode)
        {
            var finalPos = AppManager.I.JourneyHelper.GetFinalJourneyPosition();
            int NBasePlaySession = 2;

            for (int s = 1; s <= finalPos.Stage; s++)
            {
                for (int lb = 1; lb <= finalPos.LearningBlock; lb++)
                {
                    for (int ps = 1; ps <= NBasePlaySession; ps++)
                    {
                        var jp = new JourneyPosition(s, lb, ps);
                        if (AppManager.I.DB.HasPlaySessionDataById(jp.ToStringId()))
                            if (AppManager.I.Teacher.CanMiniGameBePlayedAtPlaySession(jp, minigameCode))
                                return new JourneyPosition(s, lb, ps);
                    }
                    int assessmentCode = 100;
                    var jp_assessment = new JourneyPosition(s, lb, assessmentCode);

                    if (AppManager.I.DB.HasPlaySessionDataById(jp_assessment.ToStringId()))
                        if (AppManager.I.Teacher.CanMiniGameBePlayedAtPlaySession(jp_assessment, minigameCode))
                            return new JourneyPosition(s, lb, assessmentCode);
                }
            }
            return null;
        }
        #endregion

        #region Info getters

        public List<LearningBlockInfo> GetLearningBlockInfosForStage(int targetStage)
        {            
            // @todo: this could use the new ScoreHelper methods
            // @todo: probably move this to ScoreHelper
            List<LearningBlockInfo> learningBlockInfo_list = new List<LearningBlockInfo>();

            List<Database.LearningBlockData> learningBlockData_list = FindLearningBlockDataOfStage(targetStage);
            foreach (var learningBlockData in learningBlockData_list) {
                LearningBlockInfo info = new LearningBlockInfo();
                info.data = learningBlockData;
                info.score = 0; // 0 if not found otherwise in the next step
                learningBlockInfo_list.Add(info);
            }

            // Find all previous scores
            List<JourneyScoreData> scoreData_list = AppManager.I.ScoreHelper.GetCurrentScoreForLearningBlocksOfStage(targetStage);
            for (int i = 0; i < learningBlockInfo_list.Count; i++) {
                var info = learningBlockInfo_list[i];
                var scoreData = scoreData_list.Find(x => x.JourneyDataType == JourneyDataType.LearningBlock && x.ElementId == info.data.Id);
                info.score = scoreData.GetScore();
            }

            return learningBlockInfo_list;
        }

        /// <summary>
        /// Returns a list of all play session data with its current score for the given stage and learning block.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        public List<PlaySessionInfo> GetPlaySessionInfosForLearningBlock(int targetStage, int targetLearningBlock)
        {
            // @todo: this could use the new ScoreHelper methods
            // @todo: probably move this to ScoreHelper
            List<PlaySessionInfo> playSessionInfo_list = new List<PlaySessionInfo>();

            List<Database.PlaySessionData> playSessionData_list = FindPlaySessionDataOfStageAndLearningBlock(targetStage, targetLearningBlock);
            foreach (var playSessionData in playSessionData_list) {
                PlaySessionInfo info = new PlaySessionInfo();
                info.data = playSessionData;
                info.score = 0; // 0 if not found otherwise in the next step
                playSessionInfo_list.Add(info);
            }

            // Find all previous scores
            List<JourneyScoreData> scoreData_list = AppManager.I.ScoreHelper.GetCurrentScoreForPlaySessionsOfLearningBlock(targetStage, targetLearningBlock);
            for (int i = 0; i < playSessionInfo_list.Count; i++) {
                var info = playSessionInfo_list[i];
                var scoreData = scoreData_list.Find(x => x.JourneyDataType == JourneyDataType.PlaySession && x.ElementId == info.data.Id);
                info.score = scoreData.GetScore();
            }

            return playSessionInfo_list;
        }

        #endregion

        #region Stage -> LearningBlock -> PlaySession

        public List<Database.LearningBlockData> FindLearningBlockDataOfStage(int targetStage)
        {
            return dbManager.FindLearningBlockData(x => x.Stage == targetStage);
        }

        public List<Database.PlaySessionData> FindPlaySessionDataOfStageAndLearningBlock(int targetStage, int targetLearningBlock)
        {
            return dbManager.FindPlaySessionData(x => x.Stage == targetStage && x.LearningBlock == targetLearningBlock);
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="targetStage"></param>
        /// <returns></returns>
        public List<Database.PlaySessionData> FindPlaySessionDataOfStage(int targetStage)
        {
            return dbManager.FindPlaySessionData(x => x.Stage == targetStage);
        }

        #endregion

        public bool HasFinishedTheGame()
        {
            return PlayerIsAtFinalJourneyPosition();
        }
    }
}
