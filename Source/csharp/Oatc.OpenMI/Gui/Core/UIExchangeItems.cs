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
using System.Collections;
using System.Reflection;
using System.IO;

using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
	public abstract class UIExchangeItem : ITimeSpaceExchangeItem
	{
		protected ITimeSpaceExchangeItem _item;

        public override string ToString()
        {
            return _item.Caption;
        }

		public UIExchangeItem(ITimeSpaceExchangeItem item)
		{
			_item = item;
		}

		public ITimeSpaceExchangeItem ExchangeItem
		{
			get { return _item; }
		}

		#region IExchangeItem Members

        public IValueDefinition ValueDefinition
		{
			get { return _item.ValueDefinition; }
		}

		public ITimeSet TimeSet
		{
			get { return _item.TimeSet; }
		}

		public IElementSet ElementSet
		{
			get { return _item.ElementSet(); }
		}

    public ISpatialDefinition SpatialDefinition
    {
      get { return _item.SpatialDefinition; }
    }
    
    public IBaseLinkableComponent Component
		{
			get { return _item.Component; }
		}

        public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        protected void ForwardExchangeEvent(object sender, ExchangeItemChangeEventArgs e)
        {
            if (ItemChanged != null)
                ItemChanged(sender, e);
        }

        #endregion

		#region IDescribable Members

		public string Caption
		{
			get { return _item.Caption; }
			set { _item.Caption = value; }
		}

		public string Description
		{
			get { return _item.Description; }
			set { _item.Description = value; }
		}

		#endregion

		#region IIdentifiable Members

		public string Id
		{
			get { return _item.Id; }
		}

		#endregion
	}

	public class UIInputItem : UIExchangeItem, ITimeSpaceInput
	{
		public UIInputItem(ITimeSpaceInput item)
			: base(item)
		{
		}

		#region IInput Members


        public ITimeSpaceInput TimeSpaceInput
        {
            get { return _item as ITimeSpaceInput; }
        }
       
		public IBaseOutput Provider
		{
			get { return ((ITimeSpaceInput)_item).Provider; }
			set { ((ITimeSpaceInput)_item).Provider = value; }
		}

	    public ITimeSpaceValueSet Values
	    {
            get { return ((ITimeSpaceInput)_item).Values; }
            set { ((ITimeSpaceInput)_item).Values = value; }
	    }

	    IBaseValueSet IBaseInput.Values
		{
			get { return Values; }
            set { Values = (ITimeSpaceValueSet)value; }
		}

		#endregion
	}

	public class UIOutputItem : UIExchangeItem, ITimeSpaceOutput
	{
        UIOutputItem _parent;

        public UIOutputItem(ITimeSpaceOutput item, UIOutputItem parent =null)
			: base(item)
		{
            _parent = parent;
		}

        public UIOutputItem Parent
        {
            get 
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public ITimeSpaceOutput TimeSpaceOutput
        {
            get { return _item as ITimeSpaceOutput; }
        }


		#region Output Members

		public IList<IBaseInput> Consumers
		{
			get { return ((IBaseOutput)_item).Consumers; }
		}

		public void AddConsumer(IBaseInput consumer)
		{
			((ITimeSpaceOutput)_item).AddConsumer(consumer);
		}

		public void RemoveConsumer(IBaseInput consumer)
		{
			((ITimeSpaceOutput)_item).RemoveConsumer(consumer);
		}

        public IList<IBaseAdaptedOutput> AdaptedOutputs
        {
            get { return ((IBaseOutput)_item).AdaptedOutputs; }
        }

	    public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
	    {
	        ((IBaseOutput)_item).AddAdaptedOutput(adaptedOutput);
	    }

	    public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
	    {
            ((IBaseOutput)_item).RemoveAdaptedOutput(adaptedOutput);
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
			get 
            {
                return ((ITimeSpaceOutput)_item).Values;
            }
		}

		public ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
		{
			return ((ITimeSpaceOutput)_item).GetValues(querySpecifier);
		}

		#endregion
	}

    public class UIAdaptedOutputItem : UIOutputItem , ITimeSpaceAdaptedOutput
    {
        UIAdaptedFactory _factory;
        IIdentifiable _decoratorId;

       public UIAdaptedOutputItem(UIAdaptedFactory factory, IIdentifiable decoratorId,  ITimeSpaceAdaptedOutput item, UIOutputItem parent)
			: base(item , parent)
		{
			_factory = factory;
            _decoratorId = decoratorId;
		}

       public ITimeSpaceAdaptedOutput TimeSpaceAdaptedOutput
       {
           get { return _item as ITimeSpaceAdaptedOutput; }
       }

       public UIAdaptedFactory Factory
       {
           get { return _factory; }
       }

       public IIdentifiable DecoratorId
       {
           get { return _decoratorId; }
       }

        public IBaseOutput Adaptee
        {
            get {return ((ITimeSpaceAdaptedOutput) _item).Adaptee; }
        }

        public IList<IArgument> Arguments
        {
            get { return ((ITimeSpaceAdaptedOutput)_item).Arguments; }
        }

        public void Initialize()
        {
            ((ITimeSpaceAdaptedOutput)_item).Initialize();
        }

        public void Refresh()
        {
            ((ITimeSpaceAdaptedOutput)_item).Refresh();
        }


        public new IBaseValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
           return ((ITimeSpaceAdaptedOutput)_item).GetValues(querySpecifier);
        }

        public new IBaseValueSet Values
        {
            get
            {
                return ((ITimeSpaceAdaptedOutput)_item).Values; 
            }
        }
    }
}
