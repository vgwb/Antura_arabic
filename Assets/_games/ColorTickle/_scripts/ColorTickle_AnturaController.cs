using UnityEngine;
using System.Collections;
using System;

namespace EA4S.ColorTickle
{
    public enum eAnturaState
    {
        ANTURA_STANDBY = 0, ANTURA_MOVING_TO_LL, ANTURA_SCARE_LL, ANTURA_MOVING_TO_STANDBY
    }

    public class ColorTickle_AnturaController : MonoBehaviour {

        #region EXPOSED MEMBERS
        [Header("Movement")]
        [SerializeField]
        private float m_fMovementSpeed = 10; //Movement speed
        [SerializeField]
        private float m_fRotationSpeed = 180; //Rotation speed by degree
        [SerializeField]
        private Vector3 m_v3StartPosition;
        [SerializeField]
        private Vector3 m_v3Destination;
        [SerializeField]
        private bool m_bMovingToDestination = false; //When true Antura will move towards the setted destination

        [Header("Behaviour")]
        [SerializeField]
        private float m_fBarkTime = 0.5f; //Time spent by barking at the LL
        [Range(0f, 100f)]
        [SerializeField]
        private float m_fProbabilityToScareLL = 100f; //Percentage to scare LL each second

        [SerializeField]
        private AnturaAnim m_eAnimationOnStandby = AnturaAnim.Nothing; //Animation to execute on reaching destination
        [SerializeField]
        private AnturaAnim m_eAnimationOnMoving = AnturaAnim.Nothing; //Animation to execute while moving
        [SerializeField]
        private AnturaAnim m_eAnimationOnLLReached = AnturaAnim.Nothing; //Animation to execute on reaching destination
        #endregion

        #region PRIVATE MEMBERS
        private Antura m_oAntura;
        private eAnturaState m_eAnturaState=eAnturaState.ANTURA_STANDBY;
        private float m_fBarkTimeProgress = 0;
        #endregion

        #region EVENTS
        public Action<eAnturaState> OnStateChanged;
        #endregion

        #region GETTER/SETTER
        public Vector3 startPosition
        {
            get { return m_v3StartPosition; }
        }

        public Vector3 destination
        {
            get { return m_v3Destination; }
        }

        public float movementSpeed
        {
            get { return m_fMovementSpeed; }
            set { m_fMovementSpeed = value; }
        }

        public float rotationSpeed
        {
            get { return m_fRotationSpeed; }
            set { m_fRotationSpeed = value; }
        }

        public bool movingToDestination
        {
            get { return m_bMovingToDestination; }
            set { m_bMovingToDestination = value; }
        }

        public float barkTime
        {
            get { return m_fBarkTime; }
            set { m_fBarkTime = value; }
        }

        public float probabilityToScareLL
        {
            get { return m_fProbabilityToScareLL; }
            set { m_fProbabilityToScareLL = value; }
        }
      
        public AnturaAnim animationOnMoving
        {
            get { return m_eAnimationOnMoving; }
            set { m_eAnimationOnMoving = value; }
        }

        public AnturaAnim animationOnDestReached
        {
            get { return m_eAnimationOnLLReached; }
            set { m_eAnimationOnLLReached = value; }
        }

        public AnturaAnim animationOnStandby
        {
            get { return m_eAnimationOnStandby; }
            set { m_eAnimationOnStandby = value; }
        }

        public eAnturaState anturaState
        {
            get { return m_eAnturaState; }
        }
        #endregion

        #region INTERNALS
        void Start()
        {
            m_oAntura = gameObject.GetComponent<Antura>();
            m_oAntura.gameObject.transform.position = m_v3StartPosition;
            m_eAnturaState = eAnturaState.ANTURA_STANDBY;
            m_fBarkTimeProgress = 0;
        }

        void Update()
        {
            if (m_bMovingToDestination)
            {
                MoveTo(m_v3Destination);
            }

            if(m_eAnturaState==eAnturaState.ANTURA_SCARE_LL)
            {
                m_fBarkTimeProgress += Time.deltaTime;
                if(m_fBarkTimeProgress>=m_fBarkTime)
                {
                    AnturaNextTransition();
                }
            }

        }
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Set a new destination and start moving towards it from the current position. 
        /// </summary>
        /// <param name="v3Destination">The final world position</param>
        public void MoveToNewDestination(Vector3 v3Destination)
        {
            m_v3Destination = v3Destination;
            m_bMovingToDestination = true;
        }

