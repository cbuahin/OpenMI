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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.UpwardsComp.Standard;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.UpwardsComp.Backbone
{
  /// <summary>
  /// TODO: Update summary
  /// The input exchange item is an exchange item used for inputs
  /// in the receiving component.
  /// <para>This is a trivial implementation of OpenMI.Standard.IInputExchangeItem, refer there for further details.</para>
  /// </summary>
  [Serializable]
  public class InputExchangeItem : Oatc.OpenMI.Sdk.Backbone.Input, IInputExchangeItem
  {
    public InputExchangeItem() : base(string.Empty)
    {
    }
      
    public InputExchangeItem(string id) : base(id)
    {
    }

    public InputExchangeItem(string id, IValueDefinition valueDefinition, IElementSet elementSet) : base(id, valueDefinition, elementSet)
    {
    }

    /// <summary>
    /// Getter and setter for the quantity, wrapping the SDK ValueDefinition
    /// </summary>
    public IQuantity Quantity
    {
      get
      {
        if (_valueDefinition is IQuantity)
          return _valueDefinition as IQuantity;
        throw new NotSupportedException("A 1.4 component does not support wrapping a ValueDefinition of type: " + _valueDefinition.GetType());
      }
      set { _valueDefinition = value; }
    }

    /// <summary>
    /// Getter and setter for the element set.
    /// See <see cref="IElementSet">IExchangeItem.ElementSet</see>.
    /// </summary>
    public IElementSet ElementSet
    {
      get { return ExtensionMethods.ElementSet(this); }
      set { SpatialDefinition = value; }
    }

  }
}
