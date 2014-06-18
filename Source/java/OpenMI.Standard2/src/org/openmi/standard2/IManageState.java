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
 * Optional interface to be implemented by components in addition to the
 * IBaseLinkableComponent interface. It provides additional methods for handling
 * component state so it can be saved, restored and cleared. Note that it can be
 * left completely to the component to handle persistence of state or it can
 * also implement {@link IByteStateConverter} and provide ways for state to be
 * converted to and from an array of bytes. A third-party could then handle the
 * saving and loading of state data.
 */
public interface IManageState {

    /**
     * Requests storage of the current state. The returned Id can be used to
     * restore or delete the state data.
     *
     * @return IIdentifiable Id of the stored state
     */
    public IIdentifiable keepCurrentState();


    /**
     * Restores the state identified by the stateId. If the stateId is not known
     * an {@link IllegalArgumentException} must be thrown.
     *
     * @param stateId IIdentifiable indicating the state to be restored
     * @throws IllegalArgumentException when stateId is unknown
     */
    public void restoreState(IIdentifiable stateId);


    /**
     * Clears a state and releases any resources it occupies (e.g. memory, disk
     * space). If the stateId is not known an {@link IllegalArgumentException}
     * must be thrown.
     *
     * @param stateId IIdentifiable indicating the state to be cleared
     * @throws IllegalArgumentException when stateId is unknown
     */
    public void clearState(IIdentifiable stateId);
}
