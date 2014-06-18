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
using org.OpenMI.Standard;
using org.OpenMI.Backbone;
using org.OpenMI.Utilities.AdvancedControl;

namespace org.OpenMI.Utilities.AdvancedControl.UnitTest
{
	[TestFixture]
	public class ParameterDescriptorTest
	{
		ParameterDescriptor descriptor;

		[SetUp]
		public void Init()
		{
			descriptor = new ParameterDescriptor("parameter",0.5,3.5,0.75);
		}

		[Test]
		public void ID()
		{
			Assert.AreEqual("parameter",descriptor.ID);
			descriptor.ID = "changed";
			Assert.AreEqual("changed",descriptor.ID);
		}

		[Test]
		public void Minimum()
		{
			Assert.AreEqual(0.5,descriptor.Minimum);
			descriptor.Minimum = 1.2;
			Assert.AreEqual(1.2,descriptor.Minimum);
		}

		[Test]
		public void Maximum()
		{
			Assert.AreEqual(3.5,descriptor.Maximum);
			descriptor.Maximum = 5.2;
			Assert.AreEqual(5.2,descriptor.Maximum);
		}

		[Test]
		public void CurrentValue()
		{
			Assert.AreEqual(0.75,descriptor.CurrentValue);
			descriptor.CurrentValue = 1.37;
			Assert.AreEqual(1.37,descriptor.CurrentValue);
		}
	}
}
