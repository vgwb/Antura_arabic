namespace SRDebugger.Services.Implementation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using SRF;
    using SRF.Service;
    using UnityEngine;

    [Service(typeof (IConsoleService))]
    public class StandardConsoleService : IConsoleService
    {
        private readonly SRList<ConsoleEntry> _allConsoleEntries = new SRList<ConsoleEntry>();
        private readonly bool _collapseEnabled;
        private readonly SRList<ConsoleEntry> _consoleEntries = new SRList<ConsoleEntry>();
        private readonly ReadOnlyCollection<ConsoleEntry> _consoleEntriesReadOnly;
        private ReadOnlyCollection<ConsoleEntry> _allConsoleEntriesReadOnly;
        private bool _hasCleared;

        public StandardConsoleService()
        {
#if UNITY_5
            Application.logMessageReceived += UnityLogCallback;
#else
			Application.RegisterLogCallback(UnityLogCallback);
#endif

            SRServiceManager.RegisterService<IConsoleService>(this);

            _collapseEnabled = Settings.Instance.CollapseDuplicateLogEntries;

            _allConsoleEntriesReadOnly = _allConsoleEntries.AsReadOnly();
            _consoleEntriesReadOnly = _consoleEntries.AsReadOnly();
        }

        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }
        public int InfoCount { get; private set; }
        public event ConsoleUpdatedEventHandler Updated;

        public IList<ConsoleEntry> Entries
        {
            get
            {
                if (!_hasCleared)
                {
                    return _allConsoleEntriesReadOnly;
                }

                return _consoleEntriesReadOnly;
            }
        }

        public IList<ConsoleEntry> AllEntries
        {
            get { return _consoleEntriesReadOnly; }
        }

        public void Clear()
        {
            _hasCleared = true;
            _consoleEntries.Clear();
            ErrorCount = WarningCount = InfoCount = 0;

            OnUpdated();
        }

        public void ResetCounters() {}

        protected void OnEntryAdded(ConsoleEntry entry)
        {
            if (_hasCleared)
            {
                _consoleEntries.Add(entry);
            }

            _allConsoleEntries.Add(entry);

            OnUpdated();
        }

        protected void OnEntryDuplicated(ConsoleEntry entry)
        {
            entry.Count++;

            OnUpdated();

            // If has cleared, add this entry again for the current list
            if (_hasCleared && _consoleEntries.Count == 0)
            {
                OnEntryAdded(new ConsoleEntry(entry) {Count = 1});
            }
        }

        private void OnUpdated()
        {
            if (Updated != null)
            {
                try
                {
                    Updated(this);
                }
                catch {}
            }
        }

        private void UnityLogCallback(string condition, string stackTrace, LogType type)
        {
            //if (condition.StartsWith("[SRConsole]"))
            //    return;

            var prevMessage = _collapseEnabled && _allConsoleEntries.Count > 0
                ? _allConsoleEntries[_allConsoleEntries.Count - 1]
                : null;

            if (prevMessage != null && prevMessage.LogType == type && prevMessage.Message == condition &&
                prevMessage.StackTrace == stackTrace)
            {
                OnEntryDuplicated(prevMessage);
            }
            else
            {
                var newEntry = new ConsoleEntry
                {
                    LogType = type,
                    StackTrace = stackTrace,
                    Message = condition
                };

                OnEntryAdded(newEntry);
            }

            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    ErrorCount++;
                    break;

                case LogType.Warning:
                    WarningCount++;
                    break;

                case LogType.Log:
                    InfoCount++;
                    break;
            }
        }
    }
}
