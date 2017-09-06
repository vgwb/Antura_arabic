using Antura.Core;
using Antura.Helpers;
using Antura.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public enum ShopContext
    {
        Shopping,
        Placement
    }

    public class ShopDecorationsManager : SingletonMonoBehaviour<ShopDecorationsManager>
    {

        private int maxDecorations { get { return allShopDecorations.Count; } }

        public bool HasSlotsForDecoration(ShopDecorationObject decorationObjectToTest)
        {
            bool result = allShopDecorationSlots.Count(x => x.IsAssignableTo(decorationObjectToTest)) > 0;
            Debug.Log("Has slots? " + result);
            return result;
        }

        // TODO: delete this
        public bool HasDecorationsToUnlock {
            get {
                int nUnlocked = allShopDecorations.Count(x => !x.locked);
                return nUnlocked < maxDecorations;
            }
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
            Debug.Log("Decos: " + allShopDecorations.Count);

            allShopDecorationSlots = new List<ShopDecorationSlot>(GetComponentsInChildren<ShopDecorationSlot>());
            foreach (var slot in allShopDecorationSlots)
            {
                slot.OnSelect += SelectDecorationSlot;
            }
            Debug.Log("Slots: " + allShopDecorationSlots.Count);

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

            // Saved state TODO: UPDATE THIS
            shopState.unlockedDecorationsIDs.Add(newDecoration.id);
            AppManager.I.Player.Save();

            currentUnlockableObjectPrefab = null;
            shopContext = ShopContext.Shopping;
            foreach (var shopDecorationSlot in allShopDecorationSlots)
            {
                shopDecorationSlot.Highlight(false);
            }
        }

        public bool UnlockNewDecoration(ShopDecorationObject UnlockableDecorationObject)
        {
            // Place at the first available slot
            /*var selectedSlot = allShopDecorationSlots.FirstOrDefault(x => !x.Assigned && x.slotType == UnlockableDecorationObject.slotType);
            if (selectedSlot != null)
            {
                selectedSlot.Assign(newDecoration);
            }*/

            return true;
        }

    }
}
