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
  public class ExchangeItemTest
  {
    Output output;
    [SetUp]
    public void Init()
    {
      output = new Output("ES.Q", new Quantity("Q"), new ElementSet("ES"));
    }

    [Test]
    public void ElementSet()
    {
      ElementSet elementSet = new ElementSet("ES");
      Assert.IsTrue(elementSet.Equals(output.SpatialDefinition));
    }

    [Test]
    public void Quantity()
    {
      Assert.IsTrue(output.ValueDefinition.Equals(new Quantity("Q")));
    }

    [Test]
    public void Equals()
    {
      ElementSet elementSet = new ElementSet("ES");
      Output exchangeItem2 = new Output("ES.Q", new Quantity("Q"), elementSet);

      Assert.IsTrue(exchangeItem2.Equals(output));

      exchangeItem2.ValueDefinition = new Quantity("Q1");
      Assert.IsFalse(exchangeItem2.Equals(output));
      exchangeItem2.ValueDefinition = new Quantity("Q");
      elementSet.Caption = "ES2";

      Assert.IsFalse(exchangeItem2.Equals(output));
      Assert.IsFalse(exchangeItem2.Equals(null));
      Assert.IsFalse(exchangeItem2.Equals("string"));
    }
  }
}
