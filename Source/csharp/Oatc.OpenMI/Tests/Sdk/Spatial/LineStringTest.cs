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
﻿using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
  [TestFixture]
  public class LineStringTest
  {
    [Test]
    public void LineStringNodeBasedWrapperTest()
    {
      ICoordinate[] coordinates = new ICoordinate[4];
      coordinates[0] = new CoordinateM(1, 2, 0, 0);
      coordinates[1] = new CoordinateM(3, 4, 0, 1);
      coordinates[2] = new CoordinateM(5, 6, -5, 2);
      coordinates[3] = new CoordinateM(7, 8, 0, 3.5);

      LineString lineString = new LineString()
                                {
                                  Coordinates = coordinates,
                                  HasZ = false,
                                  IsNodeBased = true,
                                };

      LineStringWrapper wrapper = new LineStringWrapper(lineString);

      Assert.AreEqual(ElementType.PolyLine, wrapper.ElementType);
      Assert.AreEqual(4, wrapper.ElementCount);

      Assert.AreEqual(1, wrapper.GetVertexCount(0));
      Assert.AreEqual(1, wrapper.GetVertexCount(1));
      Assert.AreEqual(1, wrapper.GetVertexCount(2));
      Assert.AreEqual(1, wrapper.GetVertexCount(3));

      Assert.AreEqual(1, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(2, wrapper.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(0, wrapper.GetVertexMCoordinate(0, 0));

      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(1, 0));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(1, 0));
      Assert.AreEqual(1, wrapper.GetVertexMCoordinate(1, 0));

      Assert.AreEqual(5, wrapper.GetVertexXCoordinate(2, 0));
      Assert.AreEqual(6, wrapper.GetVertexYCoordinate(2, 0));
      Assert.AreEqual(2, wrapper.GetVertexMCoordinate(2, 0));

      Assert.AreEqual(7, wrapper.GetVertexXCoordinate(3, 0));
      Assert.AreEqual(8, wrapper.GetVertexYCoordinate(3, 0));
      Assert.AreEqual(3.5, wrapper.GetVertexMCoordinate(3, 0));

    }

    [Test]
    public void LineStringElmtBasedWrapperTest()
    {
      ICoordinate[] coordinates = new ICoordinate[4];
      coordinates[0] = new CoordinateM(1, 2, 0, 0);
      coordinates[1] = new CoordinateM(3, 4, 0, 1);
      coordinates[2] = new CoordinateM(5, 6, -5, 2);
      coordinates[3] = new CoordinateM(7, 8, 0, 3.5);

      LineString lineString = new LineString()
      {
        Coordinates = coordinates,
        HasZ = false,
        IsNodeBased = false,
      };

      LineStringWrapper wrapper = new LineStringWrapper(lineString);

      Assert.AreEqual(ElementType.PolyLine, wrapper.ElementType);
      Assert.AreEqual(3, wrapper.ElementCount);

      Assert.AreEqual(2, wrapper.GetVertexCount(0));
      Assert.AreEqual(2, wrapper.GetVertexCount(1));
      Assert.AreEqual(2, wrapper.GetVertexCount(2));

      Assert.AreEqual(1, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(2, wrapper.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(0, wrapper.GetVertexMCoordinate(0, 0));
      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(0, 1));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(0, 1));
      Assert.AreEqual(1, wrapper.GetVertexMCoordinate(0, 1));

      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(1, 0));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(1, 0));
      Assert.AreEqual(1, wrapper.GetVertexMCoordinate(1, 0));
      Assert.AreEqual(5, wrapper.GetVertexXCoordinate(1, 1));
      Assert.AreEqual(6, wrapper.GetVertexYCoordinate(1, 1));
      Assert.AreEqual(2, wrapper.GetVertexMCoordinate(1, 1));

      Assert.AreEqual(5, wrapper.GetVertexXCoordinate(2, 0));
      Assert.AreEqual(6, wrapper.GetVertexYCoordinate(2, 0));
      Assert.AreEqual(2, wrapper.GetVertexMCoordinate(2, 0));
      Assert.AreEqual(7, wrapper.GetVertexXCoordinate(2, 1));
      Assert.AreEqual(8, wrapper.GetVertexYCoordinate(2, 1));
      Assert.AreEqual(3.5, wrapper.GetVertexMCoordinate(2, 1));

    }
  }
}
