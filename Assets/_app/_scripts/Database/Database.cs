using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    public class Database : ScriptableObject
    {
        public List<MiniGameData> MiniGames;
        public List<LetterData> Letters;
        public List<WordData> Words;
        public List<PlaySessionData> PlaySessions;
        public List<AssessmentData> Assessments;
        public List<LocalizationData> Localizations;
        public List<PhraseData> Phrases;
        public List<StageData> Stages;
        public List<RewardData> Rewards;
    }
}