using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Core
{
    public class PanelFirstCheck : MonoBehaviour
    {

        void Start()
        {

        }

        public void OnClose()
        {
            gameObject.SetActive(false);
        }

    }
}