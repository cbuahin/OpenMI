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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters
{
    abstract class AddapterBase : ITimeSpaceAdaptedOutput
    {
        IIdentifiable _identifier;
        IDescribable _described;
        ITimeSpaceOutput _adapted;
        ITimeSpaceInput _target;
        private IList<IBaseAdaptedOutput> _adapters;

        protected List<IArgument> _arguments
            = new List<IArgument>();

        public AddapterBase(IIdentifiable identifier, IDescribable described, ITimeSpaceOutput adapted, ITimeSpaceInput target)
        {
            _identifier = identifier;
            _described = described;
            _target = target;
            _adapted = adapted;
            if (!_adapted.AdaptedOutputs.Contains(this))
                _adapted.AddAdaptedOutput(this);
        }

        public virtual string GetCaption()
        {
            return _described.Caption;
        }
        
        #region IAdaptedOutput Members

        public IList<IArgument> Arguments
        {
            get { return _arguments; }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public IBaseOutput Adaptee
        {
            get { return _adapted; }
        }

        public virtual void Refresh()
        {
            foreach (ITimeSpaceAdaptedOutput decorator in AdaptedOutputs)
                decorator.Refresh();
        }

        #endregion

        #region IOutput Members

        public IList<IBaseInput> Consumers
        {
            get { return _adapted.Consumers; }
        }

        public void AddConsumer(IBaseInput consumer)
        {
            _adapted.AddConsumer(consumer);
        }

        public void RemoveConsumer(IBaseInput consumer)
        {
            _adapted.RemoveConsumer(consumer);
        }

        public IList<IBaseAdaptedOutput> AdaptedOutputs
        {
            get
            {
                if (_adapters == null)
                    _adapters = new List<IBaseAdaptedOutput>();
                return _adapters;
            }
        }

        public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _adapters.Add(adaptedOutput);
        }

        public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _adapters.Remove(adaptedOutput);
        }

        IBaseValueSet IBaseOutput.Values
        {
            get { return Values; }
        }

        IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
        {
            return GetValues(querySpecifier);
        }


        public virtual ITimeSpaceValueSet Values
        {
            get { return _adapted.Values; }
        }

        public virtual ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
            return _adapted.GetValues(querySpecifier);
        }

        #endregion

        #region IExchangeItem Members

        public IValueDefinition ValueDefinition
        {
            get { return _adapted.ValueDefinition; }
        }

        public ITimeSet TimeSet
        {
            get { return _adapted.TimeSet; }
        }

        public IElementSet ElementSet
        {
            get { return _adapted.ElementSet(); }
        }

        public ISpatialDefinition SpatialDefinition
        {
          get { return _adapted.SpatialDefinition; }
        }

      public IBaseLinkableComponent Component
        {
            get { return _adapted.Component; }
        }

        public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return GetCaption(); }
            set { _described.Caption = value; }
        }

        public string Description
        {
            get { return _described.Description; }
            set { _described.Description = value; }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _identifier.Id; }
        }

        #endregion
    }
}
