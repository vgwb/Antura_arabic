using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AddressBookController : SA.Common.Pattern.Singleton<AddressBookController> {

	public static event Action OnContactsLoadedAction = delegate{};
	


	private const string DATA_SPLITTER_1 = "&#&";
	private const string DATA_SPLITTER_2 = "#&#";
	private const int byte_limit = 256;
	private static bool _isLoaded = false;

	private List<AndroidContactInfo> _contacts = new List<AndroidContactInfo>();
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void LoadContacts(){
		AndroidNative.LoadContacts();
	}

	public List<AndroidContactInfo> contacts {
		get {
			return _contacts;
		}
	}


	
	private void OnContactsLoaded(string data)  {
		if(data.Equals(string.Empty)) {
			Debug.Log("AddressBookController OnContactsLoaded, no data avaiable");
			return;
		}
		parseContacts(data);

		_isLoaded = true;
		Debug.Log ("OnContactsLoaded, total:" + _contacts.Count);
		OnContactsLoadedAction ();
	}
	
	private void parseContacts(string data) {
		string[] contactsInfo = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		foreach(string str in contactsInfo){
			if(isValid (str)) {
				AndroidContactInfo info = new AndroidContactInfo();
				if(str.Contains(DATA_SPLITTER_1)) {
					string[] tmpInfo = Regex.Split(str, DATA_SPLITTER_1);
					info.name = trimString(tmpInfo[0], 5);
					info.phone = trimString(tmpInfo[1], 6);
					//email//
					info.email = new AndroidABEmail();
					
					if(tmpInfo[2].Contains(DATA_SPLITTER_2)) {
						string[] emailInfo = Regex.Split(tmpInfo[2], DATA_SPLITTER_2);
						info.email.email = trimString(emailInfo[0], 6);
						info.email.emailType = emailInfo[1];
					}
					else {
						info.email.email = trimString(tmpInfo[2], 6);
					}
					//email//
					info.note = trimString(tmpInfo[3], 5);
					//chat//
					info.chat = new AndroidABChat();
					if(tmpInfo[4].Contains(DATA_SPLITTER_2)) {
						string[] chatInfo = Regex.Split(tmpInfo[4], DATA_SPLITTER_2);
						info.chat.name = trimString(chatInfo[0], 5);
						info.chat.type = chatInfo[1];
					}
					else {
						info.chat.name = trimString(tmpInfo[4], 5);
					}
					//chat//
					//Organization//
					info.organization = new AndroidABOrganization();
					if(tmpInfo[5].Contains(DATA_SPLITTER_2)) {
						string[] orgInfo = Regex.Split(tmpInfo[5], DATA_SPLITTER_2);
						info.organization.name = trimString(orgInfo[0], 13);
						info.organization.title = orgInfo[1];
					}
					else {
						info.organization.name = trimString(tmpInfo[5], 13);
					}
					//Photo//
					byte[] _buffer;
					string photo_bytes_array = trimString(tmpInfo[6], 6);
					if(havePhoto(photo_bytes_array)) {
						string[] array = photo_bytes_array.Split("," [0]);
						
						List<byte> listOfBytes = new List<byte> ();
						foreach(string s in array) {
							int param = System.Convert.ToInt32(s);
							int temp_param = param < 0 ? byte_limit + param : param;
							listOfBytes.Add (System.Convert.ToByte(temp_param));
						}
						
						_buffer = listOfBytes.ToArray ();
						
						Texture2D tex = new Texture2D(150, 150);
						tex.LoadImage(_buffer);					
						info.photo = tex;
						//Photo//
					}
					else {
						info.photo = null;
					}
					//Organization// 
					//Address//
					info.address = new AndroidABAddress();
					string[] adrInfo = Regex.Split(tmpInfo[7], DATA_SPLITTER_2);
					info.address.poBox = trimString(adrInfo[0], 8);
					info.address.street = adrInfo[1];
					info.address.city = adrInfo[2];
					info.address.state = adrInfo[3];
					info.address.postalCode = adrInfo[4];
					info.address.country = adrInfo[5];
					info.address.type = adrInfo[6];
					//Address//
				}
				else {
					info.name = trimString(str, 5);
				}
				
				_contacts.Add(info);
			}
		}
	}
	
	private string trimString(string str, int index) {
		return str.Substring(index);
	}
	
	private bool isValid(string str) {
		return str.Contains("Name");
	}
	
	private bool havePhoto(string str) {
		return str.Equals("-") ? false : true;
	}

	public static bool isLoaded {
		get {
			return _isLoaded;
		}
	}
}
