using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EA4S.Database;

namespace EA4S.Core
{
    public static class MinigamesUtilities
    {
        
        public static List<MainMiniGame> GetMainMiniGameList()
        {
            Dictionary<string, MainMiniGame> dictionary = new Dictionary<string, MainMiniGame>();
            List<MiniGameInfo> minigameInfoList = AppManager.I.ScoreHelper.GetAllMiniGameInfo();
            foreach (var minigameInfo in minigameInfoList) {
                if (!dictionary.ContainsKey(minigameInfo.data.Main)) {
                    dictionary[minigameInfo.data.Main] = new MainMiniGame {
                        id = minigameInfo.data.Main,
                        variations = new List<MiniGameInfo>()
                    };
                }
                dictionary[minigameInfo.data.Main].variations.Add(minigameInfo);
            }

            List<MainMiniGame> outputList = new List<MainMiniGame>();
            foreach (var k in dictionary.Keys) {
                if (dictionary[k].id != "Assessment") {
                    outputList.Add(dictionary[k]);

                }
            }

            // Sort minigames and variations based on their minimum journey position
            Dictionary<MiniGameCode, JourneyPosition> minimumJourneyPositions = new Dictionary<MiniGameCode, JourneyPosition>();
            foreach (var mainMiniGame in outputList) {
                foreach (var miniGameInfo in mainMiniGame.variations) {
                    var miniGameCode = miniGameInfo.data.Code;
                    minimumJourneyPositions[miniGameCode] = AppManager.I.JourneyHelper.GetMinimumJourneyPositionForMiniGame(miniGameCode);
                }
            }

            // First sort variations (so the first variation is in front)
            foreach (var mainMiniGame in outputList) {
                mainMiniGame.variations.Sort((g1, g2) => minimumJourneyPositions[g1.data.Code].IsMinor(
                    minimumJourneyPositions[g2.data.Code])
                    ? -1
                    : 1);
            }

            // Then sort minigames by their first variation
            outputList.Sort((g1, g2) => SortMiniGames(minimumJourneyPositions, g1, g2));

            return outputList;
        }

        public static int SortMiniGames(Dictionary<MiniGameCode, JourneyPosition> minimumJourneyPositions, MainMiniGame g1, MainMiniGame g2)
        {
            // MiniGames are sorted based on minimum play session
            var minPos1 = minimumJourneyPositions[g1.GetFirstVariationMiniGameCode()];
            var minPos2 = minimumJourneyPositions[g2.GetFirstVariationMiniGameCode()];

            if (minPos1.IsMinor(minPos2)) return -1;
            if (minPos2.IsMinor(minPos1)) return 1;

            // Check play session order
            var sharedPlaySessionData = AppManager.I.DB.GetPlaySessionDataById(minPos1.ToStringId());
            int ret = 0;
            switch (sharedPlaySessionData.Order) {
                case PlaySessionDataOrder.Random:
                    // No specific sorting
                    ret = 0;
                    break; ;
                case PlaySessionDataOrder.Sequence:
                    // In case of a Sequence PS, two minigames with the same minimum play session are sorted based on the sequence order
                    var miniGameInPlaySession1 = sharedPlaySessionData.Minigames.ToList().Find(x => x.MiniGameCode == g1.GetFirstVariationMiniGameCode());
                    var miniGameInPlaySession2 = sharedPlaySessionData.Minigames.ToList().Find(x => x.MiniGameCode == g2.GetFirstVariationMiniGameCode());
                    ret = miniGameInPlaySession1.Weight - miniGameInPlaySession2.Weight;
                    break;
            }
            return ret;
        }
    }
}