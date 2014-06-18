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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
    public class ValueSet : ITimeSpaceValueSet
    {
        protected IList<IList> values2D;

        public ValueSet()
        {
            values2D = new List<IList>();
            values2D.Add(new List<object>());
        }

        //public ValueSet(IList elementValues)
        //{
        //    values2D = new List<IList>() {elementValues};
        //}

        public ValueSet(IList<IList> values2D)
        {
            this.values2D = values2D;
        }

        public IList<IList> Values2D
        {
            get { return values2D; }
            set { values2D = value; }
        }

        public object GetValue(int timeIndex, int elementIndex)
        {
            CheckTimeIndex(this, timeIndex);
            CheckElementIndex(this, elementIndex);
            return Values2D[timeIndex][elementIndex];
        }

        public void SetValue(int timeIndex, int elementIndex, object value)
        {
            CheckTimeIndex(this, timeIndex);
            CheckElementIndex(this, elementIndex);
            Values2D[timeIndex][elementIndex] = value;
        }

        public IList GetTimeSeriesValuesForElement(int elementIndex)
        {
            CheckElementIndex(this, elementIndex);
            IList values = new List<object>(Values2D.Count);
            for (int timeIndex = 0; timeIndex < Values2D.Count; timeIndex++)
            {
                values.Add(Values2D[timeIndex][elementIndex]);
            }
            return values;
        }

        public void SetTimeSeriesValuesForElement(int elementIndex, IList values)
        {
            for (int timeIndex = 0; timeIndex < Values2D.Count; timeIndex++)
            {
                Values2D[timeIndex][elementIndex] = values[elementIndex];
            }
        }

        public IList GetElementValuesForTime(int timeIndex)
        {
            CheckTimeIndex(this,timeIndex);
            return Values2D[timeIndex];
        }

        public void SetElementValuesForTime(int timeIndex, IList values)
        {
            CheckElementIndex(this,timeIndex);
            Values2D[timeIndex] = values;
        }

        public void AddElementValuesForTime(IList values)
        {
            if (values2D == null)
            {
                values2D = new List<IList>();
            }
            values2D.Add(values);
        }

        public void AddTimeValuesForElement(IList values)
        {
            if (values2D == null)
            {
                values2D = new List<IList>();
            }

            for (int timeIndex = 0; timeIndex < values.Count; timeIndex++)
            {
                if (values2D.Count < timeIndex)
                {
                    values2D.Add(new List<object>());
                }

                Values2D[timeIndex].Add(values[timeIndex]);
            }
        }


        public static int GetTimesCount(ITimeSpaceValueSet valueSet)
        {
            return valueSet.Values2D.Count;
        }

        public static int GetElementCount(ITimeSpaceValueSet valueSet)
        {
            return (valueSet.Values2D.Count > 0) ? valueSet.Values2D[0].Count : 0;
        }

        public static void CheckTimeIndex(ITimeSpaceValueSet valueSet, int timeIndex)
        {
            if (timeIndex < 0)
            {
                throw new Exception("Invalid timeindex (" + timeIndex + "), negative index not allowed");
            }
            if (timeIndex >= GetTimesCount(valueSet))
            {
                throw new Exception("Invalid timeindex (" + timeIndex +
                    "), only " + GetTimesCount(valueSet) + " times available");
            }
        }

        public static void CheckElementIndex(ITimeSpaceValueSet valueSet, int elementIndex)
        {
            if (elementIndex < 0)
            {
                throw new Exception("Invalid elementindex (" + elementIndex + "), negative index nog allowed");
            }
            if (elementIndex >= GetElementCount(valueSet))
            {
                throw new Exception("Invalid elementindex (" + elementIndex +
                    "), only " + GetElementCount(valueSet) + " elements available");
            }
        }

        public Type ValueType
        {
            // TODO: Infer the value type from the Values2D array.
            get { throw new NotImplementedException(); }
        }

        int IBaseValueSet.NumberOfIndices
        {
            get { return (2); }
        }

        int IBaseValueSet.GetIndexCount(int[] indices)
        {
            if (indices == null || indices.Length == 0)
            {
                return (values2D.Count);
            }
            if (indices.Length > 1)
                throw new ArgumentException("Indices does not have the correct length, length must be smaller than 2", "indices");
            return (values2D[indices[0]].Count);
        }

        object IBaseValueSet.GetValue(int[] indices)
        {
            if (indices.Length != 2)
                throw new ArgumentException("Indices does not have the correct length", "indices");
            return (GetValue(indices[0], indices[1]));
        }

        void IBaseValueSet.SetValue(int[] indices, object value)
        {
            if (indices.Length != 2)
                throw new ArgumentException("Indices does not have the correct length", "indices");
            SetValue(indices[0], indices[1], value);
        }
    }
}
