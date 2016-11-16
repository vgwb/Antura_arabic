using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    // TODO: Move some stuff to IWorldBounds
    // TODO: Improve placement or add "distribute behaviour" to LLs (too much overlapping for now)
    public class DefaultPositionsProvider: IPositionsProvider
    {
        internal class AnswerCount
        {
            public int correct { get; private set; }
            public int wrong { get; private set; }

            public AnswerCount( int countCorrect, int countWrong)
            {
                correct = countCorrect;
                wrong = countWrong;
            }
        }

        List< AnswerCount> Answers;

        public void Reset()
        {
            Answers = new List< AnswerCount>();
        }

        private float height;
        private float width;
        private Vector3 center;
        private readonly float LLSize = 3.0f; //conservative size used for computations
        private readonly float SubtitlesMargin = 2.8f; //give space for subtitles

        public DefaultPositionsProvider( float cameraHeight, float cameraWidth, Vector3 worldCenter)
        {
            height = cameraHeight;
            width = cameraWidth;
            center = worldCenter;
            Reset();
        }

        public void AddQuestion( int correctAnswers, int wrongAnswers)
        {
            Answers.Add( new AnswerCount( correctAnswers, wrongAnswers));
        }
        
        private int correctCount;
        private int wrongCount;

        private void CountPositions()
        {
            correctCount = wrongCount = 0;

            foreach ( var c in Answers)
            {
                correctCount += c.correct;
                wrongCount += c.wrong;
            }

            randomLLPositions = new Vector3[correctCount + wrongCount];
            for (int i = 0; i < randomLLPositions.Length; i++)
                randomLLPositions[i] = new Vector3(1000, 1000, 1000); //outside of play area for sure
        }

        Vector3 AnswerStartGap()
        {
            var left = center;
            left.x = left.x - width / 2 + SidesMargin(); // leave some margin for additional LLs.
            left.y = left.y + height / 2 - TopMargin();
            left.z = DefaultZ();
            return left;
        }

        Vector3 AnswerEndGap()
        {
            var right = center;
            right.x = right.x + width / 2 - SidesMargin();
            right.y = right.y + height / 2 - TopMargin();
            right.z = DefaultZ();
            return right;
        }

        float DefaultZ()
        {
            return 5.0f;
        }

        float SidesMargin()
        {
            return LLSize * 1.4f;
        }

        float TopMargin()
        {
            return SubtitlesMargin + LLSize*1.5f;
        }

        // This runs in O(N^2) time compared to number of total LL of the round
        public PositionGroup GetPositions()
        {
            List< LLPositions> positions = new List< LLPositions>();
            CountPositions();

            // Text justification "algorithm"
            var left = AnswerStartGap();
            var right = AnswerEndGap();
            var dx = right.x - left.x;
            float occupiedSpace = ( Answers.Count + correctCount) * LLSize;
            float blankSpace = dx - occupiedSpace;

            if (blankSpace <= 0.5f*LLSize) // To be implemented if needed
                throw new InvalidOperationException("Need a line break becase 1 line is not enough for all");

            //  3 words => 4 white zones  (need increment by 1)
            //  |  O   O   O  |
            float spaceIncrement = blankSpace / (Answers.Count + 1);

            var currentPos = left;
            currentPos.x -= 0.5f * LLSize; // no LLs before first space
            currentPos.y = center.y + height / 2 - SubtitlesMargin - LLSize * 1.5f;

            foreach( var answ in Answers)
            {
                Position[] question_position = new Position[1];
                Position[] correct_placeholder = new Position[answ.correct];
                Position[] correct_answer = new Position[answ.correct];
                Position[] wrong_answer = new Position[answ.wrong];

                currentPos.x += spaceIncrement + LLSize; // between 2 words we need to compensate margin of 2 LLs' halfs
                question_position[0] = new Position( currentPos);

                if (answ.correct < 1)
                    throw new ArgumentException("Each question has at least a correct answer");

                for(int i=0; i < answ.correct; i++)
                {
                    currentPos.x += LLSize;
                    correct_placeholder[i] = new Position( currentPos);
                    correct_answer[i] = AddRandomLetterPosition();
                }

                for (int i = 0; i < answ.wrong; i++)
                    wrong_answer[i] = AddRandomLetterPosition();

                positions.Add( new LLPositions( question_position, correct_placeholder, correct_answer, wrong_answer));
            }

            return new PositionGroup( positions.ToArray());
        }

        // try to add a letter at random empty positions, after X attemps the letter is placed anyway

        Vector3[] randomLLPositions;
        private Position AddRandomLetterPosition()
        {
            float xMin = center.x - width / 2 + 0.7f*LLSize;
            float xMax = center.x + width / 2 - 0.7f * LLSize;
            float yMin = center.y + height / 2 - TopMargin();
            float yMax = center.y - height / 2 + 0.7f * LLSize;
            float questionXmin = AnswerStartGap().x;
            float questionXmax = AnswerEndGap().x;
            float questionYmin = center.y + height/2 - SubtitlesMargin - 3*LLSize;

            const int attemps = 20;
            Vector3 pos = new Vector3();
            pos.z = DefaultZ();

            // O(n^2) => 30 letters? max 900*attemps loops, and only at initialization time
            int counter = 0;
            while (true)
            {
                counter++;
                pos.x = UnityEngine.Random.Range( xMin, xMax);
                pos.y = UnityEngine.Random.Range( yMin, yMax);

                if (pos.y >= questionYmin) // check if LL fits beside questions
                    if (pos.x > questionXmin || pos.x < questionXmax)
                        continue; // cannot overlap to questions

                if (counter <= attemps)
                {
                    bool overlapLL = false;
                    foreach (var p in randomLLPositions)
                        if (p.DistanceIsLessThan( pos, 1.5f * LLSize))
                        {
                            overlapLL = true;
                            break;
                        }
                    
                    if(overlapLL == false)
                        return new Position( pos);
                }
                else
                    return new Position( pos);
            }
        }
    }
}