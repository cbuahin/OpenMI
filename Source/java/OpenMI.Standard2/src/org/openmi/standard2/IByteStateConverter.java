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
 * This interface is an optional complement to the {@link IManageState}
 * interface. Both are extensions to {@link IBaseLinkableComponent}, meant to
 * provide state management. It defines methods for converting a state into a
 * byte stream and reading in a state from a byte stream. This facilitates
 * external modules, e.g. a GUI or an operational control system, to save a
 * model's state somewhere as persistent state.
 */
public interface IByteStateConverter {

    /**
     * Converts the state with the stateId into a byte stream.
     *
     * @param stateId id of the state
     * @return The state identified by stateId as an array of bytes
     */
    public byte[] convertToByteArray(IIdentifiable stateId);


    /**
     * Creates a state from a byte stream and returns the identifier of this
     * state.
     * <p/>
     * Note: The state does not become the current state of the
     * {@link IBaseLinkableComponent}. For state management the {@link IManageState}
     * interface is to be used.
     *
     * @param byteArray State as a byte stream
     * @return IIdentifiable identifying the created state
     */
    public IIdentifiable convertFromByteArray(byte[] byteArray);
}
