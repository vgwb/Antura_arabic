using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S
{
 
    public class AnturaSpaceTutorial : MonoBehaviour
    {
        //note that the tutorial is totally sequentially
        enum eAnturaSpaceTutoState
        {
            ANTURA_ANIM=0, //touch antura
            COOKIE_BUTTON, //touch cookie button
            USE_ALL_COOKIES, //finish cookie
            CUSTOMIZE, //item set onto Antura
            FINISH //go to the map
        }

        #region EXPOSED MEMBERS
        [SerializeField]
        private Camera m_oCameraUI;
        [SerializeField]
        private AnturaBehaviour m_oAnturaBehaviour;
        [SerializeField]
        private GameObject m_oItemsParentUI;
        [SerializeField]
        private UnityEngine.UI.Button m_oCookieButton;
        [SerializeField]
        private TextMeshProUGUI m_oCookieNumberText;
        [SerializeField]
        private UnityEngine.UI.Button m_oCustomizationButton;
        #endregion

        #region PRIVATE MEMBERS
        private eAnturaSpaceTutoState m_eTutoState = eAnturaSpaceTutoState.ANTURA_ANIM;
        #endregion

        #region GETTER/SETTER
        
        #endregion

        #region INTERNALS
        void Start()
        {
            if(AppManager.I.Player.IsFirstContact()==false) //if this isn't the first contact disable yourself and return
            {
               
                gameObject.SetActive(false);
                return;
            }

            TutorialUI.SetCamera(m_oCameraUI);

            //setup first state, disable UI      
            m_eTutoState = eAnturaSpaceTutoState.ANTURA_ANIM;

#if UNITY_EDITOR

#else
            GlobalUI.ShowBackButton(false);
#endif

            m_oCookieButton.gameObject.SetActive(false);
            m_oCustomizationButton.gameObject.SetActive(false);

            m_oAnturaBehaviour.onAnimationByClick += AdvanceTutorial;

            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro, delegate()
            {
                TutorialUI.ClickRepeat(m_oAnturaBehaviour.gameObject.transform.position+(Vector3.forward*-2) + (Vector3.up), float.MaxValue, 1);
            });

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

                    m_oAnturaBehaviour.onAnimationByClick -= AdvanceTutorial;

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Touch, delegate () //dialog touch Antura
                    {
                        m_oCookieButton.gameObject.SetActive(true); //after the dialog make appear the cookie button
                        m_oCookieButton.interactable = false; //disable interaction temporary 
                        
                        AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () //dialog cookies
                        {
                            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_1, delegate() //dialog tap for cookies
                            {
                                m_oCookieButton.onClick.AddListener(AdvanceTutorial);//the button can call AdvanceTutorial on click
                                m_oCookieButton.interactable = true;
                            }); 

                            RectTransform _oRectCookieB = m_oCookieButton.gameObject.GetComponent<RectTransform>();
                            TutorialUI.ClickRepeat(_oRectCookieB.position/*m_oCameraUI.ScreenToWorldPoint(new Vector3(_oRectCookieB.position.x,_oRectCookieB.position.y, m_oCameraUI.nearClipPlane))*/, float.MaxValue,1);

                        });
                    });

                    break;

                case eAnturaSpaceTutoState.COOKIE_BUTTON:

                    m_eTutoState = eAnturaSpaceTutoState.USE_ALL_COOKIES;

                    TutorialUI.Clear(false);

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_2); //dialog drag cookies

                    DrawRepeatLineOnCookieButton();

                    break;

                case eAnturaSpaceTutoState.USE_ALL_COOKIES:
                    if (m_oCookieNumberText.text.CompareTo("0") == 0)
                    {
                        m_eTutoState = eAnturaSpaceTutoState.CUSTOMIZE;

                        TutorialUI.Clear(false);

                        m_oCookieButton.onClick.RemoveListener(AdvanceTutorial);

                        AudioManager.I.StopDialogue(false);

                        AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_3, delegate () //dialog get more cookies
                        {
                            m_oCustomizationButton.gameObject.SetActive(true); //after the dialog make appear the customization button

                            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Custom_1, delegate () //dialog customize
                            {
                                AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Custom_2, delegate() //dialog click customize
                                {
                                    //Not knowing how many object there are, register to all buttons
                                    foreach (var it in m_oItemsParentUI.GetComponentsInChildren<UnityEngine.UI.Button>())
                                    {
                                        it.onClick.AddListener(AdvanceTutorial);
                                    }
                                }); 

                                /*RectTransform _oRectCustomB = m_oCustomizationButton.gameObject.GetComponent<RectTransform>();
                                TutorialUI.ClickRepeat(m_oCameraUI.ScreenToWorldPoint(new Vector3(_oRectCustomB.position.x, _oRectCustomB.position.y, m_oCameraUI.nearClipPlane)), float.MaxValue, 1);
                                */
                                TutorialUI.ClickRepeat(m_oCustomizationButton.transform.position, float.MaxValue, 1);

                            });
                        });
                    }
                    
                    break;

                case eAnturaSpaceTutoState.CUSTOMIZE:

                    m_eTutoState = eAnturaSpaceTutoState.FINISH;

                    TutorialUI.Clear(false);

                    //Unregister from object buttons
                    foreach (var it in m_oItemsParentUI.GetComponentsInChildren<UnityEngine.UI.Button>())
                    {
                        it.onClick.RemoveListener(AdvanceTutorial);
                    }

                    GlobalUI.ShowBackButton(true,delegate() { NavigationManager.I.GoToScene(AppScene.Map); });

                    AudioManager.I.StopDialogue(false);

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro_AnturaSpace, delegate () //dialog go to map
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

            Vector3[] _av3Path = new Vector3[2];
            RectTransform _oRectCookieBDrag = m_oCookieButton.gameObject.GetComponent<RectTransform>();
            _av3Path[0] = m_oCookieButton.transform.position;
            _av3Path[1] = _av3Path[0] + Vector3.up * 2 + Vector3.left * 2;
            TutorialUIAnimation _oDLAnim = TutorialUI.DrawLine(_av3Path[0], _av3Path[1], TutorialUI.DrawLineMode.Finger, false, true);
            _oDLAnim.MainTween.timeScale = 0.3f;
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
