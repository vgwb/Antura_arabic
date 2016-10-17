using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class MiniGameParser : DataParser<MiniGameData, MiniGameTable>
    {
        override protected MiniGameData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new MiniGameData();

            data.Id = (string)(dict["Id"]);
            data.Variation = (string)(dict["Variation"]);
            data.Status = (string)(dict["Status"]); // TODO validate
            data.Parent = (string)(dict["Parent"]); // TODO validate
            data.Description = (string)(dict["Description"]);
            data.Title_En = (string)(dict["Title_En"]);
            data.Title_Ar = (string)(dict["Title_Ar"]);
            data.Scene = (string)(dict["Scene"]);
            data.TitleNew = (string)(dict["TitleNew"]);
            data.Team = (string)(dict["Team"]);

            data.Available = data.Status == "active";

            return data;
        }

        /* TODO: use this logic for validation of Status/Parent/Variation
        private bool ValidateMiniGame(MiniGameData data)
         {
             // Validate Id
             try
             {
                 MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Id);
             }
             catch (System.ArgumentException)
             {
                 Debug.LogError("MiniGameData (ID ?): " + "field Id is '" + data.Id + "', not available in the enum values.");
                 return false;
             }

             // Validate Parent
             try
             {
                 if (data.Parent != "")
                 {
                     MiniGameCode parsed_enum = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Parent);
                 }
             }
             catch (System.ArgumentException)
             {
                 Debug.LogError("MiniGameData (ID " + data.GetID() + "): " + "field Parent is '" + data.Parent + "', not available in the enum values.");
                 return false;
             }

             // Set derived values too
             data.Available = data.Status == "active";
             data.MiniGameCode = (MiniGameCode)System.Enum.Parse(typeof(MiniGameCode), data.Id);
             return true;
         }*/

    }
}
