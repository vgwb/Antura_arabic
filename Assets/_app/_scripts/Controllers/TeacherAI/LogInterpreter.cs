using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S.Db;

namespace EA4S.Teacher
{
    public class LogInterpreter
    {
        // Mood comes with values from 1 to 6
        public void LogMood(int mood)
        {
            var db = AppManager.Instance.DB;
            int realMood = (int)Mathf.InverseLerp(1, 6, mood);
            var data = new LogMoodData(realMood);
            db.Insert(data);
        }

        public void LogInfo(string session, InfoEvent infoEvent, string parametersString = "")
        {
            var db = AppManager.Instance.DB;
            var data = new LogInfoData(session, infoEvent, parametersString);
            db.Insert(data);
        }

        public struct LearnResultParameters
        {
            public MiniGameCode miniGameCode;
            public string dataId;
            public int nCorrect;
            public int nWrong;
        }

        public void LogLearn(List<LearnResultParameters> parameters)
        {
            // ... @todo: implement subjective vote logic for each minigame
            float vote = parameters[0].nCorrect * 1f / (parameters[0].nCorrect + parameters[0].nWrong);

            // ... @todo: update score data
        }



        public struct PlayResultParameters
        {
            public MiniGameCode miniGameCode;
            public PlaySkill skill;
            public float value;
        }

        public void LogPlay(List<PlayResultParameters> parameters)
        {
            // ... @todo: implement

            // There will be 1 vote per-skill per-minigame whenever it is played

        }

    }

}