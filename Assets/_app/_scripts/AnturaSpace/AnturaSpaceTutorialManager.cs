using Antura.AnturaSpace.UI;
using Antura.Core;
using Antura.Dog;
using Antura.Audio;
using Antura.Tutorial;
using Antura.UI;
using UnityEngine;
using System.Collections;
using Antura.Profile;
using Antura.Rewards;

namespace Antura.AnturaSpace
{
    /// <summary>
    /// Implements a tutorial for the AnturaSpace scene.
    /// </summary>
    public class AnturaSpaceTutorialManager : TutorialManager
    {
      
        #region EXPOSED MEMBERS

        private AnturaSpaceScene _mScene;

        [SerializeField]
        private Camera m_oCameraUI;

        public AnturaLocomotion m_oAnturaBehaviour;
        public AnturaSpaceUI UI;
        public ShopDecorationsManager ShopDecorationsManager;
        public UnityEngine.UI.Button m_oCookieButton;

        [SerializeField]
        private UnityEngine.UI.Button m_oCustomizationButton;

        AnturaSpaceCategoryButton m_oCategoryButton;
        AnturaSpaceItemButton m_oItemButton;

        #endregion

        protected override void InternalHandleStart()
        {
            _mScene = FindObjectOfType<AnturaSpaceScene>();
            _mScene.Antura.transform.position = _mScene.SceneCenter.position;
            _mScene.Antura.AnimationController.State = AnturaAnimationStates.sleeping;
            _mScene.CurrentState = _mScene.Sleeping;
            _mScene.HideBackButton();

            TutorialUI.SetCamera(m_oCameraUI);

            // First, disable all UI
            //_currentTutorialStep = AnturaSpaceTutorialStep.ANTURA_ANIM;
            UI.ShowBonesButton(false);
            ShopDecorationsManager.SetContextHidden();
            m_oCustomizationButton.gameObject.SetActive(false);

            // Define what tutorial phase to play
            switch (FirstContactManager.I.CurrentPhase)
            {
                 case FirstContactPhase.AnturaSpace_TouchAntura:
                    AdvanceTutorialTouchAntura();
                    break;

                case FirstContactPhase.AnturaSpace_Customization:
                    AdvanceTutorialCustomization();
                    break;

                case FirstContactPhase.AnturaSpace_Shop:
                    AdvanceTutorialShop();
                    break;

                case FirstContactPhase.AnturaSpace_Photo:
                    break;

                case FirstContactPhase.AnturaSpace_Exit:
                    AdvanceTutorialExit();
                    break;
            }
            
        }

        void Update()
        {
            if (_currentShopStep == ShopTutorialStep.USE_ALL_COOKIES && AppManager.I.Player.GetTotalNumberOfBones() <= 0) {
                AdvanceTutorialShop();
            }
        }

        #region Touch Antura

        private void AdvanceTutorialTouchAntura()
        {
            // Push the player to touch Antura

            //TutorialUI.Clear(false);

            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro, null);

            m_oAnturaBehaviour.onTouched += CompleteTutorialPhase;
            Vector3 clickOffset = m_oAnturaBehaviour.IsSleeping ? Vector3.down * 2 : Vector3.zero;
            TutorialUI.ClickRepeat(m_oAnturaBehaviour.gameObject.transform.position + clickOffset + Vector3.forward * -2 + Vector3.up,
                float.MaxValue, 1);
        }

        #endregion

        #region Customization

        private enum CustomizationTutorialStep
        {
            START,
            OPEN_CUSTOMIZE,
            SELECT_CATEGORY,
            SELECT_ITEM,
            TOUCH_ANTURA,
            FINISH
        }

        private CustomizationTutorialStep _currentCustomizationStep = CustomizationTutorialStep.START;
        private void AdvanceTutorialCustomization()
        {
            Debug.Log("CURRENT IS " + _currentCustomizationStep);
            switch (_currentCustomizationStep)
            {
                case CustomizationTutorialStep.START:
                    _currentCustomizationStep = CustomizationTutorialStep.OPEN_CUSTOMIZE;
                    TutorialUI.Clear(false);
                    AudioManager.I.StopDialogue(false);

                    //dialog get more cookies
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_3, delegate () {
                        //dialog customize
                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Custom_1, delegate () {
                            //after the dialog make appear the customization button
                            m_oCustomizationButton.gameObject.SetActive(true);
                            m_oCustomizationButton.onClick.AddListener(AdvanceTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oCustomizationButton.transform.position, float.MaxValue, 1);
                        });
                    });
                    break;


                case CustomizationTutorialStep.OPEN_CUSTOMIZE:
                    _currentCustomizationStep = CustomizationTutorialStep.SELECT_CATEGORY;

                    TutorialUI.Clear(false);

                    m_oCustomizationButton.onClick.RemoveListener(AdvanceTutorialCustomization);
                    _mScene.UI.SetTutorialMode(true);

                    StartCoroutine(WaitAndSpawnCO(
                        () => {
                            m_oCategoryButton = _mScene.UI.GetNewCategoryButton();
                            if (m_oCategoryButton == null)
                            {
                                AdvanceTutorialCustomization();
                                return;
                            }

                            m_oCategoryButton.Bt.onClick.AddListener(AdvanceTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oCategoryButton.transform.position, float.MaxValue, 1);
                        }));
                    break;

                case CustomizationTutorialStep.SELECT_CATEGORY:
                    _currentCustomizationStep = CustomizationTutorialStep.SELECT_ITEM;

                    TutorialUI.Clear(false);

                    //Unregister from category button
                    if (m_oCategoryButton != null)
                    {
                        m_oCategoryButton.Bt.onClick.RemoveListener(AdvanceTutorialCustomization);
                    }
                    else {
                        AdvanceTutorialCustomization();
                        break;
                    }

