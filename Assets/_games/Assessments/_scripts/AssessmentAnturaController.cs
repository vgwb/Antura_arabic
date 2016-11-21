using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentAnturaController : MonoBehaviour, ITickable
    {
        public AnturaAnimationController antura;

        static AssessmentAnturaController instance;
        public static AssessmentAnturaController Instance
        {
            get
            {
                return instance;
            }
        }

        private IAudioManager audioManager;

        void Awake()
        {
            instance = this;
            antura.State = AnturaAnimationStates.sleeping;
            currentState = 0;
            audioManager = AssessmentConfiguration.Instance.Context.GetAudioManager();
        }

        void Start()
        {
            TimeEngine.AddTickable( this);
            Coroutine.Start( CheckStateAndSetAnimation());
        }

        void OnMouseUp()
        {
            if(clickEnabled)
                AnturaPressed();
        }

        private bool clickEnabled = false;
        public void EnableClick()
        {
            clickEnabled = true;
        }

        private void AnturaPressed()
        {
            currentTreshold += GainPerClick;
            if(currentState<4)
                audioManager.PlaySound( soundOnClick);
        }

        [Header("Prefabs")]
        public ParticleSystem sleepingParticles;
        public Transform anturaDestination;

        [Header("Tutorial Configuration")]
        public float Treshold = 0.61f;
        public float GainPerClick = 0.3f;
        public Sfx soundOnClick;

        private float currentTreshold = 0;
        private float currentMaxTreshold = 1f;
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
            while (currentState < 4)
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
                        emission.enabled = true;
                        antura.State = AnturaAnimationStates.sleeping;
                        yield return TimeEngine.Wait( 0.35f);
                        PlayStateSound();
                        TurnAntura(-80);
                        yield return TimeEngine.Wait( 0.35f);
                        break;

                    case 1:
                        emission.enabled = false;
                        antura.State = AnturaAnimationStates.sleeping;
                        yield return TimeEngine.Wait( 0.4f);
                        PlayStateSound();
                        TurnAntura(-75f);
                        yield return TimeEngine.Wait( 1f);
                        break;

                    case 2:
                        antura.State = AnturaAnimationStates.sitting;
                        yield return TimeEngine.Wait( 0.8f);
                        PlayStateSound();
                        yield return TimeEngine.Wait( 1.9f);
                        break;

                    case 3:
                        antura.DoShout(() => audioManager.PlaySound( Sfx.DogBarking));
                        PlayStateSound();
                        yield return TimeEngine.Wait( 2.3f);
                        break;

                    case 4:
                        audioManager.PlaySound( Sfx.Win);
                        antura.DoCharge( ()=> StartMoving());
                        TurnAntura(-65f);
                        break;

                    default:
                        break;
                }

                currentTreshold = currentMaxTreshold *0.99f;
                stateDelta = 0;
            }
        }

        Action onFinishedAnimation;
        public void SetFinishedAnimationCallback( Action callback)
        {
            onFinishedAnimation = callback;
        }

        private void StartMoving()
        {
            transform.DOMove( anturaDestination.position, 3.0f)
                .SetEase( Ease.InOutSine)
                .OnComplete( ()=> onFinishedAnimation());
        }

        private void PlayStateSound()
        {
            if (playSound)
                    audioManager.PlaySound( soundToPlay);

            playSound = false;
        }

        bool playSound = false;
        Sfx soundToPlay;
        private void DecreaseState()
        {
            soundToPlay = Sfx.LetterSad;
            playSound = true;
            stateDelta = 0;
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
