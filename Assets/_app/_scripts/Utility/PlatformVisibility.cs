using UnityEngine;

public class PlatformVisibility : MonoBehaviour
{
    public bool MobileOnly;
    public bool AndroidOnly;

    void Start()
    {
        if (!Application.isEditor) {
            if (MobileOnly && (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)) {
                gameObject.SetActive(false);
            }
            if (AndroidOnly && Application.platform != RuntimePlatform.Android) {
                gameObject.SetActive(false);
            }
        }
    }
}
