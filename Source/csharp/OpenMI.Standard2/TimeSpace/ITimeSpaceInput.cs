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

namespace OpenMI.Standard2.TimeSpace
{
    /// <summary>
    /// <para>
    /// An input item that can accept values for an <see cref="ITimeSpaceComponent"/>.
    /// </para>
    /// <para>
    /// The item is a combination of an <see cref="IValueDefinition"/>, 
    /// an <see cref="IElementSet"/>, and an <see cref="ITimeSet"/>. 
    /// This combination specifies which type of data is required, where and 
    /// when, as input for an <see cref="ITimeSpaceComponent"/>.
    /// </para>
    /// </summary>
    public interface ITimeSpaceInput : ITimeSpaceExchangeItem, IBaseInput
    {
        ///<summary>
        /// The exchange item's values, as a specialized <see cref="ITimeSpaceValueSet"/>
        ///</summary>
        new ITimeSpaceValueSet Values { get; set; }
    }
}