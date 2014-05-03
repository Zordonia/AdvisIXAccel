using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{
    public static class AppSettings
    {
        /// <summary>
        /// The Sampling Frequency
        /// </summary>
        public static double SamplingFrequency
        {
            get
            {
                if(!sfreq.HasValue)
                {
                    int tmp = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["SamplingFrequency"], out tmp))
                    {
                        sfreq = tmp;
                    }
                }
                return sfreq.Value;
            }
            set
            {
                sfreq = value;
            }
        }
        static double? sfreq;
        
        /// <summary>
        /// How many seconds of data to save (based on sampling frequency)
        /// </summary>
        public static int SecondsToSaveData
        {
            get
            {
                if (!secondsToSave.HasValue)
                {
                    int tmp = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["SecondsToSaveData"], out tmp))
                    {
                        secondsToSave = tmp;
                    }
                }
                return secondsToSave.Value;
            }
            set
            {
                secondsToSave = value;
            }
        }
        static int? secondsToSave;

        /// <summary>
        /// How many seconds of data to graph (based on sampling frequency)
        /// </summary>
        public static int SecondsToGraphData
        {
            get
            {
                if (!secondsToGraph.HasValue)
                {
                    int tmp = 0;
                    if (int.TryParse(ConfigurationManager.AppSettings["SecondsToGraphData"], out tmp))
                    {
                        secondsToGraph = tmp;
                    }
                }
                return secondsToGraph.Value;
            }
            set
            {
                secondsToGraph = value;
            }
        }
        static int? secondsToGraph;
            
        
        /// <summary>
        /// The Graph Resolution Represents how many data points to skip before plotting.
        /// </summary>
        public static uint ResolutionOfGraph
        {
            get
            {
                if (!graphResolution.HasValue)
                {
                    uint tmp = 0;
                    if (uint.TryParse(ConfigurationManager.AppSettings["ResolutionOfGraph"], out tmp))
                    {
                        graphResolution = tmp;
                    }
                }
                return graphResolution.Value;
            }
            set
            {
                graphResolution = value;
            }
        }
        static uint? graphResolution;

        /// <summary>
        /// The high gain value.
        /// </summary>
        public static double HighGain
        {
            get
            {
                getAppSetting("HighGain", ref highGain);
                return highGain.Value;
            }
            set
            {
                highGain = value;
            }
        }
        static double? highGain;

        /// <summary>
        /// The low gain value.
        /// </summary>
        public static double LowGain
        {
            get
            {
                getAppSetting("LowGain", ref lowGain);
                return lowGain.Value;
            }
            set
            {
                lowGain = value;
            }
        }
        static double? lowGain;

        /// <summary>
        /// The sensor sensitivity.
        /// </summary>
        public static double SensorSensitivity
        {
            get
            {
                getAppSetting("SensorSensitivity", ref sSensitivity);
                return sSensitivity.Value;
            }
            set
            {
                sSensitivity = value;
            }
        }
        static double? sSensitivity;

        /// <summary>
        /// ADC Full Scale Range
        /// </summary>
        public static double ADCFullScaleRange
        {
            get
            {
                getAppSetting("ADCFullScaleRange", ref adcFullScaleRange);
                return adcFullScaleRange.Value;
            }
            set
            {
                adcFullScaleRange = value;
            }
        }
        static double? adcFullScaleRange;

        /// <summary>
        /// Feedback Capacitance
        /// </summary>
        public static double FeedbackCapacitance
        {
            get
            {
                getAppSetting("FeedbackCapacitance", ref feedbackCapacitance);
                return feedbackCapacitance.Value;
            }
            set
            {
                feedbackCapacitance = value;
            }
        }
        static double? feedbackCapacitance;

        /// <summary>
        /// The number of bits.
        /// </summary>
        public static double NumberOfBits
        {
            get
            {
                getAppSetting("NumberOfBits", ref numBits);
                return numBits.Value;
            }
            set
            {
                numBits = value;
            }
        }
        static double? numBits;

        static void getAppSetting(String appSetting, ref double? value)
        {
            double val;
            if (!value.HasValue)
            {
                if (double.TryParse(ConfigurationManager.AppSettings[appSetting], out val))
                {
                    value = val;
                }
            }
        }
    }
}
