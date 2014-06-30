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
using System.Collections;
using OpenMI.Standard2;

namespace Oatc.OpenMI.Sdk.Backbone
{
    public enum PredefinedDimensions
    {
        Length,
        Area,
        Volume,
        LengthPerTime,
        VolumePerTime,
        VolumePerTimePerLength,
        VolumePerTimePerArea,
        Mass,
        MassPerTime,
    }

    /// <summary>
	/// The Dimension class contains the dimension for a unit.
    /// <para>This is a trivial implementation of OpenMI.Standard.IDimension, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Dimension : IDimension
	{
        readonly double[] baseDimensionPower;

        /// <summary>
        /// Constructor
        /// </summary>
        public Dimension()
        {
            baseDimensionPower = new double[Enum.GetValues(typeof(DimensionBase)).Length];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Dimension(PredefinedDimensions predefinedDimension) : this()
        {
            switch (predefinedDimension)
            {
                case PredefinedDimensions.Length:
                    SetPower(DimensionBase.Length, 1);
                    break;
                case PredefinedDimensions.Area:
                    SetPower(DimensionBase.Length, 2);
                    break;
                case PredefinedDimensions.Volume:
                    SetPower(DimensionBase.Length, 3);
                    break;
                case PredefinedDimensions.LengthPerTime:
                    SetPower(DimensionBase.Length, 1);
                    SetPower(DimensionBase.Time, -1);
                    break;
                case PredefinedDimensions.VolumePerTime:
                    SetPower(DimensionBase.Length, 3);
                    SetPower(DimensionBase.Time, -1);
                    break;
                case PredefinedDimensions.VolumePerTimePerLength:
                    SetPower(DimensionBase.Length, 2);
                    SetPower(DimensionBase.Time, -1);
                    break;
                case PredefinedDimensions.VolumePerTimePerArea:
                    SetPower(DimensionBase.Length, 1);
                    SetPower(DimensionBase.Time, -1);
                    break;
                case PredefinedDimensions.Mass:
                    SetPower(DimensionBase.Mass, 1);
                    break;
                case PredefinedDimensions.MassPerTime:
                    SetPower(DimensionBase.Mass, 1);
                    SetPower(DimensionBase.Time, -1);
                    break;
            }
        }

        /// <summary>
		/// Returns the power of a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <returns>The power</returns>
		public double GetPower(DimensionBase baseQuantity)
		{
			return baseDimensionPower[(int) baseQuantity];
		}

		/// <summary>
		/// Sets a power for a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <param name="power">The power</param>
		public void SetPower(DimensionBase baseQuantity,double power)
		{
			baseDimensionPower[(int) baseQuantity] = power;
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="dim1">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public static bool DescribesSameAs(IDimension dim1, IDimension dim2) 
		{
            if (dim1 == null || dim2 == null) 
			 return false;
		    foreach (DimensionBase dimBase in Enum.GetValues(typeof(DimensionBase)))
		    {
                if (dim1.GetPower(dimBase) != dim2.GetPower(dimBase))
                    return false;
		    }
			return true;
		}
	}
}
