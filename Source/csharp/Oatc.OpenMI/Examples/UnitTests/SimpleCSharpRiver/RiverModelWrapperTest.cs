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
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.SimpleCSharpRiver;

namespace Oatc.OpenMI.Examples.ModelComponents.SimpleCSharpRiver.Wrapper.UnitTest
{
    /// <summary>
    /// Summary description for MyName.
    /// </summary>
    [TestFixture]
    public class RiverModelWrapperTest
    {

        ILinkableComponent riverModelLC;
        List<LinkableComponentStatusChangeEventArgs> events;
        /// <summary>
        /// Perform generic test initialization. Because this method is
        /// marked with the <code>SetUp</code> attribute, it is called
        /// prior to the invocation of each test.
        /// </summary>
        [SetUp]
        public void Init()
        {
            events = new List<LinkableComponentStatusChangeEventArgs>();
            Argument[] arguments = { new Argument("Path", @"..\..\Data\", true, "description") };
            riverModelLC = new RiverModelLinkableComponent();
            riverModelLC.StatusChanged += riverModelLC_StatusChanged;
            riverModelLC.Initialize(arguments);
        }

        /// <summary>
        /// Perform generic post-test cleanup. Because this method is
        /// marked with the <code>tearDown</code> attribute, it is called
        /// following the completion of each test method.
        /// </summary>
        [TearDown]
        public void Dispose()
        {
            riverModelLC.Finish();       
        }





        [Test]
        public void Status()
        {
            Assert.AreEqual(LinkableComponentStatus.Initialized, riverModelLC.Status);
            riverModelLC.Update();
            Assert.AreEqual(LinkableComponentStatus.Updated, riverModelLC.Status);

        }

        [Test]
        public void ComponentIdAndDescription()
        {
            Assert.AreEqual("River model", riverModelLC.Id);
            Assert.AreEqual("River model", riverModelLC.Caption);
            Assert.AreEqual("A simple river model", riverModelLC.Description);
     
        }

        [Test]
        public void InputItems()
        {
            double inputFlow = 30.32;
            IList<double> newElementValues = new List<double> { inputFlow };
            IList<IList<double>> newValues = new List<IList<double>> {newElementValues};
            //Note (AH) : it does not seem right that you are force to set a whole list, why not make
            //it possible to set individual numbers in the list. In this case only the get values are needed
            //in both inputItems and outputItems. Only difference is how the models reacto to when values
            //are changed. However, the problem is in some cases a component does not have any values
            //to present for input items.

            riverModelLC.InputItems[1].Values = new ValueSet<double>(newValues);
            riverModelLC.Update();

            double calculatedFlow =  (double) riverModelLC.OutputItems[1].Values.Values2D[0][0];
            //Note: In this case the values list only contains one values because there is only one time in the TimeSet and
            //there is only one element in the element set.

            Assert.AreEqual((1.1 + 1.2 + 30.32) * 0.9, calculatedFlow); //0.9 because the leakage coefficeint is 0.9
        
        }

        [Test]
        public void OutputItems()
        {
            DateTime time = ((Time)riverModelLC.OutputItems[0].TimeSet.Times[0]).ToDateTime();
            DateTime expectedStart = new DateTime(2004, 10, 7, 16, 38, 32);
            Assert.AreEqual(expectedStart, time);
            
        
        }

        [Test]
        public void TimeHorizon()
        {
            ITime timeHorizon = riverModelLC.OutputItems[0].TimeSet.TimeHorizon;
            DateTime start = Time.ToDateTime(timeHorizon.StampAsModifiedJulianDay);
            DateTime end = Time.ToDateTime(timeHorizon.StampAsModifiedJulianDay + timeHorizon.DurationInDays);

            DateTime expectedStart = new DateTime(2004, 10, 7, 16,38, 32);
            DateTime expectedEnd = new DateTime(2004, 10, 13, 16, 38, 32);

            Assert.AreEqual(expectedStart, start);

            Assert.AreEqual(expectedEnd, end);


        }

        [Test]
        [Ignore]
        public void Initialize()
        {
            
        }

        [Test]
        [Ignore]
        public void Validate()
        {
            
        }

        [Test]

        public void Update()
        {
            DateTime time = ((Time)riverModelLC.OutputItems[2].TimeSet.Times[0]).ToDateTime();
            DateTime expectedStart = new DateTime(2004, 10, 7, 16, 38, 32);
            Assert.AreEqual(expectedStart, time);

            double flow = (double) riverModelLC.OutputItems[2].Values.Values2D[0][0];
            double expectedFlow = 0.0;

            Assert.AreEqual(expectedFlow, flow);

            riverModelLC.Update();


            time = ((Time)riverModelLC.OutputItems[2].TimeSet.Times[0]).ToDateTime();
            expectedStart = new DateTime(2004, 10, 8, 16, 38, 32);
            Assert.AreEqual(expectedStart, time);

            flow = (double)riverModelLC.OutputItems[2].Values.Values2D[0][0];
            expectedFlow = (1.1 + 1.2 + 1.3) * 0.9;

            Assert.AreEqual(expectedFlow, flow);

            while (riverModelLC.Status != LinkableComponentStatus.Done)
            {
                riverModelLC.Update();
            }

            flow = (double)riverModelLC.OutputItems[2].Values.Values2D[0][0];
            expectedFlow = (6.1 + 6.2 + 6.3) * 0.9;

            Assert.AreEqual(expectedFlow, flow);
            
        }

        [Test]
        [Ignore]public void Finish()
        {
            
        }

        [Test]
        [Ignore]
        public void CreateOutputDecorator()
        {
        
        }

        [Test]
        public void EventTest()
        {
            riverModelLC.Update();
            Assert.AreEqual(LinkableComponentStatus.Created, events[0].OldStatus);
            Assert.AreEqual(LinkableComponentStatus.Initializing, events[0].NewStatus);
            Assert.AreEqual(LinkableComponentStatus.Initializing, events[1].OldStatus);
            Assert.AreEqual(LinkableComponentStatus.Initialized, events[1].NewStatus);
            Assert.AreEqual(LinkableComponentStatus.Initialized, events[2].OldStatus);
            Assert.AreEqual(LinkableComponentStatus.Updating, events[2].NewStatus);
            Assert.AreEqual(LinkableComponentStatus.Updating, events[3].OldStatus);
            Assert.AreEqual(LinkableComponentStatus.Updated, events[3].NewStatus);

        }

        void riverModelLC_StatusChanged(object sender, LinkableComponentStatusChangeEventArgs e)
        {
            events.Add(e);
        }


 
    }
}