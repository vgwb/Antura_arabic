using UnityEngine;
using System.Collections;
using System;

namespace EA4S.ColorTickle
{
    public class ColorTickle_LLController : MonoBehaviour
    {
        #region EXPOSED MEMBERS
        [SerializeField]
        private float m_fSpeed = 10; //Movement speed
        [SerializeField]
        private Vector3 m_v3StartPosition;
        [SerializeField]
        private Vector3 m_v3Destination;
        [SerializeField]
        private bool m_bMovingToDestination = false; //When true the letter will move towards the setted destination

        [SerializeField]
        private LLAnimationStates m_eAnimationOnMoving = LLAnimationStates.LL_still; //Animation to execute while moving
        [SerializeField]
        private LLAnimationStates m_eAnimationOnDestReached = LLAnimationStates.LL_still; //Animation to execute on reaching destination
        #endregion

        #region PRIVATE MEMBERS
        private LetterObjectView m_oLetter;
        #endregion

        #region EVENTS
        public Action OnDestinationReached;
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

        public float speed
        {
            get { return m_fSpeed; }
            set { m_fSpeed = value; }
        }

        public bool movingToDestination
        {
            get { return m_bMovingToDestination; }
            set { m_bMovingToDestination = value; }
        }

        public LLAnimationStates animationOnMoving
        {
            get { return m_eAnimationOnMoving; }
            set { m_eAnimationOnMoving = value; }
        }

        public LLAnimationStates animationOnDestReached
        {
            get { return m_eAnimationOnDestReached; }
            set { m_eAnimationOnDestReached = value; }
        }
    
        #endregion

        #region INTERNALS
        void Start()
        {
            m_oLetter = gameObject.GetComponent<LetterObjectView>();
            m_oLetter.gameObject.transform.position = m_v3StartPosition;
        }

        void Update()
        {
            if (m_bMovingToDestination)
            {
                MoveTo(m_v3Destination);
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
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary>
        /// Move the object from the current position to the final destination withe the setted speed.
        /// </summary>
        /// <param name="v3Destination">The final world position</param>
        private void MoveTo(Vector3 v3Destination)
        {
            Vector3 _v3MaxMovement = v3Destination - gameObject.transform.position;
            Vector3 _v3PartialMovement = _v3MaxMovement.normalized * m_fSpeed * Time.deltaTime;

            if (_v3PartialMovement.sqrMagnitude >= _v3MaxMovement.sqrMagnitude) //if we reached the destination
            {
                //position on the destination
                gameObject.transform.position = v3Destination;
                m_bMovingToDestination = false;

                //change animation and play sound
                m_oLetter.SetWalkingSpeed(0);
                m_oLetter.SetState(m_eAnimationOnDestReached);
                AudioManager.I.PlayLetter(m_oLetter.Data.Key);

                if(OnDestinationReached!=null) //launch event
                {
                    OnDestinationReached();
                }
            }
            else //make the progress for this frame
            {
                m_oLetter.SetWalkingSpeed(1);
                
                m_oLetter.SetState(m_eAnimationOnMoving);
                gameObject.transform.position += _v3PartialMovement;
                //m_bMovingToDestination = true;
            }
        }
        #endregion


    }
}
