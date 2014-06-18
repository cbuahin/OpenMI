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
using System.Collections.Generic;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone.UnitTest
{
    [TestFixture]
    public class LinkableComponentTest
    {
        private LinkableComponent linkableComponent;
        private LinkableComponent anotherLinkableComponent;

        private LinkableComponentStatus currentStatus = LinkableComponentStatus.Failed;
        private LinkableComponentStatus formerStatus = LinkableComponentStatus.Failed;

        [SetUp]
        public void SetUp()
        {
            linkableComponent = new DerivedLinkableComponent();
            linkableComponent.StatusChanged += StatusChanged;

            anotherLinkableComponent = new DerivedLinkableComponent();
        }

        [Test]
        public void Events()
        {
            Assert.AreEqual(linkableComponent.Status, LinkableComponentStatus.Created);

            linkableComponent.Initialize();
            Assert.AreEqual(formerStatus, LinkableComponentStatus.Initializing);
            Assert.AreEqual(currentStatus, LinkableComponentStatus.Initialized);

            linkableComponent.Validate();
            Assert.AreEqual(formerStatus, LinkableComponentStatus.Validating);
            Assert.AreEqual(currentStatus, LinkableComponentStatus.Valid);

            linkableComponent.Update();
            Assert.AreEqual(formerStatus, LinkableComponentStatus.Updating);
            Assert.AreEqual(currentStatus, LinkableComponentStatus.Updated);

            linkableComponent.Finish();
            Assert.AreEqual(formerStatus, LinkableComponentStatus.Finishing);
            Assert.AreEqual(currentStatus, LinkableComponentStatus.Done);
        }

        private void StatusChanged(object sender, LinkableComponentStatusChangeEventArgs e)
        {
            formerStatus = e.OldStatus;
            currentStatus = e.NewStatus;
        }

        [Test]
        public void Equal()
        {
            Assert.AreEqual(linkableComponent, anotherLinkableComponent);
            Assert.AreEqual(linkableComponent.GetHashCode(), anotherLinkableComponent.GetHashCode());
        }
    }

    public class DerivedLinkableComponent : LinkableComponent
    {

        protected List<ITimeSpaceInput> inputItems = new List<ITimeSpaceInput>();
        protected List<ITimeSpaceOutput> outputItems = new List<ITimeSpaceOutput>();

        public override List<IAdaptedOutputFactory> AdaptedOutputFactories
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Initialize()
        {
            Status = LinkableComponentStatus.Initializing;
            Status = LinkableComponentStatus.Initialized;
        }

        public override string[] Validate()
        {
            Status = LinkableComponentStatus.Validating;
            Status = LinkableComponentStatus.Valid;
            return new string[0];
        }

        public override void Prepare()
        {
            // TODO: Move code to here?!
        }

        public override IList<IBaseInput> Inputs
        {
            get { return new ListWrapper<ITimeSpaceInput, IBaseInput>(inputItems); }
        }

        public override IList<IBaseOutput> Outputs
        {
            get { return new ListWrapper<ITimeSpaceOutput, IBaseOutput>(outputItems); }
        }

        public override void Update(params IBaseOutput[] requiredOutput)
        {
            Status = LinkableComponentStatus.Updating;
            // no action
            Status = LinkableComponentStatus.Updated;
        }

        public override void Finish()
        {
            Status = LinkableComponentStatus.Finishing;
            // no action
            Status = LinkableComponentStatus.Done;
        }
    }
}
