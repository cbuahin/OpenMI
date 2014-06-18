#region Copyright
/*
    Copyright (c) 2005-2010, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard2.dll

    OpenMI.Standard2.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard2.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;

namespace OpenMI.Standard2
{

    ///<summary>
    /// An item that can be exchanged, either as input or as output.
    ///</summary>
    /// <remarks>
    /// <para>
    /// This interface is not to be implemented directly, any class
    /// is to implement either the <see cref="IBaseInput"/> or 
    /// <see cref="IBaseOutput"/>.
    /// </para>
    /// </remarks>
    public interface IBaseExchangeItem : IIdentifiable
    {
        /// <summary>
        /// Definition of the values in the exchange item.
        /// </summary>
        /// <remarks>
        /// The <see cref="IValueDefinition"/> should never be returned directly; all implementing
        /// classes should return either an <see cref="IQuality"/>, an <see cref="IQuantity"/>, or a
        /// custom derived vale definition interface.
        /// </remarks>
        IValueDefinition ValueDefinition { get; }

        /// <summary>
        /// Gets the owner of the exchange item. For an output exchange item this is
        /// the component responsible for providing the content of the output item.
        /// It is possible for an exchange item to have no owner, in this case the
        /// method will return null.
        /// </summary>
        IBaseLinkableComponent Component { get; }

        /// <summary>
        /// The ItemChanged event is fired when the content of an exchange item has changed.
        /// This might be because its ValueDefinition has changed, its TimeSet has changed,
        /// its ElementSet has changed, its Values have changed, or any permutation of these properties.
        /// </summary>
        event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;
    }
}
