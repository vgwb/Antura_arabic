using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EA4S
{
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