using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class LetterParser : DataParser<LetterData, LetterTable>
    {
        override protected LetterData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new LetterData();

            data.Id = (string)(dict["Id"]);
            data.Number = ToI(dict["Number"]);
            data.Title = (string)(dict["Title"]);
            data.Kind = (string)(dict["Kind"]);
            data.Type = (string)(dict["Type"]);
            data.Notes = (string)(dict["Notes"]);
            data.SunMoon = (string)(dict["SunMoon"]);
            data.Sound = (string)(dict["Sound"]);
            data.Isolated = (string)(dict["Isolated"]);
            data.Initial = (string)(dict["Initial"]);
            data.Medial = (string)(dict["Medial"]);
            data.Final = (string)(dict["Final"]);
            data.Isolated_Unicode = (string)(dict["Isolated_Unicode"]);
            data.Initial_Unicode = (string)(dict["Initial_Unicode"]);
            data.Medial_Unicode = (string)(dict["Medial_Unicode"]);
            data.Final_Unicode = (string)(dict["Final_Unicode"]);

            return data;
        }
    }
}
