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
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    internal class RainRrCfMeasurementsDatabase : LinkableComponent
    {
        private readonly List<ITimeSpaceInput> _inputItems = new List<ITimeSpaceInput>();
        private readonly List<ITimeSpaceOutput> _outputItems = new List<ITimeSpaceOutput>();

        private const int _MeasurementCount = 20;
        private static readonly double _measurementsStartTimeAsMJD = new Time(new DateTime(2009, 3, 27)).StampAsModifiedJulianDay;
        private static readonly double _measurementsEndTimeAsMJD = new Time(new DateTime(2009, 3, 29)).StampAsModifiedJulianDay;
        private ITimeSet _timeSet;
        private List<IAdaptedOutputFactory> _adaptedOutputFactories;

        public static double StartTimeAsMJD
        {
            get { return _measurementsStartTimeAsMJD; }
        }

        public static double EndTimeAsMJD
        {
            get { return _measurementsEndTimeAsMJD; }
        }

        public override List<IAdaptedOutputFactory> AdaptedOutputFactories
        {
            get
            {
                return _adaptedOutputFactories;
            }
        }

        public override void Initialize()
        {
            Status = LinkableComponentStatus.Initializing;

            Id = "Rainfall Measurements";

            _adaptedOutputFactories = new List<IAdaptedOutputFactory>();
            _adaptedOutputFactories.Add(new AdaptedOutputFactory("RainRRCFTestAdaptedOutput"));

            // Create measurements
            _timeSet =
                new RainfallMeasurementsTimeSet(_MeasurementCount, _measurementsStartTimeAsMJD, _measurementsEndTimeAsMJD);

            IDictionary<string, IElementSet> elementSets = RainRrCfElementSets.CreateRainfallMeasurementsElementSets();

            const string rainId = "Rain";
            Quantity rainfallValueDefinition = new Quantity(new Unit("mm/day", 1.15741E-08, 0), rainId,
                                                            "Rainfall in mm/day");
            double startValue = 100;
            foreach (ElementSet elmSet in elementSets.Values)
            {
                Output outputItem = new RainfallOutputItem(elmSet.Caption + "." + rainId, startValue)
                                        {
                                            ValueDefinition = rainfallValueDefinition,
                                            SpatialDefinition = elmSet,
                                            TimeSet = _timeSet,
                                            Component = this,
                                        };
                _outputItems.Add(outputItem);
                startValue += 300;
            }
            Status = LinkableComponentStatus.Initialized;
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

        public override IList<IBaseInput> Inputs
        {
            get { return new ListWrapper<ITimeSpaceInput, IBaseInput>(_inputItems); }
        }

        public override IList<IBaseOutput> Outputs
        {
            get { return new ListWrapper<ITimeSpaceOutput, IBaseOutput>(_outputItems); }
        }

        public override void Update(params IBaseOutput[] requiredOutput)
        {
            Status = LinkableComponentStatus.Updating;
            // no action
            Status = LinkableComponentStatus.Updated;
        }

        public override void Finish()
        {
            Status = LinkableComponentStatus.Finishing;
            // no action
            Status = LinkableComponentStatus.Done;
        }

        #region Nested type: RainfallMeasurementsTimeSet

        internal class RainfallMeasurementsTimeSet : ITimeSet
        {
            private readonly ITime _timeHorizon;
            private readonly IList<ITime> _times;

            public RainfallMeasurementsTimeSet(int measurementCount, double startTimeAsMJD, double endTimeAsMJD)
            {
                _timeHorizon = new Time(startTimeAsMJD, endTimeAsMJD - startTimeAsMJD);
                double deltaT = (endTimeAsMJD - startTimeAsMJD) / (measurementCount - 1);
                _times = new List<ITime>();
                for (int tStep = 0; tStep < measurementCount; tStep++)
                {
                    ITime timeStamp = new Time(startTimeAsMJD + tStep * deltaT);
                    _times.Add(timeStamp);
                }
            }

            #region ITimeSet Members

            public IList<ITime> Times
            {
                get { return _times; }
            }

            public bool HasDurations
            {
                get { throw new NotImplementedException(); }
            }

            public double OffsetFromUtcInHours { get; set; }

            public ITime TimeHorizon
            {
                get { return _timeHorizon; }
            }

            #endregion
        }

        #endregion
    }

    internal class AdaptedOutputFactory : IAdaptedOutputFactory
    {
        private string _caption = String.Empty;
        private string _description = String.Empty;
        private readonly string _id;
        const string TimeInterpolatorId = "MeasurementsInterpolator";
        const string TimeExtrapolatorId = "TimeExtrapolator";

        public AdaptedOutputFactory(string id)
        {
            _id = id;
        }

        public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
        {
          // The time methods in this factory only works on an ITimeSpaceOutput
          ITimeSpaceOutput tsoutput = adaptee as ITimeSpaceOutput;
          if (tsoutput == null)
            return (new IIdentifiable[0]);
          return new IIdentifiable[] { new AdaptedOutputIdentifier(TimeInterpolatorId), new AdaptedOutputIdentifier(TimeExtrapolatorId) };
        }

        public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputIdentifier, IBaseOutput parentOutput, IBaseInput target)
        {
            IBaseAdaptedOutput adaptedOutput = null;
            if (adaptedOutputIdentifier.Id.Equals(TimeInterpolatorId))
            {
                adaptedOutput = new MeasurementsInterpolator((ITimeSpaceOutput)parentOutput);
            }
            else if (adaptedOutputIdentifier.Id.Equals(TimeExtrapolatorId))
            {
                adaptedOutput = new TimeExtrapolator((ITimeSpaceOutput)parentOutput);
            }
            if (adaptedOutput == null)
            {
                throw new Exception("AdaptedOutput id: " + adaptedOutputIdentifier.Id);
            }

            // connect adaptor and adaptee
            if (!parentOutput.AdaptedOutputs.Contains(adaptedOutput))
            {
                parentOutput.AddAdaptedOutput(adaptedOutput);
            }
            return adaptedOutput;
        }

        public IDescribable GetAdaptedOutputDescription(IIdentifiable id)
        {
            throw new NotImplementedException();
        }

        public string Id
        {
            get { return _id; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        internal class AdaptedOutputIdentifier : IIdentifiable
        {
            private readonly string _adaptedOutputId;

            public AdaptedOutputIdentifier(string adaptedOutputId)
            {
                _adaptedOutputId = adaptedOutputId;
            }

            public string Id
            {
                get
                {
                    return _adaptedOutputId;
                }
            }

            public string Caption { get; set; }
            public string Description { get; set; }
        }
    }

    internal class RainfallOutputItem : Output
    {
        private readonly double _startValue;

        public RainfallOutputItem(string id, double startValue)
            : base(id)
        {
            _startValue = startValue;
        }

        public override ITimeSpaceValueSet Values
        {
            get
            {
                int timesCount = TimeSet.Times.Count;
                IList rainfallValues = new List<double>();
                for (int t = 0; t < timesCount; t++)
                {
                    double value = _startValue + t;
                    rainfallValues.Add(value);
                }
                return new ValueSet(new List<IList> { rainfallValues });
            }
            set { throw new Exception(Component.Caption + ": Setting Values not allowed"); }
        }
    }

    internal class MeasurementsInterpolator : Output, ITimeSpaceAdaptedOutput
    {
        private ITimeSpaceOutput _adaptee;
        private SmartBuffer _buffer;
        private const string MeasurementsInterpolatorName = "MeasurementsInterpolator";

        internal MeasurementsInterpolator(ITimeSpaceOutput parentOutput)
            : base(parentOutput.Caption + "-" + MeasurementsInterpolatorName)
        {
            _valueDefinition = parentOutput.ValueDefinition;
            _spatialDefinition = parentOutput.ElementSet();
            _adaptee = parentOutput;
        }

        #region IAdaptedOutput Members

        public override IBaseLinkableComponent Component
        {
            get { return _adaptee.Component; }
            set
            {
                throw new Exception("Setting adaptedOutput id not allowed (adaptedOutput \"" + Id + "\")");
            }
        }

        public override ITimeSpaceValueSet Values
        {
            get
            {
                CheckBuffer();
                return new TimeSpaceValueSet<double>(_buffer.GetAllValues());
            }
        }

        public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier2)
        {
            CheckBuffer();
            List<IList<double>> valuesList = new List<IList<double>>();
            for (int i = 0; i < ((ITimeSpaceInput)Consumers[0]).TimeSet.Times.Count; i++)
            {
                ITime time = ((ITimeSpaceInput)Consumers[0]).TimeSet.Times[i];
                valuesList.Add(_buffer.GetValues(time));
            }
            return new TimeSpaceValueSet<double>(valuesList);
        }

        private void CheckBuffer()
        {
            if (_buffer == null)
            {
                _buffer = new SmartBuffer();
                ITimeSpaceValueSet outputItemValues = _adaptee.Values;
                IList<ITime> outputItemTimes = _adaptee.TimeSet.Times;
                for (int t = 0; t < outputItemTimes.Count; t++)
                {
                    ITime time = outputItemTimes[t];
                    _buffer.AddValues(time, outputItemValues.GetElementValuesForTime(0));
                }
            }
        }

        public IList<IArgument> Arguments
        {
            get { return new List<IArgument>(); }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public IBaseOutput Adaptee
        {
            get { return _adaptee; }
        }

        public void Refresh()
        {
            // AdaptedOutput is filled after initialization, no action needed during update

            // Update dependent adaptedOutputs
            foreach (ITimeSpaceAdaptedOutput adaptedOutput in AdaptedOutputs)
            {
                adaptedOutput.Refresh();
            }
        }

        #endregion
    }
}