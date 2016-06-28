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
	public class LocalizationDataRow : IGoogle2uRow
	{
		public string _EN;
		public string _IT;
		public LocalizationDataRow(string __STRING_ID, string __EN, string __IT) 
		{
			_EN = __EN.Trim();
			_IT = __IT.Trim();
		}

		public int Length { get { return 2; } }

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
					ret = _EN.ToString();
					break;
				case 1:
					ret = _IT.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "EN":
					ret = _EN.ToString();
					break;
				case "IT":
					ret = _IT.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "EN" + " : " + _EN.ToString() + "} ";
			ret += "{" + "IT" + " : " + _IT.ToString() + "} ";
			return ret;
		}
	}
	public sealed class LocalizationData : IGoogle2uDB
	{
		public enum rowIds {
			IGNORE, select_profile, select_your_profile, profile_name, select, delete, delete_all, create, close, progression_rate, back_to_home, pause, ignore, main_map
		};
		public string [] rowNames = {
			"IGNORE", "select_profile", "select_your_profile", "profile_name", "select", "delete", "delete_all", "create", "close", "progression_rate", "back_to_home", "pause", "ignore", "main_map"
		};
		public System.Collections.Generic.List<LocalizationDataRow> Rows = new System.Collections.Generic.List<LocalizationDataRow>();

		public static LocalizationData Instance
		{
			get { return NestedLocalizationData.instance; }
		}

		private class NestedLocalizationData
		{
			static NestedLocalizationData() { }
			internal static readonly LocalizationData instance = new LocalizationData();
		}

		private LocalizationData()
		{
			Rows.Add( new LocalizationDataRow("IGNORE", "PROFILE SECTION", ""));
			Rows.Add( new LocalizationDataRow("select_profile", "Select Profile", "Seleziona Profilo"));
			Rows.Add( new LocalizationDataRow("select_your_profile", "Select your profile", "Seleziona il tuo profilo"));
			Rows.Add( new LocalizationDataRow("profile_name", "Profile Name", "Nome Profilo"));
			Rows.Add( new LocalizationDataRow("select", "Select", "Seleziona"));
			Rows.Add( new LocalizationDataRow("delete", "Delete", "Cancella"));
			Rows.Add( new LocalizationDataRow("delete_all", "Delete All", "Cancella Tutto"));
			Rows.Add( new LocalizationDataRow("create", "Create", "Crea"));
			Rows.Add( new LocalizationDataRow("close", "Close", "Chiudi"));
			Rows.Add( new LocalizationDataRow("progression_rate", "Level", "Livello"));
			Rows.Add( new LocalizationDataRow("back_to_home", "Back To Home", "Torna alla Home"));
			Rows.Add( new LocalizationDataRow("pause", "Pause", "Pausa"));
			Rows.Add( new LocalizationDataRow("ignore", "", ""));
			Rows.Add( new LocalizationDataRow("main_map", "Main Map", "Mappa Principale"));
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
		public LocalizationDataRow GetRow(rowIds in_RowID)
		{
			LocalizationDataRow ret = null;
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
		public LocalizationDataRow GetRow(string in_RowString)
		{
			LocalizationDataRow ret = null;
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
