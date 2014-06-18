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
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest 
// purpose: Unit-testing the package Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper
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
using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest
{
	/// <summary>
	/// Summary description for TestEngine.
	/// </summary>
	public class TestEngine : Oatc.OpenMI.Wrappers.EngineWrapper.LinkableGetSetEngine
	{
		public ArrayList _dataItems;

		public ITime _timeHorizon;
		public double _timeStepLength; //seconds
		
		public string _componentDescription;
		public string _modelID;
		public string _modelDescription;

		private double _currentTime;

		public TestEngine()
		{
		    _dataItems = new ArrayList();
		}
		#region IEngine Members


		public ITime GetTimeHorizon()
		{
			return _timeHorizon;
		}



		#endregion

		#region IRunEngine Members

        public override void SetEngineValues(Oatc.OpenMI.Wrappers.EngineWrapper.EngineInputItem inputItem, ITimeSpaceValueSet values)
        {
            var doubleList = values.Values2D[0] as IList<double>;
            if (doubleList == null)
                throw new ArgumentException("ValueSet set must contain doubles", "values");
            double[] doubleArray = new double[doubleList.Count];
            doubleList.CopyTo(doubleArray, 0);
            SetValues(inputItem.ValueDefinition.Caption, inputItem.SpatialDefinition.Caption, doubleArray);
        }

		public void SetValues(string QuantityCaption, string ElementSetCaption, double[] values)
		{
			bool dataItemWasFound = false;
			foreach(DataItem dataItem in _dataItems)
			{
                if (QuantityCaption == dataItem._quantityCaption 
                    && ElementSetCaption == dataItem._elementSetCaption)
				{
					dataItem._values[0] = values[0];
					dataItemWasFound = true;
				}
			}
			if (!dataItemWasFound)
			{
				throw new Exception("Could not find data item");
			}
		}


	    protected override ITime StartTime
	    {
	        get { throw new NotImplementedException(); }
	    }

	    protected override ITime EndTime
	    {
	        get { throw new NotImplementedException(); }
	    }

	    public override ITime CurrentTime
        {
            get { return new Time(_currentTime); }
        }

  	  public override ITime GetCurrentTime(bool asStamp)
        {
            if (asStamp)
                throw new NotSupportedException();
            return (CurrentTime);
        }


        public override ITimeSpaceValueSet GetEngineValues(ExchangeItem exchangeItem)
        {
            double[] values = GetValues(exchangeItem.ValueDefinition.Caption, exchangeItem.SpatialDefinition.Caption);
            return(new TimeSpaceValueSet<double>(values));
       }

		public double[] GetValues(string QuantityCaption, string ElementSetCaption)
		{
			foreach(DataItem dataItem in _dataItems)
                if (QuantityCaption == dataItem._quantityCaption 
                    && ElementSetCaption == dataItem._elementSetCaption)
                    return dataItem._values;

			throw new Exception("Could not find data item");
		}


	    protected override void OnPrepare()
	    {
	    }

	    public override void Finish()
        {
        }

        public string GetComponentDescription()
		{
			return _componentDescription;
		}

		public ITime GetEarliestNeededTime()
		{
			return new Time(_currentTime);
		}


        public override void Initialize()
        {
            _currentTime = _timeHorizon.StampAsModifiedJulianDay;
        }

        protected override void PerformTimestep(ICollection<Wrappers.EngineWrapper.EngineOutputItem> requiredOutputItems)
        {
            _currentTime += _timeStepLength;
        }

	    protected override string[] OnValidate()
	    {
	        return(new string[0]);
	    }

	    public override bool DefaultForStoringValuesInExchangeItem
	    {
	        get { return(true); }
	    }

	    public double GetMissingValueDefinition()
		{
			
			return -999;
		}


        public override ITime GetInputTime(bool asStamp)
        {
            if (asStamp)
                throw new NotSupportedException();
            return new Time(_currentTime);
        }

		#endregion
    }
}
