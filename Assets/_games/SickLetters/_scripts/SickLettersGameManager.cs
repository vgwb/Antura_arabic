using UnityEngine;
using System.Collections;

namespace EA4S.SickLetters
{
    public class SickLettersGameManager : MonoBehaviour
    {

        public SickLettersGame game;
        

        

        // Use this for initialization
        void Start()
        {
            game = GetComponent<SickLettersGame>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        
    }
}
