using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

namespace Battlehub.UIControls
{
    public class ItemContainer : MonoBehaviour, IPointerDownHandler
    {
        private bool m_isSelected;
        public static event EventHandler Selected;
        public static event EventHandler Unselected;

        public virtual bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    if (m_isSelected)
                    {
                        if (Selected != null)
                        {
                            Selected(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (Unselected != null)
                        {
                            Unselected(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }


        public int Index
        {
            get;
            set;
        }

        public object Item
        {
            get;
            set;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            IsSelected = true;
        }

    }

}
