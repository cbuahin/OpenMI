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

package org.openmi.standard2.timespace;

import org.openmi.standard2.IDescribable;

/**
 * Data in components in OpenMI is often related to spatial coordinates,
 * either geo-referenced or not. The {@link ISpatialDefinition}
 * is the general spatial construct that all other spatial constructions
 * extend from.
 * <p/>
 * The currently most noticeable extending interfaces is the {@link IElementSet},
 * which in previous versions of the standard was the only spatial construction, and
 * which all other spatial constructions had to be wrapped into, whereas in the current
 * version the {@link IElementSet} is an extension of the {@link ISpatialDefinition}.
 * <p/>
 * Although most models encapsulate data with a static spatial definition, some advanced
 * models might contain dynamic spatial definitions (e.g. waves, moving grids). The
 * Version number has been introduced to enable tracking of spatial changes
 * over time. If the version changes, the spatial definition might need to be queried again
 * during the computation process.
 */
public interface ISpatialDefinition extends IDescribable {

    /**
     * The SpatialReferenceSystemWkt specifies the OGC Well-Known Text
     * representation of the spatial reference to be used in association with
     * the coordinates in the ElementSet.
     * <p/>
     * For ElementSets of type ElementType.ID_BASED the
     * SpatialReferenceSystemWkt property is an empty string.
     * <p/>
     * For the list of WKT strings see http://www.spatialreference.org.
     * <p/>
     * Example: For all ElementSet Types except ElementType.ID_BASED a spatial
     * reference can be defined in a form: <code>
     * PROJCS["Mercator Spheric", GEOGCS["WGS84based_GCS", DATUM["WGS84based_Datum", SPHEROID["WGS84based_Sphere", 6378137, 0], TOWGS84[0, 0, 0, 0, 0, 0, 0]], PRIMEM["Greenwich", 0, AUTHORITY["EPSG", "8901"]], UNIT["degree", 0.0174532925199433, AUTHORITY["EPSG", "9102"]], AXIS["E", EAST], AXIS["N", NORTH]], PROJECTION["Mercator"], PARAMETER["False_Easting", 0], PARAMETER["False_Northing", 0], PARAMETER["Central_Meridian", 0], PARAMETER["Latitude_of_origin", 0], UNIT["metre", 1, AUTHORITY["EPSG", "9001"]], AXIS["East", EAST], AXIS["North", NORTH]]
     * </code>
     *
     * @return WKT string for the IElementSet
     */
    public String getSpatialReferenceSystemWkt();


    /**
     * Number of data elements in the spatial axis.
     *
     * @return int Number of data elements
     */
    public int getElementCount();


    /**
     * The current version number for the spatial axis. The version
     * must be incremented if anything inside the spatial axis is changed,
     * or if an entirely new spatial axis is provided.
     *
     * @return int Version number for the spatial axis
     */
    public int getVersion();
}