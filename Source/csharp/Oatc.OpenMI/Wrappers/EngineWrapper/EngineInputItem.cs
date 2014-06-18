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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper
{

    /// <summary>
    /// <para>
    /// An abstract <see cref="ITimeSpaceInput"/> having special functionality for
    /// supporting a time stepping engine, the <see cref="LinkableEngine"/>
    /// </para>
    /// <para>
    /// It automatically updates the <see cref="LinkableEngine.ActiveInputItems"/>
    /// and has support for storing values in the exchange item, and handling time
    /// as stamps as well as spans.
    /// </para>
    /// <remarks>
    /// It extends the <see cref="InputItem"/> mostly to
    /// get the <see cref="ExchangeItem"/> functionality
    /// </remarks>
    /// </summary>
    public abstract class EngineInputItem : Input    {
        protected readonly LinkableEngine _linkableEngine;
        protected bool _storeValuesInExchangeItem;

        /// <summary>
        /// Boolean working in conjunction with <see cref="LinkableEngine.InputItemsToBeProcessed"/>.
        /// See details there.
        /// </summary>
        internal protected bool HasBeenProcessed;

        protected EngineInputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet,
                               LinkableEngine linkableEngine)
            : base(id, valueDefinition, elementSet)
        {
            _linkableEngine = linkableEngine;
            _component = linkableEngine;
            _timeSet = new TimeSet();
            _storeValuesInExchangeItem = linkableEngine.DefaultForStoringValuesInExchangeItem;
        }

        /// <summary>
        /// Boolean specifying whether values are stored in the exchange item, or
        /// whether values are written directly to the engine.
        /// <para>
        /// The values should be stored in the exchange item whenever
        /// the value is adding/subtracting/doing some operation in the engine:
        /// If the values are set once, then modified and set again
        /// the contribution should not be added more than once, which will be
        /// the case when setting this to true.
        /// </para>
        /// <para>
        /// If the item is getting/setting a value directly in the engine, 
        /// there is no need to store the values in the exchange item.
        /// </para>
        /// </summary>
        public virtual bool StoreValuesInExchangeItem
        {
            get { return _storeValuesInExchangeItem; }
            set { _storeValuesInExchangeItem = value; }
        }

        /// <summary>
        /// Returns the input time of the current input item. Can return null, 
        /// in that case the input item will not be updated.
        /// </summary>
        /// <returns>Input time for current time step</returns>
        internal protected virtual ITime GetInputTime()
        {
            return _linkableEngine.GetInputTime(!TimeSet.HasDurations);  
        }
 
        /// <summary>
        /// Overrides the <see cref="Input.Provider"/>, updating also
        /// the list <see cref="LinkableEngine.ActiveInputItems"/>
        /// </summary>
        public override IBaseOutput Provider
        {
            get { return _provider; }
            set
            {
                ITimeSpaceOutput newProvider = value as ITimeSpaceOutput;
                if (value != null && newProvider == null)
                    throw new ArgumentException("Value must be an ITimeSpaceOutput - add an adaptor");

                if (_provider == null && value != null)
                {
                    // this item goes from being inactive to being active,
                    // hence add it to the list of active input items in the LinkableEngine
                    _linkableEngine.ActiveInputItems.Add(this);
                }
                else if (_provider != null && value == null)
                {
                    // this item goes from being active to being inactive,
                    // hence remove it from the list of active input items in the LinkableEngine
                    _linkableEngine.ActiveInputItems.Remove(this);
                }
                _provider = newProvider;
            }
        }

        public override IBaseValueSet Values
        {
            get
            {
                if (_storeValuesInExchangeItem)
                    return _values;
                return (GetValuesFromEngine());
            }
            set
            {
                if (value != null && !(value is ITimeSpaceValueSet))
                    throw new ArgumentException("Values must be ITimeSpaceValueSet - you may need to add an adaptor");
                if (_storeValuesInExchangeItem)
                {
                    _values = (ITimeSpaceValueSet)value;
                    _linkableEngine.InputItemsToBeProcessed.Add(this);
                    HasBeenProcessed = false;
                }
                else
                {
                    SetValuesToEngine((ITimeSpaceValueSet)value);
                }
            }
        }

        /// <summary>
        /// Updates the input item, retrieves values from the provider and
        /// sets the values to the engine. 
        /// <para>
        /// Is called just before <see cref="LinkableEngine.PerformTimestep"/>
        /// </para>
        /// <para>
        /// The input time is retrieved from <see cref="GetInputTime"/>. Updating
        /// this input can be "disabled" by letting <see cref="GetInputTime"/>
        /// return null.
        /// </para>
        /// </summary>
        protected internal virtual void Update()
        {
            if (_provider != null)
            {
                ITime inputTime = GetInputTime();
                if (inputTime == null)
                    return; // can not update input item if time is null

                TimeSet.SetSingleTime(inputTime);

                ITimeSpaceValueSet incomingValues = _provider.GetValues(this);
                if (_storeValuesInExchangeItem)
                {
                    _values = incomingValues;
                    _linkableEngine.InputItemsToBeProcessed.Add(this);
                    HasBeenProcessed = false;
                }
                else
                {
                    // Here we do not add this input item to the list of InputItemsToBeProcessed, we process immediately
                    SetValuesToEngine(incomingValues);
                    HasBeenProcessed = true;
                }
            }
            else
            {
                throw new Exception("Trying to update an input item without a provider.");
            }
        }

        /// <summary>
        /// Function to get current values from the engine. This needs not be supported, 
        /// then an exception must be thrown.
        /// </summary>
        public abstract ITimeSpaceValueSet GetValuesFromEngine();

        /// <summary>
        /// Function to set the given values to the engine. It is
        /// assumed that the size of the input values have been checked
        /// before this function is called.
        /// </summary>
        public abstract void SetValuesToEngine(ITimeSpaceValueSet values);

    }

    /// <summary>
    /// A wrapper around an <see cref="EngineInputItem"/> that provides functionality
    /// for handling multiple providers for the input.
    /// <para>
    /// The contribution of each provider will be added together.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Most properties and methods in this class is delegated to the wrapped input item.
    /// </remarks>
    public class EngineMultiInputItemWrapper : EngineInputItem, ITimeSpaceMultiInput
    {
        private EngineInputItem _input;

        public EngineMultiInputItemWrapper(EngineInputItem input, LinkableEngine engine) 
            : base(input.Id, input.ValueDefinition, input.ElementSet(), engine)
        {
            _input = input;
        }

        public override ITimeSpaceValueSet GetValuesFromEngine()
        {
            return (_input.GetValuesFromEngine());
        }

        public override void SetValuesToEngine(ITimeSpaceValueSet values)
        {
            _input.SetValuesToEngine(values);
        }

        List<ITimeSpaceOutput> _providers = new List<ITimeSpaceOutput>();

        public override IBaseOutput Provider
        {
            get
            {
                if (_providers.Count == 0)
                    return (null);
                return _providers[0];
            }
            set
            {
                if (value == null)
                {
                    // Only remove if there is exactly one provider,
                    // providing same funtionality as the ordinary
                    // EngineInputItem in case it is used as such.
                    if (_providers.Count == 1)
                        RemoveProvider(Provider);
                }
                else
                    AddProvider(value);
            }
        }
      
        public IList<IBaseOutput> Providers
        {
            get { return(new ListWrapper<ITimeSpaceOutput,IBaseOutput>(_providers)); }
        }

        public void AddProvider(IBaseOutput provider)
        {
            // TODO: Check if provider is valid (must be ITimeSpaceOutput)
            ITimeSpaceOutput timeSpaceprovider = (ITimeSpaceOutput) provider;
            if (!_providers.Contains(timeSpaceprovider))
            {
                _providers.Add(timeSpaceprovider);
                if (_providers.Count == 1)
                    _linkableEngine.ActiveInputItems.Add(this);
            }

        }

        public void RemoveProvider(IBaseOutput provider)
        {
            // TODO: Check if provider is valid (must be ITimeSpaceOutput)
            ITimeSpaceOutput timeSpaceprovider = (ITimeSpaceOutput)provider;
            _providers.Remove(timeSpaceprovider);
            if (_providers.Count == 0)
                _linkableEngine.ActiveInputItems.Remove(this);
        }

        protected internal override ITime GetInputTime()
        {
            return _input.GetInputTime();
        }

        public override ITimeSet TimeSet
        {
            get { return _input.TimeSet; }
            set { _input.TimeSet = value; }
        }

        public override ISpatialDefinition SpatialDefinition
        {
            get { return _input.SpatialDefinition; }
            set { _input.SpatialDefinition = value; }
        }

        public override IValueDefinition ValueDefinition
        {
            get { return _input.ValueDefinition; }
            set { _input.ValueDefinition = value; }
        }

        public override IBaseValueSet Values
        {
            get { return _input.Values; }
            set { _input.Values = value; }
        }

        public override bool StoreValuesInExchangeItem
        {
            get { return _input.StoreValuesInExchangeItem; }
            set { _input.StoreValuesInExchangeItem = value; }
        }

        protected internal override void Update()
        {
            if (_providers.Count > 0)
            {
                ITime inputTime = GetInputTime();
                if (inputTime == null)
                    return;

                TimeSet.SetSingleTime(inputTime);

                // the set of values that is set to the engine
                ITimeSpaceValueSet values = null;

                // Loop through all providers
                for (int i = 0; i < _providers.Count; i++)
                {
                    ITimeSpaceOutput provider = _providers[i];
                    
                    // Create the values variable for the first provider
                    // and add the result from the remaining providers to this
                    if (i == 0)
                    {
                        values = provider.GetValues(this);
                    }
                    else
                    {
                        ITimeSpaceValueSet addValues = provider.GetValues(this);

                        // Add the addValues to the values
                        int times = values.TimesCount();
                        int elmts = values.ElementCount();

                        if (addValues.TimesCount() != times ||
                            addValues.ElementCount() != elmts)
                            throw new Exception("Size of inputs differs, valuesets can not be added");

                        for (int j = 0; j < times; j++)
                        {
                            for (int k = 0; k < elmts; k++)
                            {
                                // would be really nice if the value set was templated (to avoid casting)
                                values.Values2D[j][k] = ((double)values.Values2D[j][k]) + ((double)addValues.Values2D[j][k]);
                            }
                        }
                    }
                }
                ITimeSpaceValueSet incomingValues = values;
                
                
                if (StoreValuesInExchangeItem)
                {
                    _values = incomingValues;
                    _linkableEngine.InputItemsToBeProcessed.Add(this);
                    HasBeenProcessed = false;
                }
                else
                {
                    // Here we do not add this input item to the list of InputItemsToBeProcessed, we process immediately
                    SetValuesToEngine(incomingValues);
                    HasBeenProcessed = true;
                }
            }
            else
            {
                throw new Exception("Trying to update an input item without a provider.");
            }
            
        }
    }


}
