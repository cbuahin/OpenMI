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
 * Qualitative data described items in terms of some quality or categorization
 * that may be 'informal' or may use relatively ill-defined characteristics such
 * as warmth and flavour. However, qualitative data can include well-defined
 * aspects such as gender, nationality or commodity type. OpenMI defines the
 * IQuality interface for working with qualitative data.
 * <p/>
 * An IQuality describes qualitative data, where a value is specified as one
 * category within a number of predefined (possible) categories. These
 * categories can be ordered or not.
 * <p/>
 * For qualitative data the {@link IBaseValueSet} exchanged between linkable components
 * contains one of the possible {@link ICategory} instances per data element.
 * <p/>
 * Examples:
 * - Colors: red, green, blue
 * - Land use: nature, recreation, industry, infrastructure
 * - Rating: worse, same, better
 */
public interface IQuality extends IValueDefinition {

    /**
     * Checks if the IQuality is defined by an ordered set of {@link ICategory} or not.
     *
     * @return True if the categories are ordered
     */
    public boolean isOrdered();


    /**
     * Returns a list of the possible {@link ICategory} allowed for this
     * IQuality. If the quality is not ordered the sequence of {@link ICategory} in
     * the returned list may vary. When the quality is ordered the items
     * in the list should always be in the same sequence and reflect the
     * ordering of the categories.
     *
     * @return List of ICategory defined by the IQuality
     */
    public List<ICategory> getCategories();
}
