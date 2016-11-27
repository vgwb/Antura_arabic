// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/21

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class EndsessionMinigame : MonoBehaviour
    {
        [Tooltip("Alpha will be ignored")]
        public Color StarOffColor = Color.red;
        [Header("References")]
        public Transform Bubble;
        public Image Ico;
        public Image[] Stars;

        public int GainedStars { get; private set; }
        Color starDefColor;

        #region Unity

        void Awake()
        {
            StarOffColor.a = 1;
            starDefColor = Stars[0].color;
        }

        #endregion

        #region Public Methods

        internal void Reset()
        {
            // TODO
        }

        internal void SetIcon(Sprite _sprite)
        {
            Ico.sprite = _sprite;
        }

        internal void SetStars(int _numStars)
        {
            GainedStars = _numStars;
            for (int i = 0; i < Stars.Length; ++i) {
                Image star = Stars[i];
                star.gameObject.SetActive(true);
                if (i < _numStars) {
                    star.color = starDefColor;
                    star.transform.localScale = Vector3.one;
                } else {
                    star.color = StarOffColor;
                    star.transform.localScale = Vector3.one * 0.6f;
                }
            }
        }

        #endregion
    }
}