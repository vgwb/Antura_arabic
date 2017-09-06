using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopActionsPanelUI : MonoBehaviour
    {
        public GameObject shopActionUIPrefab;
        public Transform buttonsPivotTr;

        public void SetActions(ShopAction[] shopActions)
        {
            foreach (var shopAction in shopActions) {
                var shopActionUIgo = Instantiate(shopActionUIPrefab);
                shopActionUIgo.transform.SetParent(buttonsPivotTr);
                shopActionUIgo.transform.localScale = Vector3.one;
                shopActionUIgo.GetComponent<ShopActionUI>().SetAction(shopAction);
            }
        }
    }
}