namespace PygmyMonkey.AdvancedBuilder
{
	public interface IAdvancedCustomBuild
	{
		void OnPreBuild(Configuration configuration, System.DateTime buildDate);
		void OnPostBuild(Configuration configuration, System.DateTime buildDate);
		void OnEveryBuildDone();
	}
}
