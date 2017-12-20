using Antura.Core;
using Antura.Database;
using Antura.Keeper;
using Antura.Profile;
using Antura.Tutorial;
using System.Collections;
using UnityEngine;

namespace Antura.Map
{
    public class MapTutorialManager : TutorialManager
    {
        private StageMapsManager _stageMapsManager;
        public GameObject tutorialUiGo;

        protected override void InternalHandleStart()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();

            // All UI is deactivated, for starters
            _stageMapsManager.DeactivateAllUI();

            // TODO: at the end, re-call this to check if we still have new tutorials for this scene
            switch (FirstContactManager.I.CurrentPhase) {
                case FirstContactPhase.Map_Play:

                    _stageMapsManager.SetPlayUIActivation(true);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_Map1);
                    });

                    StartCoroutine(TutorialHintClickCO(_stageMapsManager.SelectedPin.transform));

                    // @note: this phase is completed not on Play, but when we come back after the results
                    //FirstContactManager.I.PassPhase(FirstContactPhase.Map_Play);
                    // _stageMapsManager.playButton.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(CompleteTutorialPhase);
                    break;

                case FirstContactPhase.Map_GoToAnturaSpace:

                    // Wait for a specific PS to be unlocked before triggering this!
                    if (HasReachedJourneyPosition(1, 2, 1)) {

                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_AnturaSpace, true, true, () => {
                            _stageMapsManager.SetAnturaSpaceUIActivation(true);
                            StartCoroutine(TutorialHintClickCO(_stageMapsManager.anturaSpaceButton.transform));
                        });

                        //KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro, true, true, () =>

                        // @note: this phase is completed on transition to AnturaSpace
                        // _stageMapsManager.anturaSpaceButton.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(CompleteTutorialPhase);
                        //FirstContactManager.I.PassPhase(FirstContactPhase.Map_GoToAnturaSpace);
                    } else {
                        // Let him play
                        _stageMapsManager.SetPlayUIActivation(true);
                    }
                    break;


                case FirstContactPhase.Map_GoToBook:

                    // Older phases are enabled
                    _stageMapsManager.SetAnturaSpaceUIActivation(true);

                    // Wait for a specific PS to be unlocked before triggering this!
                    if (HasReachedJourneyPosition(1, 3, 1)) {
                        _stageMapsManager.SetLearningBookUIActivation(true);
                        StartCoroutine(TutorialHintClickCO(_stageMapsManager.learningBookButton.transform));
                    } else {
                        // Let the player play
                        _stageMapsManager.SetPlayUIActivation(true);
                    }
                    break;

                case FirstContactPhase.Map_GoToMinigames:

                    // Older phases are enabled
                    _stageMapsManager.SetAnturaSpaceUIActivation(true);
                    _stageMapsManager.SetLearningBookUIActivation(true);

                    // Wait for a specific PS to be unlocked before triggering this!
                    if (HasReachedJourneyPosition(1, 4, 1)) {
                        _stageMapsManager.SetMinigamesBookUIActivation(true);
                        StartCoroutine(TutorialHintClickCO(_stageMapsManager.minigamesBookButton.transform));
                    } else {
                        _stageMapsManager.SetPlayUIActivation(true);
                    }
                    break;
            }
        }

        private bool HasReachedJourneyPosition(int st, int lb, int ps)
        {
            return AppManager.I.Player.MaxJourneyPosition.IsGreaterOrEqual(new JourneyPosition(st, lb, ps));
        }


        private IEnumerator TutorialHintClickCO(Transform targetTr)
        {
            TutorialUI.SetCamera(_stageMapsManager.UICamera);
            var pos = targetTr.position;
            pos.y += 2;
            while (true) {
                TutorialUI.Click(pos);
                yield return new WaitForSeconds(0.85f);
            }
        }
    }
}