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
namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Interface for a 2D regular structured grid
  /// <para>
  /// The grid has the origin in (<see cref="X0"/>,<see cref="Y0"/>).
  /// The origin is the lower left corner of the grid.
  /// </para>
  /// <para>
  /// The grid can be rotated around the origin, see <see cref="Orientation"/>
  /// for details.
  /// </para>
  /// <para>
  /// The grid data can either be node based, meaning that data belongs to grid nodes
  /// (where the grid lines intersect), or it can be element based, meaning that
  /// it covers the entire quadrilateral within the grid lines. For node-based
  /// data it is possible to perform bilinear interpolation of the data, while 
  /// element-based data will search for the enclosing quadrilateral and return its value.
  /// </para>
  /// </summary>
  public interface ISpatial2DRegularGrid : ISpatial2DGrid
  {
    /// <summary>
    /// X coordinate of origin of grid, lower left corner in grid
    /// </summary>
    double X0 { get; }

    /// <summary>
    /// Y coordinate of origin of grid, lower left corner in grid
    /// </summary>
    double Y0 { get; }

    /// <summary>
    /// Grid-spacing in the x-grid direction
    /// </summary>
    double Dx { get; }

    /// <summary>
    /// Grid-spacing in the y-grid direction
    /// </summary>
    double Dy { get; }

    /// <summary>
    /// Orientation is the clock-wise rotation around (<see cref="X0"/>,<see cref="Y0"/>) of the 
    /// grid. It can be thought of as the compass heading of the y-axis.
    /// <para>
    /// Regarding the compass heading: true north differs from projection north by the 
    /// convergence angle at the point (<see cref="X0"/>,<see cref="Y0"/>), i.e, it 
    /// will not necessarily be the true compass heading, but differ from the true 
    /// compass heading by thiw convergence angle.
    /// </para>
    /// </summary>
    double Orientation { get; }
  }
}