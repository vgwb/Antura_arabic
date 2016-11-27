using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ModularFramework.Core;

namespace EA4S
{
    [Obsolete("Replaced by EA4S.GameResultUI (but always use the <code>Minigames Interface</code> to access it)")]
    public class StarFlowers : MonoBehaviour
    {
        //public static StarFlowers I;

        //public Image Flower1, Flower2, Flower3, Japan1, Japan2, Bbackground;

        //string nextSceneName = string.Empty;

        //void Awake()
        //{
        //    I = this;

        //    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //    foreach (Image img in GetComponentsInChildren<Image>())
        //    {
        //        img.DOFade(0, 0);
        //        img.raycastTarget = false;
        //    }
        //}

        //public void Show(int _stars)
        //{
        //    foreach (Image img in GetComponentsInChildren<Image>())
        //    {
        //        img.raycastTarget = true;
        //    }
        //    Bbackground.raycastTarget = false;

        //    //if(_stars > 0)
        //    nextSceneName = AppManager.I.MiniGameDone();

        //    this.gameObject.SetActive(true);
        //    // Reset zone
        //    Vector2 f1pos = Flower1.rectTransform.anchoredPosition;
        //    Flower1.rectTransform.anchoredPosition = new Vector2(f1pos.x, -f1pos.y);
        //    Vector2 f2pos = Flower2.rectTransform.anchoredPosition;
        //    Flower2.rectTransform.anchoredPosition = new Vector2(f2pos.x, -f2pos.y);
        //    Vector2 f3pos = Flower3.rectTransform.anchoredPosition;
        //    Flower3.rectTransform.anchoredPosition = new Vector2(f3pos.x, -f3pos.y);


        //    Sequence sequence = DOTween.Sequence();
        //    TweenParams tParms = new TweenParams()
        //        .SetEase(Ease.InOutBack);

        //    sequence.Append(Bbackground.DOFade(1, 0.3f))
        //        .Insert(0f, Japan1.DOFade(1, 0.3f).SetAs(tParms))
        //        .Insert(0f, Japan2.DOFade(1, 0.3f).SetAs(tParms));
        //    //.Insert(0.5f, Japan1.transform.DORotate(new Vector2(360, 0), 15).SetLoops(-1));;

        //    if (_stars > 0)
        //    {
        //        sequence.Append(Flower3.DOFade(1, 0.1f));
        //        sequence.Append(Flower3.rectTransform.DOAnchorPos(f3pos, 0.3f).SetAs(tParms));
        //        sequence.AppendCallback(() => AudioManager.I.PlaySfx(Sfx.StarFlower));
        //    }

        //    if (_stars > 1)
        //    {
        //        sequence.Append(Flower2.DOFade(1, 0.1f));
        //        sequence.Append(Flower2.rectTransform.DOAnchorPos(f2pos, 0.3f).SetAs(tParms));
        //        sequence.AppendCallback(() => AudioManager.I.PlaySfx(Sfx.StarFlower));
        //    }

        //    if (_stars > 2)
        //    {
        //        sequence.Append(Flower1.DOFade(1, 0.1f));
        //        sequence.Append(Flower1.rectTransform.DOAnchorPos(f1pos, 0.3f).SetAs(tParms));
        //        sequence.AppendCallback(() => AudioManager.I.PlaySfx(Sfx.StarFlower));
        //    }

        //    sequence.Play().OnComplete(ShowButton);
        //}

        //void ShowButton()
        //{
        //    ContinueScreen.Show(Continue, ContinueScreenMode.Button);
        //}

        //public void Continue()
        //{
        //    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(nextSceneName);
        //    nextSceneName = string.Empty;
        //}

        //void OnDestroy()
        //{
        //    I = null;
        //}
    }
}