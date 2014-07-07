using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMI.Standard2.SuggestedChanges
{
    public interface IBaseExchangeItemNew : IIdentifiable
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

        ///<summary>
        /// The exchange item's values.
        ///</summary>
        IBaseValueSet Values { get; set; }


        /// <summary>
        /// Gets the owner of the exchange item. For an output exchange item this is
        /// the component responsible for providing the content of the output item.
        /// It is possible for an exchange item to have no owner, in this case the
        /// method will return null.
        /// </summary>
        IBaseLinkableComponent Component { get; }

        /// <summary>
        /// The ItemChanged event is fired when the content of an exchange item has changed.
        /// This might be because its ValueDefinition has changed, its TimeSet has changed,
        /// its ElementSet has changed, its Values have changed, or any permutation of these properties.
        /// </summary>
        event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;
    }
}
