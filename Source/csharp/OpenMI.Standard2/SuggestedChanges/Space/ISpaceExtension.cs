using OpenMI.Standard2.TimeSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ISpaceExtension
    {
        ///<summary>
        /// Spatial information (usually an element set) on the values that are available
        /// in an output exchange item, or required by an input exchange item.
        ///</summary>
        ISpatialDefinition SpatialDefinition { get; }
    }
}
