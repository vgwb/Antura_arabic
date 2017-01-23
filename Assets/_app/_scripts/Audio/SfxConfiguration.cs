using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Audio
{
    [System.Serializable]
    public class SfxConfiguration
    {
        public Sfx sfx;
        public List<AudioClip> clips = new List<AudioClip>();
        public float volume = 1;
        public float randomPitchOffset = 0;

    }
}
