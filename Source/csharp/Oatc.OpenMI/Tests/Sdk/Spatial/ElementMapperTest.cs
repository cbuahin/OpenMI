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
using System;
using System.Collections.Generic;
using System.Collections;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using NUnit.Framework;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    [TestFixture]
    public class ElementMapperTest
    {
        private static ElementSet CreateElementSet(string elementSetID)
        {
            if (elementSetID == "4 Points")
            {
                ElementSet fourPointsElementSet = new ElementSet("4 points","4 Points",ElementType.Point); 
		
                Element e0 = new Element("e0"); 
                Element e1 = new Element("e1"); 
                Element e2 = new Element("e2");
                Element e3 = new Element("e3"); 
		
                e0.AddVertex(new Coordinate( 0,100,0));
                e1.AddVertex(new Coordinate( 0,0,0));
                e2.AddVertex(new Coordinate( 100,0,0));
                e3.AddVertex(new Coordinate(100,100,0));

                fourPointsElementSet.AddElement(e0);
                fourPointsElementSet.AddElement(e1);
                fourPointsElementSet.AddElement(e2);
                fourPointsElementSet.AddElement(e3);

                return fourPointsElementSet;
            }
            if (elementSetID == "2 Points")
            {
                ElementSet twoPointsElementSet = new ElementSet("2 points","2 Points",ElementType.Point); 

                Element k0 = new Element("k0"); 
                Element k1 = new Element("k1"); 

                k0.AddVertex(new Coordinate( 0,75,0));
                k1.AddVertex(new Coordinate( 200, 50, 0));

                twoPointsElementSet.AddElement(k0);
                twoPointsElementSet.AddElement(k1);

                return twoPointsElementSet;

            }
            if (elementSetID == "4 Other Points")
            {
                ElementSet fourPointsElementSet = new ElementSet("4 Other points","4 Other Points",ElementType.Point); 
		
                Element e0 = new Element("e0"); 
                Element e1 = new Element("e1"); 
                Element e2 = new Element("e2");
                Element e3 = new Element("e3"); 
		
                e0.AddVertex(new Coordinate( 0,15,0));
                e1.AddVertex(new Coordinate( 5,15,0));
                e2.AddVertex(new Coordinate( 0,10,0));
                e3.AddVertex(new Coordinate(10,10,0));

                fourPointsElementSet.AddElement(e0);
                fourPointsElementSet.AddElement(e1);
                fourPointsElementSet.AddElement(e2);
                fourPointsElementSet.AddElement(e3);

                return fourPointsElementSet;
            }
            if(elementSetID == "3 points polyline")
            {
                ElementSet lineElementSet = new ElementSet("3 points polyline","3 points polyline",ElementType.PolyLine); 

                Element l0 = new Element("k0"); 
                Element l1 = new Element("k1");
 
                Coordinate v0 = new Coordinate(0 ,20, 0);
                Coordinate v1 = new Coordinate(0 ,10, 0);
                Coordinate v2 = new Coordinate(0 , 0, 0);

                l0.AddVertex(v0);
                l0.AddVertex(v2);

                l1.AddVertex(v1);
                l1.AddVertex(v2);

                lineElementSet.AddElement(l0);
                lineElementSet.AddElement(l1);
                return lineElementSet;
            }
            throw new Exception("Cound not find specified elementset");
        }

        [Test] // testing the Initialise method
        public void Initialise()
        {
            ElementSet fourPointsElementSet = CreateElementSet("4 Points");
            ElementSet twoPointsElementSet  = CreateElementSet("2 Points");
      
            ElementMapper elementMapper = new ElementMapper();

            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(fourPointsElementSet.ElementType, twoPointsElementSet.ElementType);
            elementMapper.Initialise(methods[0], fourPointsElementSet,  twoPointsElementSet);

            double calculated = elementMapper.GetValueFromMappingMatrix(0, 0);
            const double expected = 1;

            Assert.AreEqual(expected,calculated);
        }

        [Test] // testing the Initialise method
        public void UpdateMappingMatrix_PointPoint()
        {
            ElementSet fourPointsElementSet = CreateElementSet("4 Points");
            ElementSet twoPointsElementSet = CreateElementSet("2 Points");

            ElementMapper elementMapper = new ElementMapper();

            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(fourPointsElementSet.ElementType, twoPointsElementSet.ElementType);

            elementMapper.Initialise(methods[0], fourPointsElementSet,  twoPointsElementSet);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0)
                               +elementMapper.GetValueFromMappingMatrix(0, 1)
                               +elementMapper.GetValueFromMappingMatrix(0, 2)
                               +elementMapper.GetValueFromMappingMatrix(0, 3));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(1, 2));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
                               +elementMapper.GetValueFromMappingMatrix(1, 1)
                               +elementMapper.GetValueFromMappingMatrix(1, 2)
                               +elementMapper.GetValueFromMappingMatrix(1, 3));
  	      
            elementMapper.Initialise(methods[1], fourPointsElementSet,  twoPointsElementSet);
            Assert.AreEqual(0.56310461156889, elementMapper.GetValueFromMappingMatrix(0, 0),0.000000001);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0)
                               +elementMapper.GetValueFromMappingMatrix(0, 1)
                               +elementMapper.GetValueFromMappingMatrix(0, 2)
                               +elementMapper.GetValueFromMappingMatrix(0, 3));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
                               +elementMapper.GetValueFromMappingMatrix(1, 1)
                               +elementMapper.GetValueFromMappingMatrix(1, 2)
                               +elementMapper.GetValueFromMappingMatrix(1, 3),0.000000001);
        }
        [Test] // testing the Initialise method
        public void UpdateMappingMatrix_PointPolyline()
        {
            ElementSet fourPointsElementSet = CreateElementSet("4 Other Points");
            ElementSet lineElementSet = CreateElementSet("3 points polyline");
     
            ElementMapper elementMapper = new ElementMapper();
      
            // point to polyline
            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(fourPointsElementSet.ElementType, lineElementSet.ElementType);

            elementMapper.Initialise(methods[0], fourPointsElementSet, lineElementSet);
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
        
            elementMapper.Initialise(methods[0], fourPointsElementSet, lineElementSet);
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
        
            // polyline to point
            methods = SpatialAdaptedOutputFactory.GetAvailableMethods(lineElementSet.ElementType, fourPointsElementSet.ElementType);
            elementMapper.Initialise(methods[0], lineElementSet, fourPointsElementSet);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 0));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 0));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 1));
        
            elementMapper.Initialise(methods[1], lineElementSet, fourPointsElementSet);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(0.585786437626905, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0)
                               + elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 0));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(2, 1));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 0));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(3, 1));      
        }
        [Test] // testing the Initialise method
        public void UpdateMappingMatrix_PointPolygon()
        {
            ElementSet gridElementSet = new ElementSet("gridElm","G1",ElementType.Polygon);
            ElementSet fourPointsElementSet = new ElementSet("4 points","4P",ElementType.Point); 

            Coordinate v_0_20  = new Coordinate(0,20,0);
            Coordinate v_0_10  = new Coordinate(0,10,0);
            Coordinate v_0_0   = new Coordinate(0, 0,0);
            Coordinate v_5_15  = new Coordinate(5,15,0);
            Coordinate v_10_20 = new Coordinate(10,20,0);
            Coordinate v_10_15 = new Coordinate(10,15,0);
            Coordinate v_10_10 = new Coordinate(10,10,0);
            Coordinate v_10_0  = new Coordinate(10, 0,0);
            Coordinate v_15_15 = new Coordinate(15,15,0);
            Coordinate v_15_5  = new Coordinate(15,5,0);
            Coordinate v_20_20 = new Coordinate(20,20,0);
            Coordinate v_20_10 = new Coordinate(20,10,0);

            Element square1 = new Element("square1");
            Element square2 = new Element("square2");
            Element square3 = new Element("square3");

            square1.AddVertex(v_0_20);
            square1.AddVertex(v_0_10);
            square1.AddVertex(v_10_10);
            square1.AddVertex(v_10_20);

            square2.AddVertex(v_10_20);
            square2.AddVertex(v_10_10);
            square2.AddVertex(v_20_10);
            square2.AddVertex(v_20_20);

            square3.AddVertex(v_0_10);
            square3.AddVertex(v_0_0);
            square3.AddVertex(v_10_0);
            square3.AddVertex(v_10_10);

            gridElementSet.AddElement(square1);
            gridElementSet.AddElement(square2);
            gridElementSet.AddElement(square3);

            Element point_5_15  = new Element("point 5, 15");
            Element point_10_15 = new Element("point 10, 15");
            Element point_15_15 = new Element("point 15, 15");
            Element point_15_5  = new Element("point 15, 5");

            point_5_15.AddVertex(v_5_15);
            point_10_15.AddVertex(v_10_15);
            point_15_15.AddVertex(v_15_15);
            point_15_5.AddVertex(v_15_5);

            fourPointsElementSet.AddElement(point_5_15);
            fourPointsElementSet.AddElement(point_10_15);
            fourPointsElementSet.AddElement(point_15_15);
            fourPointsElementSet.AddElement(point_15_5);
        
            ElementMapper elementMapper = new ElementMapper();
      
            // point to polygon

            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(fourPointsElementSet.ElementType, gridElementSet.ElementType);
			
            elementMapper.Initialise(methods[0], fourPointsElementSet, gridElementSet);
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0.5, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 3));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 3));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 3));

            // polygon to point
            methods = SpatialAdaptedOutputFactory.GetAvailableMethods(gridElementSet.ElementType, fourPointsElementSet.ElementType);

            elementMapper.Initialise(methods[0], gridElementSet, fourPointsElementSet);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(0, 2));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(1, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 0));
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(2, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(2, 2));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 0));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 1));
            Assert.AreEqual(0, elementMapper.GetValueFromMappingMatrix(3, 2));
        }
        [Test] // testing the Initialise method
        public void UpdateMappingMatrix_PolylinePolygon()
        {
            ElementSet twoSquaresGrid = new ElementSet("TwoSquaresGrid","TwoSquaresGrid",ElementType.Polygon);
  		
            Element e1 = new Element("e1");
            Element e2 = new Element("e2");
  		
            e1.AddVertex(new Coordinate(1,1,0));
            e1.AddVertex(new Coordinate(3,1,0));
            e1.AddVertex(new Coordinate(3,3,0));
            e1.AddVertex(new Coordinate(1,3,0));

            e2.AddVertex(new Coordinate(3,1,0));
            e2.AddVertex(new Coordinate(5,1,0));
            e2.AddVertex(new Coordinate(5,3,0));
            e2.AddVertex(new Coordinate(3,3,0));

            twoSquaresGrid.AddElement(e1);
            twoSquaresGrid.AddElement(e2);

            ElementSet twoLines = new ElementSet("TwoLines","TwoLines",ElementType.PolyLine);

            Element l1 = new Element("l1");
            Element l2 = new Element("l2");

            l1.AddVertex(new Coordinate(0,2.5,0));
            l1.AddVertex(new Coordinate(2,2.5,0));
            l2.AddVertex(new Coordinate(2,2.5,0));
            l2.AddVertex(new Coordinate(4,1.5,0));

            twoLines.AddElement(l1);
            twoLines.AddElement(l2);
      
            // Line to Polygon
            ElementMapper elementMapper = new ElementMapper();

            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(twoLines.ElementType, twoSquaresGrid.ElementType);
            elementMapper.Initialise(methods[0], twoLines,twoSquaresGrid);
            Assert.AreEqual(1/(1+Math.Sqrt(1+Math.Pow(0.5,2))),elementMapper.GetValueFromMappingMatrix(0,0),"Test1");
            Assert.AreEqual(1-1/(1+Math.Sqrt(1+Math.Pow(0.5,2))),elementMapper.GetValueFromMappingMatrix(0,1),"Test2");
            Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(1,0),"Test3");
            Assert.AreEqual(1,elementMapper.GetValueFromMappingMatrix(1,1),"Test4");

            elementMapper.Initialise(methods[1], twoLines,twoSquaresGrid);
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,0),"Test5");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,1),"Test6");
            Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(1,0),"Test7");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test8");
      
            // Polygon To PolyLine
            methods = SpatialAdaptedOutputFactory.GetAvailableMethods(twoSquaresGrid.ElementType, twoLines.ElementType);
            elementMapper.Initialise(methods[0], twoSquaresGrid, twoLines);
            Assert.AreEqual(1.0,elementMapper.GetValueFromMappingMatrix(0,0),"Test9");
            Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(0,1),"Test10");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,0),"Test11");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test12");

            elementMapper.Initialise(methods[1], twoSquaresGrid, twoLines);
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(0,0),"Test13");
            Assert.AreEqual(0.0,elementMapper.GetValueFromMappingMatrix(0,1),"Test14");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,0),"Test15");
            Assert.AreEqual(0.5,elementMapper.GetValueFromMappingMatrix(1,1),"Test16");
        }
        [Test] // testing the Initialise method
        public void UpdateMappingMatrix_PolygonPolygon()
        {
            Coordinate v1_0_10  = new Coordinate(0,10,0);
            Coordinate v1_0_0   = new Coordinate(0,0,0);
            Coordinate v1_10_0  = new Coordinate(10,0,0);
            Coordinate v1_10_10 = new Coordinate(10,10,0);
            Coordinate v1_20_0  = new Coordinate(20,0,0);
            Coordinate v1_20_10 = new Coordinate(20,10,0);
            Coordinate v1_5_9   = new Coordinate(5,9,0);
            Coordinate v1_5_1   = new Coordinate(5,1,0);
            Coordinate v1_15_5  = new Coordinate(15,5,0);

            Element leftSquare  = new Element("LeftSquare");
            leftSquare.AddVertex(v1_0_10);
            leftSquare.AddVertex(v1_0_0);
            leftSquare.AddVertex(v1_10_0);
            leftSquare.AddVertex(v1_10_10);
      
            Element rightSquare = new Element("RightSquare");
            rightSquare.AddVertex(v1_10_10);
            rightSquare.AddVertex(v1_10_0);
            rightSquare.AddVertex(v1_20_0);
            rightSquare.AddVertex(v1_20_10);

            Element triangle    = new Element("Triangle");
            triangle.AddVertex(v1_5_9);
            triangle.AddVertex(v1_5_1);
            triangle.AddVertex(v1_15_5);

            ElementSet twoSquareElementSet      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.Polygon);
            ElementSet triangleElementSet       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.Polygon);
            twoSquareElementSet.AddElement(leftSquare);
            twoSquareElementSet.AddElement(rightSquare);
            triangleElementSet.AddElement(triangle);
      
            ElementMapper elementMapper = new ElementMapper();
            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(twoSquareElementSet.ElementType, triangleElementSet.ElementType);
            elementMapper.Initialise(methods[0], triangleElementSet,  twoSquareElementSet);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test1");
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(1, 0),"Test2");
      
            elementMapper.Initialise(methods[0], twoSquareElementSet, triangleElementSet);
            Assert.AreEqual(0.75, elementMapper.GetValueFromMappingMatrix(0, 0),0.000000001,"Test3");
            Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 1),"Test4");
	      
            elementMapper.Initialise(methods[1], triangleElementSet,  twoSquareElementSet);
            Assert.AreEqual(0.3, elementMapper.GetValueFromMappingMatrix(0, 0),"Test5");
            Assert.AreEqual(0.1, elementMapper.GetValueFromMappingMatrix(1, 0),"Test6");
	    
            elementMapper.Initialise(methods[1], twoSquareElementSet, triangleElementSet);
            Assert.AreEqual(0.75, elementMapper.GetValueFromMappingMatrix(0, 0),0.0000000001,"Test7");
            Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 1),"Test8");
      
            Coordinate v2_0_2 = new Coordinate(0,2,0);
            Coordinate v2_0_0 = new Coordinate(0,0,0);
            Coordinate v2_2_0 = new Coordinate(2,0,0);
            Coordinate v2_1_2 = new Coordinate(1,2,0);
            Coordinate v2_1_0 = new Coordinate(1,0,0);
            Coordinate v2_3_0 = new Coordinate(3,0,0);

            Element leftTriangle2  = new Element("leftTriangle");
            leftTriangle2.AddVertex(v2_0_2);
            leftTriangle2.AddVertex(v2_0_0);
            leftTriangle2.AddVertex(v2_2_0);

            Element rightTriangle2  = new Element("rightTriangle");
            rightTriangle2.AddVertex(v2_1_2);
            rightTriangle2.AddVertex(v2_1_0);
            rightTriangle2.AddVertex(v2_3_0);
      
            ElementSet leftTriangleElementSet2      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.Polygon);
            ElementSet rightTriangleElementSet2       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.Polygon);
            leftTriangleElementSet2.AddElement(leftTriangle2);
            rightTriangleElementSet2.AddElement(rightTriangle2);

      
            elementMapper.Initialise(methods[0], leftTriangleElementSet2, rightTriangleElementSet2);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test9");

            elementMapper.Initialise(methods[0], rightTriangleElementSet2, leftTriangleElementSet2);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test10");
      
            elementMapper.Initialise(methods[1], leftTriangleElementSet2, rightTriangleElementSet2);
            Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 0),"Test11");

            elementMapper.Initialise(methods[1], rightTriangleElementSet2, leftTriangleElementSet2);
            Assert.AreEqual(0.25, elementMapper.GetValueFromMappingMatrix(0, 0),"Test12");
      
            Coordinate v3_0_2 = new Coordinate(0,2,0);
            Coordinate v3_0_0 = new Coordinate(0,0,0);
            Coordinate v3_2_0 = new Coordinate(2,0,0);
            Coordinate v3_1_2 = new Coordinate(1,2,0);
            Coordinate v3_1_0 = new Coordinate(1,0,0);
            Coordinate v3_3_2 = new Coordinate(3,2,0);
    
            Element leftTriangle3  = new Element("leftTriangle");
            leftTriangle3.AddVertex(v3_0_2);
            leftTriangle3.AddVertex(v3_0_0);
            leftTriangle3.AddVertex(v3_2_0);

            Element rightTriangle3  = new Element("rightTriangle");
            rightTriangle3.AddVertex(v3_1_2);
            rightTriangle3.AddVertex(v3_1_0);
            rightTriangle3.AddVertex(v3_3_2);
      
            ElementSet leftTriangleElementSet3      = new ElementSet("TwoSquareElementSet","TwoSquareElementSet",ElementType.Polygon);
            ElementSet rightTriangleElementSet3       = new ElementSet("TriangleElementSet","TriangleElementSet",ElementType.Polygon);
            leftTriangleElementSet3.AddElement(leftTriangle3);
            rightTriangleElementSet3.AddElement(rightTriangle3);

            elementMapper.Initialise(methods[0], leftTriangleElementSet3, rightTriangleElementSet3);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test13");

            elementMapper.Initialise(methods[0], rightTriangleElementSet3, leftTriangleElementSet3);
            Assert.AreEqual(1, elementMapper.GetValueFromMappingMatrix(0, 0),"Test14");
      
            elementMapper.Initialise(methods[1], leftTriangleElementSet3, rightTriangleElementSet3);
            Assert.AreEqual(0.125, elementMapper.GetValueFromMappingMatrix(0, 0),"Test15");

            elementMapper.Initialise(methods[1], rightTriangleElementSet3, leftTriangleElementSet3);
            Assert.AreEqual(0.125, elementMapper.GetValueFromMappingMatrix(0, 0),"Test16");
        }


        [Test] // testing the Initialise method
        public void MapValues()
        {
            ElementSet gridElementSet = new ElementSet("RegularGrid","RegularGrid",ElementType.Polygon);
            ElementSet fourPointsElementSet = new ElementSet("4 points","4P",ElementType.Point); 

            Coordinate v_0_20  = new Coordinate(0,20,0);
            Coordinate v_0_10  = new Coordinate(0,10,0);
            Coordinate v_0_0   = new Coordinate(0, 0,0);
            Coordinate v_5_15  = new Coordinate(5,15,0);
            Coordinate v_10_20 = new Coordinate(10,20,0);
            Coordinate v_10_15 = new Coordinate(10,15,0);
            Coordinate v_10_10 = new Coordinate(10,10,0);
            Coordinate v_10_0  = new Coordinate(10, 0,0);
            Coordinate v_15_15 = new Coordinate(15,15,0);
            Coordinate v_15_5  = new Coordinate(15,5,0);
            Coordinate v_20_20 = new Coordinate(20,20,0);
            Coordinate v_20_10 = new Coordinate(20,10,0);

            Element square1 = new Element("square1");
            Element square2 = new Element("square2");
            Element square3 = new Element("square3");

            square1.AddVertex(v_0_20);
            square1.AddVertex(v_0_10);
            square1.AddVertex(v_10_10);
            square1.AddVertex(v_10_20);

            square2.AddVertex(v_10_20);
            square2.AddVertex(v_10_10);
            square2.AddVertex(v_20_10);
            square2.AddVertex(v_20_20);

            square3.AddVertex(v_0_10);
            square3.AddVertex(v_0_0);
            square3.AddVertex(v_10_0);
            square3.AddVertex(v_10_10);

            gridElementSet.AddElement(square1);
            gridElementSet.AddElement(square2);
            gridElementSet.AddElement(square3);

            Element point_5_15  = new Element("point 5, 15");
            Element point_10_15 = new Element("point 10, 15");
            Element point_15_15 = new Element("point 15, 15");
            Element point_15_5  = new Element("point 15, 5");

            point_5_15.AddVertex(v_5_15);
            point_10_15.AddVertex(v_10_15);
            point_15_15.AddVertex(v_15_15);
            point_15_5.AddVertex(v_15_5);

            fourPointsElementSet.AddElement(point_5_15);
            fourPointsElementSet.AddElement(point_10_15);
            fourPointsElementSet.AddElement(point_15_15);
            fourPointsElementSet.AddElement(point_15_5);
      
            ElementMapper elementMapper = new ElementMapper();
      
            // point to polygon  

            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(fourPointsElementSet.ElementType, gridElementSet.ElementType);
            elementMapper.Initialise(methods[0], fourPointsElementSet, gridElementSet);
            IList values = new List<double>();
            values.Add(0d);
            values.Add(10d);
            values.Add(20d);
            values.Add(30d);
            ITimeSpaceValueSet fourPointsScalarSet = new ValueSet(new List<IList>{values});

            ITimeSpaceValueSet gridScalarSet = elementMapper.MapValues(fourPointsScalarSet);
            IList elementValuesForTime = gridScalarSet.GetElementValuesForTime(0);
            Assert.AreEqual(5, elementValuesForTime[0]);
            Assert.AreEqual(20, elementValuesForTime[1]);
            Assert.AreEqual(0, elementValuesForTime[2]);
			
            // polygon to point
            methods = SpatialAdaptedOutputFactory.GetAvailableMethods(gridElementSet.ElementType, fourPointsElementSet.ElementType);
            elementMapper.Initialise(methods[0], gridElementSet, fourPointsElementSet);
            fourPointsScalarSet = elementMapper.MapValues(gridScalarSet);
            elementValuesForTime = fourPointsScalarSet.GetElementValuesForTime(0);
            Assert.AreEqual(5, elementValuesForTime[0]);
            Assert.AreEqual(5, elementValuesForTime[1]);
            Assert.AreEqual(20, elementValuesForTime[2]);
            Assert.AreEqual(0, elementValuesForTime[3]);
        }

        [Test]
        public void GetAvailableMethods()
        {
            ElementType fromElementType = ElementType.Polygon;
            ElementType toElementType  = ElementType.Polygon;

            IIdentifiable[] availableMethods = SpatialAdaptedOutputFactory.GetAvailableMethods(fromElementType, toElementType);
            Assert.AreEqual(ElementMapperMethod.WeightedMean, SpatialAdaptedOutputFactory.GetMethod(availableMethods[0]));
            Assert.AreEqual(ElementMapperMethod.WeightedSum, SpatialAdaptedOutputFactory.GetMethod(availableMethods[1]));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExpectedException_UpdateMappingMatrix_ElementChecker()
        {
            //Two Vertices in point element error
            ElementSet elementSet = new ElementSet("test","test",ElementType.Point);
            Element e1 = new Element("e1");
            e1.AddVertex(new Coordinate(1,1,1));
            e1.AddVertex(new Coordinate(2,2,2)); //here the error is introduced on purpose
						
            elementSet.AddElement(e1);
			
            ElementMapper elementMapper = new ElementMapper();
            IIdentifiable method = SpatialAdaptedOutputFactory.GetAvailableMethods(ElementType.PolyLine,ElementType.Polygon)[0];
            elementMapper.Initialise(method,elementSet,elementSet);
        }

        [Test]
        public void GetAvailableAdaptedOutputs()
        {
            Dictionary<String, List<IArgument>> adaptedOutputTypes = new Dictionary<String, List<IArgument>>();

            foreach (ElementType fromElementType in Enum.GetValues(typeof(ElementType)))
            {
                Console.WriteLine(" ");
                Console.WriteLine("========================= FROM " + fromElementType + " ====================================");

                foreach (ElementType toElementType in Enum.GetValues(typeof (ElementType)))
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("-------------------------- TO " + toElementType + " --------------------------------------");

                    IIdentifiable[] availableMethods = SpatialAdaptedOutputFactory.GetAvailableMethods(fromElementType, toElementType);

                    foreach (IIdentifiable availableMethodIdentifier in availableMethods)
                    {
                        List<IArgument> arguments = SpatialAdaptedOutputFactory.GetAdaptedOutputArguments(availableMethodIdentifier);
                        adaptedOutputTypes.Add(availableMethodIdentifier.Id, arguments);

                        Console.WriteLine(" ");
                        Console.WriteLine(".........................................");
                        Console.WriteLine("method: " + availableMethodIdentifier.Id);
                        for (int i = 0; i < arguments.Count; i++)
                        {
                            Console.WriteLine(" ");
                            Console.WriteLine("Key:         " + arguments[i].Caption);
                            Console.WriteLine("Value:       " + arguments[i].Value);
                            Console.WriteLine("ReadOnly:    " + arguments[i].IsReadOnly);
                            Console.WriteLine("Description: " + arguments[i].Description);
                        }
                    }
                }
            }

            List<IArgument> argumentsForElementMapper801 = adaptedOutputTypes["ElementMapper801"];
            Assert.IsNotNull(argumentsForElementMapper801, "Arguments for ElementMapper 801");

            bool descriptionWasFound     = false;
            bool typeWasFound            = false;
            bool fromElementTypeWasFound = false;
            bool toElementTypeWasFound   = false;

            for (int i = 0; i < argumentsForElementMapper801.Count; i++)
            {
                /*if (operation.Arguments[i].Key == "Id")
                {
                    Assert.AreEqual("801",operation.Arguments[i].Value);
                    IDWasFound = true;
                }*/
                if (argumentsForElementMapper801[i].Caption == "Description")
                {
                    Assert.AreEqual("Polygon-to-polygon Weighted Sum", argumentsForElementMapper801[i].Value);
                    descriptionWasFound = true;
                }
                if (argumentsForElementMapper801[i].Caption == "Type")
                {
                    Assert.AreEqual("SpatialMapping", argumentsForElementMapper801[i].Value);
                    typeWasFound = true;
                }
                if (argumentsForElementMapper801[i].Caption == "FromElementType")
                {
                    Assert.AreEqual("Polygon", argumentsForElementMapper801[i].Value);
                    fromElementTypeWasFound = true;
                }
                if (argumentsForElementMapper801[i].Caption == "ToElementType")
                {
                    Assert.AreEqual("Polygon", argumentsForElementMapper801[i].Value);
                    toElementTypeWasFound = true;
                }
            }

            //Assert.AreEqual(true,IDWasFound);
            Assert.IsTrue(descriptionWasFound, "descriptionWasFound");
            Assert.IsTrue(typeWasFound, "typeWasFound");
            Assert.IsTrue(fromElementTypeWasFound, "fromElementTypeWasFound");
            Assert.IsTrue(toElementTypeWasFound, "toElementTypeWasFound");
		
        }
    }
}