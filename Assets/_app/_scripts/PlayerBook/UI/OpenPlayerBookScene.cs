using EA4S.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EA4S.PlayerBook
{
    /// <summary>
    /// Button that allows access to the PlayerBook.
    /// </summary>
    // refactor: should be grouped with Map scripts
    public class OpenPlayerBookScene : MonoBehaviour, IPointerClickHandler
    {
        public Image IcoAvatar;

        void Start()
        {
            IcoAvatar.sprite = AppManager.I.Player.GetAvatar();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NavigationManager.I.OpenPlayerBook();
        }

    }
}