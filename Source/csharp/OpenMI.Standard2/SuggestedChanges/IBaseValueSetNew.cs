using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface IBaseValueSetNew
    {
        /// <summary>
        /// Definition of the values in the exchange item.
        /// </summary>
        /// <remarks>
        /// The <see cref="IValueDefinition"/> should never be returned directly; all implementing
        /// classes should return either an <see cref="IQuality"/>, an <see cref="IQuantity"/>, or a
        /// custom derived vale definition interface.
        /// </remarks>
        IValueDefinition ValueDefinition { get; }

        /// <summary>
        /// Returns the number of possible indices (dimensions) for the value set.
        /// </summary>
        /// <returns>number of indices, zero based</returns>
        int NumberOfIndices { get; }

        /// <summary>
        /// Returns the length (max index count) of the dimension specified by the
        /// given indices. To get the size of the first dimension, use a zero-length
        /// integer array as input argument. Length of indices must be a least one
        /// smaller than the <see cref="NumberOfIndices"/>
        /// </summary>
        /// <param name="indices">indices of the dimension to get the length of</param>
        /// <returns>length of the specified dimension</returns>
        int GetIndexCount(int[] indices);

        /// <summary>
        /// Returns the value object specified by the given array of indices.
        /// The length of the array of indices is N, so that the index for
        /// each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
        /// </summary>
        ///<param name="indices">indices index value for each dimension</param>
        ///<returns>the value object for the given indices</returns>
        Object GetValue(int[] indices);

        /// <summary>
        /// Set the value object specified by the given array of indices.
        /// The length of the array of indices is N, so that the index for
        /// each dimension is specified. Otherwise an IllegalArgumentException must be thrown.
        /// </summary>
        /// <param name="indices">indices index value for each dimension</param>
        /// <param name="value">value the value object for the given indices</param>
        void SetValue(int[] indices, Object value);

    }
}
