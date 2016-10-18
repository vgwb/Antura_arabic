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

            data.Id = ToString(dict["Id"]);
            data.English = ToString(dict["English"]);
            data.Arabic = ToString(dict["Arabic"]);

            return data;
        }
    }
}
