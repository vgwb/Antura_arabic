namespace PygmyMonkey.AdvancedBuilder
{
	public enum PlatformType
	{
		Android = 0,
		iOS,
		WebPlayer,
		Windows,
		Mac,
		Linux,
		WindowsPhone8,
		WindowsStore,
		WebGL,
		BlackBerry,
		#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
		tvOS,
		#endif
	}
}