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
 * The LinkableComponentStatus enumerates the possible statuses that an
 * IBaseLinkableComponent can be in. A state diagram showing the possible statuses
 * and the transitions from one status to another can be found in the
 * documentation on the OpenMI 2.0.
 * <p/>
 * They are also mentioned in the documentation of the various methods of the
 * {@link IBaseLinkableComponent} interface.
 */
public enum LinkableComponentStatus {

    /**
     * The linkable component instance has just been created. This status must
     * and will be followed by INITIALIZING.
     */
    CREATED,

    /**
     * The linkable component is initializing itself. This status will end in a
     * status change to INITIALIZED or INVALID.
     */
    INITIALIZING,

    /**
     * The linkable component has successfully initialized itself. The relations
     * between its inputs/outputs and those of other components can be
     * established. This status will end in a status change to INITIALIZED or
     * FAILED.
     */
    INITIALIZED,

    /**
     * After relations between the component's inputs/outputs and those of other
     * components have been established, the component is validating whether its
     * required input will be available when it will update itself, and whether
     * indeed it will be able to provide the required output during this update.
     * <p/>
     * This VALIDATING status will end in a status change to VALID or INVALID.
     */
    VALIDATING,

    /**
     * The linkable component is in a valid state. When updating itself its
     * required input will be available, and it it will be able to provide the
     * required output.
     */
    VALID,

    /**
     * The linkable component is in an invalid state. When updating itself not
     * all required input will be available, and/or it will not be able to
     * provide the required output.
     */
    INVALID,

    /**
     * The component is preparing itself for the first getValues call. This
     * PREPARING state will end in a status change to UPDATED or FAILED.
     */
    PREPARING,

    /**
     * The linkable component wants to update itself, but is not able yet to
     * perform the actual computation, because it is still waiting for input
     * data from other components.
     */
    WAITING_FOR_DATA,

    /**
     * The linkable component is updating itself. It has received all required
     * input data from other components, and is now performing the actual
     * computation.
     * <p/>
     * This UPDATING state will end in a status change to UPDATED, DONE or
     * INVALID.
     */
    UPDATING,

    /**
     * The linkable component has successfully updated itself.
     */
    UPDATED,

    /**
     * The last update process that the component performed was the final one. A
     * next call to the update method will leave the component's internal state
     * unchanged.
     */
    DONE,

    /**
     * The linkable component was requested to perform the actions to be
     * performed before it will either be disposed or re-intialized again.
     * Typical actions would be writing the final result files, close all open
     * files, free memory, etc. When all required actions have been performed,
     * the status switches to CREATED when re-initialization is possible. The
     * status switches to FINISHED when the component is to be disposed.
     */
    FINISHING,

    /**
     * The linkable component has successfully performed its finalization
     * actions. Re-initialization of the component instance is not possible and
     * should not be attempted. Instead the instance should be disposed, e.g.
     * through the garbage collection mechanism.
     */
    FINISHED,

    /**
     * The linkable component has failed initialize itself, failed to prepare
     * itself for computation, or failed to complete its update process.
     */
    FAILED
}
