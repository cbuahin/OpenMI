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
using Oatc.OpenMI.Sdk.DevelopmentSupport;

namespace Oatc.OpenMI.Sdk.Backbone.Generic
{
    /// <summary>
    /// A List of <see cref="IList{T}"/> which also implements <see cref="IList{IList}"/>.
    /// 
    /// Can be used in <see cref="TimeSpaceValueSet{T}"/> for both the generic <see cref="TimeSpaceValueSet{T}.Values2D"/> 
    /// and non-generic <see cref="ValueSet.Values2D"/>
    /// </summary>
    /// <remarks>
    /// All the non-generic IList functions are implemented explicitly, i.e., can not be used directly
    /// from the <see cref="ListIList{T}"/> class. If some day the non-generic support is not needed, it
    /// should be possible to remove these functions immediately.
    /// </remarks>
    public class ListIList<T> : List<IList<T>>, IList<IList>
    {
        public ListIList() { }
        public ListIList(int count) : base(count) { }
        public ListIList(IList<T> elementValues) : base(1)
        {
            this.Add(elementValues);
        }

        IEnumerator<IList> IEnumerable<IList>.GetEnumerator()
        {
            return (new ListEnumeratorWrapper<T>(GetEnumerator()));
        }

        void ICollection<IList>.Add(IList item)
        {
            var Titem = item as IList<T>;
            if (Titem != null)
            {
                Add(Titem);
                return;
            }
            throw new ArgumentException("Type mismatch. Argument is not of type IList<T>", "item");
        }

        bool ICollection<IList>.Contains(IList item)
        {
            var Titem = item as IList<T>;
            if (Titem != null)
            {
                return (Contains(Titem));
            }
            return (false);
        }

        void ICollection<IList>.CopyTo(IList[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<IList>.Remove(IList item)
        {
            var Titem = item as IList<T>;
            if (Titem != null)
            {
                return (Remove(Titem));
            }
            return (false);
        }

        bool ICollection<IList>.IsReadOnly
        {
            get { return (false); }
        }

        int IList<IList>.IndexOf(IList item)
        {
            var Titem = item as IList<T>;
            if (Titem != null)
            {
                return (IndexOf(Titem));
            }
            return (-1);
        }

        void IList<IList>.Insert(int index, IList item)
        {
            var Titem = item as IList<T>;
            if (Titem != null)
            {
                Insert(index, Titem);
                return;
            }
            throw new ArgumentException("Type mismatch. Argument is not of type IList<T>", "item");
        }

        IList IList<IList>.this[int index]
        {
            get { return (new ListWrapper<T>(this[index])); }
            set
            {
                var Titem = value as IList<T>;
                if (Titem != null)
                {
                    this[index] = Titem;
                    return;
                }
                throw new ArgumentException("Type mismatch. Argument is not of type IList<T>", "value");
            }
        }
    }

    /// <summary>
    /// A List of arrays which implements both generic and non-generic version of <see cref="IList{IList}"/>.
    /// 
    /// Can be used in <see cref="TimeSpaceValueSet{T}"/> for both the generic <see cref="TimeSpaceValueSet{T}.Values2D"/> 
    /// and non-generic <see cref="ValueSet.Values2D"/>. Especially, it is used in <see cref="ValueSetArray{T}"/>
    /// </summary>
    /// <remarks>
    /// All the non-generic IList functions are implemented explicitly, i.e., can not be used directly
    /// from the <see cref="ListArray{T}"/> class. If some day the non-generic support is not needed, it
    /// should be possible to remove these functions immediately.
    /// 
    /// Same is the case with the generic IList functions, such that only the "true" functions are exposed
    /// directly.
    /// </remarks>
    public class ListArray<T> : List<T[]>, IList<IList<T>>, IList<IList>
    {
        public ListArray() { }

        public ListArray(int count) : base(count) { }

        IEnumerator<IList> IEnumerable<IList>.GetEnumerator()
        {
            return (new ListEnumeratorWrapper<T, T[]>(GetEnumerator()));
        }

        IEnumerator<IList<T>> IEnumerable<IList<T>>.GetEnumerator()
        {
            return (new ListEnumeratorWrapper<T, T[]>(GetEnumerator()));
        }

        void ICollection<IList<T>>.Add(IList<T> item)
        {
            T[] array = item as T[];
            if (array == null)
            {
                // Copy data to new array
                array = new T[item.Count];
                item.CopyTo(array, 0);
            }
            Add(array);
        }

        void ICollection<IList>.Add(IList item)
        {
            IList<T> tItem = item as IList<T>;
            if (tItem != null)
            {
                ((ICollection<IList<T>>)this).Add(tItem);
                return;
            }
            throw new ArgumentException("Type mismatch", "item");
        }

        bool ICollection<IList<T>>.Contains(IList<T> item)
        {
            T[] array = item as T[];
            if (array != null)
            {
                return (Contains(array));
            }
            return (false);
        }

        bool ICollection<IList>.Contains(IList item)
        {
            IList<T> tItem = item as IList<T>;
            if (tItem != null)
            {
                return (((ICollection<IList<T>>)this).Contains(tItem));
            }
            return (false);
        }

        void ICollection<IList<T>>.CopyTo(IList<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection<IList>.CopyTo(IList[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<IList<T>>.Remove(IList<T> item)
        {
            T[] array = item as T[];
            if (array != null)
            {
                return (Remove(array));
            }
            return (false);
        }

        bool ICollection<IList>.Remove(IList item)
        {
            T[] array = item as T[];
            if (array != null)
            {
                return (Remove(array));
            }
            return (false);
        }

        bool ICollection<IList>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<IList<T>>.IsReadOnly
        {
            get { return false; }
        }

        int IList<IList<T>>.IndexOf(IList<T> item)
        {
            T[] array = item as T[];
            if (array != null)
            {
                return (IndexOf(array));
            }
            return (-1);
        }

        int IList<IList>.IndexOf(IList item)
        {
            T[] array = item as T[];
            if (array != null)
            {
                return (IndexOf(array));
            }
            return (-1);
        }

        void IList<IList<T>>.Insert(int index, IList<T> item)
        {
            T[] array = item as T[];
            if (array == null)
            {
                // Copy data to new array
                array = new T[item.Count];
                item.CopyTo(array, 0);
            }
            Insert(index, array);
        }


        void IList<IList>.Insert(int index, IList item)
        {
            IList<T> tItem = item as IList<T>;
            if (tItem != null)
            {
                ((IList<IList<T>>)this).Insert(index, tItem);
                return;
            }
            throw new ArgumentException("Type mismatch", "item");
        }

        IList IList<IList>.this[int index]
        {
            get { return (new ListWrapper<T>(this[index])); }
            set
            {
                // Try unwrap if possible
                ListWrapper<T> wrapper = value as ListWrapper<T>;
                if (wrapper != null)
                {
                    ((IList<IList<T>>)this)[index] = wrapper.WrappedList;
                    return;
                }
                IList<T> tItem = value as IList<T>;
                if (tItem != null)
                {
                    ((IList<IList<T>>)this)[index] = tItem;
                    return;
                }
                throw new ArgumentException("Type mismatch", "value");
            }
        }

        IList<T> IList<IList<T>>.this[int index]
        {
            get { return (this[index]); }
            set
            {
                T[] array = value as T[];
                if (array == null)
                {
                    array = new T[value.Count];
                    value.CopyTo(array, 0);
                }
                this[index] = array;
            }
        }
    }

}
