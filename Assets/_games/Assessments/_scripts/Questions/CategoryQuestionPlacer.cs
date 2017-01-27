using Kore.Coroutines;
using System.Collections;
using EA4S.MinigamesCommon;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class CategoryQuestionPlacer : DefaultQuestionPlacer
    {
        public CategoryQuestionPlacer(  IAudioManager audioManager, float questionSize, float answerSize,
                                        AssessmentEvents events)
            :base( audioManager, questionSize, answerSize, events, false)
        {

        }

        public override IEnumerator GetPlaceCoroutine( bool playAudio)
        {
            if (playAudio)
                // warn our heirs
                Debug.LogWarning( "playAudio, parameter not used for Categorization questions");

            // Count questions and answers
            int questionsNumber = 0;
            int placeHoldersNumber = 0;

            foreach (var q in allQuestions)
            {
                questionsNumber++;
                placeHoldersNumber += q.PlaceholdersCount();
            }

            var bounds = WorldBounds.Instance;

            // Text justification "algorithm"
            var gap = bounds.QuestionGap();

            float occupiedSpace = answerSize * placeHoldersNumber; //Different from default placer
            float blankSpace = gap - occupiedSpace;

            float spaceIncrement = blankSpace / (questionsNumber + 1);

            var flow = AssessmentOptions.Instance.LocaleTextFlow;
            float sign;
            Vector3 currentPos;
            
            if (flow == TextFlow.RightToLeft)
            {
                currentPos = bounds.ToTheRightQuestionStart();
                sign = -1;
            }
            else
            {
                currentPos = bounds.ToTheLeftQuestionStart();
                currentPos.x += answerSize / 2.0f;
                sign = 1;
            }

            currentPos.y -= bounds.LetterSize()*1.35f;

            int questionIndex = 0;
            for (int i = 0; i < questionsNumber; i++)
            {
                currentPos.x += spaceIncrement * sign;
                float min = 1000, max = -1000;

                foreach (var p in allQuestions[ questionIndex].GetPlaceholders())
                {
                    currentPos.x += (answerSize * sign) / 2;

                    if (currentPos.x > max)
                        max = currentPos.x;

                    if (currentPos.x < min)
                        min = currentPos.x;

                    yield return PlacePlaceholder(allQuestions[ questionIndex], p, currentPos);
                    currentPos.x += (answerSize * sign) / 2;
                }

                var questionPos = currentPos;
                questionPos.y += bounds.LetterSize()*1.35f;
                questionPos.x = (max + min) /2f;

                // Category questions never read the category
                yield return PlaceQuestion( allQuestions[ questionIndex], questionPos, false);

                questionIndex++;
            }

            // give time to finish animating elements
            yield return Wait.For(0.65f);
            isAnimating = false;
        }
    }
}
