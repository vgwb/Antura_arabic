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

        // Psyco (1 to 5 indicators)
        public int MainMood = 3;
        public int Impatient = 3;
        public int Impulsive = 3;
        public int Genius = 3;
        public int Bored = 3;
        public int Collector = 3;
        public int Frustrated = 3;


        // Skills
        public float Precision;
        public float Reaction;
        public float Memory;
        public float Logic;
        public float Rhythm;
        public float Musicality;
        public float Sight;
        
    }
}