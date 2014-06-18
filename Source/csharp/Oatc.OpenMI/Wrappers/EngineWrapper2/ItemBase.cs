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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public abstract class ItemBase : ITimeSpaceExchangeItem
    {
        IIdentifiable _iIdentifiable;

        protected IDescribable _iDescribable;
        protected IValueDefinition _iValueDefinition;
        protected IElementSet _iElementSet;
        protected ITimeSpaceComponent _iComponent;

        public abstract string CurrentState();

        public ItemBase(string id)
        {
            _iIdentifiable = new Identifier(id);
        }

        public void Initialise(ITimeSpaceComponent iLC, IValueDefinition iVD, IElementSet iES)
        {
            _iValueDefinition = iVD;
            _iElementSet = iES;
            _iComponent = iLC;

            _iDescribable = new Describable(
                string.Format("{0}, {1}", _iValueDefinition.Caption, _iElementSet.Caption),
                string.Format("{0}, {1}", _iValueDefinition.Description, _iElementSet.Description));
        }

        #region IExchangeItem Members

        public IValueDefinition ValueDefinition
        {
            get { return _iValueDefinition; }
        }

        public IElementSet ElementSet
        {
            get { return _iElementSet; }
        }

        public ISpatialDefinition SpatialDefinition
        {
            get { return _iElementSet; }
        }

        public IBaseLinkableComponent Component
        {
            get { return _iComponent; }
        }

        public ITimeSet TimeSet
        {
            get { return null; }
        }

        public abstract event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get
            {
                return _iDescribable.Caption;
            }
            set
            {
                _iDescribable.Caption = value;
            }
        }

        public string Description
        {
            get
            {
                return _iDescribable.Description;
            }
            set
            {
                _iDescribable.Description = value;
            }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _iIdentifiable.Id; }
        }

        #endregion
    }
}
