/*
 * Copyright (c) 2005-2010, OpenMI Association
 * <http://www.openmi.org/>
 *
 * This file is part of openmi-standard2.jar
 *
 * openmi-standard2.jar is free software; you can redistribute it and/or
 * modify it under the terms of the Lesser GNU General Public License as
 * published by the Free Software Foundation; either version 3 of the
 * License, or (at your option) any later version.
 *
 * openmi-standard2.jar is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the Lesser GNU
 * General Public License for more details.
 *
 * You should have received a copy of the Lesser GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

package org.openmi.standard2;

import java.lang.reflect.Type;

/**
 * The IBaseValueSet represents a general multi-dimensional set of
 * values. Each value is of type ValueType.
 * <p/>
 * The size of each dimension can vary, depending on the indices provided, e.g.
 * in a 2D matrix each row can have different lengths. Example, assuming the data
 * is stored as a double[][] matrix, then matrix[0].length need not equal
 * matrix[1].length.
 */
public interface IBaseValueSet {

    /**
     * The object type of the values that will be available in the value set that is
     * returned by the value property and the getValue method.
     *
     * @return Type of object used for values
     */
    public Type getValueType();


    /**
     * Returns the number of possible indices (dimensions) for the value set.
     *
     * @return number of indices, zero based
     */
    public int getNumberOfIndices();


    /**
     * Returns the length (max index count) of the dimension specified by the
     * given indices.
     * <p/>
     * To get the size of the first dimension, use a zero-length
     * integer array as input argument. Length of indices must be a least one
     * smaller than the {@link #getNumberOfIndices()}.
     *
     * @param indices of the dimension to get the length of
     * @return length of the specified dimension
     */
    public int getIndexCount(int[] indices);


    /**
     * Returns the value object specified by the given array of indices.
     * The length of the array of indices is N, so that the index for
     * each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
     *
     * @param indices index value for each dimension
     * @return the value object for the given indices
     */
    public Object getValue(int[] indices);


    /**
     * Set the value object specified by the given array of indices.
     * The length of the array of indices is N, so that the index for
     * each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
     *
     * @param indices index value for each dimension
     * @param value   the value object for the given indices
     */
    public void setValue(int[] indices, Object value);
}
