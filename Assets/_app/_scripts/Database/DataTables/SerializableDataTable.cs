using System.Collections.Generic;

namespace EA4S.Db
{
    public class SerializableDataTable<K> : List<K>, IDataTable where K : IData
    {
        public List<IData> GetList()
        {
            return new List<IData>(this.GetValues());
        }

        public IEnumerable<IData> GetValues()
        {
            foreach (var value in this)
            {
                yield return value;
            }
        }

        public IEnumerable<K> GetValuesTyped()
        {
            foreach (var value in this)
            {
                yield return value;
            }
        }

        public IData GetValue(string id)
        {
            return Find(x => x.GetId() == id);
        }

        public int GetDataCount()
        {
            return this.Count;
        }
    }
}

