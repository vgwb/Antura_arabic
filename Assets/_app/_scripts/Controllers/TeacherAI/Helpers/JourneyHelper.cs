using System.Collections.Generic;

namespace EA4S.Teacher
{
    #region Info wrappers

    public class LearningBlockInfo
    {
        public Db.LearningBlockData data;
        public float score;
        public List<PlaySessionInfo> playSessions = new List<PlaySessionInfo>();
    }

    public class PlaySessionInfo
    {
        public Db.PlaySessionData data;
        public float score;
    }

    #endregion

    public class JourneyHelper
    {
        private DatabaseManager dbManager;
        private TeacherAI teacher;

        public JourneyHelper(DatabaseManager _dbManager, TeacherAI _teacher)
        {
            this.dbManager = _dbManager;
            this.teacher = _teacher;
        }


        #region Info getters

        public List<LearningBlockInfo> GetLearningBlockInfosForStage(int targetStage)
        {
            List<LearningBlockInfo> learningBlockInfo_list = new List<LearningBlockInfo>();

            List<Db.LearningBlockData> learningBlockData_list = FindLearningBlockDataOfStage(targetStage);
            foreach (var learningBlockData in learningBlockData_list) {
                LearningBlockInfo info = new LearningBlockInfo();
                info.data = learningBlockData;
                info.score = 0; // 0 if not found otherwise in the next step
                learningBlockInfo_list.Add(info);
            }

            // Find all previous scores
            List<Db.ScoreData> scoreData_list = teacher.scoreHelper.GetCurrentScoreForLearningBlocksOfStage(targetStage);
            for (int i = 0; i < learningBlockInfo_list.Count; i++) {
                var info = learningBlockInfo_list[i];
                var scoreData = scoreData_list.Find(x => x.TableName == typeof(Db.LearningBlockData).Name && x.ElementId == info.data.Id);
                info.score = scoreData.Score;
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
            List<PlaySessionInfo> playSessionInfo_list = new List<PlaySessionInfo>();

            // @todo: place this Find ps -> lb -> stage somewhere else where it is easier to find (maybe the dbManager itself? or this journeyHelper instead?)
            List<Db.PlaySessionData> playSessionData_list = FindPlaySessionDataOfStageAndLearningBlock(targetStage, targetLearningBlock);
            foreach (var playSessionData in playSessionData_list) {
                PlaySessionInfo info = new PlaySessionInfo();
                info.data = playSessionData;
                info.score = 0; // 0 if not found otherwise in the next step
                playSessionInfo_list.Add(info);
            }

            // Find all previous scores
            List<Db.ScoreData> scoreData_list = teacher.scoreHelper.GetLearningBlockScores(targetStage, targetLearningBlock);
            for (int i = 0; i < playSessionInfo_list.Count; i++) {
                var info = playSessionInfo_list[i];
                var scoreData = scoreData_list.Find(x => x.TableName == typeof(Db.PlaySessionData).Name && x.ElementId == info.data.Id);
                info.score = scoreData.Score;
            }

            return playSessionInfo_list;
        }

        #endregion

        #region Stage -> LearningBlock -> PlaySession

        public List<Db.LearningBlockData> FindLearningBlockDataOfStage(int targetStage)
        {
            return dbManager.FindLearningBlockData(x => x.Stage == targetStage);
        }

        public List<Db.PlaySessionData> FindPlaySessionDataOfStageAndLearningBlock(int targetStage, int targetLearningBlock)
        {
            return dbManager.FindPlaySessionData(x => x.Stage == targetStage && x.LearningBlock == targetLearningBlock);
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="targetStage"></param>
        /// <returns></returns>
        public List<Db.PlaySessionData> FindPlaySessionDataOfStage(int targetStage)
        {
            return dbManager.FindPlaySessionData(x => x.Stage == targetStage);
        }

        #endregion

    }
}
