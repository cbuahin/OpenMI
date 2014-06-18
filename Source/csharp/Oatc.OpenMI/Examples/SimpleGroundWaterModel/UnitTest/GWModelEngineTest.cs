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
//    Copyright (C) 2005 HarmonIT Consortium
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
//  Created on:      December 1st 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel;

using NUnit.Framework;



namespace Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.UnitTest
{
	/// <summary>
	/// Summary description for GWModelEngineTest.
	/// </summary>
	[TestFixture]
    public class GWModelEngineTest
	{
        Oatc.OpenMI.Sdk.Wrapper.IEngine _gwModelEngine;
        Hashtable _arguments;
        DateTime  _simulationStart;
        DateTime  _simulationEnd;

		public GWModelEngineTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        [SetUp]
        public void Init()
        {
            _arguments = new Hashtable();

            _arguments.Add("simulationStart" , "1990,1,2,0,0,0");
            _arguments.Add("simulationEnd"   , "1990,2,1,0,0,0");
            _arguments.Add("nx"              , "2" );
            _arguments.Add("ny"              , "2");
            _arguments.Add("ox"              , "1000" );
            _arguments.Add("oy"              , "1000");
            _arguments.Add("cellSize"        , "1000");
            _arguments.Add("groundWaterLevel", "1.0");
            _arguments.Add("modelID"         , "GWModelEngineModelID");
            _arguments.Add("gridAngle"       , "0");

            _gwModelEngine = new GWModelEngine();
            _gwModelEngine.Initialize(_arguments);

            _simulationStart = new DateTime(1990,1,2,0,0,0);
            _simulationEnd   = new DateTime(1990,2,1,0,0,0);
        }

        [TearDown]
        public void ClearUp()
        {
            _gwModelEngine.Finish();
            _gwModelEngine.Finish();
        }


        [Test][Ignore("Test is not implemented")]
        public void GetInputExchangeItem()
        {
            
        }

        [Test]
        public void GetTimeHorizon()
        {
            Assert.AreEqual(_simulationStart, CalendarConverter.ModifiedJulian2Gregorian(_gwModelEngine.GetTimeHorizon().Start.ModifiedJulianDay));
            Assert.AreEqual(_simulationEnd, CalendarConverter.ModifiedJulian2Gregorian(_gwModelEngine.GetTimeHorizon().End.ModifiedJulianDay));
            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetModelID()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetInputExchangeItemCount()
        {
        
        }

        [Test][Ignore("Test is not implemented")]
        public void GetOutputExchangeItem()
        {
        
        }

        [Test][Ignore("Test is not implemented")]
        public void GetModelDescription()
        {
         
        }

        [Test][Ignore("Test is not implemented")]
        public void GetOutputExchangeItemCount()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void SetValues()
        {
            
        }

        [Test]
        public void GetComponentID()
        {
            Assert.AreEqual("GWModelEngineComponentID",_gwModelEngine.GetComponentID());
            
        }

        [Test][Ignore("Test is not implemented")]
        public void Finish()
        {
            
        }

        [Test]
        public void GetCurrentTime()
        {
            Assert.AreEqual(_simulationStart, CalendarConverter.ModifiedJulian2Gregorian(((ITimeStamp)_gwModelEngine.GetCurrentTime()).ModifiedJulianDay));
            _gwModelEngine.PerformTimeStep();
            Assert.AreEqual(_simulationStart + new System.TimeSpan(1,0,0,0,0), CalendarConverter.ModifiedJulian2Gregorian(((ITimeStamp)_gwModelEngine.GetCurrentTime()).ModifiedJulianDay));

            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetValues()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void Dispose()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetComponentDescription()
        {
        
        }

        [Test]
        public void GetEarliestNeededTime()
        {
            Assert.AreEqual(_simulationStart, CalendarConverter.ModifiedJulian2Gregorian(_gwModelEngine.GetEarliestNeededTime().ModifiedJulianDay));
            _gwModelEngine.PerformTimeStep();
            Assert.AreEqual(_simulationStart + new System.TimeSpan(1,0,0,0,0), CalendarConverter.ModifiedJulian2Gregorian(_gwModelEngine.GetEarliestNeededTime().ModifiedJulianDay));

        
        }

        [Test][Ignore("Test is not implemented")]
        public void Initialize()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void PerformTimeStep()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetMissingValueDefinition()
        {
            
        }

        [Test][Ignore("Test is not implemented")]
        public void GetInputTime()
        {
          
        }
	}
}
