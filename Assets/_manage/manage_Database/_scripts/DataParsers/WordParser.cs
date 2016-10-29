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
            data.Kind = ParseEnum<WordKind>(data, dict["Kind"]);
            data.Category = ParseEnum<WordCategory>(data, dict["Category"]);
            data.Arabic = ToString(dict["Arabic"]);

            // TODO: should instead be an array of ID, but we need to solve the european-to-arabic matching!
            data.Letters = new string[] { (string)(dict["Letters"]) }; // ParseIDArray<LetterData, LetterTable>(data, (string)(dict["Letters"]), db.letterTable);

            data.Transliteration = ToString(dict["Transliteration"]);
            data.Difficulty = ToInt(dict["Difficulty"]);
            data.Drawing = ToInt(dict["Drawing"]);

            return data;
        }
    }
}
