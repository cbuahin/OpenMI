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
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel;

using NUnit.Framework;

namespace Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.UnitTest
{
    [TestFixture]
    public class GwModelLCTest
    {
        ILinkableComponent gwModelLC;

        
        [SetUp]
        public void Init()
        {
            gwModelLC = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GWModelLC();

            Oatc.OpenMI.Sdk.Backbone.Argument[] arguments = new Argument[10];
            arguments[0] = new Oatc.OpenMI.Sdk.Backbone.Argument("simulationStart" , "1990,1,2,0,0,0"       , true," year, month, day, hour, minute, second for simulation start");
            arguments[1] = new Oatc.OpenMI.Sdk.Backbone.Argument("simulationEnd"   , "1990,2,1,0,0,0"       , true," year, month, day, hour, minute, second for simulation start");
            arguments[2] = new Oatc.OpenMI.Sdk.Backbone.Argument("nx"              , "2"                    , true, "Number of grid cells in x-direction");
            arguments[3] = new Oatc.OpenMI.Sdk.Backbone.Argument("ny"              , "2"                    , true, "Number of grid cells in y-direction");
            arguments[4] = new Oatc.OpenMI.Sdk.Backbone.Argument("ox"              , "1000"                 , true, "origo X");
            arguments[5] = new Oatc.OpenMI.Sdk.Backbone.Argument("oy"              , "2000"                 , true, "origo Y");
            arguments[6] = new Oatc.OpenMI.Sdk.Backbone.Argument("cellSize"        , "900"                  , true, "cell size");
            arguments[7] = new Oatc.OpenMI.Sdk.Backbone.Argument("groundWaterLevel", "1.0"                  , true, "Ground Wagter Level");
            arguments[8] = new Oatc.OpenMI.Sdk.Backbone.Argument("modelID"         , "GWModelEngineModelID" , true, "model ID");
            arguments[9] = new Oatc.OpenMI.Sdk.Backbone.Argument("gridAngle"       , "0"                    , true, "rotation angle for the grid");

            gwModelLC.Initialize(arguments);
        }

        [TearDown]
        public void ClearUp()
        {
           gwModelLC.Finish();
           gwModelLC.Dispose();
		}

        [Test]
        public void EarliestInputTime()
        {
            gwModelLC.Prepare();
            DateTime earliestInputTime = CalendarConverter.ModifiedJulian2Gregorian(gwModelLC.EarliestInputTime.ModifiedJulianDay);
            Assert.AreEqual(new DateTime(1990,1,2,0,0,0), earliestInputTime);
        }
                
        [Test]
        public void ModelID()
        {
           Assert.AreEqual("GWModelEngineModelID",gwModelLC.ModelID);
        }

        [Test]
        public void TimeHorizon()
        {
            DateTime startDate = new DateTime(1990,1,2,0,0,0);
            DateTime endDate   = new DateTime(1990,2,1,0,0,0);

            Assert.AreEqual(startDate, CalendarConverter.ModifiedJulian2Gregorian(gwModelLC.TimeHorizon.Start.ModifiedJulianDay));
            Assert.AreEqual(endDate, CalendarConverter.ModifiedJulian2Gregorian(gwModelLC.TimeHorizon.End.ModifiedJulianDay));
        }

        [Test]
        public void OutputExchangeItemCount()
        {
            Assert.AreEqual(2,gwModelLC.OutputExchangeItemCount);
        }

        [Test]
        public void GetOutputExchangeItem()
        {
            Assert.AreEqual("BaseGrid", gwModelLC.GetOutputExchangeItem(0).ElementSet.ID);
            Assert.AreEqual(ElementType.XYPolygon, gwModelLC.GetOutputExchangeItem(0).ElementSet.ElementType);
        }



        [Test]
        public void GetValues()

        {
            Oatc.OpenMI.Examples.ModelComponents.SpatialModels.DummyLC.DumLC dummyLC = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.DummyLC.DumLC();
            Oatc.OpenMI.Sdk.Backbone.Link link = new Link();
            link.SourceComponent = gwModelLC;
            link.TargetComponent = dummyLC;
            link.ID = "linkID";
            link.TargetElementSet = new Oatc.OpenMI.Sdk.Backbone.ElementSet("dd","dd",ElementType.IDBased, new SpatialReference());
            link.TargetQuantity = new Oatc.OpenMI.Sdk.Backbone.Quantity(new Oatc.OpenMI.Sdk.Backbone.Unit("dd",1,0,"kk"),"Kk","kk",global::OpenMI.Standard.ValueType.Scalar, new Dimension());
            link.SourceElementSet = gwModelLC.GetOutputExchangeItem(1).ElementSet;
            link.SourceQuantity = gwModelLC.GetOutputExchangeItem(1).Quantity;
            gwModelLC.AddLink(link);
            gwModelLC.Prepare();
            double julianTime = Oatc.OpenMI.Sdk.DevelopmentSupport.CalendarConverter.Gregorian2ModifiedJulian(new DateTime(1990,1,3,0,0,0));
            Assert.AreEqual(1.0, ((IScalarSet) gwModelLC.GetValues(new Oatc.OpenMI.Sdk.Backbone.TimeStamp(julianTime),"linkID")).GetScalar(0));
        }
	}
}
