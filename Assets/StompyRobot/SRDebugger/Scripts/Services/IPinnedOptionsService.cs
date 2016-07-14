namespace SRDebugger.Services
{
    using Internal;

    public interface IPinnedUIService
    {
        bool IsProfilerPinned { get; set; }
        void Pin(OptionDefinition option, int order = -1);
        void Unpin(OptionDefinition option);
        bool HasPinned(OptionDefinition option);
    }
}
