using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WavDotNet.Core;

namespace WavDotNet.Tests.UnitTesting
{
    [TestClass]
    public class SamplesTest
    {
        [TestMethod]
        public void ArrayConstructor()
        {
            var testSamples = new int[1000];
            for (var i = 0; i < 1000; i++)
            {
                testSamples[i] = i;
            }
            var samples = new Samples<int>(testSamples);

            Assert.IsNotNull(samples);
            Assert.AreEqual(1000, samples.Count);
            Assert.AreEqual(100, samples[100]);
            Assert.AreEqual(typeof(int), samples.SampleType);

            var j = 0;
            foreach (var sample in samples)
            {
                Assert.AreEqual(j, sample);
                j++;
            }
        }

        [TestMethod]
        public void ListConstructor()
        {
            var testSamples = new List<int>();
            for (var i = 0; i < 1000; i++)
            {
                testSamples.Add(i);
            }
            var samples = new Samples<int>(testSamples);

            Assert.IsNotNull(samples);
            Assert.AreEqual(1000, samples.Count);
            Assert.AreEqual(100, samples[100]);
            Assert.AreEqual(typeof(int), samples.SampleType);

            var j = 0;
            foreach (var sample in samples)
            {
                Assert.AreEqual(j, sample);
                j++;
            }
        }

        [TestMethod]
        public void IEnumerableConstructor()
        {
            var testSamples = new Queue<int>();
            for (var i = 0; i < 1000; i++)
            {
                testSamples.Enqueue(i);
            }
            var samples = new Samples<int>(testSamples);

            Assert.IsNotNull(samples);
            Assert.AreEqual(1000, samples.Count);
            Assert.AreEqual(100, samples[100]);
            Assert.AreEqual(typeof(int), samples.SampleType);

            var j = 0;
            foreach (var sample in samples)
            {
                Assert.AreEqual(j, sample);
                j++;
            }
        }

        [TestMethod]
        public void ManualConstructor()
        {
            var testSamples = new int[1000];
            for (var i = 0; i < 1000; i++)
            {
                testSamples[i] = i;
            }
            var samples = new Samples<int>(() => testSamples.Length, i => testSamples[i], testSamples.GetEnumerator());

            Assert.IsNotNull(samples);
            Assert.AreEqual(1000, samples.Count);
            Assert.AreEqual(100, samples[100]);
            Assert.AreEqual(typeof(int), samples.SampleType);

            var j = 0;
            foreach (var sample in samples)
            {
                Assert.AreEqual(j, sample);
                j++;
            }
        }
    }
}
