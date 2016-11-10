using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S.Teacher
{
    public class LogInterpreter
    {
        /// <summary>
        /// Play result param
        /// </summary>
        public struct PlayResultParameters
        {
            public MiniGameCode miniGameCode;
            public PlaySkill skill;
            public float value;
        }

        // Useful data
        const int MIN_MOOD = 1;
        const int MAX_MOOD = 6;

        // References
        DatabaseManager db;

        public LogInterpreter(DatabaseManager db)
        {
            this.db = db;
        }

        public void LogMood(int mood)
        {
            float realMood = Mathf.InverseLerp(MIN_MOOD, MAX_MOOD, mood);
            var data = new LogMoodData(realMood);
            db.Insert(data);
        }

        public void LogInfo(string session, InfoEvent infoEvent, string parametersString = "")
        {
            var data = new LogInfoData(session, infoEvent, parametersString);
            db.Insert(data);
        }

        #region Play

        public void LogPlay(List<PlayResultParameters> parameters)
        {
            // ... @todo: implement

            // There will be 1 vote per-skill per-minigame whenever it is played
        }

        #endregion


        #region Learn

        /// <summary>
        /// General parameters used to define the learning result for each minigame instance
        /// </summary>
        public struct LearnResultParameters
        {
            public DbTables table;
            public string elementId;
            public int nCorrect;
            public int nWrong;
        }

        /// <summary>
        /// Specific rules per mini game
        /// </summary>
        public class MiniGameLearnRules
        {
            public enum VoteLogic
            {
                Threshold,
                SuccessRatio
            }

            public VoteLogic voteLogic;
            public float logicParameter;                // for example, success threshold 
            public float minigameVoteSkewOffset;        // takes into account that some minigames are skewed
            public float minigameImportanceWeight;      // takes into account that some minigames are more important on learning in respect to others

            public MiniGameLearnRules()
            {
                voteLogic = VoteLogic.SuccessRatio;
                logicParameter = 0f;
                minigameVoteSkewOffset = 0f;
                minigameImportanceWeight = 1f;
            }
        }

        public void LogLearn(MiniGameCode miniGameCode,  List<LearnResultParameters> resultsList)
        {
            var learnRules = GetLearnRules(miniGameCode);

            foreach (var result in resultsList)
            {
                float score = 0f;
                float successRatio = result.nCorrect * 1f / (result.nCorrect + result.nWrong);
                switch (learnRules.voteLogic)
                {
                    case MiniGameLearnRules.VoteLogic.Threshold:
                        // Uses a binary threshold
                        float threshold = learnRules.logicParameter;
                        score = successRatio > threshold ? 1f : -1f;
                        break;
                    case MiniGameLearnRules.VoteLogic.SuccessRatio:
                        // Uses directly the success ratio to drive the vote
                        score = Mathf.InverseLerp(-1f, 1f, successRatio);
                        break;
                }
                score *= learnRules.minigameImportanceWeight;
                score += learnRules.minigameVoteSkewOffset;

                // We also update the score data
                db.UpdateScoreData(result.table, result.elementId, score);
            }
        }

        private MiniGameLearnRules GetLearnRules(MiniGameCode code)
        {
            MiniGameLearnRules rules = new MiniGameLearnRules();
            switch (code)
            {
                case MiniGameCode.Balloons_letter:  // @todo: set correct ones per each minigame
                    rules.voteLogic = MiniGameLearnRules.VoteLogic.Threshold;
                    rules.logicParameter = 0.5f;
                    break;

                default:
                    rules.voteLogic = MiniGameLearnRules.VoteLogic.SuccessRatio;
                    break;
            }
            return rules;
        }

        #endregion

    }

}