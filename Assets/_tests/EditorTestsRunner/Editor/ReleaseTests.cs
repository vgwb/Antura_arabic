using Antura.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace Antura.Tests.Release
{
    [TestFixture]
    [Category("Antura Release")]
    internal class ReleaseTests
    {
        [Test]
        public void CheckAppConstants()
        {
            if (AppConfig.DebugLogEnabled) {
                UnityEngine.Debug.Log("DebugLogEnabled should be FALSE");
                Assert.Fail();
            }

            if (AppConfig.UnityAnalyticsEnabled == false) {
                UnityEngine.Debug.Log("UnityAnalyticsEnabled should be TRUE");
                Assert.Fail();
            }

            if (AppConfig.DebugPanelEnabledAtStartup) {
                UnityEngine.Debug.Log("DebugPanelEnabledAtStartup should be FALSE");
                Assert.Fail();
            }

            if (AppConfig.DebugLogDbInserts) {
                UnityEngine.Debug.Log("DebugLogDbInserts should be FALSE");
                Assert.Fail();
            }

            if (AppConfig.DisableFirstContact) {
                UnityEngine.Debug.Log("DisableFirstContact should be FALSE");
                Assert.Fail();
            }
        }
    }
}
