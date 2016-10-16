//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
	[System.Serializable]
	public class LanguagesRow : IGoogle2uRow
	{
		public string _Localization_ID;
		public string _Name;
		public bool _Enabled;
		public LanguagesRow(string __STRING_ID, string __Localization_ID, string __Name, string __Enabled) 
		{
			_Localization_ID = __Localization_ID.Trim();
			_Name = __Name.Trim();
			{
			bool res;
				if(bool.TryParse(__Enabled, out res))
					_Enabled = res;
				else
					Debug.LogError("Failed To Convert _Enabled string: "+ __Enabled +" to bool");
			}
		}

		public int Length { get { return 3; } }

		public string this[int i]
		{
		    get
		    {
		        return GetStringDataByIndex(i);
		    }
		}

		public string GetStringDataByIndex( int index )
		{
			string ret = System.String.Empty;
			switch( index )
			{
				case 0:
					ret = _Localization_ID.ToString();
					break;
				case 1:
					ret = _Name.ToString();
					break;
				case 2:
					ret = _Enabled.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "Localization_ID":
					ret = _Localization_ID.ToString();
					break;
				case "Name":
					ret = _Name.ToString();
					break;
				case "Enabled":
					ret = _Enabled.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "Localization_ID" + " : " + _Localization_ID.ToString() + "} ";
			ret += "{" + "Name" + " : " + _Name.ToString() + "} ";
			ret += "{" + "Enabled" + " : " + _Enabled.ToString() + "} ";
			return ret;
		}
	}
	public sealed class Languages : IGoogle2uDB
	{
		public enum rowIds {
			EN, AR, IT
		};
		public string [] rowNames = {
			"EN", "AR", "IT"
		};
		public System.Collections.Generic.List<LanguagesRow> Rows = new System.Collections.Generic.List<LanguagesRow>();

		public static Languages Instance
		{
			get { return NestedLanguages.instance; }
		}

		private class NestedLanguages
		{
			static NestedLanguages() { }
			internal static readonly Languages instance = new Languages();
		}

		private Languages()
		{
			Rows.Add( new LanguagesRow("EN", "EN", "English", "TRUE"));
			Rows.Add( new LanguagesRow("AR", "AR", "Arabic", "TRUE"));
			Rows.Add( new LanguagesRow("IT", "IT", "Italian", "FALSE"));
		}
		public IGoogle2uRow GetGenRow(string in_RowString)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}
		public IGoogle2uRow GetGenRow(rowIds in_RowID)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LanguagesRow GetRow(rowIds in_RowID)
		{
			LanguagesRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LanguagesRow GetRow(string in_RowString)
		{
			LanguagesRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
