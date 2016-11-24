using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentAnturaController : MonoBehaviour, ITickable
    {
        public AnturaAnimationController antura { get; set; }

        static AssessmentAnturaController instance;
        public static AssessmentAnturaController Instance
        {
            get
            {
                return instance;
            }
        }

        private IAudioManager audioManager;

        private IEnumerator TutorialClicks()
        {
            clickEnabled = false;
            yield return TimeEngine.Wait( 0.6f);
            TutorialUI.Click( TutorialHelper.GetWorldPosition());
            yield return TimeEngine.Wait( 0.1f);
            AssessmentConfiguration.Instance.Context.GetAudioManager().PlaySound( Sfx.ThrowObj);
            yield return TimeEngine.Wait( 0.6f);
            clickEnabled = true;
        }

        void Awake()
        {
            instance = this;
            currentState = 0;
            audioManager = AssessmentConfiguration.Instance.Context.GetAudioManager();
        }

        void Start()
        {
            TimeEngine.AddTickable( this);
        }

        Action playPushAnturaSound;

        bool isAnimating = false;

        public void StartAnimation( Action soundCallback)
        {
            isAnimating = true;
            playPushAnturaSound = soundCallback;
            Coroutine.Start( CheckStateAndSetAnimation());
        }

        void OnMouseUp()
        {
            if(clickEnabled)
                AnturaPressed();
        }

        private bool clickEnabled = false;

        private void AnturaPressed()
        {
            currentTreshold += GainPerClick;
            if (currentState < 3)
            {
                var sound = audioManager.PlaySound( soundOnClick);
                sound.Volume = 0.5f;
            }
        }

        public ParticleSystem sleepingParticles { get; set; }
        public Transform anturaDestination { get; set; }

        public readonly float Treshold = 0.9f;
        public readonly float GainPerClick = 1f;
        public readonly Sfx soundOnClick = Sfx.ThrowObj;

        private float currentTreshold = 0;
        private float currentMaxTreshold = 0.9f;
        private int currentState;
        private int stateDelta;

        void OnDestroy()
        {
            TimeEngine.RemoveTickable( this);
        }

        void TurnAntura(float degrees)
        {
            transform.DORotate( new Vector3( 0, degrees, 0), 0.6f).SetEase( Ease.InOutSine);
        }

        IEnumerator CheckStateAndSetAnimation()
        {
            antura.State = AnturaAnimationStates.walking;

            yield return TimeEngine.Wait( 1.0f);

            yield return transform
                .DOMove( anturaCenter.position, 3.0f)
                .SetEase( Ease.InOutSine);

            yield return TimeEngine.Wait( 2.6f);
            sleepingParticles = Instantiate( sleepingParticles, paritclesPos) as ParticleSystem;
            sleepingParticles.transform.localPosition = Vector3.zero;
            antura.State = AnturaAnimationStates.sleeping;
            yield return TimeEngine.Wait( 2.1f);

            playPushAnturaSound();            
            yield return TimeEngine.Wait( 1.0f);
            Coroutine.Start(TutorialClicks());

            bool tutorialCoroutineEnabled = true;
            while (currentState < 3)
            {
                while (stateDelta == 0)
                    yield return null;

                if (stateDelta > 0)
                    IncreaseState();

                if (stateDelta < 0)
                    DecreaseState();

                var emission = sleepingParticles.emission;

                
                switch (currentState)
                {
                    case 0:
                        if (tutorialCoroutineEnabled)
                        {
                            Coroutine.Start(TutorialClicks());
                            tutorialCoroutineEnabled = false;
                        }
                        emission.enabled = true;
                        antura.State = AnturaAnimationStates.sleeping;
                        yield return TimeEngine.Wait( 0.3f);
                        PlayStateSound();
                        TurnAntura( -75f);
                        yield return TimeEngine.Wait( 0.3f);
                        break;

                    case 1:
                        tutorialCoroutineEnabled = true;
                        emission.enabled = false;
                        antura.State = AnturaAnimationStates.sitting;
                        yield return TimeEngine.Wait( 0.8f);
                        PlayStateSound();
                        yield return TimeEngine.Wait( 1.0f);
                        break;

                    case 2:
                        antura.DoShout(() => audioManager.PlaySound( Sfx.DogBarking));
                        PlayStateSound();
                        yield return TimeEngine.Wait( 1.5f);
                        break;

                    case 3:
                        audioManager.PlaySound( Sfx.Win);
                        antura.DoCharge( ()=> StartMoving());
                        TurnAntura( -65f);
                        break;

                    default:
                        break;
                }

                currentTreshold = currentMaxTreshold *0.99f;
                stateDelta = 0;
            }
        }

        internal bool IsAnimating()
        {
            return isAnimating;
        }

        private void StartMoving()
        {
            transform.DOMove( anturaDestination.position, 3.0f)
                .SetEase( Ease.InOutSine)
                .OnComplete( ()=> isAnimating = false);
        }

        private void PlayStateSound()
        {
            if (playSound)
                    audioManager.PlaySound( soundToPlay);

            playSound = false;
        }

        bool playSound = false;
        Sfx soundToPlay;
        internal Transform anturaCenter;
        internal Transform paritclesPos;

        private void DecreaseState()
        {
            soundToPlay = Sfx.LetterSad;
            playSound = true;
            stateDelta = 0;
            if (currentState == 1)
                Coroutine.Start( TutorialClicks());

            currentState--;
        }

        private void IncreaseState()
        {
            soundToPlay = Sfx.LetterHappy;
            playSound = true;
            stateDelta = 0;
            currentState++;
        }

        bool ITickable.Update( float deltaTime)
        {
            currentTreshold -= deltaTime;

            if (currentTreshold < 0)
                currentTreshold = 0;

            if (currentTreshold < 0 && currentState > 0)
            {
                currentTreshold = currentMaxTreshold;
                stateDelta--;
            }

            if (currentTreshold > currentMaxTreshold)
            {
                currentTreshold = currentMaxTreshold /2;
                stateDelta++;
            }

            return false;
        }
    }
}
