using UnityEngine;
using System.Collections;

namespace EA4S
{
    /// <summary>
    /// Keeps the shadow at the same y value 
    /// </summary>
    public class BoneShadowController : MonoBehaviour
    {
        #region EXPOSED MEMBERS
        [SerializeField]
        private float m_fWorldY;
        [SerializeField]
        private Transform m_oTarget;
        #endregion

        #region EXPOSED MEMBERS
        private Quaternion m_oOriginalRotation;
        #endregion

        #region INTERNALS
        void Start()
        {
            m_oOriginalRotation = gameObject.transform.rotation;
        }

        void Update()
        {
            gameObject.transform.rotation = m_oOriginalRotation; //restore rotation to default
            gameObject.transform.position = new Vector3(m_oTarget.position.x, m_fWorldY, m_oTarget.position.z);// position under the target at the given height
        }
        #endregion
    }
}
