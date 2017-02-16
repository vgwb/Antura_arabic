using UnityEngine;
using System;
using System.Collections;

public class LocalNotificationTemplate  {

	private int _id;
	private string _title;
	private string _message;
	private DateTime _fireDate;


	private const string DATA_SPLITTER = "|||";


	public LocalNotificationTemplate(string data) {
	
		string[] nodes = data.Split(new string[] { DATA_SPLITTER }, StringSplitOptions.None);



		_id = System.Convert.ToInt32(nodes[0]);
		_title =  nodes[1];
		_message = nodes[2];
		_fireDate = new DateTime(System.Convert.ToInt64(nodes[3]));
	}

	public LocalNotificationTemplate(int nId, string ttl, string msg, DateTime date) {
		_id = nId;
		_title = ttl;
		_message = msg;
		_fireDate = date;
	}



	public int id {
		get {
			return _id;
		}
	}

	public string title {
		get {
			return _title;
		}
	}

	public string message {
		get {
			return _message;
		}
	}

	public DateTime fireDate {
		get {
			return _fireDate;
		}
	}



	public string SerializedString {
		get {
			return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes( id.ToString() + DATA_SPLITTER + title + DATA_SPLITTER + message + DATA_SPLITTER + fireDate.Ticks.ToString() ));
		}
	}

	public bool IsFired {
		get {
			if(System.DateTime.Now.Ticks > fireDate.Ticks) {
				return true;
			} else {
				return false;
			}
		}
	}


}
