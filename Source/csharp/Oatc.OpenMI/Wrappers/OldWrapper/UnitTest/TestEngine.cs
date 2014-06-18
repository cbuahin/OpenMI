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
// namespace: org.OpenMI.Utilities.Wrapper.UnitTest 
// purpose: UnitTest for the org.OpenMI.Utilities.Wrapper package
// file: TestEngine.cs
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
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

using System.Collections;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for TestEngine.
	/// </summary>
	public class TestEngine : IEngine
	{
	    private readonly ILinkableComponent myLinkableComponent;
	    private double currentTime;
		private double dt; //time step length [days]
		private double dx; //values are incremented by dx in each time step
		private double initialValue;
		private InputItem inputExchangeItem;
		private string modelId; //used for debugging in ordet see the difference between two instances of TestEngineLC
		private OutputItem outputExchangeItem;
		private double startTime;
		private double[] values;

        public TestEngine(ILinkableComponent myLinkableComponent)
		{
            this.myLinkableComponent = myLinkableComponent;
		    modelId = "TestEngineComponentID";
		}

	    public IInputItem GetInputExchangeItem(int exchangeItemIndex)
		{
			return inputExchangeItem;
		}

		public ITime GetTimeHorizon()
		{
			return new Time(startTime, 100.0);
		}

		public string GetModelID()
		{
			return modelId;
		}

		public int GetInputExchangeItemCount()
		{
			return 1;
		}

		public IOutputItem GetOutputExchangeItem(int exchangeItemIndex)
		{
			return outputExchangeItem;
		}

		public string GetModelDescription()
		{
			return "TestModelDescription";
		}

		public int GetOutputExchangeItemCount()
		{
			return 1;
		}

		public void SetValues(string quantityId, string elementSetId, double[] values)
		{
			for (int i = 0; i < this.values.Length; i++)
			{
				this.values[i] = values[0];
			}
		}

		public string GetComponentID()
		{
			return "testEngineCompoentID";
		}

		public void Finish()
		{
		}

		public ITime GetCurrentTime()
		{
			return new Time(currentTime);
		}

		public double[] GetValues(string quantityId, string elementSetId)
		{
			return values;
		}

		public void Dispose()
		{
		}

		public string GetComponentDescription()
		{
			return "TestEngineComponentDescription";
		}

	    public void Initialize(Hashtable properties)
		{
			dt = 1.0;
			dx = 0.0;
			initialValue = 100;

			if (properties.ContainsKey("modelID"))
			{
				modelId = (string) properties["ModelID"];
			}

			if (properties.ContainsKey("dt"))
			{
				dt = (double) properties["dt"];
			}

			if (properties.ContainsKey("dx"))
			{
				dx = (double) properties["dx"];
			}

			values = new double[3];

			for (int i = 0; i < values.Length; i++)
			{
				values[i] = initialValue;
			}

			startTime = 4000;
			currentTime = startTime;

			Element element = new Element("ElementID");
			ElementSet elementSet = new ElementSet("Description", "Caption", ElementType.IdBased);
			elementSet.AddElement(element);
			Quantity quantity = new Quantity(new Unit("Flow", 1, 0, "flow"), "Flow", "Caption");

            outputExchangeItem = new OutputItem("Somewhere.Flow", quantity, elementSet);
	        outputExchangeItem.Component = myLinkableComponent;

            inputExchangeItem = new InputItem("Somewhere.Flow", quantity, elementSet);
            outputExchangeItem.Component = myLinkableComponent;
        }

		public bool PerformTimeStep()
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] += dx;
			}

			currentTime += dt;


			return true;
		}

		public double GetMissingValueDefinition()
		{
			return -999.0;
		}
	}
}