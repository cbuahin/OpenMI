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

using System.Collections;
using System.Collections.Generic;

namespace OpenMI.Standard2.TimeSpace
{
    /// <summary>
    /// The <see cref="ITimeSpaceValueSet"/> represents an ordered two-dimensional 
    /// list of values. The first dimension stands for the times for which values
    /// are available, whereas in the second dimension each value belongs to
    /// precisely one element in the corresponding <see cref="IElementSet"/> (that was
    /// specified when asking for the values). In other words, the i-th value in
    /// that dimension of the value set corresponds to the i-th element in the
    /// IElementSet.
    /// </summary>
    public interface ITimeSpaceValueSet : IBaseValueSet
    {
        /// <summary>
        /// Two-dimensional list of values.
        /// The first IList represents time, and the contained IList the element in the IElementSet.
        /// </summary>
        IList<IList> Values2D { get; set; }

        /// <summary>
        /// Get the value for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/> from <see cref="Values2D"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        object GetValue(int timeIndex, int elementIndex);

        /// <summary>
        /// Set the value in <see cref="Values2D"/>, for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        void SetValue(int timeIndex, int elementIndex, object value);

        /// <summary>
        /// Get values from <see cref="Values2D"/>, for all times, for the specified <paramref name="elementIndex"/> .
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        IList GetTimeSeriesValuesForElement(int elementIndex);

        /// <summary>
        /// Set values in <see cref="Values2D"/>, for all times, for the specified <paramref name="elementIndex"/>.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        void SetTimeSeriesValuesForElement(int elementIndex, IList values);

        /// <summary>
        /// Get values from <see cref="Values2D"/>, for all elements, for the specified <paramref name="timeIndex"/> .
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// </summary>
        IList GetElementValuesForTime(int timeIndex);

        /// <summary>
        /// Set values in <see cref="Values2D"/>, for all elements, for the specified <paramref name="timeIndex"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// </summary>
        void SetElementValuesForTime(int timeIndex, IList values);
    }
}