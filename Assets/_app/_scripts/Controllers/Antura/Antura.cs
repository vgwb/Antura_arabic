using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public enum AnturaAnim {
        Nothing = 0,
        Run = 1,
        SitBreath = 2,
        StandBreath = 3,
        StandExcitedBreath = 4,
        StandExcitedLookR = 5,
        StandExcitedWagtail = 6,
        StandSadBreath = 7
    }

    public class Antura : MonoBehaviour
    {
        [Header("State")]
        public AnturaAnim AnimationState;
        public bool IsPirate;
        public int CollarSize;

        [Header("Scene References")]
        public Animator AnturaAnimator;
        public GameObject PropPirateHat;
        public GameObject PropPirateHook;
        public GameObject PropCollarA;
        public GameObject PropCollarB;
        public GameObject PropCollarC;

        void Start()
        {
            AnturaAnimator.Play(GetStateName(AnimationState));

            PropPirateHat.SetActive(IsPirate);
            PropPirateHook.SetActive(IsPirate);
            PropCollarA.SetActive(CollarSize == 1);
        }

        void OnMouseDown()
        {
            AudioManager.I.PlaySfx(Sfx.DogBarking);
        }

        string GetStateName(AnturaAnim state)
        {
            var stateName = "";
            switch (state) {
                case AnturaAnim.Nothing:
                    stateName = "";
                    break;
                case AnturaAnim.Run:
                    stateName = "Run";
                    break;
                case AnturaAnim.SitBreath:
                    stateName = "SitBreath";
                    break;
                case AnturaAnim.StandBreath:
                    stateName = "StandBreath";
                    break;
                case AnturaAnim.StandExcitedBreath:
                    stateName = "StandExcitedBreath";
                    break;
                case AnturaAnim.StandExcitedLookR:
                    stateName = "StandExcitedLookR";
                    break;
                case AnturaAnim.StandExcitedWagtail:
                    stateName = "StandExcitedWagtail";
                    break;
                case AnturaAnim.StandSadBreath:
                    stateName = "StandSadBreath";
                    break;
            }
            return stateName;
        }
    }
}