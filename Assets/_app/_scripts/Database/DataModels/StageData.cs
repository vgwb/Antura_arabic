using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class StageData : IData
    {
        public string Id { get; set; }
        public string Title_En { get; set; }
        public string Title_Ar { get; set; }
        public string Description { get; set; }

        public string GetId()
        {
            return Id;
        }
    }
}