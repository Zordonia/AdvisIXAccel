using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DockingAnalytics
{
    [TestClass]
    public class AppSettingsTest
    {
        [TestMethod]
        public void TestAppSettings()
        {
            testValuesSet();
            testValuesCanChange();
        }

        private void testValuesSet()
        {
            Assert.AreEqual((uint)10, AppSettings.ResolutionOfGraph);
            Assert.AreEqual(42438, AppSettings.SamplingFrequency);
            Assert.AreEqual(1, AppSettings.SecondsToSaveData);
            Assert.AreEqual(10, AppSettings.SecondsToGraphData);
            Assert.AreEqual(250, AppSettings.HighGain);
            Assert.AreEqual(0.238, AppSettings.LowGain);
            Assert.AreEqual(100, AppSettings.SensorSensitivity);
            Assert.AreEqual(3.9, AppSettings.ADCFullScaleRange);
            Assert.AreEqual(100000, AppSettings.FeedbackCapacitance);
            Assert.AreEqual(16, AppSettings.NumberOfBits);
        }

        private void testValuesCanChange()
        {
            var currVal = AppSettings.HighGain;
            AppSettings.HighGain = currVal + 10.0;
            Assert.AreNotEqual(currVal, AppSettings.HighGain);

            var initVal = AppSettings.ResolutionOfGraph;
            AppSettings.ResolutionOfGraph = initVal + 10;
            Assert.AreNotEqual(initVal, AppSettings.ResolutionOfGraph);

        }
    }
}
