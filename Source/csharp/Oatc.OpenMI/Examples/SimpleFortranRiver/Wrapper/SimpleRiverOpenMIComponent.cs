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
#region Copyright
///////////////////////////////////////////////////////////
//
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper 
// purpose: Example-implementation river model wrapper
// file: SimpleRiverOpenMIComponent.cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion

using System;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;
using System.Collections.Generic;
using System.Collections;

namespace Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper
{
	/// <summary>
	/// Summary description for SimpleRiverOpenMIComponent.
	/// </summary>
	public class SimpleRiverOpenMIComponent : Oatc.OpenMI.Sdk.Wrapper.LinkableEngine
	{
        Describable _instance = new Describable("Simple River Model", "");
        Factory _factory;

		public SimpleRiverOpenMIComponent()
		{
			_engineApiAccess = new SimpleRiverEngineWrapper();

            Id = "C0A9704A-B809-4b82-9F41-91639C93B746";

            Caption = "SimpleRiverOpenMIComponent";
            Description = "An example implementation of a simple river model using the OpenMI IEngine backbone interface";            
		}

        public override void Initialize(IArgument[] arguments)
        {
            foreach (IArgument iArg in arguments)
            {
                if (iArg.Key == "Caption")
                    _instance.Caption = iArg.Value;
                if (iArg.Key == "Description")
                    _instance.Description = iArg.Value;
            }
            base.Initialize(arguments);
        }

		protected override void SetEngineApiAccess()
		{
			_engineApiAccess = new SimpleRiverEngineWrapper();
		}

        public override global::OpenMI.Standard.IDescribable InstanceDescription
        {
            get {  return _instance; }
        }

        public override IExchangeItemDecoratorFactory GetDecoratorFactory()
        {
            if (_factory == null)
                _factory = new Factory();

            return _factory;
        }
    }

    public class SimpleRiverException : Exception 
    {
        public SimpleRiverException() { }
        public SimpleRiverException(string s) : base(s) { }
        public SimpleRiverException(string s, Exception inner) : base(s, inner) { }
    }

    public class Factory : IExchangeItemDecoratorFactory
    {
        Dictionary<IIdentifiable, IDescribable> _ids
            = new Dictionary<IIdentifiable, IDescribable>();
        
        Describable _description = new Describable("Simple River Default Factory",
            "Simple River Default Factory");
        Identifier _id = new Identifier("1");

        public Factory()
        {
            _ids.Add(new Identifier(_ids.Count.ToString()),
                new Describable("Linear", "Linear Transformation"));

            _ids.Add(new Identifier(_ids.Count.ToString()),
                new Describable("Wave", "Wave Transformation"));
        }

        #region IExchangeItemDecoratorFactory Members

        public IIdentifiable[] GetAvailableOutputDecorators(IOutputItem decoratedItem, IInputItem targetItem)
        {
            return new List<IIdentifiable>(_ids.Keys).ToArray();
        }

        public IOutputItemDecorator CreateOutputItemDecorator(IIdentifiable decoratorIdentifier, IOutputItem decoratedItem, IInputItem targetItem)
        {
            if (!_ids.ContainsKey(decoratorIdentifier))
                throw new NotImplementedException();

            switch (_ids[decoratorIdentifier].Caption)
            {
                case "Linear":
                    return new DecoratorLinear(decoratorIdentifier, decoratedItem, targetItem);
                case "Wave":
                    return new DecoratorWave(decoratorIdentifier, decoratedItem, targetItem);
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        public IDescribable GetDecoratorDescription(IIdentifiable id)
        {
            return _ids[id];
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _description.Caption; }
            set { _description.Caption = value; }
        }

        public string Description
        {
            get { return _description.Description; }
            set { _description.Description = value; }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _id.Id; }
        }

        #endregion
    }

    class DecoratorLinear : DecoratorBase
    {
        double _a = 1.0;
        double _b = 0.0;
        bool argumentsParsed = false; 

