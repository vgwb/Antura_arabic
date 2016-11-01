using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EA4S
{
    public class MinigameUIProxy : MonoBehaviour
    {
        static MinigameUIProxy instance;
        public static MinigameUIProxy I
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("[UIProxy]");
                    instance = go.AddComponent<MinigameUIProxy>();
                }

                return instance;
            }
        }

        public MinigamesUIStarbar Starbar
        {
            get
            {
                return MinigamesUI.Starbar;
            }
        }

        public MinigamesUITimer Timer
        {
            get
            {
                return MinigamesUI.Timer;
            }
        }


        public MinigamesUILives Lives
        {
            get
            {
                return MinigamesUI.Lives;
            }
        }

        void Awake()
        {
            instance = this;
            MinigamesUI.Init(MinigamesUIElement.Lives | MinigamesUIElement.Starbar | MinigamesUIElement.Timer);
            MinigamesUI.Lives.gameObject.SetActive(false);
            MinigamesUI.Starbar.gameObject.SetActive(false);
            MinigamesUI.Timer.gameObject.SetActive(false);
        }
    }
}
