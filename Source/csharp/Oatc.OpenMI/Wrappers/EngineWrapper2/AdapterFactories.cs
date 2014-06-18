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
using System.Diagnostics;

using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters
{
	public class Factory1 : FactoryBase
	{
		public Factory1()
            : base("Factory1",
			new Describable(
            "Factory1",
			"OATC ModelWrapper2 adapted source factory 1"))
		{
		}
	}

    public class Factory2 : FactoryBase
    {
        public Factory2()
            : base("Factory2",
            new Describable(
            "Factory2",
            "OATC ModelWrapper2 adapted source factory 2"))
        {
        }
    }

	public class FactoryBase : IAdaptedOutputFactory
	{
        string _id;
		IDescribable _info;

		Dictionary<IIdentifiable, IDescribable> _adapters
			= new Dictionary<IIdentifiable, IDescribable>();

		public FactoryBase(string id, IDescribable info)
		{
            _id = id;
			_info = info;

			_adapters.Add(Linear.Identifier,
				Linear.Describable);
		}

		#region IAdaptedOutputFactory Members

		public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput targetItem)
		{
			List<IIdentifiable> available = new List<IIdentifiable>();

			if (adaptee.ValueDefinition.ValueType == typeof(double)
				|| adaptee.ValueDefinition.ValueType == typeof(double[]))
			{
				available.Add(Linear.Identifier);
			}

			return available.ToArray();
		}

		public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adapterIdentifier, IBaseOutput adaptee, IBaseInput targetItem)
		{
            if (adapterIdentifier.Id == Linear.Identifier.Id)
                return new Linear((ITimeSpaceOutput) adaptee, (ITimeSpaceInput) targetItem);

			throw new NotImplementedException(adapterIdentifier.Id);
		}

		public IDescribable GetAdaptedOutputDescription(IIdentifiable adapterIdentifier)
		{
			if (!(_adapters.ContainsKey(adapterIdentifier)))
				throw new NotImplementedException(adapterIdentifier.Id);

			return _adapters[adapterIdentifier];
		}

		#endregion

		#region IDescribable Members

		public string Caption
		{
			get { return _info.Caption; }
            set { _info.Caption = value; }
		}

		public string Description
		{
            get { return _info.Description; }
            set { _info.Description = value; }
        }

		#endregion

		#region IIdentifiable Members

		public string Id
		{
			get { return _id; ; }
		}

		#endregion
	}
}