        /// <summary>
        /// Set a new destination and start position and begin to travel it. 
        /// </summary>
        /// <param name="v3Start">The start world position</param>
        /// <param name="v3Destination">The final world position</param>
        public void MoveOnNewPath(Vector3 v3Start, Vector3 v3Destination)
        {
            m_v3StartPosition = v3Start;
            m_v3Destination = v3Destination;
            MoveToNewDestination(m_v3Destination);
        }

        /// <summary>
        /// Using the given probability to launch antura action in the scene. 
        /// </summary>
        /// <returns>True when the action has succeed, false otherwise</returns>
        public bool TryLaunchAnturaDisruption()
        {
            if (m_eAnturaState == eAnturaState.ANTURA_STANDBY && UnityEngine.Random.Range(0f, 100f) < Mathf.Clamp(m_fProbabilityToScareLL,0,100) * Time.deltaTime) //check for success
            {
                AnturaNextTransition();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Force Antura to interrupt current action and go Standby on place.
        /// Use this for example when the game is fnished and you need to stop Antura
        /// </summary>
        public void ForceAnturaStandby()
        {
            //change animation
            m_oAntura.SetAnimation(m_eAnimationOnStandby);

            //set new state
            m_eAnturaState = eAnturaState.ANTURA_STANDBY;
            m_oAntura.IsBarking = false;

            //launch event
            if (OnStateChanged != null)
            {
                OnStateChanged(m_eAnturaState);
            }
                
}
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary>
        /// Move the object from the current position to the final destination withe the setted speed.
        /// </summary>
        /// <param name="v3Destination">The final world position</param>
        private void MoveTo(Vector3 v3Destination)
        {
            Vector3 _v3MaxMovement = v3Destination - gameObject.transform.position;
            Vector3 _v3PartialMovement = _v3MaxMovement.normalized * m_fMovementSpeed * Time.deltaTime;



            if (_v3PartialMovement.sqrMagnitude >= _v3MaxMovement.sqrMagnitude) //if we reached the destination
            {
                //position on the destination
                //gameObject.transform.position = v3Destination;
                gameObject.transform.Translate(_v3MaxMovement,Space.World);
                //gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(_v3MaxMovement), m_fSpeed * Time.deltaTime );

                m_bMovingToDestination = false;               

                AnturaNextTransition();
                
            }
            else //make the progress for this frame
            {
                
                m_oAntura.SetAnimation(m_eAnimationOnMoving);
                //gameObject.transform.position += _v3PartialMovement;
                gameObject.transform.Translate(_v3PartialMovement, Space.World);
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(_v3MaxMovement), m_fRotationSpeed * Time.deltaTime);

                //m_bMovingToDestination = true;
            }
        }

        /// <summary>
        /// Progress through the states for Antura: standby->movetoletter->bark->movetostandby->...
        /// </summary>
        private void AnturaNextTransition()
        {
            if (m_eAnturaState == eAnturaState.ANTURA_STANDBY) //go to the letter
            {
                m_bMovingToDestination = true;
                m_oAntura.SetAnimation(m_eAnimationOnMoving);

                m_eAnturaState = eAnturaState.ANTURA_MOVING_TO_LL;
            }
            else if (m_eAnturaState == eAnturaState.ANTURA_MOVING_TO_LL)//letter reached
            {
                //swap dest and start
                Vector3 _v3Temp = m_v3StartPosition;
                m_v3StartPosition = m_v3Destination;
                m_v3Destination = _v3Temp;

                //change animation and play sound
                m_oAntura.SetAnimation(m_eAnimationOnLLReached);
                m_oAntura.IsBarking = true;

                m_fBarkTimeProgress = 0;

                //set new state
                m_eAnturaState = eAnturaState.ANTURA_SCARE_LL;

            }
            else if (m_eAnturaState == eAnturaState.ANTURA_SCARE_LL) //return back
            {
                m_bMovingToDestination = true;
                m_oAntura.SetAnimation(m_eAnimationOnMoving);
                m_oAntura.IsBarking = false;

                m_eAnturaState = eAnturaState.ANTURA_MOVING_TO_STANDBY;
            }
            else if (m_eAnturaState == eAnturaState.ANTURA_MOVING_TO_STANDBY)//gone back to start
            {
                //swap dest and start
                Vector3 _v3Temp = m_v3StartPosition;
                m_v3StartPosition = m_v3Destination;
                m_v3Destination = _v3Temp;

                //change animation
                m_oAntura.SetAnimation(m_eAnimationOnStandby);

                //set new state
                m_eAnturaState = eAnturaState.ANTURA_STANDBY;

            }
            

            //launch event
            {
                if (OnStateChanged != null) 
                OnStateChanged(m_eAnturaState);
            }
        }
        #endregion

    }
}
