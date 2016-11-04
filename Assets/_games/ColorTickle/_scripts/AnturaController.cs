using UnityEngine;
using System.Collections;

namespace EA4S.ColorTickle
{
    public class AnturaController : MonoBehaviour
    {
        #region PRIVATE MEMBERS
        Antura m_Antura;
        bool m_Enable = false;
        Vector3 m_StartPosition;
        Vector3 m_LetterPosition;
        Vector3 m_DistanceFromLetter;
        AnturaContollerState m_State = AnturaContollerState.SLEEPING;
        float m_RotationToReach;

        float m_AnturaTimer = 3;

        #endregion
        
        enum AnturaContollerState
        {
            SLEEPING, REACHINGLETTER, COMINGBACK, ROTATION_BACK, ROTATION

        }

        public bool isEnable
        {
            set { m_Enable = value; }  
        }


        public Vector3 letterPosition
        {
            set {
                m_LetterPosition = value;
                m_DistanceFromLetter = m_LetterPosition - m_StartPosition ;
            }
        }
        // Use this for initialization
        void Start()
        {
            m_Antura = gameObject.GetComponent<Antura>();
            m_StartPosition = gameObject.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Enable)
            {                
                if (m_State == AnturaContollerState.SLEEPING)
                {
                    m_AnturaTimer -= Time.deltaTime;
                    if (m_AnturaTimer < 0.0f)
                    {
                        m_State = AnturaContollerState.REACHINGLETTER;
                        m_Antura.SetAnimation(AnturaAnim.Run);
                    }
                }
                else
                {
                    if (m_State == AnturaContollerState.REACHINGLETTER)
                    {
                        if (gameObject.transform.position.z >= m_LetterPosition.z + 2.0f)
                        {
                            gameObject.transform.position += m_DistanceFromLetter * Time.deltaTime;
                        }
                        else
                        {
                            m_State = AnturaContollerState.ROTATION_BACK;
                            float YAngle;
                            Vector3 YAxis = Vector3.up;
                            transform.rotation.ToAngleAxis(out YAngle, out YAxis);
                            m_RotationToReach = YAngle + 180.0f;
                        }
                    }

                    if (m_State == AnturaContollerState.ROTATION_BACK)
                    {
                        float YAngle;
                        Vector3 YAxis = Vector3.up;
                        transform.rotation.ToAngleAxis(out YAngle, out YAxis);
                        if (YAngle <= m_RotationToReach)
                        {
                            gameObject.transform.Rotate(Vector3.up * 180.0f * Time.deltaTime * 2);
                        }
                        else
                        {
                            m_State = AnturaContollerState.COMINGBACK;
                        }
                    }

                    if (m_State == AnturaContollerState.COMINGBACK)
                    {
                        if (gameObject.transform.position.z <= m_StartPosition.z)
                        {
                            gameObject.transform.position -= m_DistanceFromLetter * Time.deltaTime ;
                        }
                        else
                        {
                            m_State = AnturaContollerState.ROTATION;
                            float YAngle;
                            Vector3 YAxis = Vector3.up;
                            transform.rotation.ToAngleAxis(out YAngle, out YAxis);
                            m_RotationToReach = YAngle - 180.0f;
                        }
                    }

                    if (m_State == AnturaContollerState.ROTATION)
                    {
                        float YAngle;
                        Vector3 YAxis = Vector3.up;
                        transform.rotation.ToAngleAxis(out YAngle, out YAxis);
                        if (YAngle >= m_RotationToReach)
                        {
                            gameObject.transform.Rotate(Vector3.up * (-180.0f) * Time.deltaTime * 2);
                        }
                        else
                        {
                            m_Antura.SetAnimation(AnturaAnim.SitBreath);
                            m_State = AnturaContollerState.SLEEPING;
                            m_AnturaTimer = 3.0f;
                        }
                    }

                }
        
            }

        }
    }
}


