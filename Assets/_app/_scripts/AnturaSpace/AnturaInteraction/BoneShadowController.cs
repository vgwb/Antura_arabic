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
        private float m_fYHeight;
        [SerializeField]
        private Transform m_oTarget;
        #endregion

        #region INTERNALS
        void Start()
        {

        }

        void Update()
        {
            gameObject.transform.position = new Vector3(m_oTarget.position.x, m_fYHeight, m_oTarget.position.z);
        }
        #endregion
    }
}
