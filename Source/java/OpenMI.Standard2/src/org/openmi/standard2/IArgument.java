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
import java.util.List;

/**
 * The IArgument interface is used to set the arguments of a
 * {@link IBaseLinkableComponent} and the arguments of an {@link IBaseAdaptedOutput}.
 */
public interface IArgument extends IIdentifiable {

    /**
     * The type of the value of the argument, E.g. a integral type like string,
     * integer or double, or a non integral type, such as a time series object.
     *
     * @return type of the value of the argument
     */
    public Type getValueType();


    /**
     * Specifies whether the argument is optional or not. If the
     * <code>value</code> property returns null and
     * <code>isOptional == false</code>, a value has to be set before the
     * argument can be used.
     *
     * @return true when the argument is optional, false if it is required
     */
    public boolean isOptional();


    /**
     * Defines whether the value property may be edited. This is used to let an
     * IBaseLinkableComponent or an IBaseAdaptedOutput present the actual value of an
     * argument that can not be changed by the user, but is needed to determine
     * the values of other arguments or is informative in any other way.
     *
     * @return true if the argument is editable, false if it is not
     */
    public boolean isReadOnly();


    /**
     * The current value of the argument. If no value has been set yet, a
     * default value is returned. If <code>null</code> is returned, this means
     * that the default value is <code>null</code>.
     *
     * @return value of the argument
     */
    public Object getValue();


    /**
     * Set the value of the argument.
     *
     * @param value The new value.
     */
    public void setValue(Object value);


    /**
     * The default value of the argument. This value will be returned by the getValue
     * method if not specific value has been set for the argument.
     *
     * @return default argument value
     */
    public Object getDefaultValue();


    /**
     * List of possible and allowed values for this argument. If for integral
     * types or component specific types all possible values are allowed,
     * <code>null</code> is returned. A list with length 0 indicates that there
     * is indeed a limitation on the possible values, but that currently no
     * values are possible. Effectively this means that the value will not and
     * cannot be set.
     *
     * @return List of possible values for this argument
     */
    public List<Object> getPossibleValues();


    /**
     * The argument's value, represented as a string. If <code>ValueType</code>
     * indicates that the argument's value is not of the type string, this
     * function offers the possibility to treat it as a string, e.g. to let the
     * GUI persist the value in the composition file.
     *
     * @return string representing the current argument's value
     */
    public String getValueAsString();


    /**
     * Attempts to set the argument's value from the specified string. If the
     * <code>ValueType</code> indicates that the argument's value is not of the
     * type string, this function will try to decode the given string into a
     * proper value. In this case it serves as the counter part for the
     * getValueAsString method.
     *
     * @param value string to decode into the new argument's value
     */
    public void setValueAsString(String value);
}
