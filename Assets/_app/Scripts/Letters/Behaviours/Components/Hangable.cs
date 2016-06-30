using UnityEngine;
using System.Collections;
namespace CGL.Antura {
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour {

        /// <summary>
        /// On drag state.
        /// </summary>
        void OnMouseDrag() {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
            //transform.localPosition = new Vector3(objPosition.x, objPosition.y, transform.localPosition.z);
            transform.localPosition = new Vector3(objPosition.x, transform.localPosition.y, objPosition.z);
        }
    }
}
