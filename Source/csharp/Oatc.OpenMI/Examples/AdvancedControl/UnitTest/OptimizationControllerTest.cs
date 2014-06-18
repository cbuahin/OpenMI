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
using org.OpenMI.Standard;
using org.OpenMI.Backbone;
using org.OpenMI.Utilities.AdvancedControl;

namespace org.OpenMI.Utilities.AdvancedControl.UnitTest
{
	[TestFixture]
	public class OptimizationControllerTest
	{
		OptimizationController controller;
		ILinkableComponent modelA;
		ILinkableComponent deployer;

		Link link1;
		Link link2;
		Link link3;

		[SetUp]
		public void Init()
		{
			controller = new OptimizationController(1);
			IArgument[] properties = new Argument[1];
			properties[0] = new Argument("Parameter","P0,-100,100,3.5",true,"Parameter 1");
			controller.Initialize(properties);
			modelA = new TestFunction();
			deployer = new TestComponent();
			ElementSet slot1 = new ElementSet("P0","P0",ElementType.IDBased,null);
			link1 = new Link(controller,slot1,null,deployer,slot1,null,"link1");
			deployer.AddLink(link1);
			controller.AddLink(link1);

			link2 = new Link(modelA,slot1,null,controller,slot1,null,"link2");
			controller.AddLink(link2);
			modelA.AddLink(link2);

			link3 = new Link(controller,slot1,null,modelA,slot1,null,"link3");
			controller.AddLink(link3);
			modelA.AddLink(link3);
		}

		[Test]
		public void OptimizationController()
		{
			IScalarSet results = (IScalarSet) controller.GetValues(
				new org.OpenMI.Backbone.TimeStamp(0.0),"link1");
			Assert.AreEqual(0.49989928036496,results.GetScalar(0),0.000000001);
		}

	}


	public class TestFunction:LinkableComponent
	{
		public override void Prepare()
		{
		}

		public override string ComponentDescription
		{
			get
			{
				return "TestComponent";
			}
		}

		public override string Validate()
		{
			return "";
		}

		public override ITimeSpan TimeHorizon
		{
			get
			{
				return new org.OpenMI.Backbone.TimeSpan(new TimeStamp(0.0),new TimeStamp(1.0));
			}
		}

		public override void Finish()
		{
		}

		public override void Dispose()
		{
		}

		public override IValueSet GetValues(ITime time, string linkID)
		{
			ILink link = GetAcceptingLinks()[0];
			double x = ((IScalarSet) link.SourceComponent.GetValues(
				new org.OpenMI.Backbone.TimeStamp(0.0),link.ID)).GetScalar(0);
			return new ScalarSet(new double[] {(x-0.5)*(x-0.5)});
		}

		public override ITimeStamp EarliestInputTime
		{
			get
			{
				return new TimeStamp(0.0);
			}
		}

		public override string ComponentID
		{
			get
			{
				return "Test Component";
			}
		}

		public override void Initialize(IArgument[] properties)
		{
		}

		public override string ModelDescription
		{
			get
			{
		
				return "Test Component";
			}
		}

		public override string ModelID
		{
			get
			{

				return "Test Component";
			}
		}

		public override org.OpenMI.Standard.EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			return new org.OpenMI.Standard.EventType ();
		}

		public override int GetPublishedEventTypeCount()
		{
			return 0;
		}

	}
}
