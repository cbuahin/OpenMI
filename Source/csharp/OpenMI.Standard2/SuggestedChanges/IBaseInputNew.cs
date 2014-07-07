using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface IBaseInputNew : IBaseExchangeItemNew
    {
        ///<summary>
        /// The provider this input should get its values from.
        ///</summary>
        IBaseOutput Provider { get; set; }

    }
}
