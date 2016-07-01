using UnityEngine;
using System.Collections;
namespace CGL.Antura {
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour {

        bool OnDrag = false;

        Transform tmpTarget;

        void OnMouseDown() {
            OnDrag = true;
            gameObject.GetComponent<Animator>().Play("hold");
        }

        void OnMouseUp() {
            OnDrag = false;
            gameObject.GetComponent<Animator>().Play("run");
        }

        void Update() {
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                    transform.position = hit.point;
                }
            }
        }
        
    }
}
