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
 * Unit interface, describing the physical unit of a {@link IQuantity}.
 */
public interface IUnit extends IDescribable {

    /**
     * The unit's dimension.
     *
     * @return IDimension
     */
    public IDimension getDimension();


    /**
     * Conversion factor to SI ('A' in: SI-value = A * quantity-value + B).
     *
     * @return SI conversion factor
     */
    public double getConversionFactorToSI();


    /**
     * Offset to SI ('B' in: SI-value = A * quantity-value + B).
     *
     * @return SI offset
     */
    public double getOffsetToSI();
}
