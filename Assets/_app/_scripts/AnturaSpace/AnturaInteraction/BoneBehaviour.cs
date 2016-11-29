using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class BoneBehaviour : MonoBehaviour
    {

        #region EXPOSED MEMBERS
        [SerializeField]
        private Rigidbody m_oBoneRigidbody;

        [Header("Simple Throw")]

        [SerializeField]
        private Vector3 m_v3ThrowDirection;
        [SerializeField]
        private float m_fThrowMagnitude=10f;
        [SerializeField]
        private ForceMode m_eThrowForceMode;

        [Header("Rotation")]

        [SerializeField]
        private Vector3 m_v3RotationDirection;
        [SerializeField]
        private float m_fRotationMinMagnitude = 0f;
        [SerializeField]
        private float m_fRotationMaxMagnitude = 10f;
        [SerializeField]
        private ForceMode m_eRotationForceMode;

        [Header("Drag")]

        [SerializeField]
        private float m_fDragThrowMagnitudeScaling = 1f;
        [SerializeField]
        private float m_fTimeSampling = 0.0333f;
        [SerializeField]
        private ForceMode m_eReleaseForceMode;
        #endregion

        #region PRIVATE MEMBERS
        //private Rigidbody m_oBoneRigidboy;
        bool m_bIsDragged = false;
        private Vector3 m_v3LastPosition;
        private float m_fTimeProgression=0;
        #endregion

        #region GETTER/SETTER

        public float boneRotation_MaxMagnitude
        {
            get { return m_fRotationMaxMagnitude; }
            set
            {
                m_fRotationMaxMagnitude = value;
                CorrectMinMaxRotation();
            }
        }

        public float boneRotation_MinMagnitude
        {
            get { return m_fRotationMinMagnitude; }
            set
            {
                m_fRotationMinMagnitude = value;
                CorrectMinMaxRotation();
            }
        }

        public Rigidbody boneRigidbody
        { get;set; }

        
        #endregion

        #region INTERNALS
        void Start()
        {

            //CorrectMinMaxRotation();
            if (m_fRotationMaxMagnitude < m_fRotationMinMagnitude)
            {
                Debug.Log("Warning, unvalid min/max values");
            }

        }

        void Update()
        {

            if(m_bIsDragged) //if this bone is being dragged
            {
                m_fTimeProgression += Time.deltaTime;

                if(m_fTimeProgression>=m_fTimeSampling)
                {
                    m_v3LastPosition = transform.position;//Store dragging data to prepare for the releasing throw
                    m_fTimeProgression -= m_fTimeSampling;
                }
                

                //set the bone position on the pointer(x,y) at it's current distance from the camera
                float _fCameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _fCameraDistance));
                   
                //now keep it on an imaginary plane inclined to 45 degree by setting z equal to y
                transform.position=new Vector3(transform.position.x, transform.position.y, transform.position.y);

            }

        }
        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// The bone is throw with the setted forces
        /// </summary>
        public void SimpleThrow()
        {
            Debug.Log("SimpleThrow");

            //CorrectMinMaxRotation();

            m_oBoneRigidbody.isKinematic = true; //resets actives forces
            m_oBoneRigidbody.isKinematic = false;

            ApplyDefaultForces();
        }

        public void Drag()
        {
            Debug.Log("Dragging");

            m_oBoneRigidbody.isKinematic = true; //resets actives forces

            m_bIsDragged = true;

            m_fTimeProgression = 0;

            m_v3LastPosition = transform.position;
        }
        #endregion

        public void LetGo()
        {
            Debug.Log("Let Go");

            m_oBoneRigidbody.isKinematic = false;

            m_bIsDragged = false;

            m_fTimeProgression = 0;

            //apply stored forces
            ApplyDragForces();
        }
        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Apply the default forces on the Rigidbody
        /// </summary>
        private void ApplyDefaultForces()
        {
            //Add rotation with random magnitude
            m_oBoneRigidbody.AddTorque(m_v3RotationDirection.normalized * Random.Range(m_fRotationMinMagnitude, m_fRotationMaxMagnitude), m_eRotationForceMode);
            //Add translation
            m_oBoneRigidbody.AddForce(m_v3ThrowDirection.normalized * m_fThrowMagnitude, m_eThrowForceMode);
        }

        /// <summary>
        /// Apply the drag forces on the Rigidbody
        /// </summary>
        private void ApplyDragForces()
        {
            //Add rotation with random magnitude
            m_oBoneRigidbody.AddTorque(m_v3RotationDirection.normalized * Random.Range(m_fRotationMinMagnitude, m_fRotationMaxMagnitude), m_eRotationForceMode);
            //Add translation
            m_oBoneRigidbody.AddForce((transform.position-m_v3LastPosition) * m_fDragThrowMagnitudeScaling, m_eReleaseForceMode);
        }

        /// <summary>
        /// Correct errors of min/max values for rotation
        /// </summary>
        private void CorrectMinMaxRotation()
        {
            if (m_fRotationMaxMagnitude < m_fRotationMinMagnitude) 
            {
                m_fRotationMinMagnitude = m_fRotationMaxMagnitude;
            }
        }
        #endregion
    }
}
