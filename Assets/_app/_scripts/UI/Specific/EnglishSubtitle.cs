using UnityEngine;

namespace EA4S.UI
{
    /// <summary>
    /// constrain visibility of GameObject to EnglishSubtitles setting
    /// </summary>
    public class EnglishSubtitle : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(AppManager.I.GameSettings.EnglishSubtitles);
        }

    }
}