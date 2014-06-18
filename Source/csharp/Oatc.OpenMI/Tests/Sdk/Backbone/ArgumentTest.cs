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

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
  [TestFixture]
  public class ArgumentTest
  {
    [Test]
    public void Constructor()
    {
      IArgument param = new ArgumentString("key", "value");
      Assert.AreEqual("key", param.Caption);
      Assert.AreEqual("value", param.Value);

      IArgument param2 = new ArgumentString(param);
      Assert.AreEqual(param, param2);
    }

    [Test]
    public void Key()
    {
      IArgument param = new ArgumentString("OperationKey");
      Assert.AreEqual("OperationKey", param.Caption);
    }

    [Test]
    public void Value()
    {
      IArgument param = new ArgumentString("id1", "OperationValue");
      Assert.AreEqual("OperationValue", param.Value);
    }

    [Test]
    public void ReadOnly()
    {
      ArgumentString param = new ArgumentString("Fred");
      param.IsReadOnly = true;
      Assert.AreEqual(true, param.IsReadOnly);
      param.IsReadOnly = false;
      Assert.AreEqual(false, param.IsReadOnly);
    }

    [Test]
    public void Description()
    {
      IArgument param = new ArgumentString("Fred");
      param.Description = "Description";
      Assert.AreEqual("Description", param.Description);
    }

    [Test]
    public void Equals()
    {
      IArgument param1 = new ArgumentString("key", "value");
      IArgument param2 = new ArgumentString("key", "value");

      Assert.IsTrue(param1.Equals(param2));
      param1.Caption = "key1";
      Assert.IsFalse(param1.Equals(param2));
      param1.Caption = "key";
      param1.Value = "value1";
      Assert.IsFalse(param1.Equals(param2));

      Assert.IsFalse(param1.Equals(null));
      Assert.IsFalse(param1.Equals("string"));
    }

  }
}
