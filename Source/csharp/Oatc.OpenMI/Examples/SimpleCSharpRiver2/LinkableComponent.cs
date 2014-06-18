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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;

using Oatc.OpenMI.Wrappers.EngineWrapper2;
using OpenMI.Standard2.TimeSpace;
using LinkableComponentMW2 = Oatc.OpenMI.Wrappers.EngineWrapper2.LinkableComponent;

namespace Oatc.OpenMI.Examples.SimpleCSharpRiver2
{
    public class LinkableComponent : LinkableComponentMW2
    {
        Engine _engine;
		bool _monitorExchangeEvents;
        bool _showGroundWaterLevelTargets;

		Dictionary<ITimeSpaceInput, int> _nodeIndexs
            = new Dictionary<ITimeSpaceInput, int>();
		Dictionary<ITimeSpaceOutput, int> _branchIndexs
            = new Dictionary<ITimeSpaceOutput, int>();

        public LinkableComponent()
			: base("RiverModelLinkableComponent")
		{
			_describes.Description = "Oatc.SimpleCSharpRiver";

            _factories.Add(new Factory1());
            _factories.Add(new Factory2());

			_arguments.AddRange(new IArgument[] {
					new ArgumentFileInfo("FileInNetwork", true),
					new ArgumentFileInfo("FileInBoundaryConditions", true),
					new ArgumentFileInfo("FileOut"),
					new ArgumentBool("MonitorExchangeEvents", false),
					new ArgumentBool("ShowGroundWaterLevelTargets", false),
					});
		}

		#region Engine overides

		protected override ITime EngineInitialize(List<IArgument> arguments)
		{
            Debug.Assert(true);
			_engine = new Engine();

			FileInfo fileInNetwork = base.GetFirstCaptionMatchArgumentValueAsRootedFileInfo(
				"FileInNetwork");

			FileInfo fileInBoundaryConditions = base.GetFirstCaptionMatchArgumentValueAsRootedFileInfo(
				"FileInBoundaryConditions");

			FileInfo fileOut = base.GetFirstCaptionMatchArgumentValueAsRootedFileInfo(
				"FileOut");

			_monitorExchangeEvents = (bool)base.GetArgumentValueFirstCaptionMatch(
                "MonitorExchangeEvents");
            _showGroundWaterLevelTargets = (bool)base.GetArgumentValueFirstCaptionMatch(
                "ShowGroundWaterLevelTargets");

			_engine.ReadInputFiles(fileInNetwork.FullName, fileInBoundaryConditions.FullName);
			_engine.init(fileOut.FullName);

			_describes.Caption = _engine.GetModelID();

			// Time horizon --
			DateTime startTime = _engine.GetSimulationStartTime();
			int numberOfTimeSteps = _engine.GetNumberOfTimeSteps();
			double timeStepLength = _engine.GetTimeStepLength();

			double duration = (numberOfTimeSteps * timeStepLength) / (3600 * 24.0);
			double start = Time.ToModifiedJulianDay(startTime);
            TimeSet timeExtent = new TimeSet();
            timeExtent.TimeHorizon = new Time(start, duration);
		    TimeExtent = timeExtent; 

			int nNodes = _engine.GetNumberOfNodes();
			int nBranches = nNodes - 1;

            Point2D[] points = new Point2D[nNodes];

            for (int n = 0; n < nNodes; ++n)
                points[n] = new Point2D(_engine.GetXCoordinate(n), _engine.GetYCoordiante(n));

            double initialValue = 0;
            double defaultValue = 0;
            double relaxation = 1; // for linear extrapolation, 1 = None
			double timeTolerance = 1.0 / (24.0*60.0*60.0*100.0); // 1/100th sec

            ItemOutBase source;
            ItemInBase target;
            
            for (int n = 0; n < nBranches; ++n)
			{
                source = new FlowAlongBranch(
                    string.Format("River branch [{0}]", n),
                    new List<Point2D[]> { new Point2D[2] { points[n], points[n + 1] } },
 					initialValue,
					relaxation,
					this);
    
				if (_monitorExchangeEvents)
					source.ItemChanged += new EventHandler<ExchangeItemChangeEventArgs>(ForwardExchangeEvent);
				
				_branchIndexs.Add(source, n);
				
                Sources.Add(source);
			}

			for (int n = 0; n < nNodes; ++n)
			{
                target = new InflowAtNode(
                    string.Format("Inflow location [{0}]", n),
                    points[n],
                    defaultValue,
					timeTolerance,
					this);

				if (_monitorExchangeEvents)
					target.ItemChanged += new EventHandler<ExchangeItemChangeEventArgs>(ForwardExchangeEvent);

				_nodeIndexs.Add(target, n);

				Targets.Add(target);

                if (_showGroundWaterLevelTargets)
                {
                    target = new GroundWaterLevelAtNode(
                        string.Format("Ground Water level, location [{0}]", n),
                        points[n],
                        defaultValue,
                        timeTolerance,
                        this);

                    if (_monitorExchangeEvents)
                        target.ItemChanged += new EventHandler<ExchangeItemChangeEventArgs>(ForwardExchangeEvent);

                    _nodeIndexs.Add(target, n);

                    Targets.Add(target);
                }
			}

            return new Time(_engine.GetCurrentTime());
		}

