using UnityEngine;
using UnityEngine.UI;
using DG.DeExtensions;
using EA4S.Core;
using EA4S.Audio;
using EA4S.UI;
using EA4S.Database;
using EA4S.Rewards;

namespace EA4S.GamesSelector
{
    /// <summary>
    /// User interface of the GamesSelector.
    /// </summary>
    public class GamesSelectorUI : MonoBehaviour
    {
        public GameObject[] Stars;
        public TextRender TitleCode;
        public TextRender TitleEnglish;
        public TextRender TitleArabic;

        void Start()
        {
            // Fill with data
            JourneyPosition journeyPos = (AppManager.Instance as AppManager).Player.CurrentJourneyPosition;
            PlaySessionData playSessionData = (AppManager.Instance as AppManager).DB.GetPlaySessionDataById(journeyPos.ToStringId());
            LearningBlockData learningBlock = (AppManager.Instance as AppManager).DB.GetLearningBlockDataById(playSessionData.Stage + "." + playSessionData.LearningBlock.ToString());
            TitleCode.text = journeyPos.ToString();
            TitleArabic.text = learningBlock.Title_Ar;
            TitleEnglish.text = learningBlock.Title_En;

            // play the tutorial only if in LB 1.1
            if (journeyPos.Stage == 1 && journeyPos.LearningBlock == 1) {
                AudioManager.I.PlayDialogue(learningBlock.GetTitleSoundFilename(), PlayTutorialAudio);
                //KeeperManager.I.PlayDialog(learningBlock.GetTitleSoundFilename(), false, true, PlayTutorialAudio);
            } else {
                AudioManager.I.PlayDialogue(learningBlock.GetTitleSoundFilename());
            }

            if (!journeyPos.IsMinor((AppManager.Instance as AppManager).Player.MaxJourneyPosition)) {
                // First time playing this session: 0 stars
                SetStars(0);
            } else {
                int unlockedRewards = RewardSystemManager.GetUnlockedRewardForPlaysession((AppManager.Instance as AppManager).Player.CurrentJourneyPosition);
                SetStars(unlockedRewards + 1);
            }
            //            PlaySessionData playSessionData = AppManager.I.DB.GetPlaySessionDataById(journeyPos.PlaySession);
        }

        void PlayTutorialAudio()
        {
            AudioManager.I.PlayDialogue(LocalizationDataId.SelectGame_Tuto_2, null, true);
            //KeeperManager.I.PlayDialog(LocalizationDataId.SelectGame_Tuto_2, false, true);
        }

        void SetStars(int _tot)
        {
            if (_tot > 3) _tot = 3;
            for (int i = 0; i < Stars.Length; ++i) {
                GameObject star = Stars[i];
                star.SetActive(i < _tot);
                star.transform.parent.GetComponent<Image>().SetAlpha(i < _tot ? 1f : 0.3f);
            }
        }

    }
}