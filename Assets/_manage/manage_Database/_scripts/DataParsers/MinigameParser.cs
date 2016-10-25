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
            data.Type = ParseEnum<MiniGameType>(data, dict["Type"]);
            data.Description = ToString(dict["Description"]);
            data.IntroArabic = ToString(dict["IntroArabic"]);
            data.Title_En = ToString(dict["Title_En"]);
            data.Title_Ar = ToString(dict["Title_Ar"]);
            data.Scene = ToString(dict["Scene"]);

            data.Available = ToString(dict["Status"]) == "active";

            return data;
        }

        //// DEPRECATED: no more parents
        //private void ValidateParent(MiniGameData data, object json_object)
        //{
        //    if ((string)json_object != "") // Empty is fine
        //    {
        //        // We try to parse the parent as a MiniGameCode enum, triggering validation if this cannot be done.
        //        ParseEnum<MiniGameCode>(data, json_object);
        //    }
        //}

        //// DEPRECATED: no more status
        //private void ValidateStatus(MiniGameData data, object json_object)
        //{
        //    // Status must be one of a given list
        //    string target_string = ToString(json_object);
        //    List<string> correct_strings = new List<string> { "", "dev", "standby", "active" };
        //    if (!correct_strings.Contains(target_string)) {
        //        LogValidation(data, "Status value is not among the avaliable ones (" + target_string + " found)");
        //    }
        //}

    }
}
