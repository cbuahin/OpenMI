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
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// An adapted output that does element mapping from one spatial definition 
  /// (elementset) to another.
  /// <para>
  /// The adapted output retrieves values from the adaptee, and converts them
  /// to the target element set that was provided in the constructor.
  /// </para>
  /// <para>
  /// The <see cref="SpatialDefinition"/> returned by this adapted output
  /// is the target element set. Most of the other properties are inherited
  /// from the adaptee, like the <see cref="ValueDefinition"/>.
  /// </para>
  /// </summary>
  public class ElementMapperAdaptedOutput : AbstractAdaptedOutput, ITimeSpaceOutputAdder
  {
    private readonly ElementMapper _elementMapper;
    private readonly IIdentifiable _methodId;
    private readonly IElementSet _target;

    /// <summary>
    /// Query input, used when calling getValues on adaptee, i.e.
    /// <code>_adaptee.GetValues(_query)</code>
    /// instead of 
    /// <code>_adaptee.GetValues(this)</code>
    /// </summary>
    private readonly Input _query;

    public ElementMapperAdaptedOutput(IIdentifiable methodId, ITimeSpaceOutput adaptee, IElementSet target)
      : base(adaptee.Id + "->" + methodId, adaptee)
    {
      _timeSet = new TimeSet();
      _methodId = methodId;
      _target = target;
      _elementMapper = new ElementMapper();
      IElementSet set = adaptee.ElementSet();
      _elementMapper.Initialise(ref _methodId,ref set,ref target);

      // The query item must match the adaptee on every point but 
      // the timeset.
      _query = new Input(_id)
                 {
                   Caption = _caption,
                   Description = _description,
                   ValueDefinition = adaptee.ValueDefinition,
                   SpatialDefinition = adaptee.SpatialDefinition,
                   Component = adaptee.Component, // because this._component is not set yet.
                 };
    }

    public override IValueDefinition ValueDefinition
    {
      get { return Adaptee.ValueDefinition; }
    }

    public override ISpatialDefinition SpatialDefinition
    {
      get { return (_target); }
    }

    public override ITimeSpaceValueSet Values
    {
      get 
      {
          ITimeSpaceValueSet valueSet = _adaptee.Values;
          return _elementMapper.MapValues(ref valueSet); 
      }
    }

    public override ITimeSet TimeSet
    {
      get { return _adaptee.TimeSet; }
    }

    public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier2)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier2 as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      if (!(timeSpaceQuery is ITimeSpaceInput))
      {
        throw new OpenMIException("Get Values can only be called with an Input as argument") { Component = Adaptee.Component, Output = this };
      }
      

      /// Using a _query item in the 
      ///     _adaptee.GetValues(_query) 
      /// instead of 
      ///     _adaptee.GetValues(this)
      /// 
      /// Reason: 
      /// If using the latter, it would be required to set the internal 
      ///     TimeSet = timeSpaceQuery.TimeSet;
      /// but then internal Values (retrieved from adaptee) does not match TimeSet. 
      /// And also:
      /// - the TimeSet is set to override and return _adaptee.TimeSet
      /// - the spatial definition would not match the adaptee

      // If uncommenting this, comment out also the overidden TimeSet property above
      //TimeSet = timeSpaceQuery.TimeSet;

      // Set query time to internal query item
      _query.TimeSet = timeSpaceQuery.TimeSet;

      ITimeSpaceValueSet incomingValues = _adaptee.GetValues(_query);
      ITimeSpaceValueSet<double> resultValues = ElementMapper.CreateResultValueSet(incomingValues.TimesCount(), SpatialDefinition.ElementCount);

      // Transform the values from the adaptee
      _elementMapper.MapValues(ref resultValues,ref incomingValues);

      return resultValues;
    }

    /// <summary>
    /// Compared to <see cref="GetValues(IBaseExchangeItem)"/>, 
    /// this version adds the values to the targetSet (for reuse or adding to a targetSet)
    /// </summary>
    public void GetValues(ITimeSpaceValueSet<double> targetSet, IBaseExchangeItem querySpecifier2)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier2 as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      if (!(timeSpaceQuery is ITimeSpaceInput))
      {
        throw new OpenMIException("Get Values can only be called with an Input as argument") { Component = Adaptee.Component, Output = this };
      }

      // Set query time to internal query item
      _query.TimeSet = timeSpaceQuery.TimeSet;

      ITimeSpaceValueSet incomingValues = _adaptee.GetValues(_query);

      // Transform the values from the adaptee
      _elementMapper.MapValues(ref targetSet,ref incomingValues);
    }


    public override void Refresh()
    {
      foreach (ITimeSpaceAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }
  }
}