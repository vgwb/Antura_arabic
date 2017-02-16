using UnityEngine;
using System.Collections;

public class AN_SettingsActivityAction  {
	
	/**
     * Activity Action: Show system settings.
    */
	public const string ACTION_SETTINGS = "android.settings.SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of APNs.
     */
	public const string ACTION_APN_SETTINGS = "android.settings.APN_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of current location
     */
	public const string ACTION_LOCATION_SOURCE_SETTINGS = "android.settings.LOCATION_SOURCE_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of wireless controls
     * such as Wi-Fi, Bluetooth and Mobile networks..
     */
	public const string ACTION_WIRELESS_SETTINGS = "android.settings.WIRELESS_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow entering/exiting airplane mode.
     */
	public const string ACTION_AIRPLANE_MODE_SETTINGS = "android.settings.AIRPLANE_MODE_SETTINGS";
	
	/**
     * Activity Action: Show settings for accessibility modules.
     */
	public const string ACTION_ACCESSIBILITY_SETTINGS = "android.settings.ACCESSIBILITY_SETTINGS";
	
	/**
     * Activity Action: Show settings to control access to usage information.
     */
	public const string ACTION_USAGE_ACCESS_SETTINGS = "android.settings.USAGE_ACCESS_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of security and
     * location privacy.
     */
	public const string ACTION_SECURITY_SETTINGS = "android.settings.SECURITY_SETTINGS";
	
	/**
     * Activity Action: Show trusted credentials settings, opening to the user tab,
     * to allow management of installed credentials.
     */
	public const string ACTION_TRUSTED_CREDENTIALS_USER = "com.android.settings.TRUSTED_CREDENTIALS_USER";
	
	/**
     * Activity Action: Show dialog explaining that an installed CA cert may enable
     * monitoring of encrypted network traffic.
     */
	public const string ACTION_MONITORING_CERT_INFO = "com.android.settings.MONITORING_CERT_INFO";
	
	/**
     * Activity Action: Show settings to allow configuration of privacy options.
     */
	public const string ACTION_PRIVACY_SETTINGS = "android.settings.PRIVACY_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of Wi-Fi.
     */
	public const string ACTION_WIFI_SETTINGS = "android.settings.WIFI_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of a static IP
     * address for Wi-Fi.
     */
	public const string ACTION_WIFI_IP_SETTINGS = "android.settings.WIFI_IP_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of Bluetooth.
     */
	public const string ACTION_BLUETOOTH_SETTINGS = "android.settings.BLUETOOTH_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of Wifi Displays.
     */
	public const string ACTION_WIFI_DISPLAY_SETTINGS = "android.settings.WIFI_DISPLAY_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of
     * cast endpoints.
     */
	public const string ACTION_CAST_SETTINGS = "android.settings.CAST_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of date and time.
     */
	public const string ACTION_DATE_SETTINGS = "android.settings.DATE_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of sound and volume.
     */
	public const string ACTION_SOUND_SETTINGS = "android.settings.SOUND_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of display.
     */
	public const string ACTION_DISPLAY_SETTINGS = "android.settings.DISPLAY_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of locale.
     */
	public const string ACTION_LOCALE_SETTINGS = "android.settings.LOCALE_SETTINGS";
	
	/**
     * Activity Action: Show settings to configure input methods, in particular
     * allowing the user to enable input methods.
     */
	public const string ACTION_VOICE_INPUT_SETTINGS = "android.settings.VOICE_INPUT_SETTINGS";
	
	/**
     * Activity Action: Show settings to configure input methods, in particular
     * allowing the user to enable input methods.
     */
	public const string ACTION_INPUT_METHOD_SETTINGS = "android.settings.INPUT_METHOD_SETTINGS";
	
	/**
     * Activity Action: Show settings to enable/disable input method subtypes.
     */
	public const string ACTION_INPUT_METHOD_SUBTYPE_SETTINGS = "android.settings.INPUT_METHOD_SUBTYPE_SETTINGS";
	
	/**
     * Activity Action: Show a dialog to select input method.
     */
	public const string ACTION_SHOW_INPUT_METHOD_PICKER = "android.settings.SHOW_INPUT_METHOD_PICKER";
	
	/**
     * Activity Action: Show settings to manage the user input dictionary.
     */

	public const string ACTION_USER_DICTIONARY_SETTINGS = "android.settings.USER_DICTIONARY_SETTINGS";
	
	/**
     * Activity Action: Adds a word to the user dictionary.
     */

	public const string ACTION_USER_DICTIONARY_INSERT = "com.android.settings.USER_DICTIONARY_INSERT";
	
	/**
     * Activity Action: Show settings to allow configuration of application-related settings.
     */

	public const string ACTION_APPLICATION_SETTINGS = "android.settings.APPLICATION_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of application
     */

	public const string ACTION_APPLICATION_DEVELOPMENT_SETTINGS = "android.settings.APPLICATION_DEVELOPMENT_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of quick launch shortcuts.
     */

	public const string ACTION_QUICK_LAUNCH_SETTINGS = "android.settings.QUICK_LAUNCH_SETTINGS";
	
	/**
     * Activity Action: Show settings to manage installed applications.
     */

	public const string ACTION_MANAGE_APPLICATIONS_SETTINGS = "android.settings.MANAGE_APPLICATIONS_SETTINGS";
	
