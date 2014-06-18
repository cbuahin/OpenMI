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
    /// <summary>
    /// A ValueDefinition describes a value returned by the Values property
    /// and the GetValues function of the <see cref="IBaseExchangeItem"/>.
    /// </summary>
    /// <remarks>
    /// This interface is not meant to be implemented directly.
    /// Instead, implement either <see cref="IQuality"/> or <see cref="IQuantity"/>,
    /// or a custom derived vale definition interface.
    /// </remarks>
    public interface IValueDefinition : IDescribable
    {
        /// <summary>
        /// The object types of value that will be available in the <see cref="IBaseValueSet"/> that is
        /// returned by the Values property and the GetValues function of the <see cref="IBaseExchangeItem"/>.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// The value representing that data is missing
        /// </summary>
        Object MissingDataValue { get; }
    }
}
