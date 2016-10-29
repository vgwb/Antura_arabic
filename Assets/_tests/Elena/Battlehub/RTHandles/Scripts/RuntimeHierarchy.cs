using UnityEngine;
using Battlehub.UIControls;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Battlehub.RTHandles
{
    public class RuntimeHierarchy : MonoBehaviour
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public ItemsControl ItemsControl;
        public Type TypeCriteria = typeof(GameObject);


        private UnityEngine.Object m_selectedItem;
        public UnityEngine.Object SelectedItem
        {
            get
            {
                return m_selectedItem;
            }
            set
            {
                m_selectedItem = value;
                ItemsControl.SelectedItem = value;
            }
        }

        public static bool IsPrefab(Transform This)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                throw new InvalidOperationException("Does not work in edit mode");
            }
            return This.gameObject.scene.buildIndex < 0;
        }

        private void Start()
        {
            ItemsControl.ItemDataBinding += OnDataBinding;
            ItemsControl.SelectionChanged += OnSelectionChanged;

            List<UnityEngine.Object> filtered = new List<UnityEngine.Object>();
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            for(int i = 0; i < objects.Length; ++i)
            {
                GameObject obj = objects[i] as GameObject;
                if(obj == null)
                {
                    continue;
                }

                if (!IsPrefab(obj.transform))
                {
                    if (TypeCriteria == typeof(GameObject))
                    {
                        filtered.Add(obj);
                    }
                    else
                    {
                        Component component = obj.GetComponent(TypeCriteria);
                        if(component)
                        {
                            filtered.Add(component);
                        }
                    }
                }
            }

            ItemsControl.Items = filtered;
            ItemsControl.SelectedItem = m_selectedItem;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private void OnDataBinding(object sender, ItemDataBindingEventArgs e)
        {
            UnityEngine.Object dataItem = e.Item as UnityEngine.Object;
            if (dataItem != null)
            {
                Text text = e.ItemPresenter.GetComponentInChildren<Text>(true);
                text.text = dataItem.name;
            }
        }

        private void OnDestroy()
        {
            if(ItemsControl != null)
            {
                ItemsControl.ItemDataBinding -= OnDataBinding;
                ItemsControl.SelectionChanged -= OnSelectionChanged;
            }
        }
    }
}

