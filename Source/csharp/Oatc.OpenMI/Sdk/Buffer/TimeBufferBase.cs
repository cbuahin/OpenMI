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

namespace Oatc.OpenMI.Sdk.Buffer
{
    public abstract class TimeBufferBase : DerivedOutputBase
    {
        protected SmartBuffer _buffer;
        private TimeSet _timeBufferTimeSet;

        protected TimeBufferBase(string id)
            : base(id)
        {
            CreateBufferAndTimeSet();
        }

        protected TimeBufferBase(string id, IOutput decoratedOutput)
            : base(id, decoratedOutput)
        {
            CreateBufferAndTimeSet();
        }

        protected TimeBufferBase(string id, IOutput decoratedOutput, IInput target)
            : base(id, decoratedOutput, target)
        {
            CreateBufferAndTimeSet();
        }

        public override IOutput ParentOutput
        {
            set
            {
                base.ParentOutput = value;
                UpdateTimeHorizonFromDecoratedOutputItem();
            }
        }

        public override ITimeSet TimeSet
        {
            get { return timeSet; }
            set { throw new Exception("Setting Timeset not allowed"); }
        }

        public override IValueSet GetValues(IExchangeItem querySpecifier)
        {
            if (querySpecifier.TimeSet == null || querySpecifier.TimeSet.Times == null ||
                querySpecifier.TimeSet.Times.Count == 0)
            {
                throw new Exception("Invalid query specifier \"" + querySpecifier.Id +
                                    "\" for in GetValues() call to time decorater " + Id);
            }
            if (ParentOutput.TimeSet == null || ParentOutput.TimeSet.Times == null)
            {
                throw new Exception("Invalid time specifier in decorated output item \"" + ParentOutput.Id +
                                    "\" for in GetValues() call to time decorater " + Id);
            }

            // Determinee query time and currently available time
            double queryTimeAsMJD =
                querySpecifier.TimeSet.Times[querySpecifier.TimeSet.Times.Count - 1].StampAsModifiedJulianDay +
                querySpecifier.TimeSet.Times[querySpecifier.TimeSet.Times.Count - 1].DurationInDays;
            double availableTimeAsMJD = Double.NegativeInfinity;
            IList<ITime> decoratedOutputItemTimes = ParentOutput.TimeSet.Times;
            if (decoratedOutputItemTimes.Count > 0)
            {
                availableTimeAsMJD =
                    decoratedOutputItemTimes[decoratedOutputItemTimes.Count - 1].StampAsModifiedJulianDay +
                    decoratedOutputItemTimes[decoratedOutputItemTimes.Count - 1].DurationInDays;
            }

            // Update as far as needed.
            ILinkableComponent linkableComponent = ParentOutput.Component;
            while ((linkableComponent.Status == LinkableComponentStatus.Valid ||
                    linkableComponent.Status == LinkableComponentStatus.Updated) &&
                   availableTimeAsMJD < queryTimeAsMJD)
            {
                linkableComponent.Update();
                // Determine newly available time
                decoratedOutputItemTimes = ParentOutput.TimeSet.Times;
                availableTimeAsMJD =
                    decoratedOutputItemTimes[decoratedOutputItemTimes.Count - 1].StampAsModifiedJulianDay +
                    decoratedOutputItemTimes[decoratedOutputItemTimes.Count - 1].DurationInDays;
            }

            // Return the values for the required time(s)
            IList<IList<double>> resultValues = new List<IList<double>>();
            if (querySpecifier.TimeSet != null && querySpecifier.TimeSet.Times != null)
            {
                for (int t = 0; t < querySpecifier.TimeSet.Times.Count; t++)
                {
                    resultValues.Add(new List<double>());
                    ITime queryTime = querySpecifier.TimeSet.Times[t];
                    List<double> valuesForTimeStep = _buffer.GetValues(queryTime);
                    foreach (double d in valuesForTimeStep)
                    {
                        resultValues[t].Add(d);
                    }
                }
            }
            return new ValueSet<double>(resultValues);
        }

        public override void Refresh()
        {
            if (ParentOutput.Component.Status != LinkableComponentStatus.Validating &&
                ParentOutput.Component.Status != LinkableComponentStatus.Updating)
            {
                throw new Exception(
                    "Update function can only be called from component when it is validating or updating");
            }
            AddNewValuesToBuffer();

            // Update dependent derivedOutput
            foreach (IDerivedOutput derivedOutput in DerivedOutputs)
            {
                derivedOutput.Refresh();
            }
        }

        private void CreateBufferAndTimeSet()
        {
            _buffer = new SmartBuffer();
            timeSet = null;
            _timeBufferTimeSet = new TimeSet();
            UpdateTimeHorizonFromDecoratedOutputItem();
        }

        private void UpdateTimeHorizonFromDecoratedOutputItem()
        {
            if (ParentOutput != null)
            {
                if (ParentOutput.TimeSet == null)
                {
                    // The decorated item has no time set, should not occur
                    throw new Exception("Parent output \"" + ParentOutput.Id + "\" has no time set");
                }
                // Use the time horizon of the decorated item
                ITime decoratedTimeHorizon = ParentOutput.TimeSet.TimeHorizon;
                if (decoratedTimeHorizon != null)
                {
                    _timeBufferTimeSet.TimeHorizon =
                        new Time(decoratedTimeHorizon.StampAsModifiedJulianDay,
                                 decoratedTimeHorizon.DurationInDays);
                }
            }
            else
            {
                _timeBufferTimeSet.TimeHorizon = null;
            }
        }

        private void AddNewValuesToBuffer()
        {
            IValueSet decoratedOutputItemValues = ParentOutput.Values;

            if (decoratedOutputItemValues == null)
            {
                throw new Exception("DerivedOutput \"" + Id +
                                    "\" did not receive values from Decorated OutputItem \"" + ParentOutput.Id +
                                    "\"");
            }

            for (int t = 0; t < ParentOutput.TimeSet.Times.Count; t++)
            {
                ITime time = ParentOutput.TimeSet.Times[t];
                IList elementSetValues = decoratedOutputItemValues.GetElementValuesForTime(t);
                _buffer.SetOrAddValues(time, elementSetValues);
            }
        }
    }
}