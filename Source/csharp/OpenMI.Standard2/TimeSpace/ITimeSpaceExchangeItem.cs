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
    ///<summary>
    /// A time / space dependent item that can be exchanged, either as input or as output.
    ///</summary>
    public interface ITimeSpaceExchangeItem : IBaseExchangeItem
    {
        ///<summary>
        /// Time information on the values that are available in an output exchange item, 
        /// or required by an input exchange item
        ///</summary>
        ITimeSet TimeSet { get; }

        ///<summary>
        /// Spatial information (usually an element set) on the values that are available
        /// in an output exchange item, or required by an input exchange item.
        ///</summary>
        ISpatialDefinition SpatialDefinition { get; }
    }
}