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


        //public IGoogle2uRow GetGenRow(rowIds in_RowID)
        //{
        //	IGoogle2uRow ret = null;
        //	try
        //	{
        //		ret = Rows[(int)in_RowID];
        //	}
        //	catch( System.Collections.Generic.KeyNotFoundException ex )
        //	{
        //		Debug.LogError( in_RowID + " not found: " + ex.Message );
        //	}
        //	return ret;
        //}
        //public LocalizationDataRow GetRow(rowIds in_RowID)
        //{
        //	LocalizationDataRow ret = null;
        //	try
        //	{
        //		ret = Rows[(int)in_RowID];
        //	}
        //	catch( System.Collections.Generic.KeyNotFoundException ex )
        //	{
        //		Debug.LogError( in_RowID + " not found: " + ex.Message );
        //	}
        //	return ret;
        //}
        //public LocalizationDataRow GetRow(string in_RowString)
        //{
        //	LocalizationDataRow ret = null;
        //	try
        //	{
        //		ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
        //	}
        //	catch(System.ArgumentException) {
        //		Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
        //	}
        //	return ret;
        //}

    }

}
