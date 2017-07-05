using UnityEngine;

namespace EA4S.Utilities
{
    public class PlatformVisibility : MonoBehaviour
    {
        public bool MobileOnly;
        public bool AndroidOnly;
        public bool DesktopOnly;

        void Start()
        {
            if (!Application.isEditor)
            {
                if (MobileOnly &&
                    (Application.platform != RuntimePlatform.Android &&
                     Application.platform != RuntimePlatform.IPhonePlayer))
                {
                    gameObject.SetActive(false);
                }
                if (AndroidOnly && Application.platform != RuntimePlatform.Android)
                {
                    gameObject.SetActive(false);
                }

                if (DesktopOnly && Application.platform != RuntimePlatform.WindowsPlayer)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}