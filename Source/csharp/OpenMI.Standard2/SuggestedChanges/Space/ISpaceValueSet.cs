using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ISpaceValueSet : IBaseValueSetNew , ISpaceExtension
    {
        /// <summary>
        /// Two-dimensional list of values.
        /// The first IList represents time, and the contained IList the element in the IElementSet.
        /// </summary>
        IList Values { get; set; }

        /// <summary>
        /// Get the value for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/> from <see cref="Values2D"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        object GetValue(int elementIndex);

        /// <summary>
        /// Set the value in <see cref="Values2D"/>, for the specified <paramref name="timeIndex"/> and <paramref name="elementIndex"/>.
        /// If the data is time independent, <code>timeIndex</code> must be specified as 0.
        /// If the data is not related to a location, <code>elementIndex</code> must be specified as 0.
        /// </summary>
        void SetValue(int elementIndex, object value);

    }
}
