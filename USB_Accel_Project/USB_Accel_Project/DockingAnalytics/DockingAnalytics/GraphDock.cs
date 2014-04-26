using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ZedGraph;

namespace DockingAnalytics
{
    public partial class GraphDock : Document 
    {
        //public GraphInfo GraphInformation { get; set; }
        public UpdateGraph GraphUpdateView { get; private set; }
        //public String GraphName { get; set; }
        public MenuStrip GraphMenu { get; set; }
        //private List<LineObj> prevLines = new List<LineObj>();
        

        public GraphDock() : base()
        {
            InitializeComponent();
            //GraphInformation = new GraphInfo();
            GraphUpdateView = AnalyticsController.Instance.UpdateGraphMainView;
            GraphUpdateView.Register(this);
            
            GraphMenu = new MenuStrip();
            GraphMenu.Items.Add("Close");
            GraphMenu.Items.Add("Close All But This");
            GraphMenu.Items.Add("Extract");
            //GraphMenu.Show();
            //GraphUpdateView.Show(this.DockPanel, DockState.DockBottom);
            //this.CloseButtonVisible = true;
            this.zedGraphControl1.MouseMoveEvent += new ZedGraphControl.ZedMouseEventHandler(zedGraphControl1_MouseMoveEvent);
            this.zedGraphControl1.MouseUpEvent += new ZedGraphControl.ZedMouseEventHandler(zedGraphControl1_MouseUpEvent);
            this.zedGraphControl1.MouseDownEvent += new ZedGraphControl.ZedMouseEventHandler(zedGraphControl1_MouseDownEvent);
            this.DockStateChanged += new EventHandler(GraphDock_DockStateChanged);
            this.ParentChanged += new EventHandler(GraphDock_ParentChanged);
            this.MouseUp += new MouseEventHandler(GraphDock_MouseUp);
            this.ZedGraphControl.GraphPane.Title.IsVisible = false;
            // this.ZedGraphControl.GraphPane.XAxis.Title = 
        }

        public ZedGraphControl ZedGraphControl { get { return zedGraphControl1; } }

        void GraphDock_MouseUp(object sender, MouseEventArgs e)
        {
            GraphDock gd = sender as GraphDock;
        }

        void GraphDock_ParentChanged(object sender, EventArgs e)
        {
            GraphDock gd = sender as GraphDock;
            if (gd != null)
            {
                Console.WriteLine("Dock parent changed" + gd.Text);
                if (gd.Container == null)
                {
                    Console.WriteLine("Alone22");
                }
                else
                {
                    Console.WriteLine(gd.Container.Components.Count);
                }
            }
        }

        void GraphDock_DockStateChanged(object sender, EventArgs e)
        {
            Console.WriteLine("GRAPH DOCK- DOCK STATE CHANGED");
            GraphDock gd = sender as GraphDock;
            gd.DockPanel.Tag = GlobalVars.GraphNum;
            Console.WriteLine(GlobalVars.GraphNum);
            this.DockPanel.HandleDestroyed += new EventHandler(DockPanel_HandleDestroyed);
            //if (gd.ControlRemoved)
              //  gd.Close();
            //GraphDock gd = sender as GraphDock;
            //Console.WriteLine("Dock state changed:" + gd.Text);
            //if (gd.Container == null)
            //{
            //    Console.WriteLine("Alone");
            //}
            //else
            //{
            //    Console.WriteLine(gd.DockPanel.Container.Components.Count);
            //}
        }

        void DockPanel_HandleDestroyed(object sender, EventArgs e)
        {
            Console.WriteLine("Handle Destroyed");
        }

        bool zedGraphControl1_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            return false;
        }

