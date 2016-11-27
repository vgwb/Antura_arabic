using UnityEngine;
using UnityEngine.EventSystems;

namespace EA4S
{
    public class OpenPlayerBookScene : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            NavigationManager.I.OpenPlayerBook();
        }

    }
}