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
            data.Kind = ToString(dict["Kind"]);
            data.Type = ToString(dict["Type"]);
            data.Notes = ToString(dict["Notes"]);
            data.SunMoon = ToString(dict["SunMoon"]);
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
    }
}
