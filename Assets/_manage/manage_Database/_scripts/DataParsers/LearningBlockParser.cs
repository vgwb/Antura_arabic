using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class LearningBlockParser : DataParser<LearningBlockData, LearningBlockTable>
    {
        override protected LearningBlockData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new LearningBlockData();

            data.Stage = ToInt(dict["Stage"]);
            data.LearningBlock = ToInt(dict["LearningBlock"]);
            data.Id = data.Stage + "." + data.LearningBlock;

            data.Description = ToString(dict["Description"]);
            data.Title = ToString(dict["Title"]);
            data.IntroArabic = ToString(dict["IntroArabic"]);
            data.Reward = ParseID<RewardData, RewardTable>(data, (string)dict["Reward"], db.GetRewardTable());

            data.Focus = ParseEnum<LearningBlockDataFocus>(data, (string)dict["Focus"]);

            data.Letters = ParseIDArray<LetterData, LetterTable>(data, (string)dict["Letters"], db.GetLetterTable());
            data.Words = ParseIDArray<WordData, WordTable>(data, (string)dict["Words"], db.GetWordTable());
            data.Words_previous = ParseIDArray<WordData, WordTable>(data, (string)dict["Words_previous"], db.GetWordTable());
            data.Phrases = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases"], db.GetPhraseTable());
            data.Phrases_previous = ParseIDArray<PhraseData, PhraseTable>(data, (string)dict["Phrases_previous"], db.GetPhraseTable());

            data.AssessmentType = ToString(dict["AssessmentType"]);
            data.AssessmentData = ToString(dict["AssessmentData"]);

            return data;
        }

        protected override bool CanHaveSameKeyMultipleTimes
        {
            get
            {
                return true;
            }
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            ExtractEnum(rowdicts_list, "Focus", addNoneValue:true);
        }

        protected override void FinalValidation(LearningBlockTable table, Database db)
        {
            // Field 'NumberOfPlaySessions' can be found only at the end
            var playSessionsList = new List<PlaySessionData>(db.GetPlaySessionTable().GetValuesTyped());
            foreach (var data in table.GetValuesTyped())
            {
                int nPlaySessions = playSessionsList.FindAll(x => x.Stage == data.Stage && x.LearningBlock == data.LearningBlock).Count;
                data.NumberOfPlaySessions = nPlaySessions;
            }

        }
    }
}
