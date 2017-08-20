using Antura.Core;
using Antura.UI;
using DG.DeExtensions;
using DG.DeInspektor.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Profile
{
    [RequireComponent(typeof(UIButton))]
    public class PlayerIcon : MonoBehaviour
    {
        enum EndgameState
        {
            Unfinished,
            Finished,
            FinishedWAllStars
        }

        [Tooltip("If TRUE automatically initializes to the current player")]
        [DeToggleButton]
        public bool AutoInit;

        public Sprite EndgameHat, EndgameHatWStars;
        public Image HighlightImage;
        public Image HatImage;
        public Image IconImage;
        public string Uuid { get; private set; }

        public UIButton UIButton
        {
            get {
                if (fooUIButton == null) {
                    fooUIButton = this.GetComponent<UIButton>();
                }
                return fooUIButton;
            }
        }

        UIButton fooUIButton;

        #region Unity

        void Start()
        {
            if (!AutoInit) return;

            if (AppManager.I.PlayerProfileManager.CurrentPlayer != null) {
                Init(AppManager.I.PlayerProfileManager.CurrentPlayer.GetPlayerIconData());
            }
        }

        #endregion

        #region Public

        public void Init(PlayerIconData playerIconData)
        {
            Uuid = playerIconData.Uuid;
            //Debug.Log("playerIconData " + playerIconData.Uuid + " " + playerIconData.Gender + " " + playerIconData.AvatarId + " " + playerIconData.Tint + " " + playerIconData.IsDemoUser + " > " + playerIconData.HasFinishedTheGame + "/" + playerIconData.HasFinishedTheGameWithAllStars);
            EndgameState endgameState = playerIconData.HasFinishedTheGameWithAllStars
                ? EndgameState.FinishedWAllStars
                : playerIconData.HasFinishedTheGame
                    ? EndgameState.Finished
                    : EndgameState.Unfinished;
            SetAppearance(playerIconData.Gender, playerIconData.AvatarId, playerIconData.Tint, playerIconData.IsDemoUser, endgameState, playerIconData.HasMaxStarsInCurrentPlaySessions);
        }

        [DeMethodButton("DEBUG: Select", mode = DeButtonMode.PlayModeOnly)]
        public void Select(string _uuid)
        {
            UIButton.Toggle(Uuid == _uuid);
        }

        [DeMethodButton("DEBUG: Deselect", mode = DeButtonMode.PlayModeOnly)]
        public void Deselect()
        {
            UIButton.Toggle(false);
        }

        #endregion

        void SetAppearance(PlayerGender gender, int avatarId, PlayerTint tint, bool isDemoUser, EndgameState endgameState, bool hasMaxStarsInCurrentPlaySessions)
        {
            if (gender == PlayerGender.None) {
                Debug.LogWarning("Player gender set to NONE");
            }
            Color color = isDemoUser ? new Color(0.4117647f, 0.9254903f, 1f, 1f) : PlayerTintConverter.ToColor(tint);
            UIButton.Ico = IconImage;   // forced icon
            UIButton.ChangeDefaultColors(color, color.SetAlpha(0.5f));
            UIButton.Ico.sprite = isDemoUser
                ? Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + "god")
                : Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + (gender == PlayerGender.None ? "M" : gender.ToString()) +
                                         avatarId);
            HatImage.gameObject.SetActive(endgameState != EndgameState.Unfinished);

            switch (endgameState) {
                case EndgameState.Finished:
                    HatImage.sprite = EndgameHat;
                    break;
                case EndgameState.FinishedWAllStars:
                    HatImage.sprite = EndgameHatWStars;
                    break;
            }

            Debug.Log("hasMaxStarsInCurrentPlaySessions: " + hasMaxStarsInCurrentPlaySessions);
            HighlightImage.gameObject.SetActive(hasMaxStarsInCurrentPlaySessions);
        }

        [DeMethodButton("DEBUG: Randomize Appearance", mode = DeButtonMode.PlayModeOnly)]
        void RandomizeAppearance()
        {
            float rnd0 = UnityEngine.Random.value;
            float rnd1 = UnityEngine.Random.value;
            float rnd2 = UnityEngine.Random.value;
            float rnd3 = UnityEngine.Random.value;
            SetAppearance(
                rnd0 <= 0.5f ? PlayerGender.F : PlayerGender.M,
                UnityEngine.Random.Range(1, 5),
                (PlayerTint) UnityEngine.Random.Range(1, 8),
                rnd1 <= 0.2f,
                rnd2 < 0.33f ? EndgameState.Unfinished : rnd2 < 0.66f ? EndgameState.Finished : EndgameState.FinishedWAllStars,
                rnd3 <= 0.5f
            );
        }
    }
}