using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class FadeInFadeOut : MonoBehaviour
    {
        Tween showTween;

        void Start()
        {
//        showTween = this.GetComponent<Image>().DOFade(0, 0.36f).From().SetEase(Ease.Linear).SetAutoKill(false)
//            .OnRewind(()=> this.gameObject.SetActive(false));
        }
    }
}
