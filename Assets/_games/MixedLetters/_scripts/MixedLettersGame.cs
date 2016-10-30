using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EA4S.MixedLetters
{
    public class MixedLettersGame : MiniGame
    {
        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        public DropZoneController[] dropZoneControllers;
        public RotateButtonController[] rotateButtonControllers;

        public Db.WordData wordInPlay;
        public List<LL_LetterData> lettersInOrder;

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);

            lettersInOrder = new List<LL_LetterData>();

            for (int i = 0; i < rotateButtonControllers.Length; i++)
            {
                rotateButtonControllers[i].SetDropZone(dropZoneControllers[i]);
            }

            Physics.IgnoreLayerCollision(0, 5);

            ResetScene();
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return MixedLettersConfiguration.Instance;
        }

        public void ShowDropZones()
        {
            int numLetters = lettersInOrder.Count;
            bool isEven = numLetters % 2 == 0;
            float dropZoneWidthWithSpace = Constants.DROP_ZONE_WIDTH + 0.4f;
            float dropZoneXStart = isEven ? numLetters / 2 - 0.5f : Mathf.Floor(numLetters / 2);
            dropZoneXStart *= dropZoneWidthWithSpace;

            for (int i = 0; i < numLetters; i++)
            {
                DropZoneController dropZoneController = dropZoneControllers[i];
                dropZoneController.Enable();

                Vector3 dropZonePosition = dropZoneController.transform.position;
                dropZonePosition.x = dropZoneXStart - i * dropZoneWidthWithSpace;
                dropZoneController.SetPosition(dropZonePosition);

                RotateButtonController rotateButtonController = rotateButtonControllers[i];
                rotateButtonController.Enable();

                Vector3 rotateButtonPosition = dropZonePosition;
                rotateButtonPosition.y -= 1.35f;
                rotateButtonController.SetPosition(rotateButtonPosition);
            }

            for (int i = numLetters; i < dropZoneControllers.Length; i++)
            {
                dropZoneControllers[i].Disable();
                rotateButtonControllers[i].Disable();
            }
        }

        private void HideDropZones()
        {
            foreach (DropZoneController dropZoneController in dropZoneControllers)
            {
                dropZoneController.Disable();
            }

            foreach (RotateButtonController rotateButtonController in rotateButtonControllers)
            {
                rotateButtonController.Disable();
            }
        }

        public void ResetScene()
        {
            HideDropZones();
            SeparateLettersSpawnerController.instance.DisableLetters();
            lettersInOrder.Clear();
        }

        public void GenerateNewWord()
        {
            wordInPlay = AppManager.Instance.Teacher.GimmeAGoodWord();
            lettersInOrder.AddRange(ArabicAlphabetHelper.LetterDataListFromWord(wordInPlay.Arabic, AppManager.Instance.Letters));
        }
    }
}
