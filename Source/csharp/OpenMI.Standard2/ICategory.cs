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
    /// The ICategory describes one item of a possible categorization. It is used by
    /// the <see cref="IQuality"/> interface for describing qualitative data.
    /// <para>
    /// For qualitative data the <see cref="IBaseValueSet"/> exchanged between <see cref="IBaseLinkableComponent"/>s
    /// contains one of the possible <code>ICategory</code> instances per data element.
    /// </para>
    /// <para>
    /// A category defines one "class" within a "set of classes".
    /// </para>
    /// </summary>
    public interface ICategory : IDescribable
    {
        /// <summary>
        /// Value for this category.
        /// </summary>
        /// <example>
        /// "blue" in a "red"/"green"/"blue" set.
        /// </example>
        object Value { get; }
    }
}
