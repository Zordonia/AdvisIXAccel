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
        public static double SecondsToSaveData
        {
            get
            {
                if (!secondsToSave.HasValue)
                {
                    double tmp = 0;
                    if (double.TryParse(ConfigurationManager.AppSettings["SecondsToSaveData"], out tmp))
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
        static double? secondsToSave;

        /// <summary>
        /// How many seconds of data to graph (based on sampling frequency)
        /// </summary>
        public static double SecondsToGraphData
        {
            get
            {
                if (!secondsToGraph.HasValue)
                {
                    double tmp = 0;
                    if (double.TryParse(ConfigurationManager.AppSettings["SecondsToGraphData"], out tmp))
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
        static double? secondsToGraph;
            
        
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
                getAppSettingDouble("HighGain", ref highGain);
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
                getAppSettingDouble("LowGain", ref lowGain);
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
                getAppSettingDouble("SensorSensitivity", ref sSensitivity);
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
                getAppSettingDouble("ADCFullScaleRange", ref adcFullScaleRange);
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
                getAppSettingDouble("FeedbackCapacitance", ref feedbackCapacitance);
                return feedbackCapacitance.Value;
            }
            set
            {
                feedbackCapacitance = value;
            }
        }
        static double? feedbackCapacitance;

        public static double SensorCapacitance
        {
            get
            {
                getAppSettingDouble("SensorCapacitance", ref sensorCapacitance);
                return sensorCapacitance.Value;
            }
            set
            {
                sensorCapacitance = value;
            }
        }
        static double? sensorCapacitance;

        /// <summary>
        /// The number of bits.
        /// </summary>
        public static double NumberOfBits
        {
            get
            {
                getAppSettingDouble("NumberOfBits", ref numBits);
                return numBits.Value;
            }
            set
            {
                numBits = value;
            }
        }
        static double? numBits;
        
        /// <summary>
        /// The LSB value.
        /// </summary>
        public static double LSBValue
        {
            get
            {
                getAppSettingDouble("LSBValue", ref lsbValue);
                return lsbValue.Value;
            }
            set
            {
                lsbValue = value;
            }
        }
        static double? lsbValue;

        /// <summary>
        /// The First Channels Offset value.
        /// </summary>
        public static int ChannelOneOffset
        {
            get
            {
                getAppSettingInt("ChannelOneOffset", ref channelOneOffset);
                return channelOneOffset.Value;
            }
            set
            {
                channelOneOffset = value;
            }
        }
        static int? channelOneOffset;
        
        /// <summary>
        /// The Second Channels Offset value.
        /// </summary>
        public static int ChannelTwoOffset
        {
            get
            {
                getAppSettingInt("ChannelTwoOffset", ref channelTwoOffset);
                return channelTwoOffset.Value;
            }
            set
            {
                channelTwoOffset = value;
            }
        }
        static int? channelTwoOffset;

        /// <summary>
        /// The First Channels Header value.
        /// </summary>
        public static int ChannelOneHeader
        {
            get
            {
                getAppSettingInt("ChannelOneHeader", ref channelOneHeader);
                return channelOneHeader.Value;
            }
            set
            {
                channelOneHeader = value;
            }
        }
        static int? channelOneHeader;
        
        /// <summary>
        /// The First Channels Header value.
        /// </summary>
        public static int ChannelTwoHeader
        {
            get
            {
                getAppSettingInt("ChannelTwoHeader", ref channelTwoHeader);
                return channelTwoHeader.Value;
            }
            set
            {
                channelTwoHeader = value;
            }
        }
        static int? channelTwoHeader;
        
        /// <summary>
        /// The Initial Header value.
        /// </summary>
        public static int InitialHeader
        {
            get
            {
                getAppSettingInt("InitialHeader", ref initHeader);
                return initHeader.Value;
            }
            set
            {
                initHeader = value;
            }
        }
        static int? initHeader;

        static void getAppSettingDouble(String appSetting, ref double? value)
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
        
        static void getAppSettingInt(String appSetting, ref int? value)
        {
            int val;
            if (!value.HasValue)
            {
                if (int.TryParse(ConfigurationManager.AppSettings[appSetting], out val))
                {
                    value = val;
                }
            }
        }
    }
}
