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
﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oatc.OpenMI.Examples.GroundWaterModel;
using Oatc.OpenMI.Examples.SimpleSCharpRiver;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest1;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  [TestFixture]
  public class LinkableEnginesTest
  {

    private Input queryItem1;
    private Input queryItem2;
    private bool flowItemsAsSpan;

    //private ILinkableComponent gwModel;

    protected IRiverModelFactory _riverModelFactory;

    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelFactory();
    }

    public int GwModelInstance = 0;

    public LinkableEngine CreateRiverModel()
    {
      return (_riverModelFactory.CreateRiverModel());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      return (_riverModelFactory.CreateRiverModelArguments(model));
    }

    public ITimeSpaceComponent CreateGwModel()
    {
      switch (GwModelInstance)
      {
        case 0:
          return (new GWModelLC());
        default:
          throw new Exception("GwModelInstance out of range");
      }
    }

    [SetUp]
    public void CreateModelsAndInputItemsForQuery()
    {
      flowItemsAsSpan = false;

      Quantity dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
      Quantity waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      queryItem1 = new Input("discharge, to be retrieved from some output item", dischargeQuantity, idBasedElementSetA);
      queryItem1.TimeSet = new TimeSet();

      queryItem2 = new Input("water level, to be retrieved from some output item", waterlevelQuantity, idBasedElementSetA);
      queryItem2.TimeSet = new TimeSet();
    }


    [Test]
    public void GetValuesFromTimeSeries()
    {
      ITimeSpaceComponent timeSeries = new TimeSeriesComponent();

      // initialize model
      timeSeries.Initialize();

      ITimeSpaceOutput output = (ITimeSpaceOutput)timeSeries.Outputs[0];

      // specify query times
      Time startTime = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
      double firstTriggerGetValuesTime = startTime.StampAsModifiedJulianDay;
      double secondTriggerGetValuesTime = firstTriggerGetValuesTime + 12.1;
      double thirdTriggerGetValuesTime = firstTriggerGetValuesTime + 16.7;

      // get values for specified query times - initial time, will return initial value
      queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = output.GetValues(queryItem1);
      Assert.AreEqual(1, values.GetValue(0, 0), "value for first query time");

      // 12.1 days from 01-01
      queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = output.GetValues(queryItem1);
      Assert.AreEqual(13, values.GetValue(0, 0), "value for second query time");

      // 16.7 days from 01-01 
      queryItem1.TimeSet.SetSingleTimeStamp(thirdTriggerGetValuesTime);
      values = output.GetValues(queryItem1);
      Assert.AreEqual(17, values.GetValue(0, 0), "value for third query time");
    }




  }

  public class LinkableEngineDelegateTest : LinkableEnginesTest
  {
    [TestFixtureSetUp]
    public new void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelDelegateFactory();
    }
  }

  public class LinkableEngineInterfaceTest : LinkableEnginesTest
  {
    [TestFixtureSetUp]
    public new void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelInterfaceFactory();
    }
  }
}