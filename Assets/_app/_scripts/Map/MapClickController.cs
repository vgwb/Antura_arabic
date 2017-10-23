using Antura.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antura.Map
{
    /// <summary>
    /// Controls click on the Map.
    /// </summary>
    public class MapClickController : MonoBehaviour
    {
        public PlayerPin playerPin;
        public StageMapsManager stageMapsManager;

        void LateUpdate()
        {
            // Touch movement controls
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    if (hit.collider.CompareTag("Pin"))
                    {
                        AudioManager.I.PlaySound(Sfx.UIButtonClick);

                        var pin = hit.collider.transform.gameObject.GetComponent<Pin>();
                        stageMapsManager.SelectPin(pin);
                    }
                }
            }
        }

    }
}