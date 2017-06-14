using EA4S.Core;
using EA4S.Helpers;
using EA4S.LivingLetters;
using UnityEngine;

namespace EA4S.Intro
{
    /// <summary>
    /// A special LivingLetter character with special animations.
    /// </summary>
    // refactor: remove the references to the Maze minigame
    public class IntroRocketCharacter : MonoBehaviour
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
            LL.Initialize((AppManager.Instance as AppManager).Teacher.GetAllTestLetterDataLL().GetRandom()); 
            LL.SetState(LLAnimationStates.LL_rocketing);
            LL.Horraying = true;
        }

        public void SetDestination ()
        {
            Destination = transform.position;
            Destination -= new Vector3(200, 0, 0);
            Path = transform.position - Destination;
            m_Move = true;
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