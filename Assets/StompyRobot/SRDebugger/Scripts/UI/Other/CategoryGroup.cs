namespace SRDebugger.UI.Other
{
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;

    public class CategoryGroup : SRMonoBehaviourEx
    {
        [RequiredField] public RectTransform Container;

        [RequiredField] public Text Header;
    }
}
