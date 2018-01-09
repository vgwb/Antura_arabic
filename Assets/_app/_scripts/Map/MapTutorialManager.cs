using System;
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

        protected override void InternalHandleStart()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();

            // All UI is deactivated, for starters
            _stageMapsManager.DeactivateAllUI();

            // Always activate the QUIT button
            _stageMapsManager.SetExitButtonActivation(true);

            // Antura Space (auto-unlocked from the start)
            if (CheckNewUnlockPhaseAt(1, 1, 1, FirstContactPhase.Map_GoToAnturaSpace, LocalizationDataId.Map_Intro_AnturaSpace))
                return;

            // Profile (auto-unlocked from the start)
            if (CheckNewUnlockPhaseAt(1, 1, 1, FirstContactPhase.Map_GoToProfile, LocalizationDataId.Map_Intro_AnturaSpace))
                return;

            // TODO: at the end, call CompleteCurrentPhase, if we need more phases in the same scene
            // Check for sequential phases
            bool isPlayingSequentialPhase = false;
            switch (FirstContactManager.I.CurrentPhaseInSequence)
            {
                case FirstContactPhase.Map_PlaySession:

                    _stageMapsManager.SetPlayUIActivation(true);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_Map1);
                    });

                    StartCoroutine(TutorialHintClickCO(_stageMapsManager.SelectedPin.transform));

                    // @note: this phase is completed not on Play, but when we come back after the results
                    // check the FirstContactManager navigation filtering
                    isPlayingSequentialPhase = true;
                    break;
            }
            if (isPlayingSequentialPhase) return;


            // New features unlocking

            // Book
            if (CheckNewUnlockPhaseAt(1, 3, 1, FirstContactPhase.Map_GoToBook, LocalizationDataId.Map_Intro_AnturaSpace))
                return;

            // MiniGames
            if (CheckNewUnlockPhaseAt(1, 4, 1, FirstContactPhase.Map_GoToMinigames, LocalizationDataId.Map_Intro_AnturaSpace))
                return;

            // If nothing is being unlocked, let the player play
            _stageMapsManager.SetPlayUIActivation(true);
            StopTutorialRunning();
        }

        private void AutoUnlockAndComplete(FirstContactPhase phase)
        {
            if (!FirstContactManager.I.HasCompletedPhase(phase))
            {
                FirstContactManager.I.CompletePhase(phase);
            }
        }

        private bool CheckNewUnlockPhaseAt(int st, int lb, int ps, FirstContactPhase phase, LocalizationDataId localizationDataId)
        {
            if (st == 1 && lb == 1 && ps == 1) AutoUnlockAndComplete(phase);
            bool unlockCondition = HasReachedJourneyPosition(st, lb, ps);
            bool isJustUnlocked = IsPhaseToBeCompleted(phase, unlockCondition);
            if (isJustUnlocked)
            {
                KeeperManager.I.PlayDialog(localizationDataId, true, true, () =>
                {
                    _stageMapsManager.SetUIActivationByContactPhase(phase);
                    StartCoroutine(TutorialHintClickCO(_stageMapsManager.GetGameObjectByContactPhase(phase).transform));
                });
                return true;
            }
            else if (IsPhaseUnlocked(phase))
            {
                _stageMapsManager.SetUIActivationByContactPhase(phase);
            }
            return false;
        }

        private bool HasReachedJourneyPosition(int st, int lb, int ps)
        {
            return AppManager.I.Player.MaxJourneyPosition.IsGreaterOrEqual(new JourneyPosition(st, lb, ps));
        }

        private IEnumerator TutorialHintClickCO(Transform targetTr)
        {
            TutorialUI.SetCamera(_stageMapsManager.UICamera);
            while (true) {
                TutorialUI.Click(targetTr.position);
                yield return new WaitForSeconds(0.85f);
            }
        }
    }
}