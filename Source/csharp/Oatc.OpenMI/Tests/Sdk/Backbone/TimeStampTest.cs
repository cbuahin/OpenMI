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

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
	[TestFixture]
	public class TimeStampTest
	{
		Time time;
		[SetUp]
		public void Init()
		{
			time = new Time(12345.3);
		}

		[Test]
		public void Constructor()
		{
			Time timeStamp2 = new Time(time);
			Assert.AreEqual(time,timeStamp2);

            Time timeStamp3 = new Time(new DateTime(1858, 11, 17)); // the zero Julian time
            Assert.AreEqual(0.0, timeStamp3.StampAsModifiedJulianDay);
		}
		[Test]
		public void ModifiedJulianDay()
		{
			Assert.AreEqual(12345.3,time.StampAsModifiedJulianDay);
			time.StampAsModifiedJulianDay = 54321.7;
			Assert.AreEqual(54321.7,time.StampAsModifiedJulianDay);
		}
		[Test]
		public void Equals()
		{
			Time timeStamp1 = new Time(12345.3);
			Assert.IsTrue(time.Equals(timeStamp1));
            Assert.AreEqual(time.GetHashCode(), timeStamp1.GetHashCode());

			timeStamp1.StampAsModifiedJulianDay = 34.0;
			Assert.IsFalse(time.Equals(timeStamp1));
            Assert.AreNotEqual(time.GetHashCode(), timeStamp1.GetHashCode());

			Assert.IsFalse(time.Equals(null));
			Assert.IsFalse(time.Equals("string"));
            Assert.AreNotEqual(time.GetHashCode(), timeStamp1.GetHashCode());
        }

		[Test]
		public void CompareTo()
		{
			Time timeStamp1 = new Time(12345.3);
			Assert.AreEqual(0.0,time.CompareTo(timeStamp1));
			timeStamp1.StampAsModifiedJulianDay = 10000;
			Assert.IsTrue(time.CompareTo(timeStamp1)>0.0);
			Assert.IsTrue(timeStamp1.CompareTo(time)<0.0);
		}

		[Test]
		public void ToString()
		{
			Assert.AreEqual("1892-09-04 07:12:00", time.ToString());
		}

        [Test]
        public void ToDateTime()
        {
            DateTime testDate = new DateTime(2008, 12, 31);
            Time time1 = new Time(testDate);
            Assert.AreEqual(testDate, time1.ToDateTime());
        }
	}
}
