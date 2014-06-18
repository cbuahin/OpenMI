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
    /// The <see cref="IBaseValueSet"/> represents a general multi-dimensional set of
    /// values. Each value is of type <see cref="ValueType"/>
    /// <para>
    /// The size of each dimension can vary, depending on the indices provided, e.g.
    /// in a 2D matrix each row can have different lengths. Example, assuming the data
    /// is stored as a double[][] matrix, then matrix[1].Length need not equal 
    /// matrix[2].Length.
    /// </para>
    /// </summary>
    public interface IBaseValueSet
    {
        /// <summary>
        /// The object type of the values that will be available in the value set that is
        /// returned by the Values property and the GetValues function.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Returns the number of possible indices (dimensions) for the value set.
        /// </summary>
        /// <returns>number of indices, zero based</returns>
        int NumberOfIndices { get; }

        /// <summary>
        /// Returns the length (max index count) of the dimension specified by the
        /// given indices. To get the size of the first dimension, use a zero-length
        /// integer array as input argument. Length of indices must be a least one
        /// smaller than the <see cref="NumberOfIndices"/>
        /// </summary>
        /// <param name="indices">indices of the dimension to get the length of</param>
        /// <returns>length of the specified dimension</returns>
        int GetIndexCount(int[] indices);

        /// <summary>
        /// Returns the value object specified by the given array of indices.
        /// The length of the array of indices is N, so that the index for
        /// each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
        /// </summary>
        ///<param name="indices">indices index value for each dimension</param>
        ///<returns>the value object for the given indices</returns>
        Object GetValue(int[] indices);

        /// <summary>
        /// Set the value object specified by the given array of indices.
        /// The length of the array of indices is N, so that the index for
        /// each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
        /// </summary>
        /// <param name="indices">indices index value for each dimension</param>
        /// <param name="value">value the value object for the given indices</param>
        void SetValue(int[] indices, Object value);

    }
}