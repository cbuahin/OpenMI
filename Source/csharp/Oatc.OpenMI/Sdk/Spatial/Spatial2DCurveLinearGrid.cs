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

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Class specifying a 2D curve linear grid, see <see cref="ISpatial2DGrid"/>
  /// for details.
  /// </summary>
  public class Spatial2DCurveLinearGrid : AbstractSpatialDefinition, ISpatial2DGrid
  {
    public ICoordinate[,] Coordinates { get; set; }

    public bool IsNodeBased { get; set; }

    public int ElementCount
    {
      get { return (XCount*YCount); }
    }

    public int XCount { get { return ((IsNodeBased) ? Coordinates.GetLength(0) : Coordinates.GetLength(0) - 1); } }
    public int YCount { get { return ((IsNodeBased) ? Coordinates.GetLength(1) : Coordinates.GetLength(1) - 1); } }

    public ICoordinate GetCoordinate(int j, int k)
    {
      if (j < 0 || j >= Coordinates.GetLength(0))
        throw new ArgumentOutOfRangeException("j");
      if (k < 0 || k >= Coordinates.GetLength(1))
        throw new ArgumentOutOfRangeException("k");
      return (Coordinates[j, k]);
    }

  }






}
