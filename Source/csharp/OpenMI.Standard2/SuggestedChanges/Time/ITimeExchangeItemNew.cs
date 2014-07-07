using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface ITimeExchangeItemNew : IBaseExchangeItemNew 
    {
       ///<summary>
       /// The exchange item's values.
       ///</summary>
       new  ITimeValueSet Values { get; set; }
    }
}
