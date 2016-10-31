using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class PhraseParser : DataParser<PhraseData, PhraseTable>
    {
        override protected PhraseData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new PhraseData();

            data.Id = ToString(dict["Id"]);
            data.English = ToString(dict["English"]);
            data.Arabic = ToString(dict["Arabic"]);
            data.Linked = ToString(dict["Linked"]);

            return data;
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
        }

        protected override void FinalValidation(PhraseTable table)
        {
            // Linked is validated with a final validation step, since it is based on this same table
            foreach(var phraseData in table.GetValuesTyped())
            {
                if (table.GetValue(phraseData.Linked) == null)
                {
                    LogValidation(phraseData, "Cannot find id of PhraseData for Linked value " + phraseData.Linked + " (found in phrase " + phraseData.Id + ")");
                }
            }

        }

    }
}
