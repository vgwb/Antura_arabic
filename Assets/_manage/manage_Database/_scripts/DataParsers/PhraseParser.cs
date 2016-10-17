using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class PhraseParser : DataParser<PhraseData, PhraseTable>
    {
        override protected PhraseData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new PhraseData();

            data.Id = (string)(dict["Id"]);
            data.English = (string)(dict["English"]);
            data.Arabic = (string)(dict["Arabic"]);

            return data;
        }
    }
}
