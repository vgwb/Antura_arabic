using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AN_Storage
{

	public static void Save (string key, string data)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidNative.SaveToCacheStorage (key, System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data)));
		#else
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (data)));
		#endif
	}

	public static void Save (string key, Texture2D texture)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidNative.SaveToCacheStorage (key, System.Convert.ToBase64String(texture.EncodeToPNG()));
		#else
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (texture.EncodeToPNG ()));
		#endif
	}

	public static void Save (string key, byte[] data)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidNative.SaveToCacheStorage (key, System.Convert.ToBase64String(data));
		#else
		PlayerPrefs.SetString (key, System.Convert.ToBase64String (data));
		#endif
	}

	public static string GetString (string key)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		string base64str = AndroidNative.GetFromCacheStorage (key);
		if (!base64str.Equals (string.Empty)) {
		byte[] decodedFromBase64 = System.Convert.FromBase64String (base64str);
		return System.Text.Encoding.UTF8.GetString (decodedFromBase64);
		} else {
		return string.Empty;
		}
		#else
		if (PlayerPrefs.HasKey (key)) {
			byte[] decodedFromBase64 = System.Convert.FromBase64String (PlayerPrefs.GetString (key));
			return System.Text.Encoding.UTF8.GetString (decodedFromBase64);
		} else {
			return string.Empty;
		}
		#endif
	}

	public static Texture2D GetTexture (string key)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		string base64str = AndroidNative.GetFromCacheStorage (key);
		if (!base64str.Equals (string.Empty)) {
		byte[] decodedFromBase64 = System.Convert.FromBase64String (base64str);
		Texture2D img = new Texture2D (1, 1, TextureFormat.DXT5, false);
		img.LoadImage (decodedFromBase64);

		return img;
		} else {
		return Texture2D.whiteTexture;
		}
		#else
		if (PlayerPrefs.HasKey (key)) {
			byte[] decodedFromBase64 = System.Convert.FromBase64String (PlayerPrefs.GetString (key));
			Texture2D img = new Texture2D (1, 1, TextureFormat.DXT5, false);
			img.LoadImage (decodedFromBase64);

			return img;
		} else {
			return Texture2D.whiteTexture;
		}
		#endif
	}

	public static byte[] GetData (string key)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		string base64str = AndroidNative.GetFromCacheStorage (key);
		if (!base64str.Equals (string.Empty)) {
		return System.Convert.FromBase64String (base64str);
		} else {
		return new byte[0];
		}
		#else
		if (PlayerPrefs.HasKey (key)) {
			return System.Convert.FromBase64String (PlayerPrefs.GetString (key));
		} else {
			return new byte[0];
		}
		#endif
	}

}
