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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public abstract class ItemOutDoubleBase : ItemOutBase
    {
        List<DataPair> _cache
            = new List<DataPair>();
        DataPair _initial = null;

        // Relaxation
        // 1 = last value in cache
        // 0 = Full Extrapolation
        // For me the other way around makes more scence, 
        // but must maintain version 1 compatability
        double _relaxation = 1;

        class DataPair
        {
            ITime _time;
            double _value;

            public DataPair(ITimeSet timeSet, IList value)
            {
                _time = timeSet.Times[0];
                _value = (double)value[0];
            }

            public DataPair(ITime time, double value)
            {
                _time = time;
                _value = value;
            }

            public double Time
            {
                get { return _time.StampAsModifiedJulianDay; }
            }

            public double Value
            {
                get { return _value; }
            }
        }

        public ItemOutDoubleBase(string id, ITime initialTime, double initialValue, double relaxation)
            : base(id)
        {
            _initial = new DataPair(initialTime, initialValue);
            _relaxation = relaxation;
        }

        public override string CurrentState()
        {
            return string.Format("Cache[{0}] = ({1} .. {2})",
                _cache.Count.ToString(),
                _cache.Count > 0 ? _cache[0].Time.ToString() : "",
                _cache.Count > 0 ? _cache[_cache.Count - 1].Time.ToString() : "");
        }

        public override void CachePush(ITimeSet timeSet, IList value)
        {
            if (Consumers == null || Consumers.Count == 0)
                return;

            ValidTimeSet(timeSet);
            ValidValue(value);

            _cache.Add(new DataPair(timeSet, value));

            // Tidy up cache

            double? timeLowest = null;
            double timeTarget;

            foreach (ITimeSpaceInput item in Consumers)
            {
                ValidTimeSet(item.TimeSet);

                timeTarget = item.TimeSet.Times[0].StampAsModifiedJulianDay;

                if (timeLowest == null)
                    timeLowest = item.TimeSet.Times[0].StampAsModifiedJulianDay;
                else if (timeTarget < timeLowest)
                    timeLowest = timeTarget;
            }

            if (timeLowest == null)
                _cache.Clear();
            else if (_cache.Count > 2)
            {
                // Only need to keep one value below lowest of consumers
                // for one point interpolation and minimum of 2 points
                // total for extrapolation

                int nLowest = 0;

                while (nLowest < _cache.Count
                    && _cache[nLowest].Time < timeLowest.Value)
                    ++nLowest;

                if (nLowest == _cache.Count - 1)
                    --nLowest; // maintain at least two items

                if (nLowest < _cache.Count && nLowest > 0)
                    _cache.RemoveRange(0, nLowest); // O(n)
            }
        }

        public override IList<IList<double>> CacheGet()
        {
            // DataPair's are local, need to turn into OpenMI reconised objects

            IList<IList<double>> objs = new List<IList<double>>(2 * _cache.Count);

            for (int i = 0; i < _cache.Count; i++)
            {
                DataPair pair = _cache[i];
                objs[i] = new List<double> { pair.Time, pair.Value };
            }

            return objs;
        }

        public override bool CacheUpdateSource(ITimeSpaceInput source, bool forceCacheUpdate)
        {
            ValidTimeSet(source.TimeSet);

            double required = source.TimeSet.Times[0].StampAsModifiedJulianDay;

            int nAbove = -1;

            for (int n = 0; n < _cache.Count; ++n)
            {
                if (_cache[n].Time > required)
                {
                    nAbove = n;
                    break;
                }
            }

            IList<IList<double>> list = new List<IList<double>>();
            double timeRatio, value;

            if (nAbove == -1)
            {
                if (!forceCacheUpdate)
                    return false;

                if (_cache.Count == 0)
                    value = _initial.Value;
                else if (_cache.Count == 1)
                    value = _cache[0].Value;
                else
                {
                    DataPair prev = _cache[_cache.Count - 2];
                    DataPair last = _cache[_cache.Count - 1];

                    timeRatio = (required - prev.Time) / (last.Time - prev.Time);
                    double extrapolated = prev.Value + timeRatio * (last.Value - prev.Value);

                    value = last.Value + (1 - _relaxation) * (extrapolated - last.Value);
                }

                list.Add(new List<double> { value });
                source.Values = new TimeSpaceValueSet<double>(list);
                return true;
            }

            DataPair above = _cache[nAbove];
            DataPair below = nAbove > 0 ? _cache[nAbove - 1] : _initial;

            if (below == null)
                throw new NotImplementedException();

            timeRatio = (required - below.Time) / (above.Time - below.Time);
            value = below.Value + timeRatio * (above.Value - below.Value);

            list.Add(new List<double> { value });
            source.Values = new TimeSpaceValueSet<double>(list);

            return true;
        }

        override public void ValidTimeSet(ITimeSet iTimeSet)
        {
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

        override public void ValidValue(IList value)
        {
            if (value == null)
                throw new ArgumentException("value == null");
            if (value.Count != 1)
                throw new ArgumentException("value.Count != 1");
            if (!(value[0] is double))
                throw new ArgumentException("!(value[0] is double)");
        }
	}
}
