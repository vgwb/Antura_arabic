using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class AssessmentParser : DataParser<AssessmentData, AssessmentTable>
    {
        override protected AssessmentData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new AssessmentData();

            data.Id = ParseEnum<AssessmentType>(data, dict["Id"]);
            data.Title = ToString(dict["Title"]);
            data.Description = ToString(dict["Description"]);

            return data;
        }
    }
}
