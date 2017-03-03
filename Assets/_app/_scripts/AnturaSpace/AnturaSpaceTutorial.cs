using EA4S.Audio;
using EA4S.Core;
using EA4S.Tutorial;
using EA4S.UI;
using UnityEngine;

namespace EA4S.AnturaSpace
{
 
    /// <summary>
    /// Implements a tutorial for the AnturaSpace scene.
    /// </summary>
    public class AnturaSpaceTutorial : MonoBehaviour
    {
        // conventions: rename enum 
        //note that the tutorial is totally sequentially
        enum eAnturaSpaceTutoState
        {
            ANTURA_ANIM=0, //touch antura
            COOKIE_BUTTON, //touch cookie button
            USE_ALL_COOKIES, //finish cookies
            CUSTOMIZE, //item set onto Antura
            FINISH //go to the map
        }

        #region EXPOSED MEMBERS
        private AnturaSpaceManager m_sceneManager;
        [SerializeField]
        private Camera m_oCameraUI;
        
        public AnturaLocomotion m_oAnturaBehaviour;
        //[SerializeField]
        //private GameObject m_oItemsParentUI;

        public AnturaSpaceUI UI;
        public UnityEngine.UI.Button m_oCookieButton;
        [SerializeField]
        private UnityEngine.UI.Button m_oCustomizationButton;
        #endregion

        #region PRIVATE MEMBERS
        private eAnturaSpaceTutoState m_eTutoState = eAnturaSpaceTutoState.ANTURA_ANIM;
        private bool m_bIsDragAnimPlaying = false;

        public bool IsRunning { get; private set; }
        #endregion

        #region GETTER/SETTER

        #endregion

        void Awake()
        {
            IsRunning = true;
        }

        #region INTERNALS
        void Start()
        {

            if(AppManager.I.Player.IsFirstContact()==false) //if this isn't the first contact disable yourself and return
            {
                gameObject.SetActive(false);
                IsRunning = false;
                return;
            }

            m_sceneManager = FindObjectOfType<AnturaSpaceManager>();
            m_sceneManager.Antura.transform.position = m_sceneManager.SceneCenter.position;
            m_sceneManager.Antura.AnimationController.State = AnturaAnimationStates.sleeping;
            m_sceneManager.CurrentState = m_sceneManager.Sleeping;

            TutorialUI.SetCamera(m_oCameraUI);

            //setup first state, disable UI    
            m_eTutoState = eAnturaSpaceTutoState.ANTURA_ANIM;
            UI.ShowBonesButton(false);
            m_oCustomizationButton.gameObject.SetActive(false);

            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro, null);

            m_oAnturaBehaviour.onTouched += AdvanceTutorial;
            Vector3 clickOffset = m_oAnturaBehaviour.IsSleeping ? Vector3.down * 2 : Vector3.zero;
            TutorialUI.ClickRepeat(m_oAnturaBehaviour.gameObject.transform.position + clickOffset + (Vector3.forward * -2) + (Vector3.up), float.MaxValue, 1);
        }

