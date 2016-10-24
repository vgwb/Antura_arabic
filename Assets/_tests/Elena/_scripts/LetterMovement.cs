using UnityEngine;
using System.Collections;

namespace EA4S.TestE
{
    public class LetterMovement : MonoBehaviour {

        public MiniMap miniMapScript;
        public float speed;
        Transform target;
        public Vector3 posDot;
        public int pos;
        void Start () {
            ResetPosLetter(1, miniMapScript.pinRight);
        }

        void Update () {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTheRightDot(miniMapScript.pinRight);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTheLeftDot(miniMapScript.pinLeft);
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posDot.x, transform.position.y, posDot.z), speed * Time.deltaTime);
        }
        void MoveToTheRightDot(Vector3 pinPos)
        {
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
            if (pos < (miniMapScript.posDots.Length-1))
            {
                pos++;
                posDot = miniMapScript.posDots[pos];
            }
        }
        void MoveToTheLeftDot(Vector3 pinPos)
        {
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
            if (pos > 0)
            {
                pos--;
                posDot = miniMapScript.posDots[pos];
            }
        }
        public void ResetPosLetter(int nPin, Vector3 pin)
        {
            pos = 0;
            posDot = miniMapScript.posDots[pos];
            transform.LookAt(pin);
        }
    }
}


