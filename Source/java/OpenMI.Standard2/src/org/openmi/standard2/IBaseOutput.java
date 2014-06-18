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

import java.util.List;

/**
 * An output exchange item that can deliver values from an IBaseLinkableComponent.
 * <p/>
 * If an output does not provide the data in the way a consumer would like to
 * have it the output can be adapted by an {@link IBaseAdaptedOutput}, which can
 * transform the data according to the consumer's wishes. E.g. by performing
 * interpolation in time, spatial aggregation, etc.).
 */
public interface IBaseOutput extends IBaseExchangeItem {

    /**
     * Returns the list of IBaseInput that will consume data from this
     * output, by calling the getValues method. Every input that will call this
     * method needs to register itself to the output by calling the addConsumer
     * method first. If the input is not interested any longer it should remove
     * itself by calling the removeConsumer method.
     * <p/>
     * Note: Please be aware that the "unadulterated" values in the output
     * provided by the read only values property may be called anyway, even if
     * there are no values available.
     *
     * @return List of IBaseInput that consume data from the output
     */
    public List<IBaseInput> getConsumers();


    /**
     * Adds an IBaseInput as consumer to the IBaseOutput. After adding itself the
     * IBaseInput can used the getValues method to retrieve data from the output.
     * <p/>
     * If a consumer is added that can not be handled or that is incompatible
     * with the already added consumers an IllegalStateException will be thrown.
     * <p/>
     * The method must and will automatically set the consumer's provider.
     *
     * @param consumer The IBaseInput that will retrieve values from this IBaseOutput
     */
    public void addConsumer(IBaseInput consumer);


    /**
     * Removes an IBaseInput from the list of consumers of the IBaseOutput. When an
     * input is not interested any longer in calling the getValues method of the
     * IBaseOutput it should remove itself by calling this method.
     *
     * @param consumer The IBaseInput that no longer will be using the IBaseOutput
     */
    public void removeConsumer(IBaseInput consumer);


    /**
     * Returns the list of IBaseAdaptedOutput that have this IBaseOutput as
     * adaptee. This is the list of IBaseAdaptedOutput who's refresh method
     * will be called when the output's values have been updated. The list
     * should be unmodifiable, use the methods addAdaptedOutput and
     * removeAdaptedOutput to modify the collection.
     *
     * @return List of IBaseAdaptedOutput based on this IBaseOutput
     */
    public List<IBaseAdaptedOutput> getAdaptedOutputs();


    /**
     * Adds an IBaseAdaptedOutput to the collection of outputs that use data from
     * this IBaseOutput. Before using data it must be added by calling this method.
     * If an IBaseAdaptedOutput is added that can not be handled or that is
     * incompatible with the already added IBaseAdaptedOutput an exception will be
     * thrown.
     *
     * @param adaptedOutput IBaseAdaptedOutput that will use data from this IBaseOutput
     */
    public void addAdaptedOutput(IBaseAdaptedOutput adaptedOutput);


    /**
     * Removes an IBaseAdaptedOutput from the collection of outputs that use data
     * from this IBaseOutput. If an adapted output is not interested any longer
     * in this outputs item data, it should remove itself by calling this method.
     *
     * @param adaptedOutput IBaseAdaptedOutput that will no longer use this IBaseOutput
     */
    public void removeAdaptedOutput(IBaseAdaptedOutput adaptedOutput);


    /**
     * The current values of the IBaseOutput.
     *
     * @return IBaseValueSet with current IBaseOutput data
     */
    public IBaseValueSet getValues();


    /**
     * Provides the values matching the value definition specified by the
     * querySpecifier. Extensions can overwrite this base version to include
     * more details in the query, e.g. time and space.
     * <p/>
     * Note: One might expect the querySpecifier to be of the type IBaseInput,
     * because every input that calls the getValues method needs to add itself
     * as a consumer first. However the {@link IBaseExchangeItem} part of the
     * {@link IBaseInput} suffices to specify what is required. Therefore to have
     * the flexibility to loosen the "always register as a consumer" approach in
     * future releases of the Standard, it is chosen to provide an IBaseExchangeItem
     * as parameter.
     *
     * @param querySpecifier The IBaseExchangeItem definition of the requested values
     * @return IBaseValueSet with output data matching the specified query
     */
    public IBaseValueSet getValues(IBaseExchangeItem querySpecifier);
}
