using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using DG.Tweening;
using System;

namespace EA4S {
    public class AssessmentObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Image Draw;
        public TextMeshProUGUI Label;
        public Image Circle;
        public Color Color;
        public ILivingLetterData data;

        public bool IsWord = false;
        public bool IsLocked = false;
        public bool IsInteractable = true;
        AssessmentManager manager;
        SpriteLineRenderer line;
        

        #region 
        /// <summary>
        /// Init object.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_isWord">if true word else is a draw.</param>
        public void Init(ILivingLetterData _data, bool _isWord) {
            data = _data;
            IsWord = _isWord;
            if (!IsWord) {
                Draw.sprite = _data.DrawForLivingLetter;
                //SetColor(_color);
                Label.gameObject.SetActive(false);
            } else {
                Label.text = _data.TextForLivingLetter;
                Draw.gameObject.SetActive(false);
            }
            HideCyrcle(0);
        }

        public void InjectManager(AssessmentManager _manager) {
            manager = _manager;
        }

        public void SetColor(Color _color) {
            Color = _color;
            Circle.color = _color;
        }

        public void HideCyrcle(float _time = 0) {
            Circle.DOFade(0, _time);
        }

        public void ShowCyrcle(float _time = 0) {
            Circle.DOFade(1, _time);
        }
        #endregion

        #region Line

        void Update() {
            if (line)
                line.OnDraw(transform.position, Input.mousePosition);
        }

        #endregion

        #region inputEvents

        public void OnPointerDown(PointerEventData eventData) {
            if (!IsInteractable)
                return;
            if (IsLocked)
                manager.UnlockObjects(Color);
            SetColor(manager.GetAvailableColor());
            line = manager.GetLine(this.Color);
            ShowCyrcle(1);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (!IsInteractable)
                return;
            foreach (var item in eventData.hovered) {
                AssessmentObject other = item.GetComponent<AssessmentObject>();
                if (!other)
                    continue;
                if (other && other.IsWord != IsWord) {
                    if (other.IsLocked)
                        manager.UnlockObjects(other.Color);
                    other.SetColor(Color);
                    other.ShowCyrcle(1);
                    other.IsLocked = IsLocked = true;
                    manager.OnReleaseOnWord(this, other);
                    line = null;
                    return;
                }
            }
            HideCyrcle(1);
            manager.UnlockObjects(Color);
            line = null;
        }

        #endregion
    }
}
