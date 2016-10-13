using UnityEngine;
using System.Collections;


namespace EA4S.MakeFriends
{
    public class LivingLetterArea : MonoBehaviour
    {
        public GameObject livingLetterPrefab;
        public bool left;
        public Vector3 offscreenPosition;
        public Vector3 startingPosition;
        public Vector3 celebrationPosition;
        public Vector3 friendlyExitPosition;
        public Vector3 entranceRotation;
        public Vector3 exitRotation;
        public Vector3 friendlyExitRotation;
        public float entranceDuration;
        public float celebrationDuration;
        public float friendlyExitDuration;
        public float speakDelay;
        public float movingAwayDuration;

        [HideInInspector]
        public LivingLetterController livingLetter;

        private Vector3 nextMovingAwayPosition;


        void Start()
        {
            nextMovingAwayPosition = startingPosition + (offscreenPosition - startingPosition) / 3f;
        }

        public void SpawnLivingLetter(WordData wordData)
        {
            var instance = Instantiate(livingLetterPrefab, offscreenPosition, Quaternion.Euler(entranceRotation), this.transform) as GameObject;
            livingLetter = instance.GetComponent<LivingLetterController>();
            livingLetter.Init(wordData);
            livingLetter.container = this.gameObject;
        }

        public void MakeEntrance()
        {
            livingLetter.MakeEntrance(offscreenPosition, startingPosition, entranceRotation, entranceDuration, speakDelay, exitRotation);
        }

        public void MakeFriendlyExit()
        {
            livingLetter.MakeFriendlyExit(friendlyExitPosition, friendlyExitRotation, friendlyExitDuration);
        }

        public void GoToFriendsZone(FriendsZone zone)
        {
            livingLetter.GoToFriendsZone(zone, left);
        }
            
        public void MoveAwayAngrily()
        {
            livingLetter.MoveAwayAngrily(nextMovingAwayPosition, exitRotation, movingAwayDuration, 1f);
            nextMovingAwayPosition += (offscreenPosition - startingPosition) / 3f;
        }

        public void Celebrate()
        {
            livingLetter.Celebrate(celebrationPosition, entranceRotation, celebrationDuration);
        }

        public void Reset()
        {
            livingLetter = null;
            var lingeringLetter = GetComponentInChildren<LivingLetterController>();
            if (lingeringLetter != null)
            {
                Destroy(lingeringLetter.gameObject);
            }
            nextMovingAwayPosition = startingPosition + (offscreenPosition - startingPosition) / 3f;
        }


    }
}