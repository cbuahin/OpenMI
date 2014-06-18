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
 * The ExchangeItemChangeEventArgs contains the information that will
 * be passed when the {@link IBaseExchangeItem} fires the <code>itemChanged</code> event.
 * <p/>
 * Note: Sending exchange item events is optional, so it should not be used as a
 * mechanism to build critical functionality upon.
 */
public class ExchangeItemChangeEventArgs {

    private IBaseExchangeItem exchangeItem;
    private String message;


    /**
     * Default constructor. Creates a new instance with an empty message and
     * null as exchangeItem. Properties need to be set before actually using the
     * instance.
     */
    public ExchangeItemChangeEventArgs() {
        this(null, "");
    }


    /**
     * Constructor that also initializes the exchangeItem and message properties.
     *
     * @param exchangeItem
     * @param message
     */
    public ExchangeItemChangeEventArgs(IBaseExchangeItem exchangeItem, String message) {
        this.exchangeItem = exchangeItem;
        this.message = message;
    }


    /**
     * Gets the exchange item of which the status has been changed.
     *
     * @return IExchangeItem the event is send for
     */
    public IBaseExchangeItem getExchangeItem() {
        return exchangeItem;
    }


    public void setExchangeItem(IBaseExchangeItem exchangeItem) {
        this.exchangeItem = exchangeItem;
    }


    /**
     * Gets the message describing the way in which the status of the exchange item
     * has been changed.
     *
     * @return String message
     */
    public String getMessage() {
        return message;
    }


    public void setMessage(String message) {
        this.message = message;
    }
}
