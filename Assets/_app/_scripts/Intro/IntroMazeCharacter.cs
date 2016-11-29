using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Helpers;

namespace EA4S
{
    public class IntroMazeCharacter : MonoBehaviour
    {

        public LetterObjectView LL;
        //public List<GameObject> particles;

        [HideInInspector]
        public float m_Velocity;
        
        bool m_Move = false;
        Vector3 Destination;
        Vector3 Path;

        void Start()
        {
            LL.Init(AppManager.I.Teacher.GetAllTestLetterDataLL().GetRandomElement()); 
            LL.SetState(LLAnimationStates.LL_rocketing);
        }

        public void SetDestination ()
        {
            Destination = transform.position;
            Destination -= new Vector3(200, 0, 0);
            Path = transform.position - Destination;
            m_Move = true;
        }

        public void SetDestination(Vector3 destination)
        {

        }

        //public void toggleVisibility(bool value)
        //{
        //    foreach (GameObject particle in particles) particle.SetActive(value);
        //}

        void Update()
        {
            if (m_Move)
            {
                if (transform.position.x > Destination.x)
                {
                    transform.position -= Path * Time.deltaTime * m_Velocity;
                }
                else
                {
                    m_Move = false;
                    gameObject.SetActive(false);
                }                             
            }

        }
    }
}