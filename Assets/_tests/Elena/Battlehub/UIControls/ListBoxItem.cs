using UnityEngine.UI;

namespace Battlehub.UIControls
{
    public class ListBoxItem : ItemContainer
    {
        private Toggle m_toggle;
        
        public override bool IsSelected
        {
            get { return base.IsSelected; }
            set
            {
                if (base.IsSelected != value)
                {
                    m_toggle.isOn = value;
                    base.IsSelected = value;
                }
            }
        }

        private void Awake()
        {
            m_toggle = GetComponent<Toggle>();
            m_toggle.interactable = false;
            m_toggle.isOn = IsSelected;
        }
    }

}
