using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{
    /// <summary>
    /// The SphereCamera class provides the translations needed to look at a particular location from 
    /// a particular sphere that contains that location as its central point.
    /// </summary>
    class SphereCamera
    {

        #region Local Variable Declarations

        private double theta = 0;
        private double ro = 0;
        private Vertex3 normal;
        //private double PI = 3.14159;
        //private static double elevationAngle = 90.0;
        //private static double inclinationAngle = 180.0;
        private Vertex3 lookAT;
        private float radius;
        private float x = 0.0f, y = 0.0f, z = 0.0f;
        #endregion

        #region Constructor(s) for SphereCamera Class

        /// <summary>
        /// Constructor for the SphereCamera class, initiates variables according to the parent call to the
        /// constructor.
        /// </summary>
        /// <param name="origin">The vertex3 that represents the origin of the sphere(the central point).</param>
        /// <param name="radius">The value of the radius of the sphere, allows for the camera to be positioned
        /// at radius distance from the origin.</param>
        public SphereCamera(Vertex3 origin, float radius)
        {
            lookAT = origin;
            this.radius = radius;
            x = (float)((this.radius * Math.Sin(theta) * Math.Cos(ro))+lookAT.getVals()[0]);
            z = (float)((this.radius * Math.Sin(theta) * Math.Sin(ro))+lookAT.getVals()[1]);
            y = (float)((this.radius * Math.Cos(theta)) + lookAT.getVals()[2]);
            normal = new Vertex3((float)(-Math.Sin(ro) * Math.Cos(theta)), (float)(-Math.Sin(ro) * Math.Sin(theta)),(float) -Math.Cos(ro));
        }
        #endregion

        #region Move Camera Region

        /// <summary>
        /// The moveCamera method accepts the theta and ro delta values, (the latitude and longitude angle changes)
        /// and performs the correct translation of the camera position.
        /// NOTE: This may need to be changed to reflect a change after it has already moved, currently
        /// I believe it resets after every move, so a subsequent move will have the camera at a different orientation
        /// than it begins with.
        /// </summary>
        /// <param name="deltaTheta">The latitude change of the camera.</param>
        /// <param name="deltaRo">The longitude change of the camera.</param>
        public void moveCamera(double deltaTheta, double deltaRo)
        {
            theta += deltaTheta;
            ro += deltaRo;
            x = (float)((this.radius * Math.Sin(theta) * Math.Cos(ro))+lookAT.getVals()[0]);
            z = (float)((this.radius * Math.Sin(theta) * Math.Sin(ro))+lookAT.getVals()[1]);
            y = (float)((this.radius * Math.Cos(theta)) + lookAT.getVals()[2]); 
            normal = new Vertex3((float)(-Math.Sin(ro) * Math.Cos(theta)), (float)(-Math.Sin(ro) * Math.Sin(theta)), (float)-Math.Cos(ro));
        }

        /// <summary>
        /// The moveCamera method accepts a delta angle value, and then a bool which determines which angle the camera should
        /// be moving against. The other angle is not used, so we can get a clear left-to-right or up-to-down movement.
        /// </summary>
        /// <param name="delta">The angle which we are to use.</param>
        /// <param name="isRo">The bool that determines wether this angle is a deltaRO or a deltaTHETA angle.</param>
        public void moveCamera(double delta, bool isRo)
        {
            if (isRo)
            {// If this is a ro angle
                ro += delta;
            }
            else
            {
                theta += delta;
            }
            x = (float)((this.radius * Math.Sin(theta) * Math.Cos(ro)) + lookAT.getVals()[0]);
            z = (float)((this.radius * Math.Sin(theta) * Math.Sin(ro)) + lookAT.getVals()[1]);
            y = (float)((this.radius * Math.Cos(theta)) + lookAT.getVals()[2]);
            normal = new Vertex3((float)(-Math.Sin(ro) * Math.Cos(theta)), (float)(-Math.Sin(ro) * Math.Sin(theta)), (float)-Math.Cos(ro));
        }

        /// <summary>
        /// The moveRadius method accepts the parameter of the change in radius and changes the value of 
        /// the radius to reflect that. This should simply move the camera in or out of the origin viewing plane.
        /// </summary>
        /// <param name="deltaRad"></param>
        public void moveRadius(float deltaRad)
        {
            this.radius += deltaRad;
            moveCamera(0, 0);
        }
        #endregion

        #region Mutators to Private Variables

        /// <summary>
        /// Mutator method for changing the point at which the camera looks.
        /// </summary>
        /// <param name="newLookAt">The new point we wish to look at.</param>
        /// <returns>True if the lookAt variable is changed, false otherwise.</returns>
        public bool changeLookAt(Vertex3 newLookAt)
        {
            if (newLookAt != null)
            {
                lookAT = newLookAt;
                moveCamera(0, 0);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Accessors to Private Variables

        /// <summary>
        /// Accessor for the normal Vertex3 that represents our position
        /// </summary>
        /// <returns>Returns the normal variable that represents the position and direction of camera.</returns>
        public Vertex3 getNormal() { return normal; }

        /// <summary>
        /// Accessor for the current position of the SphereCamera
        /// </summary>
        /// <returns>The current vertex position of the camera.</returns>
        public Vertex3 GetPosition()
        {
            return new Vertex3(x, y, z);
        }

        /// <summary>
        /// Accessor for the x position of the camera.
        /// </summary>
        /// <returns>The x value of the camera's position.</returns>
        public double getX() { return x; }

        /// <summary>
        /// Accessor for the y position of the camera.
        /// </summary>
        /// <returns>The y value of the camera's position.</returns>
        public double getY() { return y; }

        /// <summary>
        /// Accessor for the z position of the camera.
        /// </summary>
        /// <returns>The z value of the camera's position.</returns>
        public double getZ() { return z; }

        /// <summary>
        /// Accessor for the theta angle of the camera around the sphere.
        /// </summary>
        /// <returns>Returns the theta angle of the camera.</returns>
        public double getTheta() { return theta; }

        /// <summary>
        /// Accessor for the ro angle of the camera around the sphere.
        /// </summary>
        /// <returns>Reutnrs the ro angle of the camera.</returns>
        public double getRo() { return ro; }

        /// <summary>
        /// Accessor for the origin position of the camera, the Vertex3 position that the camera is directed towards.
        /// </summary>
        /// <returns>The Vertex3 value of the cameras [lookAT] variable, or a representation of the 
        /// position that the camera is directed towards.</returns>
        public Vertex3 GetOrigin() { return lookAT; }
        #endregion

    }
}
