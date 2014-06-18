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
 * Used to create instances of {@link IBaseAdaptedOutput} items. An IBaseAdaptedOutput
 * can be used to make another IBaseAdaptedOutput or an IBaseOutput suitable for new use
 * or purpose. If possible the factory on request provides an adaptor that fits
 * an existing output to a specified target input.
 */
public interface IAdaptedOutputFactory extends IIdentifiable {

    /**
     * Gets a list of identifiers of the available {@link IBaseAdaptedOutput} that
     * can make the adaptee match the target. If the target is <code>null</code>
     * the identifiers of all IBaseAdaptedOutput that can adapt the adaptee are
     * returned.
     *
     * @param adaptee IBaseOutput to adapt
     * @param target  IBaseInput to adapt the adaptee to, can be null
     * @return Array of identifiers for the available IBaseAdaptedOutput
     */
    public IIdentifiable[] getAvailableAdapterIds(IBaseOutput adaptee, IBaseInput target);


    /**
     * Creates a {@link IBaseAdaptedOutput} that adapts the adaptee so that it fits
     * the target.
     * <p/>
     * The adaptedOutputId used must be one of the IIdentifiable instances
     * returned by the getAvailableAdapterIds method.
     * <p/>
     * The returned IBaseAdaptedOutput will already be registered with the adaptee.
     *
     * @param adaptedOutputId The identifier of the IBaseAdaptedOutput to create
     * @param adaptee         IBaseOutput to adapt
     * @param target          IBaseInput to adapt the adaptee to, can be null
     * @return IAdaptedOutput created
     */
    public IBaseAdaptedOutput createAdaptedOutput(IIdentifiable adaptedOutputId,
                                                  IBaseOutput adaptee, IBaseInput target);
}
