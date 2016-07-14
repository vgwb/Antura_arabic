using SRDebugger.Services;
using SRDebugger.Services.Implementation;
using SRF.Service;

public static class SRDebug
{
    public const string Version = SRDebugger.VersionInfo.Version;

    public static IDebugService Instance
    {
        get { return SRServiceManager.GetService<IDebugService>(); }
    }

    public static void Init()
    {
        if (!SRServiceManager.HasService<IConsoleService>())
        {
            // Force console service to be created early, so it catches more of the initialization log.
            new StandardConsoleService();
        }

        // Load the debug service
        SRServiceManager.GetService<IDebugService>();
    }
}
