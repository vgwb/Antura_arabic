using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class BoneBehaviour : MonoBehaviour
    {

        #region EXPOSED MEMBERS
        [Header("Throw")]

        [SerializeField]
        private Vector3 m_v3BoneThrow_Direction;
        [SerializeField]
        private float m_fBoneThrow_Magnitude=10f;
        [SerializeField]
        private ForceMode m_eBoneThrow_ForceMode;

        [Header("Rotation")]

        [SerializeField]
        private Vector3 m_v3BoneRotation_Direction;
        [SerializeField]
        private float m_fBoneRotation_MinMagnitude = 0f;
        [SerializeField]
        private float m_fBoneRotation_MaxMagnitude = 10f;
        [SerializeField]
        private ForceMode m_eBoneRotation_ForceMode;
        #endregion

        #region PRIVATE MEMBERS
        private Rigidbody m_oBoneRigidboy;
        #endregion

        #region GETTER/SETTER

        public float boneRotation_MaxMagnitude
        {
            get { return m_fBoneRotation_MaxMagnitude; }
            set
            {
                m_fBoneRotation_MaxMagnitude = value;
                CorrectMinMaxRotation();
            }
        }

        public float boneRotation_MinMagnitude
        {
            get { return m_fBoneRotation_MinMagnitude; }
            set
            {
                m_fBoneRotation_MinMagnitude = value;
                CorrectMinMaxRotation();
            }
        }

        public Rigidbody boneRigidbody
        { get;set; }

        
        #endregion

        #region INTERNALS
        void Start()
        {
            m_oBoneRigidboy=GetComponent<Rigidbody>();

            CorrectMinMaxRotation();

            ApplyForces();
        }
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Apply the setted forces on the Rigidbody
        /// </summary>
        public void ApplyForces()
        {
            //Add rotation with random magnitude
            m_oBoneRigidboy.AddTorque(m_v3BoneRotation_Direction.normalized * Random.Range(m_fBoneRotation_MinMagnitude, m_fBoneRotation_MaxMagnitude), m_eBoneRotation_ForceMode);
            //Add translation
            m_oBoneRigidboy.AddForce(m_v3BoneThrow_Direction.normalized * m_fBoneThrow_Magnitude, m_eBoneThrow_ForceMode);
        }
        #endregion

        #region PRIVATE FUNCTIONS
        /// <summary>
        /// Correct errors of min/max values for rotation
        /// </summary>
        private void CorrectMinMaxRotation()
        {
            if (m_fBoneRotation_MaxMagnitude < m_fBoneRotation_MinMagnitude) 
            {
                m_fBoneRotation_MinMagnitude = m_fBoneRotation_MaxMagnitude;
            }
        }
        #endregion
    }
}
