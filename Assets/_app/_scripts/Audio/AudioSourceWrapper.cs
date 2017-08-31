using Antura.Minigames;
using DG.DeAudio;
using UnityEngine;

namespace Antura.Audio
{
    public class AudioSourceWrapper : IAudioSource
    {
        public DeAudioGroup Group { get; private set; }

        AudioClip clip;
        DeAudioSource source;
        AudioManager manager;

        bool paused = false;

        public bool IsPlaying
        {
            get {
                if (source == null || source.audioSource == null)
                    return false;

                return source.isPlaying;
            }
        }

        bool loop;

        public bool Loop
        {
            get { return loop; }

            set {
                loop = value;

                if (source != null)
                    source.loop = value;
            }
        }

        float pitch;

        public float Pitch
        {
            get { return pitch; }

            set {
                pitch = value;

                if (source != null)
                    source.pitch = value;
            }
        }

        float volume;

        public float Volume
        {
            get { return volume; }

            set {
                volume = value;

                if (source != null)
                    source.volume = value;
            }
        }

        float duration;

        public float Duration
        {
            get { return duration; }
        }

        public float Position
        {
            get {
                if (source == null || source.audioSource == null)
                    return 0;

                return source.time;
            }
            set {
                if (source != null)
                    source.time = value;
            }
        }

        public void Stop()
        {
            if (source != null && source.audioSource != null) {
                source.loop = false;
                source.Stop();
            }

            source = null;
            paused = false;
        }

        public void Play()
        {
            if (paused && source != null) {
                source.Resume();
            } else {
                paused = false;

                if (source != null) {
                    source.Stop();
                }

                source = Group.Play(clip);
                source.locked = true;

                source.pitch = pitch;
                source.volume = volume;
                source.loop = loop;
                manager.OnAudioStarted(this);
            }
        }

        public void Pause()
        {
            if (source != null && source.Pause()) {
                paused = true;
            }
        }

        public bool Update()
        {
            if (source != null) {
                if (!source.isPlaying && !source.isPaused && source.time == 0 && !manager.IsAppPaused) {
                    source.Stop();
                    source.locked = false;
                    source = null;
                    return true;
                }
            }
            return false;
        }

        public AudioSourceWrapper(DeAudioSource source, DeAudioGroup group, AudioManager manager)
        {
            this.source = source;
            this.Group = group;
            this.manager = manager;
            this.clip = source.clip;
            duration = source.duration;
            loop = source.loop;
            volume = source.volume;
            pitch = source.pitch;

            manager.OnAudioStarted(this);
        }
    }
}