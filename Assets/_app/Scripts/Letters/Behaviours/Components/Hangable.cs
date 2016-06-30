using UnityEngine;
using System.Collections;
namespace CGL.Antura {
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour {

        bool OnDrag = false;

        void OnMouseDown() {
            OnDrag = true;
        }

        void OnMouseUp() {
            OnDrag = false;
        }

        void Update() {
            if (OnDrag) {
                //if (!Terrain)
                //    return;

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000)) {
                    transform.position = hit.point;
                }

                //RaycastHit hit;
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                //if (Terrain.Raycast(ray, out hit, Mathf.Infinity)) {
                //    transform.position = hit.point;
                //}
            }
        }

        


        /// <summary>
        /// On drag state.
        /// </summary>
        //void OnMouseDrag() {

        //        Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, dist.z);
        //        Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);
        //        transform.position = worldPos;


        //        //Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //        //Vector3 objPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));
        //        ////transform.localPosition = new Vector3(objPosition.x, objPosition.y, transform.localPosition.z);
        //        //transform.localPosition = new Vector3(objPosition.x, transform.localPosition.y, objPosition.z);
        //    }
    }
}
