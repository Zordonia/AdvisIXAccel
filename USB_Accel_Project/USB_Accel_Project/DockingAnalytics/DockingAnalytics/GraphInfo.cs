using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace DockingAnalytics
{
    public class CurveItemTypePair
    {
        public CurveItem Curve { get; set; }
        public CurveObject CurveObj { get; set; }
        public CurveType Type { get; set; }
        public String Name { get; set; }
        public Scale Scale { get; set; }
        public int NCurves { get; set; }
        public CurrentCoordinates CurrCoordinates { get; set; }
        public int XIndex { get; private set; }
        public double XMax { get; private set; }
        public double XMin { get; private set; }
        public double YMax { get; private set; }
        public double YMin { get; private set; }
        public bool SymbolsOn { get; set; }
        public bool DrawCursorLines { get; set; }
        public bool DrawCursorDot { get; set; }
        private LineObj _hCursorLine = new LineObj();
        private LineObj _vCursorLine = new LineObj();
        public LineObj HorizontalCursorLine
        {
            get
            {
                return _hCursorLine;
            }
            set
            {
                this.PrevHCursorLine = _hCursorLine;
                _hCursorLine = value;
            }
        }
        public LineObj VerticalCursorLine
        {
            get
            {
                return _vCursorLine;
            }
            set
            {
                this.PrevVCursorLine = _vCursorLine;
                _vCursorLine = value;
            }
        }
        public LineObj PrevHCursorLine { get; private set; }
        public LineObj PrevVCursorLine { get; private set; }

        public LineItem PreviousCursorDot { get; private set; }
        private LineItem _cursorDot;
        public LineItem CursorDot
        {
            get
            {
                return _cursorDot;
            }
            set
            {
                PreviousCursorDot = _cursorDot;
                _cursorDot = value;
            }
        }

        public CurveItemTypePair(CurveItem curve, CurveType type, String name, int nC)
        {
            Curve = curve;
            Type = type;
            Scale = GlobalVars.DEFAULTSCALE(type);
            CurrCoordinates = new CurveItemTypePair.CurrentCoordinates();
            DrawCursorDot = true;
            DrawCursorLines = true;
            HorizontalCursorLine = new LineObj();
            VerticalCursorLine = new LineObj();
            SymbolsOn = true;
            Name = name;
            ComputeInterestingValues();
            CurveObj = new CurveObject((PointPairList)curve.Points, nC);
            CurveObj.XMAX = this.XMax;
            CurveObj.YMAX = this.YMax;
            NCurves = nC;
        }

        public void ChangeScale(String scale)
        {
            Scale.ChangeYScale(scale, Type);
            PointPairList pplNew = new PointPairList();
            foreach (PointPair pp in (Curve.Points as PointPairList))
            {
                pplNew.Add(pp.X * Scale.X, pp.Y * Scale.Y-Scale.YOFF);
            }
            Curve = new GraphPane().AddCurve(Name, pplNew, Curve.Color);
            ComputeInterestingValues();
            CurveObj = new CurveObject((PointPairList)Curve.Points, NCurves);
            CurveObj.XMAX = this.XMax;
            CurveObj.YMAX = this.YMax;
        }

        public class CurrentCoordinates
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public void UpdateCoordinates(double curX, double curY)
        {
            double xStep = Curve.Points[Curve.NPts - 1].X / Curve.NPts;
            double xIndex = curX / xStep;
            int xInd = (int)xIndex;
            xInd = xInd < 0 ? 0 : xInd > Curve.NPts - 1 ? Curve.NPts - 1 : xInd;
            CurrCoordinates.Y = Curve.Points[xInd].Y;
            CurrCoordinates.X = Curve.Points[xInd].X;
            XIndex = xInd;
        }

        virtual public void GetCurveValues(ref double xMinref, ref double xMaxref, ref double yMinref, ref double yMaxref, ref double RMSValueref)
        {
            xMinref = XMin;
            xMaxref = XMax;
            yMinref = YMin;
            yMaxref = YMax;
            RMSValueref = -1;
        }
        public void ComputeInterestingValues()
        {
            double xM = Double.MinValue, yM = Double.MinValue, ymi = Double.MaxValue, xmi = Double.MaxValue;
            PointPairList ppl = (PointPairList)Curve.Points;
            foreach (PointPair pp in ppl)
            {
                if (pp.X < xmi)
                {
                    xmi = pp.X;
                }
                if (pp.X > xM)
                {
                    xM = pp.X;
                }
                if (pp.Y < ymi)
                {
                    ymi = pp.Y;
                }
                if (pp.Y > yM)
                {
                    yM = pp.Y;
                }
            }
            XMax = xM;
            XMin = xmi;
            YMax = yM;
            YMin = ymi;
            GlobalVars.XMAX = XMax;
            GlobalVars.YMAX = YMax;
            GlobalVars.YMIN = YMin;
        }
    }

    public enum GraphType
    {
        ThreeD,
        TwoD,
    }

    public enum CurveType
    {
        Acceleration,
        Velocity,
        Magnitude,
        Power,
    }

    public struct NameTypeKey
    {
        public String Name { get; set; }
        public CurveType Type { get; set; }
    }

    public class GraphInfo
    {

        public Dictionary<CurveType, int> CurveTypeCounts { get { return _curveTypeCounts; } }
        public Dictionary<CurveType, Tuple<Scale, int>> PredominantScale { get { return _predScaleCount; } }

        private Dictionary<CurveType, Tuple<Scale, int>> _predScaleCount = new Dictionary<CurveType, Tuple<Scale, int>>();

        private Dictionary<CurveType, int> _curveTypeCounts = new Dictionary<CurveType, int>();

        private List<CurveItem> CurveItems { get;set; }
        public List<CurveItemTypePair> CurveTypeItems { get; private set; }

        private CurveType? _maxCurType = null;
        private CurveType? _max2CurType = null;

        public CurveType? MaxCurveType { get { return _maxCurType; } }
        public CurveType? Max2CurveType { get { return _max2CurType; } }

        public int NCurves { get { return CurveTypeItems.Count; } }
        public String GraphName { get; private set; }
        public Scale Scale{get;private set;}

        public GraphInfo()
        {
            CurveItems = new List<CurveItem>();
            CurveTypeItems = new List<CurveItemTypePair>();
            Scale = new Scale();
        }

        private void SetPredominantCurveTypes()
        {
            int mostC = 0, sMostC = 0;

            foreach (CurveType key in _curveTypeCounts.Keys)
            {
                if (_curveTypeCounts[key] > mostC)
                {
                    mostC = _curveTypeCounts[key];
                    _maxCurType = key;
                }
                else if (_curveTypeCounts[key] > sMostC)
                {
                    sMostC = _curveTypeCounts[key];
                    _max2CurType = key;
                }
            }
        }

        public void Add(CurveItem curve, CurveType type,String name)
        {
            CurveItemTypePair citp = new CurveItemTypePair(curve, type, name, NCurves);
            CurveTypeItems.Add(citp);

            if (_curveTypeCounts.ContainsKey(type))
            {
                _curveTypeCounts[type]++;
            }
            else
            {
                _curveTypeCounts.Add(type, 1);
            }
            SetPredominantCurveTypes();
        }

        public void Insert(int Index, CurveItemTypePair citp)
        {
            CurveTypeItems.Insert(Index, citp);
            if (_curveTypeCounts.ContainsKey(citp.Type))
            {
                _curveTypeCounts[citp.Type]++;
            }
            else
            {
                _curveTypeCounts.Add(citp.Type, 1);
            }
        }

        public void Add(CurveItem curve, String name)
        {
            throw new NotImplementedException();
            // @todo: Check Scale
            CurveItems.Add(curve);
        }

        public void Remove(CurveItem curve, CurveType type)
        {
            // @todo: Recheck Scale
            CurveItems.Remove(curve);
            SetPredominantCurveTypes();
            if (_curveTypeCounts.ContainsKey(type))
            {
                if (_curveTypeCounts[type] > 1)
                {
                    _curveTypeCounts[type]--;
                }
                else
                {
                    _curveTypeCounts.Remove(type);
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