	/**
     * Activity Action: Show settings to manage all applications.
     */
	public const string ACTION_MANAGE_ALL_APPLICATIONS_SETTINGS = "android.settings.MANAGE_ALL_APPLICATIONS_SETTINGS";
	
	/**
     * Activity Action: Show screen of details about a particular application.
     */
	public const string ACTION_APPLICATION_DETAILS_SETTINGS = "android.settings.APPLICATION_DETAILS_SETTINGS";
	
	/**
     * Activity Action: Show the "app ops" settings screen.
     */
	public const string ACTION_APP_OPS_SETTINGS = "android.settings.APP_OPS_SETTINGS";
	
	/**
     * Activity Action: Show settings for system update functionality.
     */
	public const string ACTION_SYSTEM_UPDATE_SETTINGS = "android.settings.SYSTEM_UPDATE_SETTINGS";
	
	/**
     * Activity Action: Show settings to allow configuration of sync settings.
     */
	public const string ACTION_SYNC_SETTINGS = "android.settings.SYNC_SETTINGS";
	
	/**
     * Activity Action: Show add account screen for creating a new account.
     */
	public const string ACTION_ADD_ACCOUNT = "android.settings.ADD_ACCOUNT_SETTINGS";
	
	/**
     * Activity Action: Show settings for selecting the network operator.
     */

	public const string ACTION_NETWORK_OPERATOR_SETTINGS = "android.settings.NETWORK_OPERATOR_SETTINGS";
	
	/**
     * Activity Action: Show settings for selection of 2G/3G.
     */
	public const string ACTION_DATA_ROAMING_SETTINGS = "android.settings.DATA_ROAMING_SETTINGS";
	
	/**
     * Activity Action: Show settings for internal storage.
     */
	public const string ACTION_INTERNAL_STORAGE_SETTINGS = "android.settings.INTERNAL_STORAGE_SETTINGS";
	/**
     * Activity Action: Show settings for memory card storage.
     */
	public const string ACTION_MEMORY_CARD_SETTINGS = "android.settings.MEMORY_CARD_SETTINGS";
	
	/**
     * Activity Action: Show settings for global search.
     */
	public const string ACTION_SEARCH_SETTINGS = "android.search.action.SEARCH_SETTINGS";
	
	/**
     * Activity Action: Show general device information settings (serial
     * number, software version, phone number, etc.).
     */
	public const string ACTION_DEVICE_INFO_SETTINGS = "android.settings.DEVICE_INFO_SETTINGS";
	
	/**
     * Activity Action: Show NFC settings.
     */
	public const string ACTION_NFC_SETTINGS = "android.settings.NFC_SETTINGS";
	
	/**
     * Activity Action: Show NFC Sharing settings.
     */
	public const string ACTION_NFCSHARING_SETTINGS = "android.settings.NFCSHARING_SETTINGS";
	
	/**
     * Activity Action: Show NFC Tap & Pay settings
     */

	public const string ACTION_NFC_PAYMENT_SETTINGS = "android.settings.NFC_PAYMENT_SETTINGS";
	
	/**
     * Activity Action: Show Daydream settings.
     */
	public const string ACTION_DREAM_SETTINGS = "android.settings.DREAM_SETTINGS";
	
	/**
     * Activity Action: Show Notification listener settings.
     */
	public const string ACTION_NOTIFICATION_LISTENER_SETTINGS = "android.settings.NOTIFICATION_LISTENER_SETTINGS";

	
	/**
     * Activity Action: Show settings for video captioning.
     */
	public const string ACTION_CAPTIONING_SETTINGS = "android.settings.CAPTIONING_SETTINGS";
	
	/**
     * Activity Action: Show the top level print settings.
     */
	public const string ACTION_PRINT_SETTINGS = "android.settings.ACTION_PRINT_SETTINGS";

	/**
     * Activity Action: Show the regulatory information screen for the device.
     */
	public const string ACTION_SHOW_REGULATORY_INFO = "android.settings.SHOW_REGULATORY_INFO";
	
	/**
     * Activity Action: Show Device Name Settings.
     */
	public const string DEVICE_NAME_SETTINGS = "android.settings.DEVICE_NAME";
	
	/**
     * Activity Action: Show pairing settings.
     */
	public const string ACTION_PAIRING_SETTINGS = "android.settings.PAIRING_SETTINGS";
	
	/**
     * Activity Action: Show battery saver settings.
     */
	public const string ACTION_BATTERY_SAVER_SETTINGS = "android.settings.BATTERY_SAVER_SETTINGS";
	
	/**
     * Activity Action: Show Home selection settings. If there are multiple activities
     * that can satisfy the {@link Intent#CATEGORY_HOME} intent, this screen allows you
     * to pick your preferred activity.
     */

	public const string ACTION_HOME_SETTINGS = "android.settings.HOME_SETTINGS";
	
	/**
     * Activity Action: Show notification settings.
     */
	public	const string ACTION_NOTIFICATION_SETTINGS = "android.settings.NOTIFICATION_SETTINGS";
	
	/**
     * Activity Action: Show notification settings for a single app.
     */
	public const string ACTION_APP_NOTIFICATION_SETTINGS = "android.settings.APP_NOTIFICATION_SETTINGS";
	
	/**
     * Activity Action: Show notification redaction settings.
     */
	public const string ACTION_APP_NOTIFICATION_REDACTION = "android.settings.ACTION_APP_NOTIFICATION_REDACTION";
}
