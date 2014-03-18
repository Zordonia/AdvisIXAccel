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
    public partial class TestGL : Document
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
            ScaleList.AddRange(new int[GlobalVars.GraphSteps3d + 1]);
            MyCamera = new SphereCamera(new Vertex3(110, 0, 30), 157);
        }

        public TestGL()
        {
            InitializeProperties();
            InitializeComponent();
        }


        private void glControl1_Load(object sender, EventArgs e)
        {

            if ((Loaded == false) && (sender == null))
            {
                return;
            }
            Loaded = true;
            CameraMatrix = Matrix4.LookAt(MyCamera.GetPosition().ToVector3(), MyCamera.GetOrigin().ToVector3(), new Vector3(0, 1, 0));

            XAxesPosition = 225.0f;
            ZAxesPosition = (float)(GraphInformation.NCurves * 5.0 + 10);
            YAxesPosition = 30.0f;

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
            glControl1.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LoadMatrix(ref CameraMatrix);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(OpenTK.Graphics.OpenGL.DepthFunction.Lequal);

            DrawAxesPlane();
            DrawAxesScale();
            glControl1.SwapBuffers();
            GlobalVars.ScaleChanged = false; 
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Console.WriteLine("RESIZED: "+this.Name);
            Console.WriteLine();
            //Console.WriteLine("X Position: " + ClientRectangle.X);
            //Console.WriteLine("Y Position: " + ClientRectangle.Y);
            //Console.WriteLine("Width: " + ClientRectangle.Width);
            //Console.WriteLine("Height: " + ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 5.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
            glControl1.Invalidate();
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
                    GL.TexCoord2(0, 0); GL.Vertex3(5 * i, 0, GlobalVars.ZMAX + 20);
                    GL.TexCoord2(0, 1); GL.Vertex3(5 * i + 5, 0, GlobalVars.ZMAX + 20);
                    GL.TexCoord2(1, 1); GL.Vertex3(5 * i + 5, 0, GlobalVars.ZMAX);
                    GL.TexCoord2(1, 0); GL.Vertex3(5 * i, 0, GlobalVars.ZMAX);
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


        private void glControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        /// <summary>
        /// Event for when the glControl1 is focused and the mouse moves, this should be used when
        /// navigating through the screen using the mouse.
        /// </summary>
        /// <param name="sender">The object that sent this event.</param>
        /// <param name="e">The arguments for the MouseEvent, should include the X and Y coordinates.</param>
        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double xmin = 0, xmax = 0, ymin = 0, ymax = 0, rms = 0;
            if (GraphInformation.NCurves != 0)
            {
                GraphInformation.CurveTypeItems[CurrentFileIndex].GetCurveValues(ref xmin, ref xmax, ref ymin, ref ymax, ref rms);
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
                    //DrawCurrentValue();
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
                    //DrawCurrentValue();
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
                    DrawLineGraph = !DrawLineGraph;
                }

                if (e.KeyCode == Keys.D2)
                {
                    DrawMesh = !DrawMesh;
                }

                if (e.KeyCode == Keys.D3)
                {
                    DrawBarGraph = !DrawBarGraph;
                }


                glControl1.Invalidate();
                return;
            }
        }





    }
}