using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
    [TestFixture]
    public class ElementOperationAdaptorTest
    {


        [Test]
        public void ElementAreaOperationTest()
        {
            Spatial2DCurveLinearGrid grid = new Spatial2DCurveLinearGrid()
            {
                IsNodeBased = false,
                Caption = "grid",
                Description = "grid"
            };
            grid.Coordinates = new ICoordinate[3, 4];
            grid.Coordinates[0, 0] = new Coordinate(0, 0);
            grid.Coordinates[1, 0] = new Coordinate(5, 0);
            grid.Coordinates[2, 0] = new Coordinate(10, 0);

            grid.Coordinates[0, 1] = new Coordinate(0, 5);
            grid.Coordinates[1, 1] = new Coordinate(5, 5);
            grid.Coordinates[2, 1] = new Coordinate(10, 7);

            grid.Coordinates[0, 2] = new Coordinate(0, 12);
            grid.Coordinates[1, 2] = new Coordinate(5, 12);
            grid.Coordinates[2, 2] = new Coordinate(10, 16);

            grid.Coordinates[0, 3] = new Coordinate(0, 15);
            grid.Coordinates[1, 3] = new Coordinate(3, 17);
            grid.Coordinates[2, 3] = new Coordinate(7, 20);

            TestMperSOutput mperS = new TestMperSOutput("dummy", new Spatial2DGridWrapper(grid));

            foreach (double d in mperS.GetValues(null).GetElementValuesForTime<double>(0))
            {
                Assert.AreEqual(1.0, d);
            }

            ElementAreaOperationAdaptor adaptor = new ElementAreaOperationAdaptor("dummyadaptor", mperS);
            adaptor.Initialize();
            IQuantity quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * area^1", quantity.Caption);
            Assert.AreEqual("m/s * m^2", quantity.Unit.Caption);
            Assert.AreEqual(3, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            double[] values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(25, values[0]);
            Assert.AreEqual(30, values[1]);
            Assert.AreEqual(35, values[2]);
            Assert.AreEqual(40, values[3]);
            Assert.AreEqual(17, values[4]);
            Assert.AreEqual(29, values[5]);

            adaptor.Arguments[0].Value = -1.0;
            adaptor.Initialize();
            quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * area^-1", quantity.Caption);
            Assert.AreEqual(-1, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(1.0 / 25, values[0]);
            Assert.AreEqual(1.0 / 30, values[1]);
            Assert.AreEqual(1.0 / 35, values[2]);
            Assert.AreEqual(1.0 / 40, values[3]);
            Assert.AreEqual(1.0 / 17, values[4]);
            Assert.AreEqual(1.0 / 29, values[5]);

            adaptor.Arguments[0].Value = 0.5;
            adaptor.Initialize();
            quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * area^0.5", quantity.Caption);
            Assert.AreEqual(2, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(5, values[0]);
            Assert.AreEqual(Math.Sqrt(30), values[1], 1e-12);
            Assert.AreEqual(Math.Sqrt(35), values[2], 1e-12);
            Assert.AreEqual(Math.Sqrt(40), values[3], 1e-12);
            Assert.AreEqual(Math.Sqrt(17), values[4], 1e-12);
            Assert.AreEqual(Math.Sqrt(29), values[5], 1e-12);
        }


        [Test]
        public void ElementLineLengthOperationTest()
        {
            LineString polyLine = new LineString()
            {
                IsNodeBased = false,
                Caption = "grid",
                Description = "grid"
            };
            polyLine.Coordinates = new ICoordinate[4];
            polyLine.Coordinates[0] = new Coordinate(0, 0);
            polyLine.Coordinates[1] = new Coordinate(5, 0);
            polyLine.Coordinates[2] = new Coordinate(8, 4);
            polyLine.Coordinates[3] = new Coordinate(7, 5);

            TestMperSOutput mperS = new TestMperSOutput("dummy", new LineStringWrapper(polyLine));

            foreach (double d in mperS.GetValues(null).GetElementValuesForTime<double>(0))
            {
                Assert.AreEqual(1.0, d);
            }

            ElementLineLengthOperationAdaptor adaptor = new ElementLineLengthOperationAdaptor("dummyadaptor", mperS);
            adaptor.Initialize();
            IQuantity quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * length^1", quantity.Caption);
            Assert.AreEqual("m/s * m^1", quantity.Unit.Caption);
            Assert.AreEqual(2, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            double[] values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(5, values[0]);
            Assert.AreEqual(5, values[1]);
            Assert.AreEqual(Math.Sqrt(2), values[2]);

            adaptor.Arguments[0].Value = -1.0;
            adaptor.Initialize();
            quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * length^-1", quantity.Caption);
            Assert.AreEqual(0, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(1.0 / 5, values[0]);
            Assert.AreEqual(1.0 / 5, values[1]);
            Assert.AreEqual(1.0 / Math.Sqrt(2), values[2]);

            adaptor.Arguments[0].Value = 0.5;
            adaptor.Initialize();
            quantity = adaptor.Quantity();
            Assert.AreEqual("vd: m/s * length^0.5", quantity.Caption);
            Assert.AreEqual(1.5, quantity.Unit.Dimension.GetPower(DimensionBase.Length));

            values = adaptor.GetValues(null).GetElementValuesForTime<double>(0);
            Assert.AreEqual(Math.Sqrt(5), values[0], 1e-12);
            Assert.AreEqual(Math.Sqrt(5), values[1], 1e-12);
            Assert.AreEqual(Math.Sqrt(Math.Sqrt(2)), values[2], 1e-12);
        }


    }


    // Simple test class that outputs 1 for each element in the element set
    class TestMperSOutput : Output<double>
    {
        public TestMperSOutput(string id, IElementSet elmtSet)
            : base(id)
        {
            _spatialDefinition = elmtSet;
            Dimension dim = new Dimension(PredefinedDimensions.LengthPerTime);
            Unit unit = new Unit("m/s") { Dimension = dim };
            _valueDefinition = new Quantity(unit, "vd: m/s", "vd: m/s");

            _values = CreateValues();
        }

        public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
            return CreateValues();
        }

        private ITimeSpaceValueSet CreateValues()
        {
            IElementSet set = this.ElementSet();
            ValueSetArray<double> valueSet = new ValueSetArray<double>();
            double[] elmtValues = new double[set.ElementCount];
            for (int i = 0; i < set.ElementCount; i++)
            {
                elmtValues[i] = 1.0;
            }
            valueSet.Values2DArray.Add(elmtValues);
            return valueSet;
        }
    }



}
