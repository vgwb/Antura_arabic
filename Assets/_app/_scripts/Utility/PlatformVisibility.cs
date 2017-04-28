using UnityEngine;

public class PlatformVisibility : MonoBehaviour
{
    public bool AndroidOnly;

    void Start()
    {
        if (!Application.isEditor) {
            if (AndroidOnly && Application.platform != RuntimePlatform.Android) {
                gameObject.SetActive(false);
            }
        }
    }
}
