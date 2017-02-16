using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FB_Object  {


	//The Open Graph object ID of the object being sent.
	public string Id;

	//Object image url's
	public List<string> ImageUrls = new List<string>();

	//Object title
	public string Title;

	//Object type
	public string Type;

	//Object created time
	public DateTime CreatedTime;
	public string CreatedTimeString;



	public void SetCreatedTime(string time_string) {
		CreatedTimeString = time_string;
		CreatedTime =  DateTime.Parse(time_string);
	}

	public void AddImageUrl(string url) {
		ImageUrls.Add(url);
	}

}
