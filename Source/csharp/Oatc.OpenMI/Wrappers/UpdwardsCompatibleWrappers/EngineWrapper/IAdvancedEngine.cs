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
// file: IAdvancedEngine.cs
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
//  Created on:      18 August 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

namespace Oatc.UpwardsComp.EngineWrapper
{
	/// <summary>
	/// The IAdvancedEngine interface is introduced in order to facilitate models
	/// where different quantities are calculated base on different time step lengths.
	/// One example of such model could be a multi-domain model such a models for combined
	/// ground water and surface water. Typically the time step length for the ground 
	/// water calculations will be much longer that the time step length for the surface 
	/// water calculations. In the IEngine interface values are pulled from the engine 
	/// through the GetValues method, which returns a IValueSet. In this cases it will 
	/// be assumed that the accociated time is the current time which is obtained through 
	/// the IEngine interface through the GetCurrentTime. By use of the IAdvanceEngine 
	/// interface accociated values can be pulled from the engine through the 
	/// GetValues method that will return an instance of the TimeValues class, 
	/// which contains a IValueSet and the associated ITime. The IAdvanced i
	/// nterface is implemented as a separate interface in order to facilitate 
	/// backward compatibility. 
	/// Summary description for IAdvancedEngine.
	/// </summary>
	public interface IAdvancedEngine : IEngine
	{
		/// <summary>
		/// The GetValues method will return an instance of the TimeValues class, 
		/// which is the currently calculated values as IValueSet and the associated time as ITime.
		/// </summary>
		/// <param name="quantityID">The Quantity ID for the requested values</param>
		/// <param name="ElementSetID">The ElementSet ID for the requested values</param>
		/// <returns>TimeValueSet which is the calculated values and the associated time</returns>
		new TimeValueSet GetValues(string quantityID, string ElementSetID);
	}
}