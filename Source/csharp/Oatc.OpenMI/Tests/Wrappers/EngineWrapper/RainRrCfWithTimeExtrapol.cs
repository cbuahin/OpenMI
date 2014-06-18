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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    [TestFixture]
    public class RainRrCfWithTimeExtrapol
    {
        private Quantity _dischargeQuantity;
        private Quantity _waterlevelQuantity;
        private ElementSet _idBasedElementSetA;
        private Input _dischargeQueryItem;
        private Input _waterlevQueryItem;

        [SetUp]
        public void CreateQueryExchangeItems()
        {
            _dischargeQuantity = new Quantity(new Unit(PredefinedUnits.CubicMeterPerSecond), null, "Discharge");
            _waterlevelQuantity = new Quantity(new Unit(PredefinedUnits.Meter), null, "Water Level");

            _idBasedElementSetA = new ElementSet(null, "ElmSet-A", ElementType.IdBased);
            _idBasedElementSetA.AddElement(new Element("elm-1"));

            _dischargeQueryItem = new Input("discharge, to be retrieved from some output item", _dischargeQuantity,
                                            _idBasedElementSetA);
            _dischargeQueryItem.TimeSet = new TimeSet();

            _waterlevQueryItem = new Input("water level, to be retrieved from some output item", _waterlevelQuantity,
                                           _idBasedElementSetA);
            _waterlevQueryItem.TimeSet = new TimeSet();
        }

        [Test]
        public void GetValuesOnCfOutputItem()
        {
            // create the component
            // connect query and output item
            // take care that component becomes valid and produces initial output for connected items
            ITimeSpaceComponent cf = RainRrCfComponents.CreateChannelFlowInstance("CF-2");
            ITimeSpaceOutput selectedOutput = RainRrCfCompositions.FindOutputItem(cf, "node-4.discharge");
            selectedOutput.AddConsumer(_dischargeQueryItem);
            cf.Validate();

            // check initial values
            Assert.AreEqual(7000.0, (double) selectedOutput.Values.Values2D[0][0], "Value[0] as property");

            // set query time for getting values
            _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 28, 12, 0, 0));
            ITimeSpaceValueSet values = selectedOutput.GetValues(_dischargeQueryItem);
            Assert.IsNotNull(values, "values != null");
            Assert.AreEqual(7001.5, (double) values.Values2D[0][0], "value[0] from GetValues 1");

            _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 29, 0, 0, 0));
            values = selectedOutput.GetValues(_dischargeQueryItem);
            Assert.IsNotNull(values, "values != null");
            Assert.AreEqual(7002.0, (double) values.Values2D[0][0], "value[0] from GetValues 1");

            try
            {
              _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 30, 0, 0, 0));
                selectedOutput.GetValues(_dischargeQueryItem);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.StartsWith("Could not update engine \""));
            }
        }


        [Test]
        public void GetValuesOnTimeExtrapolator()
        {
            // create the component
            // connect query and output item
            // take care that component becomes valid and produces initial output for connected items
            ITimeSpaceComponent cf = RainRrCfComponents.CreateChannelFlowInstance("CF-2");
            ITimeSpaceOutput selectedOutput = RainRrCfCompositions.FindOutputItem(cf, "node-2.waterlevel");
            cf.Validate();

            ITimeSpaceAdaptedOutput timeExtrapolator =
                (ITimeSpaceAdaptedOutput)RainRrCfCompositions.ConnectItemsUsingAdaptedOutput(cf,
                                                                    selectedOutput, _waterlevQueryItem,
                                                                    RainRrCfCompositions.timeExtrapolatorId);

            // set query time for getting values
            _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 28, 12, 0, 0));
            ITimeSpaceValueSet values = timeExtrapolator.GetValues(_dischargeQueryItem);
            Assert.IsNotNull(values, "values != null");
            Assert.AreEqual(4001.5, (double) values.Values2D[0][0], "value[0] from GetValues 1");

            _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 29, 0, 0, 0));
            values = timeExtrapolator.GetValues(_dischargeQueryItem);
            Assert.IsNotNull(values, "values != null");
            Assert.AreEqual(4002.0, (double) values.Values2D[0][0], "value[0] from GetValues 1");

            _dischargeQueryItem.TimeSet.SetSingleTimeStamp(new DateTime(2009, 3, 30, 0, 0, 0));
            timeExtrapolator.GetValues(_dischargeQueryItem);
            Assert.AreEqual(4002.0, (double) values.Values2D[0][0], "value[0] from GetValues 1");
        }
    }
}