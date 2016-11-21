using UnityEngine;
using System.Collections;

namespace EA4S.MixedLetters
{
    public class VictimLLController : MonoBehaviour
    {
        public static VictimLLController instance;
        public LetterObjectView letterObjectView;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void DoHooray()
        {
            letterObjectView.DoHorray();
        }
    }
}