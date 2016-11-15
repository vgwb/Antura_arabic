using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// DefaultCamera is looking at Vector3.Forward and is ortographics:
    /// this class helps various elements placers
    /// </summary>
    public class WorldBounds : MonoBehaviour
    {
        private float height;
        private float width;
        private Vector3 center;
        private Camera mainCamera;
        public float SubtitlesMargin;

        static WorldBounds instance;
        public static WorldBounds Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly float LLSize = 3.0f; //conservative size used for computations

        void Awake()
        {
            instance = this;
            mainCamera = Camera.main;
            center = Vector3.zero;
            height = mainCamera.orthographicSize * 2;
            width = height * mainCamera.aspect;
        }

        public Vector3 QuestionSpaceStart()
        {
            var left = center;
            left.x = left.x - width / 2 + SidesMargin(); // leave some margin for additional LLs.
            left.y = left.y + height / 2 - TopMargin();
            left.z = DefaultZ();
            return left;
        }

        public Vector3 QuestionSpaceEnd()
        {
            var right = center;
            right.x = right.x + width / 2 - SidesMargin();
            right.y = right.y + height / 2 - TopMargin();
            right.z = DefaultZ();
            return right;
        }

        public float QuestionGap()
        {
            return width - 2 * SidesMargin();
        }

        public Vector3 OneLineQuestionStart()
        {
            Vector3 position = QuestionSpaceStart();
            position.x -= 0.5f * LetterSize(); // no LLs before first space
            position.y = height / 2 - SubtitlesMargin - LetterSize() * 1.5f;
            return position;
        }

        /// <summary>
        /// Return space occupied by question if placed in single line fashion
        /// </summary>
        /// <param name="questions"> questions for this round</param>
        /// <returns>size in world units</returns>
        public float SingleLineOccupiedSpace( int count)
        {
            return (count) * LetterSize();
        }

        public float DefaultZ()
        {
            return 5.0f;
        }

        public float SidesMargin()
        {
            return LLSize * 1.4f;
        }

        public float TopMargin()
        {
            return SubtitlesMargin + LLSize * 1.5f;
        }

        public float LetterSize()
        {
            return LLSize;
        }

        public float HalfLetterSize()
        {
            return 0.5f * LLSize;
        }
    }
}
