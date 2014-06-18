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
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// A <see cref="IBaseAdaptedOutput"/> can register as an adapted output
  /// for more than one adaptee. Everytime yet another adaptee is to
  /// be added, use the <see cref="CreateChild"/> to create a <see cref="IBaseAdaptedOutput"/>
  /// that can be registered with the adaptee.
  /// <para>
  /// This is in itself an <see cref="IBaseOutput"/>, however the values
  /// of the <see cref="IBaseOutput.Values"/> are not well defined 
  /// (since it depends on more than one adaptee). It is therefore
  /// not possible to connect any type of adaptor to this adaptor, especially if they 
  /// are based on any of the above mentioned properties.
  /// </para>
  /// </summary>
  public interface IBaseMultiAdaptedOutput : IBaseOutput
  {
    /// <summary>
    /// Creates a child <see cref="IBaseAdaptedOutput"/> that can be used by the 
    /// <paramref name="adaptee"/> in the <see cref="IBaseOutput.AddAdaptedOutput"/>
    /// method.
    /// <para>
    /// The child wraps all methods of the mother <see cref="IBaseMultiAdaptedOutput"/>
    /// but has its own <see cref="IBaseAdaptedOutput.Adaptee"/> matching the 
    /// <paramref name="adaptee"/>.
    /// </para>
    /// </summary>
    /// <param name="adaptee">Output to register as an adaptor to</param>
    /// <returns>A child <see cref="IBaseAdaptedOutput"/> to be used by the <paramref name="adaptee"/></returns>
    IBaseAdaptedOutput CreateChild(IBaseOutput adaptee);

    /// <summary>
    /// Arguments needed to let the adapted output do its work. An unmodifiable
    /// list of the (modifiable) arguments should be returned that can be used to
    /// get info on the arguments and to modify argument values. Validation of changes
    /// is done when they occur (e.g. using notifications).
    /// 
    /// <returns>Unmodifiable list of IArgument for the adapted output</returns>
    /// </summary>
    IList<IArgument> Arguments { get; }

    /// <summary>
    /// Let the adapted output initialize itself, based on the current values
    /// specified by the arguments. Only after initialize is called the refresh
    /// method might be called.
    /// <para>
    /// A component must invoke the <see cref="Initialize()"/> method of all its
    /// adapted outputs at the end of the component's Prepare phase.
    /// In case of stacked adapted outputs, the adaptee must be initialized first.
    /// </para>
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Output item that this adaptedOutput extracts content from.
    /// In the adapter design pattern, it is the item being adapted.
    /// </summary>
    IList<IBaseOutput> Adaptees { get; }

    /// <summary>
    /// Request the adapted output to refresh itself. This method will be
    /// called by the adaptee, when it has been refreshed/updated. In the
    /// implementation of the refresh method the adapted output should
    /// update its contents according to the changes in the adaptee.
    /// <para>
    /// After updating itself the adapted output must call refresh on all
    /// its adapted outputs, so the chain of outputs refreshes itself.
    /// </para>
    /// </summary>
    void Refresh();
  
  }


}
