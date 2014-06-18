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
    /// The LinkableComponentStatusChangeEventArgs contains the information that will be passed when the
    /// <see cref="IBaseLinkableComponent"/> fires the <code>StatusChanged</code> event.
    ///</summary>
    public class LinkableComponentStatusChangeEventArgs : EventArgs
    {
        ///<summary>
        /// Constructor.
        ///</summary>
        public LinkableComponentStatusChangeEventArgs()
        {
            Message = String.Empty;
        }

        ///<summary>
        /// The linkable component that fired the status change event.
        ///</summary>
        public IBaseLinkableComponent LinkableComponent { get; set; }
        ///<summary>
        /// The linkable component's status before the status change.
        ///</summary>
        public LinkableComponentStatus OldStatus { get; set; }
        ///<summary>
        /// The linkable component's status after the status change.
        ///</summary>
        public LinkableComponentStatus NewStatus { get; set; }
        ///<summary>
        /// A message providing additional information on the status change.
        /// If there is no message, an empty string is returned.
        ///</summary>
        public string Message { get; set; }
    }
}
