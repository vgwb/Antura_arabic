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
        [SerializeField]
        private UnityEngine.UI.Button m_oExitButton;
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

            m_oCookieButton.gameObject.SetActive(false);
            m_oCustomizationButton.gameObject.SetActive(false);
            m_oExitButton.gameObject.SetActive(false);

            m_oAnturaBehaviour.onAnimationByClick += AdvanceTutorial;

            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro);

        }

        
        void Update()
        {

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

                    m_oAnturaBehaviour.onAnimationByClick -= AdvanceTutorial;

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Touch, delegate () //dialog touch Antura
                    {
                        m_oCookieButton.gameObject.SetActive(true); //after the dialog make appear the cookie button
                        m_oCookieButton.onClick.AddListener(AdvanceTutorial);//the button call AdvanceTutorial on click

                        AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () //dialog cookies
                        {
                            AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_1); //dialog tap for cookies
     
                        });
                    });

                    break;

                case eAnturaSpaceTutoState.COOKIE_BUTTON:

                    m_eTutoState = eAnturaSpaceTutoState.USE_ALL_COOKIES;

                    AudioManager.I.PlayDialog(Db.LocalizationDataId.AnturaSpace_Tuto_Cookie_2); //dialog drag cookies

                    break;

                case eAnturaSpaceTutoState.USE_ALL_COOKIES:
                    if (m_oCookieNumberText.text.CompareTo("0") == 0)
                    {
                        m_eTutoState = eAnturaSpaceTutoState.CUSTOMIZE;

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
                            });
                        });
                    }
                    
                    break;

                case eAnturaSpaceTutoState.CUSTOMIZE:

                    m_eTutoState = eAnturaSpaceTutoState.FINISH;

                    //--TODO think how detect antura dressup
                    m_oCustomizationButton.onClick.RemoveListener(AdvanceTutorial);
                    m_oExitButton.gameObject.SetActive(true);
                    AudioManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro_AnturaSpace); //dialog go to map
                    //--
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
