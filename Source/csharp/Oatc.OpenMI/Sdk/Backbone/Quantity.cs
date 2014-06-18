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
    /// The Quantity class contains a unit, description, id, and dimension.
    /// <para>This is a trivial implementation of OpenMI.Standard.IQuantity, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class Quantity : IQuantity
    {
        private IUnit unit = new Unit("[-]");
        private string description = string.Empty;
        private string caption = string.Empty;
        private Type valueType = typeof(double);
    	private Object missingDataValue = -999.0d;

    	/// <summary>
        /// Constructor
        /// </summary>
        public Quantity()
        {
        }

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caption">Caption</param>
        public Quantity(String caption)
        {
            this.caption = caption;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The quantity to copy</param>
        public Quantity(IQuantity source)
        {
            Description = source.Description;
            Caption = source.Caption;
            Unit = source.Unit;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <param name="description">Description</param>
        /// <param name="caption">Caption</param>
        public Quantity(IUnit unit, string description, string caption)
        {
            this.unit = unit;
            this.description = description;
            this.caption = caption;
        }

		/// <summary>
		/// Getter and setter for MissingDataValue
		/// </summary>
		public Object MissingDataValue
		{
			get { return missingDataValue; }
			set { missingDataValue = value; }
		}

		/// <summary>
        /// Getter and setter for Caption
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        /// <summary>
        /// Getter and setter for description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Type ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        public static bool DescribesSameAs(IValueDefinition valueDefinition1, IValueDefinition valueDefinition2)
        {
            if ( ((valueDefinition1 is IQuality) && (valueDefinition2 is IQuantity)) ||
                 ((valueDefinition1 is IQuantity) && (valueDefinition2 is IQuality))   )
                return false;
            if (valueDefinition1 is IQuantity)
            {
                return Dimension.DescribesSameAs(((IQuantity)valueDefinition1).Unit.Dimension, ((IQuantity)valueDefinition2).Unit.Dimension);
            }
            return true;
        }

        /// <summary>
        /// Getter and setter for unit
        /// </summary>
        public IUnit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        /// <summary>
        /// Returns the Caption
        /// </summary>
        /// <returns>Caption</returns>
        public override String ToString()
        {
            return Caption;
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
            Quantity q = (Quantity) obj;
            if (!Caption.Equals(q.Caption))
                return false;
            if (!Unit.Equals(q.Unit))
                return false;
            if (Description == null && q.Description != null)
                return false;
            if (Description != null && !Description.Equals(q.Description))
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
            if (unit != null) hashCode += unit.GetHashCode();
            return hashCode;
        }
    }
}