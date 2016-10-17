using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

namespace EA4S.Db.Loader
{
    public abstract class DataParser<D, Dtable> where D : IData where Dtable : IDictionary<string,D>
    {
        public void Parse(string json, Database db, Dtable table)
        {
            table.Clear();  // we re-generate the whole table

            var list = Json.Deserialize(json) as List<object>;
            foreach (var row in list)
            {
                var dict = row as Dictionary<string, object>;
                var data = CreateData(dict, db);

                if (table.ContainsKey(data.GetID()))
                {
                    LogValidation(data, "found multiple ID.");
                    continue;
                }

                table.Add(data.GetID(), data);
            }
        }

        protected abstract D CreateData(Dictionary<string, object> dict, Database db);

        protected T ParseEnum<T>(D data, string enum_string)
        {
            T parsed_enum = default(T);
            try
            {
                parsed_enum = (T)System.Enum.Parse(typeof(T), enum_string);
            }
            catch
            {
                LogValidation(data, "field valued '" + enum_string + "', not available as an enum value for type " + typeof(T).ToString() + ".");
            }
            return parsed_enum;
        }

        protected string[] ParseIDArray<OtherD, OtherDTable>(D data, string array_string, OtherDTable table) where OtherDTable : IDictionary<string, OtherD> where OtherD : IData
        {
            if (table == null)
            {
                LogValidation(data, "Table of type " + typeof(OtherDTable).Name + " was null!");
            }

            var array = array_string.Split(',');
            if (array_string == "") return new string[0];  // skip if empty (could happen if the string was empty)    
            foreach (var vi in array) 
            {
                var v = vi.Trim(' '); // remove spaces
                if (!table.ContainsKey(v))
                {
                    LogValidation(data, "could not find a reference inside " + typeof(OtherDTable).Name + " for ID " + v);
                }
            }
            return array;
        }

        protected void LogValidation(D data, string msg)
        {
            Debug.LogWarning(data.GetType().ToString() + " (ID " + data.GetID() + "): " + msg);
        }

        #region Conversions
        protected int ToI(object o)
        {
            // Force empty to 0
            if ((string)o == "")
                return 0;

            int target_i = 0;
            if (!int.TryParse((string)o, out target_i))
            {
                Debug.LogError("Object " + (string)o + " should be an int.");
            }
            return target_i;
        }
        #endregion

    }

}
