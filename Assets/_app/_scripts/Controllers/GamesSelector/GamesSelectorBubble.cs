// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/23

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class GamesSelectorBubble : MonoBehaviour
    {
        public GameObject Main;
        public GameObject Cover; // Has collider
        public SpriteRenderer Ico;
        public ParticleSystem PouffParticleSys;

        public bool IsOpen { get; private set; }
        Tween shakeTween, openTween;

        #region Unity

        void OnDestroy()
        {
            shakeTween.Kill(true);
            openTween.Kill(true);
        }

        #endregion

        #region Public Methods

        public void Setup(string _icoResourcePath, float _x)
        {
            Open(false);
            Ico.sprite = Resources.Load<Sprite>(_icoResourcePath);
            this.transform.localPosition = new Vector3(_x, 0, 0);
            shakeTween = DOTween.Sequence().SetLoops(-1, LoopType.Yoyo)
                .Append(Cover.transform.DOShakeScale(4, 0.035f, 6, 90f, false))
                .Join(Cover.transform.DOShakeRotation(7, 7, 3, 90f, false));
        }

        public void Open(bool _doOpen = true)
        {
            IsOpen = _doOpen;
            Cover.SetActive(!_doOpen);
            Main.SetActive(_doOpen);

            if (_doOpen) {
                PouffParticleSys.gameObject.SetActive(true);
                PouffParticleSys.time = 0;
                PouffParticleSys.Play();
                shakeTween.Kill(true);
                openTween = Main.transform.DOPunchRotation(new Vector3(0, 0, 45), 0.75f);
                AudioManager.I.PlaySfx(Sfx.Poof);
            } else {
                PouffParticleSys.Stop();
                PouffParticleSys.Clear();
                PouffParticleSys.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}