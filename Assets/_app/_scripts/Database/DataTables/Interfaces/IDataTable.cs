using System.Collections;
using System.Collections.Generic;

namespace EA4S.Db
{
    public interface IDataTable
    {
        List<IData> GetList();
        IEnumerable<IData> GetValues();
        IData GetValue(string id);
        int GetDataCount();
    }
}