        public DecoratorLinear(IIdentifiable identifiable, IOutputItem decorated, IInputItem target)
            : base (identifiable, new Describable("Linear: Ax + B", 
                "Linear transformation y = Ax + B"), decorated, target)
        {
            _arguments.Add(new Argument("A", "1.0", false, "A in y = Ax + B"));
            _arguments.Add(new Argument("B", "0.0", false, "B in y = Ax + B"));
        }

        public override IList Values
        {
            get
            {
                if (!argumentsParsed)
                {                  
                    _a = double.Parse(_arguments[0].Value);
                    _b = double.Parse(_arguments[1].Value);
                    argumentsParsed = true;
                }

                IList values = _decorated.Values;

                for (int n = 0; n < values.Count; ++n)
                    values[n] = _a * (double)values[n] + _b;

                return values;
            }
        }
    }

    class DecoratorWave : DecoratorBase
    {
        double _phase = 0.0;
        double _amplitude = 1.0;
        double _frequancy = 1.0;
        bool argumentsParsed = false; 

        public DecoratorWave(IIdentifiable identifiable, IOutputItem decorated, IInputItem target)
            : base(identifiable, new Describable("Wave: A*sin(F*t + P)",
                "Wave transformation y = Amplitude * sin(frequancy * time) + phase"), decorated, target)
        {
            _arguments.Add(new Argument("Amplitude", "0.0", false, "A in A*sin(F*t + P)"));
            _arguments.Add(new Argument("Phase", "0.0", false, "P in y = A*sin(F*t + P)"));
            _arguments.Add(new Argument("Frequency", "0.0", false, "F in y = A*sin(F*t + P)"));
        }

        public override IList Values
        {
            get
            {
                if (!argumentsParsed)
                {
                    _amplitude = double.Parse(_arguments[0].Value);
                    _phase = double.Parse(_arguments[1].Value);
                    _frequancy = double.Parse(_arguments[1].Value);
                    argumentsParsed = true;
                }

                IList values = _decorated.Values;

                // ADH: is this right time, also needs error checking
                double t = _decorated.TimeSet.Times[_decorated.TimeSet.Times.Count - 1].StampAsModifiedJulianDay;

                for (int n = 0; n < values.Count; ++n)
                    values[n] = _amplitude * Math.Sin(_frequancy * t + _phase);

                return values;
            }
        }
    }

    abstract class DecoratorBase : IOutputItemDecorator
    {
        protected IIdentifiable _identifiable;
        protected IDescribable _describable;
        protected IOutputItem _decorated;
        protected IInputItem _target;
        protected List<IArgument> _arguments = new List<IArgument>();

        public DecoratorBase(IIdentifiable identifiable, IDescribable descibable, IOutputItem decorated, IInputItem target)
        {
            _identifiable = identifiable;
            _describable = descibable;
            _decorated = decorated;
            _target = target;
        }

        #region IOutputItemDecorator Members

        public IList<IArgument> Arguments
        {
            get { return _arguments; }
        }

        public IOutputItem DecoratedOutputItem
        {
            get { return _decorated; }
            set { _decorated = value; }
        }

        #endregion

        #region IOutputItem Members

        public IList<IInputItem> Consumers
        {
            get { return _decorated.Consumers; }
        }

        public void AddConsumer(IInputItem consumer)
        {
            _decorated.AddConsumer(consumer);
        }

        public void RemoveConsumer(IInputItem consumer)
        {
            _decorated.RemoveConsumer(consumer);
        }

        public bool IsAvailable
        {
            get { return _decorated.IsAvailable; }
        }

        #endregion

        #region IExchangeItem Members

        public IValueDefinition ValueDefinition
        {
            get { return _decorated.ValueDefinition; }
        }

        public ITimeSet TimeSet
        {
            get { return _decorated.TimeSet; }
        }

        public IElementSet ElementSet
        {
            get { return _decorated.ElementSet; }
        }

        public ILinkableComponent Component
        {
            get { return _decorated.Component; }
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _describable.Caption; }
            set { _describable.Caption = value; }
        }

        public string Description
        {
            get { return _describable.Description; }
            set { _describable.Description = value; }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _identifiable.Id; }
        }

        #endregion

        #region IOutputItemDecorator Members


        public void Update()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOutputItem Members

        public abstract IList Values { get; }
      
        #endregion
    }
}
