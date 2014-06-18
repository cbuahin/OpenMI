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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper
{
  /// <summary>
  /// <para>
  /// An abstract <see cref="ITimeSpaceOutput"/> having special functionality for
  /// supporting a time stepping engine, the <see cref="LinkableEngine"/>
  /// </para>
  /// <para>
  /// It automatically updates the <see cref="LinkableEngine.ActiveOutputItems"/>
  /// and has support for storing values in the exchange item, and handling time
  /// as stamps as well as spans.
  /// </para>
  /// </summary>
  public abstract class EngineOutputItem : Output
  {

    protected readonly LinkableEngine _linkableEngine;
    protected bool _storeValuesInExchangeItem;

    protected EngineOutputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet, LinkableEngine linkableEngine)
      : base(id, valueDefinition, elementSet)
    {
      _linkableEngine = linkableEngine;
      _component = linkableEngine;
      _timeSet = new TimeSet { HasDurations = false };
      _storeValuesInExchangeItem = linkableEngine.DefaultForStoringValuesInExchangeItem;
    }

    /// <summary>
    /// Flag indicating if data is stored in the exhange item, or must be retrieved from the engine.
    /// <para>
    /// If true, the output values are stored in <see cref="Values"/>, and 
    /// updated just after a time step has been performed (by <see cref="Update"/>).
    /// </para>
    /// <para>
    /// If false, the output values are not stored, but retrieved every time
    /// by a call to the <see cref="GetValueFromEngine"/>, i.e. whenever
    /// the <see cref="Values"/> are requested.
    /// </para>
    /// </summary>
    public bool StoreValuesInExchangeItem
    {
      get { return _storeValuesInExchangeItem; }
      set { _storeValuesInExchangeItem = value; }
    }

    /// <summary>
    /// Get the output time of this output item. Used by <see cref="Update"/>.
    /// <para>
    /// A return value of null indicates that this output can not be updated 
    /// for the current time step.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The base implementation asks the <see cref="LinkableEngine.GetCurrentTime"/>
    /// for the time. Override this method if a particular output item uses another
    /// time span.
    /// </remarks>
    /// <returns>The output time, null if the engine can not update the output at this time</returns>
    protected virtual ITime GetOutputTime()
    {
      return _linkableEngine.GetCurrentTime(!TimeSet.HasDurations);
    }

    public override void AddConsumer(IBaseInput consumer)
    {
      if (consumer == null)
        throw new ArgumentNullException("consumer", "Can not be null");
      base.AddConsumer(consumer);
      // Add it to list of active output items
      _linkableEngine.ActiveOutputItems.Add(this);
    }

    public override void RemoveConsumer(IBaseInput consumer)
    {
      base.RemoveConsumer(consumer);
      if (_consumers.Count == 0 && _adaptedOutputs.Count == 0)
        _linkableEngine.ActiveOutputItems.Remove(this);
    }

    public override void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      if (adaptedOutput == null)
        throw new ArgumentNullException("adaptedOutput", "Can not be null");
      base.AddAdaptedOutput(adaptedOutput);
      // Add it to list of active output items
      _linkableEngine.ActiveOutputItems.Add(this);
    }

    public override void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      base.RemoveAdaptedOutput(adaptedOutput);
      if (_consumers.Count == 0 && _adaptedOutputs.Count == 0)
        _linkableEngine.ActiveOutputItems.Remove(this);
    }

    public override ITimeSpaceValueSet Values
    {
      get
      {
        return _storeValuesInExchangeItem ? _values : (GetValueFromEngine());
      }
      set
      {
        if (_storeValuesInExchangeItem)
          _values = value;
        else
          throw new NotSupportedException();
      }
    }

    public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      CheckSpecificationAndTryUpdateIfRequired(timeSpaceQuery);
      return Values;
    }

    /// <summary>
    /// Checks the <paramref name="querySpecifier"/> if it matches the current values. If not,
    /// try and update the component and the item.
    /// 
    /// If failing, this will throw an exception
    /// </summary>
    protected void CheckSpecificationAndTryUpdateIfRequired(ITimeSpaceExchangeItem querySpecifier)
    {
      if (!ExchangeItemHelper.OutputAndInputElementSetsFit(this, querySpecifier))
      {
        throw new Exception("ElementSet of output item \"" + Id +
                            "\" does not fit the ElementSet of requesting item \"" + querySpecifier.Id);
      }

      if (!ExchangeItemHelper.OutputAndInputTimeSetsFit(this, querySpecifier))
      {
        TimeComponentUpdater.Update(this, querySpecifier);

        // TODO JGR: Remove commented code when tested properly
        //if (querySpecifier.TimeSet == null ||
        //    querySpecifier.TimeSet.Times == null ||
        //    querySpecifier.TimeSet.Times.Count != 1)
        //{
        //  // Time independent target item, but for some reason this output item's time set does
        //  // not fit (most probably because is has either 0 or more then 1 timesteps).
        //  throw new Exception("Given the TimeSet of output item \"" + Id +
        //                      "\", it can not produce one set of values for \"" + querySpecifier.Id + "\"");
        //}

        //// Compute until the value are available indeed
        //// Check the max #of steps, to avoid an eternal loop
        //double targetTimeAsMJD = querySpecifier.TimeSet.Times[0].End().StampAsModifiedJulianDay;

        //while ((_linkableEngine.Status == LinkableComponentStatus.Valid ||
        //    _linkableEngine.Status == LinkableComponentStatus.Updated) &&
        //    _linkableEngine.CurrentTime.StampAsModifiedJulianDay + Time.EpsilonForTimeCompare < targetTimeAsMJD)
        //{
        //  _linkableEngine.Update();
        //}

      }
      if (!ExchangeItemHelper.OutputAndInputTimeSetsFit(this, querySpecifier))
      {
        throw new Exception("Could not update engine \"" + _linkableEngine.Id + "\" to required time for output item \"" + Id +
                            "\" (requiring input item \"" + querySpecifier.Id + "\"). Use a Time Extrapolator.");
      }

    }

    /// <summary>
    /// Function to get current values from the engine.
    /// </summary>
    public abstract ITimeSpaceValueSet GetValueFromEngine();

    /// <summary>
    /// Updates the values in the exchange item and the times in the time set.
    /// <para>
    /// This method is called just after the <see cref="LinkableEngine.PerformTimestep"/>
    /// of the <see cref="LinkableEngine"/>. If it returns true, all its output adaptors
    /// are refreshed.
    /// </para>
    /// <para>
    /// If a particular output item is not updated by a performed time step, 
    /// override this method and let it return false when not updated.
    /// </para>
    /// </summary>
    /// <returns>true if output item was updated, false if not</returns>
    public virtual bool Update()
    {
      ITime time = GetOutputTime();
      if (time == null)
        return false;

      TimeSet.SetSingleTime(time);
      if (_storeValuesInExchangeItem)
      {
        _values = GetValueFromEngine();
      }
      else
      {
        // no action needed, engine update call has fill exchange item
      }
      return true;
    }

  }
}
