#region Copyright
/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Interface for a 2D grid. 
  /// <para>
  /// Coordinates of each grid line intersection can be requested by the 
  /// <see cref="GetCoordinate"/>
  /// </para>
  /// <para>
  /// The grid data can either be node based, meaning that data belongs to grid nodes
  /// (where the grid lines intersect), or it can be element based, meaning that
  /// it covers the entire quadrilateral within the grid lines. For node-based
  /// data it is possible to perform bilinear interpolation of the data, while 
  /// element-based data has to search for the enclosing quadrilateral and return its value.
  /// </para>
  /// <para>
  /// For node base values there are as many coordinates as there are data values.
  /// </para>
  /// <para>
  /// For element base values there are 1 more coordinate in each direction than 
  /// there are data values. Example, if there are 3 x 2 data values, there 
  /// will be 4 x 3 coordinates.
  /// </para>
  /// </summary>
  public interface ISpatial2DGrid : ISpatialDefinition
  {
    /// <summary>
    /// Defines whether the data is based on the grid nodes (intersection of grid lines)
    /// or is covering an entire grid element (quadrilateral)
    /// </summary>
    bool IsNodeBased { get; set; }

    /// <summary>
    /// Number of data in the x-grid direction
    /// </summary>
    int XCount { get; }

    /// <summary>
    /// Number of data in the y-grid direction
    /// </summary>
    int YCount { get; }

    /// <summary>
    /// Return the node coordinate of the j'th (x grid-direction) and
    /// k'th (y grid-direction) grid line intersection.
    /// </summary>
    /// <param name="j">Zero based index in x grid-direction</param>
    /// <param name="k">Zero based index in y grid-direction</param>
    /// <returns></returns>
    ICoordinate GetCoordinate(int j, int k);
  }
}