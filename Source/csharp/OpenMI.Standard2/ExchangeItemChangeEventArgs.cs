#region Copyright

/*
    Copyright (c) 2005-2010, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard2.dll

    OpenMI.Standard2.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard2.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;

namespace OpenMI.Standard2
{
    ///<summary>
    /// The ExchangeItemChangeEventArgs contains the information that will be passed when the
    /// <see cref="IBaseExchangeItem"/> fires the <code>ExchangeItemValueChanged</code> event.
    /// <para>
    /// Sending exchange item events is optional, so it should not be used as a
    /// mechanism to build critical functionality upon.
    /// </para>
    ///</summary>
    public class ExchangeItemChangeEventArgs: EventArgs
    {
        ///<summary>
        /// Default constructor. Creates a new instance with an empty message and
        /// null as exchangeItem. Properties need to be set before actually using
        /// the instance.
        ///</summary>
        public ExchangeItemChangeEventArgs()
        {
            ExchangeItem = null;
            Message = String.Empty;
        }


        /// <summary>
        /// Constructor that also initializes the <see cref="ExchangeItem"/>
        /// and the <see cref="Message"/> property
        /// </summary>
        public ExchangeItemChangeEventArgs(IBaseExchangeItem exchangeItem, string message)
        {
          ExchangeItem = exchangeItem;
          Message = message;
        }

        ///<summary>
        /// The exchange item of which the status has been changed.
        ///</summary>
        public IBaseExchangeItem ExchangeItem { get; set; }

        ///<summary>
        /// A message that describes the way in which the status of the exchange item has been changed.
        ///</summary>
        public string Message { get; set; }
    }
}
