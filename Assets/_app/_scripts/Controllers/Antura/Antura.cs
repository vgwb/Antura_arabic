using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public enum AnturaAnim {
        Nothing = 0,
        Run = 1,
        SitBreath = 2,
        SitBreathV2 = 3,
        StandBreath = 4,
        StandExcitedBreath = 5,
        StandExcitedLookR = 6,
        StandExcitedWagtail = 7,
        StandSadBreath = 8
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

    public enum AnturaEyes {
        Normal = 0,
        Closed = 1,
        Angry = 2,
        Injured = 3,
        Soso = 4,
        Normal2 = 5
    }

    public class Antura : MonoBehaviour
    {
        [Header("Behavior")]
        public bool ClickToBark;
        public bool ClickToChangeDress = false;
        public bool DisableAnimator = false;

        [Header("Starting State")]
        public AnturaAnim AnimationState;
        public bool IsPirate;
        public AnturaCollars AnturaCollar;
        public AnturaColors AnturaColor;
        public AnturaEyes AnturaEye;

        [Header("References")]
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

        [Header("Antura Eyes")]
        public Texture EyesNormal;
        public Texture EyesClosed;
        public Texture EyesAngry;
        public Texture EyesInjured;
        public Texture EyesSoso;
        public Texture EyesNormal2;

        public int CostumeId;

        void Start()
        {
            if (DisableAnimator) {
                AnturaAnimator.enabled = false;
            }
            PlayAnimation();

            if (CostumeId > 0) {
                SetPreset(CostumeId);
            }

            Refresh();
        }

        public void SetPreset(int id)
        {
            CostumeId = id;
            switch (id) {
                case 1:
                    IsPirate = false;
                    AnturaColor = AnturaColors.Yellow;
                    AnturaCollar = AnturaCollars.Small;
                    break;
                case 2:
                    IsPirate = false;
                    AnturaColor = AnturaColors.Pinata;
                    AnturaCollar = AnturaCollars.Big;
                    break;
                case 3:
                    IsPirate = true;
                    AnturaColor = AnturaColors.Pirate;
                    AnturaCollar = AnturaCollars.None;
                    break;
            }
            Refresh();
        }

        void PlayAnimation()
        {
            if (!DisableAnimator) {
                if (AnimationState != AnturaAnim.Nothing) {
                    AnturaAnimator.Play(GetStateName(AnimationState));
                } else {
                    AnturaAnimator.StopPlayback();
                }
            }
        }

        void Refresh()
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

            switch (AnturaEye) {
                case AnturaEyes.Normal:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesNormal);
                    break;
                case AnturaEyes.Closed:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesClosed);
                    break;
                case AnturaEyes.Angry:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesAngry);
                    break;
                case AnturaEyes.Injured:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesInjured);
                    break;
                case AnturaEyes.Soso:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesSoso);
                    break;
                case AnturaEyes.Normal2:
                    AnturaEyesMaterial.material.SetTexture("_MainTex", EyesNormal2);
                    break;
            }
            PlayAnimation();
        }

        void OnMouseDown()
        {
            if (ClickToChangeDress)
                RandomDress();

            if (ClickToBark)
                AudioManager.I.PlaySfx(Sfx.DogBarking);
        }

        void RandomDress()
        {
            IsPirate = (Random.Range(0, 100) > 80);
            AnturaColor = GenericUtilites.GetRandomEnum<AnturaColors>();
            AnturaCollar = GenericUtilites.GetRandomEnum<AnturaCollars>();
            AnturaEye = GenericUtilites.GetRandomEnum<AnturaEyes>();
            AnimationState = GenericUtilites.GetRandomEnum<AnturaAnim>();
            Refresh();
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
                case AnturaAnim.SitBreathV2:
                    stateName = "SitBreathV2";
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