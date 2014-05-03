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
            Assert.AreEqual(1, AppSettings.HighGain);
            Assert.AreEqual(2, AppSettings.LowGain);
            Assert.AreEqual(3, AppSettings.SensorSensitivity);
            Assert.AreEqual(4, AppSettings.ADCFullScaleRange);
            Assert.AreEqual(5, AppSettings.FeedbackCapacitance);
            Assert.AreEqual(6, AppSettings.NumberOfBits);
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
