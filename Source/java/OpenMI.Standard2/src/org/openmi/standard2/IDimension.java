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

/**
 * Defines the order of dimension in each {@link DimensionBase} for a {@link IUnit}.
 */
public interface IDimension {

    /**
     * Enumeration for base dimensions.
     */
    public enum DimensionBase {

        /**
         * Base dimension length.
         */
        LENGTH,
        /**
         * Base dimension mass.
         */
        MASS,
        /**
         * Base dimension time.
         */
        TIME,
        /**
         * Base dimension electric current.
         */
        ELECTRIC_CURRENT,
        /**
         * Base dimension temperature.
         */
        TEMPERATURE,
        /**
         * Base dimension amount of substance.
         */
        AMOUNT_OF_SUBSTANCE,
        /**
         * Base dimension luminous intensity.
         */
        LUMINOUS_INTENSITY,
        /**
         * Base dimension currency.
         */
        CURRENCY
    }


    /**
     * Returns the power for the requested dimension.
     * <p/>
     * Example: For a quantity such as flow, which may have the unit m3/s, the
     * getPower method must work as follows:
     * <p/>
     * dimension.getPower(DimensionBase.AMOUNT_OF_SUBSTANCE) -->returns 0
     * dimension.getPower(DimensionBase.CURRENCY) --> returns 0
     * dimension.getPower(DimensionBase.ELECTRIC_CURRENT) --> returns 0
     * dimension.getPower(DimensionBase.LENGTH) --> returns 3
     * dimension.getPower(DimensionBase.LUMINOUS_INTENSITY) --> returns 0
     * dimension.getPower(DimensionBase.MASS) --> returns 0
     * dimension.getPower(DimensionBase.TEMPERATURE) --> returns 0
     * dimension.getPower(DimensionBase.TIME) --> returns -1
     *
     * @param baseQuantity to get the power of
     * @return double Power of the requested DimensionBase
     */
    public double getPower(DimensionBase baseQuantity);
}
