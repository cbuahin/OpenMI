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
using System.ComponentModel;

using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;
using OatcTime = Oatc.OpenMI.Sdk.Backbone.Time;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.Controls
{
    public class PropertyPane
    {
        static public object Selection(object obj)
        {
            if (obj == null)
                return null;

            if (obj is ITimeSpaceComponent)
                return new Component((ITimeSpaceComponent)obj);

            if (obj is ITimeSpaceAdaptedOutput)
                return new AdaptedOutput((ITimeSpaceAdaptedOutput)obj);

            if (obj is ITimeSpaceOutput)
                return new Output((ITimeSpaceOutput)obj);

            if (obj is ITimeSpaceInput)
                return new Input((ITimeSpaceInput)obj);

            if (obj is IQuality)
                return new Quality((IQuality)obj);

            if (obj is IQuantity)
                return new Quantity((IQuantity)obj);

            if (obj is IElementSet)
                return new ElementSet((IElementSet)obj);

            if (obj is UIExchangeItem) // RECURSION
                return Selection(((UIExchangeItem)obj).IExchangeItem);

            if (obj is UIConnection.Link)
                return new Link((UIConnection.Link)obj);

            return null;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Identifiable
        {
            string _id;

            public Identifiable(IIdentifiable iIdentifiable)
            {
                _id = iIdentifiable.Id;
            }

            public override string ToString()
            {
                return _id;
            }

            public string Id
            {
                get { return _id; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Describable
        {
            string _caption;
            string _description;

            public Describable(IDescribable iDescribable)
            {
                _caption = iDescribable.Caption;
                _description = iDescribable.Description;
            }

            public override string ToString()
            {
                return _caption;
            }

            public string Caption
            {
                get { return _caption; }
            }

            public string Description
            {
                get { return _description; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Component")]
        public class Component : Describable
        {
            string _id;
            IBaseLinkableComponent _component;
            TimeSet _timeExtent;
            AdaptedOutputFactory[] _factories = null;
            Argument[] _arguments = null;
            Input[] input = null;
            Output[] output = null;

            public Component(IBaseLinkableComponent component)
                : base(component)
            {
                _id = component.Id;
                _component = component;
                _timeExtent = new TimeSet(component.TimeExtent());
            }

            public string Id
            {
                get { return _id; }
            }

            public TimeSet TimeExtent
            {
                get { return _timeExtent; }
            }

            //[DisplayName("Cascading update calls disabled")]
            //public bool CascadingUpdateCallsDisabled
            //{
            //    get { return _component.CascadingUpdateCallsDisabled; }
            //}

            public string Status
            {
                get { return _component.Status.ToString(); }
            }

            public AdaptedOutputFactory[] Factories
            {
                get
                {
                    if (_factories == null)
                    {
                        _factories = new AdaptedOutputFactory[_component.AdaptedOutputFactories.Count];
                        for (int n = 0; n < _component.AdaptedOutputFactories.Count; ++n)
                            _factories[n] = new AdaptedOutputFactory(_component.AdaptedOutputFactories[n]);
                    }

                    return _factories;
                }
            }

            public Argument[] Arguments
            {
                get
                {
                    if (_arguments == null)
                    {
                        _arguments = new Argument[_component.Arguments.Count];

                        for (int n = 0; n < _component.Arguments.Count; ++n)
                            _arguments[n] = new Argument(_component.Arguments[n]);
                    }

                    return _arguments;
                }
            }

            [DisplayName("Targets (Input items)")]
            public Input[] Input
            {
                get
                {
                    if (input == null)
                    {
                        input = new Input[_component.Inputs.Count];

                        for (int n = 0; n < _component.Inputs.Count; ++n)
                            input[n] = new Input((ITimeSpaceInput)_component.Inputs[n]);
                    }

                    return input;
                }
            }

            [DisplayName("Sources (Output items)")]
            public Output[] Output
            {
                get
                {
                    if (output == null)
                    {
                        output = new Output[_component.Outputs.Count];

                        for (int n = 0; n < _component.Outputs.Count; ++n)
                            output[n] = new Output((ITimeSpaceOutput)_component.Outputs[n]);
                    }

                    return output;
                }
            }

        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class AdaptedOutputFactory : Describable
        {
            IAdaptedOutputFactory _factory;

            public AdaptedOutputFactory(IAdaptedOutputFactory factory)
                : base(factory)
            {
                _factory = factory;
            }

            public string Id
            {
                get { return _factory.Id; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("ExchangeItem")]
        public class ExchangeItem : Describable
        {
            string _id;
            ValueDefinition _valueDefinition;
            ElementSet _elementSet;
            TimeSet _timeSet;
            Component _component;

            public ExchangeItem(ITimeSpaceExchangeItem item)
                : base(item)
            {
                _id = item.Id;
                if (item.ValueDefinition != null)
                {
                    if (item.ValueDefinition is IQuality)
                        _valueDefinition = new Quality((IQuality)item.ValueDefinition);
                    else
                        _valueDefinition = new Quantity((IQuantity)item.ValueDefinition);
                }
                if (item.ElementSet() != null)
                    _elementSet = new ElementSet(item.ElementSet());
                if (item.TimeSet != null)
                    _timeSet = new TimeSet(item.TimeSet);
                if (item.Component != null)
                    _component = new Component(item.Component);
            }

            public string Id
            {
                get { return _id; }
            }

            [DisplayName("Value Definition")]
            public ValueDefinition ValueDefinition
            {
                get { return _valueDefinition; }
            }

            [DisplayName("Element Set")]
            public ElementSet ElementSet
            {
                get { return _elementSet; }
            }

            [DisplayName("Time Set")]
            public TimeSet TimeSet
            {
                get { return _timeSet; }
            }

            public Component Component
            {
                get { return _component; }
            }
        }

        /*
         * Recursive calls between these two ???
         * 
                [TypeConverter(typeof(ExpandableObjectConverter))]
                [Category("ExchangeItem")]
                public class InputItem : ExchangeItem
                {
                    OutputItem _provider;

                    public InputItem(IInput iInputItem)
                        : base(iInputItem)
                    {
                        if (iInputItem.Provider != null)
                            _provider = new OutputItem(iInputItem.Provider);
                    }

                    public OutputItem Provider
                    {
                        get { return _provider; }
                    }
                }

                [TypeConverter(typeof(ExpandableObjectConverter))]
                [Category("ExchangeItem")]
                public class OutputItem : ExchangeItem
                {
                    List<InputItem> _consumers = new List<InputItem>();

                    public OutputItem(Output IOutput)
                        : base(IOutput)
                    {
                        foreach (IInput i in IOutput.Consumers)
                            _consumers.Add(new InputItem(i));
                    }

                    public List<PropertyPane.InputItem> Consumers
                    {
                        get { return _consumers; }
                    }
                }
        */

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("ExchangeItem")]
        public class Input : ExchangeItem
        {
            string _provider = string.Empty;

            public Input(ITimeSpaceInput iInput)
                : base(iInput)
            {
                if (iInput.Provider != null)
                    _provider = iInput.Provider.Caption;
            }

            public string Provider
            {
                get { return _provider; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("ExchangeItem")]
        public class Output : ExchangeItem
        {
            List<string> _consumers = new List<string>();

            public Output(ITimeSpaceOutput iOutput)
                : base(iOutput)
            {
                foreach (ITimeSpaceInput item in iOutput.Consumers)
                    _consumers.Add(item.Caption);
            }

            public List<string> Consumers
            {
                get { return _consumers; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("ExchangeItem")]
        public class AdaptedOutput : Output
        {
            Output deriver;
            Argument[] _arguments;

            public AdaptedOutput(ITimeSpaceAdaptedOutput adaptedOutput)
                : base(adaptedOutput)
            {
                if (adaptedOutput.Adaptee != null)
                    deriver = new Output((ITimeSpaceOutput)adaptedOutput.Adaptee);

                _arguments = new Argument[adaptedOutput.Arguments.Count];

                for (int n = 0; n < adaptedOutput.Arguments.Count; ++n)
                    _arguments[n] = new Argument(adaptedOutput.Arguments[n]);
            }

            [DisplayName("Decorated Output Item")]
            public Output DeriverOutput
            {
                get { return deriver; }
            }

            public Argument[] Arguments
            {
                get { return _arguments; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Argument : Describable
        {
            protected IArgument _iArgument;
            string[] _possibleValues = null;

            public Argument(IArgument iArgument)
                : base(iArgument)
            {
                _iArgument = iArgument;
            }

            public override string ToString()
            {
                return string.Format("{0} = {1}", _iArgument.Caption, _iArgument.Value);
            }

            [DisplayName("ID")]
            public string Id
            {
                get { return _iArgument.Id; }
            }

            public string Value
            {
                get { return _iArgument.ValueAsString; }
                set
                {
                    if (!_iArgument.IsReadOnly)
                        _iArgument.ValueAsString = value;
                }
            }

            [DisplayName("Default value")]
            public string DefaultValue
            {
                get { return _iArgument.DefaultValue.ToString(); }
            }

            [DisplayName("Is read only")]
            public bool IsReadOnly
            {
                get { return _iArgument.IsReadOnly; }
            }

            [DisplayName("Is optional")]
            public bool IsOptional
            {
                get { return _iArgument.IsOptional; }
            }

            [DisplayName("Value type")]
            public Type ValueType
            {
                get { return _iArgument.ValueType; }
            }

            [DisplayName("Possible values")]
            public string[] PossibleValues
            {
                get
                {
                    if (_possibleValues == null && _iArgument.PossibleValues != null)
                    {
                        _possibleValues = new string[_iArgument.PossibleValues.Count];

                        for (int n = 0; n < _iArgument.PossibleValues.Count; ++n)
                            _possibleValues[n] = _iArgument.PossibleValues[n].ToString();
                    }

                    return _possibleValues;
                }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Time
        {
            ITime _iTime;
            DateTime _start;
            DateTime _end;

            public Time(ITime iTime)
            {
                _iTime = iTime;
                _start = new OatcTime(iTime).ToDateTime();
                _end = new OatcTime(iTime.StampAsModifiedJulianDay + iTime.DurationInDays).ToDateTime();
            }

            public override string ToString()
            {
                return _iTime.DurationInDays <= 0
                    ? _start.ToString("u")
                    : string.Format("[{0}, {1})", _start.ToString("u"), _end.ToString("u"));
            }

            [DisplayName("Stamp [Modified Julian]")]
            public double StampAsModifiedJulianDay
            {
                get { return _iTime.StampAsModifiedJulianDay; }
            }

            [DisplayName("Duration [Days]")]
            public double DurationInDays
            {
                get { return _iTime.DurationInDays; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class TimeSet
        {
            ITimeSet _iTimeSet;
            Time _horizon = null;
            Time[] _times = null;

            public TimeSet(ITimeSet iTimeSet)
            {
                _iTimeSet = iTimeSet;
                _horizon = new Time(_iTimeSet.TimeHorizon);
            }

            public override string ToString()
            {
                return string.Format("[{0}] {1}",
                    _iTimeSet.Times != null ? _iTimeSet.Times.Count : 0,
                    _horizon != null ? _horizon.ToString() : "");
            }

            [DisplayName("Has Durations")]
            public bool HasDurations
            {
                get { return _iTimeSet.HasDurations; }
            }

            [DisplayName("Offset from UTC [Hrs]")]
            public double OffsetFromUtcInHours
            {
                get { return _iTimeSet.OffsetFromUtcInHours; }
            }

            [DisplayName("Time Horizon")]
            public Time TimeHorizon
            {
                get { return _horizon; }
            }

            public Time[] Times
            {
                get
                {
                    if (_times == null)
                    {
                        _times = new Time[_iTimeSet.Times.Count];
                        for (int n = 0; n < _iTimeSet.Times.Count; ++n)
                            _times[n] = new Time(_iTimeSet.Times[n]);
                    }

                    return _times;
                }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ValueDefinition : Describable
        {
            string _valueType;

            public ValueDefinition(IValueDefinition iValueDefinition)
                : base(iValueDefinition)
            {
                _valueType = iValueDefinition.ValueType.ToString();
            }

            [DisplayName("Value Type")]
            public string ValueType
            {
                get { return _valueType; }
            }
			
			public Object MissingDataValue
			{
				get;
				set;
			}

		}

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Quality : ValueDefinition
        {
            bool _isOrdered;
            List<Category> _categories;

            public Quality(IQuality iQuality)
                : base(iQuality)
            {
                _isOrdered = iQuality.IsOrdered;
            }

            [DisplayName("Is Ordered")]
            public bool IsOrdered
            {
                get { return _isOrdered; }
            }

            public List<Category> Categories
            {
                get { return _categories; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Category : Describable
        {
            string _value;

            public Category(ICategory iCategory)
                : base(iCategory)
            {
                _value = iCategory.Value.ToString();
            }

            public string Value
            {
                get { return _value; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Quantity : ValueDefinition
        {
            IQuantity _quantity;
            Unit _unit = null;
            Dimension _dimension = null;

            public Quantity(IQuantity iQuantity)
                : base(iQuantity)
            {
                _quantity = iQuantity;
                _unit = new Unit(_quantity.Unit);
            }

            public Unit Unit
            {
                get
                {
                    if (_unit == null)
                        _unit = new Unit(_quantity.Unit);

                    return _unit;
                }
            }

            [DisplayName("Dimensions")]
            public Dimension Dimension
            {
                get
                {
                    if (_dimension == null)
                        _dimension = new Dimension(_quantity.Unit.Dimension);

                    return _dimension;
                }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Unit
        {
            IUnit _unit;

            public Unit(IUnit unit)
            {
                _unit = unit;
            }

            public override string ToString()
            {
                return _unit.Caption;
            }

            public string Caption
            {
                get { return _unit.Caption; }
            }

            public string Description
            {
                get { return _unit.Description; }
            }

            [Description("Conversion to SI units. Factor A in SI = Ax + B")]
            public double ConversionFactorToSI
            {
                get { return _unit.ConversionFactorToSI; }
            }

            [Description("Conversion to SI units. Offset B in SI = Ax + B")]
            public double OffSetToSI
            {
                get { return _unit.OffSetToSI; }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Dimension
        {
            IDimension _dimension;

            public Dimension(IDimension dimension)
            {
                _dimension = dimension;
            }

            [Description("Length,Mass,Time,ElectricCurrent,Temperature,AmountOfSubstance,LuminousIntensity, Currency")]
            public override string ToString()
            {
                string s = "";
                if (Length != 0.0)
                    s += string.Format("Length: {0}, ", Length);
                if (Mass != 0.0)
                    s += string.Format("Mass: {0}, ", Mass);
                if (Time != 0.0)
                    s += string.Format("Time: {0}, ", Time);
                if (ElectricCurrent != 0.0)
                    s += string.Format("Electric Current: {0}, ", ElectricCurrent);
                if (Temperature != 0.0)
                    s += string.Format("Temperature: {0}, ", Temperature);
                if (AmountOfSubstance != 0.0)
                    s += string.Format("Amount Of Substance: {0}, ", AmountOfSubstance);
                if (LuminousIntensity != 0.0)
                    s += string.Format("Luminous Intensity: {0}, ", LuminousIntensity);
                if (Currency != 0.0)
                    s += string.Format("Currency: {0}, ", Currency);

                return s;
            }

            [Description("Power to raise dimension Length too to generate required quantity unit")]
            public double Length
            {
                get { return _dimension.GetPower(DimensionBase.Length); }
            }

            [Description("Power to raise dimension Mass too to generate required quantity unit")]
            public double Mass
            {
                get { return _dimension.GetPower(DimensionBase.Mass); }
            }

            [Description("Power to raise dimension Time too to generate required quantity unit")]
            public double Time
            {
                get { return _dimension.GetPower(DimensionBase.Time); }
            }

            [Description("Power to raise dimension Electric Current too to generate required quantity unit")]
            [DisplayName("Electric Current")]
            public double ElectricCurrent
            {
                get { return _dimension.GetPower(DimensionBase.ElectricCurrent); }
            }

            [Description("Power to raise dimension Temperature too to generate required quantity unit")]
            public double Temperature
            {
                get { return _dimension.GetPower(DimensionBase.Temperature); }
            }

            [Description("Power to raise dimension Amount Of Substance too to generate required quantity unit")]
            [DisplayName("Amount Of Substance")]
            public double AmountOfSubstance
            {
                get { return _dimension.GetPower(DimensionBase.AmountOfSubstance); }
            }

            [Description("Power to raise dimension Luminous Intensity too to generate required quantity unit")]
            [DisplayName("Luminous Intensity")]
            public double LuminousIntensity
            {
                get { return _dimension.GetPower(DimensionBase.LuminousIntensity); }
            }

            [Description("Power to raise dimension Currency too to generate required quantity unit")]
            public double Currency
            {
                get { return _dimension.GetPower(DimensionBase.Currency); }
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ElementSet
        {
            IElementSet _elementSet;

            public ElementSet(IElementSet elementSet)
            {
                _elementSet = elementSet;
            }

            public override string ToString()
            {
                return _elementSet.Caption;
            }

            public string Caption
            {
                get { return _elementSet.Caption; }
            }

            public string Description
            {
                get { return _elementSet.Description; }
            }

            [DisplayName("Spatial Reference WKT")]
            public string SpatialReference
            {
                get
                {
                    return _elementSet.SpatialReferenceSystemWkt;
                }
            }

            [DisplayName("Element Type")]
            public string ElementType
            {
                get { return _elementSet.ElementType.ToString(); }
            }

            public int Version
            {
                get { return _elementSet.Version; }
            }

            [DisplayName("Element Count")]
            public int ElementCount
            {
                get { return _elementSet.ElementCount; }
            }

            [DisplayName("Has M")]
            public bool HasM
            {
                get { return _elementSet.HasM; }
            }

            [DisplayName("Has Z")]
            public bool HasZ
            {
                get { return _elementSet.HasZ; }
            }
        }


        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Link
        {
            UIConnection.Link _link;
            Input _target;
            Output _source;

            public Link(UIConnection.Link link)
            {
                _link = link;
            }

            public override string ToString()
            {
                return _link.ToString();
            }

            [Category("Link")]
            public Input Target
            {
                get
                {
                    if (_target == null)
                        _target = new Input((ITimeSpaceInput)_link.Target.IExchangeItem);

                    return _target;
                }
            }

            [Category("Link")]
            public Output Source
            {
                get
                {
                    if (_source == null)
                    {
                        if (_link.Source.IExchangeItem is ITimeSpaceAdaptedOutput)
                            _source = new AdaptedOutput((ITimeSpaceAdaptedOutput)_link.Source.IExchangeItem);
                        else
                            _source = new Output((ITimeSpaceOutput)_link.Source.IExchangeItem);
                    }

                    return _source;
                }
            }
        }
    }
}
