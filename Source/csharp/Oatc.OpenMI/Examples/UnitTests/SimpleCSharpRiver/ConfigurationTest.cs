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
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

using Oatc.PossibleSdk;

namespace Oatc.SimpleCSharpRiver.UnitTest
{
    [TestFixture]
    public class ConfigurationTest
    {
        Argument[] upperRiverArguments;
        Argument[] lowerRiverArguments;

        [SetUp]
        public void Init()
        {
            upperRiverArguments = new[] {new Argument("Path", @"..\..\Data\UpperRiver\", true, "description") };
            lowerRiverArguments = new[] { new Argument("Path", @"..\..\Data\LowerRiver\", true, "description") };
        }

        [Test]
        [Ignore]
        public void RiverToRiver()
        {
            Assert.IsTrue(false, "Text not running correctly, to be checked by Gena/Rob/Stef");

            ILinkableComponent upperRiver = new RiverModelLinkableComponent();
            ILinkableComponent lowerRiver = new RiverModelLinkableComponent();

            upperRiver.Initialize(upperRiverArguments);
            lowerRiver.Initialize(lowerRiverArguments);

            upperRiver.OutputItems[2].AddConsumer(lowerRiver.InputItems[0]);

            int numberOfUpdates;
            numberOfUpdates = 0;

            lowerRiver.Update();
            numberOfUpdates++;
            Assert.AreEqual(1.1 + 1.2 + 1.3, (double)upperRiver.OutputItems[2].Values.Values2D[0][0]);

            Assert.AreEqual(1.1 + 1.2 + 1.3 + 1.1, (double)lowerRiver.OutputItems[0].Values.Values2D[0][0]);

            lowerRiver.Update();
            numberOfUpdates++;
            Assert.AreEqual(2.1 + 2.2 + 2.3 + 2.1, (double)lowerRiver.OutputItems[0].Values.Values2D[0][0]);

            //now run til the end of simulation

            while (lowerRiver.Status != LinkableComponentStatus.Done)
            {
                lowerRiver.Update();
                numberOfUpdates++;
            }
            Assert.AreEqual(6.1 + 6.2 + 6.3 + 6.1, (double)lowerRiver.OutputItems[0].Values.Values2D[0][0]);
            Assert.AreEqual(4, numberOfUpdates);
        }
    }
}
