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

using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Buffer
{
    /// <summary>
    /// Support functions for the Buffer.
    /// </summary>
    public class BufferHelper
    {
        /// <summary>
        /// return true if ta is before tb (eveluates (ta less than tb)
        /// </summary>
        /// <param name="ta">Time ta</param>
        /// <param name="tb">time tb</param>
        /// <returns>true if ta is before tb (eveluates (ta less than tb)</returns>
        public static bool IsBefore(ITime ta, ITime tb)
        {
            return ta.EndStampAsModifiedJulianDay() < tb.StampAsModifiedJulianDay;
        }

        /// <summary>
        /// GetVal will get the index´th number of the axisNumber´th component of the 
        /// ValueSet.
        /// </summary>
        /// <param name="values">ValueSet to read the value from.</param>
        /// <param name="index">Index of the value in the ValueSet.</param>
        /// <returns>The index´th number of the axisNumber´th component</returns>
        public static double GetVal(double[] values, int index)
        {
            try
            {
                double x;

                if (index > values.Length - 1)
                {
                    throw new System.Exception("Index exceeds dimension of ValueSet.");
                }

                x = values[index];

                return x;
            }
            catch (System.Exception e)
            {
                throw new System.Exception("GetVal failed", e);
            }
        }
    }
}
