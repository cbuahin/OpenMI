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
using System.Diagnostics;
using System.IO;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
  /// <summary>
  /// Base class for many ILinkableComponent implementations
  /// 
  /// User implements virtual Engine??? methods as required
  /// otherwise default implementation makes sence in many ocasions.
  /// Overridden methods are allowed to throw, this class deals 
  /// correctly with resulting exception and Status issues.
  /// 
  /// Technically need not be abstract, but force implementer to think
  /// </summary>
  public abstract class LinkableComponent : ITimeSpaceComponent, IDisposable
  {
    Identifier _identifier;
    LinkableComponentStatus _status
      = LinkableComponentStatus.Created;
    bool _pullDrivenOnly = true;
    Dictionary<string, IArgument> _args;
    DirectoryInfo _omiPath = null; // added by UI or null
    ITimeSet _timeExtent;

    protected Describable _describes;
    protected List<IArgument> _arguments
      = new List<IArgument>();
    protected List<IAdaptedOutputFactory> _factories
      = new List<IAdaptedOutputFactory>();

    List<IBaseInput> _targets = new List<IBaseInput>();
    List<IBaseOutput> _sources = new List<IBaseOutput>();
    List<IBaseInput> _activeTargets;
    List<IBaseOutput> _activeSources;

    public event EventHandler<LinkableComponentStatusChangeEventArgs> StatusChanged;

    public ITimeSet TimeExtent
    {
      get { return _timeExtent; }
      set { _timeExtent = value; }
    }

    public List<IBaseInput> Targets
    {
      get { return _targets; }
      set { _targets = value; }
    }

    public List<IBaseOutput> Sources
    {
      get { return _sources; }
      set { _sources = value; }
    }

    public List<IBaseInput> ActiveTargets
    {
      get
      {
        if (_activeTargets == null)
        {
          _activeTargets = new List<IBaseInput>(_targets.Count);

          foreach (ITimeSpaceInput item in _targets)
            if (item.Provider != null)
              _activeTargets.Add(item);
        }

        return _activeTargets;
      }
    }

    public List<IBaseOutput> ActiveSources
    {
      get
      {
        if (_activeSources == null)
        {
          _activeSources = new List<IBaseOutput>(_sources.Count);

          foreach (ITimeSpaceOutput item in _sources)
            if (item.Consumers != null && item.Consumers.Count > 0)
              _activeSources.Add(item);
        }

        return _activeSources;
      }
    }


    #region protected

    protected LinkableComponent(string id)
    {
      _identifier = new Identifier(id);
      _describes = new Describable(id);
    }

    protected object GetArgumentValueFirstCaptionMatch(string caption)
    {
      if (_args == null)
      {
        _args = new Dictionary<string, IArgument>();

        foreach (IArgument iArg in _arguments)
          if (!_args.ContainsKey(iArg.Caption))
            _args.Add(iArg.Caption, iArg);
      }

      if (!_args.ContainsKey(caption))
        throw new ArgumentNullException(caption);

      return _args[caption].Value;
    }

    protected FileInfo GetFirstCaptionMatchArgumentValueAsRootedFileInfo(string key)
    {
      return EnsureRooted(
        (FileInfo)GetArgumentValueFirstCaptionMatch(
        key), _omiPath);
    }

    #endregion protected

    #region static utilities TODO move to SDK

    public static IArgument ArgumentFileInfoValueParse(IArgument iArg, string value)
    {
      Debug.Assert(iArg.ValueType == typeof(FileInfo));

      IArgument arg = new ArgumentFileInfo(iArg)
                        {
                          Value = new FileInfo(value)
                        };
      return arg;
    }

    public static string ArgumentFileInfoValueSerialise(IArgument iArg)
    {
      Debug.Assert(iArg.ValueType == typeof(FileInfo));
      return ((FileInfo)iArg.Value).FullName;
    }

    public static FileInfo EnsureRooted(FileInfo maybeRooted, DirectoryInfo root)
    {
      if (Path.IsPathRooted(maybeRooted.FullName))
        return maybeRooted;

      if (root == null)
        throw new ArgumentNullException("root");

      return new FileInfo(Path.Combine(root.FullName, maybeRooted.FullName));
    }

    #endregion static utilities TODO move to SDK

    #region overidable

    /// <summary>
    /// INitialise engine for run start
    /// </summary>
    /// <param name="arguments">Arguments</param>
    /// <returns>New engine time</returns>
    protected virtual ITime EngineInitialize(List<IArgument> arguments)
    {
      return null;
    }

    protected virtual bool EngineValidate(out List<string> validationMessages)
    {
      validationMessages = new List<string>();
      validationMessages.Add("WARNING: No validation implemented, assumed OK");
      return true;
    }

    protected virtual bool EngineComputationCompleted()
    {
      return false;
    }

    protected virtual void EngineUpdateFromTargets()
    {
    }

    /// <summary>
    /// Perform an engine compution
    /// </summary>
    /// <returns>New engine time</returns>
    protected virtual ITime EngineCompute()
    {
      return null;
    }

    protected virtual void EngineUpdateSources()
    {
    }

    protected virtual void EngineFinish()
    {
    }

    #endregion overidable

    void CatchTidyRethrow(Exception e)
    {
      // TODO Communicate idea to OATC
      /* Exception Responsabilities
       * Any ILinkableComponent method can throw an exception of any type
       * Before final throw out of a ILinkableComponent method
       *  o Change Status to Failed
       *  o Notify any delegates of Status change
       *  o Throw
       *  o Does NOT call Finish, thats for the calling code as Finish
       *    could in turn throw an exception!
       */

      try
      {
        Status = LinkableComponentStatus.Failed;
      }
      catch
      {
      }

      throw e;
    }

    enum StateCalled { Initialize, Validate, Update, Finish, WaitingForData, };

    void ValidCallOrThrow(StateCalled called, LinkableComponentStatus[] allowed)
    {
      // TODO make LinkableComponentStatus [flags] & 1,2,4,8 etc ??

      foreach (LinkableComponentStatus allowable in allowed)
        if (_status == allowable)
          return;

      throw new InvalidOperationException(
          string.Format(
          "Component in state {0} when {1} called",
          _status, called));
    }

    #region ILinkableComponent Members

    public LinkableComponentStatus Status
    {
      get { return _status; }
      set
      {
        LinkableComponentStatus oldStatus = _status;

        _status = value;

        if (StatusChanged != null)
        {
          var args = new LinkableComponentStatusChangeEventArgs { LinkableComponent = this, OldStatus = oldStatus, NewStatus = value };
          StatusChanged(this, args);
        }
      }
    }

    public IList<IArgument> Arguments
    {
      get { return _arguments.ToArray(); }
    }

    public IList<IBaseInput> Inputs
    {
      get { return _targets; }
    }

    public IList<IBaseOutput> Outputs
    {
      get { return _sources; }
    }

    public void Initialize()
    {
      try
      {
        ValidCallOrThrow(StateCalled.Initialize,
          new LinkableComponentStatus[] { 
						LinkableComponentStatus.Created,
						LinkableComponentStatus.Done,
						LinkableComponentStatus.Failed,
					});

        Status = LinkableComponentStatus.Initializing;

        // Maybe UI or whatever was kind and added a OMI_PATH argument
        // so we can resove relative paths in other arguments

        try
        {
          _omiPath = (DirectoryInfo)GetArgumentValueFirstCaptionMatch(
            "OMI_PATH");
        }
        catch (ArgumentNullException)
        {
        }

        ITime iCurrentTime = EngineInitialize(_arguments);

        // Update TimeExtent so monitoring programs
        // can determine current engine time, usful for
        // progress monitoring.
        if (iCurrentTime != null && _timeExtent != null)
        {
          _timeExtent.Times.Clear();
          _timeExtent.Times.Add(iCurrentTime);
        }
        Status = LinkableComponentStatus.Initialized;
      }
      catch (Exception e)
      {
        CatchTidyRethrow(e);
      }
    }

    public string[] Validate()
    {
      try
      {
        ValidCallOrThrow(StateCalled.Validate,
          new LinkableComponentStatus[] { 
						LinkableComponentStatus.Initialized,
						LinkableComponentStatus.Updated,
						LinkableComponentStatus.WaitingForData,
						LinkableComponentStatus.Valid,
						LinkableComponentStatus.Invalid,
					});

        Status = LinkableComponentStatus.Validating;

        List<string> validationMessages;

        Status = EngineValidate(out validationMessages)
          ? LinkableComponentStatus.Valid
          : LinkableComponentStatus.Invalid;

        return validationMessages.ToArray();
      }
      catch (Exception e)
      {
        CatchTidyRethrow(e);
      }

      return null;
    }

    public void Prepare()
    {
      // TODO: Move code here?!
    }

    public void Update(params IBaseOutput[] requiredOutputItems)
    {
      try
      {
        ValidCallOrThrow(StateCalled.WaitingForData,
          new LinkableComponentStatus[] { 
						LinkableComponentStatus.Initialized,
						LinkableComponentStatus.Valid,
						LinkableComponentStatus.Updated,
					});

        if (EngineComputationCompleted())
        {
          /* TODO new state in Standard WaitingForFinish
           * Cant call finish here as might clear up resources
           * associated with output items buggers etc.
           * so just need to return and wait for external
           * controller to call finish
           * 
           * Desisable to have a new state
           * LinkableComponentStatus.WaitingForFinish
           * to make this clear in UI's etc.
           * 
           * For now will use Finishing
           */

          Status = LinkableComponentStatus.Finishing;
          return;
        }

        // TODO Note I prefer WaitingForData before Updating

        Status = LinkableComponentStatus.WaitingForData;

        EngineUpdateFromTargets();

        Status = LinkableComponentStatus.Updating;

        ITime iCurrentTime = EngineCompute();

        // Update TimeExtent so monitoring programs
        // can determine current engine time, usful for
        // progress monitoring.
        if (iCurrentTime != null && _timeExtent != null)
        {
          _timeExtent.Times.Clear();
          _timeExtent.Times.Add(iCurrentTime);
        }

        EngineUpdateSources();

        Status = LinkableComponentStatus.Updated;
      }
      catch (Exception e)
      {
        CatchTidyRethrow(e);
      }
    }

    public void Finish()
    {
      bool failed = Status == LinkableComponentStatus.Failed;

      try
      {
        ValidCallOrThrow(StateCalled.Finish,
          new LinkableComponentStatus[] { 
						LinkableComponentStatus.Initialized,
						LinkableComponentStatus.Valid,
						LinkableComponentStatus.Invalid,
						LinkableComponentStatus.Updated,
						LinkableComponentStatus.WaitingForData,
						LinkableComponentStatus.Failed,
						LinkableComponentStatus.Finishing, // TODO remove if we have WaitingForFinish
					});

        Status = LinkableComponentStatus.Finishing;
        EngineFinish();
        Status = LinkableComponentStatus.Done;
      }
      catch (Exception e)
      {
        CatchTidyRethrow(e);
      }
      finally
      {
        if (failed)
          Status = LinkableComponentStatus.Failed;
      }
    }

    public List<IAdaptedOutputFactory> AdaptedOutputFactories
    {
      get
      {
        return _factories;
      }
    }

    public bool CascadingUpdateCallsDisabled
    {
      get { return _pullDrivenOnly; }
      set { _pullDrivenOnly = value; }
    }

    #endregion

    #region IDescribable Members

    public string Caption
    {
      get { return _describes.Caption; }
      set { _describes.Caption = value; }
    }

    public string Description
    {
      get { return _describes.Description; }
      set { _describes.Description = value; }
    }

    #endregion

    #region IIdentifiable Members

    public string Id
    {
      get { return _identifier.Id; }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      if (Status != LinkableComponentStatus.Done)
        Finish();
    }

    #endregion
  }
}
