using System;
using UnityEngine.UI;
using ModularFramework.Modules;

namespace EA4S {

    [Serializable]
    public class PlayerProfile : IPlayerProfile {

        public string Key { get; set; }
        public int Id;
        public int AvatarId;
        public int Age;
        public string Name;

        // Mood (1 to 5 indicators)
        public float MainMood = 3f;
        public float Impatient = 3f;
        public float Impulsive = 3f;
        public float Genius = 3f;
        public float Bored = 3f;
        public float Collector = 3f;
        public float Frustrated = 3f;

        // PlaySkills
        public float Precision;
        public float Reaction;
        public float Memory;
        public float Logic;
        public float Rhythm;
        public float Musicality;
        public float Sight;
        
    }
}