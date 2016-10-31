using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class LetterParser : DataParser<LetterData, LetterTable>
    {
        override protected LetterData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new LetterData();

            data.Id = ToString(dict["Id"]);
            data.Number = ToInt(dict["Number"]);
            data.Title = ToString(dict["Title"]);
            data.Kind = ParseEnum<LetterDataKind>(data, dict["Kind"]);
            data.BaseLetter = ToString(dict["BaseLetter"]);
            data.Symbol = ToString(dict["Symbol"]);
            data.Type = ParseEnum<LetterDataType>(data, dict["Type"]);
            data.Tag = ToString(dict["Tag"]);
            data.Notes = ToString(dict["Notes"]);
            data.SunMoon = ParseEnum<LetterDataSunMoon>(data, dict["SunMoon"]);
            data.Sound = ToString(dict["Sound"]);
            data.Isolated = ToString(dict["Isolated"]);
            data.Initial = ToString(dict["Initial"]);
            data.Medial = ToString(dict["Medial"]);
            data.Final = ToString(dict["Final"]);
            data.Isolated_Unicode = ToString(dict["Isolated_Unicode"]);
            data.Initial_Unicode = ToString(dict["Initial_Unicode"]);
            data.Medial_Unicode = ToString(dict["Medial_Unicode"]);
            data.Final_Unicode = ToString(dict["Final_Unicode"]);

            return data;
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            ExtractEnum(rowdicts_list, "Kind");
            ExtractEnum(rowdicts_list, "Type", addNoneValue:true);
            ExtractEnum(rowdicts_list, "SunMoon", addNoneValue:true);
        }

        protected override void FinalValidation(LetterTable table, Database db)
        {
            // Fields 'BaseLetter' and 'Symbol' are validated with a final validation step, since they are based on this same table
            foreach (var data in table.GetValuesTyped())
            {
                if (data.BaseLetter != "" && table.GetValue(data.BaseLetter) == null)
                {
                    LogValidation(data, "Cannot find id of LetterData for BaseLetter value " + data.BaseLetter + " (found in letter " + data.Id + ")");
                }

                if (data.Symbol != "" && table.GetValue(data.Symbol) == null)
                {
                    LogValidation(data, "Cannot find id of LetterData for Symbol value " + data.Symbol + " (found in letter " + data.Id + ")");
                }
            }
        }

    }
}
