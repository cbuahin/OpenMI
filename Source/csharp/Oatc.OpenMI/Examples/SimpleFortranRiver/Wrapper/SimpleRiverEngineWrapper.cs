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
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper 
// purpose: Example-implementation river model wrapper
// file: SimpleRiverEngineWrapper.cs
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
using Oatc.OpenMI.Sdk.Backbone.Generic;
using Oatc.OpenMI.Wrappers.EngineWrapper;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;


namespace Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper
{
    /// <summary>
    /// SimpleRiverEngineWrapper wraps the <see cref="SimpleRiverEngineDotNetAccess"/>
    /// in an <see cref="LinkableGetSetEngine"/>.
    /// </summary>
    public class SimpleRiverEngineWrapper : LinkableGetSetEngine
    {
        private SimpleRiverEngineDotNetAccess _simpleRiverEngine;

        double _simulationStartTime; //simulation start time in Modified Julian Time

        public SimpleRiverEngineWrapper()
        {
            _simpleRiverEngine = new SimpleRiverEngineDotNetAccess();
            Arguments = new List<IArgument>();
            Arguments.Add(new ArgumentString("ModelId", null));
            Arguments.Add(new ArgumentString("FilePath"));
            Arguments.Add(new ArgumentString("SimFileName"));
            Arguments.Add(new ArgumentBool("FlowItemsAsSpan", false));
            Arguments.Add(new ArgumentDouble("TimeStepLength", -1));
            Arguments.Add(new ArgumentBool("StoreValuesInExchangeItems", false));
            Description = "Simple River is an example component";
        }

        protected override ITime StartTime
        {
            get { return (new Time(_simulationStartTime)); }
        }

        protected override ITime EndTime
        {
            get
            {
                int numberOfTimeSteps = _simpleRiverEngine.GetNumberOfTimeSteps();
                double timestepInDays = _simpleRiverEngine.GetTimeStepLength()/(60*60*24);
                return (new Time(_simulationStartTime + numberOfTimeSteps * timestepInDays));
            }
        }

        public ITime GetTimeHorizon()
        {
            double timeStepLengthInDays = _simpleRiverEngine.GetTimeStepLength()/(60 * 60 * 24);
            int numberOfTimeSteps = _simpleRiverEngine.GetNumberOfTimeSteps();
            double simulationEndTime = _simulationStartTime + (numberOfTimeSteps * timeStepLengthInDays);
            return new Time(new Time(_simulationStartTime), new Time(simulationEndTime));
        }

        public override ITime CurrentTime
        {
            get { return (GetCurrentTime(true)); }
        }

        public ITime GetCurrentTime()
        {
            double time = _simulationStartTime + _simpleRiverEngine.GetCurrentTime() / ((double)(24 * 3600));
            return new Time(time);
        }

        public override ITime GetCurrentTime(bool asStamp)
        {
            if (asStamp)
                return (new Time(GetCurrentTime()));
            double timeStepInDays = _simpleRiverEngine.GetTimeStepLength() / (60 * 60 * 24);
            return (new Time(GetCurrentTime().StampAsModifiedJulianDay - timeStepInDays, timeStepInDays));
        }

        public override ITime GetInputTime(bool asStamp)
        {
            if (asStamp)
                return new Time(_simulationStartTime+_simpleRiverEngine.GetInputTime()/(60*60*24));
            double timeStepInDays = _simpleRiverEngine.GetTimeStepLength()/(60*60*24);
            return (new Time(_simulationStartTime + _simpleRiverEngine.GetInputTime()/(60*60*24) - timeStepInDays, timeStepInDays));
        }

