////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSDateTimePicker : SA.Common.Pattern.Singleton<IOSDateTimePicker>  {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_ShowDP(int mode);

	[DllImport ("__Internal")]
	private static extern void _ISN_ShowDPWithTime(int mode, double seconds);
		
	#endif

	public Action<DateTime> OnDateChanged = delegate {};
	public Action<DateTime> OnPickerClosed = delegate {};


	//--------------------------------------
	// Initialization
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------

	/// <summary>
	/// Displays DateTimePickerUI with DateTimePicker Mode.
	///
	///<param name="mode">An object that contains the IOSDateTimePicker mode.</param>
	/// </summary>	
	public void Show(IOSDateTimePickerMode mode) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowDP( (int) mode);
		#endif
	}

	/// <summary>
	/// Displays DateTimePickerUI with DateTimePicker Mode and pre-set date.
	///
	///<param name="mode">An object that contains the IOSDateTimePicker mode</param>
	///<param name="name">An object DateTime that contains pre-set date</param>
	/// </summary>
	public void Show(IOSDateTimePickerMode mode, DateTime dateTime) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			DateTime sTime = new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc);
			double unixTimestamp = (dateTime - sTime).TotalSeconds;
			_ISN_ShowDPWithTime( (int) mode, unixTimestamp);	
		#endif
	}

	//--------------------------------------
	// Events
	//--------------------------------------

	private void DateChangedEvent(string time) {
		DateTime dt  = DateTime.Parse(time);

		OnDateChanged(dt);
	}

	private void PickerClosed(string time) {
		DateTime dt  = DateTime.Parse(time);

		OnPickerClosed(dt);
	}
}
