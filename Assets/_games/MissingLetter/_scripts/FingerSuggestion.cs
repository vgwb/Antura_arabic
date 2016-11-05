using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FingerSuggestion : MonoBehaviour
{
    Tween t;

    public void DoSuggestion(GameObject target)
    {
        gameObject.SetActive(true);
        transform.position = target.transform.position + Vector3.back * 3f + Vector3.up * 3;
        transform.parent = target.transform;
        DoSuggMove();
    }

    private void DoSuggMove()
    {
        t = transform.DOMoveZ(transform.position.z + 2, 0.7f).OnComplete(
          delegate
          {
              t = transform.DOMoveZ(transform.position.z - 2, 0.7f).OnComplete(DoSuggMove);
          });
    }

    void OnDisable()
    {
        t.Kill();
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}
