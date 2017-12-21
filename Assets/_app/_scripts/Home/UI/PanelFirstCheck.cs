using Antura.Scenes;
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
            (HomeScene.I as HomeScene).CloseFirstCheckPanel();
            gameObject.SetActive(false);
        }

    }
}