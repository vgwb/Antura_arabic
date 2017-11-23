using Antura.Profile;

namespace Antura.Rewards
{
    public class RewardsTutorialManager : TutorialManager
    {
        private RewardsScene _mScene;

        protected override void InternalHandleStart()
        {
            _mScene = FindObjectOfType<RewardsScene>();

            switch (FirstContactManager.I.CurrentPhase)
            {
                case FirstContactPhase.Intro:
                    _mScene.AnturaSpaceBtton.gameObject.SetActive(false);
                    break;
            }
        }
    }
}