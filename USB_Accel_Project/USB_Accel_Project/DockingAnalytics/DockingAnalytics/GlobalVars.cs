using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DockingAnalytics
{
    public static class GlobalVars
    {
        public static Scale DEFAULTSCALE(CurveType t) { return new Scale(t); }
        public static int GraphNum { get; set; }
        /// <summary>
        /// accel_Freq updated as of 06.06.12
        /// </summary>
        private static double accel_Freq = 42438.55; //44000;// 16500; // Was 44000 (Changed according to clock cycle time)
        private static double acc_YScale = (1.5 * 2) / 4096; // To get to v's
        private static double acc_YOffset = ((1.5 * 100 * 2) / 4096) * 4096 / 2; // To get offset
        private static double max_res_freq = 10000;
        public static double AccelFreq { get { return accel_Freq; } }
        public static double AccelYScale { get { return acc_YScale; } }
        public static double AccelYOffset { get { return acc_YOffset; } }
        public static double MaxResFreq { get { return max_res_freq; } }

        private static double X_MAX = 0;
        private static double X_ACTMAX = 200;
        private static double Y_MAX = 0;
        private static double Y_ACTMAX = 25;
        private static double Z_MAX = 5;
        private static double Z_MIN = 0;
        private static double Z_ACTMAX = 30;
        private static double NUM_POINTS = 0;
        private static double Y_STEP = 0;
        private static double Z_WIDTH = 5;
        private static double Y_MIN = 0;
        private static int _3DGRAPHSTEPS = 40;
        private static double _SINGLESTEP3D = 1;
        private static double _FIVESTEP3D = 5;
        private static bool _SCALECHANGED = true;
        private static string[] accelScales = new string[] { "m/s²", "cm/s²", "ft/s²", "km/hr/s", "mi/hr/s", "g", "in/s²","volts" };
        private static string[] velocScales = new string[] { "m/s", "cm/s", "mm/s", "mph", "ft/s", "ft/hr", "in/s", "km/h" };
        private static string[] powerScales = new string[] { "volts" };
        private static string[] magnScales = new string[] { "volts" };

        public static double ZMAX { get { return Z_MAX; } set { Z_MAX = value; } }
        public static double ZMIN { get { return Z_MIN; } set { Z_MIN = value; } }
        public static double ZACTMAX { get { return Z_ACTMAX; } set { Z_ACTMAX = value; } }

        public static string[] AccelerationScales { get { return accelScales; } }
        public static string[] VelocityScales { get { return velocScales; } }
        public static string[] MagnitudeScales { get { return magnScales; } }
        public static string[] PowerScales { get { return powerScales; } }

        public static bool ScaleChanged { get { return _SCALECHANGED; } set { _SCALECHANGED = value; } }

        public static int GraphSteps3d
        {
            get
            {
                return _3DGRAPHSTEPS;
            }
            set
            {
                _3DGRAPHSTEPS = value;
            }
        }

        public static double SingleStep3d
        {
            get
            {
                return _SINGLESTEP3D;
            }
            set
            {
                _SINGLESTEP3D = value;
            }
        }

        public static double FiveStep3d
        {
            get
            {
                return _FIVESTEP3D;
            }
            set
            {
                _FIVESTEP3D = value;
            }
        }

        public static double XMAX
        {
            get { return X_MAX; }
            set
            {
                X_MAX = value;
                if (value > GlobalVars.MaxResFreq)
                {
                    X_MAX = GlobalVars.MaxResFreq;
                }
                _SCALECHANGED = true;
            }
        }

        public static double XACTMAX { get { return X_ACTMAX; } set { X_ACTMAX = value; } }

        public static double YMAX
        {
            get { return Y_MAX; }
            set
            {
                Y_MAX = value;
            }
        }

        public static double YMIN
        {
            get { return Y_MIN; }
            set
            {
                Y_MIN = value;
            }
        }
        public static double YACTMAX { get { return Y_ACTMAX; } set { Y_ACTMAX = value; } }
        public static double NUMPOINTS { get { return NUM_POINTS; } set { NUM_POINTS = value; } }
        public static double YSTEP { get { return Y_STEP; } set { Y_STEP = value; } }
        public static double ZWIDTH { get { return Z_WIDTH; } set { Z_WIDTH = value; } }


        public static Color GetRandomColor(int hue)
        {
            Random r = new Random();
            int red = 0, green = 0, blue = 0;
            switch (hue)
            {
                case 0: // Red Case
                    red = r.Next(180, 256);
                    green = r.Next(0, 60);
                    blue = r.Next(0, 60);
                    break;
                case 1: // Green Case
                    red = r.Next(0, 60);
                    green = r.Next(180, 256);
                    blue = r.Next(0, 60);
                    break;
                case 2: // Blue Case
                    red = r.Next(0, 60);
                    green = r.Next(0, 60);
                    blue = r.Next(180, 256);
                    break;
                default: // Default Case
                    red = r.Next(100, 160);
                    green = r.Next(100, 160);
                    blue = r.Next(100, 160);
                    break;
            }
            Color color = Color.FromArgb(red, green, blue);
            return color;
        }

    }
}
