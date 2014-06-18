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
using System.Text;
using NUnit.Framework;
using Oatc.OpenMI.Examples.SimpleSCharpRiver;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest1;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  /// <summary>
  /// Testing the C# simple river model, <see cref="RiverModelLC"/>.
  /// <para>
  /// The GetValues tests are organised as follows:
  /// <list type="bullet">
  /// <item>GetValues: Tests the raw GetValues method</item>
  /// <item>GetValues1X: couples one river model with some boundary components</item>
  /// <item>GetValues2X: couples two river models, and some boundary components</item>
  /// </list>
  /// </para>
  /// </summary>
  [TestFixture]
  public class RiverModelTest
  {

    protected IRiverModelFactory _riverModelFactory;

    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelFactory();
    }


    private Input _queryItem1;
    private Input _queryItem2;
    private bool _flowItemsAsSpan;

    public LinkableEngine CreateRiverModel()
    {
      return (_riverModelFactory.CreateRiverModel());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      return (_riverModelFactory.CreateRiverModelArguments(model));
    }

    [SetUp]
    public void CreateModelsAndInputItemsForQuery()
    {
      _flowItemsAsSpan = false;

      Quantity dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
      Quantity waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      _queryItem1 = new Input("discharge, to be retrieved from some output item", dischargeQuantity, idBasedElementSetA);
      _queryItem1.TimeSet = new TimeSet();

      _queryItem2 = new Input("water level, to be retrieved from some output item", waterlevelQuantity, idBasedElementSetA);
      _queryItem2.TimeSet = new TimeSet();
    }

    [Test]
    public void CreateInitializeTest()
    {
      ITimeSpaceComponent riverModel = CreateRiverModel();
      Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Created);

      List<IArgument> arguments = CreateRiverModelArguments(riverModel);
      arguments.Add(new ArgumentBool("flowItemsAsSpan", _flowItemsAsSpan));

      riverModel.Arguments.ApplyArguments(arguments);
      riverModel.Initialize();
      Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Initialized);
      Assert.AreEqual("RiverModel Default Model ID", riverModel.Id);
    }

    /// <summary>
    /// Testing that the GetValues call fromt the river model returns correct results
    /// </summary>
    [Test]
    public void GetValues()
    {
      // Connect query item(s) to output item(s)
      // Take care that component becomes valid (and has produced initial output for connected items)

      ITimeSpaceComponent riverModel = CreateRiverModel();
      List<IArgument> arguments = CreateRiverModelArguments(riverModel);
      arguments.Add(new ArgumentBool("flowItemsAsSpan", _flowItemsAsSpan));
      riverModel.Arguments.ApplyArguments(arguments);
      riverModel.Initialize();

      ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(riverModel, "Branch:2:Flow");
      flowOnBranch.AddConsumer(_queryItem1);
      riverModel.Validate();
      Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Valid);
      riverModel.Prepare();
      Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Updated);

      // check initial values
      Assert.AreEqual(1, ValueSet.GetElementCount(flowOnBranch.Values), "#values for " + flowOnBranch.Id);
      Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times
      _queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 1, 3, 0, 0, 0));
      ITimeSpaceValueSet values = flowOnBranch.GetValues(_queryItem1);
      Assert.IsNotNull(values, "values != null");
      double flow3 = 35.0 / 4.0; // = 10 * (1.0 / 2.0 + 1.0 / 4.0 + 1.0 / 8.0) = 8.75
      Assert.AreEqual(flow3, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      // set next query time
      _queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 4, 0, 0, 0));
      values = flowOnBranch.GetValues(_queryItem1);
      Assert.IsNotNull(values, "values != null");
      flow3 = 10 * (1.0 / 2.0 + 1.0 / 4.0 + 1.0 / 8.0); // = 8.75
      Assert.AreEqual(flow3, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      // ask for same time again
      values = flowOnBranch.GetValues(_queryItem1);
      Assert.IsNotNull(values, "values != null");
      Assert.AreEqual(flow3, (double)values.GetValue(0, 0), "value[0] from GetValues 1");

      try
      {
        // set query time back in time
        _queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 3, 0, 0, 0));
        flowOnBranch.GetValues(_queryItem1);
      }
      catch (Exception e)
      {
        Assert.IsTrue(e.Message.StartsWith("Could not update engine \""));
      }

      try
      {
        // set query time beyond time horizon
        _queryItem1.TimeSet.SetSingleTimeStamp(new DateTime(2005, 2, 28, 0, 0, 0));
        flowOnBranch.GetValues(_queryItem1);
      }
      catch (Exception e)
      {
        Assert.IsTrue(e.Message.StartsWith("Could not update engine \""));
      }
    }

    /// <summary>
    /// One river coupled with a trigger (ID-based), with a time buffer in between.
    /// </summary>
    [Test]
    public void GetValues1A()
    {
      LinkableEngine riverModelLE = CreateRiverModel();
      ITimeSpaceComponent riverModelLC = riverModelLE;

      // initialize model
      List<IArgument> riverArguments = CreateRiverModelArguments(riverModelLC);
      riverArguments.Add(Argument.Create("ModelID", "RiverModel", true, "argument"));
      riverArguments.Add(Argument.Create("TimeStepLength", 3600));
      riverModelLC.Arguments.ApplyArguments(riverArguments);
      riverModelLC.Initialize();

      // Link output and trigger with a time buffer
      ITimeSpaceOutput output = (ITimeSpaceOutput)riverModelLC.Outputs[2];
      IAdaptedOutputFactory adaptedOutputFactory = riverModelLC.AdaptedOutputFactories[0];
      IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(output, _queryItem1);
      ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], output, _queryItem1);
      adaptedOutput.AddConsumer(_queryItem1);

      riverModelLC.Validate();
      Assert.IsTrue(riverModelLC.Status == LinkableComponentStatus.Valid);
      riverModelLC.Prepare();
      Assert.IsTrue(riverModelLC.Status == LinkableComponentStatus.Updated);

      // specify query times
      double firstTriggerGetValuesTime = riverModelLE.CurrentTime.StampAsModifiedJulianDay;
      double secondTriggerGetValuesTime = firstTriggerGetValuesTime + 2;
      double thirdTriggerGetValuesTime = firstTriggerGetValuesTime + 4.3;

      // check initial values
      Assert.AreEqual(1, output.Values.Values2D[0].Count, "#values for " + output.Id);
      Assert.AreEqual(7.0, (double)output.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times
      _queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(7.0, values.GetValue(0, 0), "value for first query time");

      // only runoff inflow, 10 L/s
      _queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(35.0 / 4.0, values.GetValue(0, 0), "value for second query time");

      // still only runoff inflow, 10 L/s            
      _queryItem1.TimeSet.SetSingleTimeStamp(thirdTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(35.0 / 4.0, values.GetValue(0, 0), "value for third query time");

      riverModelLC.Finish();
    }

    /// <summary>
    /// Coupling one river with a <see cref="TimeSeriesComponent"/> (inflow to first node), 
    /// and a trigger component.
    /// <para>
    /// Compared to <see cref="GetValues1A"/>, the <see cref="TimeSeriesComponent"/>
    /// is added as variable inflow to the first node, hence the flow through
    /// the river should change with time.
    /// </para>
    /// </summary>
    [Test]
    public void GetValues1B()
    {
      ITimeSpaceComponent timeSeries = new TimeSeriesComponent();
      LinkableEngine riverModelLE = CreateRiverModel();
      ITimeSpaceComponent riverModelLC = riverModelLE;

      // initialize model
      timeSeries.Initialize();

      List<IArgument> riverArguments = CreateRiverModelArguments(riverModelLC);
      riverArguments.Add(Argument.Create("ModelID", "upperRiverModel", true, "argument"));
      riverArguments.Add(new ArgumentBool("flowItemsAsSpan", true));
      riverArguments.Add(Argument.Create("TimeStepLength", 3600));

      riverModelLC.Arguments.ApplyArguments(riverArguments);
      riverModelLC.Initialize();

      // Connect time series component and river
      IBaseOutput tsOutput = timeSeries.Outputs[0];
      IBaseInput riverInput = riverModelLC.Inputs[0];
      tsOutput.AddConsumer(riverInput);

      // Connect tigger to river
      ITimeSpaceOutput output = (ITimeSpaceOutput)riverModelLC.Outputs[2];
      IAdaptedOutputFactory adaptedOutputFactory = riverModelLC.AdaptedOutputFactories[0];
      IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(output, _queryItem1);
      ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], output, _queryItem1);
      adaptedOutput.AddConsumer(_queryItem1);

      // Validate and prepare
      riverModelLC.Validate();
      Assert.IsTrue(riverModelLC.Status == LinkableComponentStatus.Valid);
      riverModelLC.Prepare();
      Assert.IsTrue(riverModelLC.Status == LinkableComponentStatus.Updated);

      // specify query times
      double firstTriggerGetValuesTime = riverModelLE.CurrentTime.StampAsModifiedJulianDay;
      double secondTriggerGetValuesTime = firstTriggerGetValuesTime + 12.1;
      double thirdTriggerGetValuesTime = firstTriggerGetValuesTime + 16.7;

      // check initial values
      Assert.AreEqual(1, output.Values.Values2D[0].Count, "#values for " + output.Id);
      Assert.AreEqual(7.0, (double)output.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times - initial time, will return initial value
      _queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(7.0, values.GetValue(0, 0), "value for first query time");

      // 12.1 days from 01-01, 10 L/s runoff + 13 L/s inflow on first node
      _queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(35.0 / 4.0 + 13.0 / 8.0, values.GetValue(0, 0), "value for second query time");

      // 16.7 days from 01-01, 10 L/s runoff + 17 L/s inflow on first node
      _queryItem1.TimeSet.SetSingleTimeStamp(thirdTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(35.0 / 4.0 + 17.0 / 8.0, values.GetValue(0, 0), "value for third query time");

      riverModelLC.Finish();
    }

    /// <summary>
    /// Running with two instances of the <see cref="RiverModelLC"/>.
    /// <list type="bullet">
    /// <item>The two rivers are running with the same timestep</item>
    /// <item>The link is ID based, with flow from upper river last node to lower river first node</item>
    /// <item>The time is span based</item>
    /// </list>
    /// </summary>
    [Test]
    public void GetValues2A()
    {
      //TODO: 1: The RiverModelEngine should change the inflow over time. As it is now the inflow is the same
      //         in all time steps. Another idea would be to have a output exchange item that hold the accumulated
      //         inflow, this could be useful when testing the manage state interface.
      //
      //       2: Make this test run with the two river using different timesteps and with the source river
      //          starting ealier that the target river.
      //
      //       3: In this test also events could be tested. Simply test if all the required events are
      //          thrown during the simulations.

      ITimeSpaceComponent upperRiver = CreateRiverModel();
      ITimeSpaceComponent lowerRiver = CreateRiverModel();

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> upperRiverArguments = CreateRiverModelArguments(upperRiver);
      upperRiverArguments.Add(Argument.Create("ModelID", "upperRiverModel", true, "argument"));
      upperRiverArguments.Add(new ArgumentBool("flowItemsAsSpan", false));
      upperRiver.Arguments.ApplyArguments(upperRiverArguments);
      upperRiver.Initialize();

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> lowerRiverArguments = CreateRiverModelArguments(lowerRiver);
      lowerRiverArguments.Add(Argument.Create("ModelID", "lowerRiverModel", true, "argument"));
      lowerRiverArguments.Add(new ArgumentBool("flowItemsAsSpan", false));
      lowerRiver.Initialize(lowerRiverArguments);

      Assert.AreEqual("upperRiverModel", upperRiver.Id);
      Assert.AreEqual("lowerRiverModel", lowerRiver.Id);

      // Link upper river outflow to lower river inflow
      IBaseOutput upperRiverOutput = upperRiver.Outputs[2];
      IBaseInput lowerRiverInput = lowerRiver.Inputs[0];
      upperRiverOutput.AddConsumer(lowerRiverInput);

      // Link some lower river output item to the 'triggering' query item
      // Put a time interpolator in between, to take care that any required time stamp can be provided
      ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(lowerRiver, "Branch:2:Flow");
      IAdaptedOutputFactory adaptedOutputFactory = lowerRiver.AdaptedOutputFactories[0];
      IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(flowOnBranch, _queryItem1);
      ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], flowOnBranch, _queryItem1);
      adaptedOutput.AddConsumer(_queryItem1);

      // Connections have been established, validate and prepare the models
      lowerRiver.Validate();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Valid);
      lowerRiver.Prepare();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Updated);

      upperRiver.Validate();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Valid);
      upperRiver.Prepare();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Updated);

      // check initial values
      Assert.AreEqual(1, flowOnBranch.Values.Values2D[0].Count, "#values for " + flowOnBranch.Id);
      Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

      // specify query times
      double firstTriggerGetValuesTime = ((LinkableEngine)lowerRiver).CurrentTime.StampAsModifiedJulianDay;
      double secondTriggerGetValuesTime = firstTriggerGetValuesTime + 3;
      double thirdTriggerGetValuesTime = firstTriggerGetValuesTime + 4.3;

      // check initial values
      Assert.AreEqual(1, flowOnBranch.Values.Values2D[0].Count, "#values for " + flowOnBranch.Id);
      Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times, same as initial time, therefor intial values
      _queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(7.0, values.GetValue(0, 0), "value for first query time");

      // upper river provides 35/4 inflow to lower river. Lower river last branch flow:
      // 35/4 (from own runoff) + 35/4/8 (from first node inflow) = 315/32
      _queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(315.0 / 32.0, values.GetValue(0, 0), "value for second query time");

      // upper river provides 35/4 inflow to lower river. Lower river last branch flow:
      // 35/4 (from own runoff) + 35/4/8 (from first node inflow) = 315/32
      _queryItem1.TimeSet.SetSingleTimeStamp(thirdTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      Assert.AreEqual(315.0 / 32.0, values.GetValue(0, 0), "value for third query time");

      upperRiver.Finish();
      lowerRiver.Finish();

    }

    /// <summary>
    /// This is a variation of <see cref="GetValues2A"/>. In this test also
    /// a <see cref="TimeSeriesComponent"/> is adding inflow at the first node
    /// of the upper river. This is what makes this test different from
    /// <see cref="GetValues2A"/>, where everytning is the same for every time step
    /// </summary>
    [Test]
    public void GetValues2BInputAsSpans()
    {
      RunGetValues2BTest(true);
    }

    /// <summary>
    /// This is a variation of <see cref="GetValues2BInputAsStamps"/>, where now
    /// the exchange items store its values as stamps instead of spans.
    /// </summary>
    [Test]
    public void GetValues2BInputAsStamps()
    {
      RunGetValues2BTest(false);
    }

    private void RunGetValues2BTest(bool inputTimesAsSpans)
    {
      ITimeSpaceComponent timeSeries = new TimeSeriesComponent();
      ITimeSpaceComponent upperRiver = CreateRiverModel();
      ITimeSpaceComponent lowerRiver = CreateRiverModel();

      timeSeries.Initialize();

      IArgument flowAsStampsArgument = new ArgumentBool("flowItemsAsSpan", inputTimesAsSpans);

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> upperRiverArguments = CreateRiverModelArguments(upperRiver);
      upperRiverArguments.Add(Argument.Create("ModelID", "upperRiverModel", true, "argument"));
      upperRiverArguments.Add(flowAsStampsArgument);
      upperRiver.Initialize(upperRiverArguments);

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> lowerRiverArguments = CreateRiverModelArguments(lowerRiver);
      lowerRiverArguments.Add(Argument.Create("ModelID", "lowerRiverModel", true, "argument"));
      lowerRiverArguments.Add(flowAsStampsArgument);
      lowerRiver.Initialize(lowerRiverArguments);

      Assert.AreEqual("upperRiverModel", upperRiver.Id);
      Assert.AreEqual("lowerRiverModel", lowerRiver.Id);

      // Link upper river inflow to timeseries
      IBaseInput upperRiverInput = upperRiver.Inputs[0];
      IBaseOutput timeSeriesOutput = timeSeries.Outputs[0];
      timeSeriesOutput.AddConsumer(upperRiverInput);

      // Link upper river outflow to lower river inflow
      IBaseOutput upperRiverOutput = upperRiver.Outputs[2];
      IBaseInput lowerRiverInput = lowerRiver.Inputs[0];
      upperRiverOutput.AddConsumer(lowerRiverInput);

      // Link some lower river output item to the 'triggering' query item
      // Put a time interpolator in between, to take care that any required time stamp can be provided
      ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(lowerRiver, "Branch:2:Flow");
      IAdaptedOutputFactory adaptedOutputFactory = lowerRiver.AdaptedOutputFactories[0];
      IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(flowOnBranch, _queryItem1);
      ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], flowOnBranch, _queryItem1);

      adaptedOutput.AddConsumer(_queryItem1);

      // Connections have been established, validate the models
      timeSeries.Validate();
      Assert.IsTrue(timeSeries.Status == LinkableComponentStatus.Valid);
      timeSeries.Prepare();

      lowerRiver.Validate();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Valid);
      lowerRiver.Prepare();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Updated);

      upperRiver.Validate();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Valid);
      upperRiver.Prepare();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Updated);

      // specify query times
      double startTime = ((LinkableEngine)lowerRiver).CurrentTime.StampAsModifiedJulianDay;
      double firstTriggerGetValuesTime = startTime + 12.5;
      double secondTriggerGetValuesTime = startTime + 16.2;

      // check initial values
      Assert.AreEqual(1, flowOnBranch.Values.Values2D[0].Count, "#values for " + flowOnBranch.Id);
      Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times, 12.5 days after 01-01 (13 L/s inflow from timeseries)
      // Upper river provides 35/4 (runoff) + 13/8) to lower river
      // Lower river last branch flow: 35/4 (own runoff) + 35/4/8 (upper runoff) + 13/8/8 (upper inflow) = 315/32 + 13/64 
      _queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = adaptedOutput.GetValues(_queryItem1);
      if (inputTimesAsSpans)
      {
        Assert.AreEqual(315.0 / 32.0 + 13.0 / 64.0, values.GetValue(0, 0), "value for second query time");
      }
      else
      {
        double vala = 315.0 / 32.0 + 13.0 / 64.0; // value at 12/01
        double valb = 315.0 / 32.0 + 14.0 / 64.0; // value at 13/01
        double val = 0.5 * vala + 0.5 * valb;     // interpolate
        Assert.AreEqual(val, values.GetValue(0, 0), "value for second query time");
      }

      // get values for specified query times, 16.2 days after 01-01 (17 L/s inflow from timeseries)
      // Upper river provides 35/4 (runoff) + 17/8) to lower river
      // Lower river last branch flow: 35/4 (own runoff) + 35/4/8 (upper runoff) + 17/8/8 (upper inflow) = 315/32 + 17/64 
      _queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      if (inputTimesAsSpans)
      {
        Assert.AreEqual(315.0 / 32.0 + 17.0 / 64.0, values.GetValue(0, 0), "value for third query time");
      }
      else
      {
        double vala = 315.0 / 32.0 + 17.0 / 64.0; // value at 16/01
        double valb = 315.0 / 32.0 + 18.0 / 64.0; // value at 17/01
        double val = 0.8 * vala + 0.2 * valb;     // interpolate
        Assert.AreEqual(val, (double)values.GetValue(0, 0), 1e-10, "value for third query time");
      }

      timeSeries.Finish();
      upperRiver.Finish();
      lowerRiver.Finish();
    }


    [Test]
    public void GetValues2CInputAsSpans()
    {
      RunGetValues2CTest(true, false);
    }

    /// <summary>
    /// This is a variation of the <see cref="GetValues2BInputAsStamps"/>, 
    /// where the time step of the two rivers differ.
    /// </summary>
    [Test]
    public void GetValues2CInputAsStamps()
    {
      RunGetValues2CTest(false, false);
    }

    /// <summary>
    /// This is a variation of the <see cref="GetValues2CInputAsStamps"/>, 
    /// where the values are stored in the exchange item.
    /// </summary>
    [Test]
    public void GetValues2CInputAsStampsStoredInItems()
    {
      RunGetValues2CTest(false, true);
    }

    private void RunGetValues2CTest(bool inputTimesAsSpans, bool storeValuesInItems)
    {
      ITimeSpaceComponent timeSeries = new TimeSeriesComponent();
      ITimeSpaceComponent upperRiver = CreateRiverModel();
      ITimeSpaceComponent lowerRiver = CreateRiverModel();

      _flowItemsAsSpan = true;

      timeSeries.Initialize();

      IArgument flowAsStampsArgument = new ArgumentBool("flowItemsAsSpan", inputTimesAsSpans);
      IArgument storeInItemsArgument = new ArgumentBool("storeValuesInExchangeItems", storeValuesInItems);

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> upperRiverArguments = CreateRiverModelArguments(upperRiver);
      upperRiverArguments.Add(Argument.Create("ModelID", "upperRiverModel", true, "argument"));
      upperRiverArguments.Add(flowAsStampsArgument);
      upperRiverArguments.Add(Argument.Create("TimeStepLength", 21600));
      upperRiverArguments.Add(storeInItemsArgument);
      upperRiver.Arguments.ApplyArguments(upperRiverArguments);
      upperRiver.Initialize();

      // The ModelID is passed in order to make it easier to debug, otherwise you cannot se the difference between the two istances of RiverModelLC
      List<IArgument> lowerRiverArguments = CreateRiverModelArguments(lowerRiver);
      lowerRiverArguments.Add(Argument.Create("ModelID", "lowerRiverModel", true, "argument"));
      lowerRiverArguments.Add(flowAsStampsArgument);
      lowerRiverArguments.Add(Argument.Create("TimeStepLength", 86400));
      lowerRiverArguments.Add(storeInItemsArgument);
      lowerRiver.Arguments.ApplyArguments(lowerRiverArguments);
      lowerRiver.Initialize();

      Assert.AreEqual("upperRiverModel", upperRiver.Id);
      Assert.AreEqual("lowerRiverModel", lowerRiver.Id);

      // Link upper river outflow to lower river inflow
      IBaseInput upperRiverInput = upperRiver.Inputs[0];
      IBaseOutput timeSeriesOutput = timeSeries.Outputs[0];
      timeSeriesOutput.AddConsumer(upperRiverInput);

      // Link upper river outflow to lower river inflow
      // Put a time interpolator in between, to handle the non equal time steps
      IBaseOutput upperRiverOutput = upperRiver.Outputs[2];
      IBaseInput lowerRiverInput = lowerRiver.Inputs[0];
      IAdaptedOutputFactory adaptedOutputFactory = upperRiver.AdaptedOutputFactories[0];
      IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(upperRiverOutput, lowerRiverInput);
      ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], upperRiverOutput, lowerRiverInput);

      adaptedOutput.AddConsumer(lowerRiverInput);

      // Link some lower river output item to the 'triggering' query item
      // Put a time interpolator in between, to take care that any required time stamp can be provided
      ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(lowerRiver, "Branch:2:Flow");
      adaptedOutputFactory = lowerRiver.AdaptedOutputFactories[0];
      adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(flowOnBranch, _queryItem1);
      adaptedOutput = (ITimeSpaceAdaptedOutput)
          adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIds[0], flowOnBranch, _queryItem1);

      adaptedOutput.AddConsumer(_queryItem1);

      // Connections have been established, validate the models
      timeSeries.Validate();
      Assert.IsTrue(timeSeries.Status == LinkableComponentStatus.Valid);
      timeSeries.Prepare();

      lowerRiver.Validate();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Valid);
      lowerRiver.Prepare();
      Assert.IsTrue(lowerRiver.Status == LinkableComponentStatus.Updated);

      upperRiver.Validate();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Valid);
      upperRiver.Prepare();
      Assert.IsTrue(upperRiver.Status == LinkableComponentStatus.Updated);

      // specify query times
      double startTime = ((LinkableEngine)lowerRiver).CurrentTime.StampAsModifiedJulianDay;
      double firstTriggerGetValuesTime = startTime + 12.5;
      double secondTriggerGetValuesTime = startTime + 16.2;

      // check initial values
      Assert.AreEqual(1, flowOnBranch.Values.Values2D[0].Count, "#values for " + flowOnBranch.Id);
      Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

      // get values for specified query times
      _queryItem1.TimeSet.SetSingleTimeStamp(firstTriggerGetValuesTime);
      ITimeSpaceValueSet values = adaptedOutput.GetValues(_queryItem1);
      if (inputTimesAsSpans)
      {
        Assert.AreEqual(315.0 / 32.0 + 13.0 / 64.0, values.GetValue(0, 0), "value for second query time");
      }
      else
      {
        Assert.AreEqual(10.0546875, values.GetValue(0, 0), "value for second query time");
      }

      _queryItem1.TimeSet.SetSingleTimeStamp(secondTriggerGetValuesTime);
      values = adaptedOutput.GetValues(_queryItem1);
      if (inputTimesAsSpans)
      {
        Assert.AreEqual(315.0 / 32.0 + 17.0 / 64.0, values.GetValue(0, 0), "value for second query time");
      }
      else
      {
        Assert.AreEqual(10.112499999999955, values.GetValue(0, 0), "value for third query time");
      }

      timeSeries.Finish();
      upperRiver.Finish();
      lowerRiver.Finish();
    }


  }

  /// <summary>
  /// Testing the delegate version of the river model
  /// </summary>
  public class RiverModelDelegateTest : RiverModelTest
  {
    [TestFixtureSetUp]
    public new void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelDelegateFactory();
    }
  }

  /// <summary>
  /// Testing the interface version of the river model
  /// </summary>
  public class RiverModelInterfaceTest : RiverModelTest
  {
    [TestFixtureSetUp]
    public new void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelInterfaceFactory();
    }
  }



}
