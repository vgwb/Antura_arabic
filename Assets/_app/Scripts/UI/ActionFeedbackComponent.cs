using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UniRx;

[RequireComponent(typeof(RectTransform))]
public class ActionFeedbackComponent : MonoBehaviour {

    public Image OkImage;
    public Image KoImage;

    public Vector2 ShowPos = new Vector2(0, 150);
    public Vector2 HidePos = new Vector2(0, -150);

    bool show = false;
    bool feedback = false;
    RectTransform rt;

    // Use this for initialization
    void Start () {
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = HidePos;
    }

    /// <summary>
    /// Show feedback positive or negative.
    /// </summary>
    /// <param name="_feedback"></param>
    public void Show(bool _feedback) {
        OkImage.enabled = _feedback;
        KoImage.enabled = !_feedback;

        Sequence sequence = DOTween.Sequence();
        TweenParams tParms = new TweenParams()
            .SetEase(Ease.OutElastic);
        sequence.Play();
        sequence.Append(rt.DOAnchorPos(ShowPos, 0.3f).SetAs(tParms));
        sequence.AppendInterval(_feedback ? 0.5f : 0.2f);
        sequence.Append(rt.DOAnchorPos(HidePos, 0.3f).SetAs(tParms));
    }
}
