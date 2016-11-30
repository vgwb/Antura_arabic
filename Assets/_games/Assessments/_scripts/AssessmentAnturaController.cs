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
            idleTime = 0;
            clickEnabled = false;
            yield return TimeEngine.Wait( 0.4f);
            TutorialUI.Click( TutorialHelper.GetWorldPosition());
            yield return TimeEngine.Wait( 0.1f);
            AssessmentConfiguration.Instance.Context.GetAudioManager().PlaySound( Sfx.UIPopup);
            yield return TimeEngine.Wait( 0.2f);
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
        Action playGoneSound;
        bool isAnimating = false;

        public void StartAnimation( Action pushSound, Action goneSound)
        {
            isAnimating = true;
            playPushAnturaSound = pushSound;
            playGoneSound = goneSound;
            Coroutine.Start( CheckStateAndSetAnimation());
        }

        void OnMouseUp()
        {
            idleTime = 0;
            if (clickEnabled)
                AnturaPressed();
        }

        private bool clickEnabled = false;

        private bool soundPlayed = false;
        private void AnturaPressed()
        {
            currentTreshold += GainPerClick;
            if (currentState < 3)
            {
                if (soundPlayed == false)
                {
                    var sound = audioManager.PlaySound( Sfx.UIPopup);
                    soundPlayed = true;
                    sound.Volume = 0.5f;
                }
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

            idleTime = 0;
            while (currentState < 3)
            {
                while (stateDelta == 0)
                {
                    if(idleTime > 3f)
                    {
                        Coroutine.Start( TutorialClicks());
                        idleTime = 0;
                    }
                        
                    yield return null;
                }
                    

                if (stateDelta > 0)
                    IncreaseState();

                if (stateDelta < 0)
                    DecreaseState();

                var emission = sleepingParticles.emission;

                
                switch (currentState)
                {
                    case 0:
                        Coroutine.Start( TutorialClicks());
                        emission.enabled = true;
                        antura.State = AnturaAnimationStates.sleeping;
                        yield return TimeEngine.Wait( 0.3f);
                        PlayStateSound();
                        TurnAntura( -75f);
                        yield return TimeEngine.Wait( 0.3f);
                        soundPlayed = false;
                        break;

                    case 1:
                        Coroutine.Start(TutorialClicks());
                        emission.enabled = false;
                        antura.State = AnturaAnimationStates.sitting;
                        yield return TimeEngine.Wait( 0.8f);
                        PlayStateSound();
                        yield return TimeEngine.Wait( 1.0f);
                        soundPlayed = false;
                        break;

                    case 2:
                        Coroutine.Start(TutorialClicks());
                        antura.DoShout(() => audioManager.PlaySound( Sfx.DogBarking));
                        PlayStateSound();
                        yield return TimeEngine.Wait( 1.5f);
                        soundPlayed = false;
                        break;

                    case 3:
                        audioManager.PlaySound( Sfx.Win);
                        antura.DoCharge( ()=> StartMoving());
                        TurnAntura( -65f);
                        soundPlayed = false;
                        break;

                    default:
                        break;
                }

                currentTreshold = currentMaxTreshold *0.99f;
                stateDelta = 0;
            }

            yield return TimeEngine.Wait( 1.0f);
        }

        internal bool IsAnimating()
        {
            return isAnimating;
        }

        private void StartMoving()
        {
            playGoneSound();
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

        float idleTime = 0;

        bool ITickable.Update( float deltaTime)
        {
            currentTreshold -= deltaTime;
            idleTime += deltaTime;

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
