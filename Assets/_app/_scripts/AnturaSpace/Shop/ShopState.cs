using System.Collections.Generic;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopSlotState
    {
        public ShopDecorationSlotType slotType;
        public int slotIndex;
        public string decorationID;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public bool MatchesSlot(ShopDecorationSlot slot)
        {
            return slotType == slot.slotType && slotIndex == slot.slotIndex;
        }

        public override string ToString()
        {
            return slotType + "-" + slotIndex + " with decoration " + decorationID;
        }
    }

    public class ShopState
    {
        public List<ShopSlotState> occupiedSlots = new List<ShopSlotState>();

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

        public override string ToString()
        {
            string s = "";
            foreach (var slotState in occupiedSlots)
            {
                s += "- slot " + slotState + "\n";
            }
            return s;
        }

    }
}