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
 * The ICategory describes one item of a possible categorization. It is used by
 * the {@link IQuality} interface for describing qualitative data.
 * <p/>
 * For qualitative data the {@link IBaseValueSet} exchanged between linkable components
 * contains one of the possible {@code ICategory} instances per data element.
 * <p/>
 * A category defines one "class" within a "set of classes".
 */
public interface ICategory extends IDescribable {

    /**
     * Value for this category.
     * <p/>
     * Example: "blue" in a "red"/"green"/"blue" set.
     *
     * @return Object representing the value
     */
    public Object getValue();
}
