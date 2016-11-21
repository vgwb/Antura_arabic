using System;
using DG.DeAudio;
using UnityEngine;

namespace EA4S
{
    public class SampleAudioSource : IAudioSource
    {
        AudioClip clip;
        DeAudioSource source;
        DeAudioGroup group;

        bool paused = false;

        public bool IsPlaying
        {
            get
            {
                if (source.audioSource == null)
                    return false;

                return source.isPlaying;
            }
        }

        public bool Loop
        {
            get
            {
                return source.loop;
            }

            set
            {
                source.loop = value;
            }
        }

        public float Pitch
        {
            get
            {
                return source.pitch;
            }

            set
            {
                source.pitch = value;
            }
        }

        public float Volume
        {
            get
            {
                return source.volume;
            }

            set
            {
                source.volume = value;
            }
        }

        public float Duration {
            get
            {
                return source.duration;
            }
        }

        public float Position
        {
            get
            {
                return source.time;
            }
            set
            {
                source.time = value;
            }
        }

        public void Stop()
        {
            if (source != null)
                source.Stop();
        }

        public void Play()
        {
            if (paused)
            {
                paused = false;
                source.Resume();
            }
            else
            {
                var oldPitch = source.pitch;
                var oldVolume = source.volume;
                var oldLoop = source.loop;

                source = group.Play(clip);

                source.pitch = oldPitch;
                source.volume = oldVolume;
                source.loop = oldLoop;
            }
        }

        public void Pause()
        {
            if (source.Pause())
                paused = true;
        }

        public SampleAudioSource(DeAudioSource source, DeAudioGroup group)
        {
            this.source = source;
            this.group = group;
            this.clip = source.clip;
        }
    }
}
