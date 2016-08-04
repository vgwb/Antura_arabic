using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class Credits : MonoBehaviour
    {

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}