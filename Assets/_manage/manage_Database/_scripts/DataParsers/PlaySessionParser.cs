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
            data.Focus = DidacticalFocus.Letters; // TODO: (sometimes it is empty!) ParseEnum<DidacticalFocus>(data, (string)dict["Focus"]);

            data.Letters = ParseIDArray<LetterData, LetterTable>(data, (string)dict["Letters"], db.GetLetterTable());
            data.Words = ParseIDArray<WordData, WordTable>(data, (string)dict["Words"], db.GetWordTable());
            data.Words_previous = ParseIDArray<WordData, WordTable>(data, (string)dict["Words_previous"], db.GetWordTable());
            data.Phrases = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases"], db.GetPhraseTable());
            data.Phrases_previous = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases_previous"], db.GetPhraseTable());

            data.AssessmentType = AssessmentType.Alphabet; // TODO: (sometimes it is empty!) ParseEnum<AssessmentType>(data, (string)dict["AssesmentType"]);
            data.AssessmentData = ToString(dict["AssessmentData"]);

            data.Minigames = CustomParseMinigames(data, dict, db.GetMiniGameTable());

            return data;
        }

        public List<MiniGameInPlaySession> CustomParseMinigames(PlaySessionData data, Dictionary<string, object> dict, MiniGameTable table) 
        {
            var list = new List<MiniGameInPlaySession>();
            for(int enum_i=0; enum_i < System.Enum.GetValues(typeof(MiniGameCode)).Length; enum_i++)
            {  
                var enum_string = ((MiniGameCode)enum_i).ToString();
                if (enum_string == "") continue; // this means that the enum does not exist
                if (enum_string == "0") continue; // 0 does not exist in the table

                if (!dict.ContainsKey(enum_string))
                {
                    // TODO: what to do if the enum is not found in the dict? tell once?
                    //Debug.LogWarning(data.GetType().ToString() + " could not find minigame column for " + enum_string);
                    continue;
                }

                var minigameStruct = new MiniGameInPlaySession();
                minigameStruct.MiniGame_Id = enum_string;
                minigameStruct.Weight = (string)dict[enum_string] == "" ? 0 : ToInt(dict[enum_string]);
                list.Add(minigameStruct);
            }
            return list;
        }
        
    }
}
