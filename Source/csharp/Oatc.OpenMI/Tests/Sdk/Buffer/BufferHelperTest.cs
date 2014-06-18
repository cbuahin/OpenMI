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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Buffer.UnitTest
{
    /// <summary>
    /// Summary description for SupportTest.
    /// </summary>
    [TestFixture]
    public class BufferHelperTest
    {
        [Test]
        public void IsBefore()
        {
            ITime t4 = new Time(4);
            ITime t7 = new Time(7);
            Assert.AreEqual(true, BufferHelper.IsBefore(t4, t7));
            Assert.AreEqual(false, BufferHelper.IsBefore(t7, t4));
            Assert.AreEqual(false, BufferHelper.IsBefore(t4, t4));

            ITime t35 = new Time(3, 2);
            ITime t46 = new Time(4, 2);
            ITime t57 = new Time(5, 2);
            ITime t68 = new Time(6, 2);

            Assert.AreEqual(true, BufferHelper.IsBefore(t35, t68));
            Assert.AreEqual(false, BufferHelper.IsBefore(t68, t35));
            Assert.AreEqual(false, BufferHelper.IsBefore(t35, t46));
            Assert.AreEqual(false, BufferHelper.IsBefore(t35, t57));
            Assert.AreEqual(false, BufferHelper.IsBefore(t35, t35));

            Assert.AreEqual(true, BufferHelper.IsBefore(t4, t57));
            Assert.AreEqual(false, BufferHelper.IsBefore(t4, t35));
            Assert.AreEqual(false, BufferHelper.IsBefore(t7, t35));
            Assert.AreEqual(false, BufferHelper.IsBefore(t4, t46));
            Assert.AreEqual(false, BufferHelper.IsBefore(t7, t57));

            Assert.AreEqual(true, BufferHelper.IsBefore(t35, t7));
            Assert.AreEqual(false, BufferHelper.IsBefore(t35, t4));
            Assert.AreEqual(false, BufferHelper.IsBefore(t68, t4));
            Assert.AreEqual(false, BufferHelper.IsBefore(t57, t7));
        }
    }
}