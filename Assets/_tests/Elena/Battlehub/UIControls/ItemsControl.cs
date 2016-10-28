using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

using UnityEngine;

namespace Battlehub.UIControls
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public object[] OldItems
        {
            get;
            private set;
        }

        public int[] OldIndices
        {
            get;
            private set;
        }

        public object[] NewItems
        {
            get;
            private set;
        }

        public int[] NewIndices
        {
            get;
            private set;
        }

        public object OldItem
        {
            get
            {
                if(OldItems == null)
                {
                    return null;
                }
                return OldItems[0];
            }
        }

        public int OldIndex
        {
            get
            {
                if(OldIndices == null)
                {
                    return -1;
                }

                return OldIndices[0];
            }
        }

        public object NewItem
        {
            get
            {
                if(NewItems == null)
                {
                    return null;
                }
                return NewItems[0];
            }
        }

        public int NewIndex
        {
            get
            {
                if(NewIndices == null)
                {
                    return -1;
                }
                return NewIndices[0];
            }
        }

        public SelectionChangedEventArgs(object[] oldItems, int[] oldIndices, object[] newItems, int[] newIndices)
        {
            OldItems = oldItems;
            OldIndices = oldIndices;
            NewItems = newItems;
            NewIndices = newIndices;
        }

        public SelectionChangedEventArgs(object oldItem, int oldIndex, object newItem, int newIndex)
        {
            OldItems = new[] { oldItem };
            OldIndices = new[] { oldIndex };
            NewItems = new[] { newItem };
            NewIndices = new[] { newIndex };
        }
    }

    public class ItemAddEventArgs : EventArgs
    {
        public int Index
        {
            get;
            private set;
        }

        public ItemAddEventArgs(int index)
        {
            Index = index;
        }
    }

    public class ItemRemovedEventArgs : EventArgs
    {
        public object Item
        {
            get;
            private set;
        }

        public ItemRemovedEventArgs(object item)
        {
            Item = item;
        }
    }

    public class ItemDataBindingEventArgs : EventArgs
    {
        public object Item
        {
            get;
            private set;
        }

        public GameObject ItemPresenter
        {
            get;
            private set;
        }

        public ItemDataBindingEventArgs(object item, GameObject itemPresenter)
        {
            Item = item;
            ItemPresenter = itemPresenter;
        }
    }

    public class CancelEventArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }
    }

    public class ItemsControl : MonoBehaviour, IPointerDownHandler
    { 
        [SerializeField]
        private GameObject ItemContainerPrefab;
        public Transform Panel;
        public event EventHandler<ItemDataBindingEventArgs> ItemDataBinding;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private ItemContainer[] m_itemContainers;

        private IList m_items;
        public IList Items
        {
            get { return m_items; }
            set
            {
                m_items = value;
                DataBind();
            }
        }

        private ItemContainer m_selectedItem;
        public object SelectedItem
        {
            get
            {
                if (m_selectedItem == null)
                {
                    return null;
                }
                return m_selectedItem.Item;
            }
            set
            {
                SelectedIndex = IndexOf(value);
            }
        }

        public int SelectedIndex
        {
            get
            {
                if (m_selectedItem == null)
                {
                    return -1;
                }

                return m_selectedItem.Index;
            }
            set
            {
                if(m_selectedItem == null && value == -1)
                {
                    return;
                }

                if(m_selectedItem != null && m_selectedItem.Index == value)
                {
                    return;
                }

                ItemContainer oldItemContainer = m_selectedItem;
                object oldItem = null;
                int oldIndex = -1;
                if (oldItemContainer != null)
                {
                    oldItemContainer.IsSelected = false;
                    oldItem = oldItemContainer.Item;
                    oldIndex = oldItemContainer.Index;
                }

                m_selectedItem = m_itemContainers.Where(i => i.Index == value).FirstOrDefault();

                object newItem = null;
                int newIndex = -1;
                if (m_selectedItem != null)
                {
                    m_selectedItem.IsSelected = true;
                    newItem = m_selectedItem.Item;
                    newIndex = m_selectedItem.Index;
                }

                if(SelectionChanged != null)
                {
                    SelectionChanged(this, new SelectionChangedEventArgs(oldItem, oldIndex, newItem, newIndex));
                }
            }
        }

        private void Awake()
        {
            if (Panel == null)
            {
                Panel = transform;
            }
            m_itemContainers = GetComponentsInChildren<ItemContainer>();
        }


        private void OnEnable()
        {
            ItemContainer.Selected += OnItemSelected;
        }

        private void OnDisable()
        {
            ItemContainer.Selected -= OnItemSelected;
        }

        public int IndexOf(object obj)
        {
            if(m_items == null)
            {
                return -1;
            }

            if(obj == null)
            {
                return -1;
            }

            return m_items.IndexOf(obj);
        }

        private void OnItemSelected(object sender, System.EventArgs e)
        {
            if (!CanHandleEvent(sender))
            {
                return;
            }

            SelectedIndex = ((ItemContainer)sender).Index;

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SelectedIndex = -1;
        }

        protected bool CanHandleEvent(object sender)
        {
            ItemContainer itemContainer = sender as ItemContainer;
            if(!itemContainer)
            {
                return false;
            }
            return itemContainer.transform.IsChildOf(Panel);
        }

        protected virtual void DataBind()
        {
            m_itemContainers = GetComponentsInChildren<ItemContainer>();
            if (m_items == null)
            {
                for (int i = 0; i < m_itemContainers.Length; ++i)
                {
                    Destroy(m_itemContainers[i].gameObject);
                }
            }
            else
            {
                int deltaItems = m_items.Count - m_itemContainers.Length;
                if (deltaItems > 0)
                {
                    int index = m_itemContainers.Length;
                    Array.Resize(ref m_itemContainers, m_itemContainers.Length + deltaItems);
                    while (index < m_itemContainers.Length)
                    {
                        GameObject container = Instantiate(ItemContainerPrefab);
                        container.transform.SetParent(Panel, false);
                        ItemContainer itemContainer = container.GetComponent<ItemContainer>();
                        if (itemContainer == null)
                        {
                            itemContainer = container.AddComponent<ItemContainer>();
                        }

                        m_itemContainers[index] = itemContainer;
                        index++;
                    }
                }
                else
                {
                    int newLength = m_itemContainers.Length + deltaItems;
                    for (int i = m_itemContainers.Length - 1; i >= newLength; i--)
                    {
                        Destroy(m_itemContainers[i]);
                    }

                    Array.Resize(ref m_itemContainers, newLength);
                }

                int count = m_items.Count;
                for (int i = 0; i < count; ++i)
                {
                    object item = m_items[i];
                    ItemContainer itemContainer = m_itemContainers[i];
                    if (itemContainer != null)
                    {
                        itemContainer.Index = i;
                        itemContainer.Item = item;
                        if (ItemDataBinding != null)
                        {
                            ItemDataBindingEventArgs args = new ItemDataBindingEventArgs(item, itemContainer.gameObject);
                            ItemDataBinding(this, args);
                        }
                    }
                }
            }

         
        }

     
    }
}

