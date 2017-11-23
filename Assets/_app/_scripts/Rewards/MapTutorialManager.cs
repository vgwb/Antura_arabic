using System.Collections;
using Antura.Database;
using Antura.Keeper;
using Antura.Profile;
using Antura.Rewards;
using Antura.Tutorial;
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
                case FirstContactPhase.Map_GoToAnturaSpace:
                    _stageMapsManager.DeactivateUI();
                    _stageMapsManager.mapStageIndicator.gameObject.SetActive(false);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_AnturaSpace, true, true, () =>
                        {
                            _stageMapsManager.anturaSpaceButton.SetActive(true);
                            StartCoroutine(CO_Tutorial());
                        });
                    });

                    FirstContactManager.I.PassPhase(FirstContactPhase.Map_GoToAnturaSpace);
                    break;

                case FirstContactPhase.Map_Play:
                    _stageMapsManager.ActivateUI();
                    _stageMapsManager.mapStageIndicator.gameObject.SetActive(false);

                    FirstContactManager.I.PassPhase(FirstContactPhase.Map_Play);

                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () => {
                        KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_Map1);
                    });

                    //tuto anim on the play button
                    StartCoroutine(CO_Tutorial_PlayButton());
                    break;
            }
        }

        private IEnumerator CO_Tutorial()
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

        private IEnumerator CO_Tutorial_PlayButton()
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