using UnityEngine;
using System.Collections;

namespace EA4S.TestE
{
    public class AnturaMovement : MonoBehaviour {

        public MiniMap miniMapScript;
        public float speed;
        Transform target;
        public Vector3 v;
        public int pos;
        void Start () {
            pos = 0;
            v = miniMapScript.posSteps[pos];
        }
	
	    void Update () {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.LookAt(new Vector3(miniMapScript.pos2.position.x,
                    miniMapScript.pos2.position.y+3,miniMapScript.pos2.position.z));
                if(pos<4)
                {
                    pos++;
                    v = miniMapScript.posSteps[pos];
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.LookAt(new Vector3(miniMapScript.pos1.position.x,
                   miniMapScript.pos1.position.y + 3, miniMapScript.pos1.position.z)); ;
                if (pos > 0)
                {
                    pos--;
                    v = miniMapScript.posSteps[pos];
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(v.x, transform.position.y, v.z), speed * Time.deltaTime);

        }
    }
}


