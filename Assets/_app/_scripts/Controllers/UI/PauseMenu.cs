using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class PauseMenu : MonoBehaviour
    {

        public static PauseMenu I;

        [Header("Buttons")]
        public MenuButton BtPause;
        public MenuButton BtExit, BtRestart, BtMusic, BtFx, BtResume;
        [Header("Other")]
        public GameObject PauseMenuContainer;
        public Image MenuBg;
        public RectTransform AnturaFace;

        public bool IsMenuOpen { get; private set; }

//        Button btPause;
        MenuButton[] menuBts;
        float timeScaleAtMenuOpen = 1;
        Sequence openMenuTween;
        Tween anturaBobTween;

        void Awake() {
            I = this;
        }

        void Start() {
            menuBts = PauseMenuContainer.GetComponentsInChildren<MenuButton>(true);

            // Tweens - Antura face bobbing
            anturaBobTween = AnturaFace.DORotate(new Vector3(0, 0, -20), 0.6f).SetRelative().SetUpdate(true).SetAutoKill(false).Pause()
                .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            // Tweens - menu
            CanvasGroup[] cgButtons = new CanvasGroup[menuBts.Length];
            for (int i = 0; i < menuBts.Length; i++) cgButtons[i] = menuBts[i].GetComponent<CanvasGroup>();
            openMenuTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .OnPlay(() =>
                {
                    PauseMenuContainer.SetActive(true);
                    anturaBobTween.Restart();
                })
                .OnRewind(() =>
                {
                    PauseMenuContainer.SetActive(false);
                    anturaBobTween.Rewind();
                });
            openMenuTween.Append(MenuBg.DOFade(0, 0.5f).From())
                .Join(AnturaFace.DOAnchorPosY(750f, 0.6f).From().SetEase(Ease.OutBack));
            const float btDuration = 0.16f;
            for (int i = 0; i < menuBts.Length; ++i) {
                CanvasGroup cgButton = cgButtons[i];
                RectTransform rtButton = cgButton.GetComponent<RectTransform>();
                openMenuTween.Insert(i * 0.05f, rtButton.DOScale(cgButton.transform.localScale * 2f, btDuration).From().SetEase(Ease.InQuad))
                    .Join(rtButton.DOAnchorPosX(100f + 50f * i, btDuration * 2f).From(true).SetEase(Ease.OutQuad))
                    .Join(cgButton.DOFade(0, btDuration).From().SetEase(Ease.InQuad));
            }

            // Deactivate pause menu
            PauseMenuContainer.SetActive(false);

            // Listeners
            BtPause.Bt.onClick.AddListener(() => OnClick(BtPause));
            foreach (MenuButton bt in menuBts) {
                MenuButton b = bt; // Redeclare to fix Unity's foreach issue with delegates
                b.Bt.onClick.AddListener(() => OnClick(b));
            }
        }

        void OnDestroy() {
            openMenuTween.Kill();
            anturaBobTween.Kill();
            BtPause.Bt.onClick.RemoveAllListeners();
            foreach (MenuButton bt in menuBts)
                bt.Bt.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Opens or closes the pause menu
        /// </summary>
        /// <param name="_open">If TRUE opens, otherwise closes</param>
        public void OpenMenu(bool _open) {
            IsMenuOpen = _open;
            
            // Set toggles
            BtMusic.Toggle(AudioManager.I.MusicEnabled);
            BtFx.Toggle(CameraGameplayController.I.FxEnabled);

            if (_open) {
                timeScaleAtMenuOpen = Time.timeScale;
                Time.timeScale = 0;
                if (AppManager.Instance.CurrentGameManagerGO != null)
                    AppManager.Instance.CurrentGameManagerGO.SendMessage("DoPause", true, SendMessageOptions.DontRequireReceiver);
                openMenuTween.timeScale = 1;
                openMenuTween.PlayForward();
                AudioManager.I.PlaySfx(Sfx.UIPauseIn);
            } else {
                Time.timeScale = timeScaleAtMenuOpen;
                openMenuTween.timeScale = 2; // Speed up tween when going backwards
                if (AppManager.Instance.CurrentGameManagerGO != null)
                    AppManager.Instance.CurrentGameManagerGO.SendMessage("DoPause", false, SendMessageOptions.DontRequireReceiver);
                openMenuTween.PlayBackwards();
                AudioManager.I.PlaySfx(Sfx.UIPauseOut);
            }
        }

        /// <summary>
        /// Callback for button clicks
        /// </summary>
        void OnClick(MenuButton _bt) {

            if (_bt == BtPause) {
                OpenMenu(!IsMenuOpen);
            } else if (!openMenuTween.IsPlaying()) { // Ignores pause menu clicks when opening/closing menu
                switch (_bt.Type) {
                case MenuButtonType.Back: // Exit
                    OpenMenu(false);
                    AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Start");
                    break;
                case MenuButtonType.MusicToggle: // Music on/off
                    AudioManager.I.ToggleMusic();
                    BtMusic.Toggle(AudioManager.I.MusicEnabled);
                    break;
                case MenuButtonType.FxToggle: // FX on/off
                    CameraGameplayController.I.EnableFX(!CameraGameplayController.I.FxEnabled);
                    BtFx.Toggle(CameraGameplayController.I.FxEnabled);
                    break;
                case MenuButtonType.Restart: // Restart
                    OpenMenu(false);
                    break;
                case MenuButtonType.Continue: // Resume
                    OpenMenu(false);
                    break;
                }
            }
        }
    }
}