        void Update()
        {
            if(m_eTutoState==eAnturaSpaceTutoState.USE_ALL_COOKIES && AppManager.I.Player.GetTotalNumberOfBones()<=0)
            {
                AdvanceTutorial();
            }
        }

        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Advance the tutorial in his sequential flow.
        /// </summary>
        public void AdvanceTutorial()
        {
            if(!gameObject.activeSelf) //block any attempt to advance if tutorial isn't active
            {
                return;
            }

            switch(m_eTutoState)
            {
                case eAnturaSpaceTutoState.ANTURA_ANIM:

                    m_eTutoState = eAnturaSpaceTutoState.COOKIE_BUTTON;

                    TutorialUI.Clear(false);

                    m_oAnturaBehaviour.onTouched -= AdvanceTutorial;

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Touch, delegate () //dialog Antura
                    {

                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () //dialog cookies
                        {
                            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_1, delegate () //dialog tap for cookies
                            {
                                UI.ShowBonesButton(true); //after the dialog make appear the cookie button
                                m_oCookieButton.onClick.AddListener(AdvanceTutorial);//the button can call AdvanceTutorial on click

                                //RectTransform _oRectCookieB = m_oCookieButton.gameObject.GetComponent<RectTransform>();
                                TutorialUI.ClickRepeat(m_oCookieButton.transform.position/*m_oCameraUI.ScreenToWorldPoint(new Vector3(_oRectCookieB.position.x,_oRectCookieB.position.y, m_oCameraUI.nearClipPlane))*/, float.MaxValue, 1);

                            });

                        });
                    });

                    break;

                case eAnturaSpaceTutoState.COOKIE_BUTTON:

                    m_eTutoState = eAnturaSpaceTutoState.USE_ALL_COOKIES;

                    TutorialUI.Clear(false);

                    m_oCookieButton.onClick.RemoveListener(AdvanceTutorial);

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_2); //dialog drag cookies

                    m_bIsDragAnimPlaying = true;
                    DrawRepeatLineOnCookieButton();

                    //Register delegate to disable draw line after done
                    UnityEngine.EventSystems.EventTrigger.Entry _oEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                    _oEntry.eventID = UnityEngine.EventSystems.EventTriggerType.BeginDrag;
                    _oEntry.callback.AddListener((data) => { m_bIsDragAnimPlaying = false; });

                    m_oCookieButton.GetComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(_oEntry);
                    break;

                case eAnturaSpaceTutoState.USE_ALL_COOKIES:

                        m_eTutoState = eAnturaSpaceTutoState.CUSTOMIZE;

                        TutorialUI.Clear(false);

                        AudioManager.I.StopDialogue(false);

                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_3, delegate () //dialog get more cookies
                        {
                            
                            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Custom_1, delegate () //dialog customize
                            {
                                m_oCustomizationButton.gameObject.SetActive(true); //after the dialog make appear the customization button
                                m_oCustomizationButton.onClick.AddListener(AdvanceTutorial);

                                /*AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Custom_2, delegate() //dialog click customize
                                {
                                    //Not knowing how many object there are, register to all buttons
                                    //foreach (var it in m_oItemsParentUI.GetComponentsInChildren<UnityEngine.UI.Button>())
                                    //{
                                    //   it.onClick.AddListener(AdvanceTutorial);
                                    //}
                                    m_oCustomizationButton.onClick.AddListener(AdvanceTutorial);
                                });
                                */

                                /*RectTransform _oRectCustomB = m_oCustomizationButton.gameObject.GetComponent<RectTransform>();
                                TutorialUI.ClickRepeat(m_oCameraUI.ScreenToWorldPoint(new Vector3(_oRectCustomB.position.x, _oRectCustomB.position.y, m_oCameraUI.nearClipPlane)), float.MaxValue, 1);
                                */
                                TutorialUI.ClickRepeat(m_oCustomizationButton.transform.position, float.MaxValue, 1);

                            });
                        });
                    
                    
                    break;

                case eAnturaSpaceTutoState.CUSTOMIZE:
                    IsRunning = false;
                    m_eTutoState = eAnturaSpaceTutoState.FINISH;

                    TutorialUI.Clear(false);

                    /*//Unregister from object buttons
                    foreach (var it in m_oItemsParentUI.GetComponentsInChildren<UnityEngine.UI.Button>())
                    {
                        it.onClick.RemoveListener(AdvanceTutorial);
                    }*/
                    m_oCustomizationButton.onClick.RemoveListener(AdvanceTutorial);

                    m_sceneManager.ShowBackButton();

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.Map_Intro_AnturaSpace, delegate () //dialog go to map
                    {
                        //TutorialUI.ClickRepeat(m_oCameraUI.ScreenToWorldPoint(new Vector3(GlobalUI.I.BackButton.RectT.position.x, GlobalUI.I.BackButton.RectT.position.y, m_oCameraUI.nearClipPlane)), float.MaxValue, 1);
                    });
                   
                    break;

                default:
                    break;
            }

            
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private void DrawRepeatLineOnCookieButton()
        {
            TutorialUI.Clear(false);

            if(!m_bIsDragAnimPlaying) //stop 
            {
                return;
            }

            Vector3[] _av3Path = new Vector3[3];
            m_oCookieButton.gameObject.GetComponent<RectTransform>();
            _av3Path[0] = m_oCookieButton.transform.position;
            _av3Path[1] = _av3Path[0] + Vector3.up * 4 + Vector3.left * 2;
            _av3Path[2] = m_oCameraUI.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));

            _av3Path[2].z = _av3Path[1].z;

            TutorialUIAnimation _oDLAnim = TutorialUI.DrawLine(_av3Path, TutorialUI.DrawLineMode.Finger, false, true);
            _oDLAnim.MainTween.timeScale = 0.8f;
            _oDLAnim.OnComplete(delegate()
            {
                if(m_eTutoState!=eAnturaSpaceTutoState.CUSTOMIZE)
                {
                    DrawRepeatLineOnCookieButton();
                }
            });
        }
        #endregion
    }
}
