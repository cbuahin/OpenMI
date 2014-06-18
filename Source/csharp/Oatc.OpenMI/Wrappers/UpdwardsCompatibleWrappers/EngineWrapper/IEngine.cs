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
// file: IEngine.cs
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
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

using System.Collections;
using Oatc.UpwardsComp.Backbone;
using Oatc.UpwardsComp.Standard;
using ITime=Oatc.UpwardsComp.Standard.ITime;

namespace Oatc.UpwardsComp.EngineWrapper
{
	/// <summary>
	/// The class Wrapper.LinkableEngine will access the model engine through
	/// this interface.
	/// </summary>
	public interface IEngine
	{
		/// <summary>
		/// Returns the ModelID. The ModelID identifies the populated model component. 
		/// Example: "River Rhine"
		/// </summary>
		/// <returns>ModelID</returns>
		string GetModelID();

		/// <summary>
		/// Return the Model Description. The Model Description is a description of the populated
		/// model component.
		/// </summary>
		/// <returns>Model description</returns>
		string GetModelDescription();

		/// <summary>
		/// Return the time horison for the populated model compoent. The Time Horizon for a model i typically
		/// the same as the simulation period, which normally depend on de available input data. When you model 
		/// is running in the OpenMI environment, the model component must be able to return values within the 
		/// TimeHorizon
		/// </summary>
		/// <returns>TimeHorizon</returns>
		ITimeSpan GetTimeHorizon();

		/// <summary>
		/// Returns the number of input exchange items for the populated model component.
		/// </summary>
		/// <returns>InputExchangeItemCount</returns>
		int GetInputExchangeItemCount();

		/// <summary>
		/// Returns the number of output exchange items for the populated model component.
		/// </summary>
		/// <returns>OutputExchangeItemCount</returns>
		int GetOutputExchangeItemCount();

		/// <summary>
		/// Returns a specific output exchange item from the populated model component.
		/// </summary>
		/// <param name="exchangeItemIndex">index number</param>
		/// <returns>OutputExchangeItem according the the index number</returns>
		OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex);

		/// <summary>
		/// Returns a specific input exchange item from the populated model component.
		/// </summary>
		/// <param name="exchangeItemIndex">index number</param>
		/// <returns>InputExchangeItem according the the index number</returns>
		InputExchangeItem GetInputExchangeItem(int exchangeItemIndex);

		/// <summary>
		/// Initialize will typically be invoked just after creation of the object
		/// that implements the IRunEngine interface.
		/// </summary>
		/// <param name="properties">
		/// Hashtable with the same contents as the Component arguments
		/// in the ILinkableComponent interface. Typically any information
		/// needed for initialization of the model will be included in this table.
		/// This could be path and file names for input files.
		/// </param>
		void Initialize(Hashtable properties);

		/// <summary>
		/// This method will be invoked after all computations are completed. Deallocation of memory
		/// and closing files could be implemented in this method
		/// </summary>
		void Finish();

		/// <summary>
		/// This method will be invoked after all computations are completed
		/// and after the Finish method has been invoked
		/// </summary>
		void Dispose();

		/// <summary>
		/// This method will make the model engine perform one time step.
		/// </summary>
		/// <returns> Returns true if the time step was completed,
		/// otherwise it will return false
		/// </returns>
		bool PerformTimeStep();

		/// <summary>
		/// Get the current time of the model engine
		/// </summary>
		/// <returns>The current time for the model engine</returns>
		ITime GetCurrentTime();

		/// <summary>
		/// Get the time for which the next input is needed for
		/// a specific Quantity and ElementSet combination
		/// </summary>
		/// <param name="QuantityID">ID for the quantity</param>
		/// <param name="ElementSetID">ID for the ElementSet</param> 
		/// <returns>ITimeSpan or ITimeStamp	</returns>
		ITime GetInputTime(string QuantityID, string ElementSetID);

		/// <summary>
		/// Get earlist needed time, which can be used 
		/// to clear the buffer. For most time stepping model engines this
		/// time will be the time for the previous time step.
		/// </summary>
		/// <returns>TimeStamp</returns>
		ITimeStamp GetEarliestNeededTime();

		/// <summary>
		/// Sets values in the model engine
		/// </summary>
		/// <param name="QuantityID">quantityID associated to the values</param>
		/// <param name="ElementSetID">elementSetID associated to the values</param> 
		/// <param name="values">The values</param> 
		void SetValues(string QuantityID, string ElementSetID, IValueSet values);

		/// <summary>
		/// Gets values from the model engine
		/// </summary>
		/// <param name="QuantityID">quantityID associated to the requested values</param>
		/// <param name="ElementSetID">elementSetID associated to the requested values</param>  
		/// <returns>The requested values</returns>
		IValueSet GetValues(string QuantityID, string ElementSetID);

		/// <summary>
		/// In some situations a valied values cannot be return when the 
		/// Wrapper.IRunEngine.GetValues is invoked. In such case a missing values
		/// can be returned. The GetMissingValeusDefinition method can be used to query which definition
		/// of a missing value that applies to this particular model component. Example of missing value
		/// definition could be: -999.99
		/// </summary>
		/// <returns>Missing value definition</returns>
		double GetMissingValueDefinition();

		/// <summary>
		/// Get the ComponentID. The component ID is the name of the non-populated component. This is typically 
		/// the product name of your model engine.
		/// </summary>
		/// <returns>Component ID</returns>
		string GetComponentID();

		/// <summary>
		/// Get a description of your component. This description refers to the non-populated component. This is 
		/// typically a description of what your component does and which methods that are used. E.g. "Finite element
		/// based ground water model".
		/// </summary>
		/// <returns>Component description</returns>
		string GetComponentDescription();
	}
}