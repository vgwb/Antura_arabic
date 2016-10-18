using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public class WordParser : DataParser<WordData, WordTable>
    {
        override protected WordData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new WordData();

            data.Id = ToString(dict["Id"]);
            data.Kind = ToString(dict["Kind"]);
            data.Category = ToString(dict["Category"]);
            data.English = ToString(dict["English"]);
            data.Arabic = ToString(dict["Arabic"]);

            // TODO: should instead be an array of ID, but we need to solve the european-to-arabic matching!
            data.Letters = new string[] { (string)(dict["Letters"]) }; // ParseIDArray<LetterData, LetterTable>(data, (string)(dict["Letters"]), db.letterTable);

            data.Transliteration = ToString(dict["Transliteration"]);
            data.Difficulty = ToInt(dict["Difficulty"]);
            data.Group = ToString(dict["Group"]);
            data.Drawing = ToInt(dict["Drawing"]);

            return data;
        }
    }
}
