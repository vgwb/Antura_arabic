using UnityEngine;

namespace EA4S.ReservedArea
{
    public class ReservedAreaDialog : MonoBehaviour
    {
        int parentLockCounter;

        void OnEnable()
        {
            parentLockCounter = 0;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnBtnGreen()
        {
            parentLockCounter++;
        }

        public void OnBtnRed()
        {
            if (parentLockCounter == 7) {
                UnlockParentControls();
            } else {
                parentLockCounter = 8; // disabling
            }
        }

        void UnlockParentControls()
        {
            AppManager.I.NavigationManager.GoToReservedArea();
        }
    }
}