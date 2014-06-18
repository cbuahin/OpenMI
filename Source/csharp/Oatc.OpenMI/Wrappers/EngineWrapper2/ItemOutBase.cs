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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public abstract class ItemOutBase : ItemBase, ITimeSpaceOutput
    {
        List<ITimeSpaceInput> _consumers
            = new List<ITimeSpaceInput>();
        List<IBaseAdaptedOutput> _decorators;

        public abstract void ValidTimeSet(ITimeSet iTimeSet);
        public abstract void ValidValue(IList value);

        public abstract bool CacheUpdateSource(ITimeSpaceInput source, bool forceCacheUpdate);
        public abstract void CachePush(ITimeSet iTimeSet, IList value);
        public abstract IList<IList<double>> CacheGet();

        public ItemOutBase(string id)
            : base(id)
        { }

        #region IOutputItem Members

        public IList<IBaseInput> Consumers
        {
            get { return new ListWrapper<ITimeSpaceInput,IBaseInput>(_consumers); }
        }

        public void AddConsumer(IBaseInput consumer)
        {
            _consumers.Add((ITimeSpaceInput)consumer);

            if (consumer.Provider != this)
                consumer.Provider = this;
        }

        public void RemoveConsumer(IBaseInput consumer)
        {
            _consumers.Remove((ITimeSpaceInput)consumer);

            if (consumer.Provider == this)
                consumer.Provider = null;
        }

        public IList<IBaseAdaptedOutput> AdaptedOutputs
        {
            get
            {
                if (_decorators == null)
                {
                    _decorators = new List<IBaseAdaptedOutput>();
                }
                return _decorators;
            }
        }

        public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _decorators.Add(adaptedOutput);
        }

        public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _decorators.Remove(adaptedOutput);
        }

        IBaseValueSet IBaseOutput.Values
        {
            get { return Values; }
        }

        IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
        {
            return GetValues(querySpecifier);
        }

        public ITimeSpaceValueSet Values
        {
            get { return new TimeSpaceValueSet<double>(CacheGet()); }
        }

        #endregion

        // Called from source
        public ITimeSpaceValueSet GetValues(IBaseExchangeItem exchange)
        {
            if (!(exchange is ITimeSpaceInput))
                throw new InvalidOperationException();

            ITimeSpaceInput source = (ITimeSpaceInput)exchange;

            ValidTimeSet(source.TimeSet);

            // TODO Check Component.Status befor update ? gridlock issues?
            // this.GetValues(source) should

            bool unavailable = false;
            bool forceCacheUpdate = false;

            while (!CacheUpdateSource(source, forceCacheUpdate))
            {
                // TODO Multithreading sleep and retry
                unavailable
                    = Component.Status != LinkableComponentStatus.Initialized
                    && Component.Status != LinkableComponentStatus.Updated
                    && Component.Status != LinkableComponentStatus.Valid;

                if (unavailable)
                    forceCacheUpdate = true;
                else
                    Component.Update(new ITimeSpaceOutput[] { this });
            }

            if (ItemChanged != null)
                ItemChanged(this, new ExchangeItemChangeEventArgs { ExchangeItem = this, Message = CurrentState() });

            return ((ITimeSpaceInput)source).Values;
        }

        public override event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        // Called from component
        public void Update(ITimeSet timeSet, IList value)
        {
            CachePush(timeSet, value);

            if (ItemChanged != null)
                ItemChanged(this, new ExchangeItemChangeEventArgs { ExchangeItem = this, Message = CurrentState() });
        }
    }
}
