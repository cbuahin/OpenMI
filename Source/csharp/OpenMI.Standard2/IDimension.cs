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
    /// Enumeration for base dimensions
    /// </summary>
    public enum DimensionBase
    {
        /// <summary>
        /// Base dimension length.
        /// </summary>
        Length,

        /// <summary>
        /// Base dimension mass.
        /// </summary>
        Mass,

        /// <summary>
        /// Base dimension time.
        /// </summary>
        Time,

        /// <summary>
        /// Base dimension electric current.
        /// </summary>
        ElectricCurrent,

        /// <summary>
        /// Base dimension temperature.
        /// </summary>
        Temperature,

        /// <summary>
        /// Base dimension amount of substance.
        /// </summary>
        AmountOfSubstance,

        /// <summary>
        /// Base dimension luminous intensity.
        /// </summary>
        LuminousIntensity,

        /// <summary>
        /// Base dimension currency.
        /// </summary>
        Currency
    }

    /// <summary>
    /// Defines the order of dimension in each <see cref="DimensionBase"/> for a unit
    /// </summary>
    public interface IDimension
    {
        /// <summary>
        /// <para>Returns the power for the requested dimension</para>
        /// 
        /// </summary>
        /// <example>
        /// <para>For a quantity such as flow, which may have the unit m3/s, the GetPower method must
        /// work as follows:</para>
        /// 
        /// <para>myDimension.GetPower(DimensionBase.AmountOfSubstance) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Currency)          --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.ElectricCurrent)   --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Length)            --> returns 3</para>
        /// <para>myDimension.GetPower(DimensionBase.LuminousIntensity) --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Mass)              --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Temperature)       --> returns 0</para>
        /// <para>myDimension.GetPower(DimensionBase.Time)              --> returns -1</para>
        /// </example>
        double GetPower(DimensionBase baseQuantity);
    }
}
