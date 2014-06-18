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
﻿using System;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Class representing a line string
  /// </summary>
  /// <remarks>
  /// Class is based on the <see cref="MultiPoint"/> due to their similarities
  /// </remarks>
  public class LineString : MultiPoint, ILineString
  {
    public override int ElementCount { get { return ((IsNodeBased) ? Coordinates.Count : Coordinates.Count - 1); } }
    public bool IsNodeBased { get; set; }
    public bool IsClosed { get; set; }
  }

  /// <summary>
  /// Wrapping an <see cref="LineString"/> into an <see cref="IElementSet"/>
  /// </summary>
  /// <summary>
  /// Class wrapping an <see cref="ILineString"/> into an <see cref="IElementSet"/>
  /// </summary>
  public class LineStringWrapper : AbstractSpatialDefinitionWrapper, IElementSet
  {
    private readonly ILineString _lineString;

    public LineStringWrapper(ILineString lineString) : base("LineStringWrapper")
    {
      _lineString = lineString;
      Caption = "LineString wrapper";
      Description = "Wraps an ILineString into an IElementSet";
    }

    protected override ISpatialDefinition SpatialDefinition
    {
      get { return (_lineString); }
    }

    public virtual ElementType ElementType
    {
      get { return ((_lineString.IsNodeBased) ? ElementType.PolyLine : ElementType.PolyLine); }
    }

    public int GetElementIndex(IIdentifiable elementId)
    {
      throw new NotImplementedException();
    }

    public IIdentifiable GetElementId(int index)
    {
      throw new NotImplementedException();
    }

    public int GetVertexCount(int elementIndex)
    {
      return ((_lineString.IsNodeBased) ? 1 : 2);
    }

    public int GetFaceCount(int elementIndex)
    {
      return (0);
    }

    public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
    {
      throw new InvalidOperationException();
    }

    public bool HasZ
    {
      get { return (_lineString.HasZ); }
    }

    public bool HasM
    {
      get { return (_lineString.HasM); }
    }

    public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
    {
      return (_lineString.Coordinates[elementIndex+vertexIndex].X);
    }

    public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
    {
      return (_lineString.Coordinates[elementIndex + vertexIndex].Y);
    }

    public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
    {
      return (_lineString.Coordinates[elementIndex + vertexIndex].Z);
    }

    public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
    {
      return (_lineString.Coordinates[elementIndex + vertexIndex].M);
    }
  }
}
