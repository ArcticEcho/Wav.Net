using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WavMath = WavDotNet.Tools.Math;

namespace WavDotNet.Tests.UnitTesting
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void TestToDecibels()
        {
            Assert.AreEqual(WavMath.ToDecibels(10D), 20);
            Assert.AreEqual(WavMath.ToDecibels(10F), 20);
            Assert.AreEqual(WavMath.ToDecibels(100D), 40);
            Assert.AreEqual(WavMath.ToDecibels(100F), 40);
        }

        [TestMethod]
        public void TestToAmplitude()
        {
            Assert.AreEqual(WavMath.ToAmplitude(100D), 100000);
            Assert.AreEqual(WavMath.ToAmplitude(100F), 100000);
        }
    }
}
