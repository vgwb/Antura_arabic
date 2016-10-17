using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Db
{
    [System.Serializable]
    public class MiniGameTable : SerializableDictionary<string, MiniGameData> { }

    [System.Serializable]
    public class LetterTable : SerializableDictionary<string, LetterData> { }

    [System.Serializable]
    public class WordTable : SerializableDictionary<string, WordData> { }

    [System.Serializable]
    public class PlaySessionTable : SerializableDictionary<string, PlaySessionData> { }

    [System.Serializable]
    public class AssessmentTable : SerializableDictionary<string, AssessmentData> { }

    [System.Serializable]
    public class LocalizationTable : SerializableDictionary<string, LocalizationData> { }

    [System.Serializable]
    public class PhraseTable : SerializableDictionary<string, PhraseData> { }

    [System.Serializable]
    public class StageTable : SerializableDictionary<string, StageData> { }

    [System.Serializable]
    public class RewardTable : SerializableDictionary<string, RewardData> { }

}
