using Kore.Coroutines;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Assessment
{
    internal class AnswerChecker
    {
        private ICheckmarkWidget checkmarkWidget;
        private IAudioManager audioManager;
        private IDialogueManager dialogueManager;

        public AnswerChecker(    ICheckmarkWidget checkmarkWidget,
                                 IAudioManager audioManager,
                                 IDialogueManager dialogueManager)
        {
            this.checkmarkWidget = checkmarkWidget;
            this.audioManager = audioManager;
            this.dialogueManager = dialogueManager;
        }

        private bool isAnimating = false;
        private bool allCorrect = false;

        // When all answers are correct return true
        public bool AllCorrect()
        {
            if (coroutineEnded)  // Needed to see All Correct only when animation ended
            {
                coroutineEnded = false;
                isAnimating = false;
                return allCorrect; // Value setted by CheckCoroutine
            }

            return false;
        }

        // When need to check validity of answers return true
        public bool AreAllAnswered( List< PlaceholderBehaviour> placeholders)
        {
            var count = AnswerSet.GetCorrectCount();
            int linkedDroppables = 0;
            foreach (var p in placeholders)
                if (p.LinkedDroppable != null)
                    linkedDroppables++;

            return linkedDroppables >= count;
        }

        public void Check(  List< PlaceholderBehaviour> placeholders,
                            List< IQuestion> questions,
                            IDragManager dragManager)
        {
            isAnimating = true;
            coroutineEnded = false;
            allCorrect = false;
            Koroutine.Run( CheckCoroutine( placeholders, questions, dragManager));
        }

        private bool AreQuestionsCorrect( List< IQuestion> questions)
        {
            foreach (var q in questions)
                if (q.GetAnswerSet().AllCorrect() == false)
                    return false;

            return true;
        }

        private bool coroutineEnded = false;
        private IEnumerator CheckCoroutine( List< PlaceholderBehaviour> placeholders,
                                            List< IQuestion> questions,
                                            IDragManager dragManager)
        {
            dragManager.DisableInput();

            bool areAllCorrect = AreQuestionsCorrect( questions);
            if (areAllCorrect) {
                
                // Log learning progress
                foreach (var p in placeholders)
                    if (p.LinkedDroppable != null)
                    {
                        var set = p.Placeholder.GetQuestion().GetAnswerSet();
                        var answ = p.LinkedDroppable.GetAnswer();
                        if (set.IsCorrect(answ))
                            AssessmentConfiguration.Instance.Context.GetLogManager().OnAnswered(answ.Data(), false);
                    }
                
                // Just trigger OnQuestionAnswered events if all are correct
                foreach (var q in questions)
                {
                    var behaviour = q.gameObject.GetComponent< QuestionBehaviour>();
                    behaviour.OnQuestionAnswered();

                    yield return Wait.For(behaviour.TimeToWait());
                }

            } else {
                foreach (var p in placeholders) {
                    if (p.LinkedDroppable != null) {
                        var set = p.Placeholder.GetQuestion().GetAnswerSet();
                        var answ = p.LinkedDroppable.GetAnswer();
                        if (set.IsCorrect(answ) == false) {
                            AssessmentConfiguration.Instance.Context.GetLogManager().OnAnswered(answ.Data(), false);
                            p.LinkedDroppable.Detach(true);
                        }
                    }
                }
            }

            allCorrect = areAllCorrect;

            while (wrongAnswerAnimationPlaying)
                yield return null; // wait only if previous message has not finished

            if (allCorrect)
            {
                audioManager.PlaySound(Sfx.StampOK);
                yield return Wait.For( 0.4f);
                checkmarkWidget.Show(true);
                yield return Wait.For( 1.0f);
            }
            else
            {
                wrongAnswerAnimationPlaying = true;
                Koroutine.Run( WrongAnswerCoroutine());
            }

            coroutineEnded = true;
            dragManager.EnableInput();
        }

        private bool wrongAnswerAnimationPlaying = false;

        private IEnumerator WrongAnswerCoroutine()
        {
            checkmarkWidget.Show(false);
            audioManager.PlaySound(Sfx.KO);
            yield return PlayAnswerWrong();
            wrongAnswerAnimationPlaying = false;
        }

        IYieldable PlayAnswerWrong()
        {
            return dialogueManager.Speak(Localization.Random(
                                            Db.LocalizationDataId.Assessment_Wrong_1,
                                            Db.LocalizationDataId.Assessment_Wrong_2,
                                            Db.LocalizationDataId.Assessment_Wrong_3));
        }

        private bool WrongAnswerAnimationPlaying()
        {
            return wrongAnswerAnimationPlaying;
        }

        public bool IsAnimating()
        {
            return isAnimating;
        }
    }
}
