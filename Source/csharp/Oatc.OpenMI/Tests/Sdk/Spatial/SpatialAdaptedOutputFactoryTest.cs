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
﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
    [TestFixture]
    public class SpatialAdaptedOutputFactoryTest
    {
        private IQuantity waterLevelQuantity;
        private ElementSet xyPolygon;
        private ElementSet xyPointA;
        private ElementSet xyPolylineA;
        private ElementSet xyPointB;
        private IAdaptedOutputFactory adaptedOutputFactory;

        [SetUp]
        public void CreateTestData()
        {
            waterLevelQuantity = new Quantity(
                new Unit(PredefinedUnits.CubicMeterPerSecond), "Flow", "Flow");

            xyPolygon = new ElementSet("one polygon, 5 points", "xyPolygon", ElementType.Polygon);

            Element p0 = new Element("p0");

            p0.AddVertex(new Coordinate(7.5, 10, 0));
            p0.AddVertex(new Coordinate(15, 20, 0));
            p0.AddVertex(new Coordinate(10, 30, 0));
            p0.AddVertex(new Coordinate(5, 30, 0));
            p0.AddVertex(new Coordinate(0, 20, 0));
            xyPolygon.AddElement(p0);

            xyPolylineA = new ElementSet("3 points polyline", "xyPolylineA", ElementType.PolyLine);

            Element l0 = new Element("PL_0");
            Element l1 = new Element("PL_1");

            Coordinate v0 = new Coordinate(0, 20, 0);
            Coordinate v1 = new Coordinate(0, 10, 0);
            Coordinate v2 = new Coordinate(0, 0, 0);

            l0.AddVertex(v0);
            l0.AddVertex(v2);

            l1.AddVertex(v1);
            l1.AddVertex(v2);

            xyPolylineA.AddElement(l0);
            xyPolylineA.AddElement(l1);

            xyPointA = new ElementSet("4 points elementset A", "pointsA", ElementType.Point);
            Element e0 = new Element("Pnt_0");
            Element e1 = new Element("Pnt_1");
            Element e2 = new Element("Pnt_2");
            Element e3 = new Element("Pnt_3");
            e0.AddVertex(new Coordinate(0, 100, 0));
            e1.AddVertex(new Coordinate(0, 0, 0));
            e2.AddVertex(new Coordinate(100, 0, 0));
            e3.AddVertex(new Coordinate(100, 100, 0));
            xyPointA.AddElement(e0);
            xyPointA.AddElement(e1);
            xyPointA.AddElement(e2);
            xyPointA.AddElement(e3);

            xyPointB = new ElementSet("4 points elementset B", "pointsB", ElementType.Point);
            e0 = new Element("e0");
            e1 = new Element("e1");
            e0.AddVertex(new Coordinate(25, 25, 0));
            e1.AddVertex(new Coordinate(25, 75, 0));
            xyPointB.AddElement(e0);
            xyPointB.AddElement(e1);

            adaptedOutputFactory = new SpatialAdaptedOutputFactory("Oatc Spatial Adapted Output Factory");
        }

        [Test]
        public void AvailableAdaptedOutputsPolygonToPoint()
        {
            ITimeSpaceOutput adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumer = new Input(xyPointA.Caption + ".Flow") { SpatialDefinition = xyPointA, ValueDefinition = waterLevelQuantity };

            List<IDescribable> adaptedOutputDescriptions = new List<IDescribable>();
            IIdentifiable[] availableAdaptedOutputIds = GetAvailableAdaptedOutputIdsAndDescriptions(adaptee, consumer,
                                                                                                    adaptedOutputDescriptions);

            Assert.AreEqual(2, availableAdaptedOutputIds.Length, "# availableAdaptedOutputIds");
            Assert.AreEqual("ElementOperation300", availableAdaptedOutputIds[0].Id, "availableAdaptedOutputIds[0].Id");
            Assert.AreEqual("ElementOperation300", adaptedOutputDescriptions[0].Caption, "availableAdaptedOutputIds[0].Caption");
            Assert.AreEqual("Polygon operation, multiply by area", adaptedOutputDescriptions[0].Description, "availableAdaptedOutputIds[0].Description");
            
            Assert.AreEqual("ElementMapper600", availableAdaptedOutputIds[1].Id, "availableAdaptedOutputIds[0].Id");
            Assert.AreEqual("ElementMapper600", adaptedOutputDescriptions[1].Caption, "availableAdaptedOutputIds[0].Caption");
            Assert.AreEqual("Polygon-to-point Value", adaptedOutputDescriptions[1].Description, "availableAdaptedOutputIds[0].Description");
        }

        [Test]
        public void AvailableAdaptedOutputsPolygonToPolyLine()
        {
            ITimeSpaceOutput adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumer = new Input(xyPolylineA.Caption + ".Flow") { SpatialDefinition = xyPolylineA, ValueDefinition = waterLevelQuantity };

            List<IDescribable> adaptedOutputDescriptions = new List<IDescribable>();
            IIdentifiable[] availableAdaptedOutputIds = GetAvailableAdaptedOutputIdsAndDescriptions(adaptee, consumer,
                                                                                                    adaptedOutputDescriptions);

            Assert.AreEqual(3, availableAdaptedOutputIds.Length, "# availableAdaptedOutputIds");
            Assert.AreEqual("ElementOperation300", availableAdaptedOutputIds[0].Id, "availableAdaptedOutputIds[0].Id");
            Assert.AreEqual("ElementOperation300", adaptedOutputDescriptions[0].Caption, "availableAdaptedOutputIds[0].Caption");
            Assert.AreEqual("Polygon operation, multiply by area", adaptedOutputDescriptions[0].Description, "availableAdaptedOutputIds[0].Description");

            Assert.AreEqual("ElementMapper700", availableAdaptedOutputIds[1].Id, "availableAdaptedOutputIds[0].Id");
            Assert.AreEqual("ElementMapper700", adaptedOutputDescriptions[1].Caption, "availableAdaptedOutputIds[0].Caption");
            Assert.AreEqual("Polygon-to-polyline Weighted Mean", adaptedOutputDescriptions[1].Description, "availableAdaptedOutputIds[0].Description");

            Assert.AreEqual("ElementMapper701", availableAdaptedOutputIds[2].Id, "availableAdaptedOutputIds[1].Id");
            Assert.AreEqual("ElementMapper701", adaptedOutputDescriptions[2].Caption, "Inverse");
            Assert.AreEqual("Polygon-to-polyline Weighted Sum", adaptedOutputDescriptions[2].Description, "availableAdaptedOutputIds[1].Description");
        }

        [Test]
        public void CreatePolygon2PolylineAdaptedOutput()
        {
            ITimeSpaceOutput adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumer = new Input(xyPolylineA.Caption + ".Flow") { SpatialDefinition = xyPolylineA, ValueDefinition = waterLevelQuantity };

            IIdentifiable selectedAvailableAdaptedOutputId = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee, consumer)[1];
            ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)adaptedOutputFactory.CreateAdaptedOutput(selectedAvailableAdaptedOutputId, adaptee, consumer);

            adaptedOutput.AddConsumer(consumer);

            Assert.IsTrue(adaptedOutput.ElementSet() == consumer.ElementSet(), "Consumer's ElementSet");
            Assert.IsTrue(adaptedOutput.ValueDefinition == adaptee.ValueDefinition, "Adaptee's ValueDefinition");
        }

        [Test]
        public void CreatePolygon2PointAdaptedOutput2Consumers()
        {
            ITimeSpaceOutput adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumerA = new Input(xyPointA.Caption + ".Flow") { SpatialDefinition = xyPointA, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumerB = new Input(xyPointB.Caption + ".Flow") { SpatialDefinition = xyPointB, ValueDefinition = waterLevelQuantity };

            ITimeSpaceInput consumer = consumerB;

            IIdentifiable selectedAvailableAdaptedOutputId = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee, consumer)[1];
            ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)adaptedOutputFactory.CreateAdaptedOutput(selectedAvailableAdaptedOutputId, adaptee, consumer);
            adaptedOutput.AddConsumer(consumer);

            Assert.IsTrue(adaptedOutput.ElementSet().ElementType == ElementType.Point, "Consumer's ElementSet TYPE");
            Assert.AreEqual(2, adaptedOutput.ElementSet().ElementCount, "ElementSet of consumer");
            Assert.IsTrue(adaptedOutput.ValueDefinition == adaptee.ValueDefinition, "Adaptee's ValueDefinition");
        }

        [Test]
        public void Polygon2PolylineAdaptedOutputValues()
        {
            Output adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumer = new Input(xyPolylineA.Caption + ".Flow") { SpatialDefinition = xyPolylineA, ValueDefinition = waterLevelQuantity };

            IIdentifiable selectedAvailableAdaptedOutputId = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee, consumer)[1];
            IBaseAdaptedOutput adaptedOutput = adaptedOutputFactory.CreateAdaptedOutput(selectedAvailableAdaptedOutputId, adaptee, consumer);

            adaptedOutput.AddConsumer(consumer);

            IList<IList> values2D = new List<IList>();
            values2D.Add(new List<double> { 0.444 });
            adaptee.Values = new ValueSet(values2D);

            Assert.AreEqual(1, ValueSet.GetTimesCount((ITimeSpaceValueSet)adaptedOutput.Values), "adaptedOutput.Values timesCount");
            Assert.AreEqual(consumer.ElementSet().ElementCount, ValueSet.GetElementCount((ITimeSpaceValueSet)adaptedOutput.Values), "adaptedOutput.Values elementCount");
        }

        [Test]
        public void Polygon2PointAdaptedOutputValues2Consumers()
        {
            Output adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumer = new Input(xyPointA.Caption + ".Flow") { SpatialDefinition = xyPointA, ValueDefinition = waterLevelQuantity };
            //ITimeSpaceInput consumer = new Input(xyPointB.Caption + ".Flow") { ElementSet = xyPointB, ValueDefinition = waterLevelQuantity };

            IIdentifiable selectedAvailableAdaptedOutputId = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee, consumer)[1];
            IBaseAdaptedOutput adaptedOutput = adaptedOutputFactory.CreateAdaptedOutput(selectedAvailableAdaptedOutputId, adaptee, consumer);
            adaptedOutput.AddConsumer(consumer);

            IList<IList> values2D = new List<IList>();
            values2D.Add(new List<double> { 0.444 });
            adaptee.Values = new ValueSet(values2D);

            Assert.AreEqual(1, ValueSet.GetTimesCount((ITimeSpaceValueSet)adaptedOutput.Values), "adaptedOutput.Values TimesCount");
            Assert.AreEqual(4, ValueSet.GetElementCount((ITimeSpaceValueSet)adaptedOutput.Values), "adaptedOutput.Values ElementCount == 0");
        }

        [Test]
        public void Polygon2PointAdaptedOutputGetValues2Consumers()
        {
            Output adaptee = new Output(xyPolygon.Caption + ".Flow") { SpatialDefinition = xyPolygon, ValueDefinition = waterLevelQuantity };
            ITimeSpaceInput consumerA = new Input(xyPointA.Caption + ".Flow") { SpatialDefinition = xyPointA, ValueDefinition = waterLevelQuantity };

            IIdentifiable selectedAvailableAdaptedOutputId = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee, consumerA)[1];
            ITimeSpaceAdaptedOutput adaptedOutput = (ITimeSpaceAdaptedOutput)adaptedOutputFactory.CreateAdaptedOutput(selectedAvailableAdaptedOutputId, adaptee, consumerA);
            adaptedOutput.AddConsumer(consumerA);

            IList<IList> values2D = new List<IList>();
            values2D.Add(new List<double> { 0.444 });
            adaptee.Values = new ValueSet(values2D);

            ITimeSpaceValueSet adaptedValuesA = adaptedOutput.GetValues(consumerA);
            Assert.AreEqual(1, ValueSet.GetTimesCount(adaptedValuesA), "adaptedValuesA.TimesCount");
            Assert.AreEqual(consumerA.ElementSet().ElementCount, ValueSet.GetElementCount(adaptedValuesA), "adaptedValuesA.ElementCount");

        }

        [Test]
        private IIdentifiable[] GetAvailableAdaptedOutputIdsAndDescriptions(ITimeSpaceOutput adaptee, ITimeSpaceInput consumer,
                                                                            ICollection<IDescribable>
                                                                                adaptedOutputDescriptions)
        {
            IIdentifiable[] availableAdaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(adaptee,
                                                                                                          consumer);
            foreach (IIdentifiable adaptedOutputId in availableAdaptedOutputIds)
            {
                adaptedOutputDescriptions.Add(adaptedOutputId);
            }
            return availableAdaptedOutputIds;
        }
    }
}