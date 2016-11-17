using UnityEngine;
using System.Collections;
using ModularFramework.Helpers;
using System.Collections.Generic;

namespace EA4S.ColorTickle
{
    public class TutorialUIManager : MonoBehaviour
    {

        public Transform m_StartFingerPosition;
        public Transform m_EndFingerPosition;
        public float m_MaxDelay = 2.0f; // Time waiting before Start the Tutorial Animation

        [HideInInspector]
        public bool StartTutorial = false;

        float m_Delay;

        bool m_ScreenTouched = false; 

        // Use this for initialization
        void Start()
        {
            m_Delay = m_MaxDelay;
        }

        // Update is called once per frame
        void Update()
        {
            if (StartTutorial)
            {
                m_Delay += Time.deltaTime;

                if (m_Delay >= m_MaxDelay)
                {
                    TutorialUI.ClickRepeat(m_StartFingerPosition.position, 1, 1);
                    m_Delay = 0.0f;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 GoalPosition = Camera.main.WorldToScreenPoint(m_StartFingerPosition.position);
                    float dist = Vector3.Distance(GoalPosition, Input.mousePosition);
                    Debug.Log(dist);

                    if (dist <= 20.0f)
                    {
                        StartTutorial = false;
                        Vector3[] path = new Vector3[2];
                        path[0] = m_StartFingerPosition.position;
                        path[1] = m_EndFingerPosition.position;
                        TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.Finger);
                    }                   
                }           
            }

        }

    }

}