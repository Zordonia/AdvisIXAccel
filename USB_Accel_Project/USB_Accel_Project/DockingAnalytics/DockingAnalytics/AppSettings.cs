using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{
    static class AppSettings
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
    }
}
