using System.Collections;
using System.Collections.Generic;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Profile
{
    [RequireComponent(typeof(UIButton))]
    public class PlayerIcon : MonoBehaviour
    {
        [Header("Debug - Disable for BUILD")]
        public bool Randomize = false;

        public void Init(PlayerIconData playerIconData)
        {
            SetAppearance(playerIconData.Gender, playerIconData.AvatarId, playerIconData.Tint, playerIconData.IsDemoUser);
        }

        void Start()
        {
            if (Randomize)
            {
                SetAppearance(
                    UnityEngine.Random.value <= 0.5f ? PlayerGender.F : PlayerGender.M,
                    UnityEngine.Random.Range(1, 5),
                    (PlayerTint)UnityEngine.Random.Range(1, 8),
                    UnityEngine.Random.value <= 0.2f
                );
            }
        }

        void SetAppearance(PlayerGender gender, int avatarId, PlayerTint tint, bool isDemoUser)
        {
            UIButton uiButton = this.GetComponent<UIButton>();
            uiButton.ChangeDefaultColor(isDemoUser ? new Color(0.4117647f, 0.9254903f, 1f, 1f) : PlayerTintConverter.ToColor(tint));
            uiButton.Ico.sprite = isDemoUser
                ? Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + "god")
                : Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + gender + avatarId);
        }
    }
}