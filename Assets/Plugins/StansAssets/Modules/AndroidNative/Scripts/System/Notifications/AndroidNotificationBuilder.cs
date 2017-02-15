using UnityEngine;
using System.Collections;

public class AndroidNotificationBuilder {
	private int _id = 1;
	private string _title = string.Empty;
	private string _message = string.Empty;
	private int _time = 1;
	private string _sound = string.Empty;
	private string _smallIcon = string.Empty;
	private bool _vibration = false;
	private bool _showIfAppForeground = true;
	private bool _repeating = false;
	private int _repeatDelay = 60;
	private string _largeIcon = string.Empty;
	private Texture2D _bigPicture = null;
	private NotificationColor _color = null;
	private int _wakeLockTime = 10000;

	private const string SOUND_SILENT = "SOUND_SILENT";

	public class NotificationColor {
		
		private Color _value;
		
		public NotificationColor(Color value) {
			_value = value;
		}
		
		public Color Value {
			get {
				return _value;
			}
		}
	}

	public AndroidNotificationBuilder(int id, string title, string message, int time) {
		_id = id;
		_title = title;
		_message = message;
		_time = time;

		_largeIcon = AndroidNativeSettings.Instance.LocalNotificationLargeIcon == null ? string.Empty : AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name.ToLower();
		_smallIcon = AndroidNativeSettings.Instance.LocalNotificationSmallIcon == null ? string.Empty : AndroidNativeSettings.Instance.LocalNotificationSmallIcon.name.ToLower();
		_sound = AndroidNativeSettings.Instance.LocalNotificationSound == null ? string.Empty : AndroidNativeSettings.Instance.LocalNotificationSound.name;
		_vibration = AndroidNativeSettings.Instance.EnableVibrationLocal;
		_showIfAppForeground = AndroidNativeSettings.Instance.ShowWhenAppIsForeground;
		_wakeLockTime = AndroidNativeSettings.Instance.LocalNotificationWakeLockTimer;
	}

	public void SetColor(NotificationColor color) {
		_color = color;
	}

	public void SetSoundName(string sound) {
		_sound = sound;
	}

	public void SetIconName(string icon) {
		_smallIcon = icon;
	}

	public void SetVibration(bool vibration) {
		_vibration = vibration;
	}

	public void SetSilentNotification() {
		_sound = SOUND_SILENT;
	}

	public void ShowIfAppIsForeground(bool show) {
		_showIfAppForeground = show;
	}

	public void SetRepeating(bool repeat) {
		_repeating = repeat;
	}

	public void SetRepeatDelay(int delay) {
		_repeatDelay = delay;
	}

	public void SetLargeIcon(string icon){
		_largeIcon = icon;
	}

	public void SetBigPicture(Texture2D picture) {
		_bigPicture = picture;
	}

	public void SetWakeLockTime(int wakeTime) {
		_wakeLockTime = wakeTime;
	}

	public int Id {
		get {
			return _id;
		}
	}

	public string Title {
		get {
			return _title;
		}
	}

	public string Message {
		get {
			return _message;
		}
	}

	public int Time {
		get {
			return _time;
		}
	}

	public NotificationColor Color {
		get {
			return _color;
		}
	}

	public string Sound {
		get {
			return _sound;
		}
	}

	public string Icon {
		get {
			return _smallIcon;
		}
	}

	public bool Vibration {
		get {
			return _vibration;
		}
	}

	public bool ShowIfAppForeground {
		get {
			return _showIfAppForeground;
		}
	}

	public bool Repeating {
		get { 
			return _repeating;
		}
	}

	public int RepeatDelay {
		get { 
			return _repeatDelay;
		}
	}

	public string LargeIcon {
		get {
			return _largeIcon;
		}
	}

	public Texture2D BigPicture {
		get {
			return _bigPicture;
		}
	}

	public int WakeLockTime {
		get {
			return _wakeLockTime;
		}
	}
}