        public override void Initialize()
        {
            var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (IArgument argument in Arguments)
            {
                properties.Add(argument.Id, argument.Value);
            }
            string simFileName = "SimpleRiver.sim";
            object value;
            if (properties.TryGetValue("SimFileName", out value) && value != null && value.ToString().Length > 1)
            {
                simFileName = value.ToString();
            }
            // -- Create and initialize the engine --
            if (properties.TryGetValue("FilePath", out value) && value != null && value.ToString().Length > 1)
            {
                _simpleRiverEngine.Initialize(value.ToString(), simFileName);
            }
            else
            {
                string currentDir = System.IO.Directory.GetCurrentDirectory();
                _simpleRiverEngine.Initialize(currentDir, simFileName);
                //When running from the GUI, the assembly is started with the current dir
                //to the location of the OMI files. It is assumed that the data files are
                //located in the same directory.
            }

            if (properties.TryGetValue("TimeStepLength", out value) && value != null && ((double)value) > 0.0)
            {
                _simpleRiverEngine.SetTimeStepLength(Convert.ToDouble(value));
            }

            if (properties.TryGetValue("ModelId", out value) && value != null)
            {
                // Note: This does strictly not set the id to the fortran engine (no setter available)
                Id = value.ToString();
            }
            else
            {
                Id = _simpleRiverEngine.GetModelID();
            }

            Description = _simpleRiverEngine.GetModelDescription();

            // -- Time horizon --
            char[] delimiter = new char[] { '-', ' ', ':' };
            string[] strings = _simpleRiverEngine.GetSimulationStartDate().Split(delimiter);
            int StartYear = Convert.ToInt32(strings[0]);
            int StartMonth = Convert.ToInt32(strings[1]);
            int StartDay = Convert.ToInt32(strings[2]);
            int StartHour = Convert.ToInt32(strings[3]);
            int StartMinute = Convert.ToInt32(strings[4]);
            int StartSecond = Convert.ToInt32(strings[5]);
            DateTime startDate = new DateTime(StartYear, StartMonth, StartDay, StartHour, StartMinute, StartSecond);
            _simulationStartTime = new Time(startDate).StampAsModifiedJulianDay;


            // -- Build exchange items ---
            Dimension flowDimension = new Dimension();
            flowDimension.SetPower(DimensionBase.Length, 3);
            flowDimension.SetPower(DimensionBase.Time, -1);

            Unit flowUnit = new Unit("m3/sec", 1, 0, "m3/sec");
            flowUnit.Dimension = flowDimension;

            Quantity flowQuantity = new Quantity(flowUnit, "Flow description", "Flow");
            Quantity inFlowQuantity = new Quantity(flowUnit, "Inflow description", "InFlow");

            // ADH: Might not be unreasonable to set double as default in backbone constructor
            flowQuantity.ValueType = typeof(double);
            inFlowQuantity.ValueType = typeof(double);

            int numberOfNodes = _simpleRiverEngine.GetNumberOfNodes();

            TimeSet timeset = new TimeSet();
            timeset.TimeHorizon = GetTimeHorizon();
            
            TimeSet extentTimeSet = new TimeSet();
            extentTimeSet.TimeHorizon = GetTimeHorizon();
            extentTimeSet.Times.Add(GetTimeHorizon());
            _timeExtent = extentTimeSet;

            int nCount = 0;

            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                ElementSet branch = new ElementSet("description", "Branch:" + i, ElementType.PolyLine, "");
                branch.AddElement(new Element("Branch:" + i.ToString()));
                branch.Elements[0].AddVertex(new Coordinate(_simpleRiverEngine.GetXCoordinate(i), _simpleRiverEngine.GetYCoordinate(i), 0));
                branch.Elements[0].AddVertex(new Coordinate(_simpleRiverEngine.GetXCoordinate(i + 1), _simpleRiverEngine.GetYCoordinate(i + 1), 0));

                EngineEOutputItem flowFromBrach = new EngineEOutputItem("Branch:" + i + ":Flow", flowQuantity, branch, this);

                int branchIndex = i;
                EngineDInputItem inflowToBranch = new EngineDInputItem("Branch:" + i + ":InFlow", inFlowQuantity, branch, this);
                inflowToBranch.ValueSetter = delegate(ITimeSpaceValueSet values)
                                                 {
                                                     _simpleRiverEngine.AddInflow(branchIndex, (double)values.GetValue(0,0));
                                                 };

                flowFromBrach.TimeSet = timeset;

                EngineOutputItems.Add(flowFromBrach);
                EngineInputItems.Add(inflowToBranch);
            }

