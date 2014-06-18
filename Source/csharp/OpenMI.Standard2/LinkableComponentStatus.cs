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

namespace OpenMI.Standard2
{
    /// <summary>
    /// <para>The LinkableComponentStatus enumerates the possible statuses that a linkable component can be in.
    /// The state diagram showing the possible statuses and the transitions from one status to another
    /// can be found in the documentation for OpenMI 2.0 on <see cref="http://www.openmi.org"/>.</para>
    /// <para>They are also mentioned in the documentation of the various methods of the 
    /// <see cref="IBaseLinkableComponent"/>.</para>
    /// </summary>
    public enum LinkableComponentStatus
    {
        ///<summary>
        /// The linkable component instance has just been <code>Created</code>.
        /// This status must and will be followed by <see cref="Initializing"/>.
        ///</summary>
        Created,
        ///<summary>
        /// The linkable component is initializing itself.
        /// This status will end in a status change to <see cref="Initialized"/> or <see cref="Failed"/>.
        ///</summary>
        Initializing,
        ///<summary>
        /// The linkable component has succesfully initialized itself. The connections between its inputs/outputs
        /// and those of other components can be established.
        ///</summary>
        Initialized,
        ///<summary>
        /// After links between the component's inputs/outputs and those of other components have
        /// been established, the component is validating whether its required input will be available
        /// when it updates itself, and whether indeed it will be able to provide the required output
        /// during this update.
        /// This <code>Validating</code> status will end in a status change to <see cref="Valid"/> or <see cref="Invalid"/>.
        ///</summary>
        Validating,
        ///<summary>
        /// The component is in a valid state. When updating itself its required input will be available, and it
        /// it will be able to provide the required output.
        ///</summary>
        Valid,
        ///<summary>
        /// The component wants to update itself, but is not yet able to perform the actual computation, because it is
        /// still waiting for input data from other components.
        ///</summary>
        WaitingForData,
        ///<summary>
        /// The component is in an invalid state. When updating itself not all required input will be available,
        /// and/or it will not be able to provide the required output. After the user has modified the connections
        /// between the component's inputs/outputs and those of other components, the <see cref="Validating"/> state
        /// can be entered again.
        ///</summary>
        Invalid,
        ///<summary>
        /// The component is preparing itself for the first <code>GetValues()</code> call.
        /// This <code>Preparing</code> state will end in a status change to <see cref="Updated"/> or <see cref="Failed"/>.
        ///</summary>
        Preparing,
        ///<summary>
        /// The component is updating itself. It has received all required input data from other components, and is now
        /// performing the actual computation.
        /// This <code>Updating</code> state will end in a status change to <see cref="Updated"/>, <see cref="Done"/> or <see cref="Failed"/>.
        ///</summary>
        Updating,
        ///<summary>
        /// The component has succesfully updated itself.
        ///</summary>
        Updated,
        ///<summary>
        /// The last update process that the component performed was the final one. A next call to the Update
        /// method will leave the component's internal state unchanged.
        ///</summary>
        Done,
        ///<summary>
        /// The ILinkableComponent was requested to perform the actions to be
        /// performed before it will either be disposed or re-intialized again.
        /// Typical actions would be writing the final result files, close all open
        /// files, free memory, etc. When all required actions have been performed,
        /// the status switches to <see cref="Created"/>when re-initialization is possible. The
        /// status switches to <see cref="Finished"/>when the component is to be disposed.
        ///</summary>
        Finishing,

        ///<summary>
        /// The ILinkableComponent has successfully performed its finalization
        /// actions. Re-initialization of the component instance is not possible and
        /// should not be attempted. Instead the instance should be disposed, e.g.
        /// through the garbage collection mechanism.
        ///</summary>
        Finished,

        /// The component was requisted to perform the actions to be perform before it will either be disposed
        /// or re-initialized again.
        /// Typical actions would be writing the final result files, close all open files, free memory, etcetera.
        /// When all required actions have been performed, the status switches back to <see cref="Created"/> if the
        /// component supports being re-initialized. If it cannot be re-initialized, it can be released from memory.
        ///<summary>
        /// The linkable component has failed initialize itself, failed to prepare itself for computation, or failed to complete its update process.
        ///</summary>
        Failed
    }

}
