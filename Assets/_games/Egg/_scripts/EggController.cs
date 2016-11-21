using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggController : MonoBehaviour
    {
        List<EggLivingLetter> eggLivingLetters = new List<EggLivingLetter>();
        GameObject letterObjectViewPrefab;
        GameObject shadowPrefab;
        Vector3[] lettersMaxPositions;

        public GameObject emoticonPrefab;

        public GameObject egg;

        public EggPiece[] eggPieces;

        public EggControllerCollider eggCollider;

        public Transform notRotatedObjects;
        Vector3 notRotation = new Vector3(0f, 0f, 0f);
        public Transform emoticonsScale;

        public GameObject eggShadow;

        public Action onEggCrackComplete;
        public Action onEggExitComplete;

        public Action onEggPressedCallback
        {
            get { return eggCollider.pressedCallback; }
            set { eggCollider.pressedCallback = value; }
        }

        public GameObject eggParticleWin;

        public AnimationCurve inMoveCurve;
        public AnimationCurve outMoveCurve;

        Tween moveEggParentTween;
        Tween moveEggTeewn;
        Tween rotationEggTween;

        Action endTransformToCallback;
        Action endAudioQuestion;

        int currentPosition;
        Vector3 currentRotation;

        Vector3[] eggPositions;

        IAudioManager audioManager;
        IAudioSource audioSource;
        List<ILivingLetterData> lLDAudioQuestion = new List<ILivingLetterData>();

        List<ILivingLetterData> questionData = new List<ILivingLetterData>();

        int piecePoofCompleteCount = 0;
        bool eggEggCrackCompleteSent = false;

        EggEmoticonsController emoticonsController;

        public void Initialize(GameObject letterObjectViewPrefab, GameObject shadowPrefab, Vector3[] eggPositions, Vector3[] lettersMaxPositions, IAudioManager audioManager)
        {
            this.letterObjectViewPrefab = letterObjectViewPrefab;
            this.shadowPrefab = shadowPrefab;
            this.lettersMaxPositions = lettersMaxPositions;

            this.eggPositions = eggPositions;
            eggCollider.DisableCollider();

            EggShow(false);
            eggParticleWin.SetActive(false);

            this.audioManager = audioManager;

            piecePoofCompleteCount = 0;
            eggEggCrackCompleteSent = false;

            InitializeEggPices();

            currentRotation = new Vector3(0f, 0f, -90f);
            GoToPosition(0, currentRotation);

            emoticonsController = new EggEmoticonsController(emoticonsScale, emoticonPrefab);
        }

        public void Reset()
        {
            ResetCrack();

            currentRotation = new Vector3(0f, 0f, -90f);
            GoToPosition(0, currentRotation);

            DestroyQuestionLetters();
            EggShow(true);

            eggParticleWin.SetActive(true);            

            ParticleWinDisabled();

            audioSource = null;

            piecePoofCompleteCount = 0;
            eggEggCrackCompleteSent = false;
        }

        public void MoveNext(float duration, Action callback)
        {
            if (moveEggParentTween != null) { moveEggParentTween.Kill(); }
            if (rotationEggTween != null) { rotationEggTween.Kill(); }

            currentPosition++;

            if (currentPosition >= eggPositions.Length)
            {
                currentPosition = 0;
            }

            currentRotation.z += 90f;

            AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

            if (currentPosition == 1)
            {
                moveCurve = inMoveCurve;
            }
            else if (currentPosition == eggPositions.Length - 1)
            {
                moveCurve = outMoveCurve;
            }

            bool inOutRotation = currentPosition == 1 || currentPosition == eggPositions.Length - 1;

            TransformTo(eggPositions[currentPosition], inOutRotation, moveCurve, currentRotation, duration, callback);
        }

        public bool isNextToExit
        {
            get
            {
                if (currentPosition == eggPositions.Length - 2)
                    return true;

                return false;
            }
        }

        public void InitializeEggPices()
        {
            for (int i = 0; i < eggPieces.Length; i++)
            {
                eggPieces[i].onPoofEnd = OnPiecePoofComplete;
            }
        }

        public void ResetCrack()
        {
            for (int i = 0; i < eggPieces.Length; i++)
            {
                eggPieces[i].Reset();
            }
        }

        public void Cracking(float progress)
        {
            //StartTrembling();

            //int eggPiecesProgress = (int)(progress * eggPieces.Length);

            //for (int i = 0; i < eggPiecesProgress; i++)
            //{
            //    int index = isNextToExit ? ((eggPieces.Length - 1) - i) : i;

            //    bool poofDirRight = (i % 2 == 0);

            //    if ((currentPosition == 2) && (i < 2))
            //    {
            //        poofDirRight = false;
            //    }

            //    eggPieces[index].Poof(poofDirRight);
            //}
            
            if (progress == 1f)
            {
                for(int i=0; i<eggPieces.Length; i++)
                {
                    bool poofDirRight = (i % 2 == 0);

                    //if ((currentPosition == 2) && (i < 2))
                    //{
                    //    poofDirRight = false;
                    //}

                    eggPieces[i].Poof(poofDirRight);
                }

                ShowEndLetters();
            }
            else
            {
                for (int i = 0; i < eggPieces.Length; i++)
                {
                    eggPieces[i].Shake();
                }
            }
        }

        void MoveTo(Vector3 position, float duration, AnimationCurve moveCurve)
        {
            if (moveEggParentTween != null)
            {
                moveEggParentTween.Kill();
            }

            moveEggParentTween = transform.DOLocalMove(position, duration).OnComplete(delegate ()
            {
                if (endTransformToCallback != null) endTransformToCallback();

                if (onEggExitComplete != null && (currentPosition == eggPositions.Length - 1))
                {
                    onEggExitComplete();
                }

            })
            ;
            //.SetEase(moveCurve);
        }

        void InOutRotation(Vector3 rotation, float duration)
        {
            if (rotationEggTween != null)
            {
                rotationEggTween.Kill();
            }

            rotationEggTween = DOTween.To(() => egg.transform.eulerAngles.z, z => egg.transform.eulerAngles = new Vector3(egg.transform.eulerAngles.x, egg.transform.eulerAngles.y, z), rotation.z + 720f, duration * 0.95f)
                .OnComplete(delegate ()
                {
                    BouncingRotation(0.5f);
                });
        }

        void RoteteTo(Vector3 rotation, float duration)
        {
            if (rotationEggTween != null)
            {
                rotationEggTween.Kill();
            }

            rotationEggTween = egg.transform.DORotate(rotation, duration * 0.93f).OnComplete(delegate ()
            {
                BouncingRotation();
            });
        }

        void BouncingRotation(float duration = 0.8f)
        {
            float firstStepValue = 5f;
            float secondStepValue = -2.5f;

            Vector3 rotationFirstStep = Vector3.zero;
            rotationFirstStep.z += firstStepValue;
            Vector3 rotationSecondStep = Vector3.zero;
            rotationSecondStep.z += secondStepValue;

            rotationEggTween = transform.DORotate(rotationFirstStep, (duration / 10f) * 5f).OnComplete(delegate ()
            {
                rotationEggTween = transform.DORotate(rotationSecondStep, (duration / 10f) * 4f).OnComplete(delegate ()
                {
                    rotationEggTween = transform.DORotate(Vector3.zero, (duration / 10f) * 2f);
                });
            });
        }

        void TransformTo(Vector3 localPosition, bool inOutRotation, AnimationCurve moveAnimationCurve, Vector3 rotation, float duration, Action callback)
        {
            MoveTo(localPosition, duration, moveAnimationCurve);

            if (inOutRotation)
            {
                InOutRotation(rotation, duration);
            }
            else
            {
                RoteteTo(rotation, duration);
            }

            endTransformToCallback = callback;
        }

        void GoToPosition(int positionNumber, Vector3 rotation)
        {
            if (moveEggParentTween != null) { moveEggParentTween.Kill(); }
            if (rotationEggTween != null) { rotationEggTween.Kill(); }

            currentPosition = positionNumber;

            transform.localPosition = eggPositions[currentPosition];
            egg.transform.eulerAngles = rotation;
            transform.eulerAngles = Vector3.zero;
        }

        public void EnableInput()
        {
            eggCollider.EnableCollider();
        }

        public void DisableInput()
        {
            eggCollider.DisableCollider();
        }

        void Update()
        {
            if (lLDAudioQuestion.Count > 0)
            {
                if (audioSource != null)
                {
                    if (!audioSource.IsPlaying)
                    {
                        audioSource = null;
                    }
                }
                else
                {
                    ILivingLetterData letterData = lLDAudioQuestion[0];

                    audioSource = audioManager.PlayLetterData(letterData);

                    lLDAudioQuestion.RemoveAt(0);
                }
            }
            else
            {
                if (audioSource != null)
                {
                    if (!audioSource.IsPlaying)
                    {
                        audioSource = null;

                        if (endAudioQuestion != null)
                        {
                            endAudioQuestion();
                        }
                    }
                }
            }

            if (!eggEggCrackCompleteSent)
            {
                if (piecePoofCompleteCount >= eggPieces.Length)
                {
                    eggEggCrackCompleteSent = true;

                    EggShow(false);

                    if (onEggCrackComplete != null)
                    {
                        onEggCrackComplete();
                    }
                }
            }

            if (emoticonsController != null)
            {
                emoticonsController.Update(Time.deltaTime);
            }

            for (int i = 0; i < eggLivingLetters.Count; i++)
            {
                eggLivingLetters[i].Update(Time.deltaTime);
            }
        }

        public void LateUpdate()
        {
            float minY = 2.5f;
            float maxY = 4.1f;

            float maxDelta = maxY - minY;

            float zRotation = egg.transform.eulerAngles.z % 360f;

            float newYPosition = minY;

            if (zRotation <= 180)
            {
                if (zRotation < 90f)
                {
                    zRotation = 0f;
                }
                else
                {
                    zRotation += -90f;
                }
            }
            else
            {
                zRotation += -180f;

                if (zRotation <= 90f)
                {
                    zRotation = 90f - zRotation;
                }
                else
                {
                    zRotation = 0f;
                }
            }

            newYPosition += maxDelta * (zRotation / 90f);

            Vector3 newPosition = egg.transform.localPosition;

            newPosition.y = newYPosition;

            egg.transform.localPosition = newPosition;

            notRotatedObjects.eulerAngles = notRotation;
        }

        public void SetQuestion(ILivingLetterData questionData)
        {
            this.questionData.Clear();

            this.questionData.Add(questionData);
        }

        public void SetQuestion(IEnumerable<ILivingLetterData> questionData)
        {
            this.questionData.Clear();

            foreach (ILivingLetterData letterData in questionData)
            {
                this.questionData.Add(letterData);
            }
        }

        public void PlayAudioQuestion(Action endCallback)
        {
            audioSource = null;

            endAudioQuestion = endCallback;

            lLDAudioQuestion.Clear();

            for (int i = 0; i < questionData.Count; i++)
            {
                lLDAudioQuestion.Add(questionData[i]);
            }
        }

        public void ParticleWinEnabled()
        {
            //eggParticleWin.SetActive(true);

            foreach (var particles in eggParticleWin.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Play();
            }
        }

        public void ParticleWinDisabled()
        {
            foreach (var particles in eggParticleWin.GetComponentsInChildren<ParticleSystem>(true))
            {
                particles.Stop();
            }

            //eggParticleWin.SetActive(false);
        }

        public void StartShake()
        {
            for(int i=0; i<eggPieces.Length; i++)
            {
                eggPieces[i].Shake();
            }
        }

        void OnPiecePoofComplete()
        {
            piecePoofCompleteCount++;
        }

        void EggShow(bool show)
        {
            eggShadow.SetActive(show);
            egg.SetActive(show);
        }

        void ShowEndLetters()
        {
            EggLivingLetter letter;

            //float startDelay = 0.7f;
            float startDelay = 0f;

            float jumpDelay = 0.5f;

            Vector3[] lettersEndPositions = GetLettersEndPositions();

            for (int i = 0; i < questionData.Count; i++)
            {
                Action jumpCallback = null;

                if (i == questionData.Count - 1)
                {
                    jumpCallback = OnLettersJumpComplete;
                }

                Vector3 lLetterPosition = new Vector3(transform.localPosition.x, egg.transform.localPosition.y, transform.localPosition.z);
                letter = new EggLivingLetter(transform.parent, letterObjectViewPrefab, shadowPrefab, questionData[i], lLetterPosition, transform.localPosition, lettersEndPositions[i], (jumpDelay * i) + startDelay, jumpCallback);

                eggLivingLetters.Add(letter);
            }
        }

        void DestroyQuestionLetters()
        {
            for (int i = 0; i < eggLivingLetters.Count; i++)
            {
                eggLivingLetters[i].DestroyLetter();
            }

            eggLivingLetters.Clear();
        }

        Vector3[] GetLettersEndPositions()
        {
            int questionDataCount = questionData.Count;

            Vector3[] lettersEndPositions = new Vector3[questionDataCount];

            Vector3 maxLeft = lettersMaxPositions[0];
            Vector3 maxRight = lettersMaxPositions[1];

            float positionLerp = 1f / (questionDataCount + 1);

            for (int i = 0; i < questionDataCount; i++)
            {
                lettersEndPositions[i] = Vector3.Lerp(maxLeft, maxRight, 1f - (positionLerp * (i + 1)));
            }

            return lettersEndPositions;
        }

        void OnLettersJumpComplete()
        {

        }

        public void EmoticonHappy()
        {
            emoticonsController.EmoticonHappy();
        }

        public void EmoticonPositive()
        {
            emoticonsController.EmoticonPositive();
        }

        public void EmoticonNegative()
        {
            emoticonsController.EmoticonNegative();
        }

        public void EmoticonInterrogative()
        {
            emoticonsController.EmoticonInterrogative();
        }

        public void EmoticonClose()
        {
            emoticonsController.CloseEmoticons();
        }
    }
}