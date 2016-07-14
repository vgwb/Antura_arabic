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

        public void Show(string _word, bool _completed, TweenCallback _callback = null) {
            AudioManager.I.PlaySfx(Sfx.UIPopup);
            // Preset for animation
            WordLable.text = _word;
            CompletedCheck.rectTransform.DOScale(6, 0);
            CompletedCheck.DOFade(0, 0);
            // Animation sequence
            sequence = DOTween.Sequence(); 
            tParms = new TweenParams()
                .SetEase(Ease.InOutBack);
            timeScaleAtMenuOpen = Time.timeScale;
            //Time.timeScale = 0; // not working
            //sequence.timeScale = 1;
            sequence.Append(GetComponent<RectTransform>().DOAnchorPos(ShowPosition, 0.2f).SetAs(tParms));
            if (_completed) {
                InfoLable.text = "Word Completed!";
                sequence.Insert(0.2f, CompletedCheck.DOFade(1, 0.2f));
                sequence.InsertCallback(0.1f, delegate () {
                    sequence.Append(CompletedCheck.rectTransform.DOScale(1, 0.3f).SetAs(tParms));
                });
            } else {
                InfoLable.text = "New Word!";
            }
            if (AutoCloseTime >= 0) {
                sequence.InsertCallback(AutoCloseTime, delegate { Close(sequence, tParms, _callback); });
            } else {
                pendingCallback = null; // reset
                if (_callback != null)
                    pendingCallback = _callback;
            }
        }

        public void Close() {
            Debug.Log("Close");
            Close(sequence, tParms, pendingCallback);
        }

        void Close(Sequence _sequence, TweenParams _tParms, TweenCallback _callback) {
            _sequence.Append(GetComponent<RectTransform>().DOAnchorPos(HidePosition, 0.15f).SetAs(_tParms));
            if (_callback != null)
                _callback();
        }

        public void Boh() {
            Debug.Log("Boh");
        }

        void OnDestroy() {
            sequence.Kill();
        }

    }
}