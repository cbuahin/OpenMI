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
    /// Used to create instances of <see cref="IBaseAdaptedOutput"/> items.
    /// </summary>
    public interface IAdaptedOutputFactory : IIdentifiable
    {
        /// <summary>
        /// Get a list of identifiers of the available <see cref="IBaseAdaptedOutput"/>s that can make 
        /// the <paramref name="adaptee"/> match the <paramref name="target"/>. If the 
        /// <paramref name="target"/>is <code>null</code>,
        /// the identifiers of all <see cref="IBaseAdaptedOutput"/>s that can adapt the 
        /// <paramref name="adaptee"/> are returned.
        /// </summary>
        /// <param name="adaptee"><see cref="IBaseOutput"/> to adapt.</param>
        /// <param name="target"><see cref="IBaseInput"/> to adapt the adaptee to, can be <code>null</code>.</param>
        /// <returns>List of identifiers for the available <see cref="IBaseAdaptedOutput"/>s.</returns>
        IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target);
        
        /// <summary>
        /// Creates a <see cref="IBaseAdaptedOutput"/> that adapts the <paramref name="adaptee"/>
        /// so that it fits the target.
        /// <para>
        /// The adaptedOutputId used must be one of the IIdentifiable instances
        /// returned by the <see cref="GetAvailableAdaptedOutputIds"/> method.
        /// </para>
        /// <para>
        /// The returned <see cref="IBaseAdaptedOutput"/> will already be registered with the
        /// <paramref name="adaptee"/>.
        /// </para>
        /// </summary>
        /// <param name="adaptedOutputId">The identifier of the adaptedOutput to create.</param>
        /// <param name="adaptee"><see cref="IBaseOutput"/> to adapt.</param>
        /// <param name="target"><see cref="IBaseInput"/> to adapt the adaptee to, can be <code>null</code>.</param>
        /// <returns></returns>
        IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target);
    }
}
