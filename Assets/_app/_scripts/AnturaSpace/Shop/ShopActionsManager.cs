using System.Collections.Generic;
using Antura;
using UnityEngine;

public class ShopState
{
    public List<string> unlockedDecorationsIDs = new List<string>();

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ShopState CreateFromJson(string jsonData)
    {
        var shopState = JsonUtility.FromJson<ShopState>(jsonData);
        if (shopState == null) shopState = new ShopState();
        return shopState;
    }

}

public class ShopActionsManager : MonoBehaviour
{
    public ShopActionsPanelUI ShopActionsPanelUi;
    public ShopDecorationsManager ShopDecorationsManager;

    private ShopAction[] shopActions;

	void Start ()
	{
	    AppManager.I.Player.AddBones(20);

        // Setup the decorations manager
        var shopState = AppManager.I.Player.CurrentShopState;
        ShopDecorationsManager.Initialise(shopState);

        // Setup actions
        shopActions = GetComponentsInChildren<ShopAction>();
        foreach (var shopAction in shopActions)
	    {
	        shopAction.InitialiseLockedState();
	    }
	    ShopActionsPanelUi.SetActions(shopActions);

	}
	
}
