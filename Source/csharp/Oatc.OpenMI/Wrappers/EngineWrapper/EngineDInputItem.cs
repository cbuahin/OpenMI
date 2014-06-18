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
﻿using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper
{

    /// <summary>
    /// A delegate used for setting values to engine
    /// </summary>
    public delegate void DValueSetter(ITimeSpaceValueSet values);

    /// <summary>
    /// An input item that uses delegates to get and set the value.
    /// 
    /// The <see cref="ValueGetter"/> may be null, in case the input item
    /// does not support getting values.
    /// 
    /// The <see cref="ValueSetter"/> must always be set.
    /// </summary>
    public class EngineDInputItem : EngineInputItem
    {

        public EngineDInputItem(string id, IValueDefinition valueDefinition, IElementSet elementSet, LinkableEngine comp) 
            : base(id, valueDefinition, elementSet, comp)
        {
            _component = comp;
        }

        public DValueSetter ValueSetter;
        public DValueGetter ValueGetter;

        public override ITimeSpaceValueSet GetValuesFromEngine()
        {
            if (ValueGetter == null)
                throw new System.NotSupportedException();
            return (ValueGetter());
        }

        public override void SetValuesToEngine(ITimeSpaceValueSet valueSet)
        {
            ValueSetter(valueSet);
        }
    }
}
