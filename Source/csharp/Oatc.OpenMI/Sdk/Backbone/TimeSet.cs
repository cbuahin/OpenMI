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
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  public class TimeSet : ITimeSet, IEnumerable<ITime>
  {
    #region ITimeSet Members

    List<ITime> _times = new List<ITime>();
    private bool _hasDurations;
    private ITime _timeHorizon = new Time();
    private double _offsetFromUtcInHours;

    IList<ITime> ITimeSet.Times
    {
      get { return _times; }
    }

    public List<ITime> Times
    {
      get { return _times; }
      set { _times = value; }
    }

    public bool HasDurations
    {
      get { return _hasDurations; }
      set { _hasDurations = value; }
    }

    public double OffsetFromUtcInHours
    {
      get { return _offsetFromUtcInHours; }
      set { _offsetFromUtcInHours = value; }
    }

    public ITime TimeHorizon
    {
      get { return _timeHorizon; }
      set { _timeHorizon = value; }
    }

    public void Add(ITime time)
    {
      _times.Add(time);
    }

    public void RemoveRange(int index, int count)
    {
      if (_times != null)
      {
        _times.RemoveRange(index, count);
      }
    }

    public ITime this[int timeIndex]
    {
      get { return _times[timeIndex]; }
      set { _times[timeIndex] = value; }
    }

    public int Count { get { return (_times.Count); } }

    #endregion

    #region Convenience Functions

    public void SetTimeHorizonFromTimes()
    {
      TimeHorizon = new Time(Time.Start(Times[0]), Time.End(Times[Times.Count - 1]));
    }

    #endregion

    public IEnumerator<ITime> GetEnumerator()
    {
      return (_times.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
