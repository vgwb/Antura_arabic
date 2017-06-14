using EA4S.Core;
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
            gameObject.SetActive((AppManager.Instance as AppManager).AppSettings.EnglishSubtitles);
        }

    }
}