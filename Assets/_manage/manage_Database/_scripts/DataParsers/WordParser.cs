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
            data.Kind = ParseEnum<WordDataKind>(data, dict["Kind"]);
            data.Category = ParseEnum<WordDataCategory>(data, dict["Category"]);

            data.Form = CustomParseForm(data, dict["Form"]);
            data.Article = ParseEnum<WordDataArticle>(data, dict["Article"]);

            data.Arabic = ToString(dict["Arabic"]);

            // TODO: should instead be an array of ID, but we need to solve the european-to-arabic matching!
            data.Letters = new string[] { (string)(dict["Letters"]) }; // ParseIDArray<LetterData, LetterTable>(data, (string)(dict["Letters"]), db.letterTable);

            data.Difficulty = ToInt(dict["Difficulty"]);
            data.Drawing = ToInt(dict["Drawing"]);

            return data;
        }

        private WordDataForm CustomParseForm(WordData data, object enum_object)
        {
            if (ToString(enum_object) == "")
            {
                return WordDataForm.Singular;
            }
            else
            {
                return ParseEnum<WordDataForm>(data, enum_object);
            }
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            ExtractEnum(rowdicts_list, "Kind");
            ExtractEnum(rowdicts_list, "Category", addNoneValue: true);
            ExtractEnum(rowdicts_list, "Form");
            ExtractEnum(rowdicts_list, "Article", addNoneValue: true);
        }
    }
}
