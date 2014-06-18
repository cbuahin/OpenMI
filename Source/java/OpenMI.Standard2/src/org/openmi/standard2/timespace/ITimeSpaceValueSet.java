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

package org.openmi.standard2.timespace;

import org.openmi.standard2.IBaseValueSet;

import java.util.List;

/**
 * The ITimeSpaceValueSet represents an ordered two-dimensional
 * list of values. The first dimension stands for the times for which values
 * are available, whereas in the second dimension each value belongs to
 * precisely one element in the corresponding {@link IElementSet} (that was
 * specified when asking for the values). In other words, the i-th value in
 * that dimension of the value set corresponds to the i-th element in the
 * IElementSet.
 */
public interface ITimeSpaceValueSet extends IBaseValueSet {

    /**
     * Gets the two-dimensional list of values. The first List represents time,
     * and the contained List the element in the IElementSet.
     *
     * @return The two-dimensional list of values.
     */
    public List<List> getValues2D();


    /**
     * Sets the two-dimensional list of values. The first List represents time,
     * and the contained List the element in the IElementSet.
     *
     * @param values to set
     */
    public void setValues2D(List<List> values);


    /**
     * Gets the value for the specified timeIndex and elementIndex from getValues2D().
     * If the data is time independent, timeIndex must be specified as 0.
     * If the data is not related to a location, elementIndex must be specified as 0.
     *
     * @param timeIndex    to get the value for
     * @param elementIndex to get the value for
     * @return value for the specified indexes
     */
    public Object getValue(int timeIndex, int elementIndex);


    /**
     * Sets the value in getValues2D(), for the specified timeIndex and elementIndex.
     * If the data is time independent, timeIndex must be specified as 0.
     * If the data is not related to a location, elementIndex must be specified as 0.
     *
     * @param timeIndex    to set the value for
     * @param elementIndex to set the value for
     * @param value        to set for the specified indexes
     */
    public void setValue(int timeIndex, int elementIndex, Object value);


    /**
     * Gets values from getValues2D(), for all times, for the specified elementIndex.
     * If the data is not related to a location, elementIndex must be specified as 0.
     *
     * @param elementIndex to get the time series for
     * @return List with time series values
     */
    public List getTimeSeriesValuesForElement(int elementIndex);


    /**
     * Sets values in getValues2D(), for all times, for the specified elementIndex.
     * If the data is not related to a location, elementIndex must be specified as 0.
     *
     * @param elementIndex to set the time series values for
     * @param values       to set
     */
    public void setTimeSeriesValuesForElement(int elementIndex, List values);


    /**
     * Gets values from getValues2D(), for all elements, for the specified timeIndex.
     * If the data is time independent, timeIndex must be specified as 0.
     *
     * @param timeIndex to get the element series for
     * @return list the element series
     */
    public List getElementValuesForTime(int timeIndex);


    /**
     * Sets values in getValues2D(), for all elements, for the specified timeIndex.
     * If the data is time independent, timeIndex must be specified as 0.
     *
     * @param timeIndex to set the element series values for
     * @param values    to set
     */
    public void setElementValuesForTime(int timeIndex, List values);
}