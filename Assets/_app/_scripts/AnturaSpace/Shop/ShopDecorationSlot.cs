using System;
using DG.DeExtensions;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationSlot : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;
        private bool assigned = false;
        private ShopDecorationObject _assignedDecorationObject;

        private bool highlighted = false;
        public GameObject highlightMeshGO;

        public event Action<ShopDecorationSlot> OnSelect;

        public bool Assigned
        {
            get { return assigned; }
        }

        public ShopDecorationObject AssignedDecorationObject
        {
            get { return _assignedDecorationObject; }
        }

        #region Game Logic

        void Awake()
        {
            Highlight(false);
        }

        #endregion

        #region Assignment

        public void Assign(ShopDecorationObject assignedDecorationObject)
        {
            if (assigned) return;
            assigned = true;
            _assignedDecorationObject = assignedDecorationObject;
            _assignedDecorationObject.transform.SetParent(transform);
            _assignedDecorationObject.transform.localEulerAngles = Vector3.zero;
            _assignedDecorationObject.transform.localPosition = Vector3.zero;
            _assignedDecorationObject.transform.SetLocalScale(1);
        }

        public bool IsAssignableTo(ShopDecorationObject decorationObject)
        {
            return !assigned && slotType == decorationObject.slotType;
        }

        #endregion


        #region Highlight

        public void Highlight(bool choice)
        {
            highlighted = choice;
            highlightMeshGO.SetActive(choice);
        }

        #endregion

        public void OnMouseUpAsButton()
        {
            if (!highlighted) return;
            if (OnSelect != null) OnSelect.Invoke(this);
        }

    }
}