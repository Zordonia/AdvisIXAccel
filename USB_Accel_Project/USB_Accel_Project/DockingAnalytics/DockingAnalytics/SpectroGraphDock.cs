using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ZedGraph;

namespace DockingAnalytics
{
    public partial class SpectroGraphDock : Document
    {
        // Properties
        private Matrix4 CameraMatrix;
        private SphereCamera MyCamera { get; set; }
        private List<Vertex3> Vertices { get; set; }
        private List<PointPairList> PointPairListList { get; set; }
        private List<int> BitmapNameList { get; set; }
        private List<int> ScaleList { get; set; }
        private List<int> CurrentValue { get; set; }
        private MouseDevice Mouse { get; set; }
        private double xMin = 0;
        private double xMax = 1000; // TODO This should change
        RangeTrackBarToolStripItem myRangeTrackBar;

        // Local Variables
        private bool Loaded = false;
        private bool DrawBarGraph = true;
        private bool MousePressed = false;
        private bool DrawLineGraph = true;
        private bool DrawMesh = true;
        private bool DrawBinLevels = true;
        private bool CameraPan = false;

        private int MouseMovementAccumulator = 0;
        private static int CurrentFileIndex = 0;
        private int xLast = 0, yLast = 0;


        private double initXMM = 0, initYMM = 0, mouseXDelta = 0, mouseYDelta = 0, deltaXMM = 0, deltaYMM = 0;
        private double pi = Math.PI;

        private float[] MouseSpeedArr = new float[2];
        private float LookAtXPos = 110, LookAtYPos = 0, LookAtZPos = 30;
        private float XAxesPosition = 0, YAxesPosition = 0, ZAxesPosition = 0;

        public void InitializeProperties()
        {
            Mouse = new MouseDevice();
            Loaded = false;
            Vertices = new List<Vertex3>();
            PointPairListList = new List<PointPairList>();
            BitmapNameList = new List<int>();
            BitmapNameList.AddRange(new int[20]);
            CurrentValue = new List<int>();
            CurrentValue.AddRange(new int[2]);
            ScaleList = new List<int>();
            ScaleList.AddRange(new int[GlobalVars.GraphSteps3d+1]);
            MyCamera = new SphereCamera(new Vertex3(110, 0, 30), 157);
        }


        public SpectroGraphDock()
        {
            InitializeProperties();
            InitializeComponent();
            ls.Show();
            ls.ColorChanged += new PropertyChangedEventHandler(ls_ColorChanged);
        }

        void ls_ColorChanged(object sender, PropertyChangedEventArgs e)
        {
            glControl1.Invalidate();
        }


        private void glControl1_Load(object sender, EventArgs e)
        {
            
            if ((Loaded == false) && (sender == null))
            {
                return;
            } 
            Loaded = true;
            CameraMatrix = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 5.0f, 0.0f));//Matrix4.Translation(0f, 0f, 3f);

