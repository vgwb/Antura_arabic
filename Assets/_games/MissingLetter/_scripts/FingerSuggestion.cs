using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FingerSuggestion : MonoBehaviour
{
    Tween t;

    public void DoSuggestion()
    {
        t = transform.DOMoveZ(transform.position.z + 2, 0.7f).OnComplete(
            delegate
            {
                t = transform.DOMoveZ(transform.position.z - 2, 0.7f).OnComplete(DoSuggestion);
            });
    }

    void OnDisable()
    {
        t.Kill();
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}
