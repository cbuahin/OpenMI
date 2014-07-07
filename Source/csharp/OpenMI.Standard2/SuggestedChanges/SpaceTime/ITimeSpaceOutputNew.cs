using OpenMI.Standard2.SuggestedChanges.SpaceTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ITimeSpaceOutputNew: ITimeSpaceExchangeItemNew
    {
        ///<summary>
        /// Overridden version of the <see cref="IBaseOutput.GetValues"/> method.
        /// <see cref="GetValues"/> now returns an <see cref="ITimeSpaceValueSet"/>,
        /// instead of an <see cref="IBaseValueSet"/>.
        /// </summary>
        new ITimeSpaceValueSetNew GetValues(IBaseExchangeItem querySpecifier);
    }
}
