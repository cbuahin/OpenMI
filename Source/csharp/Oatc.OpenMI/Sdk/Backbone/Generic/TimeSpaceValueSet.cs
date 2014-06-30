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
using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone.Generic
{
  /// <summary>
  /// Generic version of the <see cref="ITimeSpaceValueSet"/>, having a specified return type
  /// </summary>
  /// <typeparam name="T">Type of values in the value set</typeparam>
  public interface ITimeSpaceValueSet<T> : ITimeSpaceValueSet
  {
    new IList<IList<T>> Values2D { get; set; }

    new T GetValue(int timeIndex, int elementIndex);
    void SetValue(T value, int timeIndex, int elementIndex);

    new IList<T> GetTimeSeriesValuesForElement(int elementIndex);
    void SetTimeSeriesValuesForElement(IList<T> values, int elementIndex);

    new IList<T> GetElementValuesForTime(int timeIndex);
    void SetElementValuesForTime(IList<T> values, int timeIndex);
  }



  /// <summary>
  /// An <see cref="ITimeSpaceValueSet{T}"/> implementation.
  /// </summary>
  /// <remarks>
  /// Values are stored based on an <see cref="ListIList{T}"/>.
  /// </remarks>
  public class TimeSpaceValueSet<T> : TimeSpaceValueSetBase<T> , ITimeSpaceValueSet<T>
  {
    protected ListIList<T> _values2D;

    public TimeSpaceValueSet()
      : this(new List<IList<T>>())
    {

    }

    /// <summary>
    /// Create a valueset based on the provided values.
    /// </summary>
    public TimeSpaceValueSet(IList<IList<T>> values)
    {
      _values2D = new ListIList<T>(values.Count);
      for (int i = 0; i < values.Count; i++)
      {
        var ilist = new List<T>(values[i].Count);
        foreach (T value in values[i])
        {
          ilist.Add(value);
        }
        _values2D.Add(ilist);
      }
    }

    /// <summary>
    /// Create a valueset with one time step and a number of element values. 
    /// </summary>
    public TimeSpaceValueSet(IEnumerable<T> values1D)
    {
      _values2D = new ListIList<T>(1) { new List<T>() };
      foreach (T value in values1D)
      {
        _values2D[0].Add(value);
      }
    }

    public override IList<IList<T>> Values2D
    {
      get { return _values2D; }
      set
      {
        _values2D = value as ListIList<T>;
        if (_values2D != null)
          return;

        // Copy values
        _values2D = new ListIList<T>(value.Count);
        foreach (IList<T> list in value)
        {
          T[] array = new T[list.Count];
          for (int i = 0; i < list.Count; i++)
          {
            array[i] = list[i];
          }
          _values2D.Add(array);
        }
      }
    }

    IList<IList> ITimeSpaceValueSet.Values2D
    {
      get { return (_values2D); }
      set
      {
        var list = value as IList<IList<T>>;
        if (list != null)
        {
          Values2D = list;
          return;
        }
        throw new ArgumentException("Argument does not have correct type to use in generic class", "value");
      }
    }

    public IList<T> this[int index]
    {
      get { return _values2D[index]; }
      set { _values2D[index] = value; }
    }

  }

  /// <summary>
  /// An <see cref="ITimeSpaceValueSet{T}"/> implementation.
  /// </summary>
  /// <remarks>
  /// Values are stored based on an <see cref="ListArray{T}"/>.
  /// </remarks>
  public class ValueSetArray<T> : TimeSpaceValueSetBase<T> , ITimeSpaceValueSet<T>
  {
    private ListArray<T> _values2D;

    public ValueSetArray()
    {
      _values2D = new ListArray<T>();
    }

    /// <summary>
    /// Create a valueset based on the provided values.
    /// </summary>
    public ValueSetArray(IList<IList<T>> values)
    {
      _values2D = new ListArray<T>(values.Count);
      for (int i = 0; i < values.Count; i++)
      {
        _values2D.Add(new T[values[i].Count]);
        int numElmts = values[i].Count;
        for (int j = 0; j < numElmts; j++)
        {
          _values2D[i][j] = values[i][j];
        }
      }
    }

    /// <summary>
    /// Create a valueset based on the provided values.
    /// </summary>
    public ValueSetArray(ListArray<T> values)
    {
      _values2D = values;
    }

    /// <summary>
    /// Create a valueset with one time step and a number of element values. 
    /// </summary>
    public ValueSetArray(ICollection<T> elementValues)
    {
      T[] elementArray = new T[elementValues.Count];
      int i = 0;
      foreach (T value in elementValues)
      {
        elementArray[i++] = value;
      }
      _values2D = new ListArray<T>(1) { elementArray };
    }

    public override IList<IList<T>> Values2D
    {
      get { return _values2D; }
      set
      {
        _values2D = value as ListArray<T>;
        if (_values2D != null)
          return;

        // Copy values
        _values2D = new ListArray<T>(value.Count);
        foreach (IList<T> list in value)
        {
          T[] array = new T[list.Count];
          for (int i = 0; i < list.Count; i++)
          {
            array[i] = list[i];
          }
          _values2D.Add(array);
        }
      }
    }

    IList<IList> ITimeSpaceValueSet.Values2D
    {
      get { return (_values2D); }
      set
      {
        var list = value as IList<IList<T>>;
        if (list != null)
        {
          Values2D = list;
          return;
        }
        throw new ArgumentException("Argument does not have correct type to use in generic class", "value");
      }
    }

    public ListArray<T> Values2DArray
    {
      get { return _values2D; }
      set { _values2D = value; }
    }

    public T[] this[int index]
    {
      get { return _values2D[index]; }
      set { _values2D[index] = value; }
    }

  }

  /// <summary>
  /// Abstract base implementation for a <see cref="ITimeSpaceValueSet{T}"/>.
  /// </summary>
  public abstract class TimeSpaceValueSetBase<T> : ITimeSpaceValueSet<T>
  {
    public abstract IList<IList<T>> Values2D { get; set; }

    public Type ValueType
    {
      get { return (typeof(T)); }
    }

    // Must be overwritten by extending class (can not be declared abstract!)
    IList<IList> ITimeSpaceValueSet.Values2D
    {
      get { throw new NotImplementedException("Must be implemented in extending class"); }
      set { throw new NotImplementedException("Must be implemented in extending class"); }
    }

    public T this[int timeIndex, int elementIndex]
    {
      get { return GetValue(timeIndex, elementIndex); }
      set { SetValue(value, timeIndex, elementIndex); }
    }

    public T GetValue(int timeIndex, int elementIndex)
    {
      ValueSet.CheckTimeIndex(this, timeIndex);
      ValueSet.CheckElementIndex(this, elementIndex);
      return Values2D[timeIndex][elementIndex];
    }

    public void SetValue(T value, int timeIndex, int elementIndex)
    {
      ValueSet.CheckTimeIndex(this, timeIndex);
      ValueSet.CheckElementIndex(this, elementIndex);
      Values2D[timeIndex][elementIndex] = value;
    }

    public IList<T> GetTimeSeriesValuesForElement(int elementIndex)
    {
      ValueSet.CheckElementIndex(this, elementIndex);

      IList<T> values = new List<T>(Values2D.Count);
      for (int timeIndex = 0; timeIndex < Values2D.Count; timeIndex++)
      {
        values.Add(Values2D[timeIndex][elementIndex]);
      }
      return values;
    }

    public void SetTimeSeriesValuesForElement(IList<T> values, int elementIndex)
    {
      ValueSet.CheckElementIndex(this, elementIndex);
      for (int timeIndex = 0; timeIndex < Values2D.Count; timeIndex++)
      {
        Values2D[timeIndex][elementIndex] = values[elementIndex];
      }
    }

    public IList<T> GetElementValuesForTime(int timeIndex)
    {
      ValueSet.CheckTimeIndex(this, timeIndex);
      return Values2D[timeIndex];
    }

    public void SetElementValuesForTime(IList<T> values, int timeIndex)
    {
      ValueSet.CheckElementIndex(this, timeIndex);
      Values2D[timeIndex] = values;
    }

    object ITimeSpaceValueSet.GetValue(int timeIndex, int elementIndex)
    {
      return GetValue(timeIndex, elementIndex);
    }

    void ITimeSpaceValueSet.SetValue(int timeIndex, int elementIndex, object value)
    {
      if (value is T)
        SetValue((T)value, timeIndex, elementIndex);
      else
        throw new ArgumentException("Argument does not have correct type to use in generic class", "value");
    }

    IList ITimeSpaceValueSet.GetElementValuesForTime(int timeIndex)
    {
      return (new ListWrapper<T>(GetElementValuesForTime(timeIndex)));
    }

    public void SetElementValuesForTime(int timeIndex, IList values)
    {
      if (values is IList<T>)
        SetElementValuesForTime((IList<T>)values, timeIndex);
      else
        throw new ArgumentException("Argument does not have correct type to use in generic class", "values");
    }

    IList ITimeSpaceValueSet.GetTimeSeriesValuesForElement(int elementIndex)
    {
      return (new ListWrapper<T>(GetTimeSeriesValuesForElement(elementIndex)));
    }

    public void SetTimeSeriesValuesForElement(int elementIndex, IList values)
    {
      if (values is IList<T>)
        SetTimeSeriesValuesForElement((IList<T>)values, elementIndex);
      else
        throw new ArgumentException("Argument does not have correct type to use in generic class", "values");
    }


    int IBaseValueSet.NumberOfIndices
    {
      get { return (2); }
    }

    int IBaseValueSet.GetIndexCount(int[] indices)
    {
      if (indices == null || indices.Length == 0)
      {
        return (Values2D.Count);
      }
      if (indices.Length > 1)
        throw new ArgumentException("Indices does not have the correct length, length must be smaller than 2", "indices");
      return (Values2D[indices[0]].Count);
    }

    object IBaseValueSet.GetValue(int[] indices)
    {
      if (indices.Length != 2)
        throw new ArgumentException("Indices does not have the correct length", "indices");
      return (GetValue(indices[0], indices[1]));
    }

    void IBaseValueSet.SetValue(int[] indices, object value)
    {
      if (indices.Length != 2)
        throw new ArgumentException("Indices does not have the correct length", "indices");
      if (!(value is T))
        throw new ArgumentException("value is not of the correct type", "value");
      SetValue((T)value, indices[0], indices[1]);
    }
  }

}
