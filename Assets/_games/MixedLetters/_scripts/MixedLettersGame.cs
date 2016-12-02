using ArabicSupport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EA4S.MixedLetters
{
    public class MixedLettersGame : MiniGame
    {
        public static MixedLettersGame instance;

        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }
        public TutorialGameState TutorialState { get; private set; }

        public DropZoneController[] dropZoneControllers;

        //public LL_WordData wordData;
        //public Db.WordData wordInPlay;
        private ILivingLetterData question;
        public List<ILivingLetterData> lettersInOrder;
        public GameObject victimLL;

        public List<ILivingLetterData> allLettersInAlphabet;

        public int roundNumber = 0;
        public int numRoundsWon = 0;
        public bool lastRoundWon = false;

        private bool isSpelling = true;

        protected override void OnInitialize(IGameContext context)
        {
            instance = this;

            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
            TutorialState = new TutorialGameState(this);

            lettersInOrder = new List<ILivingLetterData>();
            allLettersInAlphabet = new List<ILivingLetterData>();

            isSpelling = MixedLettersConfiguration.Instance.Variation == MixedLettersConfiguration.MixedLettersVariation.Spelling;

            if (!isSpelling)
            {
                allLettersInAlphabet = MixedLettersConfiguration.Instance.Questions.GetNextQuestion().GetCorrectAnswers().ToList();
            }

            Physics.IgnoreLayerCollision(0, 5);
            Physics.IgnoreLayerCollision(12, 11);
            Physics.IgnoreLayerCollision(10, 12);

            ResetScene();

            MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayMusic(Music.Theme9);
        }

        protected override IGameState GetInitialState()
        {
            return TutorialState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return MixedLettersConfiguration.Instance;
        }

        public void ShowDropZones()
        {
            int numLetters = lettersInOrder.Count;
            bool isEven = numLetters % 2 == 0;
            float dropZoneWidthWithSpace = Constants.DROP_ZONE_WIDTH + 1f;
            float dropZoneXStart = isEven ? numLetters / 2 - 0.5f : Mathf.Floor(numLetters / 2);
            dropZoneXStart *= dropZoneWidthWithSpace;

            for (int i = 0; i < numLetters; i++)
            {
                DropZoneController dropZoneController = dropZoneControllers[i];
                dropZoneController.Enable();

                Vector3 dropZonePosition = dropZoneController.transform.position;
                dropZonePosition.x = dropZoneXStart - i * dropZoneWidthWithSpace;
                dropZoneController.SetPosition(dropZonePosition);
            }

            for (int i = numLetters; i < dropZoneControllers.Length; i++)
            {
                dropZoneControllers[i].Disable();
            }
        }

        public void OnRoundStarted()
        {
            ShowDropZones();
            SeparateLettersSpawnerController.instance.SetLettersDraggable();
        }

        public void HideDropZones()
        {
            foreach (DropZoneController dropZoneController in dropZoneControllers)
            {
                dropZoneController.Disable();
            }
        }

        public void HideRotationButtons()
        {
            foreach (DropZoneController dropZoneController in dropZoneControllers)
            {
                dropZoneController.HideRotationButton();
            }
        }

        public void ShowGreenTicks()
        {
            for (int i = 0; i < lettersInOrder.Count; i++)
            {
                dropZoneControllers[i].ShowGreenTick();
            }
        }

        private void ResetDropZones()
        {
            foreach (DropZoneController dropZoneController in dropZoneControllers)
            {
                dropZoneController.Reset();
            }
        }

        public void ResetScene()
        {
            ResetDropZones();
            HideDropZones();
            DropZoneController.chosenDropZone = null;
            SeparateLettersSpawnerController.instance.ResetLetters();
            SeparateLettersSpawnerController.instance.DisableLetters();
            lettersInOrder.Clear();
            ParticleSystemController.instance.Reset();
            ParticleSystemController.instance.Disable();
            AnturaController.instance.Disable();
        }

        public void GenerateNewWord()
        {
            if (isSpelling)
            {
                IQuestionPack newQuestion = MixedLettersConfiguration.Instance.Questions.GetNextQuestion();
                question = newQuestion.GetQuestion();

                lettersInOrder = newQuestion.GetCorrectAnswers().ToList();
                VictimLLController.instance.letterObjectView.Init(question);
            }

            else
            {
                int numLettersPerRound = allLettersInAlphabet.Count / 6;
                int remainder = allLettersInAlphabet.Count % 6;
                lettersInOrder = allLettersInAlphabet.GetRange(roundNumber * numLettersPerRound, roundNumber == 4 ? remainder : numLettersPerRound);
                VictimLLController.instance.letterObjectView.Init(null);

                string victimLLWord = "";

                for (int i = 0; i < lettersInOrder.Count; i++)
                {
                    victimLLWord += ((LL_LetterData)lettersInOrder[i]).Data.GetChar();

                    if (i != lettersInOrder.Count - 1)
                    {
                        victimLLWord += " ";
                    }
                }

                VictimLLController.instance.SetCustomText(victimLLWord);
            }
        }

        public void SayQuestion(Action onQuestionOver)
        {
            if (MixedLettersConfiguration.Instance.Variation == MixedLettersConfiguration.MixedLettersVariation.Spelling)
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(question);

                if (onQuestionOver != null)
                {
                    onQuestionOver.Invoke();
                }
            }

            else
            {
                StartCoroutine(AlphabetPronounciationCoroutine(onQuestionOver));
            }
        }

        private IEnumerator AlphabetPronounciationCoroutine(Action onQuestionOver)
        {
            IAudioManager audioManager = MixedLettersConfiguration.Instance.Context.GetAudioManager();

            foreach (ILivingLetterData letterData in lettersInOrder)
            {
                audioManager.PlayLetterData(letterData);

                yield return new WaitForSeconds(0.75f);
            }

            if (onQuestionOver != null)
            {
                onQuestionOver.Invoke();
            }
        }

        public void VerifyLetters()
        {
            for (int i = 0; i < lettersInOrder.Count; i++)
            {
                DropZoneController dropZone = dropZoneControllers[i];
                if (dropZone.droppedLetter == null
                    || dropZone.droppedLetter.GetLetter().Id != lettersInOrder[i].Id
                      || Mathf.Abs(dropZone.droppedLetter.transform.rotation.z) > 0.1f)
                {
                    for (int j = 0; j < lettersInOrder.Count; j++)
                    {
                        SeparateLetterController letter = SeparateLettersSpawnerController.instance.separateLetterControllers[j];
                        letter.SetIsSubjectOfTutorial(roundNumber == 0 && letter == dropZone.correctLetter);
                    }

                    return;
                }
            }

            OnRoundWon();
        }

        private void OnRoundWon()
        {
            lastRoundWon = true;

            if (roundNumber != 0)
            {
                numRoundsWon++;
            }

            HideRotationButtons();
            ShowGreenTicks();
        }
    }
}
