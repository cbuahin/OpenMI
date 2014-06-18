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
  /// Data in components in OpenMI is often related to spatial coordinates, 
  /// either geo-referenced or not. The <see cref="ISpatialDefinition"/> 
  /// is the general spatial construct that all other spatial constructions
  /// extend from.
  /// <para>
  /// The currently most noticable extending interfaces is the <see cref="IElementSet"/>, 
  /// which in previous versions of the standard was the only spatial construction, and
  /// which all other spatial constructions had to be wrapped into, whereas in the current
  /// version the <see cref="IElementSet"/> is an extension of the <see cref="ISpatialDefinition"/>.
  /// </para>
  /// <para>
  /// Although most models encapsulate data with a static spatial definition, some advanced 
  /// models might contain dynamic spatial definitions (e.g. waves, moving grids). The 
  /// <see cref="Version"/> number has been introduced to enable tracking of spatial changes 
  /// over time. If the version changes, the spatial definition might need to be queried again 
  /// during the computation process.
  /// </para>
  /// </summary>
  public interface ISpatialDefinition : IDescribable
  {
    /// <summary>
    /// <para>
    /// The SpatialReferenceSystemWkt speficies the OGC Well-Known Text representation of the spatial reference 
    /// system to be used in association with the coordinates in the <see cref="ISpatialDefinition"/>. 
    /// </para>
    /// <para>
    /// For the list of WKT strings see <see cref="http://spatialreference.org/"/>.
    /// </para>
    /// </summary>
    /// <example>
    /// For all spatial axis a spatial reference can be defined in a form: 
    /// <code>
    /// PROJCS["Mercator Spheric", GEOGCS["WGS84based_GCS", DATUM["WGS84based_Datum", SPHEROID["WGS84based_Sphere", 6378137, 0], TOWGS84[0, 0, 0, 0, 0, 0, 0]], PRIMEM["Greenwich", 0, AUTHORITY["EPSG", "8901"]], UNIT["degree", 0.0174532925199433, AUTHORITY["EPSG", "9102"]], AXIS["E", EAST], AXIS["N", NORTH]], PROJECTION["Mercator"], PARAMETER["False_Easting", 0], PARAMETER["False_Northing", 0], PARAMETER["Central_Meridian", 0], PARAMETER["Latitude_of_origin", 0], UNIT["metre", 1, AUTHORITY["EPSG", "9001"]], AXIS["East", EAST], AXIS["North", NORTH]]
    /// </code>
    /// </example>
    string SpatialReferenceSystemWkt { get; }

    /// <summary>
    /// Number of data elements in the spatial axis.
    /// </summary>
    int ElementCount { get; }

    /// <summary>
    /// The current version number for the spatial axis.
    /// <para>
    /// The version must be incremented if anything inside the spatial axis 
    /// is changed, or if an entirely new spatial axis is provided.
    /// </para>
    /// </summary>
    int Version { get; }
  }
}