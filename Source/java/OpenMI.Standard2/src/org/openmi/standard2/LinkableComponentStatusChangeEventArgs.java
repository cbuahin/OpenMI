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
 * The LinkableComponentStatusChangeEventArgs contains the information that will
 * be passed when the {@link IBaseLinkableComponent} fires the <code>statusChanged</code> event.
 * <p/>
 * Note: Sending exchange item events is optional, so it should not be used as a
 * mechanism to build critical functionality upon.
 */
public class LinkableComponentStatusChangeEventArgs {

    private IBaseLinkableComponent linkableComponent = null;
    private LinkableComponentStatus oldStatus = LinkableComponentStatus.INVALID;
    private LinkableComponentStatus newStatus = LinkableComponentStatus.INVALID;
    private String message = "";


    /**
     * Creates a new instance with an empty string message.
     */
    public LinkableComponentStatusChangeEventArgs() {
        message = "";
    }


    /**
     * Gets the linkable component that fired the status change event.
     *
     * @return Sender of the event
     */
    public IBaseLinkableComponent getLinkableComponent() {
        return linkableComponent;
    }


    /**
     * Sets the linkable component that fired the status change event.
     *
     * @param linkableComponent to set
     */
    public void setLinkableComponent(IBaseLinkableComponent linkableComponent) {
        this.linkableComponent = linkableComponent;
    }


    /**
     * Gets the linkable component's status before the status change.
     *
     * @return previous component state
     */
    public LinkableComponentStatus getOldStatus() {
        return oldStatus;
    }


    /**
     * Sets the status of the linkable component before the status change.
     *
     * @param oldStatus of the linkable component
     */
    public void setOldStatus(LinkableComponentStatus oldStatus) {
        this.oldStatus = oldStatus;
    }


    /**
     * Gets the status of the linkable component after the status change.
     *
     * @return new status of the linkable component
     */
    public LinkableComponentStatus getNewStatus() {
        return newStatus;
    }


    /**
     * Sets the status of the linkable component after the status change.
     *
     * @param newStatus to set
     */
    public void setNewStatus(LinkableComponentStatus newStatus) {
        this.newStatus = newStatus;
    }


    /**
     * Gets the message providing additional information on the status
     * change. If there is no message, an empty string should be returned.
     *
     * @return the event message
     */
    public String getMessage() {
        return message;
    }


    /**
     * Sets the message providing additional information on the status
     * change. If there is no message, an empty string should be used.
     *
     * @param message to set
     */
    public void setMessage(String message) {
        this.message = message;
    }
}
