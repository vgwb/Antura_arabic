using System;

namespace EA4S.Db
{
    /// <summary>
    /// Data defining a Localization key-value pair.
    /// </summary>
    [Serializable]
    public class LocalizationData : IData
    {
        public string Id;
        public string Character;
        public string Area;
        public string When;
        public string Context;
        public string English;
        public string Arabic;
        public string AudioFile;

        public override string ToString()
        {
            return Id + ": " + English;
        }

        public string GetId()
        {
            return Id;
        }
    }
}