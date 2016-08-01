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
	public class lettersRow : IGoogle2uRow
	{
		public string _number;
		public string _title;
		public string _kind;
		public string _type;
		public string _notes;
		public string _sun_moon;
		public string _sound;
		public string _isolated;
		public string _initial;
		public string _medial;
		public string _final;
		public string _unicode;
		public string _initial_unicode;
		public string _medial_unicode;
		public string _final_unicode;
		public string _level;
		public lettersRow(string __id, string __number, string __title, string __kind, string __type, string __notes, string __sun_moon, string __sound, string __isolated, string __initial, string __medial, string __final, string __unicode, string __initial_unicode, string __medial_unicode, string __final_unicode, string __level) 
		{
			_number = __number.Trim();
			_title = __title.Trim();
			_kind = __kind.Trim();
			_type = __type.Trim();
			_notes = __notes.Trim();
			_sun_moon = __sun_moon.Trim();
			_sound = __sound.Trim();
			_isolated = __isolated.Trim();
			_initial = __initial.Trim();
			_medial = __medial.Trim();
			_final = __final.Trim();
			_unicode = __unicode.Trim();
			_initial_unicode = __initial_unicode.Trim();
			_medial_unicode = __medial_unicode.Trim();
			_final_unicode = __final_unicode.Trim();
			_level = __level.Trim();
		}

		public int Length { get { return 16; } }

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
					ret = _number.ToString();
					break;
				case 1:
					ret = _title.ToString();
					break;
				case 2:
					ret = _kind.ToString();
					break;
				case 3:
					ret = _type.ToString();
					break;
				case 4:
					ret = _notes.ToString();
					break;
				case 5:
					ret = _sun_moon.ToString();
					break;
				case 6:
					ret = _sound.ToString();
					break;
				case 7:
					ret = _isolated.ToString();
					break;
				case 8:
					ret = _initial.ToString();
					break;
				case 9:
					ret = _medial.ToString();
					break;
				case 10:
					ret = _final.ToString();
					break;
				case 11:
					ret = _unicode.ToString();
					break;
				case 12:
					ret = _initial_unicode.ToString();
					break;
				case 13:
					ret = _medial_unicode.ToString();
					break;
				case 14:
					ret = _final_unicode.ToString();
					break;
				case 15:
					ret = _level.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "number":
					ret = _number.ToString();
					break;
				case "title":
					ret = _title.ToString();
					break;
				case "kind":
					ret = _kind.ToString();
					break;
				case "type":
					ret = _type.ToString();
					break;
				case "notes":
					ret = _notes.ToString();
					break;
				case "sun/moon":
					ret = _sun_moon.ToString();
					break;
				case "sound":
					ret = _sound.ToString();
					break;
				case "isolated":
					ret = _isolated.ToString();
					break;
				case "initial":
					ret = _initial.ToString();
					break;
				case "medial":
					ret = _medial.ToString();
					break;
				case "final":
					ret = _final.ToString();
					break;
				case "unicode":
					ret = _unicode.ToString();
					break;
				case "initial-unicode":
					ret = _initial_unicode.ToString();
					break;
				case "medial-unicode":
					ret = _medial_unicode.ToString();
					break;
				case "final-unicode":
					ret = _final_unicode.ToString();
					break;
				case "level":
					ret = _level.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "number" + " : " + _number.ToString() + "} ";
			ret += "{" + "title" + " : " + _title.ToString() + "} ";
			ret += "{" + "kind" + " : " + _kind.ToString() + "} ";
			ret += "{" + "type" + " : " + _type.ToString() + "} ";
			ret += "{" + "notes" + " : " + _notes.ToString() + "} ";
			ret += "{" + "sun/moon" + " : " + _sun_moon.ToString() + "} ";
			ret += "{" + "sound" + " : " + _sound.ToString() + "} ";
			ret += "{" + "isolated" + " : " + _isolated.ToString() + "} ";
			ret += "{" + "initial" + " : " + _initial.ToString() + "} ";
			ret += "{" + "medial" + " : " + _medial.ToString() + "} ";
			ret += "{" + "final" + " : " + _final.ToString() + "} ";
			ret += "{" + "unicode" + " : " + _unicode.ToString() + "} ";
			ret += "{" + "initial-unicode" + " : " + _initial_unicode.ToString() + "} ";
			ret += "{" + "medial-unicode" + " : " + _medial_unicode.ToString() + "} ";
			ret += "{" + "final-unicode" + " : " + _final_unicode.ToString() + "} ";
			ret += "{" + "level" + " : " + _level.ToString() + "} ";
			return ret;
		}
	}
	public sealed class letters : IGoogle2uDB
	{
		public enum rowIds {
			alef, beh, teh, the, jeem, hah, khah, dal, thal, reh, zay, seen, sheen, sad, dad, tah, zah, ain, ghain
			, feh, qaf, kaf, lam, meem, noon, heh, waw, yeh, fathah, kasrah, dammah, maddah, hamza, waw_diph, yeh_diph
		};
		public string [] rowNames = {
			"alef", "beh", "teh", "the", "jeem", "hah", "khah", "dal", "thal", "reh", "zay", "seen", "sheen", "sad", "dad", "tah", "zah", "ain", "ghain"
			, "feh", "qaf", "kaf", "lam", "meem", "noon", "heh", "waw", "yeh", "fathah", "kasrah", "dammah", "maddah", "hamza", "waw_diph", "yeh_diph"
		};
		public System.Collections.Generic.List<lettersRow> Rows = new System.Collections.Generic.List<lettersRow>();

		public static letters Instance
		{
			get { return Nestedletters.instance; }
		}

		private class Nestedletters
		{
			static Nestedletters() { }
			internal static readonly letters instance = new letters();
		}

		private letters()
		{
			Rows.Add( new lettersRow("alef", "1", "alif", "letter", "long vowel", "", "", "aa", "ا", "ا", "ﺎ", "ﺎ", "0627\t", "FE8D", "FE8E", "FE8E", ""));
			Rows.Add( new lettersRow("beh", "2", "bā’", "letter", "cons", "", "moon", "b", "ب‎", "بـ‎", "ـبـ‎", "ـب‎", "0628", "FE91", "FE92", "FE90", ""));
			Rows.Add( new lettersRow("teh", "3", "tā’", "letter", "cons", "", "sun", "t", "ت‎", "تـ‎", "ـتـ‎", "ـت‎", "062A", "FE97", "FE98", "FE96", ""));
			Rows.Add( new lettersRow("the", "4", "thā’", "letter", "cons", "", "sun", "th", "ث‎", "ثـ‎", "ـثـ‎", "ـث‎", "062B", "FE9B", "FE9C", "FE9A", ""));
			Rows.Add( new lettersRow("jeem", "5", "jīm", "letter", "cons", "", "moon", "j", "ج‎", "جـ‎", "ـجـ‎", "ـج‎", "062C", "FE9F", "FEA0", "FE9E", ""));
			Rows.Add( new lettersRow("hah", "6", "ḥā’", "letter", "cons", "", "moon", "H", "ح‎", "حـ‎", "ـحـ‎", "ـح‎", "062D", "FEA3", "FEA4", "FEA2", ""));
			Rows.Add( new lettersRow("khah", "7", "khā’", "letter", "cons", "", "moon", "kh", "خ‎", "خـ‎", "ـخـ‎", "ـخ‎", "062E", "FEA7", "FEA8", "FEA6", ""));
			Rows.Add( new lettersRow("dal", "8", "dāl", "letter", "cons", "", "sun", "d", "د‎", "د‎", "ـد‎", "ـد‎", "062F", "FEA9", "FEAA", "FEAA", ""));
			Rows.Add( new lettersRow("thal", "9", "dhāl", "letter", "cons", "", "sun", "dh", "ذ‎", "ذ‎", "ـذ‎", "ـذ‎", "0630", "FEAB", "FEAC", "FEAC", ""));
			Rows.Add( new lettersRow("reh", "10", "rā’", "letter", "cons", "", "sun", "r", "ر‎", "ر‎", "ـر‎", "ـر‎", "0631", "FEAD", "FEAE", "FEAE", ""));
			Rows.Add( new lettersRow("zay", "11", "zayn / zāy / zā’", "letter", "cons", "", "sun", "z", "ز‎", "ز‎", "ـز‎", "ـز‎", "0632", "FEAF", "FEB0", "FEB0", ""));
			Rows.Add( new lettersRow("seen", "12", "sīn", "letter", "cons", "", "sun", "s", "س‎", "سـ‎", "ـسـ‎", "ـس‎", "0633", "FEB3", "FEB4", "FEB2", ""));
			Rows.Add( new lettersRow("sheen", "13", "shīn", "letter", "cons", "", "sun", "sh", "ش‎", "شـ‎", "ـشـ‎", "ـش‎", "0634", "FEB7", "FEB8", "FEB6", ""));
			Rows.Add( new lettersRow("sad", "14", "ṣād", "letter", "powerful", "", "sun", "S", "ص‎", "صـ‎", "ـصـ‎", "ـص‎", "0635", "FEBB", "FEBC", "FEBA", ""));
			Rows.Add( new lettersRow("dad", "15", "ḍād", "letter", "powerful", "", "sun", "D", "ض‎", "ضـ‎", "ـضـ‎", "ـض‎", "0636", "FEBF", "FEC0", "FEBE", ""));
			Rows.Add( new lettersRow("tah", "16", "ṭā’", "letter", "powerful", "", "sun", "T", "ط‎", "طـ‎", "ـطـ‎", "ـط‎", "0637", "FEC3", "FEC4", "FEC2", ""));
			Rows.Add( new lettersRow("zah", "17", "ẓā’", "letter", "powerful", "", "sun", "DH", "ظ‎", "ظـ‎", "ـظـ‎", "ـظ‎", "0638", "FEC7", "FEC8", "FEC6", ""));
			Rows.Add( new lettersRow("ain", "18", "‘ayn", "letter", "cons", "", "moon", "x", "ع‎", "عـ‎", "ـعـ‎", "ـع‎", "0639", "FECB", "FECC", "FECA", ""));
			Rows.Add( new lettersRow("ghain", "19", "ghayn", "letter", "cons", "", "moon", "gh", "غ‎", "غـ‎", "ـغـ‎", "ـغ‎", "063A", "FECF", "FED0", "FECE", ""));
			Rows.Add( new lettersRow("feh", "20", "fā’", "letter", "cons", "", "moon", "f", "ف‎", "فـ‎", "ـفـ‎", "ـف‎", "0641", "FED3", "FED4", "FED2", ""));
			Rows.Add( new lettersRow("qaf", "21", "qāf", "letter", "cons", "", "moon", "q", "ق‎", "قـ‎", "ـقـ‎", "ـق‎", "0642", "FED7", "FED8", "FED6", ""));
			Rows.Add( new lettersRow("kaf", "22", "kāf", "letter", "cons", "", "moon", "k", "ك‎", "كـ‎", "ـكـ‎", "ـك‎", "0643", "FEDB", "FEDC", "FEDA", ""));
			Rows.Add( new lettersRow("lam", "23", "lām", "letter", "cons", "", "sun", "l", "ل‎", "لـ‎", "ـلـ‎", "ـل‎", "0644", "FEDF", "FEE0", "FEDE", ""));
			Rows.Add( new lettersRow("meem", "24", "mīm", "letter", "cons", "", "moon", "m", "م‎", "مـ‎", "ـمـ‎", "ـم‎", "0645", "FEE3", "FEE4", "FEE2", ""));
			Rows.Add( new lettersRow("noon", "25", "nūn", "letter", "cons", "", "sun", "n", "ن‎", "نـ‎", "ـنـ‎", "ـن‎", "0646", "FEE7", "FEE8", "FEE6", ""));
			Rows.Add( new lettersRow("heh", "26", "hā’", "letter", "cons", "", "moon", "h", "ه‎", "هـ‎", "ـهـ‎", "ـه‎", "0647", "FEEB", "FEEC", "FEEA", ""));
			Rows.Add( new lettersRow("waw", "27", "wāw", "letter", "long vowel", "", "", "w, uu", "و‎", "و‎", "ـو‎", "ـو‎", "0648", "FEED", "FEEE", "FEEE", ""));
			Rows.Add( new lettersRow("yeh", "28", "yā’", "letter", "long vowel", "", "", "y, ii", "ي‎", "يـ‎", "ـيـ‎", "ـي", "064A", "FEF3", "FEF4", "FEF2", ""));
			Rows.Add( new lettersRow("fathah", "", "fatḥah", "symbol", "diacritic symbol", "", "", "a", "ـَ", "", "", "", "", "", "", "", "5"));
			Rows.Add( new lettersRow("kasrah", "", "kasrah", "symbol", "diacritic symbol", "", "", "i", "ـِ", "", "", "", "", "", "", "", "5"));
			Rows.Add( new lettersRow("dammah", "", "ḍammah", "symbol", "diacritic symbol", "", "", "u", "ـُ", "", "", "", "", "", "", "", "5"));
			Rows.Add( new lettersRow("maddah", "", "maddah", "symbol", "variation", "applied just on long vowel", "", "muted", "ـٓ", "", "", "", "", "", "", "", "5"));
			Rows.Add( new lettersRow("hamza", "", "hamza", "symbol", "variation", "applied just on long vowel", "", "", "", "", "", "", "", "", "", "", ""));
			Rows.Add( new lettersRow("waw_diph", "", "wāw diph", "diphtong", "", "variation wāw", "", "", "", "", "", "", "", "", "", "", ""));
			Rows.Add( new lettersRow("yeh_diph", "", "yā’ diph", "diphtong", "", "variation of yā’", "", "", "", "", "", "", "", "", "", "", ""));
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
		public lettersRow GetRow(rowIds in_RowID)
		{
			lettersRow ret = null;
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
		public lettersRow GetRow(string in_RowString)
		{
			lettersRow ret = null;
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
