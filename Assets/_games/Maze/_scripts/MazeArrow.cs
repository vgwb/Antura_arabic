using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MazeArrow : MonoBehaviour
    {
        public bool tweenToColor = false;
        public bool pingPong = false;

        private Color normalColor;
        private Color highlightedColor;

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

        public void OnMouseOver()
        {
            if (mazeLetter != null && !IsHighlighted)
            {
                mazeLetter.NotifyFruitGotMouseOver(this);
            }
        }

        public void Highlight()
        {
            highlightFX.SetActive(true);
            _renderer.material.color = highlightedColor;
        }

        public void Unhighlight()
        {
            highlightFX.SetActive(false);
            _renderer.material.color = normalColor;
        }

        void Awake()
        {
            _renderer = GetComponent<Renderer>();

            normalColor = _renderer.material.color;
            highlightedColor = new Color(0.4275f, 1f, 0.4471f, 1f);

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

        void Update()
        {
            /*if (!tweenToColor && !pingPong)
                return;

            if (tweenToColor)
                _renderer.material.color = Color.Lerp(_renderer.material.color, Color.green, Time.deltaTime * 2);
            else if (pingPong)
                _renderer.material.color = Color.Lerp(Color.red, Color.green, Mathf.PingPong(Time.time, 1));*/
        }
    }
}

