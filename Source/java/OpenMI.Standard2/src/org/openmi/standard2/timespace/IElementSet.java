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

import org.openmi.standard2.IIdentifiable;

/**
 * Data exchange between components in OpenMI is nearly always related to one or
 * more elements in a space, either geo-referenced or not. An element set in
 * OpenMI can be a list of 2D or 3D spatial elements or, as a special case, a
 * list of ID based (non spatial) elements. The latter is supported to allow the
 * exchange of arbitrary data that is not related to space in any way. Possible
 * element types are defined in {@link ElementType}.
 * <p/>
 * An IElementSet is composed of an ordered list of elements having a common
 * type. The geometry of each element is described by an ordered list of
 * vertices. For 3D elements (i.e. polyhedrons) the shape can be queried by
 * face. When the element set is geo-referenced co-ordinates (X,Y,Z,M) can be
 * obtained for each vertex of an element.
 * <p/>
 * A geo-referenced element set needs to have a valid spatialReferenceSystemWkt
 * property set in the ISpatialDefinition. This is a string that
 * specifies the OGC Well-Known Text representation of the spatial reference. An
 * empty string indicates that there in no spatial reference, which is only
 * valid if the ElementType is ID_BASED.
 * <p/>
 * While an IElementSet can be used to query the geometric description of a
 * model schematization, it does not necessarily provide all topological
 * knowledge on inter-element connections.
 * <p/>
 * Although most models encapsulate static element sets, some advanced models
 * might contain dynamic elements (e.g. waves). A version number has been
 * introduced to enable tracking of element set changes over time. If the
 * version changes, the element set might need to be queried again during the
 * computation process.
 * <p/>
 * For element sets of type ElemenType.ID_BASED the spatialReferenceSystemWkt
 * property should be set to an empty string.
 */
public interface IElementSet extends ISpatialDefinition {

    /**
     * {@link ElementType} of the element set. All elements in the set are of
     * this type.
     *
     * @return ElementType of the element set
     */
    public ElementType getElementType();


    /**
     * Index of element with id elementId in the element set. Indices start from
     * zero. There are no restrictions to how elements are ordered.
     *
     * @param elementId Identification for the element for which the element index is
     *                  requested. If no element in the ElementSet has the specified
     *                  elementId, -1 must be returned.
     * @return int Index of the element with the specified id
     */
    public int getElementIndex(IIdentifiable elementId);


    /**
     * Returns the IIdentifiable of the 'ElementIndex'-th element in the
     * ElementSet. Indices start from zero. If the ElementType of the element
     * set is not ID_BASED, a null or an empty string may be returned.
     *
     * @param index Of the element for which the element id is requested. If the
     *              element index is outside the range [0, number of elements - 1]
     *              an IndexOutOfBoundsException must be thrown
     * @return IIdentifiable representing the id of the element
     * @throws IndexOutOfBoundsException when the specified index is invalid
     */
    public IIdentifiable getElementId(int index);


    /**
     * Number of vertices for the element specified by the element index.
     * <p/>
     * If the getVertexCount method is invoked for element sets of type
     * ElementType.ID_BASED an IllegalStateException must be thrown.
     *
     * @param elementIndex Index of the element for which the number of
     *                     vertices is requested. If the element index is outside the
     *                     range [0, number of elements - 1], an IndexOutOfBoundsException
     *                     must be thrown
     * @return number of vertices in the element defined by the element index
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when the specified index is invalid
     */
    public int getVertexCount(int elementIndex);


    /**
     * Returns the number of faces in a 3D element. For 2D elements this returns
     * 0. If the method is invoked for element sets of type ElementType.ID_BASED
     * an IllegalStateException must be thrown.
     *
     * @param elementIndex Index of the element to get the number of faces for. If the
     *                     element index is outside the range [0, number of elements - 1]
     *                     an IndexOutOfBoundsException must be thrown
     * @return number of faces in the element defined by the element index
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when the specified index is invalid
     */
    public int getFaceCount(int elementIndex);


    /**
     * Gives an array with the vertex indices for a face. If the method is
     * invoked for element sets of type ElementType.ID_BASED an
     * IllegalStateException must be thrown.
     * <p/>
     * Note that the vertex indices for a face must be locally numbered for the
     * element (containing numbers in the range [0,
     * getVertexCount(elementIndex)-1]).
     *
     * @param elementIndex Index of the element to get the vertex indices for
     * @param faceIndex    Index of the face to get the vertex indices for
     * @return Array with the vertex indices for the face of the element
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when a specified index is invalid
     */
    public int[] getFaceVertexIndices(int elementIndex, int faceIndex);


    /**
     * Checks if the element set supports Z coordinates.
     *
     * @return True if the element set supports Z coordinates
     */
    public boolean hasZ();


    /**
     * Checks if the element set supports M coordinates.
     *
     * @return True if the element set supports M coordinates
     */
    public boolean hasM();


    /**
     * X coordinate for the vertex with vertexIndex of the element with
     * elementIndex.
     *
     * @param elementIndex Index of the element to get the coordinate for
     * @param vertexIndex  Index of the vertex to get the coordinate for
     * @return requested coordinate value
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when a specified index is invalid
     */
    public double getVertexXCoordinate(int elementIndex, int vertexIndex);


    /**
     * Y coordinate for the vertex with vertexIndex of the element with
     * elementIndex.
     *
     * @param elementIndex Index of the element to get the coordinate for
     * @param vertexIndex  Index of the vertex to get the coordinate for
     * @return requested coordinate value
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when a specified index is invalid
     */
    public double getVertexYCoordinate(int elementIndex, int vertexIndex);


    /**
     * Z coordinate for the vertex with vertexIndex of the element with
     * elementIndex.
     *
     * @param elementIndex Index of the element to get the coordinate for
     * @param vertexIndex  Index of the vertex to get the coordinate for
     * @return requested coordinate value
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when a specified index is invalid
     */
    public double getVertexZCoordinate(int elementIndex, int vertexIndex);


    /**
     * M coordinate for the vertex with vertexIndex of the element with
     * elementIndex.
     *
     * @param elementIndex Index of the element to get the coordinate for
     * @param vertexIndex  Index of the vertex to get the coordinate for
     * @return requested coordinate value
     * @throws IllegalStateException     when the element set is ID_BASED
     * @throws IndexOutOfBoundsException when a specified index is invalid
     */
    public double getVertexMCoordinate(int elementIndex, int vertexIndex);
}