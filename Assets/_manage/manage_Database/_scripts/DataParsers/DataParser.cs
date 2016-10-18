using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Management
{
    public abstract class DataParser<D, Dtable> where D : IData where Dtable : IDictionary<string, D>
    {
        public void Parse(string json, Database db, Dtable table)
        {
            table.Clear();  // we re-generate the whole table

            var list = Json.Deserialize(json) as List<object>;
            foreach (var row in list) {
                var dict = row as Dictionary<string, object>;
                var data = CreateData(dict, db);

                if (table.ContainsKey(data.GetId())) {
                    LogValidation(data, "found multiple ID.");
                    continue;
                }

                table.Add(data.GetId(), data);
            }
        }

        protected abstract D CreateData(Dictionary<string, object> dict, Database db);

        protected T ParseEnum<T>(D data, object enum_object)
        {
            string enum_string = ToString(enum_object);
            T parsed_enum = default(T);
            try {
                parsed_enum = (T)System.Enum.Parse(typeof(T), enum_string);
            } catch {
                LogValidation(data, "field valued '" + enum_string + "', not available as an enum value for type " + typeof(T).ToString() + ".");
            }
            return parsed_enum;
        }

        protected string[] ParseIDArray<OtherD, OtherDTable>(D data, string array_string, OtherDTable table) where OtherDTable : IDictionary<string, OtherD> where OtherD : IData
        {
            if (table == null) {
                LogValidation(data, "Table of type " + typeof(OtherDTable).Name + " was null!");
            }

            var array = array_string.Split(',');
            if (array_string == "") return new string[0];  // skip if empty (could happen if the string was empty)    
            foreach (var vi in array) {
                var v = vi.Trim(); // remove spaces
                if (!table.ContainsKey(v)) {
                    LogValidation(data, "could not find a reference inside " + typeof(OtherDTable).Name + " for ID " + v);
                }
            }
            return array;
        }

        protected void LogValidation(D data, string msg)
        {
            Debug.LogWarning(data.GetType().ToString() + " (ID " + data.GetId() + "): " + msg);
        }

        #region Conversions
        protected string ToString(object _input)
        {
            return ((string)_input).Trim();
        }

        protected int ToInt(object _input)
        {
            // Force empty to 0
            if ((string)_input == "")
                return 0;

            int target_int = 0;
            if (!int.TryParse((string)_input, out target_int)) {
                Debug.LogError("Object " + (string)_input + " should be an int.");
            }
            return target_int;
        }
        #endregion

    }

}
