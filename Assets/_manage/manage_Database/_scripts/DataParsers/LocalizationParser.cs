using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class LocalizationParser : DataParser<LocalizationData, LocalizationTable>
    {
        override protected LocalizationData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new LocalizationData();

            data.Id = ToString(dict["Id"]);
            data.Character = ToString(dict["Character"]);
            data.Area = ToString(dict["Area"]);
            data.When = ToString(dict["When"]);
            data.Context = ToString(dict["Context"]);
            data.English = ToString(dict["English"]);
            data.Arabic = ToString(dict["Arabic"]);
            data.AudioFile = ToString(dict["AudioFile"]);

            return data;
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            ExtractEnum(rowdicts_list, "Id", addNoneValue: true);
        }
    }
}
