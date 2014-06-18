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


import java.util.List;

/**
 * A set of time stamps or time intervals, used to indicate where an output has
 * values and can provide values, and where an input does or may require values.
 * <p/>
 * The hasDurations method indicates whether the set contains stamps or
 * intervals.
 */
public interface ITimeSet {

    /**
     * Time stamps or spans as available in the values of an output, or as
     * required by an input.
     * <p/>
     * Specific values:
     * <p/>
     * - zero length list in case of output: time dependent item, but no values
     * available yet
     * <p/>
     * - zero length list in case of input: time dependent item, but currently
     * no values required
     * <p/>
     *
     * @return list of ITime instances
     */
    public List<ITime> getTimes();


    /**
     * True if the times have durations, i.e., are time intervals. In this case,
     * a duration value greater then zero is expected for every ITime in the
     * getTimes list.
     *
     * @return True if ITime in the set specify durations
     */
    public boolean hasDurations();


    /**
     * Time zone offset from UTC, expressed in the number of hours. Since some
     * of the world's time zones differ half an hour from their neighbours the
     * value is specified as a double.
     *
     * @return time zone offset from UTC
     */
    public double getOffsetFromUtcInHours();


    /**
     * The time horizon defines for an input for what time span it may require
     * values. This means the providers of this input can assume that the input
     * never goes back further in time than the time horizon's begin time {@code #getTimeHorizon()
     * .getStampAsModifiedJulianDay()}. Also, it will never go further ahead than the
     * time horizon's end time {@code #getTimeHorizon().getStampAsModifiedJulianDay() +
     * getTimeHorizon().getDurationInDays()}. For an output item, and thus for an adapted
     * output item, the time horizon indicates in what time span the item can provide
     * values.
     * <p/>
     * Specific values:
     * <p/>
     * - getTimeHorizon().getStampAsModifiedJulianDay == Double.NEGATIVE_INFINITY: far back in time
     * <p/>
     * - getTimeHorizon().getStampAsModifiedJulianDay == Double.POSITIVE_INFINITY: far in the future
     *
     * @return time horizon specified as an ITime
     */
    public ITime getTimeHorizon();
}