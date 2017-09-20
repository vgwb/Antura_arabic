using System;
using DG.DeExtensions;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationSlotGroup : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;

        void Awake()
        {
            foreach (var shopDecorationSlot in GetComponentsInChildren<ShopDecorationSlot>())
            {
                shopDecorationSlot.slotType = slotType;
            }
        }

    }
}