using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class LocalizationParser : DataParser<LocalizationData, LocalizationTable>
    {
        override protected LocalizationData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new LocalizationData();

            data.Id = (string)(dict["Id"]);
            data.Character = (string)(dict["Character"]);
            data.Context = (string)(dict["Context"]);
            data.English = (string)(dict["English"]);
            data.Arabic = (string)(dict["Arabic"]);
            data.AudioFile = (string)(dict["AudioFile"]);

            return data;
        }
    }
}
