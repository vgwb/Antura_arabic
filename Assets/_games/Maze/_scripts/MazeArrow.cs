using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MazeArrow : MonoBehaviour
    {
        private readonly Vector3 WRONG_MARK_OFFSET = Vector3.left * 1.25f;

        public bool tweenToColor = false;
        public bool pingPong = false;

        private Color normalColor;
        private Color highlightedColor;
        private Color unreachedColor;

        Renderer _renderer;

        private MazeLetter mazeLetter;
        private GameObject highlightFX;

        private bool IsHighlighted
        {
            get
            {
                return highlightFX.activeSelf;
            }
        }

        public void SetMazeLetter(MazeLetter mazeLetter)
        {
            this.mazeLetter = mazeLetter;
        }

        public void Highlight()
        {
            if (!IsHighlighted)
            {
                highlightFX.SetActive(true);
                _renderer.material.color = highlightedColor;
            }
        }

        public void Unhighlight()
        {
            highlightFX.SetActive(false);
            _renderer.material.color = normalColor;
        }

        public void MarkAsUnreached()
        {
            _renderer.material.color = unreachedColor;

            Vector3 rotatedVector = new Vector3();
            float rotationAngle = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            rotatedVector.x = WRONG_MARK_OFFSET.x * Mathf.Cos(rotationAngle) + WRONG_MARK_OFFSET.z * Mathf.Sin(rotationAngle);
            rotatedVector.z = -WRONG_MARK_OFFSET.x * Mathf.Sin(rotationAngle) + WRONG_MARK_OFFSET.z * Mathf.Cos(rotationAngle);

            Tutorial.TutorialUI.MarkNo(transform.position + rotatedVector);
        }

        void Awake()
        {
            _renderer = GetComponent<Renderer>();

            normalColor = _renderer.material.color;
            highlightedColor = new Color(0.4275f, 1f, 0.4471f, 1f);
            unreachedColor = Color.red;

            highlightFX = Instantiate(MazeGameManager.instance.arrowTargetPrefab);
            highlightFX.transform.position = transform.position;
            highlightFX.transform.localScale = Vector3.one * 2f;
            highlightFX.transform.parent = gameObject.transform;
            highlightFX.SetActive(false);
        }

        public void Reset()
        {
            tweenToColor = false;
            pingPong = false;
            Unhighlight();
        }
    }
}

