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
using System.Text;
using System.IO;
using System.Diagnostics;

using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;

using Oatc.OpenMI.Wrappers.EngineWrapper2;
using OpenMI.Standard2.TimeSpace;
using LinkableComponentMW2 = Oatc.OpenMI.Wrappers.EngineWrapper2.LinkableComponent;

namespace Oatc.OpenMI.Examples.SimpleGroundModel2
{
  public class LinkableComponent : LinkableComponentMW2
  {
    Engine _gwEngine;

    enum OmiArguments
    {
      ModelID = 0,
      SimulationStart, SimulationEnd,
      NX, NY, OriginX, OriginY, CellSize,
      GroundWaterLevel, GridAngle,
      MonitorExchangeEvents
    };

    string _modelId = "Simple Ground Model";

    DateTime _simulationStart = new DateTime();
    DateTime _simulationEnd = new DateTime();

    GridInfo _gridInfo;

    double _groundWaterLevel = 0;
    bool _monitorExchangeEvents = false;

    public LinkableComponent()
      : base("SimpleGroundModel")
    {
      _describes.Description = "Oatc.SimpleGroundModel";

      _factories.Add(new Factory1());
      _factories.Add(new Factory2());

      _arguments.AddRange(new IArgument[] 
      {
                    new ArgumentString(OmiArguments.ModelID.ToString(), _modelId),
					          new ArgumentFileInfo("FileOut"),
                    new ArgumentDateTime(OmiArguments.SimulationStart.ToString(), _simulationStart),
                    new ArgumentDateTime(OmiArguments.SimulationEnd.ToString(), _simulationEnd),
                    new ArgumentInt(OmiArguments.NX.ToString(), 0),
                    new ArgumentInt(OmiArguments.NY.ToString(), 0),
                    new ArgumentDouble(OmiArguments.OriginX.ToString(), 0),
                    new ArgumentDouble(OmiArguments.OriginY.ToString(), 0),
                    new ArgumentDouble(OmiArguments.CellSize.ToString(), 0),
                    new ArgumentDouble(OmiArguments.GroundWaterLevel.ToString(), 0),
                    new ArgumentDouble(OmiArguments.GridAngle.ToString(), 0),
					          new ArgumentBool(OmiArguments.MonitorExchangeEvents.ToString(), _monitorExchangeEvents),
					});
    }

    #region Engine overides

    protected override ITime EngineInitialize(List<IArgument> arguments)
    {
      _modelId = (string)base.GetArgumentValueFirstCaptionMatch(
          OmiArguments.ModelID.ToString());

      _simulationStart = (DateTime)base.GetArgumentValueFirstCaptionMatch(
          OmiArguments.SimulationStart.ToString());
      _simulationEnd = (DateTime)base.GetArgumentValueFirstCaptionMatch(
          OmiArguments.SimulationEnd.ToString());

      _gridInfo = new GridInfo(
          (double)base.GetArgumentValueFirstCaptionMatch(OmiArguments.OriginX.ToString()),
          (double)base.GetArgumentValueFirstCaptionMatch(OmiArguments.OriginY.ToString()),
          (double)base.GetArgumentValueFirstCaptionMatch(OmiArguments.CellSize.ToString()),
          (int)base.GetArgumentValueFirstCaptionMatch(OmiArguments.NX.ToString()),
          (int)base.GetArgumentValueFirstCaptionMatch(OmiArguments.NY.ToString()),
          (double)base.GetArgumentValueFirstCaptionMatch(OmiArguments.GridAngle.ToString()));

      FileInfo fileOut = base.GetFirstCaptionMatchArgumentValueAsRootedFileInfo(
          "FileOut");

      _groundWaterLevel = (double)base.GetArgumentValueFirstCaptionMatch(OmiArguments.GroundWaterLevel.ToString());

      _monitorExchangeEvents = (bool)base.GetArgumentValueFirstCaptionMatch(
          OmiArguments.MonitorExchangeEvents.ToString());

      _gwEngine = new Engine(_simulationStart, _simulationEnd, _gridInfo, _groundWaterLevel, fileOut);

      IElementSet elementSet = new Grid(_gridInfo);

      _describes.Caption = _modelId;

      TimeSet timeExtent = new TimeSet();
      timeExtent.TimeHorizon = new Time(_simulationStart, _simulationEnd);

      TimeExtent = timeExtent;

      double[] zeros = new double[_gridInfo.NumberOfCells];
      double[] levels = new double[_gridInfo.NumberOfCells];

      for (int n = 0; n < _gridInfo.NumberOfCells; ++n)
      {
        zeros[n] = 0;
        levels[n] = 0;
      }

      double relaxation = 1; // for linear extrapolation, 1 = None
      double timeTolerance = 1.0 / (24.0 * 60.0 * 60.0 * 100.0); // 1/100th sec

      Targets.Add(new Inflow((Targets.Count + Sources.Count).ToString(),
              zeros,
              timeTolerance,
              new Grid(_gridInfo),
              this));

      Sources.Add(new AquiferLevel((Targets.Count + Sources.Count).ToString(),
              zeros,
              relaxation,
              new Grid(_gridInfo),
              this));

      Sources.Add(new AquiferStorage((Targets.Count + Sources.Count).ToString(),
              zeros,
              relaxation,
              new Grid(_gridInfo),
              this));

      return new Time(_gwEngine.CurrentTime);
    }

