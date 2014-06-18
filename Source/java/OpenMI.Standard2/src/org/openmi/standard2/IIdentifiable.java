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
 * Defines a method to get the Id of an OpenMI entity. The {@link IIdentifiable}
 * extends the {@link IDescribable} and therefore has, next to an id, a caption
 * and a description.
 */
public interface IIdentifiable extends IDescribable {

    /**
     * Returns the Id as a String. The Id must be unique within its context but
     * does not need to be globally unique. E.g. the id of an input exchange
     * item must be unique in the list of inputs of a {@link IBaseLinkableComponent},
     * but a similar Id might be used by an exchange item of another
     * {@link IBaseLinkableComponent}.
     *
     * @return Id as String
     */
    public String getId();
}
