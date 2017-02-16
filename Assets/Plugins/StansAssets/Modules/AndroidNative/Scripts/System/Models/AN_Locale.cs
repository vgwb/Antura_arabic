using UnityEngine;
using System.Collections;

public class AN_Locale {

	//Returns the country code for this locale, or "" if this locale doesn't correspond to a specific country.
	public string CountryCode;

	//Returns the name of this locale's country, localized to locale. Returns the empty string if this locale does not correspond to a specific country.
	public string DisplayCountry;


	//Returns the language code for this Locale or the empty string if no language was set.
	public string LanguageCode;

	//Returns the name of this locale's language, localized to locale. If the language name is unknown, the language code is returned.
	public string DisplayLanguage;

}
