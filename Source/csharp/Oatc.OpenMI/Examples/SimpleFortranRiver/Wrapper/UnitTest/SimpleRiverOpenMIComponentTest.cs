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
// file: SimpleRiverOpenMIComponentTest.cs
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
using Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using OpenMI.Standard2;
// ADH: Events still to do
using NUnit.Framework;
using OpenMI.Standard2.TimeSpace;


namespace Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest
{
    /// <summary>
    /// Summary description for SimpleRiverOpenMIComponentTest.
    /// </summary>
    [TestFixture]
    public class SimpleRiverOpenMIComponentTest
    {
        ITimeSpaceComponent simpleRiver;

        [SetUp]
        public void Init()
        {
            simpleRiver = new SimpleRiverEngineWrapper();
            IArgument[] arguments = new IArgument[1];
            arguments[0] = Argument.Create("FilePath", @"..\..\..\..\Data", true, "description");
            simpleRiver.Initialize(arguments);
        }

        [TearDown]
        public void ClearUp()
        {
            simpleRiver.Finish();
        }

        [Test]
        public void GetTimeHorizon()
        {
            ITime horizon = simpleRiver.TimeExtent.TimeHorizon;
            Assert.AreEqual(new DateTime(2004, 10, 7, 16, 38, 32), Time.Start(horizon).AsDateTime);
            Assert.AreEqual(new DateTime(2004, 10, 19, 16, 38, 32), Time.End(horizon).AsDateTime);
        }

