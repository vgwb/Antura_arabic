using Antura;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopActionUI : MonoBehaviour
{
    public Image iconUI;
    public TextMeshProUGUI amountUI;
    public Button buttonUI;

    private ShopAction shopAction;

    public void SetAction(ShopAction shopAction)
    {
        this.shopAction = shopAction;
        iconUI.sprite = shopAction.iconSprite;
        amountUI.text = shopAction.bonesCost.ToString();
        buttonUI.interactable = !shopAction.IsLocked;
    }

    public void OnClick()
    {
        if (AppManager.I.Player.GetTotalNumberOfBones() >= shopAction.bonesCost)
        {
            AppManager.I.Player.RemoveBones(shopAction.bonesCost);
            shopAction.PerformAction();
        }
    }

}
