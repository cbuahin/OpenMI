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

namespace Oatc.OpenMI.Sdk.Buffer
{
  public abstract class TimeBufferer : AbstractAdaptedOutput
  {
    protected SmartBuffer _buffer;
    //private TimeSet _timeBufferTimeSet;

    public IComponentUpdater ComponentUpdater { get; set; }

    /// <summary>
    /// Constructor without a backing adaptee. Use in case all data is available
    /// directly in the buffer, or if implementing a custom <see cref="ComponentUpdater"/>
    /// </summary>
    /// <param name="id">Id of time buffer adapted output</param>
    protected TimeBufferer(string id)
      : base(id)
    {
      CreateBufferAndTimeSet();
    }

    /// <summary>
    /// Default constructor with a backing adaptee output.
    /// </summary>
    /// <param name="id">Id of time buffer adapted output</param>
    protected TimeBufferer(string id, ITimeSpaceOutput adaptee)
      : base(id, adaptee)
    {
      ComponentUpdater = new TimeComponentUpdater(adaptee);
      CreateBufferAndTimeSet();
    }

    public override ITimeSet TimeSet
    {
      get { return _buffer.TimeSet; }
      set { throw new NotSupportedException("Setting Timeset not allowed"); }
    }


    public override ISpatialDefinition SpatialDefinition
    {
      get
      {
        if (_adaptee != null)
          return _adaptee.SpatialDefinition;
        return (base.SpatialDefinition);
      }
    }

    public override IValueDefinition ValueDefinition
    {
      get
      {
        if (_adaptee != null)
          return _adaptee.ValueDefinition;
        return (base.ValueDefinition);
      }
    }

    public TimeSet TTimeSet
    {
      get { return _buffer.TimeSet; }
    }

    public override ITimeSpaceValueSet Values
    {
      get { return _buffer.ValueSet; }
      set { throw new NotSupportedException("Setting Values not allowed"); }
    }

    public override ITimeSpaceValueSet GetValues(IBaseExchangeItem basequerySpecifier)
    {
      ITimeSpaceExchangeItem querySpecifier = basequerySpecifier as ITimeSpaceExchangeItem;
      if (querySpecifier == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      //------------------------------------------------------
      // Check if we need to update the output component

      // Time set of query must be defined and have at least 1 time
      if (querySpecifier.TimeSet == null ||
          querySpecifier.TimeSet.Times == null ||
          querySpecifier.TimeSet.Times.Count == 0)
      {
        throw new Exception("Invalid query specifier \"" + querySpecifier.Id +
                            "\" for in GetValues() call to time decorater " + Id);
      }

      // Determine query time
      double queryTimeMjd = querySpecifier.TimeSet.Times[querySpecifier.TimeSet.Times.Count - 1].End().StampAsModifiedJulianDay;

      // Determine the times available in the buffer
      double availableTimeMjd = Double.NegativeInfinity;
      IList<ITime> currentTimes = TimeSet.Times;
      if (currentTimes.Count > 0)
      {
        availableTimeMjd = currentTimes[currentTimes.Count - 1].End().StampAsModifiedJulianDay;
      }

      // Check if we need to update
      // In case the output component is "busy", this may not actually update values
      // up to queryTimeMjd, in which case the _buffer.GetValues below will extrapolate.
      if (availableTimeMjd < queryTimeMjd)
      {
        if (ComponentUpdater == null)
          throw new Exception("Failed when trying to update time buffer (no updater is specified)");
        ComponentUpdater.Update(querySpecifier);
      }

      //------------------------------------------------------
      // Retrieve values from the buffer

      // Return the values for the required time(s)
      IList<IList<double>> resultValues = new List<IList<double>>();
      if (querySpecifier.TimeSet != null && querySpecifier.TimeSet.Times != null)
      {
        for (int t = 0; t < querySpecifier.TimeSet.Times.Count; t++)
        {
          ITime queryTime = querySpecifier.TimeSet.Times[t];
          double[] valuesForTimeStep = _buffer.GetValues(queryTime);
          resultValues.Add(valuesForTimeStep);
        }
      }
      ITime earliestConsumerTime = ExchangeItemHelper.GetEarliestConsumerTime(this);
	  if (earliestConsumerTime != null)
	  {
	     _buffer.ClearBefore(earliestConsumerTime);
      }
	  return new TimeSpaceValueSet<double>(resultValues);
    }

    public override void Refresh()
    {
      if (Adaptee.Component.Status != LinkableComponentStatus.Preparing &&
          Adaptee.Component.Status != LinkableComponentStatus.Updating)
      {
        throw new Exception(
            "Update function can only be called from component when it is validating or updating");
      }
      AddNewValuesToBuffer();

      // Update dependent adaptedOutput
      foreach (ITimeSpaceAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }

    private void CreateBufferAndTimeSet()
    {
      _buffer = new SmartBuffer();
      _timeSet = null;
      _values = null;
      //_timeBufferTimeSet = new TimeSet();
      UpdateTimeHorizonFromDecoratedOutputItem();
    }

    private void UpdateTimeHorizonFromDecoratedOutputItem()
    {
      if (Adaptee != null)
      {
        if (_adaptee.TimeSet == null)
        {
          // The decorated item has no time set, should not occur
          throw new Exception("Parent output \"" + Adaptee.Id + "\" has no time set");
        }
        // Use the time horizon of the decorated item
        ITime decoratedTimeHorizon = _adaptee.TimeSet.TimeHorizon;
        if (decoratedTimeHorizon != null)
        {
          _buffer.TimeSet.TimeHorizon =
              new Time(decoratedTimeHorizon.StampAsModifiedJulianDay,
                       decoratedTimeHorizon.DurationInDays);
        }
      }
      else
      {
        _buffer.TimeSet.TimeHorizon = null;
      }
    }

    private void AddNewValuesToBuffer()
    {
      ITimeSpaceValueSet decoratedOutputItemValues = (ITimeSpaceValueSet)Adaptee.Values;

      if (decoratedOutputItemValues == null)
      {
        throw new Exception("AdaptedOutput \"" + Id +
                            "\" did not receive values from Decorated OutputItem \"" + Adaptee.Id +
                            "\"");
      }

      for (int t = 0; t < _adaptee.TimeSet.Times.Count; t++)
      {
        ITime time = _adaptee.TimeSet.Times[t];
        IList elementSetValues = decoratedOutputItemValues.GetElementValuesForTime(t);
        _buffer.SetOrAddValues(time, elementSetValues);
      }
    }
  }
}