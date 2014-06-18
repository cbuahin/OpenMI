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
using System.Text;

using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public abstract class ItemInBase : ItemBase, ITimeSpaceInput
    {
        ITimeSpaceOutput _iProvider;
        protected ITimeSet _currentTimeSet;
        ITimeSpaceValueSet _values;

        public ItemInBase(string id)
            : base(id)
        { }

        #region IInput Members

        public IBaseOutput Provider
        {
            get
            {
                return _iProvider;
            }
            set
            {
                _iProvider = (ITimeSpaceOutput)value;
            }
        }

        public ITimeSpaceValueSet Values
        {
            get { return _values; }
            set
            {
                ValidValue(value);
                _values = value;
            }
        }

        IBaseValueSet IBaseInput.Values
        {
            get { return _values; }
            set
            {
                if (value != null && !(value is ITimeSpaceValueSet))
                    throw new ArgumentException("Values must be ITimeSpaceValueSet - you may need to add an adaptor");

                Values = (ITimeSpaceValueSet) value;
            }
        }

        #endregion

        #region IExchangeItem Members

        new public ITimeSet TimeSet
        {
            get { return _currentTimeSet; }
        }

        #endregion

        public abstract void ValidTimeSet(ITimeSet iTimeSet);
        public abstract void ValidValue(ITimeSpaceValueSet value);
        public abstract bool UpdateRequired(ITimeSet request, ITimeSet current);
        public abstract void DefaultValues();

        public void Update(ITimeSet required)
        {
            if (Provider == null)
                return;

            if (!UpdateRequired(required, _currentTimeSet))
                return;

            _currentTimeSet = required;

            if (ItemChanged != null)
                ItemChanged(this, new ExchangeItemChangeEventArgs { ExchangeItem = this, Message = CurrentState() });

            Values.Values2D.Clear();

            if (Component == typeof(LinkableComponent))
                ((LinkableComponent)Component).Status
                    = LinkableComponentStatus.WaitingForData;

            // TODO: Change status for non LinkableComponent? Standard needs this?

            Values = (ITimeSpaceValueSet)Provider.GetValues(this);

            if (ItemChanged != null)
                ItemChanged(this, new ExchangeItemChangeEventArgs { ExchangeItem = this, Message = CurrentState() });
        }

        public override event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;
    }
}
