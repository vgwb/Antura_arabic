using UnityEngine;
using System;

namespace EA4S
{
    public class AppInfoParameters
    {
        public string AppVersion;
        public string Platform;
        public string User;

        public AppInfoParameters()
        {
            AppVersion = AppConstants.AppVersion;
            Platform = string.Format("{0} | (sys mem) {1} | (video mem) {2} | {3} |", SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize, Screen.width + "x" + Screen.height);
        }
    }
}