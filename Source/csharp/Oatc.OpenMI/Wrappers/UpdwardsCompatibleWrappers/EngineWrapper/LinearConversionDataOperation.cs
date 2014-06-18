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
// file: LinearConversionDataOperation .cs
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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.UpwardsComp.Backbone;
using Oatc.UpwardsComp.Standard;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.UpwardsComp.EngineWrapper
{
	/// <summary>
	/// The LinearDataOperation class is an implementation of the IDataOperation interface. 
	/// The LinearDataOperation can make linear conversion on ScalarSets. The ax+b type of operations.
	/// </summary>
	public class LinearConversionDataOperation : IDataOperation
	{
		private double _a;
		private readonly IArgument[] _arguments;
		private double _b;
		private bool _isActivated;

		/// <summary>
		/// Constructor
		/// </summary>
		public LinearConversionDataOperation()
		{
			_arguments = new IArgument[2];

		    _arguments[1] = new ArgumentDouble("A", 1, 1);
			_arguments[1].Description = "Parameter A. Used in conversion: A*x + B";
			_arguments[1].Caption = "A";
			_arguments[1].Value = "1.0";

		    _arguments[2] = new ArgumentDouble("B", 0, 0);
			_arguments[2].Description = "Parameter B. Used in conversion: A*x + B";
			_arguments[2].Caption = "B";
			_arguments[2].Value = "0.0";

			_isActivated = false;
		}

		/// <summary>
		/// DataOperation ID. In this class always "Linear Conversions" (is hardcoded)
		/// </summary>
		public string ID
		{
			get { return "Linear Conversion"; }
		}

		/// <summary>
		/// Number of dataoperation arguments. For the Linear dataoperation this number is always 3 (coefficient a, offset b and description text)
		/// </summary>
		public int ArgumentCount
		{
			get { return _arguments.Length; }
		}

		/// <summary>
		/// The linear dataoperation is valid for any input and output exchange items and can be combined with any other
		/// dataopertion, consequently this method always return true.
		/// See also documentation for : org.OpenMI.Standard.IDataOperation for details
		/// </summary>
		/// <param name="input">inputItem</param>
		/// <param name="output">outputItem</param>
		/// <returns></returns>
		public bool IsValid(ITimeSpaceInput input, ITimeSpaceOutput output)
		{
			return true;
		}

		/// <summary>
		/// Initialises the data operation. Nothing is done for the Linear dataoperation
		/// </summary>
		/// <param name="properties">arguments</param>
		public void Initialize(IArgument[] properties)
		{
		}

		/// <summary>
		/// Returns the arguments for the Linear Dataoperation
		/// </summary>
		/// <param name="argumentIndex">Argument index</param>
		/// <returns></returns>
		public IArgument GetArgument(int argumentIndex)
		{
			return _arguments[argumentIndex];
		}

		/// <summary>
		/// The prepare method should be called before the PerformDataOperation. This method is
		/// not part of the org.OpenMI.Standard.IDataOperation interface. This method will convert
		/// the arguments which originally are defined as strings to doubles and subsequently assign 
		/// these values to private field variables. The prepare method is introduced for performance
		/// reasons.
		/// </summary>
		public void Prepare()
		{
			bool argumentAWasFound = false;
			bool argumentBWasFound = false;

			_isActivated = true;

			for (int i = 0; i < _arguments.Length; i++)
			{
				if (_arguments[i].Caption == "A")
				{
					_a = Convert.ToDouble(_arguments[i].Value);
					argumentAWasFound = true;
				}

				if (_arguments[i].Caption == "B")
				{
					_b = Convert.ToDouble(_arguments[i].Value);
					argumentBWasFound = true;
				}
			}
			if (!argumentAWasFound || !argumentBWasFound)
			{
				throw new Exception("Missing argument in data operation: \"Linear Conversion\"");
			}
		}

		/// <summary>
		/// The ValueSet is converted. This method does not support VectorSet, so if the ValueSet is a Vectorset
		/// an exception will be thrown. The parameters passed in this method is not used, since all needed information
		/// is already assigned in the Prepare method.
		/// </summary>
		/// <param name="values">argumens but not used in this method</param>
		/// <returns>The converted ValueSet</returns>
        public Oatc.UpwardsComp.Standard.IValueSet PerformDataOperation(Oatc.UpwardsComp.Standard.IValueSet values)
		{
			if (_isActivated)
			{
				if (!(values is IScalarSet))
				{
					throw new Exception("The Wrapper packages only supports ScalarSets (Not VectorSets)");
				}

				double[] x = new double[values.Count];

				for (int i = 0; i < values.Count; i++)
				{
					x[i] = ((IScalarSet) values).GetScalar(i)*_a + _b;
				}

				return new ScalarSet(x);
			}

			return values; // return the values unchanged.
		}

		/// <summary>
		/// The linear dataoperation is valid for any input and output exchange items and can be combined with any other
		/// dataopertion, consequently this method always return true.
		/// See also documentation for : org.OpenMI.Standard.IDataOperation for details
		/// </summary>
		/// <param name="inputExchangeItem"></param>
		/// <param name="outputExchangeItem"></param>
		/// <param name="SelectedDataOperations"></param>
		/// <returns></returns>
		public bool IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem,
		                    IDataOperation[] SelectedDataOperations)
		{
			return true;
		}
	}
}