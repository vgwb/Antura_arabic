// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    #endregion

    public class Google2uMimeType
    {
        private static readonly IDictionary<string, string> _mappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {".csv", "text/csv"},
                {".ods", "application/oleobject"},
                {".tsv", "text/tab-separated-values"},
                {".txt", "text/plain"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"}
            };

        public static string GetMimeType(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("Null File Path");
            }

            var extension = filePath.Substring(filePath.LastIndexOf('.'));

            string mime;
            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
    }
}