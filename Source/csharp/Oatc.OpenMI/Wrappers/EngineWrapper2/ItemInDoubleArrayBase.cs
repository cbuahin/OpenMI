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
using System.Collections.Generic;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public abstract class ItemInDoubleArrayBase : ItemInBase
    {
        double[] _defaultValues;
        double _timeTolerance;

        public ItemInDoubleArrayBase(string id, ITimeSet componentTimeExtent, double[] defaultValues, double timeTolerance)
            : base(id)
        {
            TimeSet ts = new TimeSet();
            ts.HasDurations = false;
            ts.OffsetFromUtcInHours = 0;
            double startTime = componentTimeExtent.TimeHorizon.StampAsModifiedJulianDay;
            ts.TimeHorizon = componentTimeExtent.TimeHorizon;
            ts.SetSingleTimeStamp(startTime);

            _currentTimeSet = ts;
            _defaultValues = defaultValues;
            _timeTolerance = timeTolerance;

            DefaultValues();

            ValidTimeSet(_currentTimeSet);
            ValidValue(Values);
        }

        public override string CurrentState()
        {
            return string.Format("{0} = {1}",
                _currentTimeSet.Times[0].ToString(),
                Values.GetValue(0,0).ToString());
        }

        public override void DefaultValues()
        {
            Values = new TimeSpaceValueSet<double>(_defaultValues);
        }

        override public void ValidTimeSet(ITimeSet iTimeSet)
        {
            // TODO Standard move ValidTimeSet into IExchangeItem?
            if (iTimeSet == null)
                throw new ArgumentException("iTimeSet == null");
            if (iTimeSet.HasDurations)
                throw new ArgumentException("iTimeSet.HasDurations");
            if (iTimeSet.OffsetFromUtcInHours != 0)
                throw new ArgumentException("iTimeSet.OffsetFromUtcInHours != 0");
            if (iTimeSet.Times == null)
                throw new ArgumentException("iTimeSet.Times == null");
            if (iTimeSet.Times.Count != 1)
                throw new ArgumentException("iTimeSet.Times.Count != 1");
        }

        override public void ValidValue(ITimeSpaceValueSet valueSet)
        {
            // TODO Standard move ValidValue into IExchangeItem?
            if (valueSet == null)
                throw new ArgumentException("value == null");
            if (ValueSet.GetElementCount(valueSet) != _defaultValues.Length)
                throw new ArgumentException("valueSet.ElementCount != " + _defaultValues.Length.ToString());
            if (ValueSet.GetTimesCount(valueSet) != 1)
                throw new ArgumentException("valueSet.TimesCount != 1");
            if (!(valueSet.GetValue(0, 0) is double))
                throw new ArgumentException("!(valueSet.GetValue(0, 0) is double)");
        }

        override public bool UpdateRequired(ITimeSet request, ITimeSet current)
        {
            if (Values == null || ValueSet.GetElementCount(Values) * ValueSet.GetTimesCount(Values) != 1)
                return true;

            ValidTimeSet(request);
            ValidTimeSet(current);

            double dRequest = request.Times[0].StampAsModifiedJulianDay;
            double dCurrent = current.Times[0].StampAsModifiedJulianDay;

            if (Math.Abs(dCurrent - dRequest) <= _timeTolerance)
                return false; // Equal within tolerance

            // TODO Could check cache and just throw if cache inadequate

            if (dRequest < dCurrent)
                throw new InvalidOperationException("Earlier time request");

            return true;
        }
    }

}
