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
// file: SmartBufferDataOperation .cs
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
//  Created on:      18 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

using System;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The SmartBuffer data operation class is used to define the temporal relaxations factors and to define the level of validation 
	/// </summary>
	public class SmartBufferDataOperation
	{
		private readonly Argument[] _arguments;
		private bool _doExtendedValidation;
		private bool _isActivated;
		private double _relaxationFactor;

		/// <summary>
		/// Constructor
		/// </summary>
		public SmartBufferDataOperation()
		{
			_arguments = new Argument[3];

			_arguments[0] = new Argument();
			_arguments[0].Description = "Arguments associated the buffering and extrapolation";
			_arguments[0].Caption = "Type";
			_arguments[0].Value = "SmartBuffer Arguments";
			_arguments[0].ReadOnly = true;

			_arguments[1] = new Argument();
			_arguments[1].Description = "Relaxation factor used for temporal extrapolation must be in the interval [0.0,1.0]";
			_arguments[1].Caption = "Relaxation Factor";
			_arguments[1].Value = "0.0";
			_arguments[1].ReadOnly = false;

			_arguments[2] = new Argument();
			_arguments[2].Description = "Do extended validation. Must be \"true\" or \"false\"";
			_arguments[2].Caption = "Do Extended Data Validation";
			_arguments[2].Value = "true";
			_arguments[2].ReadOnly = false;

			_isActivated = false;
		}

		/// <summary>
		/// Number of arguments
		/// </summary>
		public int ArgumentCount
		{
			get { return _arguments.Length; }
		}

		/// <summary>
		/// If true the component will do extended data validation
		/// </summary>
		public bool DoExtendedValidation
		{
			get
			{
				if (!_isActivated)
				{
					throw new Exception(
						"Attemt to use DoExtendedValidation property in SmartBufferDataOperation before the prepare() method was invoked");
				}
				return _doExtendedValidation;
			}
		}

		/// <summary>
		/// Relaxation factor for temporal extrapolation
		/// </summary>
		public double RelaxationFactor
		{
			get
			{
				if (!_isActivated)
				{
					throw new Exception(
						"Attemt to use Relaxation property in SmartBufferDataOperation before the prepare() method was invoked");
				}
				return _relaxationFactor;
			}
		}

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="properties">parameters</param>
		public void Initialize(IArgument[] properties)
		{
		}

		/// <summary>
		/// get argument
		/// </summary>
		/// <param name="argumentIndex">index for the requested argument</param>
		/// <returns>the requested argument</returns>
		public IArgument GetArgument(int argumentIndex)
		{
			return _arguments[argumentIndex];
		}

		/// <summary>
		/// prepare
		/// </summary>
		public void Prepare()
		{
			bool argumentRelaxationFactorWasFound = false;
			bool argumentDoExtendedValidationWasFound = false;

			_isActivated = true;

			for (int i = 0; i < _arguments.Length; i++)
			{
				if (_arguments[i].Caption == _arguments[1].Caption) //Relaxation Factor
				{
					_relaxationFactor = Convert.ToDouble(_arguments[i].Value);
					argumentRelaxationFactorWasFound = true;
				}

				if (_arguments[i].Caption == _arguments[2].Caption) //Do extended validation
				{
					_doExtendedValidation = Convert.ToBoolean(_arguments[i].Value);
					argumentDoExtendedValidationWasFound = true;
				}
			}
			if (!argumentRelaxationFactorWasFound || !argumentDoExtendedValidationWasFound)
			{
				throw new Exception("Missing argument in data operation: \"Linear Conversion\"");
			}
		}
	}
}