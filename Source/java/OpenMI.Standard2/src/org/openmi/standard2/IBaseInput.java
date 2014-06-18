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
 * An input item that can accept values for an {@link IBaseLinkableComponent}.
 */
public interface IBaseInput extends IBaseExchangeItem {

    /**
     * Gets the provider this input should get its values from.
     *
     * @return IBaseOutput providing the values for this input
     */
    public IBaseOutput getProvider();


    /**
     * Sets the provider this input should get its values from.
     *
     * @param provider The IBaseOutput providing the values for this input
     */
    public void setProvider(IBaseOutput provider);


    /**
     * Gets the exchange item's values.
     *
     * @return IBaseValueSet with the current values of the exchange item
     */
    public IBaseValueSet getValues();


    /**
     * Sets the exchange item's values.
     *
     * @param values The IBaseValueSet with the new exchange item values
     */
    public void setValues(IBaseValueSet values);
}
