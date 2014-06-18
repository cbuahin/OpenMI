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
﻿
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oatc.OpenMI.Examples.AsciiFileReader;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    [TestFixture]
    public class AsciiFileDataComponentTest
    {

        private Quantity _dischargeQuantity;
        private Quantity _waterlevelQuantity;
        private ElementSet _idBasedElementSetA;
        private Input _queryItem1;
        private Input _queryItem2;

        [SetUp]
        public void CreateModelsAndInputItemsForQuery()
        {
            _dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
            _waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

            _idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
            _idBasedElementSetA.AddElement(new Element("elm-1"));

            _queryItem1 = new Input("discharge, to be retrieved from some output item", _dischargeQuantity, _idBasedElementSetA);
            _queryItem1.TimeSet = new TimeSet();

            _queryItem2 = new Input("water level, to be retrieved from some output item", _waterlevelQuantity, _idBasedElementSetA);
            _queryItem2.TimeSet = new TimeSet();

        }


        [Test]
        public void GetValuesTest()
        {
            string dataFileRoot = @"..\..\..\..\..\Examples\AsciiFileReader\Data\";

            AsciiFileDataComponent ascii = new AsciiFileDataComponent();
            IArgument[] arguments = new IArgument[3];
            arguments[0] = Argument.Create("File1", dataFileRoot+"FlowDataId.txt", false, "Name of first file to load");
            arguments[1] = Argument.Create("File2", dataFileRoot+"WaterlevelDataPoints.txt", false, "Name of second file to load");
            arguments[2] = Argument.Create("File3", dataFileRoot + "RainDataGrid.txt", false, "Name of third file to load");
            ascii.Arguments = arguments;
            ascii.ApplyArguments(arguments);
            ascii.Initialize();

            Assert.AreEqual(3,ascii.Outputs.Count);

            ITimeSpaceOutput flowOutput = (ITimeSpaceOutput)ascii.Outputs[0];
            ITimeSpaceOutput levelOutput = (ITimeSpaceOutput)ascii.Outputs[1];
            ITimeSpaceOutput rainOutput = (ITimeSpaceOutput)ascii.Outputs[2];

            Assert.IsNotNull(flowOutput.ValueDefinition as IQuantity);
            Assert.AreEqual("Flow", flowOutput.ValueDefinition.Caption);

            Assert.IsNotNull(levelOutput.ValueDefinition as IQuantity);
            Assert.AreEqual("WaterLevel", levelOutput.ValueDefinition.Caption);


            Time startTime = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
            double startTimeMjd = startTime.StampAsModifiedJulianDay;
            double midTimeMjd = startTimeMjd + 1.3;
            double endTimeMjd = startTimeMjd + 3;
            double beforeStartTimeMjd = startTimeMjd - 1;
            double afterEndTimeMjd = startTimeMjd + 4;


            double[] expectedFlows = new double[] { 15.4, 18.2, 22.4 };
            double[] expectedLevels = new double[] { 5.4, 8.2, 9.4 };
            double[] expectedRain = new double[] { 15.4, 18.2, 22.4, 22.4 };

            _queryItem1.TimeSet.SetSingleTimeStamp(startTimeMjd);
            ITimeSpaceValueSet flows = flowOutput.GetValues(_queryItem1);
            ITimeSpaceValueSet levels = levelOutput.GetValues(_queryItem1);
            ITimeSpaceValueSet rain = rainOutput.GetValues(_queryItem1);
            Assert.AreEqual(expectedFlows, flows.Values2D[0]);
            Assert.AreEqual(expectedLevels, levels.Values2D[0]);
            Assert.AreEqual(expectedRain, rain.Values2D[0]);

            _queryItem1.TimeSet.SetSingleTimeStamp(beforeStartTimeMjd);
            flows = flowOutput.GetValues(_queryItem1);
            levels = levelOutput.GetValues(_queryItem1);
            rain = rainOutput.GetValues(_queryItem1);
            Assert.AreEqual(expectedFlows, flows.Values2D[0]);
            Assert.AreEqual(expectedLevels, levels.Values2D[0]);
            Assert.AreEqual(expectedRain, rain.Values2D[0]);

            expectedFlows = new double[] { 0.6 * 13.8 + 0.4 * 13.9, 0.6 * 18.2 + 0.4 * 18, 0.6 * 23.5 + 0.4 * 23.6 };
            expectedLevels = new double[] { 0.6 * 3.8 + 0.4 * 3.9, 0.6 * 8.2 + 0.4 * 8, 0.6 * 10.5 + 0.4 * 10.6 };
            expectedRain = new double[] { 0.6 * 13.8 + 0.4 * 13.9, 0.6 * 18.2 + 0.4 * 18, 0.6 * 23.5 + 0.4 * 23.6, 0.6 * 23.5 + 0.4 * 23.6 };

            _queryItem1.TimeSet.SetSingleTimeStamp(midTimeMjd);
            flows = flowOutput.GetValues(_queryItem1);
            levels = levelOutput.GetValues(_queryItem1);
            rain = rainOutput.GetValues(_queryItem1);
            AssertAreEqual(expectedFlows, (IList<double>)flows.Values2D[0], 1e-8);
            AssertAreEqual(expectedLevels, (IList<double>)levels.Values2D[0], 1e-8);
            AssertAreEqual(expectedRain, (IList<double>)rain.Values2D[0], 1e-8);

            expectedFlows = new double[] { 11, 15.6, 21.3 };
            expectedLevels = new double[] { 1, 5.6, 8.3 };
            expectedRain = new double[] { 11, 15.6, 21.3, 21.3 };

            _queryItem1.TimeSet.SetSingleTimeStamp(endTimeMjd);
            flows = flowOutput.GetValues(_queryItem1);
            levels = levelOutput.GetValues(_queryItem1);
            rain = rainOutput.GetValues(_queryItem1);
            Assert.AreEqual(expectedFlows, flows.Values2D[0]);
            Assert.AreEqual(expectedLevels, levels.Values2D[0]);
            Assert.AreEqual(expectedRain, rain.Values2D[0]);

            Console.Out.WriteLine("done!");

        }

        [Test]
		[Ignore("Can not currently extrapolate")]
		public void GetValuesExtrapolateTest()
        {

            string dataFileRoot = @"..\..\..\..\..\Examples\AsciiFileReader\Data\";

            AsciiFileDataComponent ascii = new AsciiFileDataComponent();
            IArgument[] arguments = new IArgument[2];
            arguments[0] = Argument.Create("File1", dataFileRoot + "FlowDataId.txt", false, "Name of first file to load");
            arguments[1] = Argument.Create("File2", dataFileRoot + "WaterlevelDataPoints.txt", false, "Name of second file to load");
            ascii.ApplyArguments(arguments);
            ascii.Initialize();

            Assert.AreEqual(2, ascii.Outputs.Count);

            ITimeSpaceOutput flowOutput = (ITimeSpaceOutput)ascii.Outputs[0];
            ITimeSpaceOutput levelOutput = (ITimeSpaceOutput)ascii.Outputs[1];

            Time startTime = new Time(new DateTime(2005, 1, 1, 0, 0, 0));
            double startTimeMjd = startTime.StampAsModifiedJulianDay;
            double afterEndTimeMjd = startTimeMjd + 4;


            double[] expectedFlows = new double[] {11, 15.6, 21.3};
            double[] expectedLevels = new double[] {1, 5.6, 8.3};


            // TODO: Make it extrapolate instead!
            _queryItem1.TimeSet.SetSingleTimeStamp(afterEndTimeMjd);
            ITimeSpaceValueSet flows = flowOutput.GetValues(_queryItem1);
            ITimeSpaceValueSet levels = levelOutput.GetValues(_queryItem1);
            Assert.AreEqual(expectedFlows, flows.Values2D[0]);
            Assert.AreEqual(expectedLevels, levels.Values2D[0]);


        }


        public static void AssertAreEqual(IList<double> a, IList<double> b)
        {
            AssertAreEqual(a, b, 0);
        }

        public static void AssertAreEqual(IList<double> a, IList<double> b, double delta)
        {
            Assert.AreEqual(a.Count, b.Count,
                            String.Format("Expected and actual have different sizes"));
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i], b[i], delta,
                                String.Format("Expected and actual are both <System.Double[{0}]>\n  Values differ at index [{1}]",
                                              a.Count, i));
            }

        }
    }
}
