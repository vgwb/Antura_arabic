using Antura.Core;
using UnityEngine;

namespace Antura.Utilities
{
    public class PlatformVisibility : MonoBehaviour
    {
        public bool MobileOnly;
        public bool AndroidOnly;
        public bool DesktopOnly;

        void Start()
        {
            if (!Application.isEditor) {
                if (MobileOnly && !AppConstants.IsMobilePlatform()) {
                    gameObject.SetActive(false);
                }

                if (AndroidOnly && Application.platform != RuntimePlatform.Android) {
                    gameObject.SetActive(false);
                }

                if (DesktopOnly && !AppConstants.IsDesktopPlatform()) {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}