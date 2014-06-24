#region Copyright
/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
﻿using Oatc.OpenMI.Sdk.Backbone.Generic;
﻿using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  public static class ExtensionMethods
  {

    #region Methods related to linkable component

    /// <summary>
    /// Similar to the "old"/deprecated initialise method for an <see cref="IBaseLinkableComponent"/>.
    /// <para>
    /// It will call first <see cref="ApplyArguments"/>, and then call <see cref="IBaseLinkableComponent.Initialize()"/>
    /// </para>
    /// </summary>
    /// <param name="component">Component to initialise</param>
    /// <param name="arguments">Arguments to set to the component</param>
    public static void Initialize(this IBaseLinkableComponent component, IList<IArgument> arguments)
    {
      component.ApplyArguments(arguments);
      component.Initialize();
    }

    /// <summary>
    /// Apply the arguments in the <paramref name="arguments"/> list to the <paramref name="component"/>.
    /// <para>
    /// It searches through all the <see cref="IBaseLinkableComponent.Arguments"/> of the
    /// <paramref name="component"/> and sets the value for those with id's from the 
    /// <paramref name="arguments"/>.
    /// </para>
    /// <para>
    /// If an argument in the <paramref name="arguments"/> list comes with an id that does not
    /// exist in the arguments of the component, an exception is thrown.
    /// </para>
    /// </summary>
    /// <param name="component">Component to set arguments to</param>
    /// <param name="arguments">Arguments to set</param>
    public static void ApplyArguments(this IBaseLinkableComponent component, IList<IArgument> arguments)
    {
      component.Arguments.ApplyArguments(arguments);
    }

    /// <summary>
    /// Similar to the "old"/deprecated TimeExtent method for an <see cref="ITimeSpaceComponent"/>.
    /// <para>
    /// If the component is not a time space version, this will throw an exception.
    /// </para>
    /// </summary>
    /// <param name="component">Component to get time extent for</param>
    public static ITimeSet TimeExtent(this IBaseLinkableComponent component)
    {
      ITimeSpaceComponent tsComponent = component as ITimeSpaceComponent;
      if (tsComponent != null)
        return (tsComponent.TimeExtent);
      throw new NotSupportedException("The component needs to be an ITimeSpaceComponent");
    }

    #endregion

    #region Methods related to exchange items

    /// <summary>
    /// Get the value definition as a quantity, throwing an
    /// exception if it is not a quantity
    /// </summary>
    public static IQuantity Quantity(this IBaseExchangeItem item)
    {
      IQuantity quantity = item.ValueDefinition as IQuantity;
      if (quantity == null)
        throw new Exception("ValueDefinition is not a quantity");
      return (quantity);
    }

    /// <summary>
    /// Get the value definition as a quality, throwing an
    /// exception if it is not a quality
    /// </summary>
    public static IQuality Quality(this IBaseExchangeItem item)
    {
      IQuality quality = item.ValueDefinition as IQuality;
      if (quality == null)
        throw new Exception("ValueDefinition is not a quality");
      return (quality);
    }

    /// <summary>
    /// Get the <see cref="IElementSet"/> from the <paramref name="baseitem"/>,
    /// assuming it is a <see cref="ITimeSpaceExchangeItem"/> with an <see cref="IElementSet"/>
    /// as its <see cref="ITimeSpaceExchangeItem.SpatialDefinition"/>. If not, exceptions
    /// are thrown.
    /// </summary>
    public static IElementSet ElementSet(this IBaseExchangeItem baseitem)
    {
      ITimeSpaceExchangeItem item = baseitem as ITimeSpaceExchangeItem;
      if (item == null)
        throw new Exception("base item is not an ITimeSpaceExchangeItem");
      return (item.ElementSet());
    }

    /// <summary>
    /// Get the <see cref="IElementSet"/> from the <paramref name="item"/>,
    /// assuming the <see cref="ITimeSpaceExchangeItem.SpatialDefinition"/> is
    /// an <see cref="IElementSet"/>. If not, an exception is thrown.
    /// </summary>
    public static IElementSet ElementSet(this ITimeSpaceExchangeItem item)
    {
      if (item.SpatialDefinition == null)
        return (null);
      IElementSet elmtSet = item.SpatialDefinition as IElementSet;
      if (elmtSet != null)
        return (elmtSet);
      throw new NotSupportedException("The SpatialDefinition needs currently to be an IElementSet");
    }

    #endregion

    #region Methods related to ITime

    /// <summary>
    /// Adds a number of days to the Time
    /// </summary>
    public static ITime AddDays(this ITime time, double days)
    {
      return (new Time(time.StampAsModifiedJulianDay + days, time.DurationInDays));
    }

    /// <summary>
    /// Adds a number of seconds to the Time
    /// </summary>
    public static ITime AddSeconds(this ITime time, double seconds)
    {
      return (new Time(time.StampAsModifiedJulianDay + seconds / 24.0 / 3600.0, time.DurationInDays));
    }

    /// <summary>
    /// Converts the time stamp to DateTime
    /// </summary>
    /// <returns>The DateTime</returns>
    public static DateTime ToDateTime(this ITime time)
    {
      return (Time.ToDateTime(time.StampAsModifiedJulianDay));
    }

    /// <summary>
    /// Return the time as a modified julian date
    /// </summary>
    public static double ToModifiedJulianDay(this DateTime dateTime)
    {
      long ticks = dateTime.Ticks - Time.ModifiedJulianDateZeroTicks;
      return ticks / ((double)TimeSpan.TicksPerDay);
    }

    /// <summary>
    /// Return the start time of a time span
    /// </summary>
    public static ITime Start(this ITime time)
    {
      return (new Time(time.StampAsModifiedJulianDay));
    }

    /// <summary>
    /// Returns the end time of a time span, or the time in case of a time stamp
    /// </summary>
    public static ITime End(this ITime time)
    {
      return (new Time(time.StampAsModifiedJulianDay + time.DurationInDays));
    }

    /// <summary>
    /// Returns the end time of a time span, or the time in case of a time stamp
    /// </summary>
    public static double EndStampAsModifiedJulianDay(this ITime time)
    {
      return (time.StampAsModifiedJulianDay + time.DurationInDays);
    }

    #endregion

    #region Methods related to time set

    /// <summary>
    /// Set a single time stamp or span in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="timeStamp">Time to use</param>
    public static void SetSingleTime(this ITimeSet timeSet, ITime timeStamp)
    {
      if (timeStamp.DurationInDays == 0 && timeSet.HasDurations)
      {
        throw new Exception("Can not set single time STAMP in a time set with duration");
      }
      else if (timeStamp.DurationInDays > 0 && !timeSet.HasDurations)
      {
        throw new Exception("Can not set single time SPAN in a time set without duration");
      }


      if (timeSet.Times.Count > 1)
        timeSet.Times.Clear();
      if (timeSet.Times.Count == 0)
      {
        timeSet.Times.Add(timeStamp);
      }
      else
      {
        timeSet.Times[0] = timeStamp;
      }
    }

    /// <summary>
    /// Set a single time stamp in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="timestampAsMJD">Time to use</param>
    public static void SetSingleTimeStamp(this ITimeSet timeSet, double timestampAsMJD)
    {
      timeSet.SetSingleTime(new Time(timestampAsMJD));
    }

    /// <summary>
    /// Set a single time stamp in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="dateTime">Time to use</param>
    public static void SetSingleTimeStamp(this ITimeSet timeSet, DateTime dateTime)
    {
      timeSet.SetSingleTime(new Time(dateTime));
    }

    /// <summary>
    /// Set a single time stamp in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="dateTime">Time to use</param>
    /// <param name="durationInDays">Duration of span</param>
    public static void SetSingleTimeSpan(this ITimeSet timeSet, DateTime dateTime, double durationInDays)
    {
      timeSet.SetSingleTime(new Time(dateTime, durationInDays));
    }

    /// <summary>
    /// Set a single time stamp in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="startAsMJD">Start Time as Modified Julian Day</param>
    /// <param name="endAsMJD">End Time as Modified Julian Day</param>
    public static void SetSingleTimeSpan(this ITimeSet timeSet, double startAsMJD, double endAsMJD)
    {
      timeSet.SetSingleTime(new Time(startAsMJD, endAsMJD - startAsMJD));
    }

    /// <summary>
    /// Set a single time stamp in the <see cref="ITimeSet"/>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="startTime">Start Time as Modified Julian Day</param>
    /// <param name="endTime">End Time as Modified Julian Day</param>
    public static void SetSingleTimeSpan(this ITimeSet timeSet, ITime startTime, ITime endTime)
    {
      timeSet.SetSingleTimeSpan(startTime.StampAsModifiedJulianDay, endTime.StampAsModifiedJulianDay + endTime.DurationInDays);
    }

    /// <summary>
    /// Set the time horizon for an <see cref="ITimeSet"/>, assuming it is a
    /// <see cref="TimeSet"/>, if not, an exception is thrown.
    /// <para>
    /// Convenience methods, since the <see cref="ITimeSet.TimeHorizon"/> does
    /// not have any setter.
    /// </para>
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="timeHorizon">Time horizon to use</param>
    public static void SetTimeHorizon(this ITimeSet timeSet, ITime timeHorizon)
    {
      if (!(timeSet is TimeSet))
      {
        throw new Exception("SetTimeHorizon: TimeSet is not of type Backbone.TimeSet");
      }
      ((TimeSet)timeSet).TimeHorizon = timeHorizon;
    }

    /// <summary>
    /// Set the time horizon start time for an <see cref="ITimeSet"/>, assuming it is a
    /// <see cref="TimeSet"/>, if not, an exception is thrown. End time is unchanged, 
    /// i.e. the duration will change.
    /// </summary>
    /// <param name="timeSet">Time set to update</param>
    /// <param name="timeHorizonStart">New time horizon start time</param>
    public static void SetTimeHorizonStart(this ITimeSet timeSet, ITime timeHorizonStart)
    {
      double startMjd = timeHorizonStart.StampAsModifiedJulianDay;
      double endMjd = timeSet.TimeHorizon.EndStampAsModifiedJulianDay();
      ITime newHorizon = new Time(startMjd, endMjd - startMjd);
      timeSet.SetTimeHorizon(newHorizon);
    }
    #endregion

    #region Methods related to value sets

    /// <summary>
    /// Return the number of elements in the <see cref="ITimeSpaceValueSet"/>
    /// </summary>
    public static int ElementCount(this ITimeSpaceValueSet values)
    {
      return (ValueSet.GetElementCount(values));
    }

    /// <summary>
    /// Return the number of times in the <see cref="ITimeSpaceValueSet"/>
    /// </summary>
    public static int TimesCount(this ITimeSpaceValueSet values)
    {
      return (ValueSet.GetTimesCount(values));
    }

    /// <summary>
    /// Returns an array of T's from the <paramref name="values"/>, extracting
    /// data from the <paramref name="timeIndex"/> - in principle a typed version
    /// of <see cref="ITimeSpaceValueSet.GetElementValuesForTime"/> returning
    /// an array.
    /// </summary>
    /// <example>
    /// <code>
    /// ITimeSpaceValueSet values = ...
    /// double[] doubleArray = values.GetElementValuesForTime<double>(0);
    /// </code>
    /// </example>
    /// <param name="values">Values to retrieve</param>
    /// <param name="timeIndex">Index to retrieve data from.</param>
    /// <returns>Array of T</returns>
    public static T[] GetElementValuesForTime<T>(this ITimeSpaceValueSet values, int timeIndex)
    {
      IList elmtValues = values.GetElementValuesForTime(timeIndex);

      // Check if it is already an array
      T[] tArray = elmtValues as T[];
      if (tArray != null)
        return tArray;

      // Check if it is a generic list, and use C# build in extension method
      IList<T> tList = elmtValues as IList<T>;
      if (tList != null)
        return (tList.ToArray());

      // Do a manual copy (an IList which is not an IList<T>)
      tArray = new T[elmtValues.Count];
      for (int i = 0; i < elmtValues.Count; i++)
      {
        tArray[i] = (T)elmtValues[i];
      }

      return (tArray);
    }

    /// <summary>
    /// Multiply all element values in the value set with the matching factors in the factors array.
    /// <para>Assuming the value set contains doubles, and the number of elements in the value 
    /// set matches the number of factors</para>
    /// </summary>
    /// <param name="sourceValueset">Valueset to multiply</param>
    /// <param name="factors">Factors to multiply element values with</param>
    /// <returns>A new value set where values have been multiplied</returns>
    public static ITimeSpaceValueSet MultiplyElementValues(this ITimeSpaceValueSet sourceValueset, double[] factors)
    {
      IList<IList> sourceValues = sourceValueset.Values2D;
      ValueSetArray<double> targetValueSet = new ValueSetArray<double>();
      ListArray<double> targetValues = targetValueSet.Values2DArray;

      // Loop over time
      for (int i = 0; i < sourceValues.Count; i++)
      {
        // Multiply every element value with the corresponding factor
        IList<double> sourceElmtValues = (IList<double>)sourceValues[i];
        double[] targetElmtValues = new double[sourceElmtValues.Count];
        for (int j = 0; j < sourceElmtValues.Count; j++)
        {
          targetElmtValues[j] = sourceElmtValues[j] * factors[j];
        }
        targetValues.Add(targetElmtValues);
      }
      return (targetValueSet);
    }



    #endregion

    #region Methods related to list of arguments, IArgumnet[] or IList<IArgument>

    /// <summary>
    /// Creates a dictionary of the argument array, indexing each argument
    /// on its Id.
    /// </summary>
    /// <param name="array">Array input argument</param>
    /// <returns>A dictionary indexed by the argument Id's</returns>
    public static IDictionary<string, IArgument> Dictionary(this IList<IArgument> array)
    {
      Dictionary<string, IArgument> dict = new Dictionary<string, IArgument>(array.Count, StringComparer.OrdinalIgnoreCase);
      foreach (IArgument argument in array)
      {
        dict.Add(argument.Id, argument);
      }
      return (dict);
    }

    /// <summary>
    /// Apply the arguments in the <paramref name="newValues"/> list to the <paramref name="arguments"/>.
    /// <para>
    /// It searches through all the <paramref name="arguments"/> and sets the value for those with id's from the 
    /// <paramref name="newValues"/>.
    /// </para>
    /// <para>
    /// If an argument in the <paramref name="newValues"/> list comes with an id that does not
    /// exist in the <paramref name="arguments"/>, an exception is thrown.
    /// </para>
    /// </summary>
    /// <param name="arguments">Arguments to update</param>
    /// <param name="newValues">New argument values to set</param>
    public static void ApplyArguments(this IList<IArgument> arguments, IList<IArgument> newValues)
    {
      IDictionary<string, IArgument> componentArgs = arguments.Dictionary();

      foreach (IArgument inarg in newValues)
      {
        IArgument componentArg;
        if (!componentArgs.TryGetValue(inarg.Id, out componentArg))
        {
            //throw new Exception(string.Format("Argument with id '{0}' not present in component", inarg.Id));
        }
        else
        {
            //if (componentArg.Value.GetType() == inarg.Value.GetType())
            componentArg.Value = inarg.Value;
        }
      }
    }

    /// <summary>
    /// Return the value of the argument with the specified id, assuming it has a value 
    /// of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="arguments">Dictionary of arguments to search in</param>
    /// <param name="id">Id of argument to search for</param>
    /// <returns>Value of argument</returns>
    public static T GetValue<T>(this IDictionary<string, IArgument> arguments, string id)
    {
      IArgument value;
      if (!arguments.TryGetValue(id, out value))
        throw new Exception(string.Format("Unknown argument id: '{0}'", id));
      if (value.ValueType != typeof(T))
        throw new Exception(string.Format("Type mismatch for argument of id: '{0}'", id));

      return ((T)value.Value);
    }

    /// <summary>
    /// Update the value of the argument with the specified id, assuming it has a value 
    /// of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="arguments">Dictionary of arguments to search in</param>
    /// <param name="id">Id of argument to search for</param>
    /// <param name="newValue">New value to set</param>
    /// <returns>Existing value of argument</returns>
    public static T UpdateValue<T>(this IDictionary<string, IArgument> arguments, string id, T newValue)
    {
      IArgument value;
      if (!arguments.TryGetValue(id, out value))
        throw new Exception(string.Format("Unknown argument id: '{0}'", id));
      if (value.ValueType != typeof(T))
        throw new Exception(string.Format("Type mismatch for argument of id: '{0}'", id));

      T oldValue = (T)value.Value;
      value.Value = newValue;

      return (oldValue);
    }

    /// <summary>
    /// Update the value of the argument with the specified id, assuming it has a value 
    /// of type <typeparamref name="T"/>.
    /// <para>
    /// If there are many arguments, and many needs to be updated, consider instead using
    /// <code>
    /// var dictArguments = arguments.Dictionary();
    /// dictArguments.UpdateValue(is, new Value)
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="arguments">List of arguments to search in</param>
    /// <param name="id">Id of argument to search for</param>
    /// <param name="newValue">New value to set</param>
    /// <returns>Existing value of argument</returns>
    public static T UpdateValue<T>(this IList<IArgument> arguments, string id, T newValue)
    {
      foreach (IArgument argument in arguments)
      {
        if (string.Equals(argument.Id, id, StringComparison.OrdinalIgnoreCase))
        {
          if (argument.ValueType != typeof(T))
            throw new Exception(string.Format("Type mismatch for argument of id: '{0}'", id));
          T oldValue = (T)argument.Value;
          argument.Value = newValue;
          return oldValue;
        }
      }
      throw new Exception(string.Format("Unknown argument id: '{0}'", id));
    }

    #endregion

    #region Methods related to searching for exchange items and adaptors, based on their id

    /// <summary>
    /// Find the input item with the provided id.
    /// </summary>
    /// <param name="component">Component to find input item in</param>
    /// <param name="id">Id of input item to search for</param>
    /// <returns>Returns input item with id, or null if not found</returns>
    public static ITimeSpaceInput FindInputItem(this ITimeSpaceComponent component, string id)
    {
      foreach (IBaseInput input in component.Inputs)
      {
        if (string.Equals(input.Id, id, StringComparison.OrdinalIgnoreCase))
        {
          ITimeSpaceInput timeSpaceInput = input as ITimeSpaceInput;
          if (timeSpaceInput == null)
            throw new Exception("Found exchange item, but it was not a TimeSpaceInput, id = '" + id + "'");
          return (timeSpaceInput);
        }
      }
      return null;
      //throw new Exception("Could not find input item with id = '" + id + "'");
    }

    /// <summary>
    /// Find the output item with the provided id.
    /// </summary>
    /// <param name="component">Component to find output item in</param>
    /// <param name="id">Id of output item to search for</param>
    /// <returns>Returns output item with id, or null if not found</returns>
    public static ITimeSpaceOutput FindOutputItem(this ITimeSpaceComponent component, string id)
    {
      foreach (IBaseOutput input in component.Outputs)
      {
        if (string.Equals(input.Id, id, StringComparison.OrdinalIgnoreCase))
        {
          ITimeSpaceOutput timeSpaceInput = input as ITimeSpaceOutput;
          if (timeSpaceInput == null)
            throw new Exception("Found exchange item, but it was not a TimeSpaceOutput, id = '" + id + "'");
          return (timeSpaceInput);
        }
      }
      return null;
      //throw new Exception("Could not find output item with id = '" + id + "'");
    }


    /// <summary>
    /// Find the adaptor with the provided id.
    /// </summary>
    /// <param name="component">Component to find adaptor in</param>
    /// <param name="adaptorId">Id of adaptor to search for</param>
    /// <param name="output">Output item to find adaptor for.</param>
    /// <param name="input">Input item to find adaptor for. Can be null</param>
    /// <returns>Returns adaptor with id, or null if not found</returns>
    public static ITimeSpaceAdaptedOutput FindAdaptor(this ITimeSpaceComponent component, string adaptorId, ITimeSpaceOutput output, ITimeSpaceInput input)
    {
      foreach (IAdaptedOutputFactory adaptorFactory in component.AdaptedOutputFactories)
      {
        foreach (IIdentifiable availId in adaptorFactory.GetAvailableAdaptedOutputIds(output, input))
        {
          if (string.Equals(availId.Id, adaptorId, StringComparison.OrdinalIgnoreCase))
          {
            IBaseAdaptedOutput adaptor = adaptorFactory.CreateAdaptedOutput(availId, output, input);
            return adaptor as ITimeSpaceAdaptedOutput;
          }
        }
      }

      return null;
      //throw new Exception("Could not find adaptor with id = '" + id + "'");
    }

    /// <summary>
    /// Find the adaptor with the provided id.
    /// </summary>
    /// <param name="component">Component to find adaptor in</param>
    /// <param name="factoryId">Id of factory to search in, null to search all factories</param>
    /// <param name="adaptorId">Id of adaptor to search for</param>
    /// <param name="output">Output item to find adaptor for.</param>
    /// <param name="input">Input item to find adaptor for. Can be null</param>
    /// <returns>Returns adaptor with id, or null if not found</returns>
    public static ITimeSpaceAdaptedOutput FindAdaptor(this ITimeSpaceComponent component, string factoryId, string adaptorId, ITimeSpaceOutput output, ITimeSpaceInput input)
    {
      foreach (IAdaptedOutputFactory adaptorFactory in component.AdaptedOutputFactories)
      {
        // Check the id of the factory
        if (factoryId != null && !string.Equals(factoryId, adaptorFactory.Id, StringComparison.OrdinalIgnoreCase))
          continue;
        foreach (IIdentifiable availId in adaptorFactory.GetAvailableAdaptedOutputIds(output, input))
        {
          // Check the id of the adaptor
          if (string.Equals(availId.Id, adaptorId, StringComparison.OrdinalIgnoreCase))
          {
            IBaseAdaptedOutput adaptor = adaptorFactory.CreateAdaptedOutput(availId, output, input);
            return adaptor as ITimeSpaceAdaptedOutput;
          }
        }
      }

      return null;
      //throw new Exception("Could not find adaptor with id = '" + id + "'");
    }
    #endregion

  }
}
