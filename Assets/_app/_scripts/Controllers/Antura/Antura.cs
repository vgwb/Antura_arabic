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
        Pirate = 2
    }

    public class Antura : MonoBehaviour
    {
        [Header("State")]
        public AnturaAnim AnimationState;
        public bool IsPirate;
        public AnturaCollars AnturaCollar;
        public AnturaColors AnturaColor;

        [Header("Scene References")]
        public Animator AnturaAnimator;
        public Material AnturaBodyMaterial;

        [Header("Antura Props")]
        public GameObject PropPirateHat;
        public GameObject PropPirateHook;
        public GameObject PropCollarA;
        public GameObject PropCollarB;
        public GameObject PropCollarC;

        [Header("Antura Color")]
        public Texture TextureBlue;
        public Texture TexturePink;
        public Texture TexturePirate;

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
                    AnturaBodyMaterial.SetTexture("_MainTex", TextureBlue);
                    break;
                case AnturaColors.Pink:
                    AnturaBodyMaterial.SetTexture("_MainTex", TexturePink);
                    break;
                case AnturaColors.Pirate:
                    AnturaBodyMaterial.SetTexture("_MainTex", TexturePirate);
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