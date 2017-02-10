using UnityEngine;
using DG.Tweening;

namespace EA4S.Minigames.TakeMeHome
{
	
    public class TakeMeHomeTube : MonoBehaviour {

		Tweener moveTweener;

		Vector3 originalPosition;
        public bool upperTube;
        public Transform enterance;
        public GameObject aspiration;
        public GameObject winParticles;
        public GameObject cubeInfo;
        TakeMeHomeTremblingTube trembling;
        BoxCollider coll;

        Vector3 collStartSize;
        // Use this for initialization
        void Start () {
			originalPosition = transform.position;
            aspiration.SetActive(false);
            winParticles.SetActive(false);
            trembling = GetComponent<TakeMeHomeTremblingTube>();
            coll = GetComponent<BoxCollider>();
            collStartSize = coll.size;
        }

        public void showWinParticles()
        {
            //winParticles.SetActive(true);
        }
        public void hideWinParticles()
        {
            //winParticles.SetActive(false);
        }
        public void activate(TakeMeHomeLL ll)
        {
            aspiration.SetActive(true);
            //shake();
            trembling.Trembling = true;
            ll.NearTube = this;
            //coll.size += Vector3.one * 0.9f; 
        }

        public void deactivate(TakeMeHomeLL ll)
        {
            aspiration.SetActive(false);
            //moveTweener = transform.DOMove(originalPosition, 0.5f);
            trembling.Trembling = false;
            ll.NearTube = null;
            //coll.size = collStartSize;
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