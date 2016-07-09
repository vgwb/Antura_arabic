namespace PygmyMonkey.AdvancedBuilder
{
	public interface IPlatformAdditionalRenderer
	{
		/*
		 * Draw additional build summary
		 */
		void drawAdditionalBuildSummary(PlatformArchitecture platformArchitecture, ITextureCompression textureCompression, string bundleVersion);
		
		
		/*
		 * Check for specific platform warnings/errors
		 */
		void checkWarningsAndErrors(ErrorReporter errorReporter);
	}
}