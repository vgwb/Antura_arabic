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

            data.Id = ToString(dict["Id"]);
            data.Variation = ToString(dict["Variation"]);

            ValidateStatus(data, dict["Status"]);
            data.Status = ToString(dict["Status"]);

            ValidateParent(data, dict["Parent"]);
            data.Parent = ToString(dict["Parent"]);

            data.Description = ToString(dict["Description"]);
            data.Title_En = ToString(dict["Title_En"]);
            data.Title_Ar = ToString(dict["Title_Ar"]);
            data.Scene = ToString(dict["Scene"]);
            data.TitleNew = ToString(dict["TitleNew"]);
            data.Team = ToString(dict["Team"]);

            //data.MiniGameCode = ParseEnum<MiniGameCode>(data, dict["Id"]);
            data.Available = data.Status == "active";

            return data;
        }

        private void ValidateParent(MiniGameData data, object json_object)
        {
            if ((string)json_object != "") // Empty is fine
            {    
                // We try to parse the parent as a MiniGameCode enum, triggering validation if this cannot be done.
                ParseEnum<MiniGameCode>(data, json_object);
            }
        }
        private void ValidateStatus(MiniGameData data, object json_object)
        {
            // Status must be one of a given list
            string target_string = ToString(json_object);
            List<string> correct_strings = new List<string> { "", "dev", "standby", "active"};
            if (!correct_strings.Contains(target_string))
            {
                LogValidation(data, "Status value is not among the avaliable ones (" + target_string + " found)");
            }
        }

    }
}
