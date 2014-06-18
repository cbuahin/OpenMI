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
 * A value definition describes a value returned by the getValues method of a
 * {@link IBaseExchangeItem}. This interface is not meant to be implemented
 * directly. Instead, implement either {@link IQuality} or {@link IQuantity}, or
 * a custom derived {@link IValueDefinition} interface.
 */
public interface IValueDefinition extends IDescribable {

    /**
     * The type of object used to store values in the {@link IBaseValueSet} that is
     * returned by the getValues method of the {@link IBaseExchangeItem}.
     *
     * @return Type of object used for values
     */
    public Type getValueType();


    /**
     * The value representing that data is missing.
     *
     * @return missing data value
     */
    public Object getMissingDataValue();

}
