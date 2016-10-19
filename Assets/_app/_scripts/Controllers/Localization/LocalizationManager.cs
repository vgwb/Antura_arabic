using UnityEngine;
using System.Globalization;

namespace EA4S
{
    public class LocalizationManager
    {

        public static Db.LocalizationData GetLocalizationData(string id)
        {
            return AppManager.Instance.DB.GetLocalizationDataById(id);
        }

    }

}
