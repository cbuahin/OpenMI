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
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class TimeTest
	{
		Time time;

		[SetUp]
		public void Init()
		{	
			time = new Time(new Time(1.0),new Time(2.0));
		}

		[Test]
		public void Constructor ()
		{
			Time time2 = new Time(time);
			Assert.AreEqual(time, time2);
		}

		[Test]
		public void Start()
		{
			Assert.AreEqual(1.0, time.StampAsModifiedJulianDay);
		}

		[Test]
		public void Equals()
		{
		    Time time2 = new Time(new Time(1.0), new Time(2.0));

            Assert.AreEqual(time, time2);
            Assert.AreEqual(time.GetHashCode(), time2.GetHashCode());

            Assert.IsFalse(time.Equals(null));
			Assert.IsFalse(time.Equals("string"));
		}

		[Test]
		public void EqualsStart()
		{
			Assert.IsFalse(time.Equals(new Time(new Time(1.1), new Time(2.0))));
		}

		[Test]
		public void EqualsEnd()
		{
			Assert.IsFalse(time.Equals(new Time(new Time(1.0), new Time(2.1))));
		}
	}
}
