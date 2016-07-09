namespace PygmyMonkey.AdvancedBuilder
{
	public interface IDefaultRenderer
	{
		/*
		 * Draw in inspector
		 */
		void drawInspector();


		/*
		 * Check for warnings and errors
		 */
		void checkWarningsAndErrors(ErrorReporter errorReporter);
	}
}