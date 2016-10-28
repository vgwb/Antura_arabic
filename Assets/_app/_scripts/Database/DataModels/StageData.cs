using System;
using UnityEngine;

namespace EA4S.Db
{
    [Serializable]
    public class StageData : IData
    {
        public string Id;
        public string Title_En;
        public string Title_Ar;
        public string Description;

        public override string ToString()
        {
            return Id + ": " + Title_En;
        }

        public string GetId()
        {
            return Id;
        }
    }
}