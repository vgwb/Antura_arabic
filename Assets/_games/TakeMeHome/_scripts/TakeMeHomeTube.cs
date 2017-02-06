using UnityEngine;
using DG.Tweening;

namespace EA4S.Minigames.TakeMeHome
{
	
    public class TakeMeHomeTube : MonoBehaviour {

		Tweener moveTweener;

		Vector3 originalPosition;
        public bool upperTube;
        public GameObject aspiration;
        public GameObject winParticles;
        public GameObject cubeInfo;

        // Use this for initialization
        void Start () {
			originalPosition = transform.position;
            aspiration.SetActive(false);
            winParticles.SetActive(false);
        }

        public void showWinParticles()
        {
            //winParticles.SetActive(true);
        }
        public void hideWinParticles()
        {
            //winParticles.SetActive(false);
        }
        public void activate()
        {
            aspiration.SetActive(true);
            shake();
        }

        public void deactivate()
        {
            aspiration.SetActive(false);
            moveTweener = transform.DOMove(originalPosition, 0.5f);
        }
		
		// Update is called once per frame
		void Update () {
		
		}


		public void shake()
		{
			if (moveTweener != null)
			{
				moveTweener.Kill();
			}

            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Hit);
            if(upperTube)
                moveTweener = transform.DOLocalMoveY(originalPosition.y - 0.25f, 0.35f);//transform.DOShakePosition(0.5f, 0.2f, 1).OnComplete(delegate () { transform.position = originalPosition; });
            else
                moveTweener = transform.DOLocalMoveY(originalPosition.y + 0.25f, 0.35f);
        }
	}
}