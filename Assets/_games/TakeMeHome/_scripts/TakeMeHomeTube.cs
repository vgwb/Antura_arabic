using UnityEngine;
using System.Collections;
using DG.Tweening;


namespace EA4S.TakeMeHome
{
	
public class TakeMeHomeTube : MonoBehaviour {

		Tweener moveTweener;

		Vector3 originalPosition;
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
        }

        public void deactivate()
        {
            aspiration.SetActive(false);
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
			AudioManager.I.PlaySfx (Sfx.Hit);
			moveTweener = transform.DOShakePosition (0.5f, 0.2f, 1).OnComplete(delegate () { transform.position = originalPosition; });
		}
	}
}