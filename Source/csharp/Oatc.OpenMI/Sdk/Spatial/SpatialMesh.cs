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
  public class SpatialMesh : AbstractSpatialDefinition, ISpatialMesh
  {
    public bool IsNodeBased { get; set; }
    public bool HasZ { get; set; }

    public int NumberOfNodes
    {
      get { return (Nodes.Count); }
    }

    public int NumberOfElements
    {
      get { return (Connectivity.Count); }
    }

    public IList<ICoordinate> Nodes { get; set; }

    public IList<int[]> Connectivity { get; set; }

    public int ElementCount
    {
      get { return (IsNodeBased ? NumberOfNodes : NumberOfElements); }
    }
  }

  public class SpatialMeshWrapper : AbstractSpatialDefinitionWrapper, IElementSet
  {
    private ISpatialMesh _mesh;

    public SpatialMeshWrapper(ISpatialMesh mesh) : base("SpatialMeshWrapper")
    {
      _mesh = mesh;
      Caption = "SpatialMesh wrapper";
      Description = "Wraps an ISpatialMesh into an IElementSet";
    }


    protected override ISpatialDefinition SpatialDefinition
    {
      get { return (_mesh); }
    }

    public ElementType ElementType
    {
      get { return (_mesh.IsNodeBased ? ElementType.Point : ElementType.Polygon); }
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
      if (_mesh.IsNodeBased)
        return (1);
      return (_mesh.Connectivity[elementIndex].Length);
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
      get { return (_mesh.HasZ); }
    }

    public bool HasM
    {
      get { return (false); }
    }

    public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
    {
      if (_mesh.IsNodeBased)
        return (_mesh.Nodes[elementIndex].X);
      return (_mesh.Nodes[_mesh.Connectivity[elementIndex][vertexIndex]].X);
    }

    public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
    {
      if (_mesh.IsNodeBased)
        return (_mesh.Nodes[elementIndex].Y);
      return (_mesh.Nodes[_mesh.Connectivity[elementIndex][vertexIndex]].Y);
    }

    public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
    {
      if (_mesh.IsNodeBased)
        return (_mesh.Nodes[elementIndex].Z);
      return (_mesh.Nodes[_mesh.Connectivity[elementIndex][vertexIndex]].Z);
    }

    public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
    {
      if (_mesh.IsNodeBased)
        return (_mesh.Nodes[elementIndex].M);
      return (_mesh.Nodes[_mesh.Connectivity[elementIndex][vertexIndex]].M);
    }
  }


}
