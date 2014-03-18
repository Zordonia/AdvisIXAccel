using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ZedGraph;

namespace DockingAnalytics
{
    public struct GraphRange
    {
        public double XMAX { get; set; }
        public double XMIN { get; set; }
        public double YMAX { get; set; }
        public double YMIN { get; set; }


        public GraphRange(double xmi, double xma, double ymi, double yma) :this()
        {
            XMAX = xma;
            XMIN = xmi;
            YMAX = yma;
            YMIN = ymi;
        }
    }
    public class Scale
    {
        private double _origY = 0;
        public double YOFF { get; set; }
        public double Y { get; set; }
        public double X { get; set; }
        public double YOffset { get; set; }

        public String ScaleString { get; set; }

        public Scale()
        {
            X = 1;
            Y = 1;
            YOffset = 0;
            _origY = Y;
        }

        public Scale(double x, double y)//:this()
        {
            X = x;
            Y = y;
            YOffset = 0;
            _origY = Y;
        }

        public Scale(CurveType t)// :this()
        {
            X = 1;
            Y = 1;
            _origY = Y;
            YOffset = 0;
            switch (t)
            {
                case CurveType.Acceleration:
                    ScaleString = "g";
                    break;
                case CurveType.Velocity:
                    ScaleString = "in/s";
                    break;
                case CurveType.Power:
                    ScaleString = "volts";
                    break;
                case CurveType.Magnitude:
                    ScaleString = "volts";
                    break;
            }
        }

        public void ChangeYScale(String scale, CurveType type)
        {
            YOFF = 0;
            ScaleString = scale;
            double m = Y;
            if (type == CurveType.Acceleration)
            {
                switch (scale)
                {
                    case "m/s^2":
                        Y = 9.807/ m; // 9.807 m/s^2 = 1.0 g
                        break;
                    case "cm/s^2":
                        Y = 980.7/ m; // 980.7 cm/s^2 = 1.0 g
                        break;
                    case "ft/s^2":
                        Y = 32.17/ m; // 32.17 ft/s^2 = 1.0 g
                        break;
                    case "km/hr/s":
                        Y = 35.3/ m; // 35.3 km/hr/s = 1.0 g
                        break;
                    case "mi/hr/s":
                        Y = 21.94/ m; // 21.94 mi/hr/s = 1.0 g
                        break;
                    case "g":
                        Y = 1.0/ m; // 1.0 g = 1.0 g
                        break;
                    case "in/s^2":
                        Y = 386.1/ m; // 386.1 in/s^2 = 1.0 g 
                        break;
                    case "volts":
                        Y = 1 / _origY;
                        YOFF = YOffset;
                        break;
                    default:
                        //Console.WriteLine("Invalid input to Acceleration Unit.");
                        break;
                }
            }
            else if (type == CurveType.Velocity)
            {
                switch (scale)
                {
                    case "m/s":
                        Y = 0.0254; // 0.0254 m/s = 1.0 in/s
                        break;
                    case "cm/s":
                        Y = 2.54; // 2.54 cm/s = 1.0 in/s
                        break;
                    case "mm/s":
                        Y = 25.4; // 25.4 mm/s = 1.0 in/s
                        break;
                    case "mph":
                        Y = 0.05682; // 0.05682 mph = 1.0 in/s
                        break;
                    case "ft/s":
                        Y = 0.08333; // 0.08333 ft/s = 1.0 in/s
                        break;
                    case "ft/h":
                        Y = 300.0; // 300 ft/h = 1 in/s
                        break;
                    case "in/s":
                        Y = 1.0; // 1 in/s = 1 in/s
                        break;
                    case "km/h":
                        Y = 0.09144; // 0.09144 km/h = 1.0 in/s
                        break;
                    default:
                        //Console.WriteLine("Invalid input to Acceleration Unit.");
                        break;
                }
            }
        }
    }

    public class CurveInfo
    {
        public double Average { get; set; }
        public double RMS { get; set; }
        public double RMSMax {get;set;}
        public double RMSPref { get; set; }
        public CurveType Type { get; set; }
        public String FileName { get; set; }
        public Color Color { get; set; }
        public List<decimal> Values { get; set; }
        public LineItem Curve { get; set; }
        public Scale Scale { get; set; }
        public GraphRange Range { get; set; }
        public PointPairList PointPairList { get; set; }
        public List<BandObj> BandObjList { get; set; }


        public CurveInfo()
        {
            // B4 Inherited class is constructed
        }

        public CurveInfo(PointPairList ppl, double xScale, double yScale, int hue, double yOff)
        {
            this.PointPairList = new PointPairList();
            this.PointPairList = ppl;
            this.Scale = new Scale(xScale, yScale);
            this.Scale.YOffset = yOff;
            this.Color = GetRandomColor(hue);
            this.RMS = calculateRMS(ppl);
        }

        public CurveInfo(PointPairList ppl, double xScale, double yScale, int hue)
        {
            this.PointPairList = new PointPairList();
            //if (yScale != 1)
            //{
            //    foreach (PointPair pp in ppl)
            //    {
            //        this.PointPairList.Add(new PointPair(pp.X * xScale, pp.Y * yScale));
            //    }
            //}
            //else
            //{
                this.PointPairList = ppl;
            //}
            this.Scale = new Scale(xScale, yScale);
            this.Color = GetRandomColor(hue);
            this.RMS = calculateRMS(ppl);
        }


        /// <summary>
        /// Calculates the RMS of a list of values.
        /// </summary>
        /// <param name="values">The values which we should take the RMS of.</param>
        /// <returns>The value of the RMS of the particular values.</returns>
        private double calculateRMS(PointPairList values)
        {

            // For each velocity data point square it.
            double squareAccumulator = 0.0;
            for (int i = 0; i < values.Count - 1; i++)
            {
                squareAccumulator = values[i].Y * values[i].Y + squareAccumulator;
            }
            // Find the average of the data series.
            double averageOfSeries = squareAccumulator / values.Count;
            // Take the square root of this average.
            RMS = Math.Sqrt(averageOfSeries);
            RMSMax = new Random().Next((int)RMS, (int)(RMS * 1.25));
            RMSPref = new Random().Next((int)RMS, (int)(RMS * 1.1));
            return RMS;
        }

        public GraphDock CreateGraphDock()
        {
            GraphDock gd = new GraphDock();
            gd.GraphName = FileName +" "+ Type.ToString();
            gd.AddCurve(this, FileName);
            return gd;
        }

        public void CreateGraphComponents()
        {
            List<decimal> myVals = new List<decimal>();
            double xMa = double.MinValue, xMi = double.MaxValue, yMa = double.MinValue, yMi = double.MaxValue;
            Curve = new GraphPane().AddCurve(this.Type + this.FileName, PointPairList, this.Color);
            foreach (PointPair pp in PointPairList)
            {
                if (pp.X < xMi) { xMi = pp.X; }
                if (pp.Y < yMi) { yMi = pp.Y; }
                if (pp.Y > yMa) { yMa = pp.Y; }
                if (pp.X > xMa) { xMa = pp.X; }
                myVals.Add((decimal)pp.Y);
            }
            Values = myVals;
            Range = new GraphRange(xMi, xMa, yMi, yMa);
        }

        public Color GetRandomColor(int hue)
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
