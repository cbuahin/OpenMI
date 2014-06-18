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

using NUnit.Framework;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class QuantityTest
	{
		Quantity quantity;
		[SetUp]
		public void Init()
		{
			quantity = new Quantity(new Unit("UnitID",1.0,0.0,"Description"),"Description","Caption");
		}

		[Test]
		public void Constructor()
		{
			Quantity quantity2 = new Quantity(quantity);
			Assert.AreEqual(quantity,quantity2);
		}

		[Test]
		public void Caption()
		{
			Assert.AreEqual("Caption",quantity.Caption);
			quantity.Caption = "new";
			Assert.AreEqual("new",quantity.Caption);
		}

		[Test]
		public void Description()
		{
			Assert.AreEqual("Description",quantity.Description);
			quantity.Description = "new";
			Assert.AreEqual("new",quantity.Description);
		}

		[Test]
		public void Unit()
		{
			Assert.AreEqual(new Unit("UnitID",1.0,0.0,"Description"),quantity.Unit);
			quantity.Unit = new Unit("UnitID2",1.0,0.0,"Description");
			Assert.AreEqual(new Unit("UnitID2",1.0,0.0,"Description"),quantity.Unit);
		}

		[Test]
		public void Equals()
		{
			Quantity quantity2 = new Quantity(new Unit("UnitID",1.0,0.0,"Description"),"Description","Caption");

			Assert.IsTrue(quantity.Equals(quantity2));

			Assert.IsFalse(quantity.Equals(null));
			Assert.IsFalse(quantity.Equals("string"));

		}

		[Test]
		public void EqualsUnit()
		{
			Quantity quantity2 = new Quantity(new Unit("UnitID2",1.0,0.0,"Description"),"Description","Caption");

			Assert.IsFalse(quantity.Equals(quantity2));
		}

		[Test]
		public void EqualsDescription()
		{
			Quantity quantity2 = new Quantity(new Unit("UnitID",1.0,0.0,"Description"),"Description2","Caption");

			Assert.IsFalse(quantity.Equals(quantity2));
		}

		[Test]
		public void EqualsID()
		{
			Quantity quantity2 = new Quantity(new Unit("UnitID",1.0,0.0,"Description"),"Description","ID2");

			Assert.IsFalse(quantity.Equals(quantity2));
		}
	}
}
