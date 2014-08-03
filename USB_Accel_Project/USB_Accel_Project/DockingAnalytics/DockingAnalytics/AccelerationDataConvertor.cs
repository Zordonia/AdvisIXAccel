using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DockingAnalytics;

namespace DockingAnalytics
{
    public static class AccelerationDataConvertor
    {
        public static double ConvertToGs(this int value, InformationHolder.GainType gain)
        {
            // ADC Value
            var vADC = AppSettings.LSBValue * value;
            // gF Value
            double gfValue = double.MinValue;
            switch (gain)
            {
                case InformationHolder.GainType.HighGain:
                    gfValue = AppSettings.HighGain;
                    break;
                default: // Default case should never be hit
                    throw new ArgumentNullException();
                case InformationHolder.GainType.LowGain:
                    gfValue = AppSettings.LowGain;
                    break;
            }
            // VSensor
            var vSensor = vADC / gfValue / (AppSettings.SensorCapacitance / AppSettings.FeedbackCapacitance);

            // gADC
            //var gADC = vSensor / AppSettings.SensorSensitivity;
            var gADC = vSensor;
            return gADC;
        }
    }
}
