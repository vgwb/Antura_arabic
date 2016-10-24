// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/23

using UnityEngine;

namespace EA4S
{
    public class GamesSelectorBubble : MonoBehaviour
    {
        public GameObject Bg;
        public GameObject Cover; // Has collider
        public SpriteRenderer Ico;

        public bool IsOpen { get; private set; }

        #region Public Methods

        public void Setup(string _icoResourcePath, float _x)
        {
            Open(false);
            Ico.sprite = Resources.Load<Sprite>(_icoResourcePath);
            this.transform.localPosition = new Vector3(_x, 0, 0);
        }

        public void Open(bool _doOpen = true)
        {
            IsOpen = _doOpen;
            Cover.SetActive(!_doOpen);
            Bg.SetActive(_doOpen);
            Ico.gameObject.SetActive(_doOpen);

            if (_doOpen) {
                
            }
        }

        #endregion
    }
}