        [Test]
        public void Inputs()
        {
            Assert.AreEqual(8, simpleRiver.Inputs.Count);

            ITimeSpaceInput input = (ITimeSpaceInput)simpleRiver.Inputs[0];
            Assert.AreEqual("Branch:0:InFlow", input.Id);
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:0", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(0));
            Assert.AreEqual(1000.0, input.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(7000.0, input.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(6000.0, input.ElementSet().GetVertexYCoordinate(0, 1));

            input = (ITimeSpaceInput)simpleRiver.Inputs[1];
            Assert.AreEqual("Branch:1:InFlow", input.Id);
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:1", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(0));
            Assert.AreEqual(7000.0, input.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(6000.0, input.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(3000.0, input.ElementSet().GetVertexYCoordinate(0, 1));

            input = (ITimeSpaceInput)simpleRiver.Inputs[2];
            Assert.AreEqual("Branch:2:InFlow", input.Id);
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:2", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(0));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(14000.0, input.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(3000.0, input.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(1000.0, input.ElementSet().GetVertexYCoordinate(0, 1));

            input = (ITimeSpaceInput)simpleRiver.Inputs[3];
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Node:0", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.IdBased, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);

            input = (ITimeSpaceInput)simpleRiver.Inputs[4];
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Node:1", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.IdBased, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);

            input = (ITimeSpaceInput)simpleRiver.Inputs[5];
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Node:2", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.IdBased, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);

            input = (ITimeSpaceInput)simpleRiver.Inputs[6];
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Node:3", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.IdBased, input.ElementSet().ElementType);
            Assert.AreEqual(1, input.ElementSet().ElementCount);

            input = (ITimeSpaceInput)simpleRiver.Inputs[7];
            Assert.AreEqual("InFlow", input.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)input.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), input.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("AllBranches", input.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, input.ElementSet().ElementType);
            Assert.AreEqual(3, input.ElementSet().ElementCount);
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(0));
            Assert.AreEqual(1000.0, input.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(7000.0, input.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(6000.0, input.ElementSet().GetVertexYCoordinate(0, 1));
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(1));
            Assert.AreEqual(7000.0, input.ElementSet().GetVertexXCoordinate(1, 0));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexXCoordinate(1, 1));
            Assert.AreEqual(6000.0, input.ElementSet().GetVertexYCoordinate(1, 0));
            Assert.AreEqual(3000.0, input.ElementSet().GetVertexYCoordinate(1, 1));
            Assert.AreEqual(2, input.ElementSet().GetVertexCount(2));
            Assert.AreEqual(9000.0, input.ElementSet().GetVertexXCoordinate(2, 0));
            Assert.AreEqual(14000.0, input.ElementSet().GetVertexXCoordinate(2, 1));
            Assert.AreEqual(3000.0, input.ElementSet().GetVertexYCoordinate(2, 0));
            Assert.AreEqual(1000.0, input.ElementSet().GetVertexYCoordinate(2, 1));

        }


        [Test]
        public void Outputs()
        {
            Assert.AreEqual(3, simpleRiver.Outputs.Count);

            ITimeSpaceOutput output = (ITimeSpaceOutput)simpleRiver.Outputs[0];
            Assert.AreEqual("Branch:0:Flow", output.Id);
            Assert.AreEqual("Flow", output.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)output.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), output.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:0", output.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, output.ElementSet().ElementType);
            Assert.AreEqual(1, output.ElementSet().ElementCount);
            Assert.AreEqual(2, output.ElementSet().GetVertexCount(0));
            Assert.AreEqual(1000.0, output.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(7000.0, output.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(9000.0, output.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(6000.0, output.ElementSet().GetVertexYCoordinate(0, 1));

            output = (ITimeSpaceOutput)simpleRiver.Outputs[1];
            Assert.AreEqual("Branch:1:Flow", output.Id);
            Assert.AreEqual("Flow", output.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)output.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), output.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:1", output.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, output.ElementSet().ElementType);
            Assert.AreEqual(1, output.ElementSet().ElementCount);
            Assert.AreEqual(2, output.ElementSet().GetVertexCount(0));
            Assert.AreEqual(7000.0, output.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(9000.0, output.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(6000.0, output.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(3000.0, output.ElementSet().GetVertexYCoordinate(0, 1));

            output = (ITimeSpaceOutput)simpleRiver.Outputs[2];
            Assert.AreEqual("Branch:2:Flow", output.Id);
            Assert.AreEqual("Flow", output.ValueDefinition.Caption);
            Assert.AreEqual("m3/sec", ((IQuantity)output.ValueDefinition).Unit.Caption);
            Assert.AreEqual(typeof(double).ToString(), output.ValueDefinition.ValueType.ToString());
            Assert.AreEqual("Branch:2", output.ElementSet().Caption);
            Assert.AreEqual(ElementType.PolyLine, output.ElementSet().ElementType);
            Assert.AreEqual(1, output.ElementSet().ElementCount);
            Assert.AreEqual(2, output.ElementSet().GetVertexCount(0));
            Assert.AreEqual(9000.0, output.ElementSet().GetVertexXCoordinate(0, 0));
            Assert.AreEqual(14000.0, output.ElementSet().GetVertexXCoordinate(0, 1));
            Assert.AreEqual(3000.0, output.ElementSet().GetVertexYCoordinate(0, 0));
            Assert.AreEqual(1000.0, output.ElementSet().GetVertexYCoordinate(0, 1));
        }

        [Test]
        public void GetValues()
        {

            IQuantity riverModelOutflowQuantity = simpleRiver.Outputs[0].ValueDefinition as IQuantity;
            IElementSet riverModelNodeElement = ((ITimeSpaceOutput)simpleRiver.Outputs[0]).ElementSet();

            Input queryItem1;
            var dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
            var waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

            var idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
            idBasedElementSetA.AddElement(new Element("elm-1"));

            queryItem1 = new Input("discharge, to be retrieved from some output item", dischargeQuantity, idBasedElementSetA);
            queryItem1.TimeSet = new TimeSet();

            // Connect the queryItem with the first output item using a time interpolator
            TimeBufferer buffer = new TimeInterpolator((ITimeSpaceOutput)simpleRiver.Outputs[0]);
            simpleRiver.Outputs[0].AddAdaptedOutput(buffer);
			buffer.AddConsumer(queryItem1);

            simpleRiver.Validate();

            simpleRiver.Prepare();

            //IListener eventListener = new EventListener();
            //for (int i = 0; i < eventListener.GetAcceptedEventTypeCount(); i++)
            //{
            //    for (int n = 0; n < simpleRiver.GetPublishedEventTypeCount(); n++)
            //    {
            //        if (eventListener.GetAcceptedEventType(i) == simpleRiver.GetPublishedEventType(n))
            //        {
            //            simpleRiver.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
            //        }
            //    }
            //}

            double startTime = simpleRiver.TimeExtent.TimeHorizon.StampAsModifiedJulianDay;

            ITime[] times = new ITime[5];
            times[0] = new Time(startTime + 1);
            times[1] = new Time(startTime + 2);
            times[2] = new Time(startTime + 3);
            times[3] = new Time(startTime + 4);
            times[4] = new Time(startTime + 5);

            ITimeSpaceValueSet values;

            queryItem1.TimeSet.SetSingleTimeStamp(times[0].StampAsModifiedJulianDay);
            values = ((ITimeSpaceOutput)queryItem1.Provider).GetValues(queryItem1);
            Assert.AreEqual(0.55, values.GetValue(0, 0));

            queryItem1.TimeSet.SetSingleTimeStamp(times[1].StampAsModifiedJulianDay);
            values = ((ITimeSpaceOutput)queryItem1.Provider).GetValues(queryItem1);
            Assert.AreEqual(1.1, values.GetValue(0, 0));

            queryItem1.TimeSet.SetSingleTimeStamp(times[2].StampAsModifiedJulianDay);
            values = ((ITimeSpaceOutput)queryItem1.Provider).GetValues(queryItem1);
            Assert.AreEqual(1.6, values.GetValue(0, 0));

            queryItem1.TimeSet.SetSingleTimeStamp(times[3].StampAsModifiedJulianDay);
            values = ((ITimeSpaceOutput)queryItem1.Provider).GetValues(queryItem1);
            Assert.AreEqual(2.1, values.GetValue(0, 0));

            queryItem1.TimeSet.SetSingleTimeStamp(times[4].StampAsModifiedJulianDay);
            values = ((ITimeSpaceOutput)queryItem1.Provider).GetValues(queryItem1);
            Assert.AreEqual(2.6, values.GetValue(0, 0));

        }

        [Test]
        public void ModelID()
        {
            Assert.AreEqual("The river Rhine", simpleRiver.Id);
        }

        [Test]
        public void ModelDescription()
        {
            Assert.AreEqual("Simple River model for: The river Rhine", simpleRiver.Description);
        }


        [Test]
        public void TimeFrame()
        {
            Console.WriteLine("TimeHorizonStart (MJulianDay) = " + simpleRiver.TimeExtent.TimeHorizon.StampAsModifiedJulianDay);
            Console.WriteLine("TimeHorizonStart (DateTime) = " + simpleRiver.TimeExtent.TimeHorizon.ToDateTime());
            Console.WriteLine("TimeHorizonEnd (MJulianDay) = " + Time.End(simpleRiver.TimeExtent.TimeHorizon).StampAsModifiedJulianDay);
            Console.WriteLine("TimeHorizonEnd (DateTime) = " + Time.End(simpleRiver.TimeExtent.TimeHorizon).AsDateTime);
        }

    }
}