    protected override bool EngineComputationCompleted()
    {
      return _gwEngine.CurrentTime >= _gwEngine.SimulationEnd;
    }

    protected override void EngineUpdateFromTargets()
    {
      TimeSet timeSet = new TimeSet();
      timeSet.SetSingleTimeStamp(_gwEngine.CurrentTime);

      foreach (ITimeSpaceInput item in ActiveTargets)
      {
        if (item is ItemInBase)
          ((ItemInBase)item).Update(timeSet);
        else
          throw new InvalidCastException("InputItemBase");

        // TODO (ADH) Presumption is item contains 1 and only 1 
        // set of values, at the update time, when is this false?
        Debug.Assert(item.TimeSet.Times.Count == 1);

        double[] v = new double[ValueSet.GetElementCount(item.Values)];
        item.Values.GetElementValuesForTime(0).CopyTo(v, 0);

        if (item is Inflow)
          _gwEngine.SetInflows(v);
        else
          throw new NotImplementedException("EngineUpdateFromTargets");
      }
    }

    protected override ITime EngineCompute()
    {
      _gwEngine.DoTimeStep();
      return new Time(_gwEngine.CurrentTime);
    }

    protected override void EngineUpdateSources()
    {
      TimeSet timeSet = new TimeSet();
      timeSet.SetSingleTimeStamp(_gwEngine.CurrentTime);

      List<double> values = new List<double>();

      foreach (ITimeSpaceOutput item in ActiveSources)
      {
        values.Clear();

        if (item is AquiferLevel)
          values.AddRange(_gwEngine.GetAquiferLevels());
        else if (item is AquiferStorage)
          values.AddRange(_gwEngine.GetAquiferStorage());
        else
          throw new NotImplementedException();

        if (item is ItemOutBase)
        {
          ((ItemOutBase)item).Update(timeSet, values);

          foreach (ITimeSpaceAdaptedOutput decorator in item.AdaptedOutputs)
            decorator.Refresh();
        }
        else
          throw new InvalidCastException("OutputItemBase");
      }
    }

    protected override void EngineFinish()
    {
      _gwEngine.Finish();
    }

    #endregion Engine overides

    public class Factory1 : Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters.FactoryBase
    {
      public Factory1()
        : base("Factory1",
        new Describable(
        "Factory1",
        "SimpleGroundModel2 source adapters factory 1(same as ModelWrapper2 example)"))
      {
      }
    }

    public class Factory2 : Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters.FactoryBase
    {
      public Factory2()
        : base("Factory2",
        new Describable(
        "Factory2",
        "SimpleGroundModel2 source adapters factory 2(same as ModelWrapper2 example)"))
      {
      }
    }
  }
}
