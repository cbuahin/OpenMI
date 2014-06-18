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
	public class IterationControllerTest
	{
		IterationController controller;
		ILinkableComponent modelA;
		ILinkableComponent modelB;
		ILinkableComponent deployer;

		Link link1;
		Link link2;
		Link link3;
		Link link4;
		Link link5;
		Link link6;

		[SetUp]
		public void Init()
		{
			controller = new IterationController();
			Quantity Q = new Quantity("Q");
			Quantity H = new Quantity("H");
			modelA = new TestComponent();
			modelB = new TestComponent();
			deployer = new TestComponent();
			ElementSet point = new ElementSet("point","point",ElementType.IDBased,
				new SpatialReference());
			ElementSet slot1 = new ElementSet("1","1",ElementType.IDBased,new SpatialReference());
			ElementSet slot2 = new ElementSet("2","2",ElementType.IDBased,new SpatialReference());
			link1 = new Link(controller,slot1,Q,modelA,point,Q,"link1");
			controller.AddLink(link1);
			modelA.AddLink(link1);
			link2 = new Link(modelA,point,H,controller,slot2,H,"link2");
			controller.AddLink(link2);
			modelA.AddLink(link2);
			link3 = new Link(controller,slot2,H,modelB,point,H,"link3");
			controller.AddLink(link3);
			modelB.AddLink(link3);
			link4 = new Link(modelB,point,Q,controller,slot1,Q,"link4");
			controller.AddLink(link4);
			modelB.AddLink(link4);
			link5 = new Link(controller,slot1,Q,deployer,point,Q,"link5");
			deployer.AddLink(link5);
			controller.AddLink(link5);
			link6 = new Link(controller,slot2,H,deployer,point,H,"link6");
			deployer.AddLink(link6);
			controller.AddLink(link6);
		}

		[Test]
		public void ComponentDescription()
		{
			Assert.AreEqual("Iteration Controller",controller.ComponentDescription);
		}

		[Test]
		public void ComponentID()
		{
			Assert.AreEqual("Iteration Controller",controller.ComponentID);
		}

		[Test]
		public void EarliestInputTime()
		{
			Assert.AreEqual(new TimeStamp(0.0),controller.EarliestInputTime);
		}

		[Test]
		public void ModelDescription()
		{
			Assert.AreEqual("Iteration Controller",controller.ModelDescription);
		}

		[Test]
		public void ModelID()
		{
			Assert.AreEqual("Iteration Controller",controller.ModelID);
		}

		[Test]
		public void IterationController()
		{
			IScalarSet resultQ = (IScalarSet) controller.GetValues(new TimeStamp(0.1),"link5");
			IScalarSet resultH = (IScalarSet) controller.GetValues(new TimeStamp(0.1),"link6");

			Assert.AreEqual(0.5,resultQ.GetScalar(0));
			Assert.AreEqual(0.5,resultH.GetScalar(0));
		}

	}


	public class TestComponent:LinkableComponent
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
			return new ScalarSet(new double[] {0.5});
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
