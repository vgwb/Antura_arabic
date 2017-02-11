using UnityEngine;

/// <summary>
/// Helper class for drawing Enums in custom Unity Editor inspector windows.
/// </summary>
public class WMG_EnumFlagAttribute : PropertyAttribute
{
	public string enumName;
	
	public WMG_EnumFlagAttribute() {}
	
	public WMG_EnumFlagAttribute(string name) {
		enumName = name;
	}
}