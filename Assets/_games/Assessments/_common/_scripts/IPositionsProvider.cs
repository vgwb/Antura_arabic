using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Interface for placing LivingLetters on the screen, Expecting 1 implementation
    /// but more implementations possible for tweaking each single Assessment graphical
    /// appareance.
    /// </summary>
    public interface IPositionsProvider
    {
        /// <summary>
        /// Clears cached data (to be called before each Session)
        /// </summary>
        void Reset();

        /// <summary>
        /// Add a Question to the cache, it has to get all questions before computing
        /// the correct positions of everything.
        /// </summary>
        /// <param name="correctAnswers">How many correct answers</param>
        /// <param name="wrongAnswers">How many wrong answers</param>
        void AddQuestion( int correctAnswers, int wrongAnswers);

        /// <summary>
        /// Use the cache to generate valid positions for each play session.
        /// </summary>
        /// <returns>Data structure to access positions</returns>
        PositionGroup GetPositions();
    }

    public class PositionGroup
    {
        public LLPositions [] Positions { get; private set; }

        public PositionGroup( LLPositions [] positions)
        {
            Positions = positions;
        }
    }

    public static class PositionsProviderExtension
    {
        public static void AddQuestionPack( this IPositionsProvider me, IQuestionPack pack)
        {
            int countCorrectAnswers = 0;
            int countWrongAnswers = 0;
            foreach (var p in pack.GetCorrectAnswers())
                countCorrectAnswers++;

            foreach (var p in pack.GetWrongAnswers())
                countWrongAnswers++;

            me.AddQuestion( countCorrectAnswers, countWrongAnswers);
        }
    }

    public class Position
    {
        public Quaternion rotation { get; private set; }
        public Vector3 position { get; private set; }
        public Vector3 scale { get; private set; }

        public Position(Vector3 pos)
            : this(Quaternion.Euler(0,180f,0), pos, Vector3.one)
        {
        }

        public Position( Quaternion rot, Vector3 pos)
            :this( rot, pos, Vector3.one)
        {
        }

        public Position( Quaternion rot, Vector3 pos, Vector3 scl)
        {
            rotation = rot;
            position = pos;
            scale = scl;
        }
    }

    public static class TransformExtension
    {
        public static void LocalFrom( this Transform transform, Position pos)
        {
            transform.localRotation = pos.rotation;
            transform.localPosition = pos.position;
            transform.localScale = pos.scale;
        }
    }

    public class LLPositions
    {
        public LLPositions( Position[] question, 
                            Position[] correctPlaceholders,
                            Position[] correctAnswers,
                            Position[] wrongAnswers)
        {
            QuestionPivots = question;
            PlaceholderPivots = correctPlaceholders;
            CorrectPivots = correctAnswers;
            WrongPivots = wrongAnswers;
        }

        public Position[] QuestionPivots { get; private set; }
        public Position[] PlaceholderPivots { get; private set; }
        public Position[] CorrectPivots { get; private set; }
        public Position[] WrongPivots { get; private set; }
    }
}