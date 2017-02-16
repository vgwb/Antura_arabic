using UnityEngine;
using System.Collections;

public class AN_NetworkInfo  {


	//Device local Ip Address
	public string IpAddress;

	//Device Mac Address
	public string MacAddress;


	//Network Sub Mask
	public string SubnetMask;
	
	//Returns the service set identifier (SSID) of the current 802.11 network. If the SSID can be decoded as UTF-8, it will be returned surrounded by double quotation marks. Otherwise, it is returned as a string of hex digits. The SSID may be <unknown ssid> if there is no network currently connected.
	public string SSID;

	//Return the basic service set identifier (BSSID) of the current access point. The BSSID may be null if there is no network currently connected.
	public string BSSID;

	//Link speed in Mbps
	public int LinkSpeed;

	//Each configured network has a unique small integer ID, used to identify the network when performing operations on the supplicant. This method returns the ID for the currently connected network.
	public int NetworkId;

}
