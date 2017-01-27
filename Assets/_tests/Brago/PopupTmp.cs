using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class PopupTmp : MonoBehaviour
{

    public static PopupTmp I;
    public static bool IsShown { get; private set; }

    public GameObject Window;
    public TextMeshProUGUI TitleGO;
    public TextMeshProUGUI WordTextGO;
    public GameObject ButtonGO;

    Action currentCallback;
    Tween showTween = null;

    void Awake()
    {

        //showTween = this.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).From()
        //    .SetEase(Ease.OutBack).SetAutoKill(false).Pause()
        //    .OnPlay(() => this.gameObject.SetActive(true))
        //    .OnRewind(() => this.gameObject.SetActive(false));

        //this.gameObject.SetActive(false);
    }

    public void Show(bool _doShow, string text)
    {
        if (_doShow) {
            WordTextGO.text = text;
            //showTween.PlayForward();
            GetComponent<RectTransform>().position = new Vector2(Screen.width / 2, Screen.height / 2);

        } else {
            showTween.PlayBackwards();
            gameObject.SetActive(false);
        }
    }
}
