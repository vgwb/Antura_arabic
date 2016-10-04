
using System;

namespace EA4S
{
    public class SampleAudioSource : IAudioSource
    {
        Sfx? sfx;

        public bool IsPlaying
        {
            get
            {
                return false;
            }
        }

        public void Stop()
        {
            if (sfx.HasValue)
                AudioManager.I.StopSfx(sfx.Value);
        }

        public SampleAudioSource(Sfx? sfx)
        {
            this.sfx = sfx;
        }
    }
}
