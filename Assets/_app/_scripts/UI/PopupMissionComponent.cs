using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;

namespace EA4S {
    public class PopupMissionComponent : MonoBehaviour {

        public Image CompletedCheck;
        public TextMeshProUGUI InfoLable;
        public TextMeshProUGUI WordLable;
        public Image WordDraw;
        public Button ContinueButton;
        [Tooltip("Auto close popup after seconds indicated. If -1 autoclose is disabled and appear close button for that.")]
        public float AutoCloseTime = -1;

        Vector2 HidePosition;
        Vector2 ShowPosition;

        Sequence sequence;
        TweenParams tParms;
        TweenCallback pendingCallback = null;

        float timeScaleAtMenuOpen = 1;
        

        void Start() { 
            HidePosition = new Vector2(0, -750);
            ShowPosition = new Vector2(0, 0);
            GetComponent<RectTransform>().anchoredPosition = HidePosition;
            //ContinueButton.gameObject.SetActive(AutoCloseTime < 0);
        } 

        public void Show(string _word, WordData _wordData, bool _completed, TweenCallback _callback = null) {
            AudioManager.I.PlaySfx(Sfx.UIPopup);
            // Preset for animation
            WordLable.text = _word;
            CompletedCheck.rectTransform.DOScale(6, 0);
            CompletedCheck.DOFade(0, 0);
            // Animation sequence
            sequence = DOTween.Sequence().SetUpdate(true);
            tParms = new TweenParams()
                .SetEase(Ease.InOutBack);
            timeScaleAtMenuOpen = Time.timeScale;
            Time.timeScale = 0; // not working
            sequence.Append(GetComponent<RectTransform>().DOAnchorPos(ShowPosition, 0.3f).SetAs(tParms));
            if (_completed) {
                InfoLable.text = "Word Completed!";
                sequence.Insert(0.3f, CompletedCheck.DOFade(1, 0.1f));
                sequence.Append(CompletedCheck.rectTransform.DOScale(1, 0.3f).SetAs(tParms)).OnComplete(delegate() {
                    AudioManager.I.PlaySfx(Sfx.Hit);
                });
            } else {
                InfoLable.text = "New Word!";
            }
            WordDraw.sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + _wordData.Key);
            if (AutoCloseTime >= 0) {
                sequence.InsertCallback(AutoCloseTime, delegate { Close(sequence, tParms, _callback); });
            } else {
                pendingCallback = null; // reset
                if (_callback != null)
                    pendingCallback = _callback;
            }
        }

        public void Close() {
            Close(sequence, tParms, pendingCallback);
        }

        /// <summary>
        /// Close popup with actal sequence and callback.
        /// </summary>
        /// <param name="_sequence"></param>
        /// <param name="_tParms"></param>
        /// <param name="_callback"></param>
        void Close(Sequence _sequence, TweenParams _tParms, TweenCallback _callback) {
            Time.timeScale = 1;
            _sequence.Append(GetComponent<RectTransform>().DOAnchorPos(HidePosition, 0.15f).SetAs(_tParms));
            if (_callback != null)
                _callback();
        }

        void OnDestroy() {
            sequence.Kill();
        }

    }
}