using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ISpaceExchangeItemNew : IBaseExchangeItemNew 
    {
       ///<summary>
       /// The exchange item's values.
       ///</summary>
       new  ISpaceValueSet Values { get; set; }
    }
}
