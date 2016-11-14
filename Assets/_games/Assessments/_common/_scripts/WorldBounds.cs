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
            return SubtitlesMargin + LLSize * 1.5f;
        }
    }
}
