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
// namespace:Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest1
// purpose: UnitTest for the Oatc.OpenMI.Wrappers.EngineWrapper package
// file: RiverModelLC.cs
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
//    2009-09-21: Updated to version 2.0 (Jesper Grooss)
//
///////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.SimpleSCharpRiver
{
  /// <summary>
  /// <para>
  /// The river model is a simple stationary river model. The solution
  /// depends on the input of the current time step only, i.e., no history
  /// from time step to time step.
  /// </para>
  /// <para>
  /// It takes the inflow on each of the 4 nodes and moves it down the river.
  /// Half of the inflow is leaked, the other half is moved to the next node 
  /// storage and added to it. All inflow is either leaked or discharged
  /// in each timestep.
  /// </para>
  /// <para>
  /// The model has a constant runoff inflow of 10 L/s on each node. Additional 
  /// runoff or inflow can be added to each of the 4 nodes (though adding on the 
  /// last node has no effect)
  /// </para>
  /// </summary>
  /// <example>
  /// <para>
  /// For an inflow of 10 L/s (the runoff) in each node, then the flow in each branch
  /// will be 10*1/2 in the first 10*(1/2+1/4) in the second and 10*(1/2+1/4+1/8)=35/8 in the last.
  /// </para>
  /// <para>
  /// For inflow on one node, 1/2 will continue to the next node, 1/4 to the second node and 1/8 to the
  /// third node.
  /// </para>
  /// </example>
  /// <remarks>
  /// We could make the model more "dynamic" by letting 1/3 leak, 1/3 move to next node and 1/3 stay on
  /// the current node.
  /// </remarks>
  public class RiverModelLC : LinkableGetSetEngine //, OpenMI.Standard.IManageState
  {

    protected readonly Time _simulationEnd;
    protected readonly Time _simulationStart;
    protected readonly Time _currentTime;
    protected double _timeStepLengthInSeconds;
    protected double _timeStepLengthInDays;

    protected readonly int _numberOfNodes;
    protected readonly double _runoff;             // [liter pr second], constant
    protected double _lastNodeOutflow;             // [liter pr second], for mass balance check

    protected readonly double[] _xCoordinate;      // x-coordinates for the nodes
    protected readonly double[] _yCoordinate;      // y-coordinates for the nodes

    protected readonly double[] _inflowStorage;    // [liters], inflow for one time step at each node
    protected readonly double[] _flow;             // [liter pr second] branch variable
    protected readonly double[] _leakage;          // [liter pr second] branch variable
    protected readonly double[] _groundWaterLevel; // [meters], branch variable

    protected bool _flowItemsAsSpan = true;
    protected bool _storeValuesInItem = true;

    public RiverModelLC()
    {
      _xCoordinate = new double[] { 3, 5, 8, 9 };
      _yCoordinate = new double[] { 4, 7, 7, 3 };

      _numberOfNodes = _xCoordinate.Length;

      _simulationStart = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
      _simulationEnd = new Time(new DateTime(2005, 2, 10, 0, 0, 0));
      _currentTime = new Time(_simulationStart);
      _timeStepLengthInSeconds = 3600 * 24; //one day
      _timeStepLengthInDays = _timeStepLengthInSeconds / (24.0 * 3600.0);

      _inflowStorage = new double[_numberOfNodes];
      _groundWaterLevel = new double[_numberOfNodes-1];
      _flow = new double[_numberOfNodes - 1];
      _leakage = new double[_numberOfNodes - 1];
      _runoff = 10;

      for (int i = 0; i < _numberOfNodes; i++)
      {
        _inflowStorage[i] = 0;
      }

      for (int i = 0; i < _numberOfNodes-1; i++)
      {
        _groundWaterLevel[i] = -10;
      }


      List<IArgument> arguments = new List<IArgument>();

      arguments.Add(new ArgumentString("ModelId","RiverModel Default Model ID"));
      arguments.Add(new ArgumentBool("FlowItemsAsSpan", true));
      arguments.Add(new ArgumentBool("StoreValuesInExchangeitems", true));
      arguments.Add(new ArgumentDouble("TimestepLength", 3600 * 24));
    
      arguments.Add(new ArgumentDouble("xyscale", 1));
      arguments.Add(new ArgumentDouble("xoffset", 0));
      arguments.Add(new ArgumentDouble("yoffset", 0));
      
      arguments.TrimExcess();
      Arguments = arguments.AsReadOnly();

    }

    public double[] Leakage
    {
      get { return _leakage; }
    }

    public override void Initialize()
    {
      Status = LinkableComponentStatus.Initializing;

      // Handle arguments
      IDictionary<string, IArgument> argDict = Arguments.Dictionary();

      Id = argDict.GetValue<string>("ModelId");
      this.Caption = "RiverModel Default Model";

      _flowItemsAsSpan = argDict.GetValue<bool>("FlowItemsAsSpan");
      _storeValuesInItem = argDict.GetValue<bool>("StoreValuesInExchangeitems");
      _timeStepLengthInSeconds = argDict.GetValue<double>("TimestepLength");
      _timeStepLengthInDays = _timeStepLengthInSeconds / (24.0 * 3600.0);

      double xyscale = argDict.GetValue<double>("xyscale");
      double xOffset = argDict.GetValue<double>("xoffset");
      double yOffset = argDict.GetValue<double>("yoffset");

      for (int i = 0; i < _numberOfNodes; i++)
      {
        _xCoordinate[i] = xyscale*_xCoordinate[i] + xOffset;
        _yCoordinate[i] = xyscale*_yCoordinate[i] + yOffset;
      }
      
      // -- create a flow quantity --
      Dimension flowDimension = new Dimension();
      flowDimension.SetPower(DimensionBase.Length, 3);
      flowDimension.SetPower(DimensionBase.Time, -1);

      Unit literPrSecUnit = new Unit("LiterPrSecond", 0.001, 0, "Liters pr Second");

      Quantity flowQuantity = new Quantity(literPrSecUnit, "Flow", "Flow");
      Quantity leakageQuantity = new Quantity(literPrSecUnit, "Leakage", "Leakage");

      // -- create a ground water level quantity --
      Dimension levelDimension = new Dimension();
      levelDimension.SetPower(DimensionBase.Length, 1);

      Unit levelUnit = new Unit("GroundWaterLevel", 1, 0, "Ground water level");
      levelUnit.Dimension = levelDimension;

      Quantity levelQuantity = new Quantity(levelUnit, "Ground water level", "Ground Water level");

      // -- Time settings for input and output exchange items --
      ITime timeHorizon = new Time(StartTime, EndTime);

      // -- create and populate elementset to represent all branches, links betwen nodes in the river network --
      LineString fullRiverLineString =
        new LineString
          {
            Coordinates = GeometryFactory.CreateCoordinateList(_xCoordinate, _yCoordinate),
            Caption = "WholeRiver",
            Description = "WholeRiver",
            IsClosed = false,
            IsNodeBased = false,
          };
      IElementSet fullRiverElementSet = new LineStringWrapper(fullRiverLineString);

      // --- populate input exchange items for flow to individual nodes, id-based ---
      for (int i = 0; i < _numberOfNodes; i++)
      {
        string id = "Node:" + i;
        ElementSet elementSet = new ElementSet(id, id, ElementType.IdBased);
        elementSet.AddElement(new Element(id));

        EngineInputItem inputExchangeItem = CreateInputInflowToOneNode(i, id, flowQuantity, elementSet);
        inputExchangeItem.StoreValuesInExchangeItem = true; // Item is adding up, therefor store in item
        inputExchangeItem.TimeSet.SetTimeHorizon(timeHorizon);
        EngineInputItems.Add(inputExchangeItem);
      }

      // --- Populate input exchange item for flow into the whole river ---
      EngineInputItem wholeRiverFlowInputItem = CreateInputInflowToRiver(flowQuantity, fullRiverElementSet);
      wholeRiverFlowInputItem.TimeSet.SetTimeHorizon(timeHorizon);
      EngineInputItems.Add(wholeRiverFlowInputItem);

      // --- Populate input exchange item for ground water level of the whole river ---
      EngineDInputItem gwLevelInputItem = CreateInputGwLevel(levelQuantity, fullRiverElementSet);
      gwLevelInputItem.TimeSet.SetTimeHorizon(timeHorizon);
      EngineInputItems.Add(gwLevelInputItem);

      // --- Populate output exchange items for flow in river branches, id based ---
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        string id = "Branch:" + i;
        ElementSet elementSet = new ElementSet(id, id, ElementType.IdBased);
        elementSet.AddElement(new Element(id));

        EngineOutputItem outputExchangeItem = CreateOutputFlowInBranch(i, id, flowQuantity, elementSet);
        EngineOutputItems.Add(outputExchangeItem);
      }

      // --- populate output exchange items for leakage for individual branches --
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        string id = "Branch:" + i;
        ElementSet elementSet = new ElementSet(id, id, ElementType.IdBased);
        elementSet.AddElement(new Element(id));

        EngineOutputItem outputExchangeItem = CreateOutputLeakageInBranch(i, id, leakageQuantity, elementSet);
        EngineOutputItems.Add(outputExchangeItem);
      }

      // --- Populate output exchange item for leakage from the whole georeferenced river ---
      EngineOutputItem wholeRiverOutputExchangeItem = CreateOuputLeakageInRiver(leakageQuantity, fullRiverElementSet);
      EngineOutputItems.Add(wholeRiverOutputExchangeItem);

      // --- Populate output exchange item for flow from the whole georeferenced river ---
      EngineOutputItem wholeRiverFlowOutputExchangeItem = CreateOutputFlowInRiver(flowQuantity, fullRiverElementSet);
      EngineOutputItems.Add(wholeRiverFlowOutputExchangeItem);

      // --- populate with initial state variables ---
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        _flow[i] = 7;
      }

      foreach (EngineInputItem engineInputItem in EngineInputItems)
      {
        // TODO: Overwrites existing timeset, which has already had the time horizon set?
        engineInputItem.TimeSet = new TimeSet() { HasDurations = _flowItemsAsSpan };
      }
      foreach (EngineOutputItem engineOutputItem in EngineOutputItems)
      {
        // TODO: Overwrites existing timeset, which has already had the time horizon set?
        engineOutputItem.TimeSet = new TimeSet() { HasDurations = _flowItemsAsSpan };
      }

      Status = LinkableComponentStatus.Initialized;
    }

    protected virtual EngineDInputItem CreateInputGwLevel(Quantity levelQuantity, IElementSet fullRiverElementSet)
    {
      EngineDInputItem gwLevelInputItem = new EngineDInputItem("WholeRiver:GroundWaterLevel", levelQuantity, fullRiverElementSet, this) { StoreValuesInExchangeItem = true };
      gwLevelInputItem.ValueSetter = delegate(ITimeSpaceValueSet valueSet)
                                       {
                                         IList values = valueSet.GetElementValuesForTime(0);
                                         for (int i = 0; i < _numberOfNodes-1; i++)
                                         {
                                           _groundWaterLevel[i] = (double)values[i];
                                         }
                                       };
      return gwLevelInputItem;
    }

    // ============================================================
    #region Methods for creating exchange items

    protected virtual EngineInputItem CreateInputInflowToOneNode(int nodeIndex, string id, Quantity flowQuantity, IElementSet elementSet)
    {
      return new EngineEInputItem(id + ":Flow", flowQuantity, elementSet, this);
    }

    protected virtual EngineInputItem CreateInputInflowToRiver(Quantity flowQuantity, IElementSet fullRiverElementSet)
    {
      return new EngineEInputItem("WholeRiver:Flow", flowQuantity, fullRiverElementSet, this) { StoreValuesInExchangeItem = true };
    }

    protected virtual EngineOutputItem CreateOutputFlowInBranch(int branchIndex, string id, Quantity flowQuantity, ElementSet elementSet)
    {
      return new EngineEOutputItem(id + ":Flow", flowQuantity, elementSet, this);
    }

    protected virtual EngineOutputItem CreateOutputLeakageInBranch(int branchIndex, string id, Quantity leakageQuantity, ElementSet elementSet)
    {
      return new EngineEOutputItem(id + ":Leakage", leakageQuantity, elementSet, this);
    }

    protected virtual EngineOutputItem CreateOuputLeakageInRiver(Quantity leakageQuantity, IElementSet fullRiverElementSet)
    {
      return new EngineEOutputItem("WholeRiver:Leakage", leakageQuantity, fullRiverElementSet, this);
    }

    protected virtual EngineOutputItem CreateOutputFlowInRiver(Quantity flowQuantity, IElementSet fullRiverElementSet)
    {
      EngineEOutputItem wholeRiverFlowOutputExchangeItem = new EngineEOutputItem("WholeRiver:Flow", flowQuantity, fullRiverElementSet, this);
      return wholeRiverFlowOutputExchangeItem;
    }
    
    #endregion
    // ============================================================



    protected override void OnPrepare()
    {
    }

    protected override string[] OnValidate()
    {
      return new string[0];
    }

    public override void Finish()
    {
      Status = LinkableComponentStatus.Finishing;
      // no action
      Status = LinkableComponentStatus.Done;
    }
    protected override ITime StartTime
    {
      get { return _simulationStart; }
    }

    protected override ITime EndTime
    {
      get { return _simulationEnd; }
    }

    public override ITime GetCurrentTime(bool asStamp)
    {
      if (asStamp)
        return _currentTime;
      return (new Time(_currentTime.StampAsModifiedJulianDay - _timeStepLengthInDays, _timeStepLengthInDays));
    }

    public override ITime CurrentTime
    { get { return _currentTime; } }

    public override ITime GetInputTime(bool asStamp)
    {
      Time targetTime;
      if (asStamp)
        targetTime = new Time(_currentTime.StampAsModifiedJulianDay + _timeStepLengthInDays);
      else
        targetTime = new Time(_currentTime.StampAsModifiedJulianDay, _timeStepLengthInDays);
      return targetTime;
    }

    public override ITimeSpaceValueSet GetEngineValues(ExchangeItem exchangeItem)
    {
      if (exchangeItem is EngineInputItem)
      {
        // Input item, provide current internal values for this item
        // NOT supported by this computational core
        throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
      }

      ITimeSpaceValueSet values = GetOutputValuesFromComputationalCore(exchangeItem);
      return values;
    }

    private ITimeSpaceValueSet GetOutputValuesFromComputationalCore(ExchangeItem exchangeItem)
    {
      var separator = new[] { ':' };
      string[] elementSetCaptionSubStrings = exchangeItem.SpatialDefinition.Caption.Split(separator);

      List<double> values = new List<double>();

      // Output item, provide computed values
      if (exchangeItem.Id.Equals("WholeRiver:Flow"))
      {
        for (int i = 0; i < _flow.Length; i++)
        {
          values.Add(_flow[i]);
        }
      }
      else if (exchangeItem.Id.Equals("WholeRiver:Leakage"))
      {
        for (int i = 0; i < _leakage.Length; i++)
        {
          values.Add(_leakage[i]);
        }
      }
      else if (elementSetCaptionSubStrings[0] == "Branch")
      {
        int branchIndex = Convert.ToInt32(elementSetCaptionSubStrings[1]);
        if (exchangeItem.ValueDefinition.Caption.Equals("Flow"))
        {
          values.Add(_flow[branchIndex]);
        }
        else if (exchangeItem.ValueDefinition.Caption.Equals("Leakage"))
        {
          values.Add(_leakage[branchIndex]);
        }
        else
        {
          throw new Exception("Quantity Caption not recognized in GetValues method");
        }
      }
      else
      {
        throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
      }

      return new ValueSet(new List<IList> { values });
    }

    public override void SetEngineValues(EngineInputItem inputItem, ITimeSpaceValueSet values)
    {
      StoreInputValuesInComputationalCore(inputItem, values);
    }

    private void StoreInputValuesInComputationalCore(EngineInputItem inputItem, ITimeSpaceValueSet values)
    {
      char[] separator = new[] { ':' };
      string[] subStrings = inputItem.SpatialDefinition.Caption.Split(separator);

      if (inputItem.Id.Equals("WholeRiver.Flow"))
      {
        // values are numberOfNodes-1 long (input for each branch.
        // Put inflow at "upstream" node/storage
        for (int i = 0; i < _inflowStorage.Length - 1; i++)
        {
          _inflowStorage[i] += ((double)values.GetValue(0, i)) * _timeStepLengthInSeconds;
        }
      }
      else if (subStrings[0] == "Node")
      {
        int nodeIndex = Convert.ToInt32(subStrings[1]);
        _inflowStorage[nodeIndex] += ((double)values.GetValue(0, 0)) * _timeStepLengthInSeconds;
      }
      else
      {
        throw new ArgumentException("Unknown Input Item Id: \"" + inputItem.Id + "\"", "inputItem");
      }
    }

    protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
    {
      // Compute a "steady state" solution for this timestep. All water added to storage
      // are either leaked or flows out of the model

      // Let it rain/runoff equally on each node, store the amount of water in storage
      for (int i = 0; i < _numberOfNodes; i++)
      {
        _inflowStorage[i] += _runoff * _timeStepLengthInSeconds;
      }

      // From up-river and down (when ground water level is -10):
      // half of the storage flows to the next node (added to its storage)
      // and half of it leaks. All storages are emptied in the process.
      // Leakage decays as ground water levels increases. At -5 m the leakage
      // is zero. At 0 and above, the leakage is negative (inflow into river)
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        double leakFac = -_groundWaterLevel[i] * 0.1 - 0.5;
        leakFac = Math.Min(Math.Max(leakFac, -0.5), 0.5);
        double totalFlow = _inflowStorage[i] / _timeStepLengthInSeconds;
        _flow[i] = (1 - leakFac) * totalFlow;
        _leakage[i] = leakFac * totalFlow;
        _inflowStorage[i + 1] += _inflowStorage[i] - _leakage[i] * _timeStepLengthInSeconds;
      }

      // Calculate outflow from last node (for mass conservation checking)
      _lastNodeOutflow = _inflowStorage[_numberOfNodes - 1] / _timeStepLengthInSeconds;

      for (int i = 0; i < _numberOfNodes; i++)
      {
        _inflowStorage[i] = 0;
      }

      // Increase engine's current time
      // Update the exchange items time info

      _currentTime.AddSeconds(_timeStepLengthInSeconds);

      //foreach (EngineOutputItem engineOutputItem in Outputs)
      //{
      //    if (flowItemsAsSpan)
      //    {
      //        engineOutputItem.SetSingleTimeSpan(currentTime.StampAsModifiedJulianDay - timeStepLengthInDays,
      //                                           timeStepLengthInDays);
      //    }
      //    else
      //    {
      //        engineOutputItem.SetSingleTime(currentTime.StampAsModifiedJulianDay);
      //    }
      //    if (engineOutputItem.StoreValuesInExchangeItem)
      //    {
      //        engineOutputItem.Values = GetOutputValuesFromComputationalCore(engineOutputItem);
      //    }
      //}
    }

    public override bool DefaultForStoringValuesInExchangeItem
    {
      get { return _storeValuesInItem; }
    }
  }
}