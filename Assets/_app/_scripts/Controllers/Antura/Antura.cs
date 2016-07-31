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

    public enum AnturaCollars {
        None = 0,
        Small = 1,
        Medium = 2,
        Big = 3
    }

    public enum AnturaColors {
        Blue = 0,
        Pink = 1,
        Pirate = 2,
        Gray = 3,
        Pinata = 4,
        Strange = 5,
        Yellow = 7
    }

    public class Antura : MonoBehaviour
    {
        [Header("State")]
        public AnturaAnim AnimationState;
        public bool IsPirate;
        public AnturaCollars AnturaCollar;
        public AnturaColors AnturaColor;
        public bool EyesClosed;

        [Header("Scene References")]
        public Animator AnturaAnimator;
        public SkinnedMeshRenderer AnturaBodyMaterial;
        public SkinnedMeshRenderer AnturaEyesMaterial;

        [Header("Antura Props")]
        public GameObject PropPirateHat;
        public GameObject PropPirateHook;
        public GameObject PropCollarA;
        public GameObject PropCollarB;
        public GameObject PropCollarC;

        [Header("Antura Color")]
        public Material ColorBlue;
        public Material ColorPink;
        public Material ColorPirate;
        public Material ColorGray;
        public Material ColorPinata;
        public Material ColorStrange;
        public Material ColorYellow;

        int CostumeId;

        void Start()
        {
            AnturaAnimator.Play(GetStateName(AnimationState));
            RefreshDress();
            CostumeId = 0;
        }

        void RefreshDress()
        {
            PropPirateHat.SetActive(IsPirate);
            PropPirateHook.SetActive(IsPirate);
            PropCollarA.SetActive(AnturaCollar == AnturaCollars.Small);
            PropCollarB.SetActive(AnturaCollar == AnturaCollars.Medium);
            PropCollarC.SetActive(AnturaCollar == AnturaCollars.Big);

            switch (AnturaColor) {
                case AnturaColors.Blue:
                    AnturaBodyMaterial.material = ColorBlue;
                    break;
                case AnturaColors.Pink:
                    AnturaBodyMaterial.material = ColorPink;
                    break;
                case AnturaColors.Pirate:
                    AnturaBodyMaterial.material = ColorPirate;
                    break;
                case AnturaColors.Gray:
                    AnturaBodyMaterial.material = ColorGray;
                    break;
                case AnturaColors.Pinata:
                    AnturaBodyMaterial.material = ColorPinata;
                    break;
                case AnturaColors.Strange:
                    AnturaBodyMaterial.material = ColorStrange;
                    break;
                case AnturaColors.Yellow:
                    AnturaBodyMaterial.material = ColorYellow;
                    break;
            }
            AnturaAnimator.Play(GetStateName(AnimationState));
        }

        void OnMouseDown()
        {
            RandomDress();
            RefreshDress();
            AudioManager.I.PlaySfx(Sfx.DogBarking);
        }

        void RandomDress()
        {
            EyesClosed = (Random.Range(0, 100) > 50);
            IsPirate = (Random.Range(0, 100) > 50);
            AnturaColor = GetRandomEnum<AnturaColors>();
            AnturaCollar = GetRandomEnum<AnturaCollars>();
            AnimationState = GetRandomEnum<AnturaAnim>();
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

        T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }
    }
}