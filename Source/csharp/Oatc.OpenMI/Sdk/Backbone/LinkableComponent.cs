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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// The LinkableComponent provides the OpenMI interface to the wrapped engine.
  /// <para>This is a trivial implementation of OpenMI.Standard.ILinkableComponent, refer there for further details.</para>
  /// </summary>
  public abstract class LinkableComponent : MarshalByRefObject, ITimeSpaceComponent
  {
    #region Implementation of IIdentifiable / IDescribable

    private string _id;
    private string _caption;
    private string _description;

    public string Id
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
        if (_caption == null)
        {
          _caption = _id;
        }
        if (_description == null)
        {
          _description = _id;
        }
      }
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

    #endregion

    #region Implementation of ILinkableComponent

    private LinkableComponentStatus _status;
    protected TimeSet _timeExtent;

    protected LinkableComponent()
    {
      _id = "<not set>";
      Status = LinkableComponentStatus.Created;
    }

    public event EventHandler<LinkableComponentStatusChangeEventArgs> StatusChanged;

    public IList<IArgument> Arguments { get; set; }

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

    public virtual ITimeSet TimeExtent
    {
      get { return _timeExtent; }
    }

    public bool CascadingUpdateCallsDisabled { get; set; }

    #region Abstract members, to be implemented by derived classes
    #endregion

    public abstract IList<IBaseInput> Inputs { get; }
    public abstract IList<IBaseOutput> Outputs { get; }

    public abstract List<IAdaptedOutputFactory> AdaptedOutputFactories { get; }

    public abstract void Initialize();

    public abstract string[] Validate();
    public abstract void Prepare();

    public abstract void Update(params IBaseOutput[] requiredOutput);
    public abstract void Finish();

    #endregion

    #region Protected convenience functions for derivers

    protected void GetInstanceIdAndCaption(IEnumerable<IArgument> arguments)
    {
      foreach (IArgument argument in arguments)
      {
        if (argument.Caption.ToLower().Equals("id"))
        {
          Id = (string)argument.Value;
        }
        else if (argument.Caption.ToLower().Equals("caption"))
        {
          Caption = (string)argument.Value;
        }
        else if (argument.Caption.ToLower().Equals("description"))
        {
          Description = (string)argument.Value;
        }
      }
    }

    #endregion
  }
}

