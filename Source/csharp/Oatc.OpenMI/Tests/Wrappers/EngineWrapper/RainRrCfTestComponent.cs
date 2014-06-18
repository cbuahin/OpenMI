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
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    public class RainRrCfTestComponent : LinkableEngine
    {
        #region Private fields

        private readonly IDictionary<string, IElementSet> _elementSets = new Dictionary<string, IElementSet>();
        private readonly IDictionary<string, IQuantity> _quantities = new Dictionary<string, IQuantity>();
        private readonly IDictionary<string, IUnit> _units = new Dictionary<string, IUnit>();

        private double _currentTime = Double.NaN;
        private double _deltaT = 1/4d; // default 6 hour

        private double _endTimeAsMJD = RainRrCfMeasurementsDatabase.EndTimeAsMJD;
        private double _startTimeAsMJD = RainRrCfMeasurementsDatabase.StartTimeAsMJD;

        private TimeSet _timeSetForInputExchangeItems;
        private TimeSet _timeSetForOutputExchangeItems;

        #endregion

        #region Implementation of abstract functions in Backbone.LinkableComponent

        public override void Initialize()
        {
            Status = LinkableComponentStatus.Initializing;

            foreach (IArgument argument in Arguments)
            {
                if (argument.Caption.ToLower().Equals("id"))
                {
                    Id = (string) argument.Value;
                }
                else if (argument.Caption.ToLower().Equals("timehorizonoffset"))
                {
                    double timeHorizonOffset = Double.Parse((string) argument.Value);
                    _startTimeAsMJD += timeHorizonOffset;
                    _endTimeAsMJD += timeHorizonOffset;
                }
                else if (argument.Caption.ToLower().Equals("deltaT"))
                {
                    _deltaT = Double.Parse((string) argument.Value);
                }
                else if (argument.Caption.ToLower().Equals("quantity"))
                {
                    AddQuantity((string) argument.Value);
                }
                else if (argument.Caption.ToLower().Equals("elementset"))
                {
                    if (argument.Value.Equals("catchment-polygons"))
                    {
                        IDictionary<string, IElementSet> spatialElmSets =
                            RainRrCfElementSets.CreateRainfallRunoffElementSets();
                        foreach (var spatialElmSet in spatialElmSets)
                        {
                            _elementSets.Add(spatialElmSet.Key, spatialElmSet.Value);
                        }
                    }
                    else
                    {
                        AddElementset((string) argument.Value);
                    }
                }
                else if (argument.Caption.ToLower().EndsWith("putitem"))
                {
                    AddExchangeItem(argument.Caption.ToLower(), (string) argument.Value);
                }
                else
                {
                    throw new Exception("Unknown key \"" + argument.Caption + "\" in component " + Id);
                }
            }

            _currentTime = _startTimeAsMJD;

            _timeSetForOutputExchangeItems = new TimeSet();
            _timeSetForInputExchangeItems = new TimeSet();

            _timeSetForInputExchangeItems.TimeHorizon =
                _timeSetForOutputExchangeItems.TimeHorizon =
                new Time(_startTimeAsMJD, _endTimeAsMJD - _startTimeAsMJD);

            _adaptedOutputFactories = new List<IAdaptedOutputFactory>();
            _adaptedOutputFactories.Add(new AdaptedOutputFactory("AdaptedOutputFactory for " + Id));

            foreach (EngineInputItem inputItem in EngineInputItems)
            {
                inputItem.TimeSet = _timeSetForInputExchangeItems;
            }
            foreach (EngineOutputItem outputItem in EngineOutputItems)
            {
                outputItem.TimeSet = _timeSetForOutputExchangeItems;
            }

            Status = LinkableComponentStatus.Initialized;
        }

        public override void Prepare()
        {
            // TODO: Move code to here?!
        }

        protected override string[] OnValidate()
        {
            // This test component is always valid, no warnings
            return new string[0];
        }

        protected override void OnPrepare()
        {
            // Produce initial output
            ProduceInitialOutput();
            UpdateAdaptedOutputs(EngineOutputItems);
        }

        public override void Finish()
        {
            Status = LinkableComponentStatus.Finishing;
            // no action
            Status = LinkableComponentStatus.Done;
        }

        #endregion

        #region Implementation of abstract functions in ModelWrapper.LinkableEngine

        protected override ITime StartTime
        {
            get { return new Time(RainRrCfMeasurementsDatabase.StartTimeAsMJD); }
        }

        protected override ITime EndTime
        {
            get { return new Time(RainRrCfMeasurementsDatabase.EndTimeAsMJD); }
        }

        public override ITime CurrentTime
        {
            get { return new Time(_currentTime); }
        }

        public override bool DefaultForStoringValuesInExchangeItem
        {
            get { return true; }
        }

        public override ITime GetCurrentTime(bool asStamp)
        {
            if (asStamp)
                return new Time(_currentTime);
            return (new Time(_currentTime - _deltaT, _deltaT));
        }

        public override ITime GetInputTime(bool asStamp)
        {
            return new Time(_currentTime + _deltaT);
        }

        protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
        {
            // No real computation needed in this dummy model, simply produce output
            _currentTime = _currentTime + _deltaT;
            ProduceOutput(requiredOutputItems);
        }

        protected void LogIncomingValues(EngineInputItem inputItem, IList providedValues)
        {
            string message = Caption + " " + inputItem.Caption + " values:";
            foreach (double value in providedValues)
            {
                message += " " + value;
            }
            message += " <= " + inputItem.Provider.Caption + " from " + inputItem.Provider.Component.Caption +
                       ")";
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        #endregion

        #region Private functions for computation

        private void ProduceInitialOutput()
        {
            int producedOutputCount = 0;
            foreach (EngineOutputItem engineOutputItem in EngineOutputItems)
            {
                producedOutputCount++;
                double value = _currentTime - _startTimeAsMJD + 1e3*producedOutputCount;
                engineOutputItem.Values = new ValueSet(new List<IList> {new List<double> {value}});
                engineOutputItem.TimeSet.SetSingleTimeStamp(_currentTime);
            }
        }

        private void ProduceOutput(IEnumerable<EngineOutputItem> requiredOutputItems)
        {
            foreach (EngineOutputItem engineOutputItem in requiredOutputItems)
            {
                if (engineOutputItem.Values == null)
                {
                    TimeSpaceValueSet<double> defaultValueSet = new TimeSpaceValueSet<double>();
                    defaultValueSet.Values2D.Add(new List<double>{0});
                    engineOutputItem.Values = defaultValueSet;
                }
                double value = (double) engineOutputItem.Values.Values2D[0][0];
                value += _deltaT;
                engineOutputItem.Values.Values2D[0][0] = value;
                engineOutputItem.TimeSet.SetSingleTimeStamp(_currentTime);
            }
        }

        #endregion

        #region Private functions to create input and output items

        private void AddQuantity(string specifationString)
        {
            string[] specs = specifationString.Split(';');
            if (specs.Length != 2)
            {
                throw new Exception("No \";\" specified in \"" + specifationString + "\"" + ", component " + Id);
            }
            string quantId = specs[0];
            string unitId = specs[1];

            if (_quantities.ContainsKey(quantId))
            {
                throw new Exception("Quantity \"" + quantId + "\" already exists" + ", component " + Id);
            }

            IUnit unit;
            if (_units.ContainsKey(unitId))
            {
                unit = _units[unitId];
            }
            else
            {
                if (unitId.Equals("m"))
                {
                    unit = new Unit(PredefinedUnits.CubicMeterPerSecond);
                }
                else if (unitId.Equals("m3/s"))
                {
                    unit = new Unit(PredefinedUnits.CubicMeterPerSecond);
                }
                else if (unitId.Equals("mm/day"))
                {
                    unit = new Unit(PredefinedUnits.MillimeterPerDay);
                }
                else
                {
                    throw new Exception("Unknown unit: \"" + unitId + "\"" + ", component " + Id);
                }
                _units.Add(unitId, unit);
            }

            var quantity = new Quantity(unit, quantId, quantId);
            _quantities.Add(quantId, quantity);
        }

        private void AddElementset(string elementSetId)
        {
            if (_elementSets.ContainsKey(elementSetId))
            {
                throw new Exception("ElementSet \"" + elementSetId + "\" already exists");
            }

            var elementSet = new ElementSet(elementSetId, elementSetId, ElementType.IdBased, "");
            elementSet.AddElement(new Element(elementSetId));
            _elementSets.Add(elementSetId, elementSet);
        }

        private void AddExchangeItem(string inOrOut, string inputItemId)
        {
            string[] quantAndElementSet = inputItemId.Split('.');
            if (quantAndElementSet.Length != 2)
            {
                throw new Exception("No \".\" specified in \"" + inputItemId + "\"" + ", component " + Id);
            }
            string elementSetId = quantAndElementSet[0];
            string quantId = quantAndElementSet[1];

            if (!_quantities.ContainsKey(quantId))
            {
                throw new Exception("Quantity \"" + quantId + "\" does not exist in component " + Id);
            }
            if (!_elementSets.ContainsKey(elementSetId))
            {
                throw new Exception("ElementSet \"" + elementSetId + "\" does not exist in component " + Id);
            }

            if (inOrOut.Equals("inputitem"))
            {
                foreach (EngineInputItem inputItem in EngineInputItems)
                {
                    if (inputItem.Id.Equals(inputItemId))
                    {
                        throw new Exception("InputItem \"" + inputItemId + "\" already exists in component " + Id);
                    }
                }

                EngineInputItem newInputItem = new InputItem(inputItemId, _quantities[quantId],
                                                             _elementSets[elementSetId], this);
                newInputItem.StoreValuesInExchangeItem = true;
                EngineInputItems.Add(newInputItem);
            }
            else if (inOrOut.Equals("outputitem"))
            {
                foreach (EngineOutputItem outputItem in EngineOutputItems)
                {
                    if (outputItem.Id.Equals(inputItemId))
                    {
                        throw new Exception("OutputItem \"" + inputItemId + "\" already exists in component " + Id);
                    }
                }
                EngineOutputItem newOutputItem = new OutputItem(inputItemId, _quantities[quantId],
                                                                _elementSets[elementSetId], this);
                newOutputItem.StoreValuesInExchangeItem = true;
                EngineOutputItems.Add(newOutputItem);
            }
            else
            {
                throw new Exception("Unknown inOrOutOption: " + inOrOut);
            }
        }

        #endregion

        #region Input items / Output items

        #region Nested type: InputItem

        private class InputItem : EngineInputItem
        {
            public InputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet,
                             LinkableEngine component)
                : base(id, valueDefinition, elementSet, component)
            {
                // no additional action needed
            }

            public override ITimeSpaceValueSet GetValuesFromEngine()
            {
                throw new OpenMIException("Internal LinkableEngine error: Values are stored in the input item," +
                                          " so GetValuesFromEngine should not be called")
                          {Component = Component, Input = this};
            }

            public override void SetValuesToEngine(ITimeSpaceValueSet values)
            {
                throw new OpenMIException("Internal LinkableEngine error: Values are stored in the input item," +
                                          " so SetValuesToEngine should not be called")
                          {Component = Component, Input = this};
            }
        }

        #endregion

        #region Nested type: OutputItem

        private class OutputItem : EngineOutputItem
        {
            public OutputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet,
                              LinkableEngine component)
                : base(id, valueDefinition, elementSet, component)
            {
                // no additional action needed
            }

            public override ITimeSpaceValueSet GetValueFromEngine()
            {
                throw new OpenMIException("Internal LinkableEngine error: Values are stored in the output item," +
                                          " so GetValueFromEngine should not be called")
                          {Component = Component, Output = this};
            }
        }

        #endregion

        #endregion
    }
}