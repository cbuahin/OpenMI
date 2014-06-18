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

namespace OpenMI.Standard2.TimeSpace
{
    ///<summary>
    /// An <see cref="ITimeSpaceAdaptedOutput"/> adds one or more data operations on top of an output item. 
    /// It is in itself an <see cref="IBaseAdaptedOutput"/>. The adaptedOutput extends an output 
    /// item with functionality as spatial interpolation, temporal interpolation, unit conversion
    /// etc.
    /// <para>ITimeSpaceAdaptedOutput instances are created by means of an <see cref="IAdaptedOutputFactory"/>.
    /// </para>
    /// <para>The IAdaptedOutput is based on the adaptor design pattern. It adapts
    /// an IOutput or another IAdaptedOutput to make it suitable for new use or
    /// purpose. The object being adapted is typically called the "adaptee". The
    /// IAdaptedOutput replaces the DataOperation that was used in OpenMI 1.x.
    /// </para>
    ///</summary>
    public interface ITimeSpaceAdaptedOutput : IBaseAdaptedOutput, ITimeSpaceOutput
    {
    }
}