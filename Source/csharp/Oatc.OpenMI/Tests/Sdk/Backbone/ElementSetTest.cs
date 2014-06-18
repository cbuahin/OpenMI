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
using NUnit.Framework;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Backbone
{
    [TestFixture]
    public class ElementSetTest
    {
        ElementSet elementSet;
        Element element1,element2;

        [SetUp]
        public void Init() 
        {
            elementSet = new ElementSet("ElementSet","ElementSetID", ElementType.Polygon);
			
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            int[] index = {1,2,3,4,5};
            element1.AddFace(index);
            elementSet.AddElement(element1);
			
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            int[] index2 = {6,7,8,9};
            element2.AddFace(index2);
            elementSet.AddElement(element2);	
        }

        [Test]
        public void Constructor()
        {
            Assert.AreEqual("ElementSet",elementSet.Description);
            Assert.AreEqual("ElementSetID",elementSet.Caption);
            Assert.AreEqual(ElementType.Polygon,elementSet.ElementType);
            Assert.AreEqual("", elementSet.SpatialReferenceSystemWkt);

            ElementSet elementSet2 = new ElementSet(elementSet);
            Assert.AreEqual(elementSet,elementSet2);
        }

        private static bool CompareIntArrays(int[] ar1,int[] ar2)
        {
            if (ar1.Length!=ar2.Length)
                return false;
            for (int i=0;i<ar1.Length;i++)
                if (ar1[i]!=ar2[i])
                    return false;
            return true;
        }

        [Test]
        public void CompareIntArrayTest()
        {
            int[] ar1 = {1,2,3,4};
            int[] ar2 = {1,2,3,4};
            int[] ar3 = {1,2};
            int[] ar4 = {1,2,3,5};

            Assert.IsTrue(CompareIntArrays(ar1,ar2));
            Assert.IsFalse(CompareIntArrays(ar1,ar3));
            Assert.IsFalse(CompareIntArrays(ar1,ar4));
        }

        [Test]
        public void Faces()
        {
            Assert.AreEqual(1,elementSet.GetFaceCount(0));
            Assert.AreEqual(1,elementSet.GetFaceCount(1));
            int[] index = {1,2,3,4,5};
            int[] index2 = {6,7,8,9};
            Assert.IsTrue(CompareIntArrays(index,elementSet.GetFaceVertexIndices(0,0)));
            Assert.IsTrue(CompareIntArrays(index2,elementSet.GetFaceVertexIndices(1,0)));

        }

        [Test]
        public void AddElement()
        {
            Assert.AreEqual(2,elementSet.ElementCount);
            Assert.AreEqual(element1,elementSet.Elements[0]);
            Assert.AreEqual(element2,elementSet.Elements[1]);
        }

        [Test]
        public void GetElement() 
        {
            Assert.AreEqual(element1,elementSet.GetElement(0));
            Assert.AreEqual(element2,elementSet.GetElement(1));
        }

        [Test]
        public void Element()
        {
            Assert.AreEqual(element1,elementSet.Elements[0]);
            Assert.AreEqual(element2,elementSet.Elements[1]);
        }

        [Test]
        public void GetElementId()
        {
            Assert.AreEqual("element1", elementSet.GetElementId(0).Id);
            Assert.AreEqual("element2", elementSet.GetElementId(1).Id);
        }

        [Test]
        public void GetXCoordinate()
        {
            Assert.AreEqual(1.0,elementSet.GetXCoordinate(0,0));
            Assert.AreEqual(2.0,elementSet.GetXCoordinate(0,1));
            Assert.AreEqual(4.0,elementSet.GetXCoordinate(0,2));
            Assert.AreEqual(6.0,elementSet.GetXCoordinate(1,0));
            Assert.AreEqual(9.0,elementSet.GetXCoordinate(1,1));
            Assert.AreEqual(12.0,elementSet.GetXCoordinate(1,2));
        }

        [Test]
        public void GetYCoordinate()
        {
            Assert.AreEqual(2.0,elementSet.GetYCoordinate(0,0));
            Assert.AreEqual(3.0,elementSet.GetYCoordinate(0,1));
            Assert.AreEqual(5.0,elementSet.GetYCoordinate(0,2));
            Assert.AreEqual(7.0,elementSet.GetYCoordinate(1,0));
            Assert.AreEqual(10.0,elementSet.GetYCoordinate(1,1));
            Assert.AreEqual(13.0,elementSet.GetYCoordinate(1,2));
        }

        [Test]
        public void GetZCoordinate()
        {
            Assert.AreEqual(3.0,elementSet.GetZCoordinate(0,0));
            Assert.AreEqual(4.0,elementSet.GetZCoordinate(0,1));
            Assert.AreEqual(6.0,elementSet.GetZCoordinate(0,2));
            Assert.AreEqual(8.0,elementSet.GetZCoordinate(1,0));
            Assert.AreEqual(11.0,elementSet.GetZCoordinate(1,1));
            Assert.AreEqual(14.0,elementSet.GetZCoordinate(1,2));
        }

        [Test]
        public void ElementCount()
        {
            Assert.AreEqual(2,elementSet.ElementCount);
        }

        [Test]
        public void VertexCount()
        {
            Assert.AreEqual(3,elementSet.GetVertexCount(0));
            Assert.AreEqual(3,elementSet.GetVertexCount(1));
        }

        [Test]
        public void GetElementIndex()
        {
            Assert.AreEqual(0,elementSet.GetElementIndex(new Identifier("element1")));
            Assert.AreEqual(1,elementSet.GetElementIndex(new Identifier("element2")));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetElementIndexException()
        {
            elementSet.GetElementIndex(new Identifier("element3")); // will throw exception
        }

        [Test]
        public void Equals()
        {
            ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID", ElementType.Polygon);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);

            Assert.IsTrue(elementSet.Equals(elementSet1));

            Assert.IsFalse(elementSet.Equals(null));
            Assert.IsFalse(elementSet.Equals("string"));

        }

        [Test]
        public void EqualsDescription()
        {
            ElementSet elementSet1 = new ElementSet("Element","ElementSetID", ElementType.Polygon);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);

            Assert.IsFalse(elementSet.Equals(elementSet1));
        }

        [Test]
        public void EqualsID()
        {
            ElementSet elementSet1 = new ElementSet("ElementSet","ElementID", ElementType.Polygon);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);


            Assert.IsFalse(elementSet.Equals(elementSet1));
        }

        [Test]
        public void EqualsElementType()
        {
            ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID", ElementType.PolyLine);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);


            Assert.IsFalse(elementSet.Equals(elementSet1));
        }

        [Test]
        public void EqualsSpatialReference()
        {
            ElementSet elementSet1 = new ElementSet("ElementSet", "ElementSetID", ElementType.Polygon);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.0,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
			
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);

            elementSet1.SpatialReferenceSystemWkt = "PROJCS[\"Mercator Spheric\", GEOGCS[\"WGS84based_GCS\", DATUM[\"WGS84based_Datum\", SPHEROID[\"WGS84based_Sphere\", 6378137, 0], TOWGS84[0, 0, 0, 0, 0, 0, 0]], PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9102\"]], AXIS[\"E\", EAST], AXIS[\"N\", NORTH]], PROJECTION[\"Mercator\"], PARAMETER[\"False_Easting\", 0], PARAMETER[\"False_Northing\", 0], PARAMETER[\"Central_Meridian\", 0], PARAMETER[\"Latitude_of_origin\", 0], UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]], AXIS[\"East\", EAST], AXIS[\"North\", NORTH]]";

            Assert.IsFalse(elementSet.Equals(elementSet1));
        }

        [Test]
        public void EqualsVertices()
        {
            ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID", ElementType.Polygon);
            element1 = new Element("element1");
            element1.AddVertex(new Coordinate(1.1,2.0,3.0));
            element1.AddVertex(new Coordinate(2.0,3.0,4.0));
            element1.AddVertex(new Coordinate(4.0,5.0,6.0));
            elementSet1.AddElement(element1);
            element2 = new Element("element2");
            element2.AddVertex(new Coordinate(6.0,7.0,8.0));
            element2.AddVertex(new Coordinate(9.0,10.0,11.0));
            element2.AddVertex(new Coordinate(12.0,13.0,14.0));
            elementSet1.AddElement(element2);

            Assert.IsFalse(elementSet.Equals(elementSet1));
        }

        private class Identifier : IIdentifiable
        {
            public Identifier(string id) { Id = id; }
            public string Id { get; set; }
            public string Caption { get; set; }
            public string Description { get; set; }
        }
    }
}