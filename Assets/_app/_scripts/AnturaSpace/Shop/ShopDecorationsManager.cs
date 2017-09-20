using System;
using System.Collections;
using Antura.Core;
using Antura.Helpers;
using Antura.Utilities;
using System.Collections.Generic;
using System.Linq;
using Antura.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopDecorationsManager : SingletonMonoBehaviour<ShopDecorationsManager>
    {

        public Transform deletePropButtonTransform;

        private List<ShopDecorationObject> allShopDecorations = new List<ShopDecorationObject>();
        private List<ShopDecorationSlot> allShopDecorationSlots = new List<ShopDecorationSlot>();
        private ShopState shopState;
        private ShopContext shopContext;
        private ShopDecorationObject currentUnlockableObjectPrefab;

        public bool HasSlotsForDecoration(ShopDecorationObject decorationObjectToTest)
        {
            bool result = allShopDecorationSlots.Count(x => x.IsFreeAndAssignableTo(decorationObjectToTest)) > 0;
            //Debug.Log("Has slots? " + result);
            return result;
        }

        public Action<ShopContext> OnContextChange;
        public Action OnConfirmationRequested;
        public Action OnPurchaseComplete;
        public Action OnPurchaseCancelled;

        #region Context

        public ShopContext ShopContext { get {  return shopContext; } }

        private void SetContextPlacement()
        {
            shopContext = ShopContext.Placement;
            if (OnContextChange != null) OnContextChange(shopContext);
        }

        private void SetContextShopping()
        {
            currentUnlockableObjectPrefab = null;
            foreach (var shopDecorationSlot in allShopDecorationSlots)
            {
                shopDecorationSlot.Highlight(false);
            }

            shopContext = ShopContext.Shopping;
            if (OnContextChange != null) OnContextChange(shopContext);
        }

        #endregion


        #region Initialisation

        public void Initialise(ShopState shopState)
        {
            this.shopState = shopState;

            allShopDecorations = new List<ShopDecorationObject>(GetComponentsInChildren<ShopDecorationObject>());
            foreach (var shopDecoration in allShopDecorations) {
                shopDecoration.gameObject.SetActive(false);
            }
            //Debug.Log("Decorations: " + allShopDecorations.Count);

            allShopDecorationSlots = new List<ShopDecorationSlot>(GetComponentsInChildren<ShopDecorationSlot>());
            /*foreach (var slot in allShopDecorationSlots)
            {
                slot.OnSelect += SelectDecorationSlot;
            }*/
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


        #endregion

        private ShopDecorationObject SpawnNewDecoration(ShopDecorationObject UnlockableDecorationPrefab)
        {
            if (!HasSlotsForDecoration(UnlockableDecorationPrefab)) return null;

            var availableSlots = allShopDecorationSlots.Where(x => x.IsFreeAndAssignableTo(UnlockableDecorationPrefab));
            foreach (var shopDecorationSlot in availableSlots)
            {
                if (shopDecorationSlot.IsFreeAndAssignableTo(UnlockableDecorationPrefab))
                {
                    shopDecorationSlot.Highlight(true);
                }
            }

            currentUnlockableObjectPrefab = UnlockableDecorationPrefab;
            SetContextPlacement();

            // FOR DRAGGING
            var newDecoration = Instantiate(currentUnlockableObjectPrefab);
            newDecoration.Unlock();
            newDecoration.transform.localPosition = new Vector3(10000, 0, 0);
            return newDecoration;
            // this.currentDraggedDecoration = newDecoration;
        }
        

        // DEPRECATED
        /*private void SelectDecorationSlot(ShopDecorationSlot selectedSlot)
        {
            var newDecoration = Instantiate(currentUnlockableObjectPrefab);
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
        }*/

        #region Drag Placement

        private Coroutine dragCoroutine;
        private ShopDecorationObject currentDraggedDecoration;
        private ShopDecorationSlot currentDraggedSlot;

        public void CreateAndStartDragPlacement(ShopDecorationObject prefab)
        {
            var newDeco = SpawnNewDecoration(prefab);
            Debug.Log("NEW DECO " + newDeco);
            StartDragPlacement(newDeco);
        }

        private void DeleteDecoration(ShopDecorationObject decoToDelete)
        {
            var assignedSlot = allShopDecorationSlots.FirstOrDefault(x => x.HasCurrentlyAssigned(currentDraggedDecoration));
            if (assignedSlot != null) assignedSlot.Free();
            Destroy(decoToDelete.gameObject);
        }

        public void StartDragPlacement(ShopDecorationObject decoToDrag)
        {
            Debug.Log("START DRAG: " + decoToDrag);
            this.currentDraggedDecoration = decoToDrag;
            dragCoroutine = StartCoroutine(DragPlacementCO());
        }

        private void StopDragPlacement()
        {
            StopCoroutine(dragCoroutine);
            //SetContextShopping();

            if (OnConfirmationRequested != null) OnConfirmationRequested();

            // Saved state TODO: UPDATE SAVE TO REFLECT THE NEW CHANGES
            //shopState.unlockedDecorationsIDs.Add(newDecoration.id);
            //AppManager.I.Player.Save();
        }

        private void EndDragContext()
        { 
            currentDraggedDecoration = null;
            currentDraggedSlot = null;
        }


        private IEnumerator DragPlacementCO()
        {
            while (true)
            {
                // Get the closest assignable slot
                var allAssignableSlots = allShopDecorationSlots.Where(x =>
                    x.IsFreeAndAssignableTo(currentDraggedDecoration) || x.HasCurrentlyAssigned(currentDraggedDecoration));
                ShopDecorationSlot closestSlot = null;
                float minDistance = Int32.MaxValue;
                foreach (var slot in allAssignableSlots)
                {
                    float distance = (Camera.main.WorldToScreenPoint(slot.transform.position) - Input.mousePosition).sqrMagnitude;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestSlot = slot;
                    }
                }

                // Check whether we are close to the delete button, instead
                bool shouldBeDeleted = false;
                float distanceForDelete = (Camera.main.WorldToScreenPoint(deletePropButtonTransform.position) - Input.mousePosition).sqrMagnitude;
                float thresholdForDelete = 200;
                if (distanceForDelete < thresholdForDelete * thresholdForDelete)
                {
                    closestSlot = null;
                    deletePropButtonTransform.GetComponent<Image>().color = Color.cyan;
                    shouldBeDeleted = true;
                }
                else
                {
                    deletePropButtonTransform.GetComponent<Image>().color = Color.red;
                }

                // Place the object there
                if (closestSlot != currentDraggedSlot && closestSlot != null)
                {
                    if (currentDraggedSlot != null) currentDraggedSlot.Free();
                    currentDraggedSlot = closestSlot;
                    currentDraggedSlot.Assign(currentDraggedDecoration);
                }

                // Update highlights
                foreach (var slot in allShopDecorationSlots.Where(x =>x.IsAssignableTo(currentDraggedDecoration)))
                {
                    if (slot.HasCurrentlyAssigned(currentDraggedDecoration))
                    {
                        slot.Highlight(true, ShopDecorationSlot.SlotHighlight.Correct);

                    }
                    else if (slot.Assigned)
                    {
                        slot.Highlight(true, ShopDecorationSlot.SlotHighlight.Wrong);
                    }
                    else
                    {
                        slot.Highlight(true, ShopDecorationSlot.SlotHighlight.Idle);
                    }
                }

                // Check if we are stopping the dragging
                if (!Input.GetMouseButton(0))
                {
                    if (shouldBeDeleted)
                    {
                        // We delete on release
                        DeleteDecoration(currentDraggedDecoration);
                    }

                    StopDragPlacement();
                }
                yield return null;
            }
        }

        #endregion

        public void ConfirmPurchase()
        {
            EndDragContext();
            SetContextShopping();

            if (OnPurchaseComplete != null) OnPurchaseComplete();
        }

        public void CancelPurchase()
        {
            DeleteDecoration(currentDraggedDecoration);

            EndDragContext();
            SetContextShopping();

            if (OnPurchaseCancelled != null) OnPurchaseCancelled();
        }
    }
}
