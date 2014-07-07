using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges.Space
{
    public interface ISpaceOutput : ISpaceExchangeItemNew
    {
        ///<summary>
        /// Overridden version of the <see cref="IBaseOutput.GetValues"/> method.
        /// <see cref="GetValues"/> now returns an <see cref="ITimeSpaceValueSet"/>,
        /// instead of an <see cref="IBaseValueSet"/>.
        /// </summary>
        new ISpaceValueSet GetValues(IBaseExchangeItem querySpecifier);
    }
}
