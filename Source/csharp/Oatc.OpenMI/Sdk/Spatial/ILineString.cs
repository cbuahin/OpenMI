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
  /// Interface representing a curve as a line string.
  /// <para>
  /// A Curve is a 1-dimensional geometric object, where the line string is a curve storing a sequence of coordinates.
  /// </para>
  /// </summary>
  /// <remarks>
  /// Interface is based on the <see cref="IMultiPoint"/> due to their similarities
  /// </remarks>
  public interface ILineString : IMultiPoint
  {
    /// <summary>
    /// Defines whether the data is based on the line string coordinates
    /// or is covering the reach between the grid points
    /// </summary>
    bool IsNodeBased { get; set; }

    // TODO: Move HasM and HasZ to ISpatialDefinition? The OGC Geometry has it there (is3D, isMeasured).
    bool HasM { get; set; }
    bool IsClosed { get; set; }
  }
}