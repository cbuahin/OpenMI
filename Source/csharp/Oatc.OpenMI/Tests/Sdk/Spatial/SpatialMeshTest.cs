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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
  [TestFixture]
  public class SpatialMeshTest
  {
    /// <summary>
    /// Check mesh, having node based data
    /// </summary>
    [Test]
    public void NodeBasedMesh()
    {
      SpatialMesh mesh = CreateMesh(true);

      SpatialMeshWrapper wrapper = new SpatialMeshWrapper(mesh);

      Assert.AreEqual(ElementType.Point, wrapper.ElementType);
      Assert.AreEqual(13, wrapper.ElementCount);

      Assert.AreEqual(1, wrapper.GetVertexCount(0));
      Assert.AreEqual(1, wrapper.GetVertexCount(1));
      Assert.AreEqual(1, wrapper.GetVertexCount(12));

      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(5, wrapper.GetVertexYCoordinate(0, 0));

      Assert.AreEqual(7, wrapper.GetVertexXCoordinate(12, 0));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(12, 0));


    }

    /// <summary>
    /// Check mesh, having element based data
    /// </summary>
    [Test]
    public void ElementBasedMesh()
    {
      SpatialMesh mesh = CreateMesh(false);

      SpatialMeshWrapper wrapper = new SpatialMeshWrapper(mesh);

      Assert.AreEqual(ElementType.Polygon, wrapper.ElementType);
      Assert.AreEqual(14, wrapper.ElementCount);

      Assert.AreEqual(4, wrapper.GetVertexCount(0));
      Assert.AreEqual(3, wrapper.GetVertexCount(1));

      Assert.AreEqual(0, wrapper.GetVertexXCoordinate(0, 0));
      Assert.AreEqual(5, wrapper.GetVertexYCoordinate(0, 0));
      Assert.AreEqual(1, wrapper.GetVertexXCoordinate(0, 1));
      Assert.AreEqual(3, wrapper.GetVertexYCoordinate(0, 1));
      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(0, 2));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(0, 2));
      Assert.AreEqual(2, wrapper.GetVertexXCoordinate(0, 3));
      Assert.AreEqual(6, wrapper.GetVertexYCoordinate(0, 3));

      Assert.AreEqual(1, wrapper.GetVertexXCoordinate(13, 0));
      Assert.AreEqual(3, wrapper.GetVertexYCoordinate(13, 0));
      Assert.AreEqual(2, wrapper.GetVertexXCoordinate(13, 1));
      Assert.AreEqual(1, wrapper.GetVertexYCoordinate(13, 1));
      Assert.AreEqual(3, wrapper.GetVertexXCoordinate(13, 2));
      Assert.AreEqual(4, wrapper.GetVertexYCoordinate(13, 2));
    }

    /// <summary>
    /// Writes Matlab script code to std out, that can be loaded
    /// into Matlab to check whether the mesh is correctly build.
    /// </summary>
    [Test]
    [Ignore("Run manually when required")]
    public void ToMatlabScript()
    {
      SpatialMesh mesh = CreateMesh(true);
      Console.Out.WriteLine("Nodes = [");
      foreach (ICoordinate node in mesh.Nodes)
      {
        Console.Out.WriteLine("{0}, {1}, {2}", node.X, node.Y, node.Z);
      }
      Console.Out.WriteLine("];");

      Console.Out.WriteLine("Elmts = [");
      foreach (int[] elmtNodes in mesh.Connectivity)
      {
        int count = 0;
        foreach (int nodeIndex in elmtNodes)
        {
          if (count > 0)
          {
            Console.Out.Write(", ");
          }
          Console.Out.Write("{0}", nodeIndex + 1);
          count++;
        }
        while (count < 4)
        {
          Console.Out.Write(", 0");
          count++;
        }
        Console.Out.WriteLine("");
      }
      Console.Out.WriteLine("];");
      // The following prints out code that will plot the mesh
      // outline, including the quadrilateral.
      Console.Out.WriteLine("I4       = (Elmts(:,4) > 0);");
      Console.Out.WriteLine("NI       = Elmts(:,[1 2 3 1 1]);");
      Console.Out.WriteLine("NI(I4,:) = Elmts(I4,[1 2 3 4 1]);");
      Console.Out.WriteLine("NI       = NI';");
      Console.Out.WriteLine("X = Nodes(:,1);");
      Console.Out.WriteLine("Y = Nodes(:,2);");
      Console.Out.WriteLine("Z = Nodes(:,3);");
      Console.Out.WriteLine("patch(X(NI),Y(NI),Z(NI));");
    }

    /// <summary>
    /// Create a small mesh with one quadrilateral element and 13 triangular elements.
    /// There are 13 nodes in the mesh.
    /// </summary>
    private SpatialMesh CreateMesh(bool nodeBased)
    {
      ICoordinate[] nodes = new ICoordinate[13];
      nodes[0] = new Coordinate(0, 5, 0);
      nodes[1] = new Coordinate(1, 3, 0);
      nodes[2] = new Coordinate(2, 1, 0);
      nodes[3] = new Coordinate(2, 6, 0);
      nodes[4] = new Coordinate(3, 4, 0);
      nodes[5] = new Coordinate(4, 0, 0);
      nodes[6] = new Coordinate(4, 2, 0);
      nodes[7] = new Coordinate(5, 4, 0);
      nodes[8] = new Coordinate(5, 6, 0);
      nodes[9] = new Coordinate(6, 1, 0);
      nodes[10] = new Coordinate(6, 3, 0);
      nodes[11] = new Coordinate(7, 2, 0);
      nodes[12] = new Coordinate(7, 4, 0);

      int[][] elmts = new int[14][];
      elmts[0] = new int[] { 0, 1, 4, 3 };
      elmts[1] = new int[] { 3, 4, 8 };
      elmts[2] = new int[] { 4, 7, 8 };
      elmts[3] = new int[] { 7, 12, 8 };
      elmts[4] = new int[] { 7, 10, 12 };
      elmts[5] = new int[] { 10, 11, 12 };
      elmts[6] = new int[] { 9, 11, 10 };
      elmts[7] = new int[] { 6, 9, 10 };
      elmts[8] = new int[] { 6, 10, 7 };
      elmts[9] = new int[] { 5, 9, 6 };
      elmts[10] = new int[] { 4, 6, 7 };
      elmts[11] = new int[] { 2, 5, 6 };
      elmts[12] = new int[] { 2, 6, 4 };
      elmts[13] = new int[] { 1, 2, 4 };

      return new SpatialMesh()
               {
                 IsNodeBased = nodeBased,
                 HasZ = false,
                 Nodes = nodes,
                 Connectivity = elmts,
               };
    }
  }
}
