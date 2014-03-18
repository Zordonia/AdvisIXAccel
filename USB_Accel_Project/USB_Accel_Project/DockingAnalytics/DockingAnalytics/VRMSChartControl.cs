using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public partial class VRMSChartControl : UserControl
    {
        static double[] inps = new double[] { 0.01, 0.02, 0.03, 0.04, 0.07, 0.11, 0.18, 0.28, 0.44, 0.70, 0.71, 1.10 };
        static double[] mmps = new double[] { 0.28, 0.45, 0.71, 1.12, 1.80, 2.80, 4.50, 7.10, 11.2, 18.0, 28.0, 45.0 };
        static string[] classTitles = new string[] { "Class I" + Environment.NewLine + "small" + Environment.NewLine + "machines" ,
                                                          "Class II" + Environment.NewLine + "medium" + Environment.NewLine + "machines" ,
                                                          "Class III" + Environment.NewLine + "large rigid" + Environment.NewLine + "foundation" ,
                                                          "Class IV" + Environment.NewLine + "large soft" + Environment.NewLine + "foundation" };
        public enum MachineClass
        {
            ClassI,
            ClassII,
            ClassIII,
            ClassIV,
            Undefined
        }
        public class MachineClassRMSValues
        {
            public bool ISMMS { get; set; }
            public String FileName { get; set; }
            private Color _myColor = GlobalVars.GetRandomColor(2);
            public Color Color { get { return _myColor; } }
            private Point _loc = new Point(0, 0);
            public Point Location { get { return _loc; } set { _loc = value; } }
            public MachineClass Class { get; set; }
            double _mmps = 0.28;
            public double MMPS
            {
                get
                {
                    return _mmps;
                }
                set
                {
                    _mmps = value <= 50 && value >= 0.28 ? value : -1;
                    _mmps = value;
                    if (_mmps < 0)
                    {
                        return;
                        throw new ArgumentOutOfRangeException("MMPS", value, "Value must fall in range from 0 to 50");
                    }
                    _rowIndex = 0;
                    for (int i = 0; i < mmps.Length - 1; i++)
                    {
                        if (_mmps >= mmps[i] && _mmps < mmps[i + 1])
                        {
                            _rowIndex = i;
                        }
                    }
                }
            }
            double _inps = 0.0;
            public double INPS
            {
                get
                {
                    return _inps;
                }
                set
                {
                    _inps = value <= 2 && value >= 0.01 ? value : -1;
                    _inps = value;
                    if (_inps < 0)
                    {
                        return;
                        throw new ArgumentOutOfRangeException("INPS", value, "Value must fall in range from 0 to 2");
                    }
                    _rowIndex = 0;
                    for (int i = 0; i < inps.Length - 1; i++)
                    {
                        if (_inps >= inps[i] && _inps < inps[i + 1])
                        {
                            _rowIndex = i;
                            return;
                        }
                    }
                }
            }
            int _rowIndex;
            public int RowIndex { get { return _rowIndex; } }
        }

        public ObservableCollection<MachineClassRMSValues> RMSList = new ObservableCollection<MachineClassRMSValues>();

        bool[,] pixels;
        public VRMSChartControl()
        {
            InitializeComponent();
            RMSList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(RMSList_CollectionChanged);
            //RMSList.Add(new MachineClassRMSValues { MMPS = 4.5, Class = MachineClass.ClassIII, FileName = " MMPS = 4.5, Class = MachineClass.ClassIII" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 44.5, Class = MachineClass.ClassIV, FileName = "MMPS = 44.5, Class = MachineClass.ClassIV" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 12.55, Class = MachineClass.ClassI, FileName = "MMPS = 12.55, Class = MachineClass.ClassI" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 2.5, Class = MachineClass.ClassII, FileName = "2.5, Class = MachineClass.ClassII" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 8.75, Class = MachineClass.ClassIV, FileName = "8.75, Class = MachineClass.ClassIV" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 12.5, Class = MachineClass.ClassIII, FileName = "12.5, Class = MachineClass.ClassIII" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 15.4, Class = MachineClass.ClassIII, FileName = "15.4, Class = MachineClass.ClassIII" });
            //RMSList.Add(new MachineClassRMSValues { MMPS = 15.4, Class = MachineClass.ClassIII, FileName = "14.35, Class = MachineClass.ClassIII" });
            pixels = new bool[this.Width, this.Height];
        }

        void RMSList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Invalidate();
        }


        private void VRMSChartControl_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                pixels = new bool[this.Width, this.Height];
                int x = this.Width / 18;
                int y = this.Height / 4;
                foreach (double d in inps)
                {
                    string drawString = d.ToString().PadRight(4, '0');
                    System.Drawing.Font drawFont = new System.Drawing.Font(
                        "Arial", (int)(Math.Min(this.Height / 24, this.Width / 32)));
                    System.Drawing.SolidBrush drawBrush = new
                        System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    e.Graphics.DrawString(drawString, drawFont, drawBrush, x, y);
                    drawFont.Dispose();
                    drawBrush.Dispose();
                    y += (int)(Math.Ceiling(this.Height / 16.0));
                }
                x = this.Width * 11 / 72;
                y = this.Height / 4;
                foreach (double d in mmps)
                {
                    string drawString = d.ToString("0.0#").PadRight(4, '0');
                    System.Drawing.Font drawFont = new System.Drawing.Font(
                        "Times", (int)(Math.Min(this.Height / 24, this.Width / 32)));
                    System.Drawing.SolidBrush drawBrush = new
                        System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    e.Graphics.DrawString(drawString, drawFont, drawBrush, x, y);
                    drawFont.Dispose();
                    drawBrush.Dispose();
                    y += (int)(Math.Ceiling(this.Height / 16.0));
                }
                using (Brush b = new SolidBrush(Color.Green))
                {
                    e.Graphics.FillPolygon(b, new Point[] {new Point(this.Width/4, this.Height/4), 
                    new Point(this.Width/4, this.Height/4 + 3*this.Height/16),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 + (int)(Math.Ceiling(3*this.Height/16.0))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 4*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)4*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)5*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)5*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)6*this.Height/16  ))),
                    new Point(this.Width, this.Height/4 + (int)(Math.Ceiling((double)6*this.Height/16  ))),
                    new Point(this.Width, this.Height/4)});
                }
                using (Brush b = new SolidBrush(Color.Yellow))
                {
                    e.Graphics.FillPolygon(b, new Point[] {
                    new Point(this.Width/4, this.Height/4 + (int)(Math.Ceiling((double)3*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)3*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)4*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)4*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)5*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)5*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 6*this.Height/16  ))),
                    new Point(this.Width, this.Height/4 + (int)(Math.Ceiling((double)6*this.Height/16  ))),
                    new Point(this.Width, this.Height/4 + (int)(Math.Ceiling((double)10*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)10*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)9*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 9*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)8*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)8*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 7*this.Height/16  ))),
                    new Point(this.Width/4 , this.Height/4 +(int)(Math.Ceiling((double) 7*this.Height/16  )))});
                }
                using (Brush b = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillPolygon(b, new Point[] {
                    new Point(this.Width/4, this.Height/4 +(int)(Math.Ceiling((double) 12*this.Height/16  ))),
                    new Point(this.Width, this.Height/4 + (int)(Math.Ceiling((double)12*this.Height/16  ))),
                    new Point(this.Width, this.Height/4 + (int)(Math.Ceiling((double)10*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)10*this.Height/16  ))),
                    new Point(this.Width/4 + 3* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)9*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 + (int)(Math.Ceiling((double)9*this.Height/16  ))),
                    new Point(this.Width/4 + 2* this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 8*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 8*this.Height/16  ))),
                    new Point(this.Width/4 + this.Width/5, this.Height/4 +(int)(Math.Ceiling((double) 7*this.Height/16  ))),
                    new Point(this.Width/4 , this.Height/4 + (int)(Math.Ceiling((double)7*this.Height/16  )))});
                }
                int pSize = (int)(Math.Min(this.Height / 200.0, this.Width / 300.0));
                //_rowIndex = MyClass == MachineClass.Undefined ? -1 : _rowIndex;
                using (Pen p = new Pen(Color.Black, pSize))
                {
                    Pen pen = new Pen(Color.Blue, pSize * 3);
                    //if (_rowIndex == 0)
                    //{
                    //    e.Graphics.DrawLine(pen, new Point(this.Width / 18, this.Height / 4), new Point(this.Width, this.Height / 4));
                    //}
                    //else
                    {
                        e.Graphics.DrawLine(p, new Point(0, this.Height / 4), new Point(this.Width, this.Height / 4));
                    }
                    int rIndex = 0;
                    for (int j = this.Height / 4 + (int)(Math.Ceiling(this.Height / 16.0)); j < this.Height; j += (int)(Math.Ceiling(this.Height / 16.0)))
                    {
                        //if (rIndex == _rowIndex - 1 || rIndex == _rowIndex)
                        //{
                        //    e.Graphics.DrawLine(pen, new Point(this.Width / 18, j), new Point(this.Width, j));
                        //}
                        //else
                        {
                            e.Graphics.DrawLine(p, new Point(this.Width / 18, j), new Point(this.Width, j));
                        }
                        rIndex++;
                    }
                    //int classIndex = MyClass == MachineClass.ClassI ? 0 : MyClass == MachineClass.ClassII ? 1 : MyClass == MachineClass.ClassIII ? 2 : MyClass == MachineClass.ClassIV ? 3 : 4;
                    int cIndex = 0;
                    for (int i = this.Width / 4; i < this.Width; i += this.Width / 5)
                    {
                        //if (classIndex == cIndex || classIndex + 1 == cIndex)
                        //{
                        //    e.Graphics.DrawLine(pen, new Point(i, 0), new Point(i, this.Height));
                        //}
                        //else
                        {
                            e.Graphics.DrawLine(p, new Point(i, 0), new Point(i, this.Height));
                        }
                        cIndex++;
                    }
                    e.Graphics.DrawLine(p, new Point(0, this.Height / 16), new Point(this.Width / 4, this.Height / 16));
                    e.Graphics.DrawLine(p, new Point(this.Width / 18, this.Height / 4), new Point(this.Width / 18, this.Height));
                    e.Graphics.DrawLine(p, new Point(this.Width * 11 / 72, this.Height / 16), new Point(this.Width * 11 / 72, this.Height));
                    pen.Dispose();
                }



                using (System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                {
                    string drawString = "Machine";
                    string drawString2 = "in/s";
                    string drawString3 = "mm/s";
                    System.Drawing.Font drawFont = new System.Drawing.Font(
                        "Times", (int)(Math.Min(this.Height / 22, this.Width / 28)));// (int)(Math.Min(this.Height / 24, this.Width / 32)));
                    System.Drawing.Font drawFont2 = new System.Drawing.Font(
                        "Times", (int)(Math.Min(this.Height / 26, this.Width / 32)));
                    e.Graphics.DrawString(drawString, drawFont, drawBrush, this.Width / 28, 0);
                    e.Graphics.DrawString(drawString2, drawFont2, drawBrush, this.Width / 28, 2 * this.Height / 18);
                    e.Graphics.DrawString(drawString3, drawFont2, drawBrush, this.Width * 7 / 45, 2 * this.Height / 18);
                    x = this.Width * 3 / 55 + this.Width / 5;
                    foreach (string s in classTitles)
                    {
                        e.Graphics.DrawString(s, drawFont2, drawBrush, x, 0);
                        x += this.Width / 5;
                    }
                    drawFont.Dispose();
                    drawBrush.Dispose();
                }
                foreach (MachineClassRMSValues cl in RMSList)
                {
                    using (Brush brush = new SolidBrush(cl.Color))
                    {
                        int classIndex = cl.Class == MachineClass.ClassI ? 0 : cl.Class == MachineClass.ClassII ? 1 : cl.Class == MachineClass.ClassIII ? 2 : cl.Class == MachineClass.ClassIV ? 3 : 4;
                        int xPixelLoc = -100;
                        int cIndex = 0;
                        for (int i = this.Width / 4; i < this.Width; i += this.Width / 5)
                        {
                            if (classIndex == cIndex)
                            {
                                xPixelLoc = i;
                            } cIndex++;
                        }

                        int yPixelLoc = -100;
                        int rowInd = cl.Class == MachineClass.Undefined ? -1 : cl.RowIndex;

                        if (rowInd == 0)
                        {
                            yPixelLoc = this.Height / 4;
                        }
                        else
                        {
                            int rIndex = 0;
                            for (int j = this.Height / 4 + (int)(Math.Ceiling(this.Height / 16.0)); j < this.Height; j += (int)(Math.Ceiling(this.Height / 16.0)))
                            {
                                if (rIndex == rowInd)
                                {
                                    yPixelLoc = j;
                                }
                                rIndex++;
                            }
                        }
                        int min = (int)(Math.Min(this.Height / 22.0, this.Width / 32.0));

                        int[] xLocs = new int[2];
                        int[] yLocs = new int[2];
                        int delta = 0;
                        for (int i = 0; i < min; i++)
                        {
                            for (int j = 0; j < min; j++)
                            {
                                xLocs[0] = xPixelLoc + i < pixels.GetLength(0) && xPixelLoc + i > 0 ? xPixelLoc + i : 0;
                                xLocs[1] = xPixelLoc - i >= 0 && xPixelLoc - i < pixels.GetLength(0) ? xPixelLoc - i : 0;
                                yLocs[0] = yPixelLoc + i < pixels.GetLength(1) && yPixelLoc + i > 0 ? yPixelLoc + i : 0;
                                yLocs[1] = yPixelLoc - i >= 0 && yPixelLoc - i < pixels.GetLength(1) ? yPixelLoc - i : 0;
                                if (pixels[xLocs[0], yLocs[0]] || pixels[xLocs[1], yLocs[1]])
                                {
                                    delta = min + min / 3;
                                }
                            }
                        }
                        int maxPixelsInCol = this.Width / 5;
                        while (pixels[xPixelLoc + delta, yPixelLoc] && delta < maxPixelsInCol -  delta) { delta += min + min / 3; }
                        pixels[xPixelLoc + delta, yPixelLoc ] = true;
                        e.Graphics.FillEllipse(brush, new Rectangle(xPixelLoc + min / 3 + delta, yPixelLoc + min / 3, min, min));
                        cl.Location = new Point(xPixelLoc + min / 3 + delta, yPixelLoc + min / 3);
                    }
                }
                e.Graphics.TranslateTransform(0, this.Height);
                e.Graphics.RotateTransform(-90);
                using (System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                {
                    string drawString = "Vibration Velocity Vrms";
                    System.Drawing.Font drawFont = new System.Drawing.Font(
                        "Times", (int)(Math.Min(this.Height / 20, this.Width / 26)));
                    e.Graphics.DrawString(drawString, drawFont, drawBrush, 0, 0);
                    drawFont.Dispose();
                    drawBrush.Dispose();
                }
            }
            catch (Exception ex) { } // Too small
        }

        private void VRMSChartControl_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        ToolTip tip;


        List<int> rmsFileIndices = new List<int>();
        int rmsFileIndex = -1;
        private void VRMSChartControl_MouseMove(object sender, MouseEventArgs e)
        {
            int index = 0;
            int numPos = 0;
            string rmsVal = "";
            bool showTT = false;
            bool newIndexUsed = false;
            Point ttPosition = new Point();
            int[] oldIndicesUsed = new int[rmsFileIndices.Count];
            rmsFileIndices.CopyTo(oldIndicesUsed);
            rmsFileIndices.Clear();
            foreach (MachineClassRMSValues cl in RMSList)
            {
                int min = (int)(Math.Min(this.Height / 22.0, this.Width / 32.0));

                Point mouseLocation = this.PointToClient(Cursor.Position);
                if (Math.Abs(mouseLocation.X - cl.Location.X) < min &&
                    (mouseLocation.X - cl.Location.X) > 0 &&
                    Math.Abs(mouseLocation.Y - cl.Location.Y) < min &&
                    (mouseLocation.Y - cl.Location.Y) > 0)//yPixelLoc + delta - min / 3) < min)
                {
                    rmsFileIndices.Add(index);
                    showTT = true;
                    if (tip == null)
                    {
                        tip = new ToolTip();
                    }
                    if (true)//rmsFileIndex != index || string.IsNullOrEmpty(tip.GetToolTip(this)))
                    {
                        tip.ToolTipTitle = "Machine Specifications";
                        Point p = Cursor.Position;
                        ttPosition = new Point(mouseLocation.X + 15, mouseLocation.Y + 15);
                        string tmp = cl.ISMMS ? cl.MMPS.ToString() : cl.INPS.ToString();
                        rmsVal += numPos > 0 ? Environment.NewLine : "";
                        rmsVal += "File: "+cl.FileName + Environment.NewLine + "RMS Value: " + tmp;
                        numPos ++;
                    }
                    rmsFileIndex = index;
                }
                index++;
            }
            if (showTT)
            {
                bool newTT = false;
                foreach (int ind in rmsFileIndices)
                {
                    if (!oldIndicesUsed.Contains(ind))
                    {
                        newTT = true;
                    }
                }
                if (newTT ||  string.IsNullOrEmpty(tip.GetToolTip(this)))
                {
                    Point p = Cursor.Position;
                    tip.Show(rmsVal, this, ttPosition.X, ttPosition.Y, 2500);
                }
            }
        }
    }
}
