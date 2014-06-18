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
#region Copyright
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2005 HarmonIT Consortium
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      December 1st 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion

using System;
using OpenMI.Standard;
using Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel;
using NUnit.Framework;

namespace Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.UnitTest
{
	/// <summary>
	/// Summary description for BaseGridTest.
	/// </summary>
	[TestFixture]
	public class BaseGridTest
	{
		Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GridInfo _gridInfo;
		Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.BaseGrid _baseGrid;

		public BaseGridTest()
		{
			double[] CellCentersX     = {15,15,25,25,25,25,35,35,35,35,45,45};
			double[] CellCentersY     = {25,35,15,25,35,45,15,25,35,45,25,35};
			double   cellSize         = 10.0;
			int      numberOfCells    = 12;

			_gridInfo = new GridInfo(numberOfCells, CellCentersX, CellCentersY, cellSize);

			_baseGrid = new BaseGrid(_gridInfo);
		}

		[Test]
		public void GetXCoordinate()
		{
            WriteMethodNameToConsole("GetXCoordinate");
			Assert.AreEqual(10.0,_baseGrid.GetXCoordinate(0,0));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(0,1));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(0,2));
			Assert.AreEqual(10.0,_baseGrid.GetXCoordinate(0,3));

			Assert.AreEqual(10.0,_baseGrid.GetXCoordinate(1,0));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(1,1));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(1,2));
			Assert.AreEqual(10.0,_baseGrid.GetXCoordinate(1,3));

			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(2,0));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(2,1));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(2,2));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(2,3));

			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(3,0));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(3,1));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(3,2));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(3,3));

			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(4,0));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(4,1));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(4,2));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(4,3));

			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(5,0));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(5,1));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(5,2));
			Assert.AreEqual(20.0,_baseGrid.GetXCoordinate(5,3));

			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(6,0));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(6,1));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(6,2));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(6,3));

			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(7,0));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(7,1));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(7,2));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(7,3));

			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(8,0));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(8,1));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(8,2));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(8,3));

			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(9,0));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(9,1));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(9,2));
			Assert.AreEqual(30.0,_baseGrid.GetXCoordinate(9,3));

			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(10,0));
			Assert.AreEqual(50.0,_baseGrid.GetXCoordinate(10,1));
			Assert.AreEqual(50.0,_baseGrid.GetXCoordinate(10,2));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(10,3));

			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(11,0));
			Assert.AreEqual(50.0,_baseGrid.GetXCoordinate(11,1));
			Assert.AreEqual(50.0,_baseGrid.GetXCoordinate(11,2));
			Assert.AreEqual(40.0,_baseGrid.GetXCoordinate(11,3));

            // Now using a different contructor for GridInfo object

            double ox = 1000.0;
            double oy = 2000.0;
            double cellSize = 900.0;
            int    nx = 2;
            int    ny = 2;
            
            Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GridInfo gridInfo = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GridInfo(ox, oy, cellSize, nx, ny, 0);
            IElementSet elementSet = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.BaseGrid(gridInfo);

            Assert.AreEqual(ox, elementSet.GetXCoordinate(0,0));
            Assert.AreEqual(ox + cellSize , elementSet.GetXCoordinate(0,1));
            Assert.AreEqual(ox + cellSize, elementSet.GetXCoordinate(0,2));
            Assert.AreEqual(ox, elementSet.GetXCoordinate(0,3));

            Assert.AreEqual(ox + cellSize * (nx - 1), elementSet.GetXCoordinate(nx * ny - 1, 0));
            Assert.AreEqual(ox + cellSize * nx , elementSet.GetXCoordinate(nx*ny - 1, 1));
            Assert.AreEqual(ox + cellSize * nx, elementSet.GetXCoordinate(nx*ny - 1, 2));
            Assert.AreEqual(ox + cellSize * (nx - 1), elementSet.GetXCoordinate(nx * ny - 1, 3));

		
		}

		[Test]
		public void GetYCoordinate()
		{
            WriteMethodNameToConsole("GetYCoordinate");
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(0,0));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(0,1));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(0,2));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(0,3));

			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(1,0));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(1,1));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(1,2));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(1,3));

			Assert.AreEqual(10.0,_baseGrid.GetYCoordinate(2,0));
			Assert.AreEqual(10.0,_baseGrid.GetYCoordinate(2,1));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(2,2));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(2,3));
			
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(3,0));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(3,1));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(3,2));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(3,3));
			
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(4,0));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(4,1));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(4,2));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(4,3));
			
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(5,0));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(5,1));
			Assert.AreEqual(50.0,_baseGrid.GetYCoordinate(5,2));
			Assert.AreEqual(50.0,_baseGrid.GetYCoordinate(5,3));
			
			Assert.AreEqual(10.0,_baseGrid.GetYCoordinate(6,0));
			Assert.AreEqual(10.0,_baseGrid.GetYCoordinate(6,1));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(6,2));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(6,3));
			
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(7,0));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(7,1));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(7,2));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(7,3));

			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(8,0));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(8,1));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(8,2));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(8,3));

			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(9,0));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(9,1));
			Assert.AreEqual(50.0,_baseGrid.GetYCoordinate(9,2));
			Assert.AreEqual(50.0,_baseGrid.GetYCoordinate(9,3));
			
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(10,0));
			Assert.AreEqual(20.0,_baseGrid.GetYCoordinate(10,1));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(10,2));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(10,3));
			
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(11,0));
			Assert.AreEqual(30.0,_baseGrid.GetYCoordinate(11,1));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(11,2));
			Assert.AreEqual(40.0,_baseGrid.GetYCoordinate(11,3));
		}

		[Test]
		[ExpectedException(typeof(System.Exception))]
		public void GetZCoordinate()
		{
            WriteMethodNameToConsole("GetZCoordinate");
			_baseGrid.GetZCoordinate(5,0);
		}
		

		[Test]
		public void GetElementID()
		{
            WriteMethodNameToConsole("GetElementID");
			Assert.AreEqual("15_25",_baseGrid.GetElementID(0));
			Assert.AreEqual("45_35",_baseGrid.GetElementID(11));

		}

		[Test]
		public void Description()
		{
            WriteMethodNameToConsole("Description");
			Assert.AreEqual("Regular MIKE SHE Grid",_baseGrid.Description);
		}

		[Test]
		public void ID()
		{
            WriteMethodNameToConsole("ID");
			Assert.AreEqual("BaseGrid",_baseGrid.ID);
		}

		[Test]
		public void GetFaceVertexIndices()
		{
            WriteMethodNameToConsole("GetFaceVertexIndices");
			Assert.AreEqual(null,_baseGrid.GetFaceVertexIndices(0,0));
		}

		[Test]
		public void GetFaceCount()
		{
            WriteMethodNameToConsole("GetFaceCount");
			Assert.AreEqual(0,_baseGrid.GetFaceCount(0));
			Assert.AreEqual(0,_baseGrid.GetFaceCount(11));
		}

		[Test]
		public void ElementType()
		{
            WriteMethodNameToConsole("ElementType");
			Assert.AreEqual(global::OpenMI.Standard.ElementType.XYPolygon,_baseGrid.ElementType);
		}

		[Test]
		public void Version()
		{
            WriteMethodNameToConsole("Version");
			Assert.AreEqual(0,_baseGrid.Version);
		}

		[Test]
		public void ElementCount()
		{
            WriteMethodNameToConsole("ElementCount");
			Assert.AreEqual(12,_baseGrid.ElementCount);

            // Now using a different contructor for GridInfo object

            double ox = 1000.0;
            double oy = 2000.0;
            double cellSize = 900.0;
            int    nx = 2;
            int    ny = 2;
            
            Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GridInfo gridInfo = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.GridInfo(ox, oy, cellSize, nx, ny, 0);
            IElementSet elementSet = new Oatc.OpenMI.Examples.ModelComponents.SpatialModels.GroundWaterModel.BaseGrid(gridInfo);
            Assert.AreEqual(4, elementSet.ElementCount);
		}

		[Test]
		public void GetVertexCount()
		{
            WriteMethodNameToConsole("GetVertexCount");
			Assert.AreEqual(4, _baseGrid.GetVertexCount(0));
			Assert.AreEqual(4, _baseGrid.GetVertexCount(11));
		}

		[Test]
		public void SpatialReference()
		{
            WriteMethodNameToConsole("SpatialReference");
			Assert.AreEqual(new Oatc.OpenMI.Sdk.Backbone.SpatialReference("no reference"), _baseGrid.SpatialReference);
		}

		[Test]
		public void GetElementIndex()
		{
            WriteMethodNameToConsole("GetElementIndex");
			Assert.AreEqual(3,_baseGrid.GetElementIndex("25_25"));
			Assert.AreEqual(11,_baseGrid.GetElementIndex("45_35"));
		}

        

        private void WriteMethodNameToConsole(string NameSpaceClassAndMethodName)
        {
            bool isSilent = false;
            if (! isSilent)
            {
                Console.WriteLine("DHI.OpenMI.MikeShe.UnitTest.BaseGridTest." + NameSpaceClassAndMethodName + "() --- Test started");
                Console.Out.Flush();
            }
        }
	}
}
