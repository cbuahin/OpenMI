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
using System.Collections.Generic;

namespace OpenMI.Standard2
{
    /// <summary>
    /// <para>
    /// The IArgument interface is used to set the arguments of a <see cref="IBaseLinkableComponent"/>
    /// and the arguments of an <see cref="IBaseAdaptedOutput"/>
    /// </para>
    /// </summary>
    public interface IArgument : IIdentifiable
    {
        /// <summary>
        /// The type of the value of the argument, E.g. a integral type like string, integer or double,
        /// or a non integral type, such as a time series object.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Specifies whether the argument is optional or not.
        /// If the <code>Values</code> property returns null
        /// and <code>IsOptional == false</code>, a value has to be set before
        /// the argument can be used.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Defines whether the Values property may be edited. This is used to let a 
        /// <see cref="IBaseLinkableComponent"/> or an <see cref="IBaseAdaptedOutput"/>
        /// present the actual value of an argument that can not be changed by the user, 
        /// but is needed to determine the values of other arguments or is informative 
        /// in any other way.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// The current value of the argument. If no value has been set yet, a 
        /// default value is returned.
        /// <para>
        /// If <code>null</code> is returned, this means that the default 
        /// value is <code>null</code>.
        /// </para>
        /// </summary>
        object Value { get; set; }


        /// <summary>
        /// The default value of the argument.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// List of possible allowed values for this argument.
        /// If for integral types or component specific types all possible values are allowed, <code>null</code> is returned.
        /// A list with length 0 indicates that there is indeed a limitation on the possible values, but that currently
        /// no values are possible. Effectively this means that the values will not and cannot be set.
        /// </summary>
        IList<object> PossibleValues { get; }

        /// <summary>
        /// The argument's value, represented as a string. If <code>ValueType</code> indicates that the argument's value
        /// is not of the type string, the <code>ValueAsString</code> function offers the possibility to treat it as a string,
        /// e.g. to let the GUI persist the value in the composition file.
        /// </summary>
        string ValueAsString { get; set; }
    }
}