            CameraMatrix = Matrix4.LookAt(new Vector3(225.0f, 30.0f, (float)(GraphInformation.NCurves * 5.0 + 10)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 40.0f, 0.0f));//Matrix4.Translation(0f, 0f, 3f);
            CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));

            XAxesPosition = 225.0f;
            ZAxesPosition = (float)(GraphInformation.NCurves * 5.0 + 10);
            YAxesPosition = 30.0f;

            CreateFFTMesh();
            for (int go = 0; go < GraphInformation.NCurves; go++)
            {
                BitmapNameList[go] = this.BMP_CURRENTFILE(go, GraphInformation.CurveTypeItems[go].Name);
            }
            //GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);



            Reset3DScale(this, null);
        }




        /// <summary>
        /// Method for creating a mesh version of the FFT's that are populating the graph.
        /// Essentially a contour type structure that allows the user to see the reliefs of 
        /// a changing FFT representing vibration data. 
        /// </summary>
        private void CreateFFTMesh()
        {
            if (!Loaded) // Play nice
                return;
            Vertices = new List<Vertex3>();
            if (GraphInformation.NCurves < 1)
            {
                return;
            }
            double x = 0.0, y = 0.0, xb = 0.0, yb = 0.0, diff = 0.0;
            Color4 color = Color.Green;
            //bool toggle = true;
            IEnumerator<PointPair>[] pplEnumList = new IEnumerator<PointPair>[GraphInformation.NCurves];
            for (int i = 0; i < GraphInformation.NCurves; i++)
            {
                pplEnumList[i] = ((PointPairList)GraphInformation.CurveTypeItems[i].Curve.Points).ToSlice(xMin,xMax).Stretch(GlobalVars.XACTMAX).GetEnumerator();
            }
            while (pplEnumList[0].MoveNext())
            {
                for (int i = 1; i < pplEnumList.Length; i++)
                {
                    if (!pplEnumList[i].MoveNext())
                    {
                        return;
                    }
                }
                // All have moved to current;

                for (int p = 0; p < pplEnumList.Length; p++)
                {
                    xb = x;
                    yb = y;
                    x = pplEnumList[p].Current.X;
                    y = pplEnumList[p].Current.Y;
                    diff = (Math.Abs(y - yb) / y) * 100;
                    if (p % GraphInformation.NCurves == 0)
                    {
                        diff = 0;
                    }
                    if (diff < 75)
                    {
                        color = Color.Green;
                    }
                    else
                    {
                        if (diff < 150)
                        {
                            color = Color.Blue;
                        }
                        else
                        {
                            color = Color.Red;
                        }
                    }
                    PointPairList pl = (PointPairList)GraphInformation.CurveTypeItems[p].Curve.Points;
                    //Vertices.Add(new Vertex3((float)(x * (GlobalVars.XACTMAX / pl[pl.Count - 1].X)), (float)(y * (GlobalVars.YACTMAX / GlobalVars.YMAX)), (float)(p * GlobalVars.ZWIDTH), color));
                    Vertices.Add(new Vertex3((float)(x ), (float)(y * (GlobalVars.YACTMAX / GlobalVars.YMAX)), (float)(p * GlobalVars.ZWIDTH), color));
                    /*if (pplEnumList[p].MoveNext())
                    {
                        x = pplEnumList[p].Current.X;
                        y = pplEnumList[p].Current.Y;
                    }
                    vertices.Add(new Vertex3((float)x, (float)y * 750, (float)p*5,red));
                     */
                }
            }
        }


        /// <summary>
        /// Creates a bitmap (BMP) containing the fileName in raw text format to be used in the 
        /// OpenTK (3D Spectrograph).
        /// </summary>
        /// <param name="index">The index of the file.</param>
        /// <param name="bmpText">The name of the file.</param>
        /// <returns>The index that represents the BMP, so that it can be added to the OpenTK environment.</returns>
        private int BMP_CURRENTFILE(int index, String bmpText)
        {
            if (!Loaded || GraphInformation.NCurves == 0) // Play nice
                return -1;
            GL.DeleteTexture(BitmapNameList[index]);

            GL.Color3(Color.Transparent);
            TexUtil.InitTexturing();

            System.Drawing.Bitmap bmp = new Bitmap(glControl1.Width, (int)(glControl1.Height / 12.64));
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            TextRenderer.DrawText(g, bmpText, new Font(FontFamily.GenericSansSerif, (float)(glControl1.Height / 21.07), FontStyle.Bold), new Point(0, 0), Color.Gray);
            int textureId = TexUtil.CreateTextureFromBitmap(bmp);
            GL.Disable(EnableCap.Texture2D);
            return textureId;
        }


        private void Reset3DScale(object sender, EventArgs e)
        {
            if (!Loaded || GraphInformation.NCurves == 0) { return; }
            double ym = Double.MinValue, xm = Double.MinValue, ymi = Double.MaxValue, xmi = Double.MaxValue;
            double? mP = null;
            foreach (CurveItemTypePair citp in GraphInformation.CurveTypeItems)
            {
                if (citp.XMax > xm)
                {
                    xm = citp.XMax;
                }
                if (citp.YMax > ym)
                {
                    ym = citp.YMax;
                }
                if (citp.XMin < xmi)
                {
                    xmi = citp.XMin;
                }
                if (citp.YMin < ymi)
                {
                    ymi = citp.YMin;
                }
                if (mP == null || citp.CurveObj.CurvePercentageToGraph > mP)
                {
                    //mP = citp.CurveObj.CurvePercentageToGraph;
                }
            }
            GlobalVars.SingleStep3d = (this.xMax - this.xMin) / GlobalVars.XACTMAX;// GraphInformation.CurveTypeItems.Max(a => a.XMax) / GlobalVars.XACTMAX;
            GlobalVars.FiveStep3d = 5.0 * GlobalVars.SingleStep3d;
            for (int i = 0; i < GlobalVars.GraphSteps3d + 1; i++)
            {
                GL.DeleteTexture(i);
                ScaleList[i] = MakeStepBitmap(GlobalVars.FiveStep3d * i + this.xMin);
            }
            if (GraphInformation.NCurves != 0)
            {
                DrawCurrentValue();
            }
            GlobalVars.ScaleChanged = true;
            GlobalVars.XMAX = mP == null ? xm : xm * (double)mP;
            GlobalVars.YMAX = ym;
            GlobalVars.YMIN = ymi;
            //PaintThreeD("Scale Reset", null);
        }




        private void DrawCurrentValue()
        {
            if (GraphInformation.NCurves == 0) { return; }
            double xstepF  = (xMax - xMin)/GlobalVars.XACTMAX; 
            double findex = Math.Round(LookAtXPos * xstepF) + (int)xMin;
            CurveItem curve = GraphInformation.CurveTypeItems[0].Curve;
            int index = (int)((findex / curve[curve.NPts - 1].X) * curve.NPts);
            int ind2 = 0;
            double maxYValue = Double.MinValue;
            double XValue = 0;
            // TODO: Right now it only looks at the first curve, maybe get max of all curves?
            int stepResolution = (int)((xMax - xMin) / GlobalVars.XACTMAX) / 2;
            maxYValue = curve[index].Y;
            XValue = GraphInformation.CurveTypeItems[0].Curve[index].X;
            for (int i = index - stepResolution; i < index + stepResolution; i++)
            {
                ind2 = i;
                if (ind2 < 0)
                {
                    ind2 = 0;
                }
                if (ind2 > GraphInformation.CurveTypeItems[0].Curve.NPts - 1)
                {
                    ind2 = GraphInformation.CurveTypeItems[0].Curve.NPts - 1;
                }
                if (curve[ind2].Y > maxYValue)
                {
                    maxYValue = curve[ind2].Y;
                    XValue = GraphInformation.CurveTypeItems[0].Curve[ind2].X;
                }
            }
            /// Y Value
            TexUtil.InitTexturing();
            string str = String.Format("{0:#.0000e00}", maxYValue);
            System.Drawing.Bitmap bmp = new Bitmap(800, 200);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            TextRenderer.DrawText(g, str, new Font(FontFamily.GenericSansSerif, 112.0f, FontStyle.Bold), new Point(0, 0), Color.Red, TextFormatFlags.Left);
            GL.DeleteTexture(CurrentValue[0]);
            int textureId = TexUtil.CreateTextureFromBitmap(bmp);
            CurrentValue[0] = textureId;

            /// X Value
            TexUtil.InitTexturing();
            str = String.Format("{0:0000.0}", XValue);
            bmp = new Bitmap(800, 200);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            TextRenderer.DrawText(g, str, new Font(FontFamily.GenericSansSerif, 112.0f, FontStyle.Bold), new Point(0, 0), Color.Red,TextFormatFlags.Left);
            textureId = TexUtil.CreateTextureFromBitmap(bmp);
            GL.Disable(EnableCap.Texture2D);
            GL.DeleteTexture(CurrentValue[1]);
            CurrentValue[1] = textureId;
        }


        /// <summary>
        /// Paints the various components of the 3D spectrograph including the FFT in line graph,
        /// bar graph, and mesh graph forms.
        /// </summary>
        /// <param name="sender">The object that sent this event.</param>
        /// <param name="e">The Arguments for the PaintEvent.</param>
        private void PaintThreeD(object sender, PaintEventArgs e)
        {
            if (!Loaded)
                return;
            if (GlobalVars.ScaleChanged)
            {
                Reset3DScale(this, null);
            }
            glControl1.MakeCurrent();
            //Console.WriteLine(fftPain.XAxis.Scale.Max);
            //Console.WriteLine("XMAXIMUM: " + GlobalVars.XMAX
            //    + "\r\nYMAXIMUM: " + GlobalVars.YMAX
            //    + "\r\nXSTEP: " + GlobalVars.XACTMAX / GlobalVars.XMAX);
            ////if (!loaded) // Play nice
            //    return;
            if (sender.ToString().Equals("File Added") || sender.ToString().Equals("File Removed") || sender.ToString().Equals("Scale Changed"))
            {
                //this.CreateFFTMesh();
            }
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LoadMatrix(ref CameraMatrix);
            //GL.MultMatrix(ref cameraMatrix);
            addLight();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(OpenTK.Graphics.OpenGL.DepthFunction.Lequal);

    DateTime earlier = DateTime.Now;
    DateTime later = DateTime.Now;
    double result = (later - earlier).TotalMilliseconds;


            DrawAxesPlane();
            DrawAxesScale();
            int i = 0;

            IEnumerator ienum = GraphInformation.CurveTypeItems.GetEnumerator();

            while (ienum.MoveNext()) //GraphInformation.CurveTypeItems[i].Curve.NPts != 0)
            {
                CurveItemTypePair citp = (CurveItemTypePair)ienum.Current;
                if (DrawBarGraph)
                {
                    //addCurve(igList[i].getPPL(), i);
                    AddCurve(citp.CurveObj);
                }
                if (DrawLineGraph)
                {
                    AddLineGraph(citp.CurveObj, i);
                    //AddLineGraph((PointPairList)citp.Curve.Points, i);
                }
                GL.Color3(Color.Transparent);
                i++;
            }
            if (GlobalVars.ScaleChanged)
            {
                CreateFFTMesh();
                GlobalVars.ScaleChanged = false;
            }
            if (DrawMesh)
            {
                Vertex3[] vertA =  Vertices.ToArray();
                AddMesh(vertA);
            }


                               //     public static PointPairList ToSlice(this PointPairList list, double min, double max)
                               // {
                               //     if (max <= min) { min = 0; max = 1; }
                               //     PointPairList temp = new PointPairList();
                               //     int minindex = list.IndexOf(list.FirstOrDefault(a => a.X >= min));
                               //     minindex = minindex < 0 ? 0 : minindex;
                               //     int maxindex = list.FindLastIndex(delegate(PointPair pp) { return pp.X <= max; });
                               //     temp.AddRange(list.GetRange(minindex, maxindex - minindex));
                               //     return temp;
                               //}

                               // public static PointPairList Stretch(this PointPairList list, double max)
                               // {
                               //     if (list.Count == 0) { return new PointPairList(); }
                               //     PointPairList temp = new PointPairList();
                               //     //double xstep = (max / (double)(list[list.Count - 1].X - list[0].X));
                               //     double xstep = (max / list.Count);
                               //     GlobalVars.YMAX = double.MinValue;
                               //     for (int i = 0; i < list.Count; i++)
                               //     {
                               //         if (GlobalVars.YMAX < list[i].Y) { GlobalVars.YMAX = list[i].Y; }
                               //         //temp.Add(new PointPair((xstep * list[i].X), list[i].Y));
                               //         temp.Add(new PointPair((xstep * i), list[i].Y));
                               //     }
                               //     return temp;
                               // }

            //if (DrawBinLevels)
            //{
            //    IEnumerator bandListEnum = BandObjList.GetEnumerator();
            //    while (bandListEnum.MoveNext())
            //    {
            //        BandObj temp = (BandObj)bandListEnum.Current;
            //        if (temp.WESTLOC < xMin) { temp.WESTLOC = 0; }
            //        if (temp.WESTLOC > xMax) { continue; }
            //        if (temp.EASTLOC > xMax) { temp.EASTLOC = xMax; }
            //        if (temp.EASTLOC < xMin) { continue; }
            //        this.AddBinBox(temp.WESTLOC * (GlobalVars.XACTMAX / xMax),
            //            temp.EASTLOC * (GlobalVars.XACTMAX / xMax),
            //            temp.ALARM * (GlobalVars.YACTMAX / GlobalVars.YMAX),
            //            0,
            //            5 * GraphInformation.NCurves,
            //            Color.Red);
            //    }


                // SECTION REMOVED FROM PREVIOUS ANALYTICS AND TENDED TO WORK CORRECTLY
                // WILL REMOVE FROM THIS VERSION BUT KEEP COMMENTED UNTIL PROPER TESTING
                // IS DONE.
                //IEnumerator binListEnum = binList.GetEnumerator();
                //while (binListEnum.MoveNext())
                //{
                //    Bin bTemp = (Bin)binListEnum.Current;
                //    this.AddBinBox(bTemp.getFmin() * (GlobalVars.XACTMAX / GlobalVars.XMAX),
                //        bTemp.getFmax() * (GlobalVars.XACTMAX / GlobalVars.XMAX),
                //        bTemp.getAlarMax() * (GlobalVars.YACTMAX / GlobalVars.YMAX),
                //        0, 5 * NumFiles, Color.Red);
                //}
            // }

            SwitchToOrthographicView();
            DrawHeadsUpDisplay();
            SwitchToFrustrumView();
            //addText(1);
            //text adheres to \n character
            //addText("Does the text adhere to wraparound?", new Vector3(1, 1, 1));

            //GL.Disable(EnableCap.Blend);
            glControl1.SwapBuffers();
            GlobalVars.ScaleChanged = false;
        }

        private void AddCurve(CurveObject co)
        {
            if (MousePressed)
            {
                MouseMovementAccumulator++;
            }
            co.XMax = xMax;
            co.XMin = xMin;
            if (false)//MouseMovementAccumulator > 1)
            {
                co.Render(true);
            }
            else
            {
                co.Render(false);
            }
            //co.Render((MouseMovementAccumulator > 1));
        }

        LightSelection ls = new LightSelection();

        private void addLight()
        {
            float[] dv = new float[] { 1, 1, 1, 1 }; ls.GetDiffuse();
            float[] av = new float[] { 0.5f, 0.5f, 0.5f, 3.5f }; ls.GetAmbient();
            float[] pv = new float[] { 100, 100, 10, 2.6f }; ls.GetPosition();
            GL.Light(LightName.Light1, LightParameter.Diffuse, new Vector4(dv[0], dv[1], dv[2], dv[3]));
            GL.Light(LightName.Light1, LightParameter.Ambient, new Vector4(av[0], av[1], av[2], av[3]));
            GL.Light(LightName.Light1, LightParameter.Position, new Vector4(pv[0], pv[1], pv[2], pv[3]));
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Lighting);
        }

        /// <summary>
        /// Given a list of PointPairs and the index of the data location, this method
        /// adds a line graph style curve to the 3D Spectrograph environment.
        /// </summary>
        /// <param name="pl">The PointPairList of PointPairs which represent data from the 
        /// FFT of the particular file.</param>
        /// <param name="zInd">The zIndex of the file, corresponds to the file index, but zIndex
        /// is used to show the disparity of the distance from the origin of the curve.</param>
        private void AddLineGraph(CurveObject co, int zInd)
        {
            if (!Loaded) 
                return;
            PointPairList pl = co.PointPairList.ToSlice(xMin, xMax).Stretch(GlobalVars.XACTMAX);
            IEnumerator<PointPair> enumer = pl.GetEnumerator();
            double x = 0.0, y = 0.0, xb4 = 0.0, yb4 = 0.0;

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Begin(BeginMode.LineStrip);
            int accum = 0;
            while (enumer.MoveNext())// && accum < co.CurvePercentageToGraph*pl.Count)
            {
                
                xb4 = x;
                yb4 = y;
                x = enumer.Current.X;
                y = enumer.Current.Y;
                if (y * 750 < 2.5)
                {
                    GL.Color3(0.0, 1.0, 0.0);
                }
                else
                {
                    if (y * 750 < 5.0)
                    {
                        GL.Color3(0.0, 0.0, 1.0);
                    }
                    else
                    {
                        GL.Color3(1.0, 0.0, 0.0);
                    }
                }
                // 9889 GL.Vertex3(x / 1024 * 200, y * 750, zInd * 5);
                //GL.Vertex3(x * (GlobalVars.XACTMAX / GlobalVars.XMAX), y * (GlobalVars.YACTMAX / GlobalVars.YMAX), zInd * GlobalVars.ZWIDTH);
                //GL.Vertex3(x * (GlobalVars.XACTMAX / pl[pl.Count - 1].X), y * (GlobalVars.YACTMAX / GlobalVars.YMAX), zInd * GlobalVars.ZWIDTH);
                GL.Vertex3(x, y * (GlobalVars.YACTMAX / GlobalVars.YMAX), zInd * GlobalVars.ZWIDTH);
                //GL.Vertex3(x / 1024 * 25600, y, zInd * 5);
                accum++;
            }
            GL.End();
            
        }


        /// <summary>
        /// Another creation of a mesh for viewing the 3D Spectrograph.
        /// </summary>
        /// <param name="vertix">The vertix that starts the mesh.</param>
        private void AddMesh(Vertex3[] vertix)
        {
            if (!Loaded) // Play nice
                return;
            Vertex3 temp;
            double[] tempd;

            for (int i = 0; i < vertix.Length; i++)
            {
                if ((i + GraphInformation.NCurves) < vertix.Length)
                {
                    if (!(i % GraphInformation.NCurves == GraphInformation.NCurves - 1))
                    {
                        Vertex3[] tempvs = new Vertex3[3];
                        temp = vertix[i];
                        tempvs[0] = temp;
                        temp = vertix[i + 1];
                        tempvs[1] = temp;
                        temp = vertix[i + GraphInformation.NCurves];
                        tempvs[2] = temp;

                        GL.Begin(BeginMode.Triangles);
                        for (int y = 0; y < 3; y++)
                        {
                            temp = tempvs[y];
                            tempd = temp.getVals();
                            GL.Color4(temp.getColor());
                            GL.Vertex3(tempd[0], tempd[1], tempd[2]);
                        }
                        GL.End();
                        GL.Begin(BeginMode.LineStrip);
                        for (int y = 0; y < 3; y++)
                        {
                            temp = tempvs[y];
                            tempd = temp.getVals();
                            GL.Color4(Color4.Black);
                            GL.Vertex3(tempd[0], tempd[1], tempd[2]);
                        }
                        GL.End();
                    }
                    if (!(i % GraphInformation.NCurves == 0))
                    {
                        GL.Begin(BeginMode.Triangles);
                        temp = vertix[i];
                        tempd = temp.getVals();
                        GL.Color4(temp.getColor());
                        GL.Vertex3(tempd[0], tempd[1], tempd[2]);
                        temp = vertix[i + GraphInformation.NCurves];
                        tempd = temp.getVals();
                        GL.Color4(temp.getColor());
                        GL.Vertex3(tempd[0], tempd[1], tempd[2]);
                        temp = vertix[i + GraphInformation.NCurves - 1];
                        tempd = temp.getVals();
                        GL.Color4(temp.getColor());
                        GL.Vertex3(tempd[0], tempd[1], tempd[2]);
                        GL.End();
                    }
                }
            }
        }


        /// <summary>
        /// Method for adding a bin Box to the 3D Spectrograph region, similar to addBin's to 2D graph.
        /// NOTE: Currently the binBox can be added, but cannot be taken away, therefore this method works
        /// properly, but another method [removeBinBox] should be used in conjunction with this if they are
        /// not desired.
        /// </summary>
        /// <param name="x1">The first x location of the cube that represents the bin.</param>
        /// <param name="x2">The second x location of the cube that represents the bin.</param>
        /// <param name="y">The y value location of the cube that represents the bin.</param>
        /// <param name="zi">The initial z value location of the cube that represents the bin.</param>
        /// <param name="zwidth">The width of the bin that will be represented with a cube in the envirionment.</param>
        /// <param name="bincolor">The color of the bin, this should be determined by wether the bin is 
        /// in alarm mode or not, and can possibly be useful to give a percentage representation of how close/
        /// far the bin is from the alarm in either direction. i.e. greener further away and more brightly
        /// red the further past an alarm.</param>
        private void AddBinBox(double x1, double x2, double y, double zi, double zwidth, Color bincolor)
        {
            if (!Loaded) // Play nice
                return;
            double yi = (2.0 / 3.0) * y;
            

            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Begin(BeginMode.Polygon);
            // Face 1
            //V1i,2i,3i,4i
            GL.Vertex3(x1, yi, zi);
            GL.Color3(bincolor);
            GL.Vertex3(x1, y, zi);
            GL.Vertex3(x1, y, zi + zwidth);
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x1, yi, zi + zwidth);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            // Face 2
            //V1f,V4f,V3f,V2f
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x2, yi, zi);
            GL.Vertex3(x2, yi, zi + zwidth);
            GL.Color3(bincolor);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x2, y, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 3
            // V4f,V3f,V3i,V4i
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x2, yi, zi + zwidth);
            GL.Color3(bincolor);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x1, y, zi + zwidth);
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x1, yi, zi + zwidth);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 4
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x2, yi, zi);
            GL.Color3(bincolor);
            GL.Vertex3(x2, y, zi);
            GL.Vertex3(x1, y, zi);
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x1, yi, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 5
            GL.Color3(bincolor);
            GL.Vertex3(x1, y, zi);
            GL.Vertex3(x1, y, zi + zwidth);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x2, y, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 6
            GL.Color3(0.1f, 0.2f, 0.5f);
            GL.Vertex3(x1, yi, zi);
            GL.Vertex3(x1, yi, zi + zwidth);
            GL.Vertex3(x2, yi, zi + zwidth);
            GL.Vertex3(x2, yi, zi);
            GL.End();
        }

        /// <summary>
        /// Method for adding a box for the bargraph style graph. Adds a box for each element within the graph.
        /// </summary>
        /// <param name="x1">The initial x value location of the box that represents the element.</param>
        /// <param name="x2">The second x value location of the box that represents the element.</param>
        /// <param name="y">The y value that represents the value of the element.</param>
        /// <param name="zi">The initial z value location of the box that represents the element.</param>
        /// <param name="zwidth">The value for the width of the box in the x direction.(Element independant)</param>
        /// <param name="ychange">The value that shows how much the element has changed from the previous element, 
        /// this is for representation of the bar graph so that any large changes are highlighted by different
        /// colored bars.</param>
        private void AddBox(double x1, double x2, double y, double zi, double zwidth, double ychange)
        {
            if (!Loaded) // Play nice
                return;
            Color4 myTopColor = Color4.Green;
            Color4 myBottomColor = Color4.White;
            if (ychange < 10 && ychange > -10)
            {
                myTopColor = Color4.Green;
            }
            else
            {
                if (ychange < 100 && ychange > 0)
                {
                    myTopColor = Color4.DarkBlue;
                }
                else
                {
                    if (ychange > 0)
                    {
                        myTopColor = Color4.Blue;
                    }
                    else
                    {
                        if (ychange > -100)
                        {
                            myTopColor = Color4.DarkRed;
                        }
                        else
                        {
                            myTopColor = Color4.Red;
                        }
                    }
                }
            }
            myTopColor.A = (byte)255;
            myBottomColor.A = (byte)255;
            /*
            switch ((int)zi)
            {
                case 0:
                    myBottomColor = Color4.DarkRed;
                    myTopColor = Color4.Red;
                    break;
                case 5:
                    myBottomColor = Color4.DarkOrange;
                    myTopColor = Color4.Orange;
                    break;
                case 10:
                    myBottomColor = Color4.Yellow;
                    myTopColor = Color4.LightYellow;
                    break;
                case 15:
                    myBottomColor = Color4.DarkGreen;
                    myTopColor = Color4.Green;
                    break;
                case 20:
                    myBottomColor = Color4.DarkBlue;
                    myTopColor = Color4.Blue;
                    break;
                case 25:
                    myBottomColor = Color4.Indigo;
                    myTopColor = Color4.LightPink;
                    break;
            }*/
            GL.Begin(BeginMode.Polygon);
            // Face 1
            GL.Color4(myBottomColor);
            //V1
            GL.Vertex3(x1, 0, zi);
            //V2
            GL.Color4(myTopColor);
            GL.Vertex3(x1, y, zi);
            //V3
            GL.Vertex3(x1, y, zi + zwidth);
            //V4
            GL.Color4(myBottomColor);
            GL.Vertex3(x1, 0, zi + zwidth);

            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 2
            GL.Color4(myBottomColor);
            GL.Vertex3(x2, 0, zi);
            GL.Vertex3(x2, 0, zi + zwidth);
            GL.Color4(myTopColor);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x2, y, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 3
            GL.Color4(myBottomColor);
            GL.Vertex3(x2, 0, zi + zwidth);
            GL.Color4(myTopColor);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x1, y, zi + zwidth);
            GL.Color4(myBottomColor);
            GL.Vertex3(x1, 0, zi + zwidth);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 4
            GL.Color4(myBottomColor);
            GL.Vertex3(x2, 0, zi);
            GL.Color4(myTopColor);
            GL.Vertex3(x2, y, zi);
            GL.Vertex3(x1, y, zi);
            GL.Color4(myBottomColor);
            GL.Vertex3(x1, 0, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 5
            GL.Color4(myTopColor);
            GL.Vertex3(x1, y, zi);
            GL.Vertex3(x1, y, zi + zwidth);
            GL.Vertex3(x2, y, zi + zwidth);
            GL.Vertex3(x2, y, zi);
            GL.End();
            GL.Begin(BeginMode.Polygon);
            // Face 6
            GL.Color4(myBottomColor);
            GL.Vertex3(x1, 0, zi);
            GL.Vertex3(x1, 0, zi + zwidth);
            GL.Vertex3(x2, 0, zi + zwidth);
            GL.Vertex3(x2, 0, zi);
            GL.End();
        }

        /// <summary>
        /// Method for drawing the grid that encapsulate the FFT graphs.
        /// </summary>
        /// <param name="height">The height of the grids, as determined by the maximum value
        /// of the FFT graphs.</param>
        private void DrawGridLevel(double height)
        {
            if (!Loaded) // Play nice
                return;
            // Cross Lines
            for (int x = 0; x < 51; x++)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex3(x, height, GlobalVars.ZWIDTH * GraphInformation.NCurves);
                GL.Vertex3(x, height, 0);
                GL.End();
            }
            // Parallel Lines
            for (int z = 0; z < GraphInformation.NCurves + 1; z++)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex3(0, height, z * GlobalVars.ZWIDTH);
                GL.Vertex3(50, height, z * GlobalVars.ZWIDTH);
                GL.End();
            }

        }

        private void DrawAxesScale()
        {
            for (int i = 0; i < GlobalVars.GraphSteps3d + 1; i++)
            {
                int temp = ScaleList[i];
                if (((i * 5) <= LookAtXPos) && (LookAtXPos < ((i + 1) * 5)) && GraphInformation.NCurves != 0)
                {
                    TexUtil.InitTexturing();
                    GL.BindTexture(TextureTarget.Texture2D, CurrentValue[0]);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0, 0); GL.Vertex3(5 * i, 0, (this.GraphInformation.NCurves*5) + 20);
                    GL.TexCoord2(0, 1); GL.Vertex3(5 * i + 5, 0, (this.GraphInformation.NCurves*5) + 20);
                    GL.TexCoord2(1, 1); GL.Vertex3(5 * i + 5, 0, (this.GraphInformation.NCurves*5));
                    GL.TexCoord2(1, 0); GL.Vertex3(5 * i, 0, (this.GraphInformation.NCurves*5));
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);

                    TexUtil.InitTexturing();
                    GL.BindTexture(TextureTarget.Texture2D, CurrentValue[1]);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0, 0); GL.Vertex3(5 * i, GlobalVars.YACTMAX, 0);
                    GL.TexCoord2(0, 1); GL.Vertex3(5 * i + 5, GlobalVars.YACTMAX, 0);
                    GL.TexCoord2(1, 1); GL.Vertex3(5 * i + 5, GlobalVars.YACTMAX + 20, 0);
                    GL.TexCoord2(1, 0); GL.Vertex3(5 * i, GlobalVars.YACTMAX + 20, 0);
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                }
                else
                {
                    TexUtil.InitTexturing();
                    GL.BindTexture(TextureTarget.Texture2D, temp);

                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0, 0); GL.Vertex3(5 * i, GlobalVars.YACTMAX, 0);
                    GL.TexCoord2(0, 1); GL.Vertex3(5 * i + 5, GlobalVars.YACTMAX, 0);
                    GL.TexCoord2(1, 1); GL.Vertex3(5 * i + 5, GlobalVars.YACTMAX + 20, 0);
                    GL.TexCoord2(1, 0); GL.Vertex3(5 * i, GlobalVars.YACTMAX + 20, 0);
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                }
            }
        }


        public void UpdateMenu()
        {
            ToolStripMenuItem tsmitem = new ToolStripMenuItem("Band");
            //if (BandObjList.Count > 0)
            //{
            //    zoomMenuStrip.DropDownItems.Add(tsmitem);
            //}
            //for(int i = 0; i < BandObjList.Count; i++)
            //{
            //    ToolStripMenuItem tsmi = new ToolStripMenuItem("Band " +(i+1));
            //    BandObj temp = BandObjList.Get(i);
            //    String bandObjInfo = "Bandwidth: " + temp.BANDWIDTH + Environment.NewLine +
            //                            "Center Frequency: " + temp.FREQ + Environment.NewLine +
            //                            "Alarm Status: " + (temp.ALARM < temp.BAND_SUM) ;
            //    ToolStripMenuItem tsmiBandInfo = new ToolStripMenuItem(bandObjInfo);
            //    tsmi.DropDownItems.Add(tsmiBandInfo);
            //    tsmiBandInfo.Tag = temp;
            //    tsmiBandInfo.Click += new EventHandler(BandZoomClick);
            //    tsmitem.DropDownItems.Add(tsmi);
            //    tsmi.Tag = tsmiBandInfo;
                
            //    tsmi.Click += new EventHandler(BandZoomClick);
            //}
            myRangeTrackBar= new RangeTrackBarToolStripItem();
            RangeTrackBar control = myRangeTrackBar.Control as RangeTrackBar;
            if (control != null)
            {
                control.SetXMax(GlobalVars.XMAX);
                control.SetXMin(0);
                control.ScaleChangedEvent += new RangeTrackBar.ScaleChanged(control_ScaleChangedEvent);
            }
            rangeToolStripMenuItem.DropDownItems.Add(myRangeTrackBar);
        }

        bool control_ScaleChangedEvent(object sender, EventArgs e)
        {
            RangeTrackBar rtb = sender as RangeTrackBar;
            if (rtb != null)
            {
                this.xMax = GlobalVars.XMAX * rtb.XMaxPercent;
                this.xMin = GlobalVars.XMAX * rtb.XMinPercent;
                rtb.SetXMax(xMax);
                rtb.SetXMin(xMin);
                GlobalVars.ScaleChanged = true;
                glControl1.Invalidate();
            }
            return false;
        }

        private void updateRTBRange()
        {
            RangeTrackBar rtb = myRangeTrackBar.Control as RangeTrackBar;
            if (rtb != null)
            {
                rtb.SetXMax(this.xMax);
                rtb.SetXMin(this.xMin);
            }
        }

        void BandZoomClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi != null)
            {
                BandObj tag = tsmi.Tag as BandObj;
                if (tag != null)
                {
                    this.xMin = tag.FREQ - tag.BANDWIDTH / 2;
                    this.xMax = tag.FREQ + tag.BANDWIDTH / 2;
                    updateRTBRange();
                    GlobalVars.ScaleChanged = true;
                    glControl1.Invalidate();
                }
                else
                {
                    ToolStripMenuItem tsmiChild = tsmi.Tag as ToolStripMenuItem;
                    if (tsmiChild != null)
                    {
                        tsmiChild.PerformClick();
                    }
                }
            }
        }

        private void DrawAxesPlane()
        {
            if (!Loaded) // Play nice
                return;
            #region X-Y Plane
            // X-Y Plane GREEN
            // Horizontal Lines
            for (int y = 0; y < 26; y++)
            {
                GL.Begin(BeginMode.Lines);

                if (y % 5 == 0)
                {
                    GL.Color3(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    GL.Color3(0.0f, 0.0f, 0.0f);
                }
                GL.Vertex3(0, y, 0);
                GL.Vertex3(200, y, 0);
                GL.End();
            }

            // Vertical Lines
            for (int x = 0; x < 201; x++)
            {
                GL.Begin(BeginMode.Lines);
                if (MyCamera.GetOrigin().getX() == x)
                {
                    GL.Color3(1.0f, 0.0f, 0.0f);
                }
                else
                {
                    if (x % 5 == 0)
                    {
                        GL.Color3(1.0f, 1.0f, 1.0f);
                    }
                    else
                    {
                        GL.Color3(0.0f, 0.0f, 0.0f);
                    }
                }
                GL.Vertex3(x, 0, 0);
                GL.Vertex3(x, 25, 0);
                GL.End();

            }
            #endregion

            #region Z-Y Plane

            // Z-Y Plane GREEN
            // Horizontal Lines
            for (int y = 0; y < 26; y++)
            {
                GL.Begin(BeginMode.Lines);

                if (y % 5 == 0)
                {
                    GL.Color3(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    GL.Color3(0.0f, 0.0f, 0.0f);
                }
                GL.Vertex3(0, y, 0);
                GL.Vertex3(0, y, GraphInformation.NCurves * 5);
                GL.End();
            }

            // Vertical Lines
            for (int z = 0; z < GraphInformation.NCurves + 1; z++)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.Vertex3(0, 0, z * 5);
                GL.Vertex3(0, 25, z * 5);
                GL.End();
            }

            #endregion

            // Z-Y Plane Blue
        }


        /// <summary>
        /// Draws a heads up display for the user to access information
        /// </summary>
        private void DrawHeadsUpDisplay()
        {
            if (!Loaded) // Play nice
                return;
            //TexUtil.InitTexturing();

            //System.Drawing.Bitmap bmp = new Bitmap(800, 600);
            //Graphics g = Graphics.FromImage(bmp);
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //g.Clear(Color.Transparent);
            //TextRenderer.DrawText(g, "Test won't wrap", new Font(FontFamily.GenericSansSerif, 48.0f, FontStyle.Bold), new Point(0, 0), Color.Black);

            //textureId = TexUtil.CreateTextureFromBitmap(bmp);
            //GL.BindTexture(TextureTarget.Texture2D, textureId);
            //GL.Begin(BeginMode.Quads);
            //GL.TexCoord2(0, 0); GL.Vertex3(0, glControl1.Height-50, 0);
            //GL.TexCoord2(0, 1); GL.Vertex3(0, 0, 0);
            //GL.TexCoord2(1, 1); GL.Vertex3(glControl1.Width, 0, 0);
            //GL.TexCoord2(1, 0); GL.Vertex3(glControl1.Width, glControl1.Height-50, 0);
            //GL.End();

            //GL.Disable(EnableCap.Texture2D);

            CurrentGraphNameThreeD(CurrentFileIndex);
        }

        private void SwitchToOrthographicView()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, glControl1.Width, 0, glControl1.Height, -5, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void SwitchToFrustrumView()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
        }

        /// <summary>
        /// Adds the text represented by the bitmap index to the graph, used for 
        /// labeling the various files' FFT plots
        /// </summary>
        /// <param name="index">The index value which represents the bitmap created 
        /// in order to add a rendered text to the OpenTK screen.</param>
        private void CurrentGraphNameThreeD(int index)
        {
            if (!Loaded || GraphInformation.NCurves == 0) // Play nice
                return;
            int temp = BitmapNameList[index];
            GL.Color3(Color.Transparent);
            TexUtil.InitTexturing();

            GL.BindTexture(TextureTarget.Texture2D, temp);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0); GL.Vertex3(0, glControl1.Height / 12.64, 0);
            GL.TexCoord2(0, 1); GL.Vertex3(0, glControl1.Height / 63.2, 0);
            GL.TexCoord2(1, 1); GL.Vertex3(glControl1.Width, glControl1.Height / 63.2, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(glControl1.Width, glControl1.Height / 12.64, 0);
            //GL.TexCoord2(0, 0); GL.Vertex3(0, 25, index * 5);
            //GL.TexCoord2(0, 1); GL.Vertex3(0, 25, 5 * (index + 1)); 
            //GL.TexCoord2(1, 1); GL.Vertex3(0, 0, 5 * (index + 1));
            //GL.TexCoord2(1, 0); GL.Vertex3(0, 0, index * 5);

            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }

        private int MakeStepBitmap(double label)
        {
            if (!Loaded)
                return -1;
            TexUtil.InitTexturing();
            string str = String.Format("{0:0000.0}", label).PadLeft(7, ' ');

            System.Drawing.Bitmap bmp = new Bitmap(800, 200);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);
            TextRenderer.DrawText(g, str, new Font(FontFamily.GenericSansSerif, 112.0f, FontStyle.Bold), new Point(0, 0), Color.Black,TextFormatFlags.Left);
            int textureId = TexUtil.CreateTextureFromBitmap(bmp);
            GL.Disable(EnableCap.Texture2D);
            return textureId;
        }







        struct Byte4
        {
            public byte R, G, B, A;

            public Byte4(byte[] input)
            {
                R = input[0];
                G = input[1];
                B = input[2];
                A = input[3];
            }

            public uint ToUInt32()
            {
                byte[] temp = new byte[] { this.R, this.G, this.B, this.A };
                return BitConverter.ToUInt32(temp, 0);
            }

            public override string ToString()
            {
                return this.R + ", " + this.G + ", " + this.B + ", " + this.A;
            }
        }

        struct Vertex
        {
            public Byte4 Color; // 4 bytes
            public Vector3 Position; // 12 bytes

            public const byte SizeInBytes = 16;
        }



        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 5.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
            glControl1.Invalidate();
        }

        private void glControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //_3DMenuMouseMenu.Show(Cursor.Position);
            }
        }

        /// <summary>
        /// Event for when the glControl1 is focused and the mouse moves, this should be used when
        /// navigating through the screen using the mouse.
        /// </summary>
        /// <param name="sender">The object that sent this event.</param>
        /// <param name="e">The arguments for the MouseEvent, should include the X and Y coordinates.</param>
        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Byte4 Pixel = new Byte4();
            //GL.ReadPixels(e.X, glControl1.Height - e.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixel);
            //Console.WriteLine(e.X + "," + e.Y);

            //Console.WriteLine(Pixel.ToString());
            //curveLabel3D.Location = new System.Drawing.Point(e.X+13, e.Y+15);
            double xmin = 0, xmax = 0, ymin = 0, ymax = 0, rms = 0;
            if (GraphInformation.NCurves != 0)
            {
                GraphInformation.CurveTypeItems[CurrentFileIndex].GetCurveValues(ref xmin, ref xmax, ref ymin, ref ymax, ref rms);

                //curveLabel3D.Text = igList[CurrentFileIndex].getFileName() + "\rX-Min: " + xmin + "\rX-Max: " + xmax + "\rY-Min: " + ymin + "\rY-Max: " + ymax + "\rRMS Value: " + rms;
                //curveLabel3D.BackColor = Color.FromArgb(25, 51, 127);
                //curveLabel3D.Visible = false;
            }



            if (MousePressed)
            {
                if (initXMM != e.X)
                { // If we have moved in the X direction

                    deltaXMM += e.X - initXMM;
                    mouseXDelta += deltaXMM;
                    MyCamera.moveCamera((deltaXMM * pi) / 720, true);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                    deltaXMM = 0;
                }
                if (initYMM != e.Y)
                { // If we have moved in the Y direction
                    deltaYMM += e.Y - initYMM;
                    mouseYDelta += deltaYMM;
                    MyCamera.moveCamera(-(deltaYMM * pi) / 720, false);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                    deltaYMM = 0;
                }

            }
            initXMM = e.X;
            initYMM = e.Y;



            //Console.WriteLine("X Position: " + initXMM + "  Y Position: " + initYMM);
            //Console.WriteLine("Delta X: " + deltaXMM + " Delta Y: " + deltaYMM);
            //Works
            if (!Loaded)
                return;
            if (CameraPan)
            {
                int deltaX = e.X - xLast;
                int deltaY = e.Y - yLast;
                xLast = e.X;
                yLast = e.Y;
                MouseSpeedArr[0] *= (float)(5 * pi / 180);
                MouseSpeedArr[1] *= (float)(5 * pi / 180);
                //mouseSpeed[0] *= 0.9f;
                //mouseSpeed[1] *= 0.9f;
                MouseSpeedArr[0] += Mouse.YDelta / 100f;
                MouseSpeedArr[1] += Mouse.XDelta / 100f;
                //cameraMatrix = Matrix4.Mult(cameraMatrix, Matrix4.CreateRotationZ(0.05f));
                CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.RotateY(MouseSpeedArr[0]));

                CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.RotateX(MouseSpeedArr[1]));


            }

            glControl1.Invalidate();
        }

        /// <summary>
        /// Event for when the glControl1 is focused and the mouse is held down, this should be used 
        /// when navigating through the screen using the mouse.
        /// </summary>
        /// <param name="sender">The object that sent this event.</param>
        /// <param name="e">The arguments for the mouseEvent, should include the X and Y coordinates.</param>
        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            //IEnumerator<CurveItemTypePair> ienum = GraphInformation.CurveTypeItems.GetEnumerator();
            //while (ienum.MoveNext())
            //{
            //    ienum.Current.CurveObj.Render(true);
            //}


            Byte4 Pixel = new Byte4();
            GL.ReadPixels(e.X, glControl1.Height - e.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixel);

            //Console.WriteLine(Pixel.ToString());

            IEnumerator<CurveItemTypePair> ienum2 = GraphInformation.CurveTypeItems.GetEnumerator();
            int igListIndex = 0;
            while (ienum2.MoveNext())
            {
                CurveItemTypePair curr = ienum2.Current;
                if (curr.CurveObj.m_colorID[0] == Pixel.R
                    && curr.CurveObj.m_colorID[1] == Pixel.G
                    && curr.CurveObj.m_colorID[2] == Pixel.B)
                {
                    CurrentFileIndex = igListIndex;
                }
                igListIndex++;
            }
            Console.WriteLine(Pixel.ToString());
            Console.WriteLine(CurrentFileIndex);

            MousePressed = true;
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MousePressed = false;
            MouseMovementAccumulator = 0;
            deltaXMM = 0;
            deltaYMM = 0;
        }

        /// <summary>
        /// Event for when the keyboard is used when the glControl is the focus of the program. The Keys used 
        /// are WASD for movement in one direction, as well as IJKL for movement around a designated spherical
        /// region. This method should be modified to allow the user for greater motion control, especially considering
        /// the intricasy and scope of navigating through several files each containing thousands of elements of data. 
        /// Additionally controls should be added to the mousemovement of the glControl itself to aid in the 
        /// viewing of critical data regions.
        /// </summary>
        /// <param name="sender">The object that sent this event.</param>
        /// <param name="e">The Arguments for the Key Event.</param>
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender.Equals(glControl1))
            {
                if (!Loaded)
                    return;


                if (e.KeyCode == Keys.I)
                {
                    MyCamera.moveCamera(1.50 * this.pi / 180, 0);
                    Console.WriteLine("Theta: " + MyCamera.getTheta() * 180 / this.pi + " Ro: " + MyCamera.getRo() * 180 / this.pi);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.K)
                {
                    MyCamera.moveCamera(-1.50 * this.pi / 180, 0);
                    Console.WriteLine("Theta: " + MyCamera.getTheta() * 180 / this.pi + " Ro: " + MyCamera.getRo() * 180 / this.pi);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.J)
                {
                    MyCamera.moveCamera(0, 1.50 * this.pi / 180);
                    Console.WriteLine("Theta: " + MyCamera.getTheta() * 180 / this.pi + " Ro: " + MyCamera.getRo() * 180 / this.pi);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.L)
                {
                    MyCamera.moveCamera(0, -1.50 * this.pi / 180);
                    Console.WriteLine("Theta: " + MyCamera.getTheta() * 180 / this.pi + " Ro: " + MyCamera.getRo() * 180 / this.pi);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.F1)
                {
                    MyCamera.moveRadius(1.0f);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.F2)
                {
                    MyCamera.moveRadius(-1.0f);
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                }
                if (e.KeyCode == Keys.Z)
                {
                    Point mouse;

                }
                if (e.KeyCode == Keys.F9)
                {
                    GlobalVars.ZWIDTH++;
                    GlobalVars.ScaleChanged = true;
                }
                if (e.KeyCode == Keys.F10)
                {
                    GlobalVars.ZWIDTH--;
                    GlobalVars.ScaleChanged = true;
                }


                if (e.KeyCode == Keys.B)
                {
                    DrawBinLevels = !DrawBinLevels;
                }

                if (e.KeyCode == Keys.W)
                {
                    MyCamera.changeLookAt(new Vertex3(LookAtXPos - 1, LookAtYPos, LookAtZPos));
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                    //cameraMatrix = Matrix4.Mult(cameraMatrix,
                    //             Matrix4.Translation(0f, 0f, 1f * 1));
                    //zPos += -1f * 1;
                }

                if (e.KeyCode == Keys.S)
                {
                    CameraMatrix = Matrix4.Mult(CameraMatrix,
                                 Matrix4.Translation(0f, 0f, -1f * 1));
                    ZAxesPosition += 1f * 1;
                }

                if (e.KeyCode == Keys.A)
                {
                    MyCamera.changeLookAt(new Vertex3(LookAtXPos - 1, LookAtYPos, LookAtZPos));
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                    LookAtXPos--;
                    DrawCurrentValue();
                    //cameraMatrix = Matrix4.Mult(cameraMatrix,
                    //                            Matrix4.RotateY((float)(-5.0*(pi/180.0))));

                    //cameraMatrix = Matrix4.Mult(cameraMatrix,
                    //             Matrix4.Translation(-1f * 1, 0f, 0f));
                    //xPos += -1f * 1;

                }

                if (e.KeyCode == Keys.D)
                {
                    MyCamera.changeLookAt(new Vertex3(LookAtXPos + 1, LookAtYPos, LookAtZPos));
                    CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));
                    LookAtXPos++;
                    DrawCurrentValue();
                    //cameraMatrix = Matrix4.Mult(cameraMatrix,
                    //                           Matrix4.RotateY((float)(5.0 * (pi / 180.0))));
                    //cameraMatrix = Matrix4.Mult(cameraMatrix,
                    //            Matrix4.Translation(1f * 1, 0f, 0f));
                    //xPos += 1f * 1;
                }
                if (e.KeyCode == Keys.P)
                {
                    CameraPan = !CameraPan;
                }

                if (e.KeyCode == Keys.X)
                {
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(XAxesPosition, YAxesPosition, ZAxesPosition));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.RotateX((float)(5 * pi / 180)));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(-XAxesPosition, -YAxesPosition, -ZAxesPosition));
                }
                if (e.KeyCode == Keys.Y)
                {
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(XAxesPosition, YAxesPosition, ZAxesPosition));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.RotateY((float)(5 * pi / 180)));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(-XAxesPosition, -YAxesPosition, -ZAxesPosition));
                }
                if (e.KeyCode == Keys.Z)
                {
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(XAxesPosition, YAxesPosition, ZAxesPosition));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.RotateZ((float)(5 * pi / 180)));
                    CameraMatrix = Matrix4.Mult(CameraMatrix, Matrix4.Translation(-XAxesPosition, -YAxesPosition, -ZAxesPosition));
                }

                //Console.WriteLine("X Pos:" + xPos + "\nY Pos:" + yPos + "\nZ Pos:" + zPos);

                if (e.KeyCode == Keys.D1)
                {
                    lineGraphToolStripMenuItem.Checked = DrawLineGraph = !DrawLineGraph;
                }

                if (e.KeyCode == Keys.D2)
                {
                    meshGraphToolStripMenuItem.Checked = DrawMesh = !DrawMesh;
                }

                if (e.KeyCode == Keys.D3)
                {
                    barGraphToolStripMenuItem.Checked = DrawBarGraph = !DrawBarGraph;
                }


                glControl1.Invalidate();
                return;
            }
        }

        private void glControl1_Leave(object sender, EventArgs e)
        {

        }


        public override void ChangeScale(int CI_index, String scale)
        {
            //base.ChangeScale(CI_index, scale);
            //MessageBox.Show("Not yet fully implemented");
        }

        private void xminTrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            if (tb == null) { return; }
            xMin = tb.Value;
            GlobalVars.YMAX = 0.01;
            GlobalVars.ScaleChanged = true;
            glControl1.Invalidate();
        }

        private void xmaxTrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            if (tb == null) { return; }
            xMax = tb.Value;
            GlobalVars.YMAX = 0.01;
            GlobalVars.ScaleChanged = true;
            glControl1.Invalidate();
        }

        private void lineGraphToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            DrawBarGraph = barGraphToolStripMenuItem.Checked;
            DrawLineGraph = lineGraphToolStripMenuItem.Checked;
            DrawMesh = meshGraphToolStripMenuItem.Checked;
            glControl1.Invalidate();
        }

    }
    

    public static class PointPairExtensionClass
    {

        public static PointPairList ToSlice(this PointPairList list, double min, double max)
        {
            if (max <= min) { min = 0; max = 1; }
            PointPairList temp = new PointPairList();
            int minindex = list.IndexOf(list.FirstOrDefault(a => a.X >= min));
            minindex = minindex < 0 ? 0 : minindex;
            int maxindex = list.FindLastIndex(delegate(PointPair pp) { return pp.X <= max; });
            temp.AddRange(list.GetRange(minindex, maxindex - minindex));
            return temp;
       }

        public static PointPairList Stretch(this PointPairList list, double max)
        {
            if (list.Count == 0) { return new PointPairList(); }
            PointPairList temp = new PointPairList();
            //double xstep = (max / (double)(list[list.Count - 1].X - list[0].X));
            double xstep = (max / list.Count);
            GlobalVars.YMAX = double.MinValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (GlobalVars.YMAX < list[i].Y) { GlobalVars.YMAX = list[i].Y; }
                //temp.Add(new PointPair((xstep * list[i].X), list[i].Y));
                temp.Add(new PointPair((xstep * i), list[i].Y));
            }
            return temp;
        }

        public static PointPairList Translate(this PointPairList list, int x, int y)
        {
            PointPairList temp = new PointPairList();
            for (int i = 0; i < list.Count; i++)
            {
                temp.Add(new PointPair(list[i].X + x, list[i].Y + y));
            }
            return temp;
        }
    }
}
