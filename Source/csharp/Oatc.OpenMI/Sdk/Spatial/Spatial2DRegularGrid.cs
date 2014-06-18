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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Simple implementation of a <see cref="ISpatial2DRegularGrid"/>
  /// </summary>
  public class Spatial2DRegularGrid : AbstractSpatialDefinition, ISpatial2DRegularGrid
  {
    private readonly double[,] _rotationMatrix = new double[2, 2];

    public Spatial2DRegularGrid()
    {
      IsNodeBased = true;
    }

    public bool IsNodeBased { get; set; }

    public double X0 { get; set; }
    public double Y0 { get; set; }
    public double Dx { get; set; }
    public double Dy { get; set; }
    public int XCount { get; set; }
    public int YCount { get; set; }

    private double _orientation;

    /// <summary>
    /// Orientation is the clock-wise rotation around (<see cref="X0"/>,<see cref="Y0"/>) of the 
    /// grid from true north, i.e. the compass heading of the y-axis. The value is in degrees
    /// <para>
    /// True north differs from projection north by the convergence angle at the point 
    /// (<see cref="X0"/>,<see cref="Y0"/>)
    /// </para>
    /// </summary>
    public double Orientation
    {
      get { return _orientation; }
      set
      {
        _orientation = value;
        double cosOri = Math.Cos(_orientation * Math.PI / 180.0);
        double sinOri = Math.Sin(_orientation * Math.PI / 180.0);
        _rotationMatrix[0, 0] = cosOri;
        _rotationMatrix[1, 1] = cosOri;
        _rotationMatrix[0, 1] = sinOri;
        _rotationMatrix[1, 0] = -sinOri;
      }
    }

    public int ElementCount
    {
      get { return (XCount * YCount); }
    }

    public ICoordinate GetCoordinate(int j, int k)
    {
      double gridX = j * Dx;
      double gridY = k * Dy;
      double x = X0 + _rotationMatrix[0, 0] * gridX + _rotationMatrix[0, 1] * gridY;
      double y = Y0 + _rotationMatrix[1, 0] * gridX + _rotationMatrix[1, 1] * gridY;
      return (new Coordinate(x, y));
    }

    /// <summary>
    /// Return the x-coordinate for the (j,k)'th node (grid line intersection)
    /// </summary>
    public double GetX(int j, int k)
    {
      double gridX = j * Dx;
      double gridY = k * Dy;
      return (X0 + _rotationMatrix[0, 0] * gridX + _rotationMatrix[0, 1] * gridY);
    }

    /// <summary>
    /// Return the y-coordinate for the (j,k)'th node (grid line intersection)
    /// </summary>
    public double GetY(int j, int k)
    {
      double gridX = j * Dx;
      double gridY = k * Dy;
      return (Y0 + _rotationMatrix[1, 0] * gridX + _rotationMatrix[1, 1] * gridY);
    }
  }




  ///// <summary>
  ///// Wraps a <see cref="Spatial2DRegularGrid"/> into an <see cref="IElementSet"/>
  ///// </summary>
  //public class Spatial2DStructuredGridWrapper : AbstractSpatialDefinitionWrapper, IElementSet
  //{
  //  private readonly ISpatial2DRegularGrid _grid;

  //  public Spatial2DStructuredGridWrapper(Spatial2DRegularGrid grid)
  //  {
  //    _grid = grid;
  //  }

  //  protected override ISpatialDefinition SpatialDefinition
  //  {
  //    get { return (_grid); }
  //  }

  //  public ElementType ElementType
  //  {
  //    get { return (_grid.IsNodeBased ? ElementType.Point : ElementType.Polygon); }
  //  }

  //  public int GetElementIndex(IIdentifiable elementId)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public IIdentifiable GetElementId(int index)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public int GetVertexCount(int elementIndex)
  //  {
  //    if (_grid.IsNodeBased)
  //    {
  //      return (1);
  //    }
  //    return (4);
  //  }

  //  public int GetFaceCount(int elementIndex)
  //  {
  //    return (0);
  //  }

  //  public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
  //  {
  //    throw new InvalidOperationException();
  //  }

  //  public bool HasZ
  //  {
  //    get { return (false); }
  //  }

  //  public bool HasM
  //  {
  //    get { return (false); }
  //  }

  //  public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
  //  {
  //    int jv;
  //    int kv;
  //    GetGridIndex(elementIndex, vertexIndex, out jv, out kv);
  //    return _grid.GetCoordinate(jv, kv).X;
  //  }

  //  public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
  //  {
  //    int jv;
  //    int kv;
  //    GetGridIndex(elementIndex, vertexIndex, out jv, out kv);
  //    return _grid.GetCoordinate(jv, kv).Y;
  //  }

  //  private void GetGridIndex(int elementIndex, int vertexIndex, out int jv, out int kv)
  //  {
  //    int j;
  //    int k;
  //    LinearToGridIndex(elementIndex, out j, out k);
  //    if (_grid.IsNodeBased)
  //    {
  //      jv = j;
  //      kv = k;
  //    }
  //    else
  //    {
  //      switch (vertexIndex)
  //      {
  //        case 0: // lower left
  //          jv = j; kv = k; break;
  //        case 1: // lower right
  //          jv = j + 1; kv = k; break;
  //        case 2: // upper left
  //          jv = j + 1; kv = k + 1; break;
  //        case 3: // upper right
  //          jv = j; kv = k + 1; break;
  //        default:
  //          throw new ArgumentOutOfRangeException("vertexIndex");
  //      }
  //    }
  //  }

  //  public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
  //  {
  //    int jv;
  //    int kv;
  //    GetGridIndex(elementIndex, vertexIndex, out jv, out kv);
  //    return _grid.GetCoordinate(jv, kv).Z;
  //  }

  //  public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
  //  {
  //    int jv;
  //    int kv;
  //    GetGridIndex(elementIndex, vertexIndex, out jv, out kv);
  //    return _grid.GetCoordinate(jv, kv).M;
  //  }

  //  private void LinearToGridIndex(int linearIndex, out int j, out int k)
  //  {
  //    j = linearIndex % _grid.XCount;
  //    k = linearIndex / _grid.XCount;
  //  }

  //  private int GridToLinearIndex(int j, int k)
  //  {
  //    return (k * _grid.XCount + j);
  //  }

  //}

}
