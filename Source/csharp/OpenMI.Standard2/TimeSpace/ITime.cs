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
    /// Time interface based on a Modified Julian Date (number and fraction of days since 00:00 November 17, 1858).
    /// </para>
    /// <para>
    /// The ITime interface supports a time stamp as well as a time interval. 
    /// A time stamp will have its <see cref="DurationInDays"/> set to 0, while a time interval will have
    /// a positive <see cref="DurationInDays"/> value.
    /// </para>
    /// </summary>
    public interface ITime
    {
        ///<summary>
        /// Time stamp as a modified julian day value.
        ///</summary>
        double StampAsModifiedJulianDay { get; }

        /// <summary>
        /// Duration in days. O if time is a time stamp.
        /// </summary>
        double DurationInDays { get; }
    }
}