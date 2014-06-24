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
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Class for a set of points, <see cref="IMultiPoint"/>
  /// </summary>
  public class MultiPoint : AbstractSpatialDefinition, IMultiPoint
  {
    public virtual int ElementCount 
    { 
        get 
        { 
            return (Coordinates.Count);
        }
    }

    public bool HasZ { get; set; }
    public bool HasM { get; set; }

    public IList<ICoordinate> Coordinates { get; set; }
  }

  /// <summary>
  /// Class wrapping an <see cref="IMultiPoint"/> into an <see cref="IElementSet"/>
  /// </summary>
  public class MultiPointWrapper : AbstractSpatialDefinitionWrapper, IElementSet
  {
    private readonly IMultiPoint _multiPoint;

    public MultiPointWrapper(IMultiPoint multiPoint) : base("MultiPointWrapper")
    {
      _multiPoint = multiPoint;
      Caption = "MultiPoint wrapper";
      Description = "Wraps an IMultiPoint into an IElementSet";
    }

    protected override ISpatialDefinition SpatialDefinition
    {
      get { return (_multiPoint); }
    }
    
    public virtual ElementType ElementType
    {
      get { return(ElementType.Point); }
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
      return (1);
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
      get { return (_multiPoint.HasZ); }
    }

    public bool HasM
    {
      get { return (false); }
    }

    public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
    {
      if (vertexIndex != 0) 
        throw new ArgumentOutOfRangeException("vertexIndex");
      if (elementIndex < 0 || elementIndex >= _multiPoint.Coordinates.Count)
        throw new ArgumentOutOfRangeException("elementIndex");
      return (_multiPoint.Coordinates[elementIndex].X);
    }

    public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
    {
      if (vertexIndex != 0)
        throw new ArgumentOutOfRangeException("vertexIndex");
      if (elementIndex < 0 || elementIndex >= _multiPoint.Coordinates.Count)
        throw new ArgumentOutOfRangeException("elementIndex");
      return (_multiPoint.Coordinates[elementIndex].Y);
    }

    public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
    {
      if (vertexIndex != 0)
        throw new ArgumentOutOfRangeException("vertexIndex");
      if (elementIndex < 0 || elementIndex >= _multiPoint.Coordinates.Count)
        throw new ArgumentOutOfRangeException("elementIndex");
      return (_multiPoint.Coordinates[elementIndex].Z);
    }

    public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
    {
      if (vertexIndex != 0)
        throw new ArgumentOutOfRangeException("vertexIndex");
      if (elementIndex < 0 || elementIndex >= _multiPoint.Coordinates.Count)
        throw new ArgumentOutOfRangeException("elementIndex");
      return (_multiPoint.Coordinates[elementIndex].M);
    }
  }

}
