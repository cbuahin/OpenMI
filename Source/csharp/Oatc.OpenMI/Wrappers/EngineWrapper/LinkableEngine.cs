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
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper
{
  public abstract class LinkableEngine : ITimeSpaceComponent
  {
    #region Abstract methods, to be overriden by deriving Engine Component

    // Time Info and Time stepping

    /// <summary>
    /// Start time of simulation
    /// </summary>
    protected internal abstract ITime StartTime { get; }

    /// <summary>
    /// End time of simulation
    /// </summary>
    protected internal abstract ITime EndTime { get; }

    /// <summary>
    /// The current time stamp, where the engine currently has reached.
    /// </summary>
    public abstract ITime CurrentTime { get; }

    /// <summary>
    /// Get the global time for the current state/output of the engine.
    /// <para>
    /// When requesting as time span, this is the time from last time step to 
    /// the current time step.
    /// </para>
    /// <para>
    /// This is called from the <see cref="EngineOutputItem.GetOutputTime"/>
    /// (unless overridden), assuming that all output items uses the same time.
    /// </para>
    /// <para>
    /// If all output items does not use the same output time, override the 
    /// <see cref="EngineOutputItem.GetOutputTime"/> to produce the correct  
    /// output time. If all output items calculate their output time themselfs, 
    /// this implementation can be left empty.
    /// </para>
    /// </summary>
    /// <param name="asStamp">Boolean specifying whether the time is a stamp or a span.</param>
    /// <returns>The time used for output</returns>
    public abstract ITime GetCurrentTime(bool asStamp);

    /// <summary>
    /// Get the "global" input time for the current time step. 
    /// <para>
    /// When requesting as time span, this is the time from the current time step to 
    /// the next time step.
    /// </para>
    /// <para>
    /// When requesting as time stamp, this can be any time in between the
    /// time of the current time step and the next time step, depending on the
    /// type of input. Often it is the time of the next time step.
    /// </para>
    /// <para>
    /// This is called from the <see cref="EngineInputItem.GetInputTime"/>,
    /// assuming that all input items requires the same time for input.
    /// </para>
    /// <para>
    /// If all input items does not use the same input time, override the 
    /// <see cref="EngineInputItem.GetInputTime"/> to produce the correct  
    /// input time. If all input items calculate their input time themself, 
    /// this implementation can be left empty.
    /// </para>
    /// </summary>
    /// <param name="asStamp">Boolean specifying whether the time is a stamp or a span.</param>
    /// <returns>The time used for input</returns>
    public abstract ITime GetInputTime(bool asStamp);

    // Model control 
    public abstract void Initialize();

    protected internal abstract string[] OnValidate(); // todo: Make abstract validate instead?
    protected internal abstract void OnPrepare(); // todo: Make abstract Prepare instead?

    /// <summary>
    /// Make the underlying engine perform one time step
    /// <para>
    /// This methods is protected, so it can not be called from the outside.
    /// Use the <see cref="Update"/> method from the outside to make the engine
    /// perform one time step.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This method will not trigger an update of the input and output items, which
    /// is why it is hidden as protected. The <see cref="Update"/> method does exactly that.
    /// </remarks>
    /// <param name="requiredOutputItems">Set of required output items that must be updated. May be null.</param>
    // TODO JGR: should return true on success and handle failure
    protected internal abstract void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems);

    public abstract void Finish();

    #endregion

    public virtual void Prepare()
    {
      Status = LinkableComponentStatus.Preparing;
      // Prepare the engine
      OnPrepare();
      // Prepare output items to have "initial" values.
      ProcessActiveOutputItems();
      Status = LinkableComponentStatus.Updated;
    }

    public virtual string Caption { get; set; }
    public virtual string Description { get; set; }
    public virtual string Id
    {
      get
      {
        if (_id == null)
        {
          throw new Exception("Id not set");
        }
        return _id;
      }
      set
      {
        _id = value;
      }
    }

    public IList<IArgument> Arguments { get; set; }

    public IList<EngineInputItem> EngineInputItems { get { return (_inputExchangeItems); } }
    public IList<EngineOutputItem> EngineOutputItems { get { return (_outputExchangeItems); } }

    #region fields

    protected string _id;
    protected LinkableComponentStatus _status;
    protected TimeSet _timeExtent;

    protected readonly List<EngineInputItem> _inputExchangeItems = new List<EngineInputItem>();
    protected readonly List<EngineOutputItem> _outputExchangeItems = new List<EngineOutputItem>();
    protected List<IAdaptedOutputFactory> _adaptedOutputFactories;

    #endregion


    /// <summary>
    /// <para>
    /// List of output items that currently have a consumer. 
    /// </para>
    /// <para>
    /// Every <see cref="EngineOutputItem"/>
    /// that has its list of <see cref="EngineOutputItem.Consumers"/> updated, i.e., everytime
    /// either <see cref="EngineOutputItem.AddConsumer"/> or <see cref="EngineOutputItem.RemoveConsumer"/>
    /// is called, the output item must add or remove itself from the list.
    /// </para>
    /// <para>
    /// The <see cref="EngineOutputItem"/> currently handles this.
    /// </para>
    /// </summary>
    public HashSet<EngineOutputItem> ActiveOutputItems { get { return (_activeOutputItems); } }

    private readonly HashSet<EngineOutputItem> _activeOutputItems = new HashSet<EngineOutputItem>();


    /// <summary>
    /// <para>
    /// List of input items that currently has a provider. 
    /// </para>
    /// <para>
    /// Every <see cref="EngineInputItem"/>
    /// that has its <see cref="EngineInputItem.Provider"/> updated, the input item 
    /// must add or remove itself from the list.
    /// </para>
    /// </summary>
    public HashSet<EngineInputItem> ActiveInputItems { get { return (_activeInputItems); } }

    private readonly HashSet<EngineInputItem> _activeInputItems = new HashSet<EngineInputItem>();


    /// <summary>
    /// <para>
    /// List of input items that needs to be processed. These items has value 
    /// stored inside that has not yet been processed, i.e., send on to the 
    /// engine. As the first thing to do in <see cref="PerformTimestep"/>
    /// the engine must make sure to go through this list, process all the
    /// input items and clear the list.
    /// </para>
    /// <para>
    /// An EngineInputItem can add itself to this list, to indicate to the <see cref="LinkableEngine"/>
    /// that it has not yet been processed. An EngineInput item does this if either it has a provider
    /// that has updated the Values, or if the Values has been manually updated.
    /// </para>
    /// <para>
    /// When processing an input item, make sure first to check the <see cref="EngineInputItem.HasBeenProcessed"/> 
    /// flag. If that is true, then the input item has already been processed, and should be skipped.
    /// Remember to set the flag to true when done processing the item. 
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implemented as List, since this will grow up and be cleared for every time step.
    /// </para>
    /// <para>
    /// Together with the <see cref="EngineInputItem.HasBeenProcessed"/> flag this makes
    /// sure that an <see cref="EngineInputItem"/> is only processed when necessary, and only once 
    /// even if it present several times in the list. 
    /// </para>
    /// <para>
    /// To ensure an <see cref="EngineInputItem"/> is only handled once, you could also use a 
    /// HashSet which does not allow duplicates, and then drop the 
    /// <see cref="EngineInputItem.HasBeenProcessed"/> flag.
    /// </para>
    /// </remarks>
    public List<EngineInputItem> InputItemsToBeProcessed { get { return (_inputItemsToBeProcessed); } }

    private readonly List<EngineInputItem> _inputItemsToBeProcessed = new List<EngineInputItem>();


    public List<IAdaptedOutputFactory> AdaptedOutputFactories
    {
      get
      {
        if (_adaptedOutputFactories == null)
        {
          _adaptedOutputFactories = new List<IAdaptedOutputFactory>();
          _adaptedOutputFactories.Add(new TimeBufferFactory(Id + "-TimeBuffer"));
          _adaptedOutputFactories.Add(new SpatialAdaptedOutputFactory(Id + "-Spatial"));
        }
        return _adaptedOutputFactories;
      }

    }

    public LinkableComponentStatus Status
    {
      set
      {
        LinkableComponentStatus oldStatus = _status;
        _status = value;

        if (StatusChanged != null)
        {
          LinkableComponentStatusChangeEventArgs args = new LinkableComponentStatusChangeEventArgs { LinkableComponent = this, OldStatus = oldStatus, NewStatus = value };
          StatusChanged(this, args);
        }
      }
      get { return _status; }
    }

    public bool CascadingUpdateCallsDisabled { get; set; }

    public event EventHandler<LinkableComponentStatusChangeEventArgs> StatusChanged;

    public virtual ITimeSet TimeExtent
    {
      get
      {
        if (_timeExtent == null)
        {
          _timeExtent = new TimeSet();
          _timeExtent.HasDurations = true;
          _timeExtent.SetSingleTimeSpan(StartTime, EndTime);
          _timeExtent.SetTimeHorizonFromTimes();
        }
        return _timeExtent;
      }
    }

    public IList<IBaseInput> Inputs
    {
      get
      {
        ListWrapper<EngineInputItem, IBaseInput> wrapper = new ListWrapper<EngineInputItem, IBaseInput>(EngineInputItems);
        return (wrapper);
      }
    }

    public IList<IBaseOutput> Outputs
    {
      get
      {
        ListWrapper<EngineOutputItem, IBaseOutput> wrapper = new ListWrapper<EngineOutputItem, IBaseOutput>(EngineOutputItems);
        return (wrapper);
      }
    }

    public virtual bool DefaultForStoringValuesInExchangeItem { get; set; }


    public string[] Validate()
    {
      Status = LinkableComponentStatus.Validating;
      string[] validationResults = OnValidate();
      if (validationResults == null || validationResults.Length == 0)
      {
        Status = LinkableComponentStatus.Valid;
      }
      else
      {
        Status = LinkableComponentStatus.Invalid;
      }
      return validationResults;
    }


    public void Update(params IBaseOutput[] requiredOutput)
    {

      if (requiredOutput != null && requiredOutput.Length != 0)
      {
        throw new NotSupportedException("Can not handle a list of requiredOutput");
      }

      //// TODO JGr: Check that it is ok to remove usage of requiredOutput (JGr)

      //if (requiredOutput != null && requiredOutput.Length != 0)
      //{
      //    // cast required output items back to EngineOutputItems
      //    foreach (IOutput outputItem in requiredOutput)
      //    {
      //        if (outputItem.Consumers.Count > 0 || outputItem.AdaptedOutputs.Count > 0)
      //        {
      //            if (!(outputItem is EngineOutputItem))
      //            {
      //                throw new Exception("Unexpected IOutput type in Update(outputItems): " +
      //                                    outputItem.GetType());
      //            }
      //        }
      //    }
      //}

      // Check in which mode we are running
      if (!CascadingUpdateCallsDisabled)
      {
        // Pull driven mode
        UpdatePullVersion(ActiveOutputItems);
      }
      else
      {
        // 'Loop mode'
        UpdateLoopVersion(ActiveOutputItems);
      }
    }


    #region Protected implemention functions

    protected virtual void UpdatePullVersion(ICollection<EngineOutputItem> requiredOutputItems)
    {
      if (Status == LinkableComponentStatus.Done)
      {
        // Component is already done, no action
        return;
      }

      if (Status == LinkableComponentStatus.Updating)
      {
        // Update call that was invoked by bidirectional link, no action
        return;
      }

      // indicate that we are starting to compute
      Status = LinkableComponentStatus.Updating;

      // gather input for all active input items
      ProcessActiveInputItems();

      // compute and produce output for output exchange items
      PerformTimestep(requiredOutputItems);

      // update active output items
      ProcessActiveOutputItems();

      // Assuming the time step was successfull, then we can update the time horizon 
      // start time of all input items, to indicate that we are never going to ask
      // data before this time. Done after the PerformTimestep in order to support 
      // redoing of time steps.
      foreach (EngineInputItem inputItem in ActiveInputItems)
      {
        // Assuming this input item is never (ever again) going to ask 
        // for data before the current inputTime
        inputItem.TimeSet.SetTimeHorizonStart(inputItem.TimeSet.Times[0]);
      }

      // indicate that Update is done
      Status = CurrentTime.StampAsModifiedJulianDay >= EndTime.StampAsModifiedJulianDay
                   ? LinkableComponentStatus.Done
                   : LinkableComponentStatus.Updated;
    }



    protected virtual void UpdateLoopVersion(ICollection<EngineOutputItem> requiredOutputItems)
    {
      Status = LinkableComponentStatus.Updating;

      foreach (ITimeSpaceInput inputItem in Inputs)
      {
        if (inputItem.Provider != null)
        {
          if (!ExchangeItemHelper.OutputAndInputFit((ITimeSpaceOutput)inputItem.Provider, inputItem))
          {
            Status = LinkableComponentStatus.WaitingForData;
            return;
          }
        }
      }

      // All data available, get all values
      // TODO JGr: Check if we can reuse the ProcessActiveInputItems();
      foreach (ITimeSpaceInput inputItem in Inputs)
      {
        if (inputItem.Provider != null)
        {
          // get and store input
          ITimeSpaceValueSet incomingValues = (ITimeSpaceValueSet)inputItem.Provider.Values;
          inputItem.Values = incomingValues;
        }
      }

      // compute and produce output
      PerformTimestep(requiredOutputItems);

      ProcessActiveOutputItems();

      // Assuming the time step was successfull, then we can update the time horizon 
      // start time of all input items, to indicate that we are never going to ask
      // data before this time. Done here in order to support redoing of time steps
      // to the horizon start time should only be updated on a successfull performTimestep
      foreach (EngineInputItem inputItem in ActiveInputItems)
      {
        // Assuming this input item is never (ever again) going to ask 
        // for data before the current inputTime
        inputItem.TimeSet.SetTimeHorizonStart(inputItem.TimeSet.Times[0]);
      }

      // indicate that Update is done
      Status = CurrentTime.StampAsModifiedJulianDay >= EndTime.StampAsModifiedJulianDay
                   ? LinkableComponentStatus.Done
                   : LinkableComponentStatus.Updated;
    }

    protected virtual void ProcessActiveInputItems()
    {
      foreach (EngineInputItem inputItem in ActiveInputItems)
      {
        inputItem.Update();
      }

      // Set values to engine for those not yet processed
      foreach (EngineInputItem inputItem in InputItemsToBeProcessed)
      {
        if (!inputItem.HasBeenProcessed)
        {
          inputItem.SetValuesToEngine((ITimeSpaceValueSet)inputItem.Values);
          inputItem.HasBeenProcessed = true;
        }
      }
      // TODO (JGr): Consider what should happen when a timestep is to be reperformed (with a smaller timestep)
      // then we may loose an input. Should it be cleared first at successfull end of PerformTimestep?
      InputItemsToBeProcessed.Clear();
    }


    protected virtual void ProcessActiveOutputItems()
    {
      ICollection<EngineOutputItem> requiredOutputItems = ActiveOutputItems;
      foreach (EngineOutputItem outputItem in requiredOutputItems)
      {
        if (outputItem.Update())
        {
          // if updated, also update all adapted outputs
          foreach (ITimeSpaceAdaptedOutput adaptedOutput in outputItem.AdaptedOutputs)
          {
            // Only update adaptedOutputs that are actually active
            if (adaptedOutput.Consumers.Count > 0 || adaptedOutput.AdaptedOutputs.Count > 0)
              adaptedOutput.Refresh();
          }
        }
      }

    }

    #endregion

  }



}
