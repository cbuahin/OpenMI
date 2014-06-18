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
    /// This interface is an optional complement to the <see cref="IManageState"/> interface.
    /// Both are extensions to <see cref="IBaseLinkableComponent"/>, meant to provide state management.
    /// It defines methods for converting a state into a byte stream and reading
    /// in a state from a byte stream.
    /// This facilitates external modules, e.g. a GUI or an operational control system,
    /// to save a model's state somewhere as persistent state.
    /// </summary>
    public interface IByteStateConverter
    {
        /// <summary>
        /// Converts the state with the <paramref name="stateId"/> into a byte stream.
        /// </summary>
        /// <param name="stateId">id of the state.</param>
        /// <returns>The state identified by <paramref name="stateId"/> as an array of bytes.</returns>
        byte[] ConvertToByteArray(IIdentifiable stateId);

        /// <summary>
        /// Creates a state from a byte stream and returns the identifier of this state.
        /// <remarks>The state does not become the current state of the <see cref="IBaseLinkableComponent"/>.
        /// For state management the <see cref="IManageState"/> interface is to be used.</remarks>
        /// </summary>
        /// <param name="byteArray">State as a byte stream.</param>
        /// <returns><see cref="IIdentifiable"/> identifying the state.</returns>
        IIdentifiable ConvertFromByteArray(byte[] byteArray);
    }
}
