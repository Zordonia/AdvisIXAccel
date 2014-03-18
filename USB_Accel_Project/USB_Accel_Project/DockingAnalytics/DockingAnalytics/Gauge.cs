using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public partial class Gauge : UserControl
    {
        private double cMaxSpeed = 300;
        private double mSpeed;
        private double deltaOK;
        private double deltaWarning;
        private double preferredLevel;
        private int fontScl = 35;
        public bool ThresholdUse = false;
        System.Threading.Timer t;
        //Form f2 = new Form();
        //ZedGraphControl zgc = new ZedGraphControl();

        List<System.Windows.Forms.Label> lblList = new List<System.Windows.Forms.Label>();

        [DefaultValue(550.0)]
        public double PreferredValue
        {
            get { return preferredLevel; }
            set { preferredLevel = value; Invalidate(); }
        }
        [DefaultValue(150.0)]
        public double DeltaOKValue
        {
            get { return deltaOK; }
            set { deltaOK = value; Invalidate(); }
        }
        [DefaultValue(150.0)]
        public double DeltaWarningLevel
        {
            get { return deltaWarning; }
            set { deltaWarning = value; Invalidate(); }
        }
        public double MaxRMS
        {
            get { return cMaxSpeed; }
            set { cMaxSpeed = value; }
        }

        [DefaultValue(50.0)]
        public double Speed
        {
            get { return mSpeed; }
            set
            {
                if (value > cMaxSpeed)
                {
                    cMaxSpeed = value * 1.10;
                    mSpeed = value;
                }
                else
                {
                    mSpeed = value;
                }
                Invalidate();
            }
        }
        public Gauge()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Speed = 600.0;
            PreferredValue = 530.0;
            DeltaOKValue = 40;
            DeltaWarningLevel = 45;
            //f2.Controls.Add(zgc);

            //t = new System.Threading.Timer(timer, this, 10, 50);
        }

        private void timer(object sender)
        {
            if (Speed == cMaxSpeed)
            {
                Speed = 0;
            }
            Speed++;
        }

        Tuple<bool, double, Point, double, double>[] ascvalues = new Tuple<bool, double, Point, double, double>[7];
        Tuple<bool, double, Point, double, double>[] decvalues = new Tuple<bool, double, Point, double, double>[7];

        private void setValues()
        {
            int min = Math.Min(this.Width / 2, this.Height / 2);
            for (int i = 0; i < 181; i++)
            {
                double height, width;
                int algnment;
                int fontSze = Math.Max(this.Height / fontScl, 12);
                using (System.Drawing.Font drawFont = new System.Drawing.Font("Arial", fontSze))
                {
                    string drawString = ((int)(i / 180.0 * cMaxSpeed)).ToString("0.#");
                    //string drawString = (i).ToString("0.##");
                    System.Drawing.SolidBrush drawBrush = new
                        System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    SizeF sizef = this.CreateGraphics().MeasureString(drawString, drawFont);
                    algnment = i > 90 ? (int)sizef.Width : i == 90 ? (int)sizef.Width / 2 : 0;
                    height = sizef.Height;
                    width = sizef.Width;
                }
                int xm = (this.Width / 2) - (int)(Math.Cos(i * Math.PI / 180.0) * (min * (9.0 / 10)));
                int ym = (this.Height / 2) - (int)(Math.Sin(i * Math.PI / 180.0) * (min * (9.0 / 10)));
                Point mid = new Point(xm - algnment, ym);
                if (i <= 90 && i % 15 == 0)
                {
                    ascvalues[i / 15] = new Tuple<bool, double, Point, double, double>(true, i, mid, height, width);
                }
                if (i >= 90 && i % 15 == 0)
                {
                    decvalues[i / 15 - 6] = new Tuple<bool, double, Point, double, double>(true, i, mid, height, width);
                }
            }

            int skipped = 1;
            for (int j = ascvalues.Count() - 2; j >= 0; j--)
            {
                if (((int)(ascvalues[j].Item3.Y - ascvalues[j].Item4) < ascvalues[j + skipped].Item3.Y)
                    && !(ascvalues[j].Item3.X + ascvalues[j].Item5 < ascvalues[j + skipped].Item3.X))
                {
                    skipped++;
                }
                else { continue; }
            }
            if (skipped == 1)
                return;

            int str = skipped > ascvalues.Count() / 2 ? 1 : 2;
            if (skipped > ascvalues.Count() / 2 - 1)
            {
                skipped = 1;
            }
            for (int k = ascvalues.Count() - str; k >= 0; k -= skipped)
            {
                ascvalues[ascvalues.Count() - 1 - k] = new Tuple<bool, double, Point, double, double>(false, ascvalues[ascvalues.Count() - 1 - k].Item2, ascvalues[ascvalues.Count() - 1 - k].Item3, ascvalues[ascvalues.Count() - 1 - k].Item4, ascvalues[ascvalues.Count() - 1 - k].Item5);
                decvalues[k] = new Tuple<bool, double, Point, double, double>(false, decvalues[k].Item2, decvalues[k].Item3, decvalues[k].Item4, decvalues[k].Item5);
            }
        }

        private void writeHeader()
        {
            Console.WriteLine(" i   | skipped  | xOverlap? | yOverlap? ");
            Console.WriteLine("-----+----------+-----------+-----------");
        }

        private void Gauge_Paint(object sender, PaintEventArgs e)
        {
            //LineItem li = new LineItem("Values");
            //zgc.GraphPane.CurveList.Clear();
            //writeHeader();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int min = Math.Min(this.Width / 2, this.Height / 2);
            int mthickness = min / 100;
            int cthickness = mthickness > 2 ? mthickness - 2 : 2;
            int speed = (int)(mSpeed / cMaxSpeed * 180);
            Point midpoint = new Point(this.Width / 2, this.Height / 2);
            Rectangle prevFontRect = new Rectangle(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
            int skipped = 1;
            int x = midpoint.X - min;
            x = x < 0 ? 0 : x;
            int y = midpoint.Y - min;
            y = x < 0 ? 0 : y;
            Rectangle rect = new Rectangle(x, y, min * 2, min * 2);
            int prefVal = (int)(PreferredValue / cMaxSpeed * 180);
            int dOK = (int)(DeltaOKValue / cMaxSpeed * 180);
            int dWV = (int)(DeltaWarningLevel / cMaxSpeed * 180);
            if (ThresholdUse)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(200, Color.Red)))
                {
                    Pen p = new Pen(sb.Color);
                    e.Graphics.FillPie(sb, rect, 180, 180);
                    //e.Graphics.DrawArc(p, rect, 1800, 180);
                }
                using (SolidBrush sb = new SolidBrush(Color.Yellow))
                {
                    Pen p = new Pen(sb.Color);
                    e.Graphics.FillPie(sb, rect, prefVal + 180 - dWV / 2, dWV);
                    //e.Graphics.DrawArc(p, rect, prefVal + 180 - dWV / 2, dWV);
                }
                using (SolidBrush sb = new SolidBrush(Color.Green))
                {
                    Pen p = new Pen(sb.Color);
                    e.Graphics.FillPie(sb, rect, prefVal + 180 - dOK / 2, dOK);
                    //e.Graphics.DrawArc(p, rect, prefVal + 180 - dOK / 2, dOK);
                }
            }
            else
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(200, Color.Red)))
                {
                    Pen p = new Pen(sb.Color, this.Width/20);
                    e.Graphics.FillPie(sb, rect, 180, 180);
                    //e.Graphics.DrawArc(p, rect, 180, 180);
                }
                using (SolidBrush sb = new SolidBrush(Color.Green))
                {
                    Pen p = new Pen(sb.Color, this.Width / 20);
                    e.Graphics.FillPie(sb, rect, 180, prefVal);
                    //e.Graphics.DrawArc(p, rect, 180, prefVal);
                }
            }
            setValues();
            try
            {
                for (int i = 0; i < 181; i++)
                {
                    int xm = (this.Width / 2) - (int)(Math.Cos(i * Math.PI / 180.0) * (min * (9.0 / 10)));
                    int ym = (this.Height / 2) - (int)(Math.Sin(i * Math.PI / 180.0) * (min * (9.0 / 10)));
                    Point mid = new Point(xm, ym);
                    int xmN = (this.Width / 2) - (int)(Math.Cos((i + 15 * skipped) * Math.PI / 180.0) * (min * (9.0 / 10)));
                    int ymN = (this.Height / 2) - (int)(Math.Sin((i + 15 * skipped) * Math.PI / 180.0) * (min * (9.0 / 10)));
                    Point midN = new Point(xmN, ymN);
                    int x1 = (this.Width / 2) - (int)(Math.Cos(i * Math.PI / 180.0) * min);
                    int y1 = (this.Height / 2) - (int)(Math.Sin(i * Math.PI / 180.0) * min);
                    Point next = new Point(x1, y1);
                    if (i % 3 == 0)
                    {
                        if (i % 15 == 0)
                        {
                            using (Pen p = new Pen(Color.Blue, mthickness))
                            {
                                e.Graphics.DrawLine(p, mid, next);
                            }
                            int fontSze = Math.Max(this.Height / fontScl, 12);
                            using (System.Drawing.Font drawFont = new System.Drawing.Font("Arial", fontSze))
                            {
                                //string drawString = (i / 180.0 * cMaxSpeed).ToString("0.##");
                                string drawString = (i).ToString("0.##");
                                System.Drawing.SolidBrush drawBrush = new
                                    System.Drawing.SolidBrush(System.Drawing.Color.Black);
                                SizeF sizef = e.Graphics.MeasureString(drawString, drawFont);
                                int algnment = i > 90 ? (int)sizef.Width : i == 90 ? (int)sizef.Width / 2 : 0;
                                bool yValuesOverlap = i < 90 ? !(mid.Y - sizef.Height > midN.Y) : !(mid.Y > midN.Y - sizef.Height);
                                bool xValuesOverlap = !(mid.X - algnment + sizef.Width < midN.X);
                                if (!yValuesOverlap || (yValuesOverlap && !xValuesOverlap))
                                {
                                    skipped = 1;
                                    //e.Graphics.DrawString(drawString, drawFont, drawBrush, mid.X - algnment, mid.Y);
                                }
                                else
                                {
                                    skipped++;
                                }
                                using (Pen p = new Pen(Color.Red, 2))
                                {
                                    //e.Graphics.DrawRectangle(p, new Rectangle(mid.X - algnment, mid.Y, (int)sizef.Width, (int)sizef.Height));
                                }
                                //PointPairList ppl = new PointPairList();
                                //ppl.Add(new PointPair(mid.X - algnment, mid.Y));
                                //ppl.Add(new PointPair(mid.X - algnment + sizef.Width, mid.Y - sizef.Height));
                                //li.AddPoint(new PointPair(mid.X - algnment, mid.Y));
                                Color color;

                                if (!yValuesOverlap || (yValuesOverlap && !xValuesOverlap))
                                {
                                    color = Color.Blue;
                                }
                                else
                                {
                                    color = Color.Red;
                                }
                                //zgc.GraphPane.AddCurve(i.ToString().PadLeft(3) + ":" + ppl[0].ToString() +
                                //    " :: Width: " + sizef.Width + " , Height: " + sizef.Height, ppl, color);
                                drawBrush.Dispose();
                                //Console.WriteLine(" " + i.ToString().PadLeft(3) +
                                //    " |   " + (skipped ).ToString().PadLeft(4) +
                                //    "   |  " + xValuesOverlap.ToString().PadLeft(7) +
                                //    "  |  " + yValuesOverlap.ToString().PadLeft(7));
                            }
                        }
                        else
                        {
                            using (Pen p = new Pen(Color.DarkBlue, cthickness))
                            {
                                e.Graphics.DrawLine(p, mid, next);
                            }
                        }
                    }
                }
                //zgc.AxisChange();
                //zgc.Dock = DockStyle.Fill;
                //zgc.Invalidate();
                //f2.Show();

                foreach (var tupl in ascvalues.Concat(decvalues))
                {
                    int fontSze = Math.Max(this.Height / fontScl, 12);
                    using (System.Drawing.Font drawFont = new System.Drawing.Font("Arial", fontSze))
                    {
                        string drawString = ((int)(tupl.Item2 / 180.0 * cMaxSpeed)).ToString("0.#");
                        //string drawString = (tupl.Item2 ).ToString("0.##");
                        System.Drawing.SolidBrush drawBrush = new
                            System.Drawing.SolidBrush(System.Drawing.Color.Blue);
                        SizeF sizef = e.Graphics.MeasureString(drawString, drawFont);
                        if (tupl.Item1)
                        {
                            e.Graphics.DrawString(drawString, drawFont, drawBrush, tupl.Item3);
                        }
                    }
                    using (Pen p = new Pen(Color.Red, 2))
                    {
                        //e.Graphics.DrawRectangle(p, new Rectangle(tupl.Item3.X, tupl.Item3.Y, (int)tupl.Item5, (int)tupl.Item4));
                    }
                }

                using (Pen p = new Pen(Color.Black, mthickness))
                {
                    int x2 = (this.Width / 2) - (int)(Math.Cos(speed * Math.PI / 180.0) * min);
                    int y2 = (this.Height / 2) - (int)(Math.Sin(speed * Math.PI / 180.0) * min);
                    Point speedPnt = new Point(x2, y2);
                    e.Graphics.DrawLine(p, midpoint, speedPnt);
                }
                int fs = Math.Max(this.Height / 30, 12);
                Color value = Color.Black;
                if (ThresholdUse)
                {
                    if (Math.Abs(Speed - this.preferredLevel) < DeltaOKValue / 2)
                    {
                        value = Color.Green;
                    }
                    else if (Math.Abs(Speed - this.preferredLevel) < DeltaWarningLevel / 2)
                    {
                        value = Color.YellowGreen;
                    }
                    else
                    {
                        value = Color.Red;
                    }
                }
                else
                {
                    if (Speed - this.preferredLevel > 0)
                    {
                        value = Color.Red;
                    }
                    else
                    {
                        value = Color.Black;
                    }
                }
                using (System.Drawing.Font drawFont = new System.Drawing.Font("Arial", fs))
                {
                    string drawString = (Speed).ToString("0.##");
                    System.Drawing.SolidBrush drawBrush = new
                        System.Drawing.SolidBrush(value);
                    SizeF sizef = this.CreateGraphics().MeasureString(drawString, drawFont);
                    e.Graphics.DrawString(drawString, drawFont, drawBrush, midpoint.X - sizef.Width / 2, midpoint.Y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Gauge_Load(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Gauge_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
