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
using log4net;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.UpwardsComp.Backbone;
using Oatc.UpwardsComp.Standard;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.UpwardsComp.EngineWrapper
{
  public abstract class LinkableEngine : LinkableComponent
  {
    protected static readonly ILog log = LogManager.GetLogger(typeof(OutputItem));

    /// <summary>
    /// Reference to the engine. Must be assigned in the derived class
    /// </summary>
    protected IEngine _engineApiAccess;

    /// <summary>
    /// True if the Initialize method was invoked
    /// </summary>
    private bool _initializeWasInvoked;

    /// <summary>
    /// True if the Prepare method was invoked
    /// </summary>
    internal bool _prepareWasInvoked;

    /// <summary>
    /// used when comparing time in the IsLater method (see property TimeEpsilon)
    /// </summary>
    private double _timeEpsilon; // used when comparing time in the IsLater method (see property TimeEpsilon)

    /// <summary>
    /// current time of engine
    /// </summary>
    private double _currentTime = Double.NaN;

    /// <summary>
    /// COnstructor
    /// </summary>
    protected LinkableEngine()
    {
      _initializeWasInvoked = false;
      _prepareWasInvoked = false;
      _timeEpsilon = 1.0 / (1000.0 * 3600.0 * 24.0); // 1/1000 second
    }

    /// <summary>
    /// This _timeEpsilon variable is used when comparing the current time in the engine with
    /// the time specified in the parameters for the GetValue method. 
    /// if ( requestedTime > engineTime + _timeEpsilon) then PerformTimestep()..
    /// The default values for _timeEpsilon is double.Epsilon = 4.94065645841247E-324
    /// The default value may be too small for some engines, in which case the _timeEpsilon can
    /// be changed the class that you have inherited from LinkableRunEngine og LinkableEngine.
    /// </summary>
    public double TimeEpsilon
    {
      get { return _timeEpsilon; }
      set { _timeEpsilon = value; }
    }


    public string ComponentID
    {
      get { return _engineApiAccess.GetComponentID(); }
    }

    public string ComponentDescription
    {
      get { return _engineApiAccess.GetComponentDescription(); }
    }

    public string InstanceID
    {
      get { return _engineApiAccess.GetModelID(); }
    }

    internal IEngine EngineApiAccess
    {
      get { return _engineApiAccess; }
    }

    /// <summary>
    /// Set reference to the engine
    /// </summary>
    protected abstract void SetEngineApiAccess();

    public override void Initialize()
    {
      Hashtable hashtable = new Hashtable();
      for (int i = 0; i < Arguments.Count; i++)
      {
        hashtable.Add(Arguments[i].Id, Arguments[i].Value);
      }

      SetEngineApiAccess();
      if (_engineApiAccess == null)
      {
        throw new Exception("Failed to assign the engine");
      }

      _engineApiAccess.Initialize(hashtable);

      for (int i = 0; i < _engineApiAccess.GetInputExchangeItemCount(); i++)
      {
        // TODO: How important is the 
        IQuantity quantity = _engineApiAccess.GetInputExchangeItem(i).Quantity;
        IElementSet elementSet = _engineApiAccess.GetInputExchangeItem(i).ElementSet;
        String inputItemID = elementSet.Caption + "." + quantity.Caption;
        InputItem inputItem = new InputItem(_engineApiAccess, inputItemID);
        inputItem.ValueDefinition = quantity;
        inputItem.SpatialDefinition = elementSet;
        Inputs.Add(inputItem);
      }

      for (int i = 0; i < _engineApiAccess.GetOutputExchangeItemCount(); i++)
      {
        IQuantity quantity = _engineApiAccess.GetOutputExchangeItem(i).Quantity;
        IElementSet elementSet = _engineApiAccess.GetOutputExchangeItem(i).ElementSet;
        String outputItemID = elementSet.Caption + "." + quantity.Caption;
        OutputItem outputItem = new OutputItem(_engineApiAccess, outputItemID);
        outputItem.ValueDefinition = quantity;
        outputItem.SpatialDefinition = elementSet;
        Outputs.Add(outputItem);
      }

      foreach (InputItem inputItem in Inputs)
      {
        TimeSet timeset = new TimeSet();
        timeset.Times.Add(TimeHelper.ConvertTime(_engineApiAccess.GetCurrentTime()));
        inputItem.TimeSet = timeset;
        // TODO inputItem.TimeSet = TimeSet for input item;
      }
      foreach (OutputItem outputItem in Outputs)
      {
        TimeSet timeset = new TimeSet();
        timeset.Times.Add(TimeHelper.ConvertTime(_engineApiAccess.GetCurrentTime()));
        outputItem.TimeSet = timeset;
      }

      _initializeWasInvoked = true;

      Status = LinkableComponentStatus.Initialized;
    }

    public override string[] Validate()
    {
      if (!_initializeWasInvoked)
      {
        throw new Exception(
          "Validate method in the LinkableEngine cannot be invoked before the Initialize method has been invoked");
      }
      return new string[0]; // TODO: implement validation.
    }

    public override void Prepare()
    {
      try
      {
        if (!_initializeWasInvoked)
        {
          throw new Exception(
            "Prepare method in the LinkableEngine cannot be invoked before the Initialize method has been invoked");
        }

        // Remove ouput items without consumers since these are not used
        for (int i = Outputs.Count - 1; i >= 0; i--)
        {
          if (Outputs[i].Consumers.Count == 0)
          {
            Outputs.RemoveAt(i);
          }
        }

        // Remove input items without providers since these are not used.
        for (int i = Outputs.Count - 1; i >= 0; i--)
        {
          if (Inputs[i].Provider == null)
          {
            Inputs.RemoveAt(i);
          }
        }

        Validate();

        _prepareWasInvoked = true;
      }
      catch (Exception e)
      {
        string message = "Exception in LinkableComponent. ";
        message += "ComponentID: " + ComponentID + "\n";
        throw new Exception(message, e);
      }
    }

    public void Dispose()
    {
      _engineApiAccess.Dispose();
    }

    public override void Finish()
    {
      _engineApiAccess.Finish();
    }

    /// <summary>
    /// The linkable component that gets the update call should be the only linkable component in a composition that
    /// has no output items. For that reason the composition will be able to automatically detect which model is the 
    /// startup model. If there are more models without output items in a composition the composition is invalid.
    /// </summary>
    /// <param name="requiredOutputItems">Output Items to do the update for</param>
    public override void Update(params IBaseOutput[] requiredOutputItems)
    {
      if (Status == LinkableComponentStatus.Updating)
      {
        // Update call that was invoked by bidirectional link
        return;
      }

      Status = LinkableComponentStatus.Updating;

      foreach (ITimeSpaceInput inputItem in Inputs)
      {
        if (inputItem.Provider != null)
        {
          // get and store input
          ITimeSpaceValueSet providedValues = (ITimeSpaceValueSet)inputItem.Provider.GetValues(inputItem);
          LogIncomingValues(inputItem, providedValues);
          inputItem.Values = providedValues;
        }
      }

      // compute output / store in output / increment time stored in output
      foreach (OutputItem outputItem in Outputs)
      {
        // TODO get values from Engine
        // TODO outputItem.TimeSet = TimeSet for output item;
      }

      // check if more time stamps need to be done
      _currentTime = ((ITimeStamp)_engineApiAccess.GetCurrentTime()).ModifiedJulianDay;
      if (_currentTime >= _engineApiAccess.GetTimeHorizon().End.ModifiedJulianDay)
      {
            Status = LinkableComponentStatus.Done;
            return;
      }
      Status = LinkableComponentStatus.Updated;
    }

    private void LogIncomingValues(ITimeSpaceInput inputItem, ITimeSpaceValueSet providedValues)
    {
      string message = Caption + " " + inputItem.Caption + " values:";
      foreach (IList valuesArray in providedValues.Values2D)
      {
          foreach (object value in valuesArray)
          {
              message += " " + value;
          }
      }
      message += " <= " + inputItem.Provider.Caption + " from " + inputItem.Provider.Component.Caption +
                 ")";
      log.Info(message);
    }
  }
}