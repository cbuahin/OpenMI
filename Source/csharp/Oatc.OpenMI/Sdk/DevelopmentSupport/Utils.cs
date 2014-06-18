using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
  /// <summary>
  /// Utility class (static) providing a number of convenience
  /// methods that are not directly related to the OpenMI standard
  /// classes
  /// </summary>
  public static class Utils
  {
    /// <summary>
    /// <para>
    /// Finds the interval in a vector where the argument lays in between. Assumes that the
    /// vector has at least two elements. Uses binary search.
    /// </para>
    /// <para>
    /// The vector must increase strictly monotonically. If argument is below the first value, 0
    /// is returned. If argument is above last value, the length of the vector is returned.
    /// </para>
    /// <para>
    /// The result is the interval number, one based, meaning that the arg fullfills:
    /// <code>(vector[res-1] &lt; arg &amp;&amp; arg &lt;= vector[res])</code>
    /// </para>
    /// <para>
    /// Also the scale factor for interpolating is returned. For linear interpolation
    /// <code>arg = vector[res-1] + fraction*(vector[res]-vector[res-1])</code>
    /// or
    /// <code>arg = (1-fraction)*vector[res-1] + fraction*vector[res]</code>
    /// </para>
    /// <para>
    /// If arg exists in vector, the result is such that 
    /// <code>arg = vector[res]</code>
    /// i.e., fraction is one (except when arg = vector[0], then res = 1 and fraction = 0).
    /// </para>
    /// </summary>
    /// <param name="arg">Value to look up in table</param>
    /// <param name="vector">Vector, must be monoton increasing</param>
    /// <param name="fraction">number with wich to scale the highest</param>
    public static int GetInterval(this double[] vector, double arg, out double fraction)
    {
      int iHigh = vector.Length - 1;
      int iLow = 0;

      if (arg < vector[iLow])
      {
        fraction = 0;
        return (0);
      }
      if (arg > vector[iHigh])
      {
        fraction = 1;
        return (iHigh + 1);
      }

      while ((iHigh - iLow) > 1)
      {
        int mid = iLow + (iHigh - iLow) / 2;
        if (arg <= vector[mid])
          iHigh = mid;
        else
          iLow = mid;
      }
      fraction = (arg - vector[iLow]) / (vector[iHigh] - vector[iLow]);
      return (iHigh);
    }

    /// <summary>
    /// See <see cref="GetInterval(double[],double,out double)"/> for details.
    /// <para>
    /// This has an <see cref="IList{T}"/> as argument instead of a double[].
    /// </para>
    /// </summary>
    /// <remarks>
    /// This version is slower than the double[] version.
    /// </remarks>
    public static int GetInterval(this IList<double> vector, double arg, out double fraction)
    {
      int iHigh = vector.Count - 1;
      int iLow = 0;

      if (arg < vector[iLow])
      {
        fraction = 0;
        return (0);
      }
      if (arg > vector[iHigh])
      {
        fraction = 1;
        return (iHigh + 1);
      }

      while ((iHigh - iLow) > 1)
      {
        int mid = iLow + (iHigh - iLow) / 2;
        if (arg <= vector[mid])
          iHigh = mid;
        else
          iLow = mid;
      }
      fraction = (arg - vector[iLow]) / (vector[iHigh] - vector[iLow]);
      return (iHigh);
    }

    /// <summary>
    /// Version of <see cref="GetInterval(double[],double,out double)"/> (see there for details), 
    /// that extracts a double key value from the objects of type T, using the 
    /// <paramref name="keySelector"/>.
    /// Assumes the objects are ordered by this key.
    /// <para>
    /// </summary>
    /// <param name="vector">List, must be monotonically increasing in the key</param>
    /// <param name="arg">Key value to look up in table</param>
    /// <param name="fraction">Number with wich to scale the highest</param>
    /// <param name="keySelector">Delegate selecting a double key from a T object</param>
    public static int GetInterval<T>(this IList<T> vector, double arg, out double fraction, Func<T, double> keySelector)
    {
      int iHigh = vector.Count - 1;
      int iLow = 0;

      // Key value belonging to iLow
      double kLow = keySelector(vector[iLow]);
      if (arg < kLow)
      {
        fraction = 0;
        return (0);
      }
      // Key value belonging to iHigh
      double kHigh = keySelector(vector[iHigh]);
      if (arg > kHigh)
      {
        fraction = 1;
        return (iHigh + 1);
      }

      while ((iHigh - iLow) > 1)
      {
        int mid = iLow + (iHigh - iLow) / 2;
        double kMid = keySelector(vector[mid]);
        if (arg <= kMid)
        {
          iHigh = mid;
          kHigh = kMid;
        }
        else
        {
          iLow = mid;
          kLow = kMid;
        }
      }
      fraction = (arg - kLow) / (kHigh - kLow);
      return (iHigh);
    }

    #region Binary search routines for sorted IList's

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element
    /// and returns the zero-based index of the element.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list</typeparam>
    /// <param name="list">List to search in</param>
    /// <param name="key">The key to locate. The value can be null for reference types.</param>
    /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>, if item is found; 
    /// otherwise, a negative number that is the bitwise complement 
    /// of the index of the next element that is larger than item or, 
    /// if there is no larger element, the bitwise complement of Count.</returns>
    public static int BinarySearch<T>(this IList<T> list, T key)
    {
      return (BinarySearch(list, key, Comparer<T>.Default));
    }

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the provided 
    /// comparer and returns the zero-based index of the element.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list</typeparam>
    /// <param name="list">List to search in</param>
    /// <param name="key">The key to locate. The value can be null for reference types.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
    /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>, if item is found; 
    /// otherwise, a negative number that is the bitwise complement 
    /// of the index of the next element that is larger than item or, 
    /// if there is no larger element, the bitwise complement of Count.</returns>
    public static int BinarySearch<T>(this IList<T> list, T key, IComparer<T> comparer)
    {
      // Use the build in binary search, if possible
      List<T> ll = list as List<T>;
      if (ll != null)
        return (ll.BinarySearch(key, comparer));

      return list.BinarySearch(t => t, key, comparer);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element 
    /// and returns the zero-based index of the element.
    /// <para>
    /// This differs from the "ordinary" binary search in allowing a <paramref name="keySelector"/>comparer 
    /// that knows how to compare a class with its key. Example, if the list contains classes of type T having 
    /// an id number and the class is sorted on that id, then the keySelector returns the id number for that class.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list</typeparam>
    /// <typeparam name="TKey">The type of the argument to look for</typeparam>
    /// <param name="list">List to search in</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="key">The key to locate. The value can be null for reference types.</param>
    /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>, if item is found; 
    /// otherwise, a negative number that is the bitwise complement 
    /// of the index of the next element that is larger than item or, 
    /// if there is no larger element, the bitwise complement of Count.</returns>
    public static int BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
    {
      return BinarySearch(list, keySelector, key, Comparer<TKey>.Default);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the provided 
    /// comparer and returns the zero-based index of the element.
    /// <para>
    /// This differs from the "ordinary" binary search in allowing a <paramref name="keySelector"/>comparer 
    /// that knows how to compare a class with its key. Example, if the list contains classes of type T having 
    /// an id number and the class is sorted on that id, then the keySelector returns the id number for that class.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list</typeparam>
    /// <typeparam name="TKey">The type of the argument to look for</typeparam>
    /// <param name="list">List to search in</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="key">The key to locate. The value can be null for reference types.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements.</param>
    /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>, if item is found; 
    /// otherwise, a negative number that is the bitwise complement 
    /// of the index of the next element that is larger than item or, 
    /// if there is no larger element, the bitwise complement of Count.</returns>
    public static int BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key, IComparer<TKey> comparer)
    {
      int iLow = 0;
      int iHigh = list.Count - 1;
      if (iHigh < 0)
        throw new ArgumentException("List has no elements", "list");

      while (iLow <= iHigh)
      {
        int mid = iLow + (iHigh - iLow) / 2;
        int compare = comparer.Compare(keySelector(list[mid]), key);
        if (compare < 0) // when list[mid] < key
          iLow = mid + 1;
        else if (compare > 0)
          iHigh = mid - 1;
        else
          return (mid);
      }
      // No exact match found
      return ~iLow;
    }


    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the provided 
    /// comparer and returns the zero-based index of the element.
    /// <para>
    /// This differs from the "ordinary" binary search in allowing a comparer delegate that defines
    /// whether an item is found (returning 0), whether the item in the list is before (&lt;0) or after (&gt;0)
    /// that knows how to compare a class with its key. Example, if the list contains classes of type T having 
    /// an id number and the class is sorted on that id, then the keySelector returns the id number for that class.
    /// </para>
    /// <example>
    /// If having a list of doubles, to find 4.5 in the list, use:
    /// <code>
    /// int index = list.BinarySearch(d => d.CompareTo(4.5))
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list</typeparam>
    /// <param name="list">List to search in</param>
    /// <param name="comparer">A delegate/lambda expression specifying if a given item in the list is before (&lt;0), matches (0) or is after (&gt;0) the item to search for.</param>
    /// <returns>The zero-based index of item in the sorted <see cref="IList{T}"/>, if item is found; 
    /// otherwise, a negative number that is the bitwise complement 
    /// of the index of the next element that is larger than item or, 
    /// if there is no larger element, the bitwise complement of Count.</returns>
    public static int BinarySearch<T>(this IList<T> list, Func<T, int> comparer)
    {
      int iLow = 0;
      int iHigh = list.Count - 1;
      if (iHigh < 0)
        throw new ArgumentException("List has no elements", "list");

      while (iLow <= iHigh)
      {
        int mid = iLow + (iHigh - iLow) / 2;
        int compare = comparer(list[mid]);
        if (compare < 0) // when list[mid] < key
          iLow = mid + 1;
        else if (compare > 0)
          iHigh = mid - 1;
        else
          return (mid);
      }
      // No exact match found
      return ~iLow;
    }

    #endregion
  }
}
