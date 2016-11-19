using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class MiniGameParser : DataParser<MiniGameData, MiniGameTable>
    {
        override protected MiniGameData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new MiniGameData();

            data.Code = ParseEnum<MiniGameCode>(data, dict["Id"]);
            data.Main = ToString(dict["Main"]);
            data.Variation = ToString(dict["Variation"]);
            data.Type = ParseEnum<MiniGameDataType>(data, dict["Type"]);
            data.Description = ToString(dict["Description"]);
            data.Title_En = ToString(dict["Title_En"]);
            data.Title_Ar = ToString(dict["Title_Ar"]);
            data.Scene = ToString(dict["Scene"]);
            data.Available = ToString(dict["Status"]) == "active";

            return data;
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            // TODO: should we generate also the MiniGameCode? Could be useful, but it could mess with the current inspector values. 
            // ExtractEnum(rowdicts_list, "Id", customEnumName: "MiniGameCode");
            ExtractEnum(rowdicts_list, "Type");
        }

    }
}
