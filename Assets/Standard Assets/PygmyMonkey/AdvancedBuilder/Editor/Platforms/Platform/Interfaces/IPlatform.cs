namespace PygmyMonkey.AdvancedBuilder
{
	public interface IPlatform
	{
		/*
		 * Platform common properties
		 */
		PlatformProperties getPlatformProperties();


		/*
		 * Set up additional parameters
		 */
		void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration);


		/*
		 * Format destination file path
		 */
		string formatDestinationPath(string filePath);
	}
}