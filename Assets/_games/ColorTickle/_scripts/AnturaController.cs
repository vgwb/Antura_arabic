using UnityEngine;
using System.Collections;

namespace EA4S.ColorTickle
{
    public class AnturaController : MonoBehaviour
    {
        enum AnturaContollerState
        {
            SLEEPING, REACHINGLETTER, COMINGBACK, ROTATION_BACK, ROTATION, BARKING

        }

        #region PRIVATE MEMBERS
        Antura m_Antura;
        bool m_Enable = false;
        Vector3 m_StartPosition;
        Vector3 m_LetterPosition;
        Vector3 m_DistanceFromLetter;
        AnturaContollerState m_State = AnturaContollerState.SLEEPING;
        float m_RotationToReach;
        float m_BarkingTime = 1.0f;
        #endregion

        #region EVENTS
        public event System.Action LetterReached;
        public event System.Action AnturaGoingAway;
        #endregion

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
        void Awake()
        {
            m_Antura = gameObject.GetComponent<Antura>();
            m_StartPosition = gameObject.transform.position;
            m_Antura.SetAnimation(AnturaAnim.SitBreath);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Enable || m_State != AnturaContollerState.SLEEPING)
            {                
                if (m_State == AnturaContollerState.SLEEPING)
                {
                    m_State = AnturaContollerState.REACHINGLETTER;
                    m_Antura.SetAnimation(AnturaAnim.Run);
                }

                if (m_State == AnturaContollerState.REACHINGLETTER)
                {
                    if (gameObject.transform.position.z >= m_LetterPosition.z + 3.0f)
                    {
                        gameObject.transform.position += m_DistanceFromLetter * Time.deltaTime;
                    }
                    else
                    {
                        if (LetterReached != null)
                        {
                            LetterReached();
                        }
                        m_Antura.SetAnimation(AnturaAnim.StandExcitedBreath);
                        m_Antura.IsBarking = true;
                        m_State = AnturaContollerState.BARKING;
                    }
                }

                if (m_State == AnturaContollerState.BARKING)
                {                    
                    m_BarkingTime -= Time.deltaTime;
                    if (m_BarkingTime<= 0)
                    {
                        m_BarkingTime = 1.0f;
                        m_Antura.IsBarking = false;
                        m_Antura.SetAnimation(AnturaAnim.Run);
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
                        if (AnturaGoingAway != null)
                        {
                            AnturaGoingAway();
                        }
                        m_State = AnturaContollerState.COMINGBACK;
                    }
                }

                if (m_State == AnturaContollerState.COMINGBACK)
                {
                    if (gameObject.transform.position.z <= m_StartPosition.z)
                    {
                        gameObject.transform.position -= m_DistanceFromLetter * Time.deltaTime;
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
                    }
                }


            }

        }
    }
}


