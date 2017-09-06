using Antura.Core;
using Antura.Helpers;
using Antura.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationsManager : SingletonMonoBehaviour<ShopDecorationsManager>
    {

        public bool HasSlotsForDecoration(ShopDecorationObject decorationObjectToTest)
        {
            bool result = allShopDecorationSlots.Count(x => x.IsAssignableTo(decorationObjectToTest)) > 0;
            //Debug.Log("Has slots? " + result);
            return result;
        }

        private List<ShopDecorationObject> allShopDecorations = new List<ShopDecorationObject>();
        private List<ShopDecorationSlot> allShopDecorationSlots = new List<ShopDecorationSlot>();
        private ShopState shopState;
        private ShopContext shopContext;
        private ShopDecorationObject currentUnlockableObjectPrefab;

        public ShopContext ShopContext { get {  return shopContext; } }

        public void Initialise(ShopState shopState)
        {
            this.shopState = shopState;

            allShopDecorations = new List<ShopDecorationObject>(GetComponentsInChildren<ShopDecorationObject>());
            foreach (var shopDecoration in allShopDecorations) {
                shopDecoration.gameObject.SetActive(false);
            }
            //Debug.Log("Decorations: " + allShopDecorations.Count);

            allShopDecorationSlots = new List<ShopDecorationSlot>(GetComponentsInChildren<ShopDecorationSlot>());
            foreach (var slot in allShopDecorationSlots)
            {
                slot.OnSelect += SelectDecorationSlot;
            }
            //Debug.Log("Slots: " + allShopDecorationSlots.Count);

            // Load state
            foreach (var id in shopState.unlockedDecorationsIDs)
            {
                allShopDecorations.Find(x => x.id == id).Unlock();
            }

            // Initialise context
            currentUnlockableObjectPrefab = null;
            shopContext = ShopContext.Shopping;
        }

        public void PrepareNewDecorationPlacement(ShopDecorationObject UnlockableDecorationPrefab)
        {
            if (!HasSlotsForDecoration(UnlockableDecorationPrefab)) return;

            var availableSlots = allShopDecorationSlots.Where(x => x.IsAssignableTo(UnlockableDecorationPrefab));
            foreach (var shopDecorationSlot in availableSlots)
            {
                if (shopDecorationSlot.IsAssignableTo(UnlockableDecorationPrefab))
                {
                    shopDecorationSlot.Highlight(true);
                }
            }

            currentUnlockableObjectPrefab = UnlockableDecorationPrefab;
            shopContext = ShopContext.Placement;
        }

        public void SelectDecorationSlot(ShopDecorationSlot selectedSlot)
        {
            var newDecoration = Instantiate(currentUnlockableObjectPrefab);
            //var newDecoration = allShopDecorations.Where(x => x.locked).ToList().RandomSelectOne();
            newDecoration.Unlock();
            newDecoration.transform.localPosition = new Vector3(10000, 0, 0);

            selectedSlot.Assign(newDecoration);

            // Saved state TODO: UPDATE SAVE TO REFLECT THE NEW CHANGES
            shopState.unlockedDecorationsIDs.Add(newDecoration.id);
            AppManager.I.Player.Save();

            currentUnlockableObjectPrefab = null;
            shopContext = ShopContext.Shopping;
            foreach (var shopDecorationSlot in allShopDecorationSlots)
            {
                shopDecorationSlot.Highlight(false);
            }
        }

    }
}
