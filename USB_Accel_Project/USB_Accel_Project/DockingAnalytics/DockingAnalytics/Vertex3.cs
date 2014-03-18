using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace DockingAnalytics
{
    /// <summary>
    /// The Vertex3 class provides the user with options for adding Color's to Vectors, this is important for the use in
    /// the 3D Spectrographic analysis, because a collection of Vertex3's will allow the user to define a seperate color
    /// for each individual Vector, this allows the mesh and line plots to proceed with ease.
    /// </summary>
    public class Vertex3
    {
        #region Local Variable Declarations
        private float x, y, z;
        private Color4 myColor;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the Vertex3 class, takes float variables for the x y and z positions.
        /// </summary>
        /// <param name="x">The float representation of the Vertex3's x position.</param>
        /// <param name="y">The float representation of the Vertex3's y position.</param>
        /// <param name="z">The float representation of the Vertex3's z position.</param>
        public Vertex3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructor for the Vertex3 class, takes float variables for the x y and z positions, as well as a color.
        /// </summary>
        /// <param name="x">The float representation of the Vertex3's x position.</param>
        /// <param name="y">The float representation of the Vertex3's y position.</param>
        /// <param name="z">The float representation of the Vertex3's z position.</param>
        /// <param name="color">The color of this particular Vertex3.</param>
        public Vertex3(float x, float y, float z, Color4 color)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.myColor = color;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// Accessor for the color of this Vertex3 instance.
        /// </summary>
        /// <returns>The Color4 object that represents this color to be used in the 3D Spectrograph environment.</returns>
        public Color4 getColor()
        {
            return myColor;
        }

        /// <summary>
        /// Accessor for the values of the x y and z coordinates.
        /// </summary>
        /// <returns>A double[] array representing the x y and z coordinates</returns>
        public double[] getVals()
        {
            double[] vals = { x, y, z };
            return vals;
        }

        /// <summary>
        /// Accessor for the Vector3 equivalent of this Vertex3 instance.
        /// </summary>
        /// <returns>The Vector3 equivalent of this Vertex3 instance.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }

        /// <summary>
        /// Accessor for the x value of the Vertex3 instance.
        /// </summary>
        /// <returns>The x value of the Vertex3 instance.</returns>
        public double getX() { return x; }

        /// <summary>
        /// Accessor for the y value of the Vertex3 instance.
        /// </summary>
        /// <returns>The y value of the Vertex3 instance.</returns>
        public double getY() { return y; }

        /// <summary>
        /// Accessor for the z value of the Vertex3 instance.
        /// </summary>
        /// <returns>The z value of the Vertex3 instance.</returns>
        public double getZ() { return z; }
        #endregion

        #region Mutators

        /// <summary>
        /// Mutator for the color of this Vertex3 object, this is important to use when things change, such as the 
        /// FFT mesh, or the files are removed or added.
        /// </summary>
        /// <param name="color">The color which we want to set this instance of Vertex3 to.</param>
        public void setColor(Color4 color)
        {
            myColor = color;
        }
        #endregion

    }
}
