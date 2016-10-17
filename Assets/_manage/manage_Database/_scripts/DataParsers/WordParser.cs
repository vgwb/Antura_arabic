using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public class WordParser : DataParser<WordData, WordTable>
    {
        override protected WordData CreateData(Dictionary<string, object> dict, Database db)
        {
            var data = new WordData();

            data.Id = (string)(dict["Id"]);
            data.Kind = (string)(dict["Kind"]);
            data.Category = (string)(dict["Category"]);
            data.English = (string)(dict["English"]);
            data.Arabic = (string)(dict["Arabic"]);

            // TODO: should instead be an array of ID, but we need to solve the european-to-arabic matching!
            data.Letters = new string[] { (string)(dict["Letters"]) }; // ParseIDArray<LetterData, LetterTable>(data, (string)(dict["Letters"]), db.letterTable);

            data.Transliteration = (string)(dict["Transliteration"]);
            // data.DifficultyLevel = (string)(dict["Difficulty"]); TODO: rename to DIFFICULTY instead
            data.Group = (string)(dict["Group"]);

            data.NumberOfLetters = 0; // TODO!

            return data;
        }
    }
}
