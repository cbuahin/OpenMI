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
using OpenMI.Standard2;
using System.Collections.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper
{

    public class Factory : IAdaptedOutputFactory
    {
        Dictionary<IIdentifiable, IDescribable> _ids
            = new Dictionary<IIdentifiable, IDescribable>();
        
        string _description = "Simple River Default Factory";
        private string _id = "1";

        public Factory()
        {
            _ids.Add(new Identifier(_ids.Count.ToString()),
                     new Describable("Linear", "Linear Transformation"));

            _ids.Add(new Identifier(_ids.Count.ToString()),
                     new Describable("Wave", "Wave Transformation"));
        }

        #region IExchangeItemDecoratorFactory Members

        public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput decoratedItem, IBaseInput targetItem)
        {
          ITimeSpaceOutput tsoutput = decoratedItem as ITimeSpaceOutput;
          if (tsoutput == null)
              return new IIdentifiable[0];
          return new List<IIdentifiable>(_ids.Keys).ToArray();
        }

        public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable decoratorIdentifier, IBaseOutput decoratedItem, IBaseInput targetItem)
        {
            if (!_ids.ContainsKey(decoratorIdentifier))
                throw new NotImplementedException();

            switch (_ids[decoratorIdentifier].Caption)
            {
                case "Linear":
                    return new DecoratorLinear(decoratorIdentifier, (ITimeSpaceOutput)decoratedItem, (ITimeSpaceInput)targetItem);
                case "Wave":
                    return new DecoratorWave(decoratorIdentifier, (ITimeSpaceOutput)decoratedItem, (ITimeSpaceInput)targetItem);
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        public IDescribable GetAdaptedOutputDescription(IIdentifiable adaptedOutputId)
        {
            return _ids[adaptedOutputId];
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _id; }
        }

        #endregion
    }

    class DecoratorLinear : DecoratorBase
    {
        double _a = 1.0;
        double _b = 0.0;
        bool argumentsParsed = false; 

        public DecoratorLinear(IIdentifiable identifiable, ITimeSpaceOutput decorated, ITimeSpaceInput target)
            : base (identifiable, "Linear: Ax + B", "Linear transformation y = Ax + B", decorated, target)
        {
            _arguments.Add(Argument.Create("A", "1.0", false, "A in y = Ax + B"));
            _arguments.Add(Argument.Create("B", "0.0", false, "B in y = Ax + B"));
        }

        public override ITimeSpaceValueSet Values
        {
            get
            {
                if (!argumentsParsed)
                {                  
                    _a = double.Parse(_arguments[0].ValueAsString);
                    _b = double.Parse(_arguments[1].ValueAsString);
                    argumentsParsed = true;
                }

                ITimeSpaceValueSet values = _decorated.Values;

                for (int n = 0; n < values.Values2D.Count; ++n)
                    for (int m = 0; m < values.Values2D[n].Count; m++)
                        values.Values2D[n][m] = _a * (double)values.Values2D[n][m] + _b;

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

        public DecoratorWave(IIdentifiable identifiable, ITimeSpaceOutput decorated, ITimeSpaceInput target)
            : base(identifiable, "Wave: A*sin(F*t + P)",
                    "Wave transformation y = Amplitude * sin(frequancy * time + phase)", 
                    decorated, target)
        {
            _arguments.Add(Argument.Create("Amplitude", "0.0", false, "A in A*sin(F*t + P)"));
            _arguments.Add(Argument.Create("Phase", "0.0", false, "P in y = A*sin(F*t + P)"));
            _arguments.Add(Argument.Create("Frequency", "0.0", false, "F in y = A*sin(F*t + P)"));
        }

        public override ITimeSpaceValueSet Values
        {
            get
            {
                if (!argumentsParsed)
                {
                    _amplitude = double.Parse(_arguments[0].ValueAsString);
                    _phase = double.Parse(_arguments[1].ValueAsString);
                    _frequancy = double.Parse(_arguments[1].ValueAsString);
                    argumentsParsed = true;
                }

                ITimeSpaceValueSet values = _decorated.Values;

                // ADH: is this right time, also needs error checking
                double t = _decorated.TimeSet.Times[_decorated.TimeSet.Times.Count - 1].StampAsModifiedJulianDay;

                for (int n = 0; n < values.Values2D.Count; ++n)
                    for (int m = 0; m < values.Values2D[n].Count; ++m)
                        values.Values2D[n][m] = _amplitude * Math.Sin(_frequancy * t + _phase);

                return values;
            }
        }
    }

    abstract class DecoratorBase : ITimeSpaceAdaptedOutput
    {
        protected IIdentifiable _identifiable;
        protected string _caption;
        protected string _description;
        protected ITimeSpaceOutput _decorated;
        protected List<IBaseAdaptedOutput> _adaptedOutputs = new List<IBaseAdaptedOutput>();
        protected List<IBaseInput> _consumers = new List<IBaseInput>();
        protected ITimeSpaceInput _target;
        protected List<IArgument> _arguments = new List<IArgument>();

        protected DecoratorBase(IIdentifiable identifiable, string caption, string description, ITimeSpaceOutput decorated, ITimeSpaceInput target)
        {
            _identifiable = identifiable;
            _description = description;
            _caption = caption;
            _decorated = decorated;
            _target = target;
        }

        public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        public virtual void Initialize()
        {
        }

        public IList<IArgument> Arguments
        {
            get { return _arguments; }
        }

        public IBaseOutput Adaptee
        {
            get { return _decorated; }
        }

        public IList<IBaseInput> Consumers
        {
            get { return _consumers; }
        }

        public void AddConsumer(IBaseInput consumer)
        {
            _consumers.Add(consumer);
        }

        public void RemoveConsumer(IBaseInput consumer)
        {
            _consumers.Remove(consumer);
        }

        public IList<IBaseAdaptedOutput> AdaptedOutputs
        {
            get { return (_adaptedOutputs); }
        }

        public void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _adaptedOutputs.Add(adaptedOutput);
        }

        public void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            _adaptedOutputs.Remove(adaptedOutput);
        }

        IBaseValueSet IBaseOutput.Values
        {
            get { return Values; }
        }

        IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
        {
            return GetValues(querySpecifier);
        }

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
            get { return _decorated.ElementSet(); }
        }

        public ISpatialDefinition SpatialDefinition
        {
            get { return _decorated.SpatialDefinition; }
        }

        public IBaseLinkableComponent Component
        {
            get { return _decorated.Component; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Id
        {
            get { return _identifiable.Id; }
        }


        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public abstract ITimeSpaceValueSet Values { get; }

        public virtual ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
            throw new NotImplementedException();
        }

    }
}