using UnityEngine;

namespace EA4S.Minigames.Maze
{
    public class MazeArrow : MonoBehaviour
    {
        public bool tweenToColor = false;
        public bool pingPong = false;

        private Color normalColor;
        private Color highlightedColor;
        private Color unreachedColor;

        Renderer _renderer;

        private GameObject highlightFX;
        private ParticleSystem.MainModule particleSystemMainModule;

        private Color greenParticleSystemColor = new Color(0.2549f, 1f, 0f, 0.3765f);
        private Color redParticleSystemColor = new Color(1f, 0f, 0.102f, 0.3765f);
        private Color yellowParticleSystemColor = new Color(1f, 0.714f, 0f, 0.3765f);

        private enum HighlightState
        {
            None, LaunchPosition, Reached, Unreached
        }
        private HighlightState highlightState;

        public void HighlightAsLaunchPosition()
        {
            if (highlightState != HighlightState.LaunchPosition)
            {
                particleSystemMainModule.loop = true;
                particleSystemMainModule.startColor = yellowParticleSystemColor;
                highlightFX.SetActive(true);

                highlightState = HighlightState.LaunchPosition;
            }
        }

        public void HighlightAsReached()
        {
            if (highlightState != HighlightState.Reached)
            {
                particleSystemMainModule.loop = false;
                particleSystemMainModule.startColor = greenParticleSystemColor;
                highlightFX.SetActive(true);
                _renderer.material.color = highlightedColor;
                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.OK);

                highlightState = HighlightState.Reached;
            }
        }

        public void Unhighlight()
        {
            highlightFX.SetActive(false);
            _renderer.material.color = normalColor;

            highlightState = HighlightState.None;
        }

        public void MarkAsUnreached(bool isFirstUnreachedArrow)
        {
            if (highlightState != HighlightState.Unreached)
            {
                _renderer.material.color = unreachedColor;

                if (isFirstUnreachedArrow)
                {
                    particleSystemMainModule.startColor = redParticleSystemColor;
                    highlightFX.SetActive(true);
                }

                highlightState = HighlightState.Unreached;
            }
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

            particleSystemMainModule = highlightFX.GetComponent<ParticleSystem>().main;

            highlightFX.SetActive(false);
        }

        public void Reset()
        {
            tweenToColor = false;
            pingPong = false;
            particleSystemMainModule.startColor = greenParticleSystemColor;
            particleSystemMainModule.loop = true;
            Unhighlight();
        }
    }
}

