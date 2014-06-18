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
using System.Collections;
using System.Collections.Generic;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
    /// <summary>
    /// <para>
    /// Wrapping of an <see cref="IList{T}"/> of <typeparamref name="T"/>
    /// to also implement an <see cref="IList"/>.
    /// </para>
    /// <para>
    /// This will make the list available as a non-generic list, without copying the
    /// content of the list. 
    /// </para>
    /// <para>
    /// The wrapper supports all read functionality, while only partly "write" functionality. 
    /// For example, adding to the list an element which is not of type 
    /// <typeparamref name="T"/> will throw an exception.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This is required since an <see cref="IList{T}"/> does not extend an <see cref="IList"/>
    /// </remarks>
    public class ListWrapper<T> : IList, IList<T>
    {
        private readonly IList<T> _list;

        public ListWrapper(IList<T> list)
        {
            _list = list;
        }

        public IList<T> WrappedList
        {
            get { return (_list); }
        }

        public int Count
        {
            get { return (_list.Count); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (_list.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        int IList.Add(object item)
        {
            if (item is T)
            {
                int count = _list.Count;
                _list.Add((T)item);
                return count;
            }
            throw new NotSupportedException("Type mismatch in ListWrapper");
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        void IList.Insert(int index, object item)
        {
            if (item is T)
            {
                _list.Insert(index, (T)item);
            }
            else
            {
                throw new NotSupportedException("Type mismatch in ListWrapper");
            }
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return (_list.Contains(item));
        }

        bool IList.Contains(object item)
        {
            if (item is T)
                _list.Contains((T)item);
            return (false);
        }

        public bool Remove(T item)
        {
            return (_list.Remove(item));
        }

        void IList.Remove(object item)
        {
            if (item is T)
            {
                _list.Remove((T)item);
            }
        }

        public void CopyTo(Array array, int index)
        {
            // This can be optimized
            T[] t1Array = new T[array.Length];
            _list.CopyTo(t1Array, index);
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(t1Array[i], i);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return (_list.IndexOf(item));
        }

        int IList.IndexOf(object item)
        {
            if (item is T)
            {
                return (_list.IndexOf((T)item));
            }
            return (-1);
        }

        public T this[int index]
        {
            get { return (_list[index]); }
            set { _list[index] = value; }
        }

        object IList.this[int index]
        {
            get { return (_list[index]); }
            set
            {
                if (value is T)
                    _list[index] = (T)value;
                else
                    throw new NotSupportedException("Type mismatch in ListWrapper");
            }
        }

        public bool IsReadOnly
        {
            get { return (_list.IsReadOnly); }
        }

        object ICollection.SyncRoot
        {
            get { return (_list); }
        }

        bool ICollection.IsSynchronized
        {
            get { return (false); }
        }

        bool IList.IsFixedSize
        {
            get { return (false); }
        }

    }
    
    
    /// <summary>
    /// <para>
    /// Wrapping of an IList{T} to an IList{TBase}, 
    /// when <typeparamref name="T"/> extends <typeparamref name="TBase"/>.
    /// </para>
    /// <para>
    /// This will make the list available as a list with base class elements, without copying the
    /// content of the list. 
    /// </para>
    /// <para>
    /// The wrapper supports all read functionality, while only partly "write" functionality. 
    /// For example, adding to the list an element which is of type 
    /// <typeparamref name="TBase"/> and not of <typeparamref name="T"/> will throw an exception.
    /// </para>
    /// </summary>
    /// <example>
    /// Assuming having two classes where
    /// <code> C : CBase </code>
    /// When an <code>IList{CBase}</code> is required, but what is available is an
    /// <code>IList{C}</code>, then these are not compatible. The wrapper can be used
    /// to 
    /// <code>
    /// IList{C} CList = ...;
    /// IList{CBase} CBaseList = new ListWrapper{C,CBase}(CList);
    /// </code>
    /// </example>
    /// <remarks>
    /// This is required since an <see cref="IList{T}"/> does not extend an <see cref="IList{TBase}"/>
    /// even though T extends TBase
    /// </remarks>
    /// <typeparam name="T">Type extending <typeparamref name="TBase"/></typeparam>
    /// <typeparam name="TBase">Base type</typeparam>
    public class ListWrapper<T, TBase> : IList<TBase> where T : TBase
    {
        private readonly IList<T> _list;

        public ListWrapper(IList<T> list)
        {
            _list = list;
        }

        public IList<T> WrappedList
        {
            get { return _list; }
        }

        public IEnumerator<TBase> GetEnumerator()
        {
            return (new EnumeratorWrapper<T, TBase>(_list.GetEnumerator()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TBase item)
        {
            if (item is T)
            {
                _list.Add((T)item);
                return;
            }
            throw new NotSupportedException("Type mismatch in ListWrapper");
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TBase item)
        {
            if (item is T)
                _list.Contains((T)item);
            return (false);
        }

        public void CopyTo(TBase[] array, int arrayIndex)
        {
            // This can be optimized
            T[] t1Array = new T[array.Length];
            _list.CopyTo(t1Array, arrayIndex);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = t1Array[i];
            }
        }

        public bool Remove(TBase item)
        {
            if (item is T)
            {
                return (_list.Remove((T)item));
            }
            return (false);
        }

        public int Count
        {
            get { return (_list.Count); }
        }

        public bool IsReadOnly
        {
            get { return (_list.IsReadOnly); }
        }

        public int IndexOf(TBase item)
        {
            if (item is T)
            {
                return (_list.IndexOf((T)item));
            }
            return (-1);
        }

        public void Insert(int index, TBase item)
        {
            if (item is T)
            {
                _list.Insert(index, (T)item);
            }
            else
            {
                throw new NotSupportedException("Type mismatch in ListWrapper");
            }
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public TBase this[int index]
        {
            get { return (_list[index]); }
            set
            {
                if (value is T)
                    _list[index] = (T)value;
                else
                    throw new NotSupportedException("Type mismatch in ListWrapper");
            }
        }
    }

    /// <summary>
    /// <para>
    /// Wrapping of an <see cref="IEnumerable{T}"/> of <typeparamref name="T"/>
    /// to an <see cref="IEnumerable{T}"/> of <typeparamref name="TBase"/>, 
    /// under the assumption that <typeparamref name="T"/> extends <typeparamref name="TBase"/>.
    /// </para>
    /// <para>
    /// This will make the enumerable available as having base class elements, without copying the
    /// content of the enumerable. 
    /// </para>
    /// </summary>
    /// <remarks> 
    /// This is required since an <see cref="IEnumerable{T}"/> does not extend an <see cref="IEnumerable{TBase}"/>
    /// even though T extends TBase
    /// </remarks>
    /// <typeparam name="T">Type extending <typeparamref name="TBase"/></typeparam>
    /// <typeparam name="TBase">Base type</typeparam>
    public class EnumerableWrapper<T, TBase> : IEnumerable<TBase> where T : TBase
    {
        private readonly IEnumerable<T> _enumerable;

        public EnumerableWrapper(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<TBase> GetEnumerator()
        {
            IEnumerator<T> enumerator = _enumerable.GetEnumerator();
            EnumeratorWrapper<T, TBase> wrapper = new EnumeratorWrapper<T, TBase>(enumerator);
            return (wrapper);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// <para>
    /// Wrapping of an <see cref="IEnumerator{T}"/> of <typeparamref name="T"/>
    /// to an <see cref="IEnumerator{T}"/> of <typeparamref name="TBase"/>, 
    /// under the assumption that <typeparamref name="T"/> extends <typeparamref name="TBase"/>.
    /// </para>
    /// <para>
    /// This will make the enumerator available as having base class elements, without copying the
    /// content of the enumerator. 
    /// </para>
    /// </summary>
    /// <remarks> 
    /// This is required since an <see cref="IEnumerator{T}"/> does not extend an <see cref="IEnumerator{TBase}"/>
    /// even though T extends TBase
    /// </remarks>
    /// <typeparam name="T">Type extending <typeparamref name="TBase"/></typeparam>
    /// <typeparam name="TBase">Base type</typeparam>
    public class EnumeratorWrapper<T, TBase> : IEnumerator<TBase> where T : TBase
    {
        private readonly IEnumerator<T> _enumerator;

        public EnumeratorWrapper(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return (_enumerator.MoveNext());
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public TBase Current
        {
            get { return (_enumerator.Current); }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }




    /*
    /// <summary>
    /// <para>
    /// Wrapping of a generic IList enumerable to an an <see cref="IEnumerable{IList}"/>
    /// </para>
    /// <para>
    /// This will make the enumerable available as having base class elements, without copying the
    /// content of the enumerable. 
    /// </para>
    /// </summary>
    public class ListEnumerableWrapper<T,TList> : IEnumerable<IList> where TList : IList<T>
    {
        private readonly IEnumerable<TList> _enumerable;

        public ListEnumerableWrapper(IEnumerable<TList> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<IList> GetEnumerator()
        {
            IEnumerator<TList> enumerator = _enumerable.GetEnumerator();
            ListEnumeratorWrapper<T,TList> wrapper = new ListEnumeratorWrapper<T,TList>(enumerator);
            return (wrapper);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// <para>
    /// Wrapping of a generic IList enumerable to an an <see cref="IEnumerable{IList}"/>
    /// </para>
    /// <para>
    /// This will make the enumerable available as having base class elements, without copying the
    /// content of the enumerable. 
    /// </para>
    /// </summary>
    public class ListEnumerableWrapper<T> : ListEnumerableWrapper<T, IList<T>>
    {
        public ListEnumerableWrapper(IEnumerable<IList<T>> enumerable) : base(enumerable)
        {
        }
    }
    */

    /// <summary>
    /// <para>
    /// Wrapping of a generic list enumerator to an <see cref="IEnumerator{IList}"/> or
    /// an <code>IEnumerator<IList<T>></code>
    /// </para>
    /// <para>
    /// This will make the enumerator available as another type of enumerator, without copying 
    /// the content of the enumerator. 
    /// </para>
    /// <para>
    /// The <typeparamref name="TList"/> can be any type implementing IList{T}, hence also a T[].
    /// </para>
    /// </summary>
    /// <remarks>
    /// These classes are required, since IEnumerator{IList{T}} does not extend IEnumerator{IList}.
    /// The class can also be used for any other type implementing IList{T}, i.e., exposing an
    /// IEnumerator{T[]} as an IEnumerator{IList{T}} and an IEnumerator{IList}
    /// </remarks>
    /// <typeparam name="T">Any type</typeparam>
    /// <typeparam name="TList">Any type implementing/extending IList{T}</typeparam>
    public class ListEnumeratorWrapper<T, TList> : IEnumerator<IList>, IEnumerator<IList<T>> where TList : IList<T>
    {
        private readonly IEnumerator<TList> _enumerator;

        public ListEnumeratorWrapper(IEnumerator<TList> enumerator)
        {
            _enumerator = enumerator;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return (_enumerator.MoveNext());
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public IList<T> Current
        {
            get { return (_enumerator.Current); }
        }

        IList IEnumerator<IList>.Current
        {
            get { return (new ListWrapper<T>(_enumerator.Current)); }
        }

        object IEnumerator.Current
        {
            get { return(_enumerator.Current); }
        }

    }

    /// <summary>
    /// <para>
    /// Wrapping of a generic IList enumerator to an <see cref="IEnumerator{IList}"/> or
    /// an <code>IEnumerator{IList{T}}</code>
    /// </para>
    /// <para>
    /// This will make the enumerator available as another type of enumerator, without copying 
    /// the content of the enumerator. 
    /// </para>
    /// </summary>
    /// <remarks>
    /// These classes are required, since IEnumerator{IList{T}} does not extend IEnumerator{IList}.
    /// The class can also be used for any other type implementing IList{T}, i.e., exposing an
    /// IEnumerator{T[]} as an IEnumerator{IList{T}} and an IEnumerator{IList}
    /// </remarks>
    public class ListEnumeratorWrapper<T> : ListEnumeratorWrapper<T, IList<T>>
    {
        public ListEnumeratorWrapper(IEnumerator<IList<T>> enumerator) : base(enumerator)
        {
        }
    }


}
