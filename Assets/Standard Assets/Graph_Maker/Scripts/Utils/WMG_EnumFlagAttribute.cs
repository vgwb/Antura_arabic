using UnityEngine;

public class WMG_EnumFlagAttribute : PropertyAttribute
{
	public string enumName;
	
	public WMG_EnumFlagAttribute() {}
	
	public WMG_EnumFlagAttribute(string name) {
		enumName = name;
	}
}