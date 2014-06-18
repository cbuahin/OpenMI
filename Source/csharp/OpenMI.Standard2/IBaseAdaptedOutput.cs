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
using System.Collections.Generic;

namespace OpenMI.Standard2
{
    ///<summary>
    /// An <see cref="IBaseAdaptedOutput"/> adds one or more data operations on top of an output item. 
    /// It is in itself an <see cref="IBaseOutput"/>. The adaptedOutput extends an output 
    /// item with functionality as spatial interpolation, temporal interpolation, unit conversion
    /// etc.
    /// <para>
    /// <see cref="IBaseAdaptedOutput"/> instances are created by means of an <see cref="IAdaptedOutputFactory"/>.
    /// </para>
    /// <para>
    /// The <see cref="IBaseAdaptedOutput"/> is based on the adaptor design pattern. It adapts
    /// an <see cref="IBaseOutput"/> or another <see cref="IBaseAdaptedOutput"/> to make it 
    /// suitable for new use or purpose. The object being adapted is typically called the 
    /// "adaptee". The <see cref="IBaseAdaptedOutput"/> replaces the DataOperation that was
    /// used in OpenMI Standard version 1.x.
    /// </para>
    ///</summary>
    public interface IBaseAdaptedOutput : IBaseOutput
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
        IBaseOutput Adaptee { get; }

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
