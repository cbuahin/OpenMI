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
  /// <summary>
  /// <para>
  /// Methods that are specific for an time-space component.
  /// </para>
  /// </summary>
  public interface ITimeExtension
  {
    /// <summary>
    /// The <see cref="TimeExtent"/> property describes in what time span the component can operate. This can be used to support the user when creating 
    /// a composition.
    /// </summary>
    ITimeSet TimeExtent { get; }
  }
}