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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oatc.OpenMI.Sdk.Spatial
{
    /// <summary>
    /// Base matrix interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatrix<T> where T : IComparable<T>
    {
        T this[int rowIndex, int columnIndex] { get; set; }
        int RowCount { get; }
        int ColumnCount { get; }

        /// <summary>
        /// Does a matrix-vector product
        /// </summary>
        /// <param name="vector">Vector b in res = A*b</param>
        /// <returns>Vector res in res = A*b</returns>
        IList<double> Product(IList<double> vector);

        /// <summary>
        /// Does a matrix-vector product
        /// </summary>
        /// <param name="vector">Vector res in res = A*b</param>
        /// <param name="vector">Vector b in res = A*b</param>
        void Product(IList<double> res, IList<double> vector);
    }


    /// <summary>
    /// Sparse matrix having double elements
    /// </summary>
    [Serializable]
    public class DoubleSparseMatrix : IMatrix<double>
    {
        [Serializable]
        public struct Index
        {
            public Index(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int Row;

            public int Column;
        }

        public readonly Dictionary<Index, double> Values;

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public DoubleSparseMatrix(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            Values = new Dictionary<Index, double>();
        }

        public bool IsCellEmpty(int row, int column)
        {
            var index = new Index(row, column);
            return !Values.ContainsKey(index);
        }

        public double this[int row, int column]
        {
            get
            {
                var index = new Index(row, column);
                double result;
                Values.TryGetValue(index, out result);
                return result;
            }
            set
            {
                var index = new Index(row, column);

                if (value.Equals(0))
                {
                    if (Values.ContainsKey(index))
                    {
                        Values.Remove(index);
                    }

                    return;
                }

                Values[index] = value;
            }
        }

        public IList<double> Product(IList<double> vector)
        {
            var outputValues = new double[RowCount];
            Product(outputValues, vector);
            return outputValues;
        }

        public void Product(IList<double> res, IList<double> vector)
        {
            foreach (var entry in Values)
            {
                res[entry.Key.Row] += entry.Value * vector[entry.Key.Column];
            }
        }

    }

}
