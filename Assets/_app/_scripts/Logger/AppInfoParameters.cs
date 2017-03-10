using UnityEngine;

namespace EA4S.Core
{
    public class AppInfoParameters
    {
        public string AppVersion;
        public string Platform;
        public string operatingSystem;
        public string operatingSystemFamily;
        public string deviceModel;
        public string deviceName;
        public string deviceType;
        public string deviceUniqueIdentifier;
        public int systemMemorySize;
        public int graphicsMemorySize;
        public string graphicsDeviceVersion;
        public int ScreenWidth;
        public int ScreenHeight;
        public bool supportsGyroscope;
        public bool supportsVibration;
        public bool supportsAccelerometer;

        public AppInfoParameters()
        {
            AppVersion = AppConstants.AppVersion;
            deviceModel = SystemInfo.deviceModel;
            deviceName = SystemInfo.deviceName;
            deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            deviceType = SystemInfo.deviceType.ToString();
            operatingSystem = SystemInfo.operatingSystem;
            operatingSystemFamily = SystemInfo.operatingSystemFamily.ToString();
            systemMemorySize = SystemInfo.systemMemorySize;
            graphicsMemorySize = SystemInfo.graphicsMemorySize;
            graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            supportsGyroscope = SystemInfo.supportsGyroscope;
            supportsVibration = SystemInfo.supportsVibration;
            supportsAccelerometer = SystemInfo.supportsAccelerometer;
        }
    }
}