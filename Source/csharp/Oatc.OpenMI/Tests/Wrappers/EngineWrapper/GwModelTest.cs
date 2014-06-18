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
using System.Linq;
using NUnit.Framework;
using Oatc.OpenMI.Examples.GroundWaterModel;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  [TestFixture]
  public class GwModelTest
  {

    [Test]
    public void CreateInitializeTest()
    {
      ITimeSpaceComponent gwModel = new GWModelLC();
      IBaseLinkableComponent lc = gwModel;
      Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Created);

      List<IArgument> gwArguments = CreateTestArguments();

      gwModel.Arguments.ApplyArguments(gwArguments);
      gwModel.Initialize();
      Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Initialized);
      Assert.AreEqual("GWModelLC", gwModel.Id);

      Assert.AreEqual(3, gwModel.Inputs.Count);
      Assert.AreEqual(3, gwModel.Outputs.Count);

      {
        // Check the ground water level output
        ITimeSpaceOutput gwLevel = lc.Outputs[2] as ITimeSpaceOutput;
        Assert.IsNotNull(gwLevel);
        Assert.AreEqual("Grid.gwLevel", gwLevel.Id);

        // Check quantity
        IQuantity quantity = gwLevel.ValueDefinition as IQuantity;
        Assert.IsNotNull(quantity);
        Assert.AreEqual("Ground water level", quantity.Caption);
        Assert.AreEqual("gw level", quantity.Unit.Caption);
        Assert.AreEqual(1, quantity.Unit.Dimension.GetPower(DimensionBase.Length));
        Assert.AreEqual(0, quantity.Unit.Dimension.GetPower(DimensionBase.Time));
        Assert.AreEqual(1, quantity.Unit.ConversionFactorToSI);

        // Check element set
        IElementSet gridSet = gwLevel.ElementSet();
        CheckRegularGridElmtSet(gridSet);

      }

      {
        // Check the ground water inflow input
        ITimeSpaceInput gwInput = lc.Inputs[2] as ITimeSpaceInput;
        Assert.IsNotNull(gwInput);
        Assert.AreEqual("Grid.Inflow", gwInput.Id);

        // Check quantity
        IQuantity quantity = gwInput.ValueDefinition as IQuantity;
        Assert.IsNotNull(quantity);
        Assert.AreEqual("Inflow", quantity.Caption);
        Assert.AreEqual("Discharge", quantity.Unit.Caption);
        Assert.AreEqual(3, quantity.Unit.Dimension.GetPower(DimensionBase.Length));
        Assert.AreEqual(-1, quantity.Unit.Dimension.GetPower(DimensionBase.Time));
        Assert.AreEqual(0.001, quantity.Unit.ConversionFactorToSI);

        // Check element set
        IElementSet gridSet = gwInput.ElementSet();
        CheckRegularGridElmtSet(gridSet);
      }

    }

    [Test]
    public void InputOutputTests()
    {
      GWModelLC gwlc = new GWModelLC();
      IBaseLinkableComponent lc = gwlc;

      List<IArgument> arguments = CreateTestArguments();
      lc.Initialize(arguments);

      IElementSet elementSet = ((ITimeSpaceExchangeItem)lc.Inputs[2]).ElementSet();

      Quantity dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
      Quantity waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      Input waterLevelTriggerInput = new Input("Water level, to be retrieved from some output item", waterlevelQuantity, elementSet);
      waterLevelTriggerInput.TimeSet = new TimeSet();

      SimpleOutput dischargeOutput = new SimpleOutput("Discharge, to be sent to GW model", dischargeQuantity, elementSet);
      dischargeOutput.ConstOutput = 6;
      dischargeOutput.TimeSet = new TimeSet();

      ITimeSpaceInput gwInflow = UTHelper.FindInputItem(lc, "Grid.Inflow");
      ITimeSpaceOutput gwLevel = UTHelper.FindOutputItem(lc, "Grid.gwLevel");

      // Connect discharge
      gwInflow.Provider = dischargeOutput;
      dischargeOutput.AddConsumer(gwInflow);

      // Connect triggering input
      gwLevel.AddConsumer(waterLevelTriggerInput);

      lc.Validate();
      lc.Prepare();

      // specify query times
      double firstTriggerGetValuesTime = gwLevel.TimeSet.Times[0].StampAsModifiedJulianDay;
      double secondTriggerGetValuesTime = firstTriggerGetValuesTime + 3;
      double thirdTriggerGetValuesTime = firstTriggerGetValuesTime + 5;

      // check initial values
      Assert.AreEqual(8, gwLevel.Values.Values2D[0].Count);
      Assert.AreEqual(-10, (double)gwLevel.Values.GetValue(0, 0));

      ITimeSpaceValueSet values;

      // get values for initial time, therefor intial values
      waterLevelTriggerInput.TimeSet.SetSingleTime(new Time(firstTriggerGetValuesTime));
      values = gwLevel.GetValues(waterLevelTriggerInput);
      Assert.AreEqual(-10, values.GetValue(0, 0), "value for first query time");

      // get values for second query time: -10 : base gw level.
      // 10 : storageHeight is multiplied by a factor of 10
      // 3 days, 6 constant inflow, 3600*24 seconds in a day, 1000 L/M3, (100 x 200) grid cell size 
      waterLevelTriggerInput.TimeSet.SetSingleTime(new Time(secondTriggerGetValuesTime));
      values = gwLevel.GetValues(waterLevelTriggerInput);
      Assert.AreEqual(-10.0 + 10 * 3 * 6.0 * 60 * 60 * 24 / 1000 / (100 * 200), values.GetValue(0, 0), "value for first query time");

      // get values for second query time:
      // 10 : storageHeight is multiplied by a factor of 10
      // 5 days, 6 constant inflow, 3600*24 seconds in a day, 1000 L/M3, (100 x 200) grid cell size 
      waterLevelTriggerInput.TimeSet.SetSingleTime(new Time(thirdTriggerGetValuesTime));
      values = gwLevel.GetValues(waterLevelTriggerInput);
      Assert.AreEqual(-10.0 + 10 * 5 * 6.0 * 60 * 60 * 24 / 1000 / (100 * 200), values.GetValue(0, 0), "value for first query time");

      lc.Finish();

    }


    [Test]
    public void GetValuesFromGwModel()
    {
      Quantity dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
      Quantity waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      Input queryItem1 = new Input("discharge, to be retrieved from some output item", dischargeQuantity, idBasedElementSetA);
      queryItem1.TimeSet = new TimeSet();

      Input queryItem2 = new Input("water level, to be retrieved from some output item", waterlevelQuantity, idBasedElementSetA);
      queryItem2.TimeSet = new TimeSet();

      // Connect query item(s) to output item(s)
      // Take care that component becomes valid (and has produced initial output for connected items)

      ITimeSpaceComponent gwModel = new GWModelLC();
      gwModel.Initialize();

      ITimeSpaceOutput storageOnGrid = UTHelper.FindOutputItem(gwModel, "Grid.Storage");
      storageOnGrid.AddConsumer(queryItem1);
      gwModel.Validate();
      Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Valid);
      gwModel.Prepare();
      Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Updated);

      // check initial values
      Assert.AreEqual(4, ValueSet.GetElementCount(storageOnGrid.Values), "#values for " + storageOnGrid.Id);
      Assert.AreEqual(0.0, (double)storageOnGrid.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times
      queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 1, 3, 0, 0, 0));
      ITimeSpaceValueSet values = storageOnGrid.GetValues(queryItem1);
      Assert.IsNotNull(values, "values != null");
      Assert.AreEqual(0.0, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      // set next query time
      queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 4, 0, 0, 0));
      values = storageOnGrid.GetValues(queryItem1);
      Assert.IsNotNull(values, "values != null");
      Assert.AreEqual(0.0, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      // ask for same time again
      values = storageOnGrid.GetValues(queryItem1);
      Assert.IsNotNull(values, "values != null");
      Assert.AreEqual(0.0, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      try
      {
        // set query time back in time
        queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 3, 0, 0, 0));
        storageOnGrid.GetValues(queryItem1);
      }
      catch (Exception e)
      {
        Assert.IsTrue(e.Message.StartsWith("Could not update engine \""));
      }

      try
      {
        // set query time beyond time horizon
        queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 28, 0, 0, 0));
        storageOnGrid.GetValues(queryItem1);
      }
      catch (Exception e)
      {
        Assert.IsTrue(e.Message.StartsWith("Could not update engine \""));
      }
    }



    #region Helper methods and classes

    private static List<IArgument> CreateTestArguments()
    {
      List<IArgument> arguments = new List<IArgument>();
      arguments.Add(new ArgumentDouble("x0", 10));
      arguments.Add(new ArgumentDouble("y0", 10));
      arguments.Add(new ArgumentInt("xcount", 4));
      arguments.Add(new ArgumentInt("ycount", 2));
      arguments.Add(new ArgumentDouble("dx", 100));
      arguments.Add(new ArgumentDouble("dy", 200));
      return arguments;
    }

    private void CheckRegularGridElmtSet(IElementSet gridSet)
    {
      Assert.AreEqual(8, gridSet.ElementCount);

      // Check corner coordinates of first element
      Assert.AreEqual(10, gridSet.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(10, gridSet.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(110, gridSet.GetVertexXCoordinate(0, 1));
      Assert.AreEqual(10, gridSet.GetVertexYCoordinate(0, 1));
      Assert.AreEqual(110, gridSet.GetVertexXCoordinate(0, 2));
      Assert.AreEqual(210, gridSet.GetVertexYCoordinate(0, 2));
      Assert.AreEqual(10, gridSet.GetVertexXCoordinate(0, 3));
      Assert.AreEqual(210, gridSet.GetVertexYCoordinate(0, 3));
    }


    private class SimpleOutput : Output
    {
      public double ConstOutput;

      public SimpleOutput(string id)
        : base(id)
      {
      }

      public SimpleOutput(string id, IValueDefinition valueDefinition, IElementSet elementSet)
        : base(id, valueDefinition, elementSet)
      {
      }

      public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
      {
        ITimeSpaceExchangeItem item = (ITimeSpaceExchangeItem)querySpecifier;

        TimeSet.SetSingleTime(item.TimeSet.Times[0]);

        int count = item.ElementSet().ElementCount;

        double[] vals = new double[count];
        for (int i = 0; i < count; i++)
        {
          vals[i] = ConstOutput;
        }

        ValueSetArray<double> valueset = new ValueSetArray<double>(vals);

        return valueset;

      }
    #endregion
    }



  }
}
