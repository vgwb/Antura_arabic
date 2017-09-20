using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationObject : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;
        public string id;

        public void OnMouseDown()
        {
            ShopDecorationsManager.I.StartDragPlacement(this, false);
        }
    }
}