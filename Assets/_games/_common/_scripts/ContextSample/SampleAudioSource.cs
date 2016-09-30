
using System;

namespace EA4S
{
    public class SampleAudioSource : IAudioSource
    {
        public bool IsPlaying
        {
            get
            {
                return false;
            }
        }

        public void Stop()
        {
            
        }
    }
}
