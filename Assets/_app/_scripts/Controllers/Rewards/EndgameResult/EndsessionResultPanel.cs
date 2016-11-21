// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/20

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ModularFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class EndsessionResultPanel : MonoBehaviour
    {
        [Header("Settings")]
        public float Godrays360Duration = 15f;
        [Header("References")]
        public EndsessionMinigames Minigames;
        public EndsessionBar Bar;
        public CanvasGroup GodraysCanvas;
        public RectTransform Godray0, Godray1;

        bool setupDone;
        List<RectTransform> releasedMinigamesStars;
        Tween showTween, godraysTween;
        Sequence minigamesStarsToBarTween;

        #region Unity + Setup

        void Setup()
        {
            if (setupDone) return;

            setupDone = true;

            showTween = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(this.GetComponent<Image>().DOFade(0, 0.35f).From().SetEase(Ease.Linear))
                .Join(GodraysCanvas.DOFade(0, 0.35f).From().SetEase(Ease.Linear))
                .OnRewind(() => {
                    this.gameObject.SetActive(false);
                    godraysTween.Pause();
                });
            godraysTween = DOTween.Sequence().SetAutoKill(false).Pause().SetLoops(-1, LoopType.Restart)
                .Append(Godray0.DORotate(new Vector3(0, 0, 360), Godrays360Duration, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear))
                .Join(Godray1.DORotate(new Vector3(0, 0, -360), Godrays360Duration, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear));

            this.gameObject.SetActive(false);
        }

        void Awake()
        {
            Setup();
        }

        void OnDestroy()
        {
            this.StopAllCoroutines();
            showTween.Kill();
            godraysTween.Kill();
            minigamesStarsToBarTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Show(List<EndsessionResultData> _sessionData, bool _immediate)
        {
            ContinueScreen.Close(true);
            Hide(true);
            Setup();

            this.StopAllCoroutines();
            if (_immediate) showTween.Complete();
            else showTween.Restart();
            godraysTween.Restart();
            this.gameObject.SetActive(true);
            this.StartCoroutine(CO_Show(_sessionData));
        }

        public void Hide(bool _immediate)
        {
            if (!setupDone) return;

            ContinueScreen.Close(true);
            this.StopAllCoroutines();
            if (_immediate) showTween.Rewind();
            else showTween.PlayBackwards();
            Bar.Hide();
            Minigames.Hide();
            minigamesStarsToBarTween.Kill();
            if (releasedMinigamesStars != null) {
                foreach (RectTransform rt in releasedMinigamesStars) Destroy(rt.gameObject);
                releasedMinigamesStars = null;
            }
        }

        #endregion

        #region Methods

        IEnumerator CO_Show(List<EndsessionResultData> _sessionData)
        {
            yield return null;

            // Show minigames
            Bar.Hide();
            Minigames.Show(_sessionData);
            yield return new WaitForSeconds(1);

            // Show bar
            Bar.Show(_sessionData.Count * 3);
            while (!Bar.ShowTween.IsComplete()) yield return null;

            // Start filling bar and/or show Continue button
            releasedMinigamesStars = Minigames.CloneStarsToMainPanel();
            if (releasedMinigamesStars.Count > 0) {
                minigamesStarsToBarTween = DOTween.Sequence();
                Vector2 to = Bar.GetComponent<RectTransform>().anchoredPosition;
                for (int i = 0; i < releasedMinigamesStars.Count; ++i) {
                    RectTransform mgStar = releasedMinigamesStars[i];
                    minigamesStarsToBarTween.Insert(i * 0.2f, mgStar.DOAnchorPos(to, 0.3f).OnComplete(() => Bar.IncreaseBy(1)))
                        .Join(mgStar.GetComponent<Image>().DOFade(0, 0.2f).SetDelay(0.1f).SetEase(Ease.InQuad))
                        .Join(mgStar.DORotate(new Vector3(0, 0, 180), 0.3f));
                }
                yield return new WaitForSeconds(minigamesStarsToBarTween.Duration());
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