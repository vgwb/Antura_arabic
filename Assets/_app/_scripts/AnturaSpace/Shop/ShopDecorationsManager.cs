using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antura;
using Antura.Helpers;
using Antura.Utilities;

public class ShopDecorationsManager : SingletonMonoBehaviour<ShopDecorationsManager>
{
    private int maxDecorations { get { return allShopDecorations.Count; } }

    public bool HasDecorationsToUnlock
    {
        get
        {
            int nUnlocked = allShopDecorations.Count(x => !x.locked);
            return nUnlocked < maxDecorations;
        }
    }

    private List<ShopDecoration> allShopDecorations = new List<ShopDecoration>();
    private ShopState shopState;

    public void Initialise(ShopState shopState)
    {
        this.shopState = shopState;
        allShopDecorations = new List<ShopDecoration>(GetComponentsInChildren<ShopDecoration>());
        foreach (var shopDecoration in allShopDecorations)
	    {
	        shopDecoration.gameObject.SetActive(false);
	    }

        foreach (var id in shopState.unlockedDecorationsIDs)
        {
            allShopDecorations.Find(x => x.id == id).Unlock();
        }
    }
	
	public bool UnlockNewDecoration()
	{
	    if (!HasDecorationsToUnlock) return false;

	    var newDecoration = allShopDecorations.Where(x => x.locked).ToList().RandomSelectOne();
	    newDecoration.Unlock();

        shopState.unlockedDecorationsIDs.Add(newDecoration.id);
	    AppManager.I.Player.Save();

        return true;
	}

}
