using UnityEngine;
using System.Collections;
using TMPro;

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
        private GameObject m_oAntura;
        [SerializeField]
        private TextMeshProUGUI m_oTextBonesNumber;



        [Header("Test")]
        public int m_iTotalBones_Test = 10;
        #endregion

        #region PRIVATE MEMBERS
        private GameObject[] m_aoPool;
        private int[] m_aiUsedList;
        private int m_iNextToBeUsed = 0;

        //private Rigidbody m_oBonePrefabRigidboy;
        private BoneBehaviour m_oDraggedBone;
        private AnturaSpaceAnturaBehaviour m_oAnturaBehaviour;
        #endregion

        #region INTERNALS
        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true,Exit);
            


#if UNITY_EDITOR

#else
            m_iTotalBones_Test = AppManager.I.Player.GetTotalNumberOfBones();
#endif

            m_oTextBonesNumber.text = "" + m_iTotalBones_Test;

            //set the bone initial position behind the button
            /*
             * float _fCameraDistance = Mathf.Abs(Camera.main.transform.position.z - Camera.main.nearClipPlane);

            m_oBonePrefab.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(m_oTextBonesNumber.rectTransform.position.x, m_oTextBonesNumber.rectTransform.position.y, _fCameraDistance));
            */
            m_oBonePrefab.transform.position = m_oTextBonesNumber.transform.position;

            //Instantiate the pool of bones
            m_aoPool = new GameObject[m_iMaxSpawnableBones];
            m_aiUsedList = new int[m_iMaxSpawnableBones];

            for (int _iIdx = 0; _iIdx < m_iMaxSpawnableBones; ++_iIdx) {
                m_aoPool[_iIdx] = Instantiate(m_oBonePrefab);

                m_aoPool[_iIdx].SetActive(false);

                m_aiUsedList[_iIdx] = _iIdx;
            }

            //link variables
            //m_oBonePrefabRigidboy = m_oBonePrefab.GetComponent<Rigidbody>();
        
            m_oAnturaBehaviour = m_oAntura.GetComponent<AnturaSpaceAnturaBehaviour>();
            m_oAnturaBehaviour.onBoneReached += release;

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
            //AppManager.I.Player.SetTotalNumberOfBones(m_iTotalBones_Test);//assign new total bones used to the profile
#endif
            NavigationManager.I.GoToScene(AppScene.Map);
        }

        /// <summary>
        /// Make a simple throw by activating a bone in the scene.
        /// </summary>
        public void ThrowBone()
        {
            GameObject _oBone = get();

            if (m_oDraggedBone != null || _oBone == null || m_iTotalBones_Test <= 0) {
                Debug.Log("Can't throw bones");
                return;
            }

            Debug.Log("Throwing bone");
            m_oTextBonesNumber.text = "" + (--m_iTotalBones_Test);
            _oBone.SetActive(true);
            _oBone.GetComponent<BoneBehaviour>().SimpleThrow();
            m_oAnturaBehaviour.AddBone(_oBone);
        }

        /// <summary>
        /// Drag a bone around.
        /// </summary>
        public void DragBone()
        {
            GameObject _oBone = get();

            if (m_oDraggedBone!=null || _oBone == null || m_iTotalBones_Test <= 0)
            {
                Debug.Log("Can't drag bones");
                return;
            }

            Debug.Log("Dragging bone");
            m_oTextBonesNumber.text = "" + (--m_iTotalBones_Test);

            _oBone.SetActive(true);
            m_oDraggedBone = _oBone.GetComponent<BoneBehaviour>();
            m_oDraggedBone.Drag();
            m_oAnturaBehaviour.AddBone(_oBone);
        }

        /// <summary>
        /// Let go the current bone being dragged.
        /// </summary>
        public void LetGoBone()
        {
            if(m_oDraggedBone==null)
            {
                Debug.Log("No bone to let go");
                return;
            }

            Debug.Log("Let go bone");
            m_oDraggedBone.LetGo();
            m_oDraggedBone = null;

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
        /// Reset all needed properties to the prefab values.
        /// This do not make a new instance from the prefab, so
        /// any property that need to be reset must be put here.
        /// </summary>
        /// <param name="goElement"></param>
        private void ResetInstanceState(GameObject goElement)
        {
            goElement.SetActive(false);

            //transform (it's a common case)
            goElement.transform.position = m_oBonePrefab.transform.position;
            goElement.transform.rotation = m_oBonePrefab.transform.rotation;
            goElement.transform.localScale = m_oBonePrefab.transform.localScale;

            //others specific...
            //goElement.GetComponent<Rigidbody>().isKinematic = m_oBonePrefabRigidboy.isKinematic;
        }
        #endregion

    }
}