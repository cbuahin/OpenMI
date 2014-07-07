using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface IBaseAdaptedOutputNew : IBaseOutputNew
    {
        ///<summary>
        /// Arguments needed to let the adapted output do its work. An unmodifiable
        /// list of the (modifiable) arguments should be returned that can be used to
        /// get info on the arguments and to modify argument values. Validation of changes
        /// is done when they occur (e.g. using notifications).
        /// 
        /// <returns>Unmodifiable list of IArgument for the adapted output</returns>
        ///</summary>
        IList<IArgument> Arguments { get; }

        /// <summary>
        /// Let the adapted output initialize itself, based on the current values
        /// specified by the arguments. Only after initialize is called the refresh
        /// method might be called.
        /// <para>
        /// A component must invoke the <see cref="Initialize()"/> method of all its
        /// adapted outputs at the end of the component's Prepare phase.
        /// In case of stacked adapted outputs, the adaptee must be initialized first.
        /// </para>
        /// </summary>
        void Initialize();

        /// <summary>
        /// Output item that this adaptedOutput extracts content from.
        /// In the adapter design pattern, it is the item being adapted.
        /// </summary>
        IBaseOutputNew Adaptee { get; }

        /// <summary>
        /// Request the adapted output to refresh itself. This method will be
        /// called by the adaptee, when it has been refreshed/updated. In the
        /// implementation of the refresh method the adapted output should
        /// update its contents according to the changes in the adaptee.
        /// <para>
        /// After updating itself the adapted output must call refresh on all
        /// its adapted outputs, so the chain of outputs refreshes itself.
        /// </para>
        /// </summary>
        void Refresh();
    }
}
