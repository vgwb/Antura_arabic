// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/18

using System.Collections;
using DG.DeExtensions;
using DG.Tweening;
using ModularFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class EndgameResultPanel : MonoBehaviour
    {
        public Image Rays;
        public EndgameStar[] Stars;

        bool setupDone;
        int numStars;
        RectTransform raysRT;
        Tween showTween, bgTween;

        #region Unity + Setup

        void Setup()
        {
            if (setupDone) return;

            setupDone = true;
            raysRT = Rays.GetComponent<RectTransform>();

            showTween = this.GetComponent<RectTransform>().DOAnchorPosY(-960, 0.35f).From()
                .SetEase(Ease.OutBack).SetAutoKill(false).Pause()
                .OnRewind(()=> this.gameObject.SetActive(false))
                .OnComplete(()=> this.StartCoroutine(CO_ShowComplete()));
            bgTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(Rays.DOFade(0, 0.35f).From())
                .Join(Rays.transform.DORotate(new Vector3(0, 0, 360), 9f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(9999));

            this.gameObject.SetActive(false);
        }

        void Awake()
        {
            Setup();
        }

        void OnDestroy()
        {
            showTween.Kill();
            bgTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Show(bool _doShow, int _numStars = 0)
        {
            Setup();

            this.StopAllCoroutines();
            bgTween.Rewind();
            ContinueScreen.Close(true);
            if (_doShow) {
                numStars = _numStars;
                this.gameObject.SetActive(true);
                foreach (EndgameStar star in Stars) star.Reset();
                showTween.Restart();
                this.gameObject.SetActive(true);
            } else showTween.PlayBackwards();
        }

        #endregion

        #region Methods

        IEnumerator CO_ShowComplete()
        {
            int id = 0;
            while (id < numStars) {
                Stars[id].Gain();
                yield return new WaitForSeconds(0.2f);
                id++;
            }

            if (numStars > 0) {
                if (numStars == 1) raysRT.SetAnchoredPosX(Stars[0].RectT.anchoredPosition.x);
                else if (numStars == 2) raysRT.SetAnchoredPosX(Stars[0].RectT.anchoredPosition.x + (Stars[1].RectT.anchoredPosition.x - Stars[0].RectT.anchoredPosition.x) * 0.5f);
                else raysRT.SetAnchoredPosX(0);
                bgTween.Restart();
            }

            ContinueScreen.Show(Continue, ContinueScreenMode.Button);
        }

        void Continue()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(AppManager.Instance.MiniGameDone());
        }

        #endregion
    }
}