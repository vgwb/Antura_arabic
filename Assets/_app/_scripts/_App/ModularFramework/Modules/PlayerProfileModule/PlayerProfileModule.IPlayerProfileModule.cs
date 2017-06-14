namespace EA4S.Core
{
    public interface IPlayerProfileModule : IModule<IPlayerProfileModule>
    {
        GlobalOptions Options { get; set; }
        GlobalOptions LoadGlobalOptions<T>(T _defaultOptions) where T : GlobalOptions;
        void SaveAllOptions();
    }
}