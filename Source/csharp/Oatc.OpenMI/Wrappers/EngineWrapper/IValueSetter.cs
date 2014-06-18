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
﻿using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper
{
    /// <summary>
    /// Interface for setting values to an engine
    /// </summary>
    public interface IValueSetter
    {
        /// <summary>
        /// Set values to engine.
        /// </summary>
        void SetValues(ITimeSpaceValueSet values);
    }

    /// <summary>
    /// Class implementing <see cref="IValueSetter"/> and
    /// <see cref="IValueGetter"/>, where the
    /// value to get/set is one single element of a vector. I.e., when 
    /// calling <see cref="SetValues"/> the vector at the given index
    /// is updated with the first value in the <see cref="SetValues"/> argument.
    /// </summary>
    public class ValueToVectorGetSetter<T> : IValueSetter, IValueGetter
    {
        private readonly IList<T> _vector;
        private readonly int _index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="vector">Vector to set value to</param>
        /// <param name="index">Index in vector where values are set</param>
        public ValueToVectorGetSetter(IList<T> vector, int index)
        {
            _vector = vector;
            _index = index;
        }

        public void SetValues(ITimeSpaceValueSet values)
        {
            // Assuming elsewhere is taken care of that the sizes are correct.
            _vector[_index] = (T) values.GetValue(0, 0);
        }

        public ITimeSpaceValueSet GetValues()
        {
            IList<IList> res = new List<IList>(1){new List<T>(1){_vector[_index]}};
            return (new ValueSet(res));
        }
    }

    /// <summary>
    /// Class implementing <see cref="IValueSetter"/> and
    /// <see cref="IValueGetter"/>, where the
    /// values to set is an entire vector. I.e., when calling
    /// <see cref="SetValues"/>, the content of the argument
    /// is copied to the vector.
    /// </summary>
    public class VectorValueGetSetter<T> : IValueSetter, IValueGetter
    {
        private readonly IList<T> _vector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vector">Vector to set values to</param>
        public VectorValueGetSetter(IList<T> vector)
        {
            _vector = vector;
        }

        public void SetValues(ITimeSpaceValueSet values)
        {
            // Assuming elsewhere is taken care of that the sizes are correct.
            // i.e. one time step and same number of elements
            IList elementValues = values.GetElementValuesForTime(0);
            for (int i = 0; i < _vector.Count; i++)
            {
                _vector[i] = (T)elementValues[i];
            }
        }

        public ITimeSpaceValueSet GetValues()
        {
            return (new TimeSpaceValueSet<T>(_vector));
        }
    }



    /// <summary>
    /// Class implementing <see cref="IValueSetter"/> and
    /// <see cref="IValueGetter"/>, where the
    /// value to get/set is one single element of a vector. I.e., when 
    /// calling <see cref="SetValues"/> the vector at the given index
    /// is updated with the first value in the <see cref="SetValues"/> argument.
    /// <para>
    /// The values from the vector are multiplied by the scale factor 
    /// before it is returned, and values are divided by the scale factor before
    /// set to the vector.
    /// </para>
    /// </summary>
    public class ValueToVectorScaledGetSetter : IValueSetter, IValueGetter
    {
        private readonly IList<double> _vector;
        private readonly int _index;
        private readonly double _scaleFactor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="vector">Vector to set value to</param>
        /// <param name="index">Index in vector where values are set</param>
        public ValueToVectorScaledGetSetter(IList<double> vector, int index, double scaleFactor)
        {
            _vector = vector;
            _index = index;
            _scaleFactor = scaleFactor;
        }

        public void SetValues(ITimeSpaceValueSet values)
        {
            // Assuming elsewhere is taken care of that the sizes are correct.
            _vector[_index] = ((double) values.GetValue(0, 0))/_scaleFactor;
        }

        public ITimeSpaceValueSet GetValues()
        {
            IList<IList> res = new List<IList> { new List<double> { _vector[_index] * _scaleFactor } };
            return (new ValueSet(res));
        }
    }

    /// <summary>
    /// Class implementing <see cref="IValueSetter"/> and
    /// <see cref="IValueGetter"/>, where the
    /// values to set is an entire vector. I.e., when calling
    /// <see cref="SetValues"/>, the content of the argument
    /// is copied to the vector.
    /// <para>
    /// The values from the vector are multiplied by the scale factor 
    /// before it is returned, and values are divided by the scale factor before
    /// set to the vector.
    /// </para>
    /// </summary>
    public class VectorValueScaledGetSetter : IValueSetter, IValueGetter
    {
        private readonly IList<double> _vector;
        private readonly double _scaleFactor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vector">Vector to set values to</param>
        /// <param name="scaleFactor">Scalefactor that values are multiplied with when getting values</param>
        public VectorValueScaledGetSetter(IList<double> vector, double scaleFactor)
        {
            _vector = vector;
            _scaleFactor = scaleFactor;
        }

        public void SetValues(ITimeSpaceValueSet values)
        {
            double invScaleFactor = 1.0/_scaleFactor;
            // Assuming elsewhere is taken care of that the sizes are correct.
            IList elementValues = values.GetElementValuesForTime(0);
            for (int i = 0; i < _vector.Count; i++)
            {
                _vector[i] = ((double)elementValues[i])*invScaleFactor;
            }
        }

        public ITimeSpaceValueSet GetValues()
        {
            return (new TimeSpaceValueSet<double>(_vector));
        }
    }
}
