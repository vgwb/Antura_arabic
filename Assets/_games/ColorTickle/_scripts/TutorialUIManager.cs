using System.Linq;
using Antura.Tutorial;
using UnityEngine;

namespace Antura.Minigames.ColorTickle
{
    public class TutorialUIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] m_FingerPositions;
		[SerializeField]
		private float m_MaxDelay = 2.0f; // Time waiting before Start the Tutorial Animation

        [HideInInspector]
        public bool StartTutorial = false;

        float m_Delay; 

        // Use this for initialization
        void Start()
        {
            m_FingerPositions = GetComponentsInChildren<Transform>().Where(tr => tr != this.transform).ToArray();
            m_Delay = m_MaxDelay;
        }

        // Update is called once per frame
        void Update()
        {
			if (StartTutorial) {
				m_Delay += Time.deltaTime;
				if (m_Delay >= m_MaxDelay) 
				{
					m_Delay = 0;
					Vector3[] path = new Vector3[m_FingerPositions.Length];
				    for (int i = 0; i < m_FingerPositions.Length; i++)
				    {
				        path[i] = m_FingerPositions[i].position;
				    }
                    TutorialUI.DrawLine (path, TutorialUI.DrawLineMode.Finger);
				}
            }

        }

    }

}    