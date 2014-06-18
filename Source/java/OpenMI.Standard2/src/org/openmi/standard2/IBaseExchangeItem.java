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

import java.util.Observable;

/**
 * An item that can be exchanged, either as input or as output.
 * <p/>
 * Note: This interface is not to be implemented directly, any class is to
 * implement either the IBaseInput or the IBaseOutput derived interface.
 */
public interface IBaseExchangeItem extends IIdentifiable {

    /**
     * Definition of the values in the exchange item.
     * <p/>
     * The {@link IValueDefinition} should never be returned directly; all
     * implementing classes should return either an {@link IQuality}, an
     * {@link IQuantity}, or a custom derived value definition interface.
     *
     * @return Definition of the values in the exchange item
     */
    public IValueDefinition getValueDefinition();


    /**
     * Gets the owner of the exchange item. For an output exchange item this is
     * the component responsible for providing the content of the output item.
     * It is possible for an exchange item to have no owner, in this case the
     * method will return null.
     *
     * @return ILinkableComponent owning the exchange item, can be null
     */
    public IBaseLinkableComponent getComponent();


    /**
     * Returns an Observable to be used to receive notifications from the
     * exchange item about content changes. This notification is sent when
     * the exchange item content has been updated. It is the responsibility
     * of the observer to figure out which actual values have changed and
     * how to act on these changes.
     * <p/>
     * The argument passed in the Observer update call must be an instance of
     * the ExchangeItemChangeEventArgs class.
     *
     * @return Observable for receiving exchange item change notifications
     */
    public Observable getItemChangedObservable();
}
