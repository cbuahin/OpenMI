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
using OpenMI.Standard2;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary>
    /// 
    /// </summary>
    public enum PredefinedUnits
    {
        Meter,
        Liter,
        CubicMeterPerSecond,
        MillimeterPerDay,
        LiterPerSecond
    }

    /// <summary>
    /// The Unit class defines a unit for a quantity.
    /// <para>This is a trivial implementation of OpenMI.Standard.IUnit, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class Unit : IUnit
    {
        private string description = string.Empty;
        private string caption = string.Empty;
        private IDimension dimension = new Dimension();
        private double conversionFactor = 1;
        private double conversionOffset = 0;

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The unit to copy</param>
        public Unit(IUnit source)
        {
            Description = source.Description;
            Caption = source.Caption;
            ConversionFactorToSI = source.ConversionFactorToSI;
            OffSetToSI = source.OffSetToSI;
            Dimension = source.Dimension;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caption">Caption</param>
        /// <param name="conversionFactor">Conversion factor to SI</param>
        /// <param name="conversionOffset">Conversion offset to SI</param>
        public Unit(string caption, double conversionFactor, double conversionOffset)
        {
            this.caption = caption;
            this.conversionFactor = conversionFactor;
            this.conversionOffset = conversionOffset;
            description = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caption">Caption</param>
        /// <param name="conversionFactor">Conversion factor to SI</param>
        /// <param name="conversionOffset">Conversion offset to SI</param>
        /// <param name="description">Description</param>
        public Unit(string caption, double conversionFactor, double conversionOffset, string description)
        {
            this.caption = caption;
            this.conversionFactor = conversionFactor;
            this.conversionOffset = conversionOffset;
            this.description = description;
        }

        public Unit(PredefinedUnits predefinedUnit)
        {
            // default values for the properties
            double conversionFactorToBeUsed = 1.0;
            double conversionOffsetToBeUsed = 0.0;

            switch (predefinedUnit)
            {
                case PredefinedUnits.Meter:
                    caption = "m";
                    description = "meter";
                    dimension = new Dimension(PredefinedDimensions.Length);
                    break;
                case PredefinedUnits.Liter:
                    caption = "L";
                    description = "Liter";
                    dimension = new Dimension(PredefinedDimensions.Volume);
                    ConversionFactorToSI = 0.001;
                    break;
                case PredefinedUnits.CubicMeterPerSecond:
                    caption = "m3/s";
                    description = "cubic meter per second";
                    dimension = new Dimension(PredefinedDimensions.VolumePerTime);
                    break;
                case PredefinedUnits.LiterPerSecond:
                    caption = "liter/s";
                    description = "liter per second";
                    dimension = new Dimension(PredefinedDimensions.VolumePerTime);
                    break;
                case PredefinedUnits.MillimeterPerDay:
                    caption = "mm/day";
                    description = "millimeters per day";
                    conversionFactorToBeUsed = 1.15741E-08;
                    dimension = new Dimension(PredefinedDimensions.VolumePerTimePerArea);
                    break;
            }

            // set properties
            ConversionFactorToSI = conversionFactorToBeUsed;
            OffSetToSI = conversionOffsetToBeUsed;
        }

        public Unit(string caption)
        {
            Caption = caption; 
        }

        /// <summary>
        /// Getter and setter for description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Getter and setter for Dimension
        /// </summary>
        public IDimension Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        /// <summary>
        /// Getter and setter for conversion factor to SI
        /// </summary>
        public double ConversionFactorToSI
        {
            get { return conversionFactor; }
            set { conversionFactor = value; }
        }

        /// <summary>
        /// Getter and setter for offset to SI
        /// </summary>
        public double OffSetToSI
        {
            get { return conversionOffset; }
            set { conversionOffset = value; }
        }

        /// <summary>
        /// Getter and setter for Caption
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
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
            Unit u = (Unit) obj;
            if (!Caption.Equals(u.Caption))
                return false;
            if (!Description.Equals(u.Description))
                return false;
            if (!ConversionFactorToSI.Equals(u.ConversionFactorToSI))
                return false;
            if (!OffSetToSI.Equals(u.OffSetToSI))
                return false;
            return true;
        }

        ///<summary>
        /// Get Hash Code.
        ///</summary>
        ///<returns>Hash Code for the current instance.</returns>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            if (caption != null) hashCode += caption.GetHashCode();
            return hashCode;
        }
    }
}