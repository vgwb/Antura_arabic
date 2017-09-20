using System;
using DG.DeExtensions;
using DG.DeInspektor;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationSlotGroup : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;

        // @note: these are set and serialized by EditorSetup's calls in ShopDecorationManager    
        [HideInInspector]
        public ShopDecorationSlot[] slots;

        public void EditorSetup()
        {
            slots = GetComponentsInChildren<ShopDecorationSlot>();
            int sequentialIndex = 0;
            foreach (var slot in slots)
            {
                slot.slotType = slotType;
                slot.sequentialIndex = sequentialIndex++;
            }
        }

    }
}