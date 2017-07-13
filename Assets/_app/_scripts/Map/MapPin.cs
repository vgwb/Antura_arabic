using Antura.Audio;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    ///     A pin on the map. Defines an assessment play session.
    /// </summary>
    public class MapPin : MonoBehaviour
    {
        public int learningBlock;
        //public int playSession;
        public int pos;

        public bool unlocked;

        [Header("References")]
        public GameObject Dot;
        public Material blackPin;
        public Material redPin;

        public GameObject pinV1;
        public GameObject pinV2;

        public void SetJourneyPosition(int lb, int ps)
        {
            learningBlock = lb;
            //this.playSession = ps;
            pinV1.gameObject.SetActive(ps % 2 == 0);
            pinV2.gameObject.SetActive(ps % 2 == 1);
        }

        private void Start()
        {
            if (unlocked)
            {
                Dot.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                transform.GetChild(0).gameObject.SetActive(false);
                ChangeMaterialPinToRed();
                if (other.gameObject.GetComponent<PlayerPin>().playerOverDotPin)
                {
                    AudioManager.I.PlaySound(Sfx.UIButtonClick);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                transform.GetChild(0).gameObject.SetActive(true);
                ChangeMaterialPinToBlack();
            }
        }

        public void ChangeMaterialPinToBlack()
        {
            Dot.GetComponent<Renderer>().material = blackPin;
        }

        public void ChangeMaterialPinToRed()
        {
            Dot.GetComponent<Renderer>().material = redPin;
        }
    }
}