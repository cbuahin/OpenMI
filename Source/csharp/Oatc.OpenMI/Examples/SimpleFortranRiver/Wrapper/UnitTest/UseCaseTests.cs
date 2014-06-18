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
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest 
// purpose: Unit-testing the package Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper
// file: UseCaseTests.cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
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
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections;
using OpenMI.Standard2;
using NUnit.Framework;

#if ADH // ADH: This test needs rewriting

namespace Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest
{
	[TestFixture]
	public class UseCaseTests
	{

		[Test]
		public void UseCase01()
		{
			// This is the test for use case one descripbed in the OpenMI Guidelines for model migration
			// In this test a test model will provide inflow to the top node and to the node downstream
			// to the top node. The trigger will get flow from the most downstream branch.
			try
			{
				ILinkableComponent simpleRiver;
				simpleRiver = new Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.SimpleRiverOpenMIComponent();
				Argument[] arguments = new Argument[1];
                arguments[0] = new Argument("FilePath", @"..\..\", true, "description");
     
				ILinkableComponent testRiver = new TestRiverLC();
				Oatc.OpenMI.Examples.TriggerComponents.SimpleTrigger.Trigger trigger = new Oatc.OpenMI.Examples.TriggerComponents.SimpleTrigger.Trigger();

				simpleRiver.Initialize(arguments);
				testRiver.Initialize(new Argument[0]);
				trigger.Initialize(new Argument[0]);

				Link testRiverToSimpleRiverLinkTopNode = 
					new Link(testRiver,                                       //source LinkableComponent
					testRiver.GetOutputExchangeItem(0).ElementSet,            //source ElementSet
					testRiver.GetOutputExchangeItem(0).Quantity,              //source Quantity
					simpleRiver,                                              //Target LinkableComponent
					simpleRiver.GetInputExchangeItem(3).ElementSet,           //target ElementSet
					simpleRiver.GetInputExchangeItem(3).Quantity,             //target Quantity
                    "testRiverToSimpleRiverLink");                            //linkID

				Link testRiverToSimpleRiverLinkNode1 = 
					new Link(testRiver,                                       //source LinkableComponent
					testRiver.GetOutputExchangeItem(0).ElementSet,            //source ElementSet
					testRiver.GetOutputExchangeItem(0).Quantity,              //source Quantity
					simpleRiver,                                              //Target LinkableComponent
					simpleRiver.GetInputExchangeItem(4).ElementSet,           //target ElementSet
					simpleRiver.GetInputExchangeItem(4).Quantity,             //target Quantity
					"testRiverToSimpleRiverLink");                            //linkID

				Link simpleRiverToTriggerLink =
					new Link(simpleRiver,
					simpleRiver.GetOutputExchangeItem(2).ElementSet,
					simpleRiver.GetOutputExchangeItem(2).Quantity,trigger,
					simpleRiver.GetOutputExchangeItem(2).ElementSet,
					simpleRiver.GetOutputExchangeItem(2).Quantity,
					"ID2");

				testRiver.AddLink(testRiverToSimpleRiverLinkTopNode);
				simpleRiver.AddLink(testRiverToSimpleRiverLinkTopNode);

				testRiver.AddLink(testRiverToSimpleRiverLinkNode1);
				simpleRiver.AddLink(testRiverToSimpleRiverLinkNode1);

				simpleRiver.AddLink(simpleRiverToTriggerLink);
				trigger.AddLink(simpleRiverToTriggerLink);

				IListener eventListener = new Oatc.OpenMI.Examples.EventListeners.SimpleEventListener.EventListener();
				for (int i = 0; i < eventListener.GetAcceptedEventTypeCount(); i++)
				{
					for (int n = 0; n < simpleRiver.GetPublishedEventTypeCount(); n++)
					{
						if (eventListener.GetAcceptedEventType(i) == simpleRiver.GetPublishedEventType(n))
						{
							simpleRiver.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
						}
					}
					for (int n = 0; n < testRiver.GetPublishedEventTypeCount(); n++)
					{
						if (eventListener.GetAcceptedEventType(i) == testRiver.GetPublishedEventType(n))
						{
							testRiver.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
						}
					}
				}

				simpleRiver.Prepare();
				testRiver.Prepare();
				trigger.Prepare();

				double startTime = simpleRiver.TimeHorizon.Start.ModifiedJulianDay;

				Oatc.OpenMI.Sdk.Backbone.TimeStamp[] times = new Oatc.OpenMI.Sdk.Backbone.TimeStamp[5];
				times[0] = new TimeStamp(startTime + 1);
				times[1] = new TimeStamp(startTime + 2);
				times[2] = new TimeStamp(startTime + 3);
				times[3] = new TimeStamp(startTime + 4);
				times[4] = new TimeStamp(startTime + 5);
				
				trigger.Run(times);

				Assert.AreEqual(3.8,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(0)).data[0]);
				Assert.AreEqual(7.6,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(1)).data[0]); //ts1
				Assert.AreEqual(9.1,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(2)).data[0]);
				Assert.AreEqual(10.6,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(3)).data[0]);//ts2
				Assert.AreEqual(12.1,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(4)).data[0]);

				simpleRiver.Finish();
				testRiver.Finish();
				trigger.Finish();

			}
			catch (System.Exception e)
			{
				Oatc.OpenMI.Examples.ExeptionHandlers.SimpleExceptionHandler.ExceptionHandler.WriteException(e);
				throw (e);
			}
		}
		[Test]
		[Ignore("Pending implementation of mapping method in the ElementMapper")]
		public void UseCase02()
		{
			try
			{
				ILinkableComponent simpleRiver;
				simpleRiver = new Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.SimpleRiverOpenMIComponent();
				Argument[] arguments = new Argument[1];
				arguments[0] = new Argument("FilePath",@"..\..\",true,"description");
     
				ILinkableComponent runoffLC = new RunoffDataLC();
				Oatc.OpenMI.Examples.TriggerComponents.SimpleTrigger.Trigger trigger = new Oatc.OpenMI.Examples.TriggerComponents.SimpleTrigger.Trigger();

				simpleRiver.Initialize(arguments);
				runoffLC.Initialize(new Argument[0]);
				trigger.Initialize(new Argument[0]);

				// --- DataOperations -----
				int dataOperationCount = runoffLC.GetOutputExchangeItem(0).DataOperationCount;
				int dataoperationIndex = -999;
				for (int i = 0; i < dataOperationCount; i++)
				{
					for (int n = 0; n < runoffLC.GetOutputExchangeItem(0).GetDataOperation(i).ArgumentCount; n++)
					{
						if (runoffLC.GetOutputExchangeItem(0).GetDataOperation(i).GetArgument(n).Value == "Weighted Mean")
						{
							dataoperationIndex = i;
						}
					}
				}
				IDataOperation dataOperation = runoffLC.GetOutputExchangeItem(0).GetDataOperation(dataoperationIndex);

				ArrayList dataOperations = new ArrayList();
				dataOperations.Add(dataOperation);

				//-- Create links --
				Link runoffLCToSimpleRiverLink = 
					new Link(runoffLC,                                        //source LinkableComponent
					runoffLC.GetOutputExchangeItem(0).ElementSet,             //source ElementSet
					runoffLC.GetOutputExchangeItem(0).Quantity,               //source Quantity
					simpleRiver,                                              //Target LinkableComponent
					simpleRiver.GetInputExchangeItem(7).ElementSet,           //target ElementSet
					simpleRiver.GetInputExchangeItem(7).Quantity,             //target Quantity
					"runoffLCToSimpleRiverLink",                              //linkID
					"runoffLCToSimpleRiverLink Description",                  //link description
					dataOperations);										  //dataOperations


				Link simpleRiverToTriggerLink =
					new Link(simpleRiver,
					simpleRiver.GetOutputExchangeItem(2).ElementSet,
					simpleRiver.GetOutputExchangeItem(2).Quantity,trigger,
					simpleRiver.GetOutputExchangeItem(2).ElementSet,
					simpleRiver.GetOutputExchangeItem(2).Quantity,
					"ID2");

				runoffLC.AddLink(runoffLCToSimpleRiverLink);
				simpleRiver.AddLink(runoffLCToSimpleRiverLink);

				simpleRiver.AddLink(simpleRiverToTriggerLink);
				trigger.AddLink(simpleRiverToTriggerLink);

				IListener eventListener = new Oatc.OpenMI.Examples.EventListeners.SimpleEventListener.EventListener();
				for (int i = 0; i < eventListener.GetAcceptedEventTypeCount(); i++)
				{
					for (int n = 0; n < simpleRiver.GetPublishedEventTypeCount(); n++)
					{
						if (eventListener.GetAcceptedEventType(i) == simpleRiver.GetPublishedEventType(n))
						{
							simpleRiver.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
						}
					}
					for (int n = 0; n < runoffLC.GetPublishedEventTypeCount(); n++)
					{
						if (eventListener.GetAcceptedEventType(i) == runoffLC.GetPublishedEventType(n))
						{
							runoffLC.Subscribe(eventListener, eventListener.GetAcceptedEventType(i));
						}
					}
				}

				simpleRiver.Prepare();
				runoffLC.Prepare();
				trigger.Prepare();

				double startTime = simpleRiver.TimeHorizon.Start.ModifiedJulianDay;

				Oatc.OpenMI.Sdk.Backbone.TimeStamp[] times = new Oatc.OpenMI.Sdk.Backbone.TimeStamp[5];
				times[0] = new TimeStamp(startTime + 1);
				times[1] = new TimeStamp(startTime + 2);
				times[2] = new TimeStamp(startTime + 3);
				times[3] = new TimeStamp(startTime + 4);
				times[4] = new TimeStamp(startTime + 5);
				
				trigger.Run(times);

//				Assert.AreEqual(3.8,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(0)).data[0]);
				Assert.AreEqual(5.6 + 3 + 4.0/3.0,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(1)).data[0]); //ts1
//				Assert.AreEqual(9.1,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(2)).data[0]);
				Assert.AreEqual(8.6 + 3 + 4.0/3.0,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(3)).data[0]);//ts2
//				Assert.AreEqual(12.1,((ScalarSet) trigger.ResultsBuffer.GetValuesAt(4)).data[0]);

				simpleRiver.Finish();
				runoffLC.Finish();
				trigger.Finish();

			}
			catch (System.Exception e)
			{
				Oatc.OpenMI.Examples.ExeptionHandlers.SimpleExceptionHandler.ExceptionHandler.WriteException(e);
				throw (e);
			}

		}
	}
}

#endif // ADH