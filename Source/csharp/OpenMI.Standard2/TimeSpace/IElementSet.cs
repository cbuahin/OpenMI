#region Copyright

/*
    Copyright (c) 2005-2010, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard2.dll

    OpenMI.Standard2.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard2.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

namespace OpenMI.Standard2.TimeSpace
{
  /// <summary>
  /// Data exchange between components in OpenMI is nearly always related to one or
  /// more elements in a space, either geo-referenced or not. An element set in
  /// OpenMI can be a list of 2D or 3D spatial elements or, as a special case, a
  /// list of ID based (non spatial) elements. The latter is supported to allow the
  /// exchange of arbitrary data that is not related to space in any way. Possible
  /// element types are defined in <see cref="ElementType"/>.
  /// <para>
  /// An IElementSet is composed of an ordered list of elements having a common
  /// type. The geometry of each element is described by an ordered list of
  /// vertices. For 3D elements (i.e. polyhedrons) the shape can be queried by
  /// face. When the element set is geo-referenced co-ordinates (X,Y,Z,M) can be
  /// obtained for each vertex of an element.
  /// </para>
  /// <para>
  /// A geo-referenced element set needs to have a valid SpatialReferenceSystemWkt
  /// property set in the <see cref="ISpatialDefinition"/>. This is a string that
  /// specifies the OGC Well-Known Text representation of the spatial reference. An
  /// empty string indicates that there in no spatial reference, which is only
  /// valid if the ElementType is ID_BASED.
  /// </para>
  /// <para>
  /// While an IElementSet can be used to query the geometric description of a
  /// model schematization, it does not necessarily provide all topological
  /// knowledge on inter-element connections.
  /// </para>
  /// <para>
  /// Although most models encapsulate static element sets, some advanced models
  /// might contain dynamic elements (e.g. waves). A version number has been
  /// introduced to enable tracking of element set changes over time. If the
  /// version changes, the element set might need to be queried again during the
  /// computation process.
  /// </para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// For ElementSets of type ElementType.IdBased the SpatialReferenceSystemWkt property an empty string.
  /// </para>
  /// </remarks>
  public interface IElementSet : ISpatialDefinition
  {

    /// <summary>
    /// <see cref="ElementType"/> of the elementset. All elements in the set are of his type.
    /// </summary>
    ElementType ElementType { get; }

    /// <summary>
    /// Index of element with id <paramref name="elementId"/> in the elementset. Indexes start from zero.
    /// There are not restrictions to how elements are ordered.
    /// </summary>
    /// <param name="elementId">
    /// Identification string for the element for which the element index is requested.
    /// </param>
    /// <returns>
    /// Index of the element with the specified id.
    /// If no element in the ElementSet has the specified elementId, -1 must be returned.
    /// </returns>
    int GetElementIndex(IIdentifiable elementId);

    /// <summary>
    /// Returns Id of the '<paramref name="index"/>-th' element in the ElementSet. Indexes start from zero.
    /// If the ElementType of the ElementSet is not IdBased, a null or an empty string may be returned.
    /// </summary>
    /// <param name="index">
    /// The element index for which the element Caption is requested. If the element index is outside
    /// the range [0, number of elements -1], an exception must be thrown.
    /// </param>
    /// <returns>
    /// The id of the element with the specified index.
    /// If the index is out of range, an exception must be thrown.
    /// </returns>
    IIdentifiable GetElementId(int index);

    /// <summary>
    /// <para>Number of vertices for the element specified by the elementIndex.</para>
    /// 
    /// <para>If the GetVertexCount()method is invoked for element sets of type <see cref="TimeSpace.ElementType.IdBased"/>, an exception
    /// must be thrown.</para>
    /// </summary>
    /// 
    /// <param name="elementIndex">
    /// <para>The element index for the element for which the number of vertices is requested.</para>
    /// 
    /// <para>If the element index is outside the range [0, number of elements -1], an exception
    /// must be thrown.</para>
    /// </param>
    /// <returns>Number of vertices in element defined by the elementIndex.</returns>
    int GetVertexCount(int elementIndex);

    /// <summary>
    /// Returns the number of faces in a 3D element. For 2D elements this returns 0.
    /// </summary>
    /// <param name="elementIndex">
    /// <para>Index for the element</para>
    /// 
    /// <para>If the element index is outside the range [0, number of elements -1], an exception
    /// must be thrown.</para>
    /// </param>
    /// <returns>Number of faces.</returns>
    int GetFaceCount(int elementIndex);

    /// <summary>
    /// Gives an array with the vertex indices for a face. 
    /// </summary>
    /// <param name="elementIndex">Element index.</param>
    /// <param name="faceIndex">Face index.</param>
    /// <returns>The vertex indices for this face.</returns>
    /// <remarks>
    /// The vertex indices for a face must be locally numbered for the element
    /// (containing numbers in the range [0;<see cref="GetVertexCount"/>(elementIndex)-1]).
    /// </remarks>
    int[] GetFaceVertexIndices(int elementIndex, int faceIndex);

    ///<summary>
    /// True if the element set supports Z co-ordinates.
    ///</summary>
    bool HasZ { get; }

    ///<summary>
    /// True if the element set supports M co-ordinates.
    ///</summary>
    bool HasM { get; }

    /// <summary>
    /// <para>X co-ordinate for the vertex with vertexIndex of the element with elementIndex.</para>
    /// </summary>
    /// <param name="elementIndex">Element index.</param>
    /// <param name="vertexIndex">Vertex index in the element with index elementIndex.</param>
    double GetVertexXCoordinate(int elementIndex, int vertexIndex);

    /// <summary>
    /// <para>Y co-ordinate for the vertex with vertexIndex of the element with elementIndex.</para>
    /// </summary>
    /// <param name="elementIndex">Element index.</param>
    /// <param name="vertexIndex">Vertex index in the element with index elementIndex.</param>
    double GetVertexYCoordinate(int elementIndex, int vertexIndex);

    /// <summary>
    /// <para>Z co-ordinate for the vertex with vertexIndex of the element with elementIndex.</para>
    /// </summary>
    /// <param name="elementIndex">Element index.</param>
    /// <param name="vertexIndex">Vertex index in the element with index elementIndex.</param>
    double GetVertexZCoordinate(int elementIndex, int vertexIndex);

    /// <summary>
    /// <para>M co-ordinate for the vertex with VertexIndex of the element with elementIndex.</para>
    /// </summary>
    /// <param name="elementIndex">Element index.</param>
    /// <param name="vertexIndex">Vertex index in the element with index elementIndex.</param>
    double GetVertexMCoordinate(int elementIndex, int vertexIndex);
  }
}