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
using System.Linq;
using System.Text;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
  [TestFixture]
  public class Spatial2DCurveLinearGridTest
  {
    [Test]
    public void NodeGridWrapperTest()
    {

      ICoordinate[,] coordinates = new ICoordinate[4,3];
      coordinates[0,0] = new Coordinate(0,0);
      coordinates[1,0] = new Coordinate(1,0);
      coordinates[2,0] = new Coordinate(2,0);
      coordinates[3,0] = new Coordinate(3,0);

      coordinates[0,1] = new Coordinate(0,1);
      coordinates[1,1] = new Coordinate(1.2,1.1);
      coordinates[2,1] = new Coordinate(2.4,1.2);
      coordinates[3,1] = new Coordinate(3.8,1.3);
      
      coordinates[0,2] = new Coordinate(0,2);
      coordinates[1,2] = new Coordinate(1.3,2.1);
      coordinates[2,2] = new Coordinate(2.9,2.2);
      coordinates[3,2] = new Coordinate(4.5,2.3);

      Spatial2DCurveLinearGrid grid = new Spatial2DCurveLinearGrid()
                                        {
                                          Coordinates = coordinates,
                                          IsNodeBased = true,
                                        };

      Spatial2DGridWrapper wrapper = new Spatial2DGridWrapper(grid);

      Assert.AreEqual(ElementType.Point, wrapper.ElementType);
      Assert.AreEqual(12, wrapper.ElementCount);

      Assert.AreEqual(1, wrapper.GetVertexCount(0));
      Assert.AreEqual(1, wrapper.GetVertexCount(3));
      Assert.AreEqual(1, wrapper.GetVertexCount(8));
      Assert.AreEqual(1, wrapper.GetVertexCount(11));

      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(0, wrapper.GetVertexYCoordinate(0, 0));

      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(3, 0));
      Assert.AreEqual(0, wrapper.GetVertexYCoordinate(3, 0));

      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(8, 0));
      Assert.AreEqual(2, wrapper.GetVertexYCoordinate(8, 0));

      Assert.AreEqual(2.9, wrapper.GetVertexXCoordinate(10, 0));
      Assert.AreEqual(2.2, wrapper.GetVertexYCoordinate(10, 0));
    }

    [Test]
    public void ElmtGridWrapperTest()
    {

      ICoordinate[,] coordinates = new ICoordinate[4, 3];
      coordinates[0, 0] = new Coordinate(0, 0);
      coordinates[1, 0] = new Coordinate(1, 0);
      coordinates[2, 0] = new Coordinate(2, 0);
      coordinates[3, 0] = new Coordinate(3, 0);

      coordinates[0, 1] = new Coordinate(0, 1);
      coordinates[1, 1] = new Coordinate(1.2, 1.1);
      coordinates[2, 1] = new Coordinate(2.4, 1.2);
      coordinates[3, 1] = new Coordinate(3.8, 1.3);

      coordinates[0, 2] = new Coordinate(0, 2);
      coordinates[1, 2] = new Coordinate(1.3, 2.1);
      coordinates[2, 2] = new Coordinate(2.9, 2.2);
      coordinates[3, 2] = new Coordinate(4.5, 2.3);

      Spatial2DCurveLinearGrid grid = new Spatial2DCurveLinearGrid()
      {
        Coordinates = coordinates,
        IsNodeBased = false,
      };

      Spatial2DGridWrapper wrapper = new Spatial2DGridWrapper(grid);

      Assert.AreEqual(ElementType.Polygon, wrapper.ElementType);
      Assert.AreEqual(6, wrapper.ElementCount);

      Assert.AreEqual(4, wrapper.GetVertexCount(0));
      Assert.AreEqual(4, wrapper.GetVertexCount(3));
      Assert.AreEqual(4, wrapper.GetVertexCount(8));
      Assert.AreEqual(4, wrapper.GetVertexCount(11));

      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(0, wrapper.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(1, wrapper.GetVertexXCoordinate(0, 1));
      Assert.AreEqual(0, wrapper.GetVertexYCoordinate(0, 1));
      Assert.AreEqual(1.2, wrapper.GetVertexXCoordinate(0, 2));
      Assert.AreEqual(1.1, wrapper.GetVertexYCoordinate(0, 2));
      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(0, 3));
      Assert.AreEqual(1, wrapper.GetVertexYCoordinate(0, 3));

      Assert.AreEqual(2.4, wrapper.GetVertexXCoordinate(5, 0));
      Assert.AreEqual(1.2, wrapper.GetVertexYCoordinate(5, 0));
      Assert.AreEqual(3.8, wrapper.GetVertexXCoordinate(5, 1));
      Assert.AreEqual(1.3, wrapper.GetVertexYCoordinate(5, 1));
      Assert.AreEqual(4.5, wrapper.GetVertexXCoordinate(5, 2));
      Assert.AreEqual(2.3, wrapper.GetVertexYCoordinate(5, 2));
      Assert.AreEqual(2.9, wrapper.GetVertexXCoordinate(5, 3));
      Assert.AreEqual(2.2, wrapper.GetVertexYCoordinate(5, 3));

    }
  
  
  }
}
