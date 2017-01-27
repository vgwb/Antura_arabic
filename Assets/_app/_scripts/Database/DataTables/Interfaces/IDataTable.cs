using System.Collections.Generic;

namespace EA4S.Database
{
    /// <summary>
    /// Interface for a table related to a specific IData inside the static database.
    /// </summary>
    public interface IDataTable
    {
        List<IData> GetList();
        IEnumerable<IData> GetValues();
        IData GetValue(string id);
        int GetDataCount();
    }
}