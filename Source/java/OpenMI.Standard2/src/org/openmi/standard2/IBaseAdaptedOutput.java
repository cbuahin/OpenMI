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
 * An IBaseAdaptedOutput adds one or more data operations on top of an output item.
 * It is in itself an {@link IBaseOutput}. The adapted output extends an output item
 * with functionality as spatial interpolation, temporal interpolation, unit
 * conversion, etc.
 * <p/>
 * IBaseAdaptedOutput instances are created by means of an
 * {@link IAdaptedOutputFactory}.
 * <p/>
 * Note: The IBaseAdaptedOutput is based on the adapter design pattern. It adapts an
 * IBaseOutput or another IBaseAdaptedOutput to make it suitable for new use or purpose.
 * The object being adapted is typically called the "adaptee". The
 * IBaseAdaptedOutput replaces the DataOperation that was used in OpenMI 1.x.
 */
public interface IBaseAdaptedOutput extends IBaseOutput {

    /**
     * Arguments needed to let the adapted output do its work. An unmodifiable
     * list of the (modifiable) arguments should be returned that can be used to
     * get info on the arguments and to modify argument values. Validation of
     * changes can be done either when they occur (e.g. using notifications) or
     * when the initialize method is called. Initialize will always be called
     * before any call to the refresh method of the IBaseAdaptedOutput.
     *
     * @return Unmodifiable list of IArgument for the adapted output
     */
    public List<IArgument> getArguments();


    /**
     * Let the adapted output initialize itself, based on the current values
     * specified by the arguments. Only after initialize is called the refresh
     * method might be called.
     * <p/>
     * A component must invoke the {@link #initialize()} method of all its
     * adapted outputs at the end of the component's prepare phase.
     * In case of stacked adapted outputs, the adaptee must be initialized first.
     */
    public void initialize();


    /**
     * Output item that this adapted output extracts content from.
     * <p/>
     * Note: In the adapter design pattern, it is the item being adapted.
     *
     * @return IBaseOutput this adapted output gets its data from
     */
    public IBaseOutput getAdaptee();


    /**
     * Sets the output item that this adapted output extracts content from.
     * <p/>
     * Note: In the adapter design pattern this is the item being adapted.
     *
     * @param output the adapted output gets its data from
     */
    public void setAdaptee(IBaseOutput output);


    /**
     * Request the adapted output to refresh itself. This method will be called
     * by the adaptee, when it has been refreshed/updated. In the implementation
     * of the refresh method the adapted output should update its contents
     * according to the changes in the adaptee.
     * <p/>
     * After updating itself the adapted output must call refresh on all its
     * adapted outputs, so the chain of outputs refreshes itself.
     */
    public void refresh();
}
