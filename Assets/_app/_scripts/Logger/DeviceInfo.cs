using UnityEngine;

namespace EA4S.Core
{
    public class DeviceInfo
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

        public int graphicsDeviceID;
        public string graphicsDeviceName;
        public string graphicsDeviceType;
        public string graphicsDeviceVendor;
        public int graphicsDeviceVendorID;
        public string graphicsDeviceVersion;
        public int graphicsMemorySize;
        public bool graphicsMultiThreaded;
        public int graphicsShaderLevel;

        public int ScreenWidth;
        public int ScreenHeight;
        public bool supportsGyroscope;
        public bool supportsVibration;
        public bool supportsAccelerometer;
        public bool supportsLocationService;

        public DeviceInfo()
        {
            AppVersion = AppConstants.AppVersion;
            deviceModel = SystemInfo.deviceModel;
            deviceName = SystemInfo.deviceName;
            deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
            deviceType = SystemInfo.deviceType.ToString();
            operatingSystem = SystemInfo.operatingSystem;
            operatingSystemFamily = SystemInfo.operatingSystemFamily.ToString();
            systemMemorySize = SystemInfo.systemMemorySize;

            graphicsDeviceID = SystemInfo.graphicsDeviceID;
            graphicsDeviceName = SystemInfo.graphicsDeviceName;
            graphicsDeviceType = SystemInfo.graphicsDeviceType.ToString();
            graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
            graphicsDeviceVendorID = SystemInfo.graphicsDeviceVendorID;
            graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
            graphicsMemorySize = SystemInfo.graphicsMemorySize;
            graphicsMultiThreaded = SystemInfo.graphicsMultiThreaded;
            graphicsShaderLevel = SystemInfo.graphicsShaderLevel;

            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            supportsGyroscope = SystemInfo.supportsGyroscope;
            supportsVibration = SystemInfo.supportsVibration;
            supportsAccelerometer = SystemInfo.supportsAccelerometer;
            supportsLocationService = SystemInfo.supportsLocationService;
        }
    }
}