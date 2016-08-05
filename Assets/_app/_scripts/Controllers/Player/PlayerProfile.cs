using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class PlayerProfile
    {
        public int AnturaCurrentPreset;

        public PlayerProfile() {
            Reset();
        }

        public void Reset()
        {
            AnturaCurrentPreset = 0;
        }
    }
}