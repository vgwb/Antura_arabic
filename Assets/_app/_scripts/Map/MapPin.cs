using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class MapPin : MonoBehaviour
    {
        public int Number;
        public Transform RopeNode;
        public GameObject Dot;
        public int posBefore;

        void Start()
        {

        }

        void OnMouseDown()
        {
            Debug.Log("Clicked Pin " + Number);
        }
    }
}