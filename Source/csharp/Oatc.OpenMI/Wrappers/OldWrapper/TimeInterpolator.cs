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
﻿using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Buffer;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper
{
    internal class TimeInterpolator : DecoratorBase
    {
        private const string timeInterpolatorId = "timeInterpolatorId";

        private SmartBuffer buffer;

        public TimeInterpolator()
            : base(timeInterpolatorId)
        {
        }

        public override bool IsAvailable(IExchangeItem querySpecifier)
        {
            CheckBuffer();
            if (buffer.TimesCount > 0)
            {
                ITime lastTimeInBuffer = buffer.GetTimeAt(buffer.TimesCount - 1);
                IList<ITime> consumerTimes = querySpecifier.TimeSet.Times;
                ITime lastConsumerTime = consumerTimes[consumerTimes.Count - 1];
                const double epsilon = 1e-10;
                double lastConsumerTimeAsMJD = lastConsumerTime.StampAsModifiedJulianDay +
                                               lastConsumerTime.DurationInDays;
                double lastTimeInBufferAsMJD = lastTimeInBuffer.StampAsModifiedJulianDay +
                                               lastTimeInBuffer.DurationInDays;
                return lastConsumerTimeAsMJD < (lastTimeInBufferAsMJD + epsilon);
            }
            return false;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        private void CheckBuffer()
        {
            if (buffer == null)
            {
                buffer = new SmartBuffer();
                IList outputItemValues = DecoratedOutputItem.Values;
                IList<ITime> outputItemTimes = DecoratedOutputItem.TimeSet.Times;
                int i = 0;
                foreach (ITime time in outputItemTimes)
                {
                    buffer.AddValues(time, new double[] { (double)outputItemValues[i++] });
                }
            }
        }
    }
}
