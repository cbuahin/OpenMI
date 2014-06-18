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
// file: GWModelLC.cs
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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.GroundWaterModel
{
  /// <summary>
  /// A simple ground water model, which is basically just a number of bath-tubs
  /// in a regular grid. There is no interaction between neighbouring grid cells.
  /// <para>
  /// Each cell has a storage (volume of water), and the ground water level
  /// is calculated based on the storage: It is assuming the ground to absorb 10% of 
  /// water, i.e. 0.1 m^3 of storage water in a 1 m^2 cell results in
  /// a ground water level 1 meters above the ground water base level.
  /// </para>
  /// <para>
  /// Ouputs:
  /// <list type="bullet">
  /// <item>The storage</item>
  /// <item>The ground water level, based on the storage and the ground water base level</item>
  /// </list>
  /// </para>
  /// <para>
  /// Inputs:
  /// <list type="bullet">
  /// <item>The storage can be set directly (usefull for calibration scenarios)</item>
  /// <item>Discharge into/out of the model, can be negative</item>
  /// </list>
  /// </para>
  /// <para>
  /// The common scenario is to extract the ground water level from the ground water model,
  /// and based on the ground water level to calculate a discharge into the ground water model.
  /// </para>
  /// </summary>
  public sealed class GWModelLC : LinkableGetSetEngine
  {
    private Time _simulationEnd;
    private Time _simulationStart;
    private Time _currentTime;
    private double _timeStepLengthInSeconds; // [seconds]

    /// <summary>
    /// [liters] of water in each grid cell
    /// </summary>
    private double[] _storage;

    /// <summary>
    /// Origin of GW model grid
    /// </summary>
    private double _x0 = 2.0;
    /// <summary>
    /// Origin of GW model grid
    /// </summary>
    private double _y0 = 2.0;
    /// <summary>
    /// Spacing of GW model grid cells
    /// </summary>
    private double _dx = 4.0;
    /// <summary>
    /// Spacing of GW model grid cells
    /// </summary>
    private double _dy = 4.0;
    /// <summary>
    /// Number of cells in x direction
    /// </summary>
    private int _xCount = 2;
    /// <summary>
    /// Number of cells in y direction
    /// </summary>
    private int _yCount = 2;
    /// <summary>
    /// Orientation of grid, compared to grid north (compass heading, counter clockwise)
    /// </summary>
    private double _orientation = 0;

    /// <summary>
    /// Base level is the bottom of the ground water cells, assuming 
    /// nothing happens below this level.
    /// </summary>
    private double _gwBaseLevel = -10;

    private EngineInputItem _storageInput;
    private EngineInputItem _firstElementStorageInput;
    EngineOutputItem _storageOutput;
    EngineOutputItem _firstElementStorageOutput;
    private bool _useMultiInput;

    public int NumberOfElements { get { return (_xCount * _yCount); } }

    public GWModelLC()
    {
      Id = "GWModelLC";
      Caption = "Test GroundWater Model";

      _simulationStart = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
      _simulationEnd = new Time(new DateTime(2005, 2, 10, 0, 0, 0));
      _currentTime = new Time(_simulationStart);
      _timeStepLengthInSeconds = 3600 * 24; //one day

      List<IArgument> arguments = new List<IArgument>();

      arguments.Add(new ArgumentInt("XCount", _xCount));
      arguments.Add(new ArgumentInt("YCount", _yCount));
      arguments.Add(new ArgumentDouble("x0", _x0));
      arguments.Add(new ArgumentDouble("y0", _y0));
      arguments.Add(new ArgumentDouble("dx", _dx));
      arguments.Add(new ArgumentDouble("dy", _dy));
      arguments.Add(new ArgumentDouble("Orientation", _orientation));
      arguments.Add(new ArgumentDouble("BaseLevel", _gwBaseLevel));
      arguments.Add(new ArgumentDateTime("StartTime", _simulationStart.AsDateTime));
      arguments.Add(new ArgumentDateTime("EndTime", _simulationEnd.AsDateTime));
      arguments.Add(new ArgumentBool("UseMultiInput", false));
      arguments.TrimExcess();

      Arguments = arguments.AsReadOnly();

    }

    public override void Initialize()
    {

      Status = LinkableComponentStatus.Initializing;

      ReadArguments();

      // Initialize storage vector
      _storage = new double[NumberOfElements];

      for (int i = 0; i < NumberOfElements; i++)
      {
        _storage[i] = 0;
      }

      // -- Populate Exchange Items ---

      // Element set for a grid based (polygons) item
      Spatial2DRegularGrid regularGrid = new Spatial2DRegularGrid()
                                           {
                                             Description = "RegularGrid",
                                             Caption = "RegularGrid",
                                             Dx = _dx,
                                             Dy = _dy,
                                             X0 = _x0,
                                             Y0 = _y0,
                                             XCount = _xCount,
                                             YCount = _yCount,
                                             Orientation = _orientation,
                                             IsNodeBased = false,
                                           };
      Spatial2DGridWrapper regularElmtSet = new Spatial2DGridWrapper(regularGrid);

      // Element set for a Polygon based item, of the lower left cell in the grid
      Element element0 = new Element("element:0");
      element0.AddVertex(new Coordinate(_x0, _y0, 0));
      element0.AddVertex(new Coordinate(_x0 + _dx, _y0, 0));
      element0.AddVertex(new Coordinate(_x0 + _dx, _y0 + _dy, 0));
      element0.AddVertex(new Coordinate(_x0, _y0 + _dy, 0));

      // Element set for an ID based item, of the lower left cell in the grid
      ElementSet idSet =
          new ElementSet("FirstElement", "FirstElement", ElementType.IdBased);
      idSet.AddElement(element0);   // is an IdBased set required to have elements?

      // Dimensions
      Dimension dimVolume = new Dimension();
      dimVolume.SetPower(DimensionBase.Length, 3);

      Dimension dimLength = new Dimension();
      dimLength.SetPower(DimensionBase.Length, 1);
      
      Dimension dimDischarge = new Dimension();
      dimDischarge.SetPower(DimensionBase.Length, 3);
      dimDischarge.SetPower(DimensionBase.Time, -1);

      // Units
      Unit unitLiterStorage = new Unit("Storage", 0.001, 0.0, "Storage");
      unitLiterStorage.Dimension = dimVolume;

      Unit unitGwLevel = new Unit("gw level", 1.0, 0.0, "Ground water level");
      unitGwLevel.Dimension = dimLength;

      Unit unitDischarge = new Unit("Discharge", 0.001, 0, "Discharge into ground water model, [L/s]");
      unitDischarge.Dimension = dimDischarge;

      // Quantities
      Quantity quantityStorage = new Quantity(unitLiterStorage, "Storage", "Storage");

      Quantity quantityGwLevel = new Quantity(unitGwLevel, "Ground water level", "Ground water level");

      Quantity quantityInflow = new Quantity(unitDischarge, "Inflow into ground water model", "Inflow");

      
      // Storage input on the grid
      _storageInput = new EngineEInputItem("Grid.Storage", quantityStorage, regularElmtSet, this);
      _firstElementStorageInput = new EngineEInputItem("FirstElement.Storage", quantityStorage, idSet, this);

      // Storage output on the grid
      _storageOutput = new EngineEOutputItem("Grid.Storage", quantityStorage, regularElmtSet, this);
      _storageOutput.TimeSet.SetSingleTime(StartTime);
      _storageOutput.TimeSet.SetTimeHorizon(new Time(StartTime, EndTime));

      // Storage output in the first element of the grid
      _firstElementStorageOutput = new EngineEOutputItem("FirstElement.Storage", quantityStorage, idSet, this);
      _firstElementStorageOutput.TimeSet.SetSingleTime(StartTime);
      _firstElementStorageOutput.TimeSet.SetTimeHorizon(new Time(StartTime, EndTime));


      // Ground water level output. Calculated as (_gwBaseLevel + 0.1*storage height), assuming that the 
      // water can populate 10% of the ground volume.
      EngineDOutputItem gwLevelOutput = new EngineDOutputItem("Grid.gwLevel", quantityGwLevel, regularElmtSet, this);
      gwLevelOutput.StoreValuesInExchangeItem = false;
      gwLevelOutput.ValueGetter = delegate()
                                 {
                                   IList<double> res = new List<double>(NumberOfElements);
                                   for (int i = 0; i < NumberOfElements; i++)
                                   {
                                     // Convert storage from liters to m3 and find the height.
                                     double storageHeight = 0.001 * _storage[i] / (_dx * _dy);
                                     res.Add(_gwBaseLevel + 10 * storageHeight);
                                   }
                                   return new ValueSet(new List<IList> { (IList)res });
                                 };

      // Ground water inflow in the grid
      EngineDInputItem gwInflow = new EngineDInputItem("Grid.Inflow", quantityInflow, regularElmtSet, this);
      gwInflow.ValueSetter = delegate(ITimeSpaceValueSet values)
                               {
                                 IList elmtValues = values.GetElementValuesForTime(0);
                                 for (int i = 0; i < NumberOfElements; i++)
                                 {
                                   // Values arrive in [L/s], storage is in liters.
                                   _storage[i] += (double)elmtValues[i] * _timeStepLengthInSeconds;
                                   if (_storage[i] < 0) // inflow can be negative, but storage can not be zero. Mass error here :-(
                                     _storage[i] = 0;
                                 }
                               };

      EngineOutputItems.Add(_storageOutput);
      EngineOutputItems.Add(_firstElementStorageOutput);
      EngineOutputItems.Add(gwLevelOutput);

      EngineInputItems.Add(_storageInput);
      EngineInputItems.Add(_firstElementStorageInput);
      if (_useMultiInput)
        EngineInputItems.Add(new EngineMultiInputItemWrapper(gwInflow, this));
      else
        EngineInputItems.Add(gwInflow);
      
      Status = LinkableComponentStatus.Initialized;
    }

    /// <summary>
    /// Read arguments and setup the model
    /// </summary>
    private void ReadArguments()
    {
      
      // Dictionary with string key (ignoring case)
      IDictionary<string, IArgument> argDict = Arguments.Dictionary();

      _xCount = argDict.GetValue<int>("XCount");
      _yCount = argDict.GetValue<int>("YCount");
      _x0 = argDict.GetValue<double>("x0");
      _y0 = argDict.GetValue<double>("y0");
      _dx = argDict.GetValue<double>("dx");
      _dy = argDict.GetValue<double>("dy");
      _orientation = argDict.GetValue<double>("Orientation");
      _gwBaseLevel = argDict.GetValue<double>("BaseLevel");
      _simulationStart = new Time(argDict.GetValue<DateTime>("StartTime"));
      _simulationEnd = new Time(argDict.GetValue<DateTime>("EndTime"));
      _useMultiInput = argDict.GetValue<bool>("UseMultiInput");
    }

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
      double timeStepLengthInDays = _timeStepLengthInSeconds / (60 * 60 * 24);
      return (new Time(_currentTime.StampAsModifiedJulianDay - timeStepLengthInDays, timeStepLengthInDays));
    }

    public override ITime CurrentTime
    { get { return _currentTime; } }

    public override ITime GetInputTime(bool asStamp)
    {
      if (!asStamp)
        throw new NotSupportedException();
      Time targetTime = new Time(_currentTime);
      targetTime.AddSeconds(_timeStepLengthInSeconds);
      return targetTime;
    }

    public override ITimeSpaceValueSet GetEngineValues(ExchangeItem exchangeItem)
    {
      List<double> values = new List<double>();
      if (exchangeItem is EngineInputItem)
      {
        // Input item, provide current input values
        if (exchangeItem == _storageInput)
        {
          for (int i = 0; i < _storage.Length; i++)
          {
            values.Add(_storage[i]);
          }
        }
        else if (exchangeItem == _firstElementStorageInput)
        {
          values.Add(_storage[0]);
        }
        else
        {
          throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
        }
      }
      else if (exchangeItem is EngineOutputItem)
      {
        // Output item, provide computed values
        if (exchangeItem == _storageOutput)
        {
          for (int i = 0; i < _storage.Length; i++)
          {
            values.Add(_storage[i]);
          }
        }
        else if (exchangeItem == _firstElementStorageOutput)
        {
          values.Add(_storage[0]);
        }
        else
        {
          throw new ArgumentException("Unknown Exchange Item Id: \"" + exchangeItem.Id + "\"", "exchangeItem");
        }
      }
      else
      {
        throw new Exception("Should be EngineInputItem or EngineOutputItem");
      }
      return new ValueSet(new List<IList> { values });
    }

    public override void SetEngineValues(EngineInputItem inputItem, ITimeSpaceValueSet values)
    {
      if (inputItem == _storageInput)
      {
        IList elementValues = values.GetElementValuesForTime(0);
        for (int i = 0; i < _storage.Length; i++)
        {
          _storage[i] = (double)elementValues[i];
        }
      }
      else if (inputItem == _firstElementStorageInput)
      {
        _storage[0] = (double)values.GetValue(0, 0);
      }
      else
      {
        throw new ArgumentException("Unknown Input Item Id: \"" + inputItem.Id + "\"", "inputItem");
      }
    }

    protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
    {
      // Increase engine's current time, update the exchange items accordingly
      _currentTime.AddSeconds(_timeStepLengthInSeconds);
      this.TimeExtent.Times[0] = _currentTime;
      foreach (EngineOutputItem engineOutputItem in requiredOutputItems)
      {
          engineOutputItem.TimeSet.SetSingleTime(_currentTime);
      }
    }

    public override bool DefaultForStoringValuesInExchangeItem
    {
      get { return false; }
    }
  }
}