using OpenMI.Standard2.TimeSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ITimeSpaceValueSetNew : IBaseValueSetNew , ITimeSpaceExtension
    {
        /// <summary>
        /// Two-dimensional list of values.
        /// The first IList represents time, and the contained IList the element in the IElementSet.
        /// </summary>
        IList<IList> Values2D { get; set; }

        /// <summary>
        /// Get the value for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/> from <see cref="Values2D"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        object GetValue(int timeIndex, int elementIndex);

        /// <summary>
        /// Set the value in <see cref="Values2D"/>, for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        void SetValue(int timeIndex, int elementIndex, object value);

        /// <summary>
        /// Get values from <see cref="Values2D"/>, for all times, for the specified <paramref name="elementIndex"/> .
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        IList GetTimeSeriesValuesForElement(int elementIndex);

        /// <summary>
        /// Set values in <see cref="Values2D"/>, for all times, for the specified <paramref name="elementIndex"/>.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        void SetTimeSeriesValuesForElement(int elementIndex, IList values);

        /// <summary>
        /// Get values from <see cref="Values2D"/>, for all elements, for the specified <paramref name="timeIndex"/> .
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// </summary>
        IList GetElementValuesForTime(int timeIndex);

        /// <summary>
        /// Set values in <see cref="Values2D"/>, for all elements, for the specified <paramref name="timeIndex"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// </summary>
        void SetElementValuesForTime(int timeIndex, IList values);

    }
}