        bool zedGraphControl1_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            return false;
        }

        bool zedGraphControl1_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            if (myPane == null) { return true; }
            //myPane = mainPain;

            PointF mousePt = new PointF(e.X, e.Y);
            
            double curX, curY, curY2, curX2;

            myPane.ReverseTransform(mousePt, out curX, out curX2, out curY, out curY2);

            foreach (CurveItemTypePair citp in this.GraphInformation.CurveTypeItems)
            {
                double x, y = 0;
                x = citp.Curve.IsX2Axis ? curX2 : curX;
                y = citp.Curve.IsY2Axis ? curY2 : curY;
                citp.UpdateCoordinates(x, y);
                DrawCursor(citp);
            }
            this.Activate();
            //9889
            // Access update graph with this information.
            GraphUpdateView.UpdateView();
            // Tested and works properly for maintaining the current position of our mouse,
            // 
            //Console.WriteLine("Current x pos: " + curX);
            //Console.WriteLine("Current y pos: " + curY);

            // since we didn't handle the event, tell the ZedGraphControl to handle it
            return false;
        }

        public void DrawCursor(CurveItemTypePair citp)
        {
            if (citp.DrawCursorDot) { DrawCursorAsDot(citp); }
            if (citp.DrawCursorLines) { DrawCursorAsLine(citp); }
        }

        private void DrawCursorAsDot(CurveItemTypePair citp)
        {
            PointPairList cPPL = new PointPairList();
            zedGraphControl1.GraphPane.CurveList.Remove(citp.PreviousCursorDot);
            cPPL.Add(citp.Curve[citp.XIndex].X, citp.Curve[citp.XIndex].Y);
            LineItem cursorDot = zedGraphControl1.GraphPane.AddCurve("Dot", cPPL, citp.Curve.Color);
            citp.CursorDot = cursorDot;
            cursorDot.Symbol.Type = SymbolType.Circle;
            cursorDot.Symbol.Size = Symbol.Default.Size * 1.5f;
            cursorDot.Symbol.Fill.Color = cursorDot.Color;
            cursorDot.Symbol.Fill.IsVisible = true;
            cursorDot.Label.IsVisible = false;
            cursorDot.IsY2Axis = citp.Curve.IsY2Axis;
            cursorDot.IsX2Axis = citp.Curve.IsX2Axis;
        }


        private void DrawCursorAsLine(CurveItemTypePair citp)
        {
            LineObj verticalLine, horizontalLine;
            verticalLine = new LineObj(citp.CurrCoordinates.X, 0, citp.CurrCoordinates.X, 1.0);
            horizontalLine = new LineObj(0, citp.CurrCoordinates.Y, 1.0, citp.CurrCoordinates.Y);
            horizontalLine.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            if (citp.Curve.IsY2Axis)
            {
                horizontalLine.Location.CoordinateFrame = CoordType.XChartFractionY2Scale;
            }
            else
            {
                horizontalLine.Location.CoordinateFrame = CoordType.XChartFractionYScale;
            }
            verticalLine.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
            verticalLine.IsClippedToChartRect = true;
            horizontalLine.IsClippedToChartRect = true;
            verticalLine.ZOrder = ZOrder.A_InFront;
            horizontalLine.ZOrder = ZOrder.A_InFront;
            verticalLine.IsVisible = true;
            horizontalLine.IsVisible = true;
            citp.HorizontalCursorLine = horizontalLine;
            citp.VerticalCursorLine = verticalLine;
            zedGraphControl1.GraphPane.GraphObjList.Add(citp.HorizontalCursorLine);
            zedGraphControl1.GraphPane.GraphObjList.Add(citp.VerticalCursorLine);
            zedGraphControl1.GraphPane.GraphObjList.Remove(citp.PrevHCursorLine);
            zedGraphControl1.GraphPane.GraphObjList.Remove(citp.PrevVCursorLine);
        }



        public void UpdateScales()
        {
            if (this.GraphInformation.Max2CurveType != null && this.GraphInformation.Max2CurveType != this.GraphInformation.MaxCurveType)
            {
                this.zedGraphControl1.GraphPane.Y2Axis.IsVisible = true;
                var curveItem = this.GraphInformation.CurveTypeItems.First(a => a.Type == this.GraphInformation.Max2CurveType);
                this.zedGraphControl1.GraphPane.Y2Axis.Title.Text = this.GraphInformation.Max2CurveType + " in " + curveItem.Scale.ScaleString;
            }
            var curveItem2 = this.GraphInformation.CurveTypeItems.First(a => a.Type == this.GraphInformation.MaxCurveType);
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = this.GraphInformation.MaxCurveType + " in " + curveItem2.Scale.ScaleString;

            if (this.GraphInformation.MaxCurveType == CurveType.Acceleration || this.GraphInformation.MaxCurveType == CurveType.Velocity)
            {
                this.zedGraphControl1.GraphPane.XAxis.Title.Text = "Time(s)";
            }
            else
            {
                this.zedGraphControl1.GraphPane.XAxis.Title.Text = "Frequency(Hz)";
            }
        }


        new public void AddCurve(CurveInfo ci, String curveName)
        {

            //CurveObject co = new CurveObject((((PointPairList)curve.Points).ToSlice(0, 0.010).Stretch(GlobalVars.XACTMAX)), 0);
            //this.zedGraphControl1.GraphPane.AddCurve("NONE", co.PointPairList, Color.Red, SymbolType.Circle);
            this.zedGraphControl1.GraphPane.CurveList.Add(ci.Curve);
            base.AddCurve(ci, curveName);
            AddMenuItem(ci.Curve, ci.Type, curveName);

            if (ci.Type != this.GraphInformation.MaxCurveType)
            {
                ci.Curve.IsY2Axis = true;
            }
            if (SetX2Axis(ci.Type))
            {
                ci.Curve.IsX2Axis = true;
                this.zedGraphControl1.GraphPane.X2Axis.IsVisible = true;
                string scale = "Time(s)";
                if (ci.Type == CurveType.Power || ci.Type == CurveType.Magnitude)
                {
                    scale = "Frequency(Hz)";
                }
                this.zedGraphControl1.GraphPane.X2Axis.Title.Text = scale;
            }
            UpdateScales();
            this.zedGraphControl1.GraphPane.Title.Text = this.GraphName;
            this.zedGraphControl1.GraphPane.AxisChange();
            //GraphInformation.Add(curve,type);
        }

        private Tuple<bool, bool> first = new Tuple<bool, bool>(true, false);
        private bool SetX2Axis(CurveType type)
        {
            if (first.Item1)
            {
                first = new Tuple<bool, bool>(false, (type == CurveType.Acceleration || type == CurveType.Velocity));
                return false;
            }
            else
            {
                return (type == CurveType.Acceleration || type == CurveType.Velocity) != first.Item2;
            }
        }


        public void InsertCurve(int index, CurveItem curve, CurveType type, String curveName)
        {
            this.zedGraphControl1.GraphPane.CurveList.Add(curve);
            this.zedGraphControl1.GraphPane.AxisChange();
            CurveItemTypePair citp = new CurveItemTypePair(curve, type, curveName, GraphInformation.NCurves);
            DrawCursor(citp);


            base.InsertCurve(index, citp);
            UpdateScales();
        }

        public void AddMenuItem(CurveItem curve, CurveType type, String curveName)
        {
            String fN = System.IO.Path.GetFileName(curveName);
            string[] scaleArr;
            if (type.Equals(CurveType.Acceleration))
            {
                scaleArr = GlobalVars.AccelerationScales;
            }
            else
            {
                scaleArr = GlobalVars.VelocityScales;
            } 
            ToolStripMenuItem mainCurveMenuItem = new ToolStripMenuItem(fN+" "+type);
            ToolStripMenuItem colorMenuItem = new ToolStripMenuItem("Color");
            colorMenuItem.Click += new EventHandler(ChangeColor);
            ToolStripMenuItem scaleMenuItem = new ToolStripMenuItem("Scale");
            List<ToolStripItem> tsiL = new List<ToolStripItem>();
            foreach (String scale in scaleArr)
            {
                tsiL.Add(new ToolStripMenuItem(scale,null,ScaleClicked));
            }
            scaleMenuItem.DropDownItems.AddRange(tsiL.ToArray());
            ToolStripMenuItem symbolMenuItem = new ToolStripMenuItem("Symbol");
            if (type.Equals(CurveType.Magnitude) || type.Equals(CurveType.Power))
            {
                mainCurveMenuItem.DropDownItems.AddRange(new ToolStripItem[]{colorMenuItem,
                    symbolMenuItem});
            }
            else
            {
                mainCurveMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[]{colorMenuItem,
                scaleMenuItem,
                symbolMenuItem});
            }
            graphsToolStripMenuItem1.DropDownItems.Add(mainCurveMenuItem);
            symbolMenuItem.DropDownItems.AddRange(AddSymbolMenuItem());
        }

        ToolStripMenuItem[] AddSymbolMenuItem()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseDock));
            
            ToolStripMenuItem[] tsmis = new ToolStripMenuItem[12];
            for (int i = 0; i < 12; i++)
            {
                tsmis[i] = new ToolStripMenuItem();
                tsmis[i].Click +=new EventHandler(SymbolChangeClick);
            }


            tsmis[0].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem18.Image")));
            tsmis[0].Name = "toolStripMenuItem18";
            tsmis[0].Size = new System.Drawing.Size(148, 22);
            tsmis[0].Text = "Circle";

            tsmis[11].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem19.Image")));
            tsmis[11].Name = "toolStripMenuItem19";
            tsmis[11].Size = new System.Drawing.Size(148, 22);
            tsmis[11].Text = "Default";

            tsmis[1].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem37.Image")));
            tsmis[1].Name = "tsmis[1]";
            tsmis[1].Size = new System.Drawing.Size(148, 22);
            tsmis[1].Text = "Diamond";

            tsmis[2].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem80.Image")));
            tsmis[2].Name = "tsmis[2]";
            tsmis[2].Size = new System.Drawing.Size(148, 22);
            tsmis[2].Text = "HDash";

            tsmis[3].Name = "tsmis[3]";
            tsmis[3].Size = new System.Drawing.Size(148, 22);
            tsmis[3].Text = "None";

            tsmis[4].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem93.Image")));
            tsmis[4].Name = "tsmis[4]";
            tsmis[4].Size = new System.Drawing.Size(148, 22);
            tsmis[4].Text = "Plus";

            tsmis[5].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem94.Image")));
            tsmis[5].Name = "toolStripMenuItem94";
            tsmis[5].Size = new System.Drawing.Size(148, 22);
            tsmis[5].Text = "Square";

            tsmis[6].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem95.Image")));
            tsmis[6].Name = "toolStripMenuItem95";
            tsmis[6].Size = new System.Drawing.Size(148, 22);
            tsmis[6].Text = "Star";

            tsmis[7].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem96.Image")));
            tsmis[7].Name = "toolStripMenuItem96";
            tsmis[7].Size = new System.Drawing.Size(148, 22);
            tsmis[7].Text = "Triangle";

            tsmis[8].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem97.Image")));
            tsmis[8].Name = "toolStripMenuItem97";
            tsmis[8].Size = new System.Drawing.Size(148, 22);
            tsmis[8].Text = "TriangleDown";

            tsmis[9].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem98.Image")));
            tsmis[9].Name = "toolStripMenuItem98";
            tsmis[9].Size = new System.Drawing.Size(148, 22);
            tsmis[9].Text = "VDash";

            tsmis[10].Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem99.Image")));
            tsmis[10].Name = "toolStripMenuItem99";
            tsmis[10].Size = new System.Drawing.Size(148, 22);
            tsmis[10].Text = "XCross";

            return tsmis;
        }


        void ScaleClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            //tsmi = tsmi.OwnerItem as ToolStripMenuItem;
            tsmi = tsmi.OwnerItem.OwnerItem as ToolStripMenuItem;
            ToolStripMenuItem tsmiBase = tsmi.OwnerItem as ToolStripMenuItem;
            int index = tsmiBase.DropDownItems.IndexOf(tsmi) - 1;
            tsmi = sender as ToolStripMenuItem;
            ChangeScale(index, tsmi.Text);        
        }

        public override void ChangeScale(int CI_index, String scale)
        {
            zedGraphControl1.GraphPane.CurveList.Remove(GraphInformation.CurveTypeItems[CI_index].Curve);
            zedGraphControl1.GraphPane.GraphObjList.Remove(GraphInformation.CurveTypeItems[CI_index].PrevHCursorLine);
            zedGraphControl1.GraphPane.GraphObjList.Remove(GraphInformation.CurveTypeItems[CI_index].PrevVCursorLine);
            zedGraphControl1.GraphPane.CurveList.Remove(GraphInformation.CurveTypeItems[CI_index].PreviousCursorDot);
            base.ChangeScale(CI_index, scale);
            zedGraphControl1.GraphPane.CurveList.Add(GraphInformation.CurveTypeItems[CI_index].Curve);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            UpdateScales();
        }

        public override void ChangeSymbol(int CI_index, String symbol)
        {
            SymbolType type = SymbolType.Default;
            switch(symbol)
            {
                case "Circle":
                    type = SymbolType.Circle;
                    break;
                case "Default":
                    type = SymbolType.Default;
                    break;
                case "Diamond":
                    type = SymbolType.Diamond;
                    break;
                case "HDash":
                    type = SymbolType.HDash;
                    break;
                case "None":
                    type = SymbolType.None;
                    break;
                case "Plus":
                    type = SymbolType.Plus;
                    break;
                case "Square":
                    type = SymbolType.Square;
                    break;
                case "Star":
                    type = SymbolType.Star;
                    break;
                case "Triangle":
                    type = SymbolType.Triangle;
                    break;
                case "TriangleDown":
                    type = SymbolType.TriangleDown;
                    break;
                case "VDash":
                    type = SymbolType.VDash;
                    break;
                case "XCross":
                    type = SymbolType.XCross;
                    break;
            }
            zedGraphControl1.GraphPane.CurveList.Remove(GraphInformation.CurveTypeItems[CI_index].Curve);
            zedGraphControl1.GraphPane.GraphObjList.Remove(GraphInformation.CurveTypeItems[CI_index].PrevHCursorLine);
            zedGraphControl1.GraphPane.GraphObjList.Remove(GraphInformation.CurveTypeItems[CI_index].PrevVCursorLine);
            zedGraphControl1.GraphPane.CurveList.Remove(GraphInformation.CurveTypeItems[CI_index].PreviousCursorDot);
            CurveItem tempC = GraphInformation.CurveTypeItems[CI_index].Curve;
            GraphInformation.CurveTypeItems[CI_index].Curve = new GraphPane().AddCurve(tempC.Label.Text,tempC.Points, tempC.Color, type);
            zedGraphControl1.GraphPane.CurveList.Add(GraphInformation.CurveTypeItems[CI_index].Curve);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }

        void ChangeColor(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            tsmi = tsmi.OwnerItem as ToolStripMenuItem;
            ToolStripMenuItem tsmiBase = tsmi.OwnerItem as ToolStripMenuItem;
            int index = tsmiBase.DropDownItems.IndexOf(tsmi) - 1;
            System.Windows.Forms.ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                GraphInformation.CurveTypeItems[index].Curve.Color = cd.Color;
            }
        }

        public void ChangeCurveTypeColor(CurveType? type, Color color)
        {
            foreach (CurveItemTypePair citp in GraphInformation.CurveTypeItems)
            {
                if (citp.Type == type)
                {
                    citp.Curve.Color = color;
                }
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }

        new public void AddCurve(CurveItem curve, String curveName)
        {
            throw new NotImplementedException();
            this.zedGraphControl1.GraphPane.CurveList.Add(curve);
            this.zedGraphControl1.GraphPane.AxisChange();

            base.AddCurve(curve, curveName);
            //GraphInformation.Add(curve);
        }

        new public void RemoveCurve(CurveItem curve, CurveType type,  String curveName)
        {
            this.zedGraphControl1.GraphPane.CurveList.Remove(curve);

            base.RemoveCurve(curve,type,  curveName);
            //GraphInformation.Remove(curve);
        }


        private void SymbolChangeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            //tsmi = tsmi.OwnerItem as ToolStripMenuItem;
            tsmi = tsmi.OwnerItem.OwnerItem as ToolStripMenuItem;
            ToolStripMenuItem tsmiBase = tsmi.OwnerItem as ToolStripMenuItem;
            int index = tsmiBase.DropDownItems.IndexOf(tsmi) - 1;
            tsmi = sender as ToolStripMenuItem;
            ChangeSymbol(index, tsmi.Text); 
        }

        public void AddData(PointPairList list)
        {

        }

        public delegate void UpdateZedgraphDelegate(PointPairList data);
        public void UpdateZedGraphThreadSafe(PointPairList data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateZedgraphDelegate(this.UpdateZedGraphThreadSafe), data);
            }
            else
            {
                this.zedGraphControl1.GraphPane.AxisChange();

                double min = Double.MaxValue;
                double max = Double.MinValue;

                CurveItem curve = this.zedGraphControl1.GraphPane.CurveList[0];
                if (data.Count > this.zedGraphControl1.GraphPane.XAxis.Scale.Max)
                {
                    this.zedGraphControl1.GraphPane.XAxis.Scale.Max = data.Count + 250000;
                }

                this.zedGraphControl1.Update();
                this.Refresh();
            }
        }

        public void Update(BaseDock bd)
        {
            if (this.IsFloat)
            {
                if (this.Pane != null)
                {
                    Console.WriteLine("is Float");
                    GraphUpdateView = new UpdateGraph();
                    GraphUpdateView.Register(this);
                    GraphUpdateView.UpdateView();
                    GraphUpdateView.Show(Pane, DockAlignment.Bottom, 0.25);
                }
            }
            else
            {
                Console.WriteLine("is !Float");
                GraphUpdateView.Close();
                GraphUpdateView = AnalyticsController.Instance.UpdateGraphMainView;
                bd.Update(this, DockState.Document);
            }
        }

        private void GraphDock_FormClosing(object sender, FormClosingEventArgs e)
        {
            GraphDock gd = sender as GraphDock;
            Console.WriteLine(gd.ToString());
            if(AnalyticsController.Instance.isReading)
                AnalyticsController.Instance.USBAnalytics_StopUSB();


        }
    }
}
