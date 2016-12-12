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
        [SerializeField]
        private OnActiveBehaviour m_oCustomButton;
        [SerializeField]
        private Music m_oBackgroundMusic;
        #endregion

        #region PRIVATE MEMBERS
        private GameObject[] m_aoPool;
        private int[] m_aiUsedList;
        private int m_iNextToBeUsed = 0;

        private GameObject m_oCookieRootContainer;
        private BoneBehaviour m_oDraggedBone;
        private AnturaBehaviour m_oAnturaBehaviour;

        private int m_iTotalBones_Local = 0; //maybe it's redundant, but can be useful for testing purpose 
        #endregion

        #region INTERNALS
        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, Exit);
            AudioManager.I.PlayMusic(m_oBackgroundMusic);
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "enter");

            m_iTotalBones_Local = AppManager.I.Player.GetTotalNumberOfBones();

            m_oTextBonesNumber.text = "" + m_iTotalBones_Local;

            //set the bone initial position behind the button
            /*
             * float _fCameraDistance = Mathf.Abs(Camera.main.transform.position.z - Camera.main.nearClipPlane);

            m_oBonePrefab.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(m_oTextBonesNumber.rectTransform.position.x, m_oTextBonesNumber.rectTransform.position.y, _fCameraDistance));
            */
            m_oBonePrefab.transform.position = m_oTextBonesNumber.transform.position;

            //Instantiate the pool of bones
            GameObject _oTempBase = new GameObject();
            m_oCookieRootContainer = Instantiate(_oTempBase);
            m_oCookieRootContainer.name = "[Cookies]";
            Destroy(_oTempBase);


            m_aoPool = new GameObject[m_iMaxSpawnableBones];
            m_aiUsedList = new int[m_iMaxSpawnableBones];

            for (int _iIdx = 0; _iIdx < m_iMaxSpawnableBones; ++_iIdx) {
                m_aoPool[_iIdx] = Instantiate(m_oBonePrefab);

                m_aoPool[_iIdx].SetActive(false);

                m_aiUsedList[_iIdx] = _iIdx;

                m_aoPool[_iIdx].transform.SetParent(m_oCookieRootContainer.transform);
            }

            //link variables        
            m_oAnturaBehaviour = m_oAntura.GetComponent<AnturaBehaviour>();
            m_oAnturaBehaviour.onBoneReached += release;
            m_oCustomButton.OnEnableAction += delegate { m_oAnturaBehaviour.Reset(); m_oAnturaBehaviour.IsInCustomization = true; };
            m_oCustomButton.OnDisableAction += delegate { m_oAnturaBehaviour.IsInCustomization = false; };

        }

        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Go back to the map.
        /// </summary>
        public void Exit()
        {
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "exit");
            NavigationManager.I.GoToScene(AppScene.Map);
        }

        /// <summary>
        /// Make a simple throw by activating a bone in the scene.
        /// </summary>
        public void ThrowBone()
        {
            GameObject _oBone = get();

            if (m_oDraggedBone != null || _oBone == null || m_iTotalBones_Local <= 0) {
                if (AppConstants.VerboseLogging)
                    Debug.Log("Can't throw bones");
                return;
            }

            DecreaseBonesNumber();

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

            if (m_oDraggedBone != null || _oBone == null || m_iTotalBones_Local <= 0) {
                if (AppConstants.VerboseLogging)
                    Debug.Log("Can't drag bones");
                return;
            }

            DecreaseBonesNumber();

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
            if (m_oDraggedBone == null) {
                if (AppConstants.VerboseLogging)
                    Debug.Log("No bone to let go");
                return;
            }

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

        /// <summary>
        /// Decrease the total number of bones of the player.
        /// (Compilation is different from Editor to Build)
        /// </summary>
        private void DecreaseBonesNumber()
        {
            m_iTotalBones_Local = --AppManager.I.Player.TotalNumberOfBones;
            m_oTextBonesNumber.text = "" + (m_iTotalBones_Local);
        }
        #endregion


    }
}