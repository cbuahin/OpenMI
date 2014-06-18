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
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper
{
	/// <summary>
	/// The exchange item is a combination of a quantity and an element set.
    /// <para>This is a trivial implementation of OpenMI.Standard.IExchangeItem, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class BufferedOutputItem : OutputItem
	{
        private SmartBuffer buffer;

        public BufferedOutputItem(string id, IValueDefinition quantity, IElementSet elementSet, ILinkableComponent component) : base(id, quantity, elementSet)
        {
            Component = component;
        }

        public void SetValues(ITime time, object values)
        {
            if (!(values is double[]))
            {
                throw new ArgumentException("Expecting array of doubles");
            }
            buffer.AddValues(time, (double[])values);
        }

        public new IList Values()
        {
            return buffer.GetAllValues();
        }

        public IList<T> GetValues<T>(ITime[] times)
        {
            if (typeof(T) != typeof(double))
            {
                throw new NotSupportedException("Only doubles supported");
            }
            if (buffer == null)
            {
                buffer = new SmartBuffer();
            }
            IList<T> valuesList = new List<T>();
            foreach (ITime time in times)
            {
                for (int timeIndex = 0; timeIndex < TimeSet.Times.Count; timeIndex++)
                {
                    if (TimeSet.Times[timeIndex].Equals(time))
                    {
                        double[] valuesAtTime = buffer.GetValuesAt(timeIndex);
                        for (int elementIndex = 0; elementIndex < ElementSet.ElementCount; elementIndex++)
                        {
                            valuesList.Add((T)(object)valuesAtTime[ComputeIndex(timeIndex, elementIndex)]);
                        }
                        break;
                    }
                }
            }
            return valuesList;
        }

        public ITime LastBufferedTime
	    {
            get
            {
                return buffer.GetTimeAt(buffer.TimesCount - 1);
            }
	    }
	}
}
