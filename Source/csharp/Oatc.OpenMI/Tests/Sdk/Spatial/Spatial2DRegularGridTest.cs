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
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
  [TestFixture]
  public class Spatial2DRegularGridTest
  {
    /// <summary>
    /// Create a node-based grid with origin at (10,20), rotated 30 degrees
    /// clock-wise from true north
    /// </summary>
    [Test]
    public void NodeGridWrapperTest()
    {
      Spatial2DRegularGrid grid = new Spatial2DRegularGrid
                             {
                               X0 = 10,
                               Y0 = 20,
                               Dx = 2,
                               Dy = 4,
                               XCount = 4,
                               YCount = 3,
                               Orientation = 30,
                               IsNodeBased = true,
                             };

      Spatial2DGridWrapper gridWrapper = new Spatial2DGridWrapper(grid);

      double rotxx = Math.Cos(30.0/180.0*Math.PI);
      double rotxy = Math.Sin(30.0/180.0*Math.PI);
      double rotyx = -Math.Sin(30.0/180.0*Math.PI);
      double rotyy = Math.Cos(30.0/180.0*Math.PI);

      Assert.AreEqual(ElementType.Point, gridWrapper.ElementType);
      Assert.AreEqual(12, gridWrapper.ElementCount);
      
      Assert.AreEqual(1, gridWrapper.GetVertexCount(0));
      Assert.AreEqual(1, gridWrapper.GetVertexCount(3));
      Assert.AreEqual(1, gridWrapper.GetVertexCount(8));
      Assert.AreEqual(1, gridWrapper.GetVertexCount(11));

      Assert.AreEqual(10, gridWrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(20, gridWrapper.GetVertexYCoordinate(0, 0));

      Assert.AreEqual(10 + rotxx*6, gridWrapper.GetVertexXCoordinate(3, 0)); // 15.19
      Assert.AreEqual(20 + rotyx*6, gridWrapper.GetVertexYCoordinate(3, 0)); // 17

      Assert.AreEqual(10 + rotxy*8, gridWrapper.GetVertexXCoordinate(8, 0)); // 14
      Assert.AreEqual(20 + rotyy*8, gridWrapper.GetVertexYCoordinate(8, 0)); // 26.92

      Assert.AreEqual(10 + rotxx*6 + rotxy*8, gridWrapper.GetVertexXCoordinate(11, 0)); // 19,19
      Assert.AreEqual(20 + rotyx*6 + rotyy*8, gridWrapper.GetVertexYCoordinate(11, 0)); // 23.92
    }

    /// <summary>
    /// Create an element-based grid with origin at (10,20), rotated 30 degrees
    /// clock-wise from true north
    /// </summary>
    [Test]
    public void ElmtGridWrapperTest()
    {
      Spatial2DRegularGrid grid = new Spatial2DRegularGrid
      {
        X0 = 10,
        Y0 = 20,
        Dx = 2,
        Dy = 4,
        XCount = 4,
        YCount = 3,
        Orientation = 30,
        IsNodeBased = false,
      };

      Spatial2DGridWrapper gridWrapper = new Spatial2DGridWrapper(grid);

      double rotxx = Math.Cos(30.0 / 180.0 * Math.PI);
      double rotxy = Math.Sin(30.0 / 180.0 * Math.PI);
      double rotyx = -Math.Sin(30.0 / 180.0 * Math.PI);
      double rotyy = Math.Cos(30.0 / 180.0 * Math.PI);

      Assert.AreEqual(ElementType.Polygon, gridWrapper.ElementType);
      Assert.AreEqual(12, gridWrapper.ElementCount);

      Assert.AreEqual(4, gridWrapper.GetVertexCount(0));
      Assert.AreEqual(4, gridWrapper.GetVertexCount(3));
      Assert.AreEqual(4, gridWrapper.GetVertexCount(8));
      Assert.AreEqual(4, gridWrapper.GetVertexCount(11));

      // Check first element
      Assert.AreEqual(10, gridWrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(20, gridWrapper.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(10 + rotxx*2, gridWrapper.GetVertexXCoordinate(0, 1));
      Assert.AreEqual(20 + rotyx*2, gridWrapper.GetVertexYCoordinate(0, 1));
      Assert.AreEqual(10 + rotxx*2 + rotxy*4, gridWrapper.GetVertexXCoordinate(0, 2));
      Assert.AreEqual(20 + rotyx*2 + rotyy*4, gridWrapper.GetVertexYCoordinate(0, 2));
      Assert.AreEqual(10 + rotxy*4, gridWrapper.GetVertexXCoordinate(0, 3));
      Assert.AreEqual(20 + rotyy*4, gridWrapper.GetVertexYCoordinate(0, 3));

      // Check last element
      Assert.AreEqual(10 + rotxx*6 + rotxy*8, gridWrapper.GetVertexXCoordinate(11, 0));  // 19,19
      Assert.AreEqual(20 + rotyx*6 + rotyy*8, gridWrapper.GetVertexYCoordinate(11, 0));  // 23.92
      Assert.AreEqual(10 + rotxx*8 + rotxy*8, gridWrapper.GetVertexXCoordinate(11, 1));  // 20.92
      Assert.AreEqual(20 + rotyx*8 + rotyy*8, gridWrapper.GetVertexYCoordinate(11, 1));  // 22.92
      Assert.AreEqual(10 + rotxx*8 + rotxy*12, gridWrapper.GetVertexXCoordinate(11, 2)); // 22.92
      Assert.AreEqual(20 + rotyx*8 + rotyy*12, gridWrapper.GetVertexYCoordinate(11, 2)); // 26.39
      Assert.AreEqual(10 + rotxx*6 + rotxy*12, gridWrapper.GetVertexXCoordinate(11, 3)); // 21.19
      Assert.AreEqual(20 + rotyx*6 + rotyy*12, gridWrapper.GetVertexYCoordinate(11, 3)); // 27.39
    }
  }
}
