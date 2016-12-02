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

        void Awake()
        {
            instance = this;
            mainCamera = Camera.main;
            center = Vector3.zero;
            height = mainCamera.orthographicSize * 2;
            width = height * mainCamera.aspect;
        }

        public float YMin()
        {
            return -height / 2 + 0.7f * LetterSize();
        }

        public float YMax()
        {
            return -LetterSize()*1.5f;
        }

        public Vector3 RandomAnswerPosition()
        {
            float questionXmin = QuestionSpaceStart().x;
            float questionXmax = QuestionSpaceEnd().x;
            float questionYmin = height / 2 - 2f - 3 * LetterSize();
            float xMin = - width / 2 + 0.7f * LetterSize();
            float xMax = width / 2 - 0.7f * LetterSize();
            float yMin = YMin();
            float yMax = YMax();

            Vector3 pos = Vector3.zero;
            pos.z = DefaultZ();

            while (true)
            {
                //TODO: limit y position after N attemps?
                pos.x = Random.Range( xMin, xMax);
                pos.y = Random.Range( yMin, yMax);

                if (pos.y >= questionYmin) // check if LL fits beside questions
                    if (pos.x > questionXmin || pos.x < questionXmax)
                        continue; // cannot overlap to questions

                return pos;
            }
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

        public Vector3 ToTheLeftQuestionStart()
        {
            Vector3 position = QuestionSpaceStart();
            position.y = height / 2 - SubtitlesMargin - LetterSize() * 1.5f;
            return position;
        }

        public Vector3 ToTheRightQuestionStart()
        {
            Vector3 position = QuestionSpaceEnd();
            position.y = height / 2 - SubtitlesMargin - LetterSize() * 1.5f;
            return position;
        }

        public float DefaultZ()
        {
            return 5.0f;
        }

        public float SidesMargin()
        {
            return ElementsSize.LL * 1.4f;
        }

        public float TopMargin()
        {
            // No more subtitles
            return 0; //SubtitlesMargin + ElementsSize.LL * 1.5f;
        }

        public float LetterSize()
        {
            return ElementsSize.LL;
        }

        public float HalfLetterSize()
        {
            return 0.5f * ElementsSize.LL;
        }
    }
}
