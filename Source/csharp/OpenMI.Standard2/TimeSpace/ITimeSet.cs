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

using System.Collections.Generic;

namespace OpenMI.Standard2.TimeSpace
{
  /// <summary>
  /// <para>
  /// A set of time stamps or time intervals, used to indicate where an output item has values and can provide values,
  /// and where an input item does or may require values.
  /// </para>
  /// <para>
  /// The <see cref="HasDurations"/> defines whether the set contains stamps or intervals.
  /// </para>
  /// </summary>
  public interface ITimeSet
  {
    ///<summary>
    /// Time stamps or spans as available in the values of an output item, or as required by an input item.
    /// <para>Specific values:</para>
    /// <para><code>TimeSet.Times.Count == 0</code>, in case of output: time dependent item, but no values available yet or required yet</para>
    /// <para><code>TimeSet.Times.Count == 0</code>, in case of in: time dependent item, but currently no values required</para>
    ///</summary>
    IList<ITime> Times { get; }

    ///<summary>
    /// True if the <see cref="Times"/> have durations, i.e. are time intervals.
    /// In this case, a duration value greater then zero is expected for every ITime
    /// in the <see cref="Times"/> list.
    ///</summary>
    bool HasDurations { get; }

    /// <summary>
    /// Time zone offset from UTC, expressed in the number of hours. Since some of the world's time zones
    /// differ half an hour from their neighbours the value is specified as a double.
    /// </summary>
    double OffsetFromUtcInHours { get; }

    /// <summary>
    /// The time horizon defines for an input item for what time span it may require values. This means
    /// the providers of this input can assume that the input item never goes back further in time
    /// than the time horizon's begin time, <code>TimeHorizon.StampAsModifiedJulianDay</code>.
    /// Also, it will never go further ahead than the time horizon's end time,
    /// <code>TimeHorizon.StampAsModifiedJulianDay+TimeHorizon.DurationInDays</code>.
    /// For an output item, and thus for an adapted output item, the time horizon indicates in what time
    /// span the item can provide values.
    /// <para>Specific values:</para>
    /// <para><code>TimeHorizon.StampAsModifiedJulianDay == Double.NegativeInfinity</code>: far back in time</para>
    /// <para><code>TimeHorizon.Duration == Double.PositiveInfinity</code>: far in the future</para>
    /// </summary>
    ITime TimeHorizon { get; }
  }
}