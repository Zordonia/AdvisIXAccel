using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public class CurveObject : BaseObject
    {
        private int zIndex;
        private PointPairList pointPL;
        private List<Vertex3> vertices = new List<Vertex3>();
        private double zScale = 0.0, yScale = 0.0, xScale = 0.0;
        private PointPairList previous, next;
        private int currentFile = 0;
        private bool scaleChanged = false;
        private double X_MIN = 0;
        private double X_MAX = GlobalVars.XMAX;
        private double Y_MAX = GlobalVars.YMAX;
        private double X_ACTMAX = GlobalVars.XACTMAX;
        private double Z_WIDTH = GlobalVars.ZWIDTH;
        private double _curvePercToGraph = 0.1;
        public double CurvePercentageToGraph { get { return _curvePercToGraph; } set { _curvePercToGraph = value; } }
        public PointPairList PointPairList { get { return pointPL; } }


        public double XMax
        {
            get { return X_MAX; }
            set
            {
                scaleChanged = (X_MAX != value || scaleChanged); X_MAX = value;
            }
        }
        public double XMin
        {
            get { return X_MIN; }
            set
            {
                scaleChanged = (X_MIN != value || scaleChanged); X_MIN = value;
            }
        }

        /// <summary>
        /// Cunstructor for CurveObject, accepts the PointPairList of points contained within
        /// the curve.
        /// </summary>
        /// <param name="ppl">The PointPairList containing the pointpairs for this curve.</param>
        /// <param name="index">The zIndex of this curve.</param>
        public CurveObject(PointPairList ppl, int index)
            : base()
        {
            zIndex = index;
            pointPL = ppl;
            populateVertexArray();
        }

        /// <summary>
        /// Constructor for CurveObject, accepts the PointPairList of points contained within the curve, as 
        /// well as possible curves previous or next;
        /// </summary>
        /// <param name="ppl">The PointPairList containing the pointpairs for this curve.</param>
        /// <param name="index">The xIndex of this curve.</param>
        /// <param name="prevPPL">The PointPairList representing the previous PPL.</param>
        /// <param name="nextPPL">The PointPairList representing the next PPL.</param>
        public CurveObject(PointPairList ppl, int index, PointPairList prevPPL, PointPairList nextPPL)
            : base()
        {
            zIndex = index;
            pointPL = ppl;
            previous = prevPPL;
            next = nextPPL;
            populateVertexArray();
        }

        /// <summary>
        /// Mutator for previous PointPairList, if we change the previous PPL then we must also change the
        /// curve to represent the new colors.
        /// </summary>
        /// <param name="p">The PointPairList representing the previous PPL.</param>
        public void setPrevPPL(PointPairList p)
        {
            previous = p;
            populateVertexArray();
        }

        /// <summary>
        /// Mutator for next PointPairList, if we change the next PPL then we must also change the curve
        /// to represent new colors.
        /// </summary>
        /// <param name="n">The PointPairList representing the next PPL.</param>
        public void setNextPPL(PointPairList n)
        {
            next = n;
            populateVertexArray();
        }


        new public void Render(bool picking)
        {
            if (scaleChanged || GlobalVars.ScaleChanged)
            {
                populateVertexArray();
                scaleChanged = false;
            }
            if (picking)
            {
                GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit);
                GL.Disable(EnableCap.Fog);
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Dither);
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.LineStipple);
                GL.Disable(EnableCap.PolygonStipple);
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.Blend);
                //GL.Disable(EnableCap.AlphaTest);

                GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }


            IEnumerator<Vertex3> enumer = vertices.GetEnumerator();
            Vertex3 current;
            int accum = 0;
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Enable(EnableCap.ColorMaterial);
            //GL.LoadName(this.currentFile);
            while (enumer.MoveNext())// && accum < CurvePercentageToGraph*vertices.Count)
            {
                current = enumer.Current;
                if ((accum % 4) == 0)
                {
                    //GL.Material(MaterialFace.Front,MaterialParameter.AmbientAndDiffuse, current.getColor());
                    GL.Begin(BeginMode.Polygon);
                }
                GL.Material(MaterialFace.Front, MaterialParameter.Emission, current.getColor());
                accum++;
                if (picking)
                {
                    GL.Color4((byte)m_colorID[0] / 255.0, (byte)m_colorID[1] / 255.0, (byte)m_colorID[2] / 255.0, 1.0);
                }
                else
                {
                    GL.Color4(current.getColor());
                }
                GL.Vertex3(current.ToVector3());
                if ((accum % 4) == 0)
                {
                    GL.End();
                }
            }

            if (picking)
            {
                //GL.PopAttrib();
            }
        }

        private Color4 uniqueColor4(int hue)
        {
            int red = 0, green = 0, blue = 0;
            Random r = new Random();
            switch (hue)
            {
                case 0: // GREEN
                    red = r.Next(0, 116);
                    green = r.Next(127, 256);
                    blue = r.Next(0, 116);
                    break;
                case 1: // RED
                    red = r.Next(127, 256);
                    green = r.Next(0, 116);
                    blue = r.Next(0, 116);
                    break;
                case 2: // BLUE
                    red = r.Next(0, 116);
                    green = r.Next(0, 116);
                    blue = r.Next(127, 256);
                    break;
            }

            Color4 color = new Color4(red,green,blue,255);
            return color;
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
        private void addBoxVertices(float x1, float x2, float y, float zi, float zwidth, float ychange)
        {

            Color4 myTopColor = Color4.Green;
            Color4 myBottomColor = Color4.DarkGreen;
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
            //myTopColor.A = (byte)255;
            //myBottomColor.A = (byte)255;

            // Face 1
            vertices.Add(new Vertex3(x1, 0, zi, myBottomColor));
            vertices.Add(new Vertex3(x1, y, zi, myTopColor));
            vertices.Add(new Vertex3(x1, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x1, 0, zi + zwidth, myBottomColor));
            // Face 2
            vertices.Add(new Vertex3(x2, 0, zi, myBottomColor));
            vertices.Add(new Vertex3(x2, 0, zi + zwidth, myBottomColor));
            vertices.Add(new Vertex3(x2, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x2, y, zi, myTopColor));
            // Face 3
            vertices.Add(new Vertex3(x2, 0, zi + zwidth, myBottomColor));
            vertices.Add(new Vertex3(x2, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x1, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x1, 0, zi + zwidth, myBottomColor));
            // Face 4
            vertices.Add(new Vertex3(x2, 0, zi, myBottomColor));
            vertices.Add(new Vertex3(x2, y, zi, myTopColor));
            vertices.Add(new Vertex3(x1, y, zi, myTopColor));
            vertices.Add(new Vertex3(x1, 0, zi, myBottomColor));
            // Face 5
            vertices.Add(new Vertex3(x1, y, zi, myTopColor));
            vertices.Add(new Vertex3(x1, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x2, y, zi + zwidth, myTopColor));
            vertices.Add(new Vertex3(x2, y, zi, myTopColor));
            // Face 6
            vertices.Add(new Vertex3(x1, 0, zi, myBottomColor));
            vertices.Add(new Vertex3(x1, 0, zi + zwidth, myBottomColor));
            vertices.Add(new Vertex3(x2, 0, zi + zwidth, myBottomColor));
            vertices.Add(new Vertex3(x2, 0, zi, myBottomColor));
        }


        /// <summary>
        /// 
        /// </summary>
        private void populateVertexArray()
        {
            vertices.Clear();
            IEnumerator<PointPair> enumer = pointPL.ToSlice(X_MIN, X_MAX).Stretch(GlobalVars.XACTMAX).GetEnumerator();
            IEnumerator<PointPair> genum = pointPL.ToSlice(X_MIN, X_MAX).Stretch(GlobalVars.XACTMAX).GetEnumerator();
            if (previous != null && previous.Count == pointPL.Count)
            {
                genum = previous.ToSlice(X_MIN, X_MAX).Stretch(GlobalVars.XACTMAX).GetEnumerator();
            }
            double x = 0.0, y = 0.0, xb4 = 0.0, yb4 = 0.0;
            int accum = 0;
            double deltay = 0.0;
            double max = 0;
            while (enumer.MoveNext() )//&& accum < CurvePercentageToGraph*pointPL.Count)
            {
                genum.MoveNext();
                xb4 = x;
                yb4 = y;
                x = enumer.Current.X;

                y = enumer.Current.Y;
                deltay = genum.Current.Y - y;
                deltay = ((deltay / genum.Current.Y) * 100);
                if ((xb4 * (X_ACTMAX / X_MAX)) > max)
                {
                    max = (xb4 / 1024 * 50);
                }
                //if (tempind % 50 == 0)
                //{
                // * (X_ACTMAX / X_MAX))
                this.addBoxVertices((float)(xb4),// * (X_ACTMAX / X_MAX)), 
                    (float)(x),//* (X_ACTMAX / X_MAX) ),
                    (float)(y * (GlobalVars.YACTMAX / GlobalVars.YMAX)), 
                    (float)(zIndex * GlobalVars.ZWIDTH), 
                    (float)GlobalVars.ZWIDTH, (float)deltay);
                //x += 0.1;
                //xb4 += 0.1;
                //}
                accum++;
            }
            Console.WriteLine("Done");
        }

        public double XMAX { get { return X_MAX; } set { X_MAX = value; } }
        public double YMAX { get { return Y_MAX; } set { Y_MAX = value; } }


        /// <summary>
        /// Method for drawing the curve onto a GL surface.
        /// </summary>
        /// <returns>The Vertex3 list that contains the points to be plotted</returns>
        public List<Vertex3> getVertices()
        {
            if (vertices != null)
            {
                return vertices;
            }
            else
            {
                MessageBox.Show("Error: A curve object was requested when no vertices were present");
                return vertices;
            }
        }

    }
}
