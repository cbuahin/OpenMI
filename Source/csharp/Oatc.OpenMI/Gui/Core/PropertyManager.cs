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

using System;
using System.Collections;
using System.ComponentModel;

namespace Oatc.OpenMI.Gui.Controls
{
    /// <summary>
    /// This class is used to hold pairs of <c>string</c> key-value pairs.
    /// These pairs are than published like typical .NET class properties (ie. { get; set; } ) 
    /// using overrides of <see cref="ICustomTypeDescriptor">ICustomTypeDescriptor</see>
    /// methods.
    /// Main goal of this class is to be able to modify properties shown in
    /// <see cref="System.Windows.Forms.PropertyGrid">PropertyGrid</see>
    /// in run-time.
    /// </summary>
    public class PropertyManager : ICustomTypeDescriptor
    {
        #region Public members

        private readonly Hashtable _properties;

        /// <summary>
        /// Creates a new instance if <see cref="PropertyManager">PropertyManager</see> class.
        /// </summary>
        public PropertyManager()
        {
            _properties = new Hashtable();
            Tag = null;
        }

        /// <summary>
        /// Gets or sets custom user object.
        /// </summary>
        /// <remarks>
        /// This object can be used by the user for any purpose and all non-static methods of this class
        /// ignores it.
        /// Static method <see cref="ConstructPropertyManager">ConstructPropertyManager</see> sets 
        /// new value into it.
        /// </remarks>
        public object Tag { get; set; }


        /// <summary>
        /// Creates new or sets existing property of this instance.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <param name="Value">Value of the property.</param>
        /// <param name="ReadOnly">Determines whether the property is read-only.</param>		
        public void SetProperty(string Name, string Value, bool ReadOnly)
        {
            SetProperty(Name, Value, ReadOnly, null, null);
        }

        /// <summary>
        /// Creates new or sets existing property of this instance.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <param name="Value">Value of the property.</param>
        /// <param name="ReadOnly">Determines whether the property is read-only.</param>
        /// <param name="Description">Description of the property or <c>null</c> if no description is needed.</param>		
        public void SetProperty(string Name, string Value, bool ReadOnly, string Description)
        {
            SetProperty(Name, Value, ReadOnly, Description, null);
        }

        /// <summary>
        /// Creates new or sets existing property of this instance.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <param name="Value">Value of the property.</param>
        /// <param name="ReadOnly">Determines whether the property is read-only.</param>
        /// <param name="Description">Description of the property or <c>null</c> if no description is needed.</param>
        /// <param name="Category">Category of the property or <c>null</c> if no category is needed.</param>
        public void SetProperty(string Name, string Value, bool ReadOnly, string Description, string Category)
        {
            // Set attributes of this property
            var attributes = new ArrayList();
            attributes.Add(new ReadOnlyAttribute(ReadOnly));
            if (Description != null)
                attributes.Add(new DescriptionAttribute(Description));
            if (Category != null)
                attributes.Add(new CategoryAttribute(Category));

            var desc = new MyPropertyDescriptor(Name, Value, ReadOnly,
                                                (Attribute[]) attributes.ToArray(typeof (Attribute)));

            _properties[Name] = desc;
        }

        /// <summary>
        /// Gets value of some property.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <returns>Returns value of the property.</returns>
        public string GetProperty(string Name)
        {
            if (!_properties.Contains(Name))
                throw (new ArgumentException("There's no property with this name: " + Name));
            return ((string) ((MyPropertyDescriptor) _properties[Name]).GetValue(null));
        }

        /// <summary>
        /// Removes property from this instance.
        /// </summary>
        /// <param name="Name">Name of the property to be removed.</param>
        public void RemoveProperty(string Name)
        {
            _properties.Remove(Name);
        }

        #region Nested type: MyPropertyDescriptor

        private class MyPropertyDescriptor : PropertyDescriptor
        {
            private readonly bool _readOnly;
            private string _value;

            public MyPropertyDescriptor(
                string name,
                string value,
                bool readOnly,
                Attribute[] attrs) :
                    base(name, attrs)
            {
                _readOnly = readOnly;
                _value = value;
            }


            public override Type ComponentType
            {
                get { return typeof (PropertyManager); }
            }

            public override bool IsReadOnly
            {
                get { return (_readOnly); }
            }

            public override Type PropertyType
            {
                get { return typeof (string); }
            }

            public override bool CanResetValue(object component)
            {
                return (false);
            }

            public override object GetValue(object component)
            {
                return (_value);
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                if (!_readOnly)
                    _value = (string) value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return (false);
            }
        }

        #endregion

        #region ICustomTypeDescriptor explicit interface definitions

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            var ret = new MyPropertyDescriptor[_properties.Count];

            int i = 0;
            foreach (MyPropertyDescriptor desc in _properties.Values)
                ret[i++] = desc;

            return (new PropertyDescriptorCollection(ret));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    #endregion
}