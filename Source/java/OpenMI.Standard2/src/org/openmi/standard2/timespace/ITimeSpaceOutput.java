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

import org.openmi.standard2.IBaseExchangeItem;
import org.openmi.standard2.IBaseOutput;

/**
 * An output exchange item that can deliver values from a time / space dependent linkable component.
 * The output is a combination of an {@link org.openmi.standard2.IValueDefinition},
 * an {@link org.openmi.standard2.timespace.IElementSet},
 * and an {@link org.openmi.standard2.timespace.ITimeSet}. This combination specifies which type
 * of data can be provided where and when by the linkable component.
 * <p/>
 * If an output does not provide the data in the way a consumer would
 * like to have it the output can be adapted by an {@link ITimeSpaceAdaptedOutput}, which can
 * transform the data according to the consumer's wishes. E.g. by performing
 * interpolation in time, spatial aggregation, etc.).
 * <p/>
 * <p/>If a output item does provide the data in the way a consumer would lik to have it
 * the output item can be decorated by an {@link ITimeSpaceAdaptedOutput}, which can
 * transform the data according to the consumer's wishes (e.g. by performing interpolation in time,
 * spatial interpolation etc.).
 */
public interface ITimeSpaceOutput extends ITimeSpaceExchangeItem, IBaseOutput {

    /**
     * The exchange item's values, as a specialized {@link ITimeSpaceValueSet}.
     *
     * @return current values of the output
     */
    @Override
    public ITimeSpaceValueSet getValues();


    /**
     * Overridden version of the getValues method of {@link IBaseOutput} returning
     * an {@link ITimeSpaceValueSet} instead of an {@link org.openmi.standard2.IBaseValueSet}.
     *
     * @param querySpecifier The IBaseExchangeItem definition of the requested values
     * @return requested values from the output
     */
    @Override
    public ITimeSpaceValueSet getValues(IBaseExchangeItem querySpecifier);
}