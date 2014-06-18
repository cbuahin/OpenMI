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
    /// Provides descriptive information on an OpenMI entity.
    /// <para>
    /// An entity that is describable has a caption (title or heading) and a
    /// description. These are not to be used for identification (see <see cref="IIdentifiable"/>).
    /// </para>
    /// </summary>
    public interface IDescribable
    {
        /// <summary>
        /// Caption string (not to be used as an id)
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Additional descriptive information about the entity.
        /// </summary>
        string Description { get; set; }
    }
}
