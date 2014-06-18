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
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using OpenMI.Standard2.TimeSpace;


namespace Oatc.OpenMI.Examples.SimpleSCharpRiver
{
    /// <summary>
    /// The TimeSeriesComponent creates a buffer with the values 0,1,2,3,...,30 for each
    /// day starting 2004-12-31, i.e., 2005-01-01 will return the value 1.
    /// </summary>
    public class TimeSeriesComponent : LinkableComponent
    {
        readonly SmartBuffer _buffer = new SmartBuffer();

        private readonly IList<ITimeSpaceInput> _inputItems = new List<ITimeSpaceInput>();
        private readonly IList<ITimeSpaceOutput> _outputItems = new List<ITimeSpaceOutput>();
        private TimeSeriesOutputItem _timeSeriesOutputItem;

        public SmartBuffer Buffer
        {
            get { return _buffer; }
        }

        public override List<IAdaptedOutputFactory> AdaptedOutputFactories
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Initialize()
        {
            double start = Time.ToModifiedJulianDay(new DateTime(2004,12,31,0,0,0));

            for (int i = 0; i < 30; i++)
            {
                _buffer.AddValues(new Time(start + i,1.0), new[]{(double)i});
            }
            Quantity quantity = new Quantity(new Unit(PredefinedUnits.LiterPerSecond), "flow", "flow");
            ElementSet elementSet = new ElementSet("oo", "ID", ElementType.IdBased, "");
            Element element = new Element("ElementID");
            elementSet.AddElement(element);
            _timeSeriesOutputItem = new TimeSeriesOutputItem("oo.flow", quantity, elementSet, this);
            _outputItems.Add(_timeSeriesOutputItem);
        }

        public override IList<IBaseInput> Inputs
        {
            get { return new ListWrapper<ITimeSpaceInput, IBaseInput>(_inputItems); }
        }

        public override IList<IBaseOutput> Outputs
        {
            get { return new ListWrapper<ITimeSpaceOutput, IBaseOutput>(_outputItems); }
        }

        public override string[] Validate()
        {
            Status = LinkableComponentStatus.Validating;
            Status = LinkableComponentStatus.Valid;
            return new string[0];
        }

        public override void Prepare()
        {
            // TODO: Move code to here?!
        }

        public override void Update(params IBaseOutput[] requiredOutput)
        {
            if (requiredOutput != null )
            {
                if (requiredOutput.Length != 1)
                {
                    throw new Exception("Unexpected #required output item. Expected: 1, got " + requiredOutput.Length);
                }
                if (requiredOutput[0] != _timeSeriesOutputItem)
                {
                    throw new Exception("Unexpected required output item \"" + requiredOutput[0].Id + "\"");
                }
            }
        }

        public override void Finish()
        {
        }

        public class TimeSeriesOutputItem : Output
        {
            private readonly TimeSeriesComponent _timeSeriesComponent;

            public TimeSeriesOutputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet, TimeSeriesComponent 
                                                                                                                 timeSeriesComponent) : base(id, valueDefinition, elementSet)
            {
                _timeSeriesComponent = timeSeriesComponent;
                _timeSet = timeSeriesComponent._buffer.TimeSet;
                _values = timeSeriesComponent._buffer.ValueSet;
            }

            public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
            {
                ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
                if (timeSpaceQuery == null)
                    throw new ArgumentException("Query is not an ITimeSpaceExchangeItem - you may need to add an adaptor");
                if (timeSpaceQuery.TimeSet != null && timeSpaceQuery.TimeSet.Times.Count == 1)
                {
                    double[] valuesForTimeStep = _timeSeriesComponent.Buffer.GetValues(timeSpaceQuery.TimeSet.Times[0]);
                    return new ValueSet(new List<IList>{valuesForTimeStep});
                }
                throw new Exception("Invalid time specification in querying exchange item \"" + querySpecifier.Id + "\"");
            }

            public override ITimeSet TimeSet
            {
                get
                {
                    return _timeSeriesComponent.Buffer.TimeSet;
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override ITimeSpaceValueSet Values
            {
                get
                {
                    return _timeSeriesComponent.Buffer.ValueSet;
                }
                set
                {
                    throw new NotSupportedException();
                }
            }


        }
    }
}