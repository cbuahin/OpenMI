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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    [TestFixture]
    public class RainRrCfTest
    {

        const string _MaxLoopCallsExceeded = "Max # loop calls exceeded";

        private static bool CheckIfComponentsAreFinished(IEnumerable<ITimeSpaceComponent> components)
        {
            bool atLeastOneComponentFinished = false;
            foreach (ITimeSpaceComponent component in components)
            {
                if (component.Status == LinkableComponentStatus.Done)
                {
                    Console.Out.WriteLine(component.Id + " has finished, loop will be terminated");
                    atLeastOneComponentFinished = true;
                }

                if (component.Status == LinkableComponentStatus.Failed)
                {
                    Console.Error.WriteLine(component.Id + " has failed");
                    Assert.Fail("Component has failed: " + component);
                }
            }
            return atLeastOneComponentFinished;
        }

        //private static void RunLoopDriven(IEnumerable<ITimeSpaceComponent> components)
        //{
        //    bool atLeastOneComponentFinished = false;

        //    foreach (ITimeSpaceComponent component in components)
        //    {
        //        component.CascadingUpdateCallsDisabled = true;
        //    }

        //    const int maxNumLoopCalls = 10000;
        //    int numLoopCalls = 0;
        //    while (!atLeastOneComponentFinished && numLoopCalls < maxNumLoopCalls)
        //    {
        //        foreach (ITimeSpaceComponent component in components)
        //        {
        //            component.Update();
        //        }

        //        // check if components are finished
        //        atLeastOneComponentFinished = CheckIfComponentsAreFinished(components);
        //        numLoopCalls++;
        //    }
        //    if (numLoopCalls == maxNumLoopCalls)
        //    {
        //        throw new Exception(_MaxLoopCallsExceeded);
        //    }
        //}

        private static void RunPullDriven(IEnumerable<ITimeSpaceComponent> components)
        {
            ITimeSpaceComponent componentAtEndOfChain = RainRrCfCompositions.FindComponent(components, RainRrCfCompositions.channelFlowId);

            while (!(componentAtEndOfChain.Status == LinkableComponentStatus.Done ||
                     componentAtEndOfChain.Status == LinkableComponentStatus.Failed))
            {
                componentAtEndOfChain.Update();
            }
        }

        //[Test]
        //public void UpdateInLoopA()
        //{
        //    ICollection<ITimeSpaceComponent> components = RainRrCfCompositions.CreateCompositionA();
        //    try
        //    {
        //        RunLoopDriven(components);
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.IsTrue(e.Message.Contains(_MaxLoopCallsExceeded), "Check expected exception");
        //    }
        //}

        [Test]
        public void UpdatePullDrivenA()
        {
            ICollection<ITimeSpaceComponent> components = RainRrCfCompositions.CreateCompositionA();
            RunPullDriven(components);
        }

        //[Test]
        //public void UpdateInLoopB()
        //{
        //    const bool useTimeExtrapolator = false;
        //    ICollection<ITimeSpaceComponent> components = RainRrCfCompositions.CreateCompositionB(useTimeExtrapolator);

        //    try
        //    {
        //        RunLoopDriven(components);
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.IsTrue(e.Message.Contains(_MaxLoopCallsExceeded), "Check expected exception");
        //    }
        //}

        [Test]
        public void UpdatePullDrivenB()
        {
            const bool useTimeExtrapolator = false;
            ICollection<ITimeSpaceComponent> components = RainRrCfCompositions.CreateCompositionB(useTimeExtrapolator);

            try
            {
                RunPullDriven(components);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.StartsWith("Could not update engine") &&
                              e.Message.EndsWith("Use a Time Extrapolator."), "Check expected exception");
            }
        }

        [Test]
        public void UpdatePullDrivenBWithTimeExtrapolator()
        {
            const bool useTimeExtrapolator = true;
            ICollection<ITimeSpaceComponent> components = RainRrCfCompositions.CreateCompositionB(useTimeExtrapolator);
            RunPullDriven(components);
        }
    }
}