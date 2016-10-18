using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class RewardParser : DataParser<RewardData, RewardTable>
    {
        override protected RewardData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new RewardData();

            data.Id = ToString(dict["Id"]);
            data.Title = ToString(dict["Title"]);
            data.Category = ParseEnum<RewardCategory>(data, dict["Category"]);

            return data;
        }
    }
}
