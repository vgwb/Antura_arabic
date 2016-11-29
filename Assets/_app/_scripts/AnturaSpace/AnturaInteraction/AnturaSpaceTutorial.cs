using UnityEngine;
using System.Collections;

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
        private AnturaSpaceAnturaBehaviour m_oAnturaBehaviour;
        [SerializeField]
        private UnityEngine.UI.Button m_oCookieButton;
        [SerializeField]
        private UnityEngine.UI.Text m_oCookieNumberText;
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

            //setup first state, disable UI      
            m_eTutoState = eAnturaSpaceTutoState.ANTURA_ANIM;

            GlobalUI.ShowBackButton(false);
            m_oCookieButton.gameObject.SetActive(false);
            m_oCustomizationButton.gameObject.SetActive(false);

            m_oAnturaBehaviour.onAnimationByClick += AdvanceTutorial;

            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro, delegate()
            {
                TutorialUI.ClickRepeat(m_oAnturaBehaviour.gameObject.transform.position+(Vector3.forward*-2), float.MaxValue, 1);
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

                    TutorialUI.Clear(true);

                    m_oAnturaBehaviour.onAnimationByClick -= AdvanceTutorial;

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Touch, delegate () //dialog touch Antura
                    {
                        m_oCookieButton.gameObject.SetActive(true); //after the dialog make appear the cookie button
                        m_oCookieButton.onClick.AddListener(AdvanceTutorial);//the button call AdvanceTutorial on click

                        AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () //dialog cookies
                        {
                            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_1); //dialog tap for cookies
                            RectTransform _oRectCookieB = m_oCookieButton.gameObject.GetComponent<RectTransform>();
                            TutorialUI.ClickRepeat(Camera.main.ScreenToWorldPoint(new Vector3(_oRectCookieB.position.x,_oRectCookieB.position.y,Camera.main.nearClipPlane)), float.MaxValue,1);

                        });
                    });

                    break;

                case eAnturaSpaceTutoState.COOKIE_BUTTON:

                    m_eTutoState = eAnturaSpaceTutoState.USE_ALL_COOKIES;

                    TutorialUI.Clear(true);

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_2); //dialog drag cookies

                    Vector3[] _av3Path = new Vector3[2];
                    RectTransform _oRectCookieBDrag = m_oCookieButton.gameObject.GetComponent<RectTransform>();
                    _av3Path[0] = Camera.main.ScreenToWorldPoint(new Vector3(_oRectCookieBDrag.position.x, _oRectCookieBDrag.position.y, Camera.main.nearClipPlane));
                    _av3Path[1] = _av3Path[0] + Vector3.up * 2 + Vector3.left * 2;
                    TutorialUI.DrawLine(_av3Path[0], _av3Path[1], TutorialUI.DrawLineMode.FingerAndArrow, true, false);

                    break;

                case eAnturaSpaceTutoState.USE_ALL_COOKIES:
                    if (m_oCookieNumberText.text.CompareTo("0") == 0)
                    {
                        m_eTutoState = eAnturaSpaceTutoState.CUSTOMIZE;

                        TutorialUI.Clear(true);

                        m_oCookieButton.onClick.RemoveListener(AdvanceTutorial);

                        AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_3, delegate () //dialog get more cookies
                        {
                            m_oCustomizationButton.gameObject.SetActive(true); //after the dialog make appear the customization button

                            //--TODO think how detect antura dressup
                            m_oCustomizationButton.onClick.AddListener(AdvanceTutorial);//the button call AdvanceTutorial on click
                            //--

                            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Custom_1, delegate () //dialog customize
                            {
                                AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Custom_2); //dialog click customize

                                RectTransform _oRectCustomB = m_oCustomizationButton.gameObject.GetComponent<RectTransform>();
                                TutorialUI.ClickRepeat(Camera.main.ScreenToWorldPoint(new Vector3(_oRectCustomB.position.x, _oRectCustomB.position.y, Camera.main.nearClipPlane)), float.MaxValue, 1);

                            });
                        });
                    }
                    
                    break;

                case eAnturaSpaceTutoState.CUSTOMIZE:

                    m_eTutoState = eAnturaSpaceTutoState.FINISH;

                    TutorialUI.Clear(true);

                    //--TODO think how detect antura dressup
                    m_oCustomizationButton.onClick.RemoveListener(AdvanceTutorial);
                    GlobalUI.ShowBackButton(true);

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro_AnturaSpace, delegate () //dialog go to map
                    {
                        TutorialUI.ClickRepeat(Camera.main.ScreenToWorldPoint(new Vector3(GlobalUI.I.BackButton.RectT.position.x, GlobalUI.I.BackButton.RectT.position.y, Camera.main.nearClipPlane)), float.MaxValue, 1);
                    });
                    //--
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
