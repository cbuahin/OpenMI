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

using System.Collections.Generic;

namespace OpenMI.Standard2
{
    /// <summary>
    /// Qualitative data described items in terms of some quality or categorization
    /// that may be 'informal' or may use relatively ill-defined characteristics such
    /// as warmth and flavour. However, qualitative data can include well-defined
    /// aspects such as gender, nationality or commodity type. OpenMI defines the
    /// IQuality interface for working with qualitative data.
    /// <para>
    /// An IQuality describes qualitative data, where a value is specified as one
    /// category within a number of predefined (possible) categories. These
    /// categories can be ordered or not.
    /// </para><para>
    /// For qualitative data the IValueSet exchanged between ILinkableComponents
    /// contains one of the possible ICategory instances per element in the
    /// ElementSet involved.
    /// </para><para>
    /// <example>
    /// Examples:
    /// <list>
    /// <li>Colors: red, green, blue</li>
    /// <li>Land use: nature, recreation, industry, infrastructure</li>
    /// <li>Rating: worse, same, better</li>
    /// </list>
    /// </example>
    /// </para>
    /// </summary>
    public interface IQuality : IValueDefinition
    {
        /// <summary>
        /// Returns a list of the possible <see cref="ICategory"/> allowed for this
        /// IQuality. If the quality is not ordered the list contains the
        /// ICategory's in an unspecified order. When it is ordered the list
        /// contains the ICategory's in the same sequence.
        /// </summary>
        IList<ICategory> Categories { get; }

        /// <summary>
        /// Checks if the IQuality is defined by an ordered set of ICategory or not.
        /// </summary>
        bool IsOrdered { get; }
    }
}
