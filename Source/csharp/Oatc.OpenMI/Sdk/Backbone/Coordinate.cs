#region Copyright

/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion

using System;

namespace Oatc.OpenMI.Sdk.Backbone
{

    /// <summary>
    /// Interface for a Coordinate, containing a (x,y,z,m) coordinate set
    /// </summary>
    public interface ICoordinate
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        double X { get; set; }
        
        /// <summary>
        /// Y coordinate
        /// </summary>
        double Y { get; set; }
        
        /// <summary>
        /// Z coordinate
        /// </summary>
        double Z { get; set; }

        /// <summary>
        /// M coordinate. In river applications also called chainage.
        /// </summary>
        double M { get; set; }
    }

    /// <summary>
    /// The Coordinate class contains a (x,y,z) coordinate
    /// </summary>
    [Serializable]
    public class Coordinate : ICoordinate
    {
        private double _x;
        private double _y;
        private double _z;

        /// <summary>
        /// Constructor
        /// </summary>
        public Coordinate()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public Coordinate(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Coordinate(double x, double y)
        {
            _x = x;
            _y = y;
            _z = double.NaN;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The vertex to copy</param>
        public Coordinate(Coordinate source)
        {
            X = source.X;
            Y = source.Y;
            Z = source.Z;
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public double M
        {
            get { return (double.NaN); }
            set { throw new NotSupportedException(); }
        }

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
        ///<param name="obj">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Coordinate v = (Coordinate) obj;
            return (X == v.X && Y == v.Y && Z == v.Z);
        }

        ///<summary>
        /// Get Hash Code.
        ///</summary>
        ///<returns>Hash Code for the current instance.</returns>
        public override int GetHashCode()
        {
            return _x.GetHashCode() + _y.GetHashCode() + _z.GetHashCode();
        }

        ///<summary>
        /// String representation of the vertext
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            return string.Format("Coordinate: {0}, {1}, {2}", _x, _y, _z);
        }
    }


    /// <summary>
    /// The Coordinate class contains a (x,y,z) coordinate
    /// </summary>
    [Serializable]
    public class CoordinateM : ICoordinate
    {
        private double _x;
        private double _y;
        private double _z;
        private double _m;

        /// <summary>
        /// Constructor
        /// </summary>
        public CoordinateM()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <param name="m">M coordinate</param>
        public CoordinateM(double x, double y, double z, double m)
        {
            _x = x;
            _y = y;
            _z = z;
            _m = m;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public CoordinateM(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            _m = double.NaN;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public CoordinateM(double x, double y)
        {
            _x = x;
            _y = y;
            _z = double.NaN;
            _m = double.NaN;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The vertex to copy</param>
        public CoordinateM(ICoordinate source)
        {
            X = source.X;
            Y = source.Y;
            Z = source.Z;
            M = source.M;
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Getter and setter for X coordinate
        /// </summary>
        public double M
        {
            get { return _m; }
            set { _m = value; }
        }

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
        ///<param name="obj">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public override bool Equals(object obj)
        {
            ICoordinate v = obj as ICoordinate;
            if (v == null)
                return false;
            return (X == v.X && Y == v.Y && Z == v.Z && M == v.M);
        }

        ///<summary>
        /// Get Hash Code.
        ///</summary>
        ///<returns>Hash Code for the current instance.</returns>
        public override int GetHashCode()
        {
            return _x.GetHashCode() + _y.GetHashCode() + _z.GetHashCode() + _m.GetHashCode();
        }

        ///<summary>
        /// String representation of the vertext
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            return string.Format("Coordinate: {0}, {1}, {2}, {3}", _x, _y, _z, _m);
        }
    }




}