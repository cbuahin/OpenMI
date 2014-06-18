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
    /// An output exchange item that can deliver values from a time / space dependent ILinkableComponent.
    /// The output is a combination of an <see cref="IValueDefinition"/>, an <see cref="IElementSet"/>,
    /// and an <see cref="ITimeSet"/>. This combination specifies which type of data can be
    /// provided where and when by the ILinkableComponent.
    /// <para>
    /// If an output does not provide the data in the way a consumer would
    /// like to have it the output can be adapted by an <see cref="ITimeSpaceAdaptedOutput"/>, which can
    /// transform the data according to the consumer's wishes. E.g. by performing
    /// interpolation in time, spatial aggregation, etc.).
    /// </para>
    /// <para>If a output item does provide the data in the way a consumer would lik to have it
    /// the output item can be decorated by an <see cref="ITimeSpaceAdaptedOutput"/>, which can
    /// transform the data according to the consumer's wishes (e.g. by performing interpolation in time,
    /// spatial interpolation etc.).
    /// </para>
    /// </summary>
    public interface ITimeSpaceOutput : ITimeSpaceExchangeItem, IBaseOutput
    {
        ///<summary>
        /// The exchange item's values, as a specialized <see cref="ITimeSpaceValueSet"/>
        ///</summary>
        new ITimeSpaceValueSet Values { get; }

        ///<summary>
        /// Overridden version of the <see cref="IBaseOutput.GetValues"/> method.
        /// <see cref="GetValues"/> now returns an <see cref="ITimeSpaceValueSet"/>,
        /// instead of an <see cref="IBaseValueSet"/>.
        /// </summary>
        new ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier);
    }
}