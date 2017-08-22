using Antura.Core;
using DG.DeExtensions;
using UnityEngine;

namespace Antura.UI
{
    /// <summary>
    /// Mirrors UI children transforms to match reading direction
    /// </summary>
    public class ReadingDirectionMirror : MonoBehaviour
    {
        void Awake()
        {
            foreach (Transform childTr in transform)
            {
                switch (AppConstants.ReadingDirection)
                {
                    case TextFlow.LeftToRight:
                        // Default
                        break;
                    case TextFlow.RightToLeft:
                        var rectTransform = childTr.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            rectTransform.SetAnchoredPosX(-rectTransform.anchoredPosition.x);
                        }
                        break;
                }
            }

        }
    }
}