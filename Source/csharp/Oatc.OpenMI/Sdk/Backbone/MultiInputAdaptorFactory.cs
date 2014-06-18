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
using Oatc.OpenMI.Sdk.Backbone.Generic;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// Common interface for adaptors that the <see cref="MultiInputAdaptor"/> can utilize
  /// </summary>
  /// <remarks>
  /// this interface is required in order to use the "optimized" version of the
  /// element mapping (reusing of target elementset).
  /// </remarks>
  public interface ITimeSpaceOutputAdder : ITimeSpaceOutput
  {
    ///<summary>
    /// Version of <see cref="ITimeSpaceOutput.GetValues"/> where the result instead
    /// is added to the <paramref name="targetSet"/>, instead of returning a new 
    /// <see cref="ITimeSpaceValueSet"/>.
    /// <para>
    /// It is assumed the the <paramref name="targetSet"/> has the correct size, matching
    /// the <paramref name="querySpecifier"/>
    /// </para>
    /// </summary>
    void GetValues(ITimeSpaceValueSet<double> targetSet, IBaseExchangeItem querySpecifier);
  }

  /// <summary>
  /// A factory for creating adapted outputs that connects to the same input. See
  /// <see cref="MultiInputAdaptor"/> for details.
  /// <para>
  /// It also includes all the spatial mappings of the <see cref="SpatialAdaptedOutputFactory"/>
  /// </para>
  /// </summary>
  public class MultiInputAdaptorFactory : IAdaptedOutputFactory
  {
    private readonly IBaseLinkableComponent _component;

    /// <summary>
    /// List of already created <see cref="MultiInputAdaptor"/>s and which <see cref="IBaseInput"/> it adapts.
    /// </summary>
    private readonly Dictionary<IBaseInput, MultiInputAdaptor> _existingMultiInputAdaptors = new Dictionary<IBaseInput, MultiInputAdaptor>();

    /// <summary>
    /// Id of the identity adaptor, i.e. the adaptor that does nothing to data.
    /// </summary>
    private static readonly Identifier _identityAdaptor =
        new Identifier("IdentityAdaptedOutput")
            {
              Caption = "Identity adapted output",
              Description = "An adapted output that can be used when the source and the target element set exactly matches"
            };

    public MultiInputAdaptorFactory(IBaseLinkableComponent component)
    {
      _component = component;
      Id = "MultiInputAdaptorFactory";
      Caption = "MultiInputAdaptorFactory";
      Description = "A factory for creating adapted outputs, when connecting more than one output to a single input";

    }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string Id { get; private set; }

    /// <summary>
    /// A filter that will only return available adaptors, if this methods returns true.
    /// By default this is null, meaning that it allows all combinations of inputs and outputs.
    /// <para>
    /// Example, to only allow multi input for one input with id SomeId (using lambda notation):
    /// <code>
    ///    Filter = (adaptee, target) => String.Equals(adaptee.Id, SomeId);
    /// </code>
    /// </para>
    /// </summary>
    public Func<IBaseOutput, IBaseInput, bool> Filter;

    /// <summary>
    /// List of already created <see cref="MultiInputAdaptor"/>s and which <see cref="IBaseInput"/> it adapts.
    /// </summary>
    public Dictionary<IBaseInput, MultiInputAdaptor> ExistingMultiInputAdaptors
    {
      get { return _existingMultiInputAdaptors; }
    }

    public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
    {
      // Target must be this component!
      if (target == null || target.Component != _component)
        return (new IIdentifiable[0]);

      // Check for filter
      if (Filter != null && !Filter(adaptee, target))
        return (new IIdentifiable[0]);

      List<IIdentifiable> res = new List<IIdentifiable>();

      res.Add(_identityAdaptor);

      return (res.ToArray());
    }

    /// <summary>
    /// Check if the provided id was created by/can be used by this factory
    /// </summary>
    /// <param name="identifiable">Id to check</param>
    /// <returns>True of identifier can be used with this factory.</returns>
    public static bool HasId(IIdentifiable identifiable)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(_identityAdaptor.Id, identifiable.Id))
      {
        return true;
      }
      return false;
    }


    public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
    {
      MultiInputAdaptor multiInputAdaptor = null;
      
      if (adaptedOutputId == _identityAdaptor)
      {
        // If a MultiInputAdaptor already exists for the target, get that one, otherwise create a new.
        return CreateAdaptedOutput(adaptee, target);
      }

      if (multiInputAdaptor == null)
        throw new ArgumentException("Adapted output id could not be found", "adaptedOutputId");

      return (multiInputAdaptor);
    }

    /// <summary>
    /// Create an adapted output, assuming that the adaptee and target are matching (check must
    /// be done beforehand.
    /// </summary>
    /// <param name="adaptee">Adaptee to connect to multi input</param>
    /// <param name="target">Target of created multi input adaptor</param>
    /// <returns>Multi input adaptor, connected to adaptee</returns>
    public ITimeSpaceAdaptedOutput CreateAdaptedOutput(IBaseOutput adaptee, IBaseInput target)
    {
      MultiInputAdaptor multiInputAdaptor;
      if (!ExistingMultiInputAdaptors.TryGetValue(target, out multiInputAdaptor))
      {
        // Set up a multi input adaptor having the same properties as the target
        multiInputAdaptor = new MultiInputAdaptor("MultiInputAdaptor:" + target.Id)
                              {
                                SpatialDefinition = ((ITimeSpaceInput)target).SpatialDefinition,
                                ValueDefinition = target.ValueDefinition,
                              };
        ExistingMultiInputAdaptors.Add(target, multiInputAdaptor);
      }

      // Identity adaptor, create and register with multiInput
      //adaptedOutput = multiInputAdaptor.CreateChildAdaptor(adaptee);

      // The trick here is to always return and use the same multi input adaptor.
      // for all adaptees.
      multiInputAdaptor.Adaptees.Add((ITimeSpaceOutput)adaptee);
      
      // Connect adaptee and adaptor - the factory must do this.
      if (!adaptee.AdaptedOutputs.Contains(multiInputAdaptor))
      {
        adaptee.AddAdaptedOutput(multiInputAdaptor);
      }
      return (multiInputAdaptor);
    }
  }


  /// <summary>
  /// A MultiInputAdaptor can connect more than one output to a single input.
  /// <para>
  /// The <see cref="Adaptee"/> is empty, instead is used <see cref="Adaptees"/> which
  /// can contain more than one adaptee. Any <see cref="ITimeSpaceOutput"/> can
  /// be used. 
  /// </para>
  /// <para>
  /// If one of the adaptees implement the <see cref="ITimeSpaceOutputAdder"/>,
  /// unecessary copying of data is avoided.
  /// </para>
  /// </summary>
  /// <remarks>
  /// Since this adapted output adapts more than one output, properties as <see cref="Component"/>,
  /// <see cref="Values"/> etc. are not well defined, and dummy values are returned.
  /// </remarks>
  public class MultiInputAdaptor : ITimeSpaceAdaptedOutput
  {
    // TODO (JG): Decide: Leave values, timeset and elementset at null, or some default value?

    private readonly string _id;

    public MultiInputAdaptor(string id)
    {
      _id = id;
      Description =
        "MultiInputAdaptor that supports more than one adaptee. I.e. " +
        "this adaptor can be connected to several adaptees (done by the " +
        "MultiInputAdaptorFactory).";
    }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string Id { get { return _id; } }

    public IValueDefinition ValueDefinition
    {
      // TODO (JG): Assume they all are the same, and use the first child?
      get;
      set;
    }

    public IBaseLinkableComponent Component
    {
      // TODO (JG): More than one component, no simple answer 
      get { return (null); }
    }

    public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

    private List<ITimeSpaceInput> _consumers;

    public virtual IList<IBaseInput> Consumers
    {
      get
      {
        if (_consumers == null)
        {
          _consumers = new List<ITimeSpaceInput>();
        }
        return new ListWrapper<ITimeSpaceInput, IBaseInput>(_consumers.AsReadOnly());
      }
    }

    public virtual void AddConsumer(IBaseInput consumer)
    {
      ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
      if (timeSpaceConsumer == null)
        throw new ArgumentException("Must be a ITimeSpaceInput - may need to add adaptor");

      // Create list of consumers
      if (_consumers == null)
      {
        _consumers = new List<ITimeSpaceInput>();
      }

      // consumer should not be already added
      if (_consumers.Contains(timeSpaceConsumer))
      {
        //throw new Exception("consumer \"" + consumer.Caption +
        //    "\" has already been added to \"" + Caption);
      }
      else
      {
        _consumers.Add(timeSpaceConsumer);
        if (consumer.Provider != this)
        {
          consumer.Provider = this;
        }
      }
    }

    public virtual void RemoveConsumer(IBaseInput consumer)
    {
      ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
      if (timeSpaceConsumer == null || _consumers == null || !_consumers.Contains(timeSpaceConsumer))
      {
        throw new Exception("consumer \"" + consumer.Caption +
            "\" can not be removed from \"" + Caption +
            "\", because it was not added");
      }
      _consumers.Remove(timeSpaceConsumer);
      consumer.Provider = null;
    }

    readonly List<IBaseAdaptedOutput> _adaptedOutputs = new List<IBaseAdaptedOutput>();

    public IList<IBaseAdaptedOutput> AdaptedOutputs
    {
      get { return (_adaptedOutputs); }
    }

    public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      throw new NotImplementedException();
    }

    public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
    {
      throw new NotImplementedException();
    }

    public ITimeSpaceValueSet Values
    {
      // TODO (JG): decide what to return here.
      get { return new TimeSpaceValueSet<double>(); }
    }

    public ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
      if (timeSpaceQuery == null)
        throw new ArgumentException("Must be an ITimeSpaceExchangeItem, add an adaptor", "querySpecifier");

      if (_children.Count == 0)
        return (null); // TODO: Or throw an exception?

      TimeSpaceValueSet<double> resultSet = ElementMapper.CreateResultValueSet(timeSpaceQuery.TimeSet.Times.Count, this.ElementSet().ElementCount);

      // Get values from all adaptees/children, and add them together
      for (int i = 0; i < _children.Count; i++)
      {
        ITimeSpaceOutputAdder addingChild = _children[i] as ITimeSpaceOutputAdder;
        if (addingChild != null)
          addingChild.GetValues(resultSet, querySpecifier);
        else
        {
          ITimeSpaceValueSet values = _children[i].GetValues(querySpecifier);
          AddSourceToTarget(resultSet, values);
        }
      }
      return (resultSet);
    }

    IBaseValueSet IBaseOutput.Values
    {
      get { return Values; }
    }

    IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
    {
      return GetValues(querySpecifier);
    }

    public IList<IArgument> Arguments
    {
      get { return new List<IArgument>(); }
    }

    public void Initialize()
    {
    }

    public IBaseOutput Adaptee
    {
      // TODO: Nothing meaninfull to return, in case of more than one adaptee...
      get { return (null); }
    }

    private readonly List<ITimeSpaceOutput> _children = new List<ITimeSpaceOutput>();

    public IList<ITimeSpaceOutput> Adaptees
    {
      get { return (_children); }
    }


    public void Refresh()
    {
      /// Should probably not do anything. At least have to make sure
      /// that the Refresh call does not trigger an update of all other components
      /// (which in their turn will invoke refresh again)

      foreach (IBaseAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }

    public ITimeSet TimeSet
    {
      get;
      set;
    }

    public ISpatialDefinition SpatialDefinition
    {
      get;
      set;
    }

    /// <summary>
    /// Adds the content of the source values to the target values. Assumes the two
    /// are equally big.
    /// </summary>
    /// <param name="targetValues">Target set, where values are added to</param>
    /// <param name="sourceValues">Source set, containing values to add</param>
    private static void AddSourceToTarget(ITimeSpaceValueSet<double> targetValues, ITimeSpaceValueSet sourceValues)
    {
      for (int i = 0; i < targetValues.TimesCount(); i++)
      {
        double[] sourceTimeValues = sourceValues.GetElementValuesForTime<double>(i);
        IList<double> targetTimeValues = targetValues.Values2D[i];
        for (int j = 0; j < targetValues.ElementCount(); j++)
        {
          targetTimeValues[j] += sourceTimeValues[j];
        }
      }
    }

  }

}