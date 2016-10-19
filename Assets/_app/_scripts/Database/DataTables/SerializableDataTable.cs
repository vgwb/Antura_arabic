using System.Collections.Generic;

namespace EA4S.Db
{
    public class SerializableDataTable<K> : SerializableDictionary<string, K>, IDataTable where K : IData
    {
        public List<IData> GetList()
        {
            return new List<IData>(this.GetValues());
        }

        public IEnumerable<IData> GetValues()
        {
            foreach (var value in this.Values)
            {
                yield return value;
            }
        }

        public IData GetValue(string id)
        {
            if (!this.ContainsKey(id))
                return null;
            return this[id];
        }

        public int GetDataCount()
        {
            return this.Count;
        }
    }
}