            for (int i = 0; i < numberOfNodes; i++)
            {
                ElementSet node = new ElementSet("description", "Node:" + i.ToString(), ElementType.IdBased, "");
                node.AddElement(new Element("Node:" + i.ToString()));

                int nodeIndex = i;
                EngineDInputItem inflowToNode = new EngineDInputItem(node.Caption+":InFlow", inFlowQuantity, node, this);
                inflowToNode.ValueSetter = delegate(ITimeSpaceValueSet values)
                                               {
                                                   _simpleRiverEngine.AddInflow(nodeIndex, (double)values.GetValue(0,0));
                                               };
                Inputs.Add(inflowToNode);
            }

            ElementSet branches = new ElementSet("description", "AllBranches", ElementType.PolyLine, "");
            
            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                Element branch = new Element("Branch: " + i.ToString());
                branch.AddVertex(new Coordinate(_simpleRiverEngine.GetXCoordinate(i), _simpleRiverEngine.GetYCoordinate(i), 0));
                branch.AddVertex(new Coordinate(_simpleRiverEngine.GetXCoordinate(i + 1), _simpleRiverEngine.GetYCoordinate(i + 1), 0));
                branches.AddElement(branch);
            }

            EngineEInputItem inFlowToBraches = new EngineEInputItem(branches.Caption+":inFlow", inFlowQuantity, branches, this);
           
            Inputs.Add(inFlowToBraches);

            Status = LinkableComponentStatus.Initialized;

        }

        protected override void OnPrepare()
        {
        }

        protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
        {
            _simpleRiverEngine.PerformTimeStep();
        }

        public void PerformTimestep()
        {
            _simpleRiverEngine.PerformTimeStep();
        }

        public override void Finish()
        {
            _simpleRiverEngine.Finish();
        }

        public override ITimeSpaceValueSet GetEngineValues(ExchangeItem exchangeItem)
        {
            // Handle the flow output item, quantity.Caption="Flow", elementSet.Caption="Branch:[index]:Flow"
            IQuantity quantity = exchangeItem.ValueDefinition as IQuantity;
            if (quantity == null)
                throw new ArgumentException("Can only accept quantity as valuedefinition", "exchangeItem");

            double[] values;
            Char[] separator = new char[] { ':' };

            if (quantity.Caption == "Flow")
            {
                int index = Convert.ToInt32((exchangeItem.SpatialDefinition.Caption.Split(separator))[1]);
                values = new double[1];
                values[0] = _simpleRiverEngine.GetFlow(index);
            }
            else
            {
                throw new ArgumentException("Unknown Quantity/elementSet combination in GetValues", "exchangeItem");
            }

            return new TimeSpaceValueSet<double>(values);
        }

        public override void SetEngineValues(EngineInputItem inputItem, ITimeSpaceValueSet values)
        {
            IQuantity quantity = inputItem.ValueDefinition as IQuantity;
            if (quantity == null)
                throw new ArgumentException("Can only accept quantity as valuedefinition", "inputItem");
            if (quantity.Caption == "InFlow" && inputItem.SpatialDefinition.Caption == "AllBranches")
            {
                for (int i = 0; i < _simpleRiverEngine.GetNumberOfNodes(); i++)
                {
                    _simpleRiverEngine.AddInflow(i, (double)values.GetValue(0, i));
                }
            }
            else
            {
                throw new ArgumentException("Unknown quantity/elementSet combination", "inputItem");
            }
        }

        protected override string[] OnValidate()
        {
            // TODO: Put code here!
            return new string[0];
        }

        public override bool DefaultForStoringValuesInExchangeItem
        {
            get { return true; }
        }

        
    }
}