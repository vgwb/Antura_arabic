﻿using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class LivesContainer : MonoBehaviour
    {
        public GameObject Heart1;
        public GameObject Heart2;
        public GameObject Heart3;

        public void SetLives(int howmany) {
            switch (howmany) {
                case 3:
                    Heart1.SetActive(true);
                    Heart2.SetActive(true);
                    Heart3.SetActive(true);
                    break;
                case 2:
                    Heart1.SetActive(true);
                    Heart2.SetActive(true);
                    Heart3.SetActive(false);
                    break;
                case 1:
                    Heart1.SetActive(true);
                    Heart2.SetActive(false);
                    Heart3.SetActive(false);
                    break;
                case 0:
                    Heart1.SetActive(false);
                    Heart2.SetActive(false);
                    Heart3.SetActive(false);
                    break;
            }
        }
    }
}