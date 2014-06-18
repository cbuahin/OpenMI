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
 * Provides descriptive information on an OpenMI entity.
 * <p/>
 * An entity that is describable has a caption (title or heading) and a
 * description. These are not to be used for identification (see
 * {@link IIdentifiable}).
 */
public interface IDescribable {

    /**
     * Gets the caption text (not to be used as an id).
     *
     * @return String with the caption
     */
    public String getCaption();


    /**
     * Sets the caption text of the entity.
     *
     * @param caption text
     */
    public void setCaption(String caption);


    /**
     * Gets additional descriptive information about the entity.
     *
     * @return String with the information
     */
    public String getDescription();


    /**
     * Sets the description about the entity.
     *
     * @param description text
     */
    public void setDescription(String description);
}