                    StartCoroutine(WaitAndSpawnCO(
                        () => {
                            // Register on item button
                            m_oItemButton = _mScene.UI.GetNewItemButton();

                            if (m_oItemButton == null)
                            {
                                AdvanceTutorialCustomization();
                                return;
                            }

                            m_oItemButton.Bt.onClick.AddListener(AdvanceTutorialCustomization);

                            TutorialUI.ClickRepeat(m_oItemButton.transform.position, float.MaxValue, 1);
                        }));
                    break;

                case CustomizationTutorialStep.SELECT_ITEM:

                    _currentCustomizationStep = CustomizationTutorialStep.TOUCH_ANTURA;

                    TutorialUI.Clear(false);

                    _mScene.UI.SetTutorialMode(false);

                    //Unregister from Item button
                    if (m_oItemButton != null)
                    {
                        m_oItemButton.Bt.onClick.RemoveListener(AdvanceTutorialCustomization);
                    }

                    StartCoroutine(WaitAnturaInCenterCO(
                        () => {
                            // Register on Antura touch
                            m_oAnturaBehaviour.onTouched += AdvanceTutorialCustomization;

                            Vector3 clickOffset = m_oAnturaBehaviour.IsSleeping ? Vector3.down * 2 : Vector3.down * 1.5f;
                            TutorialUI.ClickRepeat(
                                m_oAnturaBehaviour.gameObject.transform.position + clickOffset + Vector3.forward * -2 + Vector3.up,
                                float.MaxValue, 1);
                        }));

                    break;

                case CustomizationTutorialStep.TOUCH_ANTURA:

                    _currentCustomizationStep = CustomizationTutorialStep.FINISH;

                    TutorialUI.Clear(false);

                    m_oAnturaBehaviour.onTouched -= AdvanceTutorialCustomization;

                    CompleteTutorialPhase();
                    break;
            }
        }

        #endregion

        #region Shop

        private enum ShopTutorialStep
        {
            START,
            COOKIE_BUTTON,
            USE_ALL_COOKIES,
            FINISH
        }

        private ShopTutorialStep _currentShopStep = ShopTutorialStep.START;
        private void AdvanceTutorialShop()
        {
            switch (_currentShopStep)
            {
                case ShopTutorialStep.START:
                    _currentShopStep = ShopTutorialStep.COOKIE_BUTTON;

                    TutorialUI.Clear(false);

                    m_oAnturaBehaviour.onTouched -= AdvanceTutorialShop;

                    AudioManager.I.StopDialogue(false);

                    //dialog Antura
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Touch, delegate () {
                        //dialog cookies
                        AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Intro_Cookie, delegate () {
                            //after the dialog make appear the cookie button
                            UI.ShowBonesButton(true);
                            //the button can call AdvanceTutorial on click
                            m_oCookieButton.onClick.AddListener(AdvanceTutorialShop);

                            TutorialUI.ClickRepeat(m_oCookieButton.transform.position, float.MaxValue, 1);

                            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_1, null);
                        });
                    });

                    break;

                case ShopTutorialStep.COOKIE_BUTTON:
                    _currentShopStep = ShopTutorialStep.USE_ALL_COOKIES;

                    TutorialUI.Clear(false);

                    m_oCookieButton.onClick.RemoveListener(AdvanceTutorialShop);

                    AudioManager.I.StopDialogue(false);

                    //dialog drag cookies
                    AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Tuto_Cookie_2);

                    m_bIsDragAnimPlaying = true;
                    DrawRepeatLineOnCookieButton();

                    //Register delegate to disable draw line after done
                    UnityEngine.EventSystems.EventTrigger.Entry _oEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                    _oEntry.eventID = UnityEngine.EventSystems.EventTriggerType.BeginDrag;
                    _oEntry.callback.AddListener((data) => { m_bIsDragAnimPlaying = false; });

                    m_oCookieButton.GetComponent<UnityEngine.EventSystems.EventTrigger>().triggers.Add(_oEntry);
                    break;


                case ShopTutorialStep.USE_ALL_COOKIES:
                    _currentShopStep = ShopTutorialStep.FINISH;
                    CompleteTutorialPhase();
                    break;
            }
        }

        #endregion

        #region Exit

        private void AdvanceTutorialExit()
        {
            TutorialUI.Clear(false);

            _mScene.ShowBackButton();
            UI.ShopPanelContainer.gameObject.SetActive(true);

            AudioManager.I.StopDialogue(false);

            TutorialUI.ClickRepeat(
                Vector3.down * 0.025f + m_oCameraUI.ScreenToWorldPoint(new Vector3(GlobalUI.I.BackButton.RectT.position.x,
                    GlobalUI.I.BackButton.RectT.position.y, m_oCameraUI.nearClipPlane)), float.MaxValue, 1);
        }

        #endregion

        #region Utility functions

        IEnumerator WaitAndSpawnCO(System.Action callback)
        {
            yield return new WaitForSeconds(0.6f);

            if (callback != null) {
                callback();
            }
        }

        IEnumerator WaitAnturaInCenterCO(System.Action callback)
        {
            while (!_mScene.Antura.IsNearTargetPosition || _mScene.Antura.IsSliping)
                yield return null;

            if (callback != null) {
                callback();
            }
        }

        private bool m_bIsDragAnimPlaying = false;
        private void DrawRepeatLineOnCookieButton()
        {
            TutorialUI.Clear(false);

            //stop 
            if (!m_bIsDragAnimPlaying) {
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
            _oDLAnim.OnComplete(delegate () {
                if (_currentCustomizationStep != CustomizationTutorialStep.OPEN_CUSTOMIZE) {
                    DrawRepeatLineOnCookieButton();
                }
            });
        }

        #endregion
    }
}