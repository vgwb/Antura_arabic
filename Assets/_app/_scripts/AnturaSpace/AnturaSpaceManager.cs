using UnityEngine;
using System.Collections;

namespace EA4S
{

    public class AnturaSpaceManager : MonoBehaviour
    {

        #region EXPOSED MEMBERS
        [SerializeField]
        private int m_iMaxSpawnableBones = 0;
        [SerializeField]
        private GameObject m_oBonePrefab;
        [SerializeField]
        private UnityEngine.UI.Text m_oTextBonesNumber;

        [Header("Test")]
        private int m_iTotalBones_Test = 10;
        #endregion

        #region PRIVATE MEMBERS
        private GameObject[] m_aoPool;
        private int[] m_aiUsedList;
        private int m_iNextToBeUsed = 0;
        #endregion

        #region INTERNALS
        void Start()
        {
            GlobalUI.ShowPauseMenu(false);

#if UNITY_EDITOR

#else
            m_iTotalBones_Test = AppManager.I.Player.GetTotalNumberOfBones();
#endif

            m_oTextBonesNumber.text = "" + m_iTotalBones_Test;

            //Instantiate the pool of bones
            m_aoPool = new GameObject[m_iMaxSpawnableBones];
            m_aiUsedList = new int[m_iMaxSpawnableBones];

            for (int _iIdx = 0; _iIdx < m_iMaxSpawnableBones; ++_iIdx) {
                m_aoPool[_iIdx] = Instantiate(m_oBonePrefab);

                m_aoPool[_iIdx].SetActive(false);

                m_aiUsedList[_iIdx] = _iIdx;
            }

        }
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Go back to the map.
        /// </summary>
        public void Exit()
        {

#if UNITY_EDITOR
#else
            //assign new total bones used to the profile
#endif
            AppManager.I.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }

        /// <summary>
        /// Activate a bone in the scene.
        /// </summary>
        public void ThrowBone()
        {
            GameObject _oBone = get();

            if (_oBone == null || m_iTotalBones_Test <= 0) {
                Debug.Log("Can't throw bones");
                return;
            }

            Debug.Log("Throwing bone");
            m_oTextBonesNumber.text = "" + (--m_iTotalBones_Test);
            _oBone.SetActive(true);
        }

        /// <summary>
        /// Force release of all elements in the pool.
        /// </summary>
        public void ReleaseAll()
        {

            for (int _iIdx = 0; _iIdx < m_iMaxSpawnableBones; ++_iIdx) {
                m_aiUsedList[_iIdx] = _iIdx;
                ResetInstanceState(m_aoPool[_iIdx]);

            }

            m_iNextToBeUsed = 0;
        }
        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Get an object from the pool if available
        /// </summary>
        /// <returns>An instance of the pool's object, or null</returns>
        private GameObject get()
        {
            if (m_iNextToBeUsed >= m_iMaxSpawnableBones) //if all objects are in use
            {
                return null;
            } else {
                ++m_iNextToBeUsed;
                return m_aoPool[m_aiUsedList[m_iNextToBeUsed - 1]];
            }
        }

        /// <summary>
        /// Make the given object reusable by the pool.
        /// Call this when finished with a getted instance.
        /// </summary>
        /// <param name="goElement">The object to give back</param>
        private void release(GameObject goElement)
        {
            if (goElement != null) {
                int _iElementID = goElement.GetInstanceID();

                if (_iElementID <= m_aoPool[0].GetInstanceID() && _iElementID >= m_aoPool[m_iMaxSpawnableBones - 1].GetInstanceID()) //if the pool posses the argument passed
                {
                    //find actual position in the pool
                    int _iPosInPool = (m_aoPool[0].GetInstanceID() - _iElementID) / (m_aoPool[0].GetInstanceID() - m_aoPool[1].GetInstanceID());

                    for (int _iIdx = m_iNextToBeUsed; _iIdx <= m_iMaxSpawnableBones; ++_iIdx) {

                        if (_iIdx == m_iMaxSpawnableBones)//if this is the last cicle (all other have already passed) we can release
                        {

                            m_iNextToBeUsed--;
                            m_aiUsedList[m_iNextToBeUsed] = _iPosInPool;

                            ResetInstanceState(goElement);

                        } else if (m_aiUsedList[_iIdx] == _iPosInPool)//else if is true that the object has already been released
                          {
                            Debug.LogWarning("This object is already released in his pool.");
                            break;
                        }


                    }
                }
            }
        }

        /// <summary>
        /// Reset all needed properties to the prefab values
        /// </summary>
        /// <param name="goElement"></param>
        private void ResetInstanceState(GameObject goElement)
        {
            goElement.SetActive(false);

            //transform
            goElement.transform.position = m_oBonePrefab.transform.position;
            goElement.transform.rotation = m_oBonePrefab.transform.rotation;
            goElement.transform.localScale = m_oBonePrefab.transform.localScale;

            //others...
        }
        #endregion

    }
}