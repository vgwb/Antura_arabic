using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace Antura.Tests
{
    [TestFixture]
    [Category("Antura Helpers")]
    internal class HelperTests
    {
        [Test]
        public void MathHelperGetAverage()
        {
            var floatList = new List<float> {0.1f, 0.4f, 0.8f, 2.99f, -1.0f};
            var average = global::Antura.Helpers.MathHelper.GetAverage(floatList);
            UnityEngine.Debug.Log(average);
            Assert.AreEqual(0.657999992f, average);
        }
        
        [Test]
        public void PassingTest()
        {
            Assert.Pass();
        }

    }
}