        private void ForwardExchangeEvent(object sender, ExchangeItemChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override bool EngineComputationCompleted()
		{
			return !(_engine.GetCurrentTimeStep() 
				< _engine.GetNumberOfTimeSteps());
		}

		protected override void EngineUpdateFromTargets()
		{
			TimeSet timeSet = new TimeSet();
			timeSet.SetSingleTimeStamp(_engine.GetCurrentTime());

			foreach (ITimeSpaceInput item in ActiveTargets)
			{
				if (item is ItemInBase)
					((ItemInBase)item).Update(timeSet);
				else
					throw new InvalidCastException("InputItemBase");

                // TODO (ADH) Presumption is item contains 1 and only 1 
                // set of values, at the update time, when is this false?
                Debug.Assert(item.TimeSet.Times.Count == 1);
                Debug.Assert(ValueSet.GetElementCount(item.Values) == 1);

                if (item is InflowAtNode)
                    _engine.SetExternalNodeInflow(
                        _nodeIndexs[item],
                        (double)item.Values.GetElementValuesForTime(0)[0]);
                else if (item is GroundWaterLevelAtNode)
                    _engine.SetGroundWaterLevel(
                        _nodeIndexs[item],
                        (double)item.Values.GetElementValuesForTime(0)[0]);
                else
                    throw new NotImplementedException("EngineUpdateFromTargets");
            }
		}

		protected override ITime EngineCompute()
		{
			_engine.MakeATimeStep();
            return new Time(_engine.GetCurrentTime());
        }

		protected override void EngineUpdateSources()
		{
			TimeSet timeSet = new TimeSet();
			timeSet.SetSingleTimeStamp(_engine.GetCurrentTime());

			List<double> values = new List<double>();

			foreach (ITimeSpaceOutput item in ActiveSources)
			{
				values.Clear();
				values.Add(_engine.GetFlow(_branchIndexs[item]));

				if (item is ItemOutBase)
				{
                    ((ItemOutBase)item).Update(timeSet, values);

                    // Update dependent decorators
                    foreach (ITimeSpaceAdaptedOutput decorator in item.AdaptedOutputs)
                    {
                        decorator.Refresh();
                    }
                }
				else
					throw new InvalidCastException("OutputItemBase");
			}
		}

		protected override void EngineFinish()
		{
			_engine.Finish();
		}

		#endregion Engine overides

        public class Factory1 : Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters.FactoryBase
		{
            public Factory1()
                : base("Factory1",
				new Describable(
                "Factory1",
				"SimpleCSharpRiver2 source adapters factory 1(same as ModelWrapper2 example)"))
			{
			}
		}

        public class Factory2 : Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters.FactoryBase
        {
            public Factory2()
                : base("Factory2",
                new Describable(
                "Factory2",
                "SimpleCSharpRiver2 source adapters factory 2(same as ModelWrapper2 example)"))
            {
            }
        }
    }
}
