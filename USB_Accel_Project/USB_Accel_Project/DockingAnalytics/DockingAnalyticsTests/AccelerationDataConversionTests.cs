using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DockingAnalytics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DockingAnalyticsTests
{
    [TestClass]
    public class AccelerationDataConversionTests
    {
        [TestMethod]
        public void TestDataConversion()
        {
            int value = 1000;
            double hgValue = value.ConvertToGs(InformationHolder.GainType.HighGain);
            Assert.AreEqual(-1.0, hgValue);
            
            double lgValue = value.ConvertToGs(InformationHolder.GainType.HighGain);
            Assert.AreEqual(-1.0, lgValue);
        }
    }
}
