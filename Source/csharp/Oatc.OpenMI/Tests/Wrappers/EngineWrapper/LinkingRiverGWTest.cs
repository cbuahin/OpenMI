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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using Oatc.OpenMI.Examples.GroundWaterModel;
using Oatc.OpenMI.Examples.SimpleSCharpRiver;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  /// <summary>
  /// Tests for linking the <see cref="RiverModelLC"/>
  /// and the <see cref="GWModelLC"/>
  ///  </summary>
  [TestFixture]
  public class LinkingRiverGwTest
  {
    //private ILinkableComponent gwModel;

    protected IRiverModelFactory _riverModelFactory;


    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _riverModelFactory = new RiverModelFactory();
    }

    public int GwModelInstance;

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

    public Input CreateDischargeInput()
    {
      Quantity dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      Input queryItem = new Input("discharge, to be retrieved from some output item", dischargeQuantity, idBasedElementSetA);
      queryItem.TimeSet = new TimeSet();

      return (queryItem);
    }

    public Input CreateVolumeInput()
    {
      Quantity volumeQuantity = new Quantity(new Unit(PredefinedUnits.Liter), null, "Volume");

      ElementSet idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
      idBasedElementSetA.AddElement(new Element("elm-1"));

      Input queryItem = new Input("Volume, to be retrieved from some output item", volumeQuantity, idBasedElementSetA);
      queryItem.TimeSet = new TimeSet();

      return (queryItem);
    }

    [SetUp]
    public void CreateModelsAndInputItemsForQuery()
    {
    }

    /// <summary>
    /// Test that couples a ground water model and a river model.
    /// <para>
    /// The river model leaks water into the ground water model
    /// and the ground water model provides a ground water level
    /// which the river model bases its leaking calculation on.
    /// </para>
    /// </summary>
    [Test]
    public void CouplingGwRiver()
    {

      /// bit 1: Decides whether the timeInterpolator or grid-to-line adaptor comes first
      /// bit 2: when true, using a 16x16 gw grid (instead of 2x2)
      /// bit 3: when true, bi-directinal: adds a link from gwModel to river, with the gw-level

      for (int runNumber = 0; runNumber < 8; runNumber++)
      {

        //if (runNumber != 3)
        //  continue;

        Console.Out.WriteLine("runNumber: " + runNumber);

        // Create trigger inputs
        Input queryDischargeItem = CreateDischargeInput();
        Input queryVolume = CreateVolumeInput();

        // Create models
        LinkableEngine riverModel = CreateRiverModel();
        ITimeSpaceComponent gwModel = CreateGwModel();

        // Add arguments and initialize
        IDictionary<string, IArgument> gwArgs = gwModel.Arguments.Dictionary();
        // Increasing model grid size (otherwise GW model runs full too fast)
        gwArgs.UpdateValue("dx", 400.0);
        gwArgs.UpdateValue("dy", 400.0);
        gwArgs.UpdateValue("x0", 200.0);
        gwArgs.UpdateValue("y0", 200.0);
        int gwGridSize = 2 * 2;
        if ((runNumber & 2) == 2)  // set 16 x 16 grid
        {
          gwArgs.UpdateValue("dx", 50.0);
          gwArgs.UpdateValue("dy", 50.0);
          gwArgs.UpdateValue("XCount", 16);
          gwArgs.UpdateValue("ycount", 16);
          gwGridSize = 16 * 16;
        }
        gwModel.Initialize();

        IDictionary<string, IArgument> riverArgs = riverModel.Arguments.Dictionary();
        // Increasing model grid size (otherwise GW model runs full too fast)
        riverArgs.UpdateValue("xyscale", 100.0);
        riverModel.Initialize();

        // Connect triggering inputs
        ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(riverModel, "Branch:2:Flow");
        TimeInterpolator flowOnBranch2 = new TimeInterpolator(flowOnBranch);
        flowOnBranch.AddAdaptedOutput(flowOnBranch2);
        flowOnBranch2.AddConsumer(queryDischargeItem);

        ITimeSpaceOutput storageInGw = UTHelper.FindOutputItem(gwModel, "Grid.Storage");
        TimeInterpolator storageInGw2 = new TimeInterpolator(storageInGw);
        storageInGw.AddAdaptedOutput(storageInGw2);
        storageInGw2.AddConsumer(queryVolume);

        //========== Couple leakage items ==========
        // put leakage from river into ground water model
        {
          ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel, "WholeRiver:Leakage");
          ITimeSpaceInput gwInflowInput = UTHelper.FindInputItem(gwModel, "Grid.Inflow");

          // Two adaptors are added: Time buffer and line-to-grid adaptor
          // they can be added in any order (though time buffer first will use less memory)
          if ((runNumber & 1) == 1)
          {
            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            riverLeakageOutputGrid.AddConsumer(gwInflowInput);

          }
          else
          {
            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutputGrid);

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutputGrid);
            riverLeakageOutputGrid.AddAdaptedOutput(riverLeakageOutput2);

            riverLeakageOutput2.AddConsumer(gwInflowInput);

          }
        }
        //========== Couple ground water level items ==========

        if ((runNumber & 4) == 4)
        {
          // put ground water level from ground water model into river
          ITimeSpaceInput riverGwleveInput = UTHelper.FindInputItem(riverModel, "WholeRiver:GroundWaterLevel");
          ITimeSpaceOutput gwLevelOutput = UTHelper.FindOutputItem(gwModel, "Grid.gwLevel");

          // Two adaptors are added: Time buffer and grid-to-line adaptor
          // they can be added in any order (though time buffer last will use less memory)
          if ((runNumber & 1) == 1)
          {
            // Time interpolator
            var gwLevelOutput2 = new TimeExtrapolator(gwLevelOutput);
            gwLevelOutput.AddAdaptedOutput(gwLevelOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            var gwLevelOutputLine =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper700"), gwLevelOutput2,
                                             riverGwleveInput.ElementSet());
            gwLevelOutput2.AddAdaptedOutput(gwLevelOutputLine);

            gwLevelOutputLine.AddConsumer(riverGwleveInput);

          }
          else
          {
            // Element mapper from polyline to polygon, weighted sum version
            var gwLevelOutputLine =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper700"), gwLevelOutput,
                                             riverGwleveInput.ElementSet());
            gwLevelOutput.AddAdaptedOutput(gwLevelOutputLine);

            // Time interpolator
            var gwLevelOutput2 = new TimeExtrapolator(gwLevelOutputLine);
            gwLevelOutputLine.AddAdaptedOutput(gwLevelOutput2);

            gwLevelOutput2.AddConsumer(riverGwleveInput);

          }
        }

        //========== Run ==========

        // Validate
        riverModel.Validate();
        Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Valid);
        gwModel.Validate();
        Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Valid);

        // Prepare
        riverModel.Prepare();
        Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Updated);
        gwModel.Prepare();
        Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Updated);


        // specify query times
        double triggerTime0 = riverModel.CurrentTime.StampAsModifiedJulianDay;
        double triggerTime1 = triggerTime0 + 1;
        double triggerTime2 = triggerTime0 + 2;
        double triggerTime3 = triggerTime0 + 12.1;
        double triggerTime4 = triggerTime0 + 16.7;

        /// Properties of the river, without gw-level input
        /// Inflow into each node from rainfall runoff is 10 L/s
        /// Inflow to node 1: 10        L/s - leaking  5   L/s on branch 1
        /// Inflow to node 2: 10 +    5 L/s - leaking 15/2 L/s on branch 2
        /// Inflow to node 3: 10 + 15/2 L/s - leaking 35/4 L/s on branch 3
        /// Total leakage 5+15/2+35/4 = (20+30+35)/4 = 85/4 L/s
        /// 
        /// Number of seconds in a day: 60*60*24 = 86400

        // check initial values
        Assert.AreEqual(1, ValueSet.GetElementCount(flowOnBranch.Values), "#values for " + flowOnBranch.Id);
        Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

        Assert.AreEqual(gwGridSize, ValueSet.GetElementCount(storageInGw.Values), "#values for " + storageInGw.Id);
        Assert.AreEqual(0, SumTimeStep(storageInGw.Values, 0));

        // get values for specified query times, 1 days
        // Totally leaking: 86400 * 85/4 = 1.836e6
        // For the bi-directional coupling:
        // the entire first day the river uses extrapolated values from the
        // gwModel, which gives a gwLevel of -10, hence same value as for the uni-directional
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime1);
        ITimeSpaceValueSet valuesV = storageInGw2.GetValues(queryDischargeItem);
        ITimeSpaceValueSet valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
        Assert.AreEqual(1.836e6, SumTimeStep(valuesV, 0), 1e-4);

        // Print out, to load in a plotting program for verification
        //StringBuilder b = new StringBuilder();
        //foreach (double val in valuesV.GetElementValuesForTime(0))
        //  b.AppendLine(val.ToString(NumberFormatInfo.InvariantInfo));
        //Console.Out.WriteLine(b.ToString());

        // get values for specified query times, 2 days
        // 2 * 86400 * 85/4 = 3.672e6
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime2);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        if ((runNumber & 4) != 4)       // unidirectional
        {
          Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
          Assert.AreEqual(3.672e6, SumTimeStep(valuesV, 0), 1e-4);
        }
        else if ((runNumber & 2) != 2)  // bi-directional 2x2 grid
        {
          Assert.AreEqual(8.843648, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(3.66390879366e6, SumTimeStep(valuesV, 0), 1e-4);
        }
        else                            // bi-directional 16x16 grid
        {
          Assert.AreEqual(9.65307, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(3.59397465219e6, SumTimeStep(valuesV, 0), 1e-4);
        }

        // get values for specified query times, 12.1 days
        // 12.1 * 86400 * 85/4 = 2.22156e7
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime3);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        if ((runNumber & 4) != 4)       // unidirectional
        {

          Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
          Assert.AreEqual(2.22156e7, SumTimeStep(valuesV, 0), 1e-4);
        }
        else if ((runNumber & 2) != 2)  // bi-directional 2x2 grid
        {
          Assert.AreEqual(9.87828, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(2.16704019338e7, SumTimeStep(valuesV, 0), 1e-4);
        }
        else                            // bi-directional 16x16 grid
        {
          Assert.AreEqual(18.546999, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(1.722400002557e7, SumTimeStep(valuesV, 0), 1e-4);
        }

        // get values for specified query times, 16.7 days
        // 16.7 * 86400 * 85/4 = 3.06612e7
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime4);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        if ((runNumber & 4) != 4)       // unidirectional
        {
          Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
          Assert.AreEqual(3.06612e7, SumTimeStep(valuesV, 0), 1e-4);
        }
        else if ((runNumber & 2) != 2)  // bi-directional 2x2 grid
        {
          Assert.AreEqual(10.255535, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(2.9595872035072e7, SumTimeStep(valuesV, 0), 1e-4);
        }
        else                            // bi-directional 16x16 grid
        {
          Assert.AreEqual(20.98699, (double)valuesQ.GetValue(0, 0), 1e-4);
          Assert.AreEqual(2.12991179998e7, SumTimeStep(valuesV, 0), 1e-4);
        }

      }
    }


    /// <summary>
    /// Test that couples a ground water model and two river models.
    /// <para>
    /// Look for the lines starting with // Note !!!:
    /// </para>
    /// </summary>
    [Test]
    public void CouplingGwRiver2()
    {

      /// runNumber 0: Using MultiInput
      /// runNumber 1: Using MultiInputAdaptor
      /// runNumber 2: Using MultiInputAdaptorFactory

      for (int runNumber = 0; runNumber < 3; runNumber++)
      {

        Console.Out.WriteLine("runNumber: " + runNumber);

        // Create trigger inputs
        Input queryDischargeItem = CreateDischargeInput();
        Input queryVolume = CreateVolumeInput();

        // Create models
        LinkableEngine riverModel = CreateRiverModel();
        LinkableEngine riverModel2 = CreateRiverModel();
        ITimeSpaceComponent gwModel = CreateGwModel();

        // Add arguments and initialize
        IDictionary<string, IArgument> gwArgs = gwModel.Arguments.Dictionary();
        // Increasing model grid size (otherwise GW model runs full too fast)
        gwArgs.UpdateValue("dx", 50.0);
        gwArgs.UpdateValue("dy", 50.0);
        gwArgs.UpdateValue("x0", 0.0);
        gwArgs.UpdateValue("y0", 200.0);
        gwArgs.UpdateValue("XCount", 24);
        gwArgs.UpdateValue("ycount", 16);
        if (runNumber == 0)
          gwArgs.UpdateValue("UseMultiInput", true);
        gwModel.Initialize();
        int gwGridSize = 24 * 16;

        IDictionary<string, IArgument> riverArgs = riverModel.Arguments.Dictionary();
        // Increasing model grid size (otherwise GW model runs full too fast)
        riverArgs.UpdateValue("xyscale", 100.0);
        riverModel.Initialize();

        IDictionary<string, IArgument> river2Args = riverModel2.Arguments.Dictionary();
        // Increasing model grid size (otherwise GW model runs full too fast)
        river2Args.UpdateValue("xyscale", 100.0);
        // Move river2 sligthly away from river1
        river2Args.UpdateValue("xoffset", -220.0);
        river2Args.UpdateValue("yoffset", 180.0);
        riverModel2.Initialize();

        // Connect triggering inputs
        ITimeSpaceOutput flowOnBranch = UTHelper.FindOutputItem(riverModel, "Branch:2:Flow");
        TimeInterpolator flowOnBranch2 = new TimeInterpolator(flowOnBranch);
        flowOnBranch.AddAdaptedOutput(flowOnBranch2);
        flowOnBranch2.AddConsumer(queryDischargeItem);

        ITimeSpaceOutput storageInGw = UTHelper.FindOutputItem(gwModel, "Grid.Storage");
        TimeInterpolator storageInGw2 = new TimeInterpolator(storageInGw);
        storageInGw.AddAdaptedOutput(storageInGw2);
        storageInGw2.AddConsumer(queryVolume);

        //========== Couple leakage items ==========
        ITimeSpaceInput gwInflowInput = UTHelper.FindInputItem(gwModel, "Grid.Inflow");


        //========== IBaseMultiInput linking ==========
        if (runNumber == 0)
        {
          /// Example of adding up two outputs into one input, by the use of 
          /// an IBaseMultiInput implementation

          Assert.IsTrue(gwInflowInput is IBaseMultiInput);
          Assert.IsTrue(gwInflowInput is ITimeSpaceMultiInput);

          // put leakage from river1 into ground water model
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel, "WholeRiver:Leakage");

            // Two adaptors are added: Time buffer and line-to-grid adaptor
            // they can be added in any order (though time buffer first will use less memory)

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: No special action
            riverLeakageOutputGrid.AddConsumer(gwInflowInput);
          }

          // put leakage from river2 into ground water model
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel2, "WholeRiver:Leakage");

            // Two adaptors are added: Time buffer and line-to-grid adaptor
            // they can be added in any order (though time buffer first will use less memory)

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: No special action
            riverLeakageOutputGrid.AddConsumer(gwInflowInput);
          }

        }

        //========== MultiInputAdaptor linking ==========
        if (runNumber == 1) {

          /// Example of adding up two outputs into one input, by the use of 
          /// a MultiInputAdaptor class

          // Note !!!: Creating a MultiInputAdaptor
          MultiInputAdaptor sourceAdder = new MultiInputAdaptor("SomeId")
                                                {
                                                  SpatialDefinition = gwInflowInput.SpatialDefinition
                                                };

          // put leakage from river1 into ground water model
          // Two adaptors are added: Time buffer and line-to-grid adaptor
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel, "WholeRiver:Leakage");

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: Adding to the list of adaptees
            sourceAdder.Adaptees.Add(riverLeakageOutputGrid);
            riverLeakageOutputGrid.AddAdaptedOutput(sourceAdder);
          }

          // put leakage from river2 into ground water model
          // Two adaptors are added: Time buffer and line-to-grid adaptor
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel2, "WholeRiver:Leakage");

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: Adding to the list of adaptees
            sourceAdder.Adaptees.Add(riverLeakageOutputGrid);
            riverLeakageOutputGrid.AddAdaptedOutput(sourceAdder);
          }

          // Note !!!: Connect the gwInflowInput and the multiInputAdaptor
          sourceAdder.AddConsumer(gwInflowInput);

        }
        
        //========== MultiInputAdaptorFactory linking ==========
        if (runNumber == 2)
        {

          /// Example of adding up two outputs into one input, by the use of 
          /// an MultiInputAdaptorFactory implementation
          
          var factory = new MultiInputAdaptorFactory(gwModel);

          // put leakage from river1 into ground water model
          // Two adaptors are added: Time buffer and line-to-grid adaptor
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel, "WholeRiver:Leakage");

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: Creating a new AdaptedOutput and adding it
            IIdentifiable[] identifiables = factory.GetAvailableAdaptedOutputIds(riverLeakageOutputGrid, gwInflowInput);
            IBaseAdaptedOutput myOutput = factory.CreateAdaptedOutput(identifiables[0], riverLeakageOutputGrid, gwInflowInput);

            myOutput.AddConsumer(gwInflowInput);
          }

          // put leakage from river2 into ground water model
          // Two adaptors are added: Time buffer and line-to-grid adaptor
          {
            ITimeSpaceOutput riverLeakageOutput = UTHelper.FindOutputItem(riverModel2, "WholeRiver:Leakage");

            // Time interpolator
            TimeInterpolator riverLeakageOutput2 = new TimeInterpolator(riverLeakageOutput);
            riverLeakageOutput.AddAdaptedOutput(riverLeakageOutput2);

            // Element mapper from polyline to polygon, weighted sum version
            ElementMapperAdaptedOutput riverLeakageOutputGrid =
              new ElementMapperAdaptedOutput(new Identifier("ElementMapper501"), riverLeakageOutput2,
                                             gwInflowInput.ElementSet());
            riverLeakageOutput2.AddAdaptedOutput(riverLeakageOutputGrid);

            // Note !!!: Creating a new AdaptedOutput and adding it
            IIdentifiable[] identifiables = factory.GetAvailableAdaptedOutputIds(riverLeakageOutputGrid, gwInflowInput);
            IBaseAdaptedOutput myOutput = factory.CreateAdaptedOutput(identifiables[0], riverLeakageOutputGrid, gwInflowInput);

            myOutput.AddConsumer(gwInflowInput);

          }

        }
        
        
        //========== Run ==========

        // Validate
        riverModel.Validate();
        Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Valid);
        riverModel2.Validate();
        Assert.IsTrue(riverModel2.Status == LinkableComponentStatus.Valid);
        gwModel.Validate();
        Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Valid);

        // Prepare
        riverModel.Prepare();
        Assert.IsTrue(riverModel.Status == LinkableComponentStatus.Updated);
        riverModel2.Prepare();
        Assert.IsTrue(riverModel2.Status == LinkableComponentStatus.Updated);
        gwModel.Prepare();
        Assert.IsTrue(gwModel.Status == LinkableComponentStatus.Updated);


        // specify query times
        double triggerTime0 = riverModel.CurrentTime.StampAsModifiedJulianDay;
        double triggerTime1 = triggerTime0 + 1;
        double triggerTime2 = triggerTime0 + 2;
        double triggerTime3 = triggerTime0 + 12.1;
        double triggerTime4 = triggerTime0 + 16.7;

        /// Properties of the river, without gw-level input
        /// Inflow into each node from rainfall runoff is 10 L/s
        /// Inflow to node 1: 10        L/s - leaking  5   L/s on branch 1
        /// Inflow to node 2: 10 +    5 L/s - leaking 15/2 L/s on branch 2
        /// Inflow to node 3: 10 + 15/2 L/s - leaking 35/4 L/s on branch 3
        /// Total leakage 5+15/2+35/4 = (20+30+35)/4 = 85/4 L/s
        /// 
        /// Number of seconds in a day: 60*60*24 = 86400

        // check initial values
        Assert.AreEqual(1, ValueSet.GetElementCount(flowOnBranch.Values), "#values for " + flowOnBranch.Id);
        Assert.AreEqual(7.0, (double)flowOnBranch.Values.GetValue(0, 0), "Value[0] as property");

        Assert.AreEqual(gwGridSize, ValueSet.GetElementCount(storageInGw.Values), "#values for " + storageInGw.Id);
        Assert.AreEqual(0, SumTimeStep(storageInGw.Values, 0));

        // get values for specified query times, 1 days
        // Totally leaking: 86400 * 85/4 = 1.836e6
        // For the bi-directional coupling:
        // the entire first day the river uses extrapolated values from the
        // gwModel, which gives a gwLevel of -10, hence same value as for the uni-directional
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime1);
        ITimeSpaceValueSet valuesV = storageInGw2.GetValues(queryDischargeItem);
        ITimeSpaceValueSet valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
        Assert.AreEqual(2 * 1.836e6, SumTimeStep(valuesV, 0), 1e-4);

        // Print out, to load in a plotting program for verification
        StringBuilder b = new StringBuilder();

        IList valV = valuesV.GetElementValuesForTime(0);
        int ivalvV = 0;
        for (int i = 0; i < 16; i++)
        {
          for (int j = 0; j < 24; j++)
          {
            b.Append(((double)valV[ivalvV++]).ToString(NumberFormatInfo.InvariantInfo));
            b.Append(" ");
          }
          b.AppendLine();
        }
        //Console.Out.WriteLine(b.ToString());

        // get values for specified query times, 2 days
        // 2 * 86400 * 85/4 = 3.672e6
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime2);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
        Assert.AreEqual(2 * 3.672e6, SumTimeStep(valuesV, 0), 1e-4);

        // get values for specified query times, 12.1 days
        // 12.1 * 86400 * 85/4 = 2.22156e7
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime3);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
        Assert.AreEqual(2 * 2.22156e7, SumTimeStep(valuesV, 0), 1e-4);

        // get values for specified query times, 16.7 days
        // 16.7 * 86400 * 85/4 = 3.06612e7
        queryDischargeItem.TimeSet.SetSingleTimeStamp(triggerTime4);
        valuesV = storageInGw2.GetValues(queryDischargeItem);
        valuesQ = flowOnBranch2.GetValues(queryDischargeItem);
        Assert.AreEqual(35.0 / 4.0, (double)valuesQ.GetValue(0, 0));
        Assert.AreEqual(2 * 3.06612e7, SumTimeStep(valuesV, 0), 1e-4);
      }
    }

    private IIdentifiable FindId(string id, IIdentifiable[] identifiables)
    {
      return Array.Find(identifiables,
                        delegate(IIdentifiable match)
                          {
                            if (string.Equals(match.Id, id,
                                              StringComparison.OrdinalIgnoreCase))
                              return (true);
                            return (false);
                          }
        );
    }


    private static double SumTimeStep(ITimeSpaceValueSet values, int timestepIndex)
    {
      IList<double> vals = (IList<double>)values.GetElementValuesForTime(timestepIndex);
      double sum = 0;
      foreach (double d in vals)
      {
        sum += d;
      }
      return (sum);
    }
  }
}
