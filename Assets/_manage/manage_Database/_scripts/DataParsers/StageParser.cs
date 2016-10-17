using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class StageParser : DataParser<StageData, StageTable>
    {
        override protected StageData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new StageData();

            data.Id = (string)(dict["Id"]);
            data.Title_En = (string)(dict["Title_En"]);
            data.Title_Ar = (string)(dict["Title_Ar"]);
            data.Description = (string)(dict["Description"]);

            return data;
        }
    }
}
