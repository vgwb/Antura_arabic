namespace SRDebugger.Services.Implementation
{
    using System;
    using System.Collections.Generic;
    using SRF;
    using SRF.Service;
    using UnityEngine;

    /// <summary>
    /// Reports system specifications and environment information. Information that can
    /// be used to identify a user is marked as private, and won't be included in any generated
    /// reports.
    /// </summary>
    [Service(typeof (ISystemInformationService))]
    public class StandardSystemInformationService : ISystemInformationService
    {
        private readonly Dictionary<string, IList<ISystemInfo>> _info = new Dictionary<string, IList<ISystemInfo>>();

        public StandardSystemInformationService()
        {
            CreateDefaultSet();
        }

        public IEnumerable<string> GetCategories()
        {
            return _info.Keys;
        }

        public IList<ISystemInfo> GetInfo(string category)
        {
            IList<ISystemInfo> list;

            if (!_info.TryGetValue(category, out list))
            {
                Debug.LogError("[SystemInformationService] Category not found: {0}".Fmt(category));
                return new ISystemInfo[0];
            }

            return list;
        }

        public Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false)
        {
            var dict = new Dictionary<string, Dictionary<string, object>>();

            foreach (var keyValuePair in _info)
            {
                var category = new Dictionary<string, object>();

                foreach (var systemInfo in keyValuePair.Value)
                {
                    if (systemInfo.IsPrivate && !includePrivate)
                    {
                        continue;
                    }

                    category.Add(systemInfo.Title, systemInfo.Value);
                }

                dict.Add(keyValuePair.Key, category);
            }

            return dict;
        }

        private void CreateDefaultSet()
        {
            _info.Add("System", new[]
            {
                Info.Create("Operating System", SystemInfo.operatingSystem),
                Info.Create("Device Name", SystemInfo.deviceName, true),
                Info.Create("Device Type", SystemInfo.deviceType),
                Info.Create("Device Model", SystemInfo.deviceModel),
                Info.Create("CPU Type", SystemInfo.processorType),
                Info.Create("CPU Count", SystemInfo.processorCount),
                Info.Create("System Memory", SRFileUtil.GetBytesReadable(((long) SystemInfo.systemMemorySize)*1024*1024))
                //Info.Create("Process Name", () => Process.GetCurrentProcess().ProcessName)
            });


#if ENABLE_IL2CPP
            const string IL2CPP = "Yes";
#else
            const string IL2CPP = "No";
#endif

            _info.Add("Unity", new[]
            {
                Info.Create("Version", Application.unityVersion),
                Info.Create("Debug", Debug.isDebugBuild),
                Info.Create("Unity Pro", Application.HasProLicense()),
                Info.Create("Genuine",
                    "{0} ({1})".Fmt(Application.genuine ? "Yes" : "No",
                        Application.genuineCheckAvailable ? "Trusted" : "Untrusted")),
                Info.Create("System Language", Application.systemLanguage),
                Info.Create("Platform", Application.platform),
                Info.Create("IL2CPP", IL2CPP),
                Info.Create("SRDebugger Version", SRDebug.Version),
            });

            _info.Add("Display", new[]
            {
                Info.Create("Resolution", () => Screen.width + "x" + Screen.height),
                Info.Create("DPI", () => Screen.dpi),
                Info.Create("Fullscreen", () => Screen.fullScreen),
                Info.Create("Orientation", () => Screen.orientation)
            });

            _info.Add("Runtime", new[]
            {
                Info.Create("Play Time", () => Time.unscaledTime),
                Info.Create("Level Play Time", () => Time.timeSinceLevelLoad),
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                Info.Create("Current Level", () => Application.loadedLevelName),
#else
                Info.Create("Current Level", () =>
                {
                    var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                    return "{0} (Index: {1})".Fmt(activeScene.name, activeScene.buildIndex);
                }),
#endif
                Info.Create("Quality Level",
                    () =>
                        QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" +
                        QualitySettings.GetQualityLevel() + ")")
            });

            // Check for cloud build manifest
            var cloudBuildManifest = (TextAsset) Resources.Load("UnityCloudBuildManifest.json");
            var manifestDict = cloudBuildManifest != null
                ? Json.Deserialize(cloudBuildManifest.text) as Dictionary<string, object>
                : null;

            if (manifestDict != null)
            {
                var manifestList = new List<ISystemInfo>(manifestDict.Count);

                foreach (var kvp in manifestDict)
                {
                    if (kvp.Value == null)
                    {
                        continue;
                    }

                    var value = kvp.Value.ToString();
                    manifestList.Add(Info.Create(GetCloudManifestPrettyName(kvp.Key), value));
                }

                _info.Add("Build", manifestList);
            }

            _info.Add("Features", new[]
            {
                Info.Create("Location", SystemInfo.supportsLocationService),
                Info.Create("Accelerometer", SystemInfo.supportsAccelerometer),
                Info.Create("Gyroscope", SystemInfo.supportsGyroscope),
                Info.Create("Vibration", SystemInfo.supportsVibration)
            });

#if UNITY_IOS

            _info.Add("iOS", new[] {

#if UNITY_5
                Info.Create("Generation", UnityEngine.iOS.Device.generation),
                Info.Create("Ad Tracking", UnityEngine.iOS.Device.advertisingTrackingEnabled),
#else
                Info.Create("Generation", iPhone.generation),
                Info.Create("Ad Tracking", iPhone.advertisingTrackingEnabled),
#endif
            });

#endif

            _info.Add("Graphics", new[]
            {
                Info.Create("Device Name", SystemInfo.graphicsDeviceName),
                Info.Create("Device Vendor", SystemInfo.graphicsDeviceVendor),
                Info.Create("Device Version", SystemInfo.graphicsDeviceVersion),
                Info.Create("Max Tex Size", SystemInfo.maxTextureSize),
#if !UNITY_5
                Info.Create("Fill Rate",
                    SystemInfo.graphicsPixelFillrate > 0 ? "{0}mpix/s".Fmt(SystemInfo.graphicsPixelFillrate) : "Unknown"),
#endif
                Info.Create("NPOT Support", SystemInfo.npotSupport),
                Info.Create("Render Textures",
                    "{0} ({1})".Fmt(SystemInfo.supportsRenderTextures ? "Yes" : "No",
                        SystemInfo.supportedRenderTargetCount)),
                Info.Create("3D Textures", SystemInfo.supports3DTextures),
                Info.Create("Compute Shaders", SystemInfo.supportsComputeShaders),
#if !UNITY_5
                Info.Create("Vertex Programs", SystemInfo.supportsVertexPrograms),
#endif
                Info.Create("Image Effects", SystemInfo.supportsImageEffects),
                Info.Create("Cubemaps", SystemInfo.supportsRenderToCubemap),
                Info.Create("Shadows", SystemInfo.supportsShadows),
                Info.Create("Stencil", SystemInfo.supportsStencil),
                Info.Create("Sparse Textures", SystemInfo.supportsSparseTextures)
            });
        }

        private static string GetCloudManifestPrettyName(string name)
        {
            switch (name)
            {
                case "scmCommitId":
                    return "Commit";

                case "scmBranch":
                    return "Branch";

                case "cloudBuildTargetName":
                    return "Build Target";

                case "buildStartTime":
                    return "Build Date";
            }

            // Return name with first letter capitalised
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }

        private class Info : ISystemInfo
        {
            private Func<object> _valueGetter;
            public string Title { get; set; }

            public object Value
            {
                get
                {
                    try
                    {
                        return _valueGetter();
                    }
                    catch (Exception e)
                    {
                        return "Error ({0})".Fmt(e.GetType().Name);
                    }
                }
            }

            public bool IsPrivate { get; set; }

            public static Info Create(string name, Func<object> getter, bool isPrivate = false)
            {
                return new Info
                {
                    Title = name,
                    _valueGetter = getter,
                    IsPrivate = isPrivate
                };
            }

            public static Info Create(string name, object value, bool isPrivate = false)
            {
                return new Info
                {
                    Title = name,
                    _valueGetter = () => value,
                    IsPrivate = isPrivate
                };
            }
        }
    }
}
