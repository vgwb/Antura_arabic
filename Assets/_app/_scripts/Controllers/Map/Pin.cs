using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class Pin : MonoBehaviour
    {
        public int Number;
        public Transform RopeNode;
        public GameObject Dot;

        void Start() {
	
        }


        void OnMouseDown() {
            Debug.Log("Clicked Pin " + Number);
        }
    }
}