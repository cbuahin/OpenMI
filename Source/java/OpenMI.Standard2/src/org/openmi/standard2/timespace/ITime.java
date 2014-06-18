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

/**
 * Time interface based on a Modified Julian Date (number and fraction of days
 * since 00:00 November 17, 1858).
 * <p/>
 * The ITime interface supports a time stamp as well as a time interval. A time
 * stamp will have its getDurationInDays set to 0, while a time interval will
 * have a greater than zero getDurationInDays value.
 */
public interface ITime {

    /**
     * Gets the current value of the time stamp.
     *
     * @return Time stamp as a Modified Julian Day value
     */
    public double getStampAsModifiedJulianDay();


    /**
     * Gets the current duration of the time interval.
     *
     * @return duration in days, O if time is a time stamp
     */
    public double getDurationInDays();
}