// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/30

using System;
using DG.DeExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class PromptPanel : MonoBehaviour
    {
        public RectTransform Content;
        public TextMeshProUGUI TfMessage;
        public UIButton BtYes, BtNo;

        Action onYes, onNo;
        Tween showTween;

        #region Unity

        void Awake()
        {
            showTween = DOTween.Sequence().SetUpdate(true).SetAutoKill(false).Pause()
                .Append(this.GetComponent<Image>().DOFade(0, 0.35f).From())
                .Join(Content.DOScale(0.0001f, 0.35f).From().SetEase(Ease.OutBack))
                .OnRewind(() => this.gameObject.SetActive(false));

            BtYes.Bt.onClick.AddListener(()=> OnClick(BtYes));
            BtNo.Bt.onClick.AddListener(()=> OnClick(BtNo));

            this.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            showTween.Kill();
            BtYes.Bt.onClick.RemoveAllListeners();
            BtNo.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        public void Show(string _message, Action _onYes, Action _onNo)
        {
            TfMessage.text = _message.IsNullOrEmpty() ? "" : _message;
            onYes = _onYes;
            onNo = _onNo;

            showTween.Restart();
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            showTween.PlayBackwards();
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton _bt)
        {
            if (showTween.IsBackwards()) return;

            if (_bt == BtYes && onYes != null) onYes();
            else if (_bt == BtNo && onNo != null) onNo();
            Close();
        }

        #endregion
    }
}