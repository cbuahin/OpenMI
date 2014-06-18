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

import org.openmi.standard2.IBaseInput;

/**
 * An input item that can accept values for an {@link ITimeSpaceComponent}.
 * <p/>
 * The item is a combination of an {@link org.openmi.standard2.IValueDefinition},
 * an {@link IElementSet}, and an {@link ITimeSet}.
 * This combination specifies which type of data is required, where and
 * when, as input for an {@link ITimeSpaceComponent}.
 */
public interface ITimeSpaceInput extends ITimeSpaceExchangeItem, IBaseInput {

    /**
     * The exchange item's values, as a specialized {@link ITimeSpaceValueSet}.
     *
     * @return current values of the input
     */
    @Override
    public ITimeSpaceValueSet getValues();
}