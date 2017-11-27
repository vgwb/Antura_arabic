using System.Collections;
using Antura.Core;
using Antura.Database;
using Antura.Keeper;
using Antura.Profile;
using Antura.Rewards;
using Antura.Tutorial;
using Antura.UI;
using UnityEngine;

namespace Antura.Map
{
    public class MapTutorialManager : TutorialManager
    {
        private StageMapsManager _stageMapsManager;
        private GameObject tutorialUiGo;

        protected override void InternalHandleStart()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();

            // TODO: at the end, re-call this to check if we still have new tutorials for this scene
            switch (FirstContactManager.I.CurrentPhase)
            {
                case FirstContactPhase.Map_Play:
                    _stageMapsManager.ActivateUI();
                    _stageMapsManager.mapStageIndicator.gameObject.SetActive(false);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_Map1);
                    });

                    //tuto anim on the play button
                    StartCoroutine(Tutorial_PlayButtonCO());

                    // TODO: on PlayButton clicked, pass this phase!
                    //FirstContactManager.I.PassPhase(FirstContactPhase.Map_Play);
                    _stageMapsManager.playButton.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(CompleteTutorialPhase);
                    break;

                case FirstContactPhase.Map_GoToAnturaSpace:
                    _stageMapsManager.DeactivateUI();
                    _stageMapsManager.mapStageIndicator.gameObject.SetActive(false);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_AnturaSpace, true, true, () =>
                        {
                            _stageMapsManager.anturaSpaceButton.SetActive(true);
                            StartCoroutine(Tutorial_GoToAnturaSpaceCO());
                        });
                    });

                    // TODO: on AnturaSpaceButton clicked, pass this phase!
                    _stageMapsManager.anturaSpaceButton.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(CompleteTutorialPhase);
                    //FirstContactManager.I.PassPhase(FirstContactPhase.Map_GoToAnturaSpace);
                    break;


                case FirstContactPhase.Map_GoToBook:

                    // TODO: wait for a specific PS to be unlocked before triggering this!
                    if (!AppManager.I.Player.MaxJourneyPosition.IsMinorOrEqual(new JourneyPosition(1,2,2)))
                    {
                        // TODO: trigger the tutorial here!
                    }
                    break;
            }
        }


        #region Play

        private IEnumerator Tutorial_PlayButtonCO()
        {
            TutorialUI.SetCamera(_stageMapsManager.UICamera);
            var pos = _stageMapsManager.playButton.transform.position;
            pos.y += 2;
            while (true)
            {
                TutorialUI.Click(pos);
                yield return new WaitForSeconds(0.85f);
            }
        }

        #endregion

        #region Go To AnturaSpace

        private IEnumerator Tutorial_GoToAnturaSpaceCO()
        {
            TutorialUI.SetCamera(_stageMapsManager.UICamera);
            var anturaBtPos = _stageMapsManager.anturaSpaceButton.transform.position;
            anturaBtPos.z -= 1;
            while (true)
            {
                TutorialUI.Click(_stageMapsManager.anturaSpaceButton.transform.position);
                yield return new WaitForSeconds(0.85f);
            }
        }

        #endregion
        private void HideTutorial()
        {
            tutorialUiGo = GameObject.Find("[TutorialUI]");
            if (tutorialUiGo != null) tutorialUiGo.transform.localScale = new Vector3(0, 0, 0);
        }

        private void ShowTutorial()
        {
            if (tutorialUiGo != null) tutorialUiGo.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}