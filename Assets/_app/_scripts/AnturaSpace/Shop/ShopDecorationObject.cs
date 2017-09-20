using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationObject : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;
        public string id;
        public bool locked = true;

        public void Unlock()
        {
            if (!locked) return;
            locked = false;
            gameObject.SetActive(true);
        }

        public void OnMouseDown()
        {
            ShopDecorationsManager.I.StartDragPlacement(this);
        }
    }
}