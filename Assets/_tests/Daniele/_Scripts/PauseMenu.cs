using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CGL.Antura.DTest
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons Containers")]
        public CanvasGroup CgPause;
        public CanvasGroup CgExit, CgRestart, CgMusic, CgResume;
        [Header("Other")]
        public GameObject PauseMenuContainer;
        public Image MenuBg;
        public RectTransform AnturaFace;

        public bool IsMenuOpen { get; private set; }

        Button btPause;
        Button[] menuBts;
        float timeScaleAtMenuOpen = 1;
        Sequence openMenuTween;
        Tween anturaBobTween;

        void Start()
        {
            btPause = CgPause.GetComponentInChildren<Button>();
            menuBts = PauseMenuContainer.GetComponentsInChildren<Button>(true);

            // Tweens - Antura face bobbing
            anturaBobTween = AnturaFace.DORotate(new Vector3(0, 0, -20), 0.6f).SetRelative().SetUpdate(true).SetAutoKill(false).Pause()
                .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            // Tweens - menu
            CanvasGroup[] cgButtons = new[] { CgExit, CgRestart, CgMusic, CgResume };
            openMenuTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .OnPlay(() => {
                    PauseMenuContainer.SetActive(true);
                    anturaBobTween.Restart();
                })
                .OnRewind(() => {
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
            btPause.onClick.AddListener(()=> OnClick(btPause));
            foreach (Button bt in menuBts) {
                Button b = bt; // Redeclare to fix Unity's foreach issue with delegates
                b.onClick.AddListener(()=> OnClick(b));
            }
        }

        void OnDestroy()
        {
            openMenuTween.Kill();
            anturaBobTween.Kill();
            btPause.onClick.RemoveAllListeners();
            foreach (Button bt in menuBts) bt.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Opens or closes the pause menu
        /// </summary>
        /// <param name="_open">If TRUE opens, otherwise closes</param>
        public void OpenMenu(bool _open)
        {
            IsMenuOpen = _open;
            if (_open) {
                timeScaleAtMenuOpen = Time.timeScale;
                Time.timeScale = 0;
                openMenuTween.timeScale = 1;
                openMenuTween.PlayForward();
            } else {
                Time.timeScale = timeScaleAtMenuOpen;
                openMenuTween.timeScale = 2; // Speed up tween when going backwards
                openMenuTween.PlayBackwards();
            }
        }

        /// <summary>
        /// Callback for button clicks
        /// </summary>
        void OnClick(Button bt)
        {

            if (bt == btPause) {
                OpenMenu(!IsMenuOpen);
            } else if (!openMenuTween.IsPlaying()) { // Ignores pause menu clicks when opening/closing menu
                int menuBtIndex = Array.IndexOf(menuBts, bt);
                switch (menuBtIndex) {
                case 0: // Exit
                    OpenMenu(false);
                    break;
                case 1: // Music on/off
                    // TODO
                    break;
                case 2: // Restart
                        
                    OpenMenu(false);
                    break;
                case 3: // Resume
                    OpenMenu(false);
                    // TODO
                    break;
                }
            }
        }
    }
}
