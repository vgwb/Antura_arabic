namespace SRDebugger.Services.Implementation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Internal;
    using SRF.Service;

    [Service(typeof (IOptionsService))]
    public class OptionsServiceImpl : IOptionsService
    {
        private readonly List<OptionDefinition> _options = new List<OptionDefinition>();
        private readonly IList<OptionDefinition> _optionsReadonly;

        public OptionsServiceImpl()
        {
            _optionsReadonly = new ReadOnlyCollection<OptionDefinition>(_options);

            Scan(SROptions.Current);
        }

        public ICollection<OptionDefinition> Options
        {
            get { return _optionsReadonly; }
        }

        public void Scan(object obj)
        {
            _options.AddRange(SRDebuggerUtil.ScanForOptions(obj));
        }
    }
}
