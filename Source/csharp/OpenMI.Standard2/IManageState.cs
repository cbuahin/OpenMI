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
  /// Optional interface to be implemented by components in addition to the
  /// <see cref="IBaseLinkableComponent"/> interface. It provides additional methods for handling
  /// component state so it can be saved, restored and cleared. It
  /// can be left completely to the component to handle persistence of state
  /// or it can also implement <see cref="IByteStateConverter"/> and provide ways
  /// for state to be converted to and from an array of bytes. A third-party
  /// could then handle the saving and loading of state data.
  /// </summary>
  public interface IManageState
  {
    /// <summary>
    /// Store the linkable component's current State
    /// </summary>
    /// <returns>Identifier of the stored state.</returns>
    IIdentifiable KeepCurrentState();

    /// <summary>
    /// Restores the state identified by the parameter stateID. If the state identifier identified by
    /// stateID is not known by the linkable component an IllegalArgumentException exception should be trown.
    /// </summary>
    /// <param name="stateId">Identifier of the state to be restored.</param>
    /// <remark>
    /// The method must throw an <code>ArgumentException</code> if an unknown <paramref name="stateId"/> is specified.
    /// </remark>
    void RestoreState(IIdentifiable stateId);

    /// <summary>
    /// Clears a state from the linkable component's memory. If the state identifier identified by
    /// stateID is not known by the linkable component an IllegalArgumentException exception should be thrown.
    /// </summary>
    /// <param name="stateId">Identifier of the state to be cleared.</param>
    /// <remark>
    /// The method must throw an <code>ArgumentException</code> if an unknown <paramref name="stateId"/> is specified.
    /// </remark>
    void ClearState(IIdentifiable stateId);
  }

}
