using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class PlaySessionParser : DataParser<PlaySessionData, PlaySessionTable>
    {
        override protected PlaySessionData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new PlaySessionData();

            data.Stage = ToInt(dict["Stage"]);
            data.LearningBlock = ToInt(dict["LearningBlock"]);
            data.PlaySession = ToInt(dict["PlaySession"]);
            data.Id = data.Stage + "." + data.LearningBlock + "." + data.PlaySession;
            data.Description = ToString(dict["Description"]);
            data.IntroArabic = ToString(dict["IntroArabic"]);
            data.Reward = ParseID<RewardData, RewardTable>(data, (string)dict["Reward"], db.GetRewardTable());

            data.Type = ToString(dict["Type"]);
            data.Focus = ParseEnum<DidacticalFocus>(data, (string)dict["Focus"]);

            data.Letters = ParseIDArray<LetterData, LetterTable>(data, (string)dict["Letters"], db.GetLetterTable());
            data.Words = ParseIDArray<WordData, WordTable>(data, (string)dict["Words"], db.GetWordTable());
            data.Words_previous = ParseIDArray<WordData, WordTable>(data, (string)dict["Words_previous"], db.GetWordTable());
            data.Phrases = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases"], db.GetPhraseTable());
            data.Phrases_previous = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases_previous"], db.GetPhraseTable());

            data.Minigames = CustomParseMinigames(data, dict, db.GetMiniGameTable());

            return data;
        }

        public List<MiniGameInPlaySession> CustomParseMinigames(PlaySessionData data, Dictionary<string, object> dict, MiniGameTable table)
        {
            var list = new List<MiniGameInPlaySession>();
            for (int enum_i = 0; enum_i < System.Enum.GetValues(typeof(MiniGameCode)).Length; enum_i++) {
                var enum_string = ((MiniGameCode)enum_i).ToString();
                if (enum_string == "") continue; // this means that the enum does not exist
                if (enum_string == "0") continue; // 0 does not exist in the table

                if (!dict.ContainsKey(enum_string)) {
                    // TODO: what to do if the enum is not found in the dict? tell once?
                    //Debug.LogWarning(data.GetType().ToString() + " could not find minigame column for " + enum_string);
                    continue;
                }

                var minigameStruct = new MiniGameInPlaySession();
                minigameStruct.MiniGameCode = (MiniGameCode)enum_i;
                minigameStruct.Weight = ToInt(dict[enum_string]);
                if (minigameStruct.Weight == 0) {
                    // Skip adding if the weight is zero
                    continue;
                }

                list.Add(minigameStruct);
            }
            return list;
        }

    }
}
