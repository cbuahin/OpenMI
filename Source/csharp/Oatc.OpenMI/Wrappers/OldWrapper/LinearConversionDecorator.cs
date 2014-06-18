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
#region Copyright

///////////////////////////////////////////////////////////
//
// namespace: Wrapper 
// purpose: Utility for wrapping model components
// file: LinearConversionDecorator .cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Lars Peter Engelbrecht Hansen & Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      13 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The LinearConversionDecorator class is an implementation of the IOutputItemDecorator interface. 
	/// The LinearConversionDecorator can make linear conversion on doubles. The ax+b type of operations.
	/// </summary>
	public class LinearConversionDecorator : DecoratorBase
	{
        private const string linearConversionDataOperationId = "Linear Conversion";

        private double a;
		private double b;
		private bool isActivated;

	    /// <summary>
		/// Constructor
		/// </summary>
        public LinearConversionDecorator() : base(linearConversionDataOperationId)
		{

			Argument arg = new Argument();
			arg.Description = "Parameter A. Used in conversion: A*x + B";
			arg.Caption = "Type";
			arg.Value = "Linear Conversion";
			arg.ReadOnly = true;
            Arguments.Add(arg);

            arg = new Argument();
			arg.Description = "Parameter A. Used in conversion: A*x + B";
			arg.Caption = "A";
			arg.Value = "1.0";
			arg.ReadOnly = false;
            Arguments.Add(arg);

            arg = new Argument();
			arg.Description = "Parameter B. Used in conversion: A*x + B";
			arg.Caption = "B";
			arg.Value = "0.0";
			arg.ReadOnly = false;
            Arguments.Add(arg);

			isActivated = false;
		}

	    public override IList Values
        {
            get
            {
                return PerformLinearConversion(DecoratedOutputItem.Values);
            }
        }

	    public override IList GetValues(IExchangeItem querySpecifier)
	    {
	        return Values;
	    }

	    /// <summary>
		/// The prepare method should be called before the PerformLinearConversion. This method is
		/// not part of the org.OpenMI.Standard.IDataOperation interface. This method will convert
		/// the arguments which originally are defined as strings to doubles and subsequently assign 
		/// these values to private field variables. The prepare method is introduced for performance
		/// reasons.
		/// </summary>
		public void Prepare()
		{
			bool argumentAWasFound = false;
			bool argumentBWasFound = false;

			for (int i = 0; i < Arguments.Count; i++)
			{
                if (Arguments[i].Caption == "A")
				{
                    a = Convert.ToDouble(Arguments[i].Value);
					argumentAWasFound = true;
				}

                if (Arguments[i].Caption == "B")
				{
                    b = Convert.ToDouble(Arguments[i].Value);
					argumentBWasFound = true;
				}
			}
			if (!argumentAWasFound || !argumentBWasFound)
			{
				throw new Exception("Missing argument in data operation: \"Linear Conversion\"");
			}
            isActivated = true;
        }

		/// <summary>
		/// The ValueSet is converted. This method does not support VectorSet, so if the ValueSet is a Vectorset
		/// an exception will be thrown. The parameters passed in this method is not used, since all needed information
		/// is already assigned in the Prepare method.
		/// </summary>
		/// <param name="values">argumens but not used in this method</param>
		/// <returns>The converted ValueSet</returns>
        public IList PerformLinearConversion(IList values)
		{
			if (! isActivated)
			{
			    Prepare();
			}

            IList valuesList = new List<double>();
            for (int i = 0; i < values.Count; i++)
			{
                valuesList.Add((double)values[i] * a + b);
			}
			return valuesList;
		}

	    public override void Update()
	    {
	        throw new NotImplementedException();
	    }
	}
}