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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
    public enum ElementMapperMethod
    {
        None,
        Nearest,
        Inverse,
        Mean,
        Sum,
        WeightedMean,
        WeightedSum,
        Distribute,
        Value
    }

    /// <summary>
    /// The ElementMapper converts one ValueSet (inputValues) associated one ElementSet (fromElements)
    /// to a new ValuesSet (return value of MapValue) that corresponds to another ElementSet 
    /// (toElements). The conversion is a two step procedure where the first step (Initialize) is 
    /// executed at initialisation time only, whereas the MapValues is executed during time stepping.
    /// 
    /// <p>The Initialize method will create a conversion matrix with the same number of rows as the
    /// number of elements in the ElementSet associated to the accepting component (i.e. the toElements) 
    /// and the same number of columns as the number of elements in the ElementSet associated to the 
    /// providing component (i.e. the fromElements).</p>
    /// 
    /// <p>Mapping is possible for any zero-, one- and two-dimensional elemets. Zero dimensional 
    /// elements will always be points, one-dimensional elements will allways be polylines and two-
    /// dimensional elements will allways be polygons.</p>
    /// 
    /// <p>The ElementMapper contains a number of methods for mapping between the different element types.
    /// As an example polyline to polygon mapping may be done either as Weighted Mean or as Weighted Sum.
    /// Typically the method choice will depend on the quantity mapped. Such that state variables such as 
    /// water level will be mapped using Weighted Mean whereas flux variables such as seepage from river 
    /// to groundwater will be mapped using Weighted Sum. The list of available methods for a given 
    /// combination of from and to element types is obtained using the GetAvailableMethods method.</p>
    /// </summary>
    /// 
    public class ElementMapper
    {

        private bool _isInitialised;

        private IMatrix<double> _mappingMatrix; // the mapping matrix
        private ElementMapperMethod? _method;
        private int _numberOfFromColumns;
        private int _numberOfToRows;

        static ElementMapper()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ElementMapper()
        {
            _numberOfToRows = 0;
            _numberOfFromColumns = 0;
            _isInitialised = false;
        }

        public IMatrix<double> MappingMatrix
        {
            get { return _mappingMatrix; }
        }

        public bool IsInitialized
        {
            get { return _isInitialised; }
        }

        public bool UseSearchTree { get; set; }

        /// <summary>
        /// Initialises the ElementMapper. The initialisation includes setting the _isInitialised
        /// flag and calls UpdateMappingMatrix for claculation of the mapping matrix.
        /// </summary>
        ///
        /// <param name="method">String description of mapping method</param> 
        /// <param name="fromElements">The IElementSet to map from.</param>
        /// <param name="toElements">The IElementSet to map to</param>
        /// 
        /// <returns>
        /// The method has no return value.
        /// </returns>
        public void Initialise(IIdentifiable method, IElementSet fromElements, IElementSet toElements)
        {
            UpdateMappingMatrix(method, fromElements, toElements);
            _isInitialised = true;
        }

        /// <summary>
        /// MapValues calculates for each set of timestep data 
        /// a resulting IValueSet through multiplication of an inputValues IValueSet
        /// vector with the mapping maprix. 
        /// </summary>
        /// <param name="inputValues">IValueSet of values to be mapped.</param>
        /// <returns>
        /// A IValueSet found by mapping of the inputValues on to the toElementSet.
        /// </returns>
        public TimeSpaceValueSet<double> MapValues(ITimeSpaceValueSet inputValues)
        {
            if (!_isInitialised)
            {
                throw new Exception(
                    "ElementMapper objects needs to be initialised before the MapValue method can be used");
            }
            if (!ValueSet.GetElementCount(inputValues).Equals(_numberOfFromColumns))
            {
                throw new Exception("Dimension mismatch between inputValues and mapping matrix");
            }

            // Make a time-space value set of the correct size
            TimeSpaceValueSet<double> result = CreateResultValueSet(inputValues.TimesCount(), _numberOfToRows);

            MapValues(result, inputValues);

            return result;
        }

        /// <summary>
        /// Creates a result value set of the size specified
        /// </summary>
        /// <returns>A Value set of the correct size.</returns>
        public static TimeSpaceValueSet<double> CreateResultValueSet(int numtimes, int numElements)
        {
            ListArray<double> outValues = new ListArray<double>(numtimes);
            for (int i = 0; i < numtimes; i++)
            {
                outValues.Add(new double[numElements]);
            }
            return new TimeSpaceValueSet<double>(outValues);
        }

        /// <summary>
        /// MapValues calculates for each set of timestep data 
        /// a resulting IValueSet through multiplication of an inputValues IValueSet
        /// vector with the mapping maprix. 
        /// <para>
        /// This version can be used if the output value set is to be reused (performance or for
        /// adding up)
        /// </para>
        /// </summary>
        /// <param name="outputValues">IValueset of mapped values, of the correct size</param>
        /// <param name="inputValues">IValueSet of values to be mapped.</param>
        public void MapValues(ITimeSpaceValueSet<double> outputValues, ITimeSpaceValueSet inputValues)
        {
            for (int i = 0; i < inputValues.Values2D.Count; i++)
            {
                _mappingMatrix.Product(outputValues.Values2D[i], inputValues.GetElementValuesForTime<double>(i));
            }
        }


        /// <summary>
        /// Calculates the mapping matrix between fromElements and toElements. The mapping method 
        /// is decided from the combination of methodDescription, fromElements.ElementType and 
        /// toElements.ElementType. 
        /// The valid values for methodDescription is obtained through use of the 
        /// GetAvailableMethods method.
        /// </summary>
        /// 
        /// <remarks>
        /// UpdateMappingMatrix is called during initialisation. UpdateMappingMatrix must be called prior
        /// to Mapvalues.
        /// </remarks>
        /// 
        /// <param name="methodIdentifier">String identification of mapping method</param> 
        /// <param name="fromElements">The IElementset to map from.</param>
        /// <param name="toElements">The IElementset to map to</param>
        ///
        /// <returns>
        /// The method has no return value.
        /// </returns>
        private void UpdateMappingMatrix(IIdentifiable methodIdentifier, IElementSet fromElements, IElementSet toElements)
        {
            try
            {
                ElementSetChecker.CheckElementSet(fromElements);
                ElementSetChecker.CheckElementSet(toElements);

                _method = SpatialAdaptedOutputFactory.GetMethod(methodIdentifier);
                _numberOfToRows = toElements.ElementCount;
                _numberOfFromColumns = fromElements.ElementCount;
                _mappingMatrix = new DoubleSparseMatrix(_numberOfToRows, _numberOfFromColumns);

                if (fromElements.ElementType == ElementType.Point && toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPoint toPoint = CreateXYPoint(toElements, i);
                            for (int j = 0; j < _numberOfFromColumns; j++)
                            {
                                XYPoint fromPoint = CreateXYPoint(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculatePointToPointDistance(toPoint, fromPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1 / _mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for point point mapping");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Point && toElements.ElementType == ElementType.PolyLine)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPolyline toPolyLine = CreateXYPolyline(toElements, i);
                            for (int j = 0; j < _numberOfFromColumns; j++)
                            {
                                XYPoint fromPoint = CreateXYPoint(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculatePolylineToPointDistance(toPolyLine,
                                                                                                        fromPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1 / _mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for point to polyline mapping");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to polyline mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Point &&
                         toElements.ElementType == ElementType.Polygon)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPolygon polygon = CreateXYPolygon(toElements, i);
                            int count = 0;
                            XYPoint point;
                            for (int n = 0; n < _numberOfFromColumns; n++)
                            {
                                point = CreateXYPoint(fromElements, n);
                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    if (_method == ElementMapperMethod.Mean)
                                    {
                                        count = count + 1;
                                    }
                                    else if (_method == ElementMapperMethod.Sum)
                                    {
                                        count = 1;
                                    }
                                    else
                                    {
                                        throw new Exception(
                                            "methodDescription unknown for point to polygon mapping");
                                    }
                                }
                            }
                            for (int n = 0; n < _numberOfFromColumns; n++)
                            {
                                point = CreateXYPoint(fromElements, n);

                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    _mappingMatrix[i, n] = 1.0 / count;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Point to polygon mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.PolyLine &&
                         toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPoint toPoint = CreateXYPoint(toElements, i);
                            for (int j = 0; j < _numberOfFromColumns; j++)
                            {
                                XYPolyline fromPolyLine = CreateXYPolyline(fromElements, j);
                                _mappingMatrix[i, j] =
                                    XYGeometryTools.CalculatePolylineToPointDistance(fromPolyLine, toPoint);
                            }
                        }

                        if (_method == ElementMapperMethod.Nearest)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                int denominator = 0;
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] == minDist)
                                    {
                                        _mappingMatrix[i, j] = 1;
                                        denominator++;
                                    }
                                    else
                                    {
                                        _mappingMatrix[i, j] = 0;
                                    }
                                }
                                for (int j = 0; j < _numberOfFromColumns; j++)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                }
                            }
                        }
                        else if (_method == ElementMapperMethod.Inverse)
                        {
                            for (int i = 0; i < _numberOfToRows; i++)
                            {
                                double minDist = _mappingMatrix[i, 0];
                                for (int j = 1; j < _numberOfFromColumns; j++)
                                {
                                    if (_mappingMatrix[i, j] < minDist)
                                    {
                                        minDist = _mappingMatrix[i, j];
                                    }
                                }
                                if (minDist == 0)
                                {
                                    int denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        if (_mappingMatrix[i, j] == minDist)
                                        {
                                            _mappingMatrix[i, j] = 1;
                                            denominator++;
                                        }
                                        else
                                        {
                                            _mappingMatrix[i, j] = 0;
                                        }
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                                else
                                {
                                    double denominator = 0;
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = 1 / _mappingMatrix[i, j];
                                        denominator = denominator + _mappingMatrix[i, j];
                                    }
                                    for (int j = 0; j < _numberOfFromColumns; j++)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("methodDescription unknown for polyline to point mapping");
                        }
                    }
                    catch (Exception e) // Catch for all of the Point to Polyline part 
                    {
                        throw new Exception("Polyline to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.PolyLine &&
                         toElements.ElementType == ElementType.Polygon)
                {
                    #region

                    try
                    {
                        // For each polygon in target
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPolygon polygon = CreateXYPolygon(toElements, i);

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                double totalLineLengthInPolygon = 0;
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    XYPolyline polyline = CreateXYPolyline(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon);
                                    totalLineLengthInPolygon += _mappingMatrix[i, n];
                                }
                                if (totalLineLengthInPolygon > 0)
                                {
                                    for (int n = 0; n < _numberOfFromColumns; n++)
                                    {
                                        _mappingMatrix[i, n] = _mappingMatrix[i, n] /
                                                               totalLineLengthInPolygon;
                                    }
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                // For each line segment in PolyLine
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    XYPolyline polyline = CreateXYPolyline(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon) / polyline.GetLength();
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "methodDescription unknown for polyline to polygon mapping");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Polyline to polygon mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.Point)
                {
                    #region

                    try
                    {
                        if (_method != ElementMapperMethod.Value)
                            throw new Exception("methodDescription unknown for polygon to point mapping");

                        // Only create search tree if number of cols/rows is larger than say 10/10.
                        bool useSearchTree = _numberOfFromColumns > 10 && _numberOfToRows > 10;
                        XYElementSearchTree<int> fromSearchTree = null;
                        ICollection<int> fromCandidateElmts = null;
                        if (useSearchTree)
                            fromSearchTree = XYElementSearchTree<int>.BuildSearchTree(fromElements);
                        else
                            fromCandidateElmts = new IntSequence(0, _numberOfFromColumns - 1);

                        for (int n = 0; n < _numberOfToRows; n++)
                        {
                            XYPoint point = CreateXYPoint(toElements, n);
                            if (useSearchTree)
                            {
                                XYExtent toExtent = XYExtentUtil.GetExtent(point, XYGeometryTools.EPSILON);
                                fromCandidateElmts = fromSearchTree.FindElements(toExtent);
                            }

                            int count = 0;

                            // Check first for strict inclusion
                            foreach (int i in fromCandidateElmts)
                            {
                                XYPolygon polygon = CreateXYPolygon(fromElements, i);
                                if (XYGeometryTools.IsPointInPolygon(point, polygon))
                                {
                                    _mappingMatrix[n, i] = 1.0;
                                    count++;
                                }
                            }
                            if (count == 0)
                            {
                                // Not strictly inside any polygon, check also edges
                                foreach (int i in fromCandidateElmts)
                                {
                                    XYPolygon polygon = CreateXYPolygon(fromElements, i);
                                    if (XYGeometryTools.IsPointInPolygonOrOnEdge(point, polygon))
                                    {
                                        _mappingMatrix[n, i] = 1.0;
                                        count++;
                                    }
                                }
                            }
                            if (count > 1)
                            {
                                // In case of more than one hit, use average
                                foreach (int i in fromCandidateElmts)
                                {
                                    if (_mappingMatrix[n, i] != 0.0)
                                        _mappingMatrix[n, i] = 1.0 / count;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Polygon to point mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.PolyLine)
                // Polygon to PolyLine
                {
                    #region

                    try
                    {
                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPolyline polyline = CreateXYPolyline(toElements, i);

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    XYPolygon polygon = CreateXYPolygon(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon) / polyline.GetLength();
                                }
                                double sum = 0;
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    sum += _mappingMatrix[i, n];
                                }
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    _mappingMatrix[i, n] = _mappingMatrix[i, n] / sum;
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                for (int n = 0; n < _numberOfFromColumns; n++)
                                {
                                    XYPolygon polygon = CreateXYPolygon(fromElements, n);
                                    _mappingMatrix[i, n] =
                                        XYGeometryTools.CalculateLengthOfPolylineInsidePolygon(
                                            polyline, polygon) / polyline.GetLength();
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "methodDescription unknown for polygon to polyline mapping");
                            }
                        }
                    }
                    catch (Exception e) // catch for all of Polygon to PolyLine
                    {
                        throw new Exception("Polygon to polyline mapping failed", e);
                    }

                    #endregion
                }
                else if (fromElements.ElementType == ElementType.Polygon &&
                         toElements.ElementType == ElementType.Polygon)
                // Polygon to Polygon
                {
                    #region

                    try
                    {
                        // Only create search tree if number of cols/rows is larger than say 100/10.
                        bool useSearchTree = _numberOfFromColumns > 10 && _numberOfToRows > 10;
                        XYElementSearchTree<int> fromSearchTree = null;
                        ICollection<int> fromCandidateElmts = null;
                        if (useSearchTree)
                            fromSearchTree = XYElementSearchTree<int>.BuildSearchTree(fromElements);
                        else
                            fromCandidateElmts = new IntSequence(0, _numberOfFromColumns - 1);

                        for (int i = 0; i < _numberOfToRows; i++)
                        {
                            XYPolygon toPolygon = CreateXYPolygon(toElements, i);
                            if (useSearchTree)
                            {
                                XYExtent toExtent = XYExtentUtil.GetExtent(toPolygon);
                                fromCandidateElmts = fromSearchTree.FindElements(toExtent);
                            }

                            foreach (int j in fromCandidateElmts)
                            {
                                XYPolygon fromPolygon = CreateXYPolygon(fromElements, j);
                                _mappingMatrix[i, j] = XYGeometryTools.CalculateSharedArea(
                                    toPolygon, fromPolygon);
                                if (_method == ElementMapperMethod.Distribute)
                                {
                                    _mappingMatrix[i, j] /= fromPolygon.GetArea();
                                }
                            }

                            if (_method == ElementMapperMethod.WeightedMean)
                            {
                                double denominator = 0;
                                foreach (int j in fromCandidateElmts)
                                {
                                    denominator = denominator + _mappingMatrix[i, j];
                                }
                                foreach (int j in fromCandidateElmts)
                                {
                                    if (denominator != 0)
                                    {
                                        _mappingMatrix[i, j] = _mappingMatrix[i, j] / denominator;
                                    }
                                }
                            }
                            else if (_method == ElementMapperMethod.WeightedSum)
                            {
                                foreach (int j in fromCandidateElmts)
                                {
                                    _mappingMatrix[i, j] = _mappingMatrix[i, j] / toPolygon.GetArea();
                                }
                            }
                            else if (_method != ElementMapperMethod.Distribute)
                            {
                                throw new Exception(
                                    "methodDescription unknown for polygon to polygon mapping");
                            }
                        }
                    }
                    catch (Exception e) // catch for all of Polygon to Polygon
                    {
                        throw new Exception("Polygon to polygon mapping failed", e);
                    }

                    #endregion
                }
                else // if the fromElementType, toElementType combination is no implemented
                {
                    throw new Exception(
                        "Mapping of specified ElementTypes not included in ElementMapper");
                }
            }
            catch (Exception e)
            {
                throw new Exception("UpdateMappingMatrix failed to update mapping matrix", e);
            }
        }

        /// <summary>
        /// Extracts the (row, column) element from the MappingMatrix.
        /// </summary>
        /// 
        /// <param name="row">Zero based row index</param>
        /// <param name="column">Zero based column index</param>
        /// <returns>
        /// Element(row, column) from the mapping matrix.
        /// </returns>
        public double GetValueFromMappingMatrix(int row, int column)
        {
            try
            {
                ValidateIndicies(row, column);
            }
            catch (Exception e)
            {
                throw new Exception("GetValueFromMappingMatrix failed.", e);
            }
            return _mappingMatrix[row, column];
        }

        /// <summary>
        /// Sets individual the (row, column) element in the MappingMatrix.
        /// </summary>
        /// 
        /// <param name="value">Element value to set</param>
        /// <param name="row">Zero based row index</param>
        /// <param name="column">Zero based column index</param>
        /// <returns>
        /// No value is returned.
        /// </returns>
        public void SetValueInMappingMatrix(double value, int row, int column)
        {
            try
            {
                ValidateIndicies(row, column);
            }
            catch (Exception e)
            {
                throw new Exception("SetValueInMappingMatrix failed.", e);
            }
            _mappingMatrix[row, column] = value;
        }

        private void ValidateIndicies(int row, int column)
        {
            if (row < 0)
            {
                throw new Exception("Negative row index not allowed. GetValueFromMappingMatrix failed.");
            }
            if (row >= _numberOfToRows)
            {
                throw new Exception(
                    "Row index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
            }
            if (column < 0)
            {
                throw new Exception("Negative column index not allowed. GetValueFromMappingMatrix failed.");
            }
            if (column >= _numberOfFromColumns)
            {
                throw new Exception(
                    "Column index exceeds mapping matrix dimension. GetValueFromMappingMatrix failed.");
            }
        }

        private static XYPoint CreateXYPoint(IElementSet elementSet, int index)
        {
            if (elementSet.ElementType != ElementType.Point)
            {
                throw new ArgumentOutOfRangeException("elementSet",
                                                      "Cannot create XYPoint, the element type of the element set is not XYPoint");
            }

            return new XYPoint(elementSet.GetVertexXCoordinate(index, 0), elementSet.GetVertexYCoordinate(index, 0));
        }

        public static XYPolyline CreateXYPolyline(IElementSet elementSet, int index)
        {
            if (!(elementSet.ElementType == ElementType.PolyLine))
            {
                throw new Exception("Cannot create XYPolyline");
            }

            var xyPolyline = new XYPolyline();
            for (int i = 0; i < elementSet.GetVertexCount(index); i++)
            {
                xyPolyline.Points.Add(new XYPoint(elementSet.GetVertexXCoordinate(index, i),
                                                  elementSet.GetVertexYCoordinate(index, i)));
            }

            return xyPolyline;
        }

        public static XYPolygon CreateXYPolygon(IElementSet elementSet, int index)
        {
            if (elementSet.ElementType != ElementType.Polygon)
            {
                throw new Exception("Cannot create XYPolyline");
            }

            var xyPolygon = new XYPolygon();

            for (int i = 0; i < elementSet.GetVertexCount(index); i++)
            {
                xyPolygon.Points.Add(new XYPoint(elementSet.GetVertexXCoordinate(index, i),
                                                 elementSet.GetVertexYCoordinate(index, i)));
            }

            return xyPolygon;
        }

        public ElementType GetTargetElementType()
        {
            throw new NotImplementedException();
        }


        public static XYPolygon CreateFromXYPolygon(IElementSet elementSet, int index)
        {
            if (elementSet.ElementType != ElementType.Polygon)
            {
                throw new Exception("Cannot create XYPolyline");
            }

            XYPolygon xyPolygon = new XYPolygon();

            for (int i = 0; i < elementSet.GetVertexCount(index); i++)
            {
                xyPolygon.Points.Add(new XYPoint(elementSet.GetVertexXCoordinate(index, i),
                                                 elementSet.GetVertexYCoordinate(index, i)));
            }

            return xyPolygon;
        }


        #region Nested class:

        class IntSequence : ICollection<int>
        {
            private int _first;
            private int _last;

            public IntSequence(int first, int last)
            {
                _first = first;
                _last = last;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return new IntSequenceEnumerator(_first, _last);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(int item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(int item)
            {
                if (_first <= item && item <= _last)
                    return true;
                return (false);
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = _first + i;
                }
            }

            public bool Remove(int item)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return (_last - _first + 1); }
            }

            public bool IsReadOnly
            {
                get { return (true); }
            }


        }

        class IntSequenceEnumerable : IEnumerable<int>
        {
            private int _first;
            private int _last;

            public IntSequenceEnumerable(int first, int last)
            {
                _first = first;
                _last = last;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return new IntSequenceEnumerator(_first, _last);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class IntSequenceEnumerator : IEnumerator<int>
        {
            private int _first;
            private int _last;
            private int _current;

            public IntSequenceEnumerator(int first, int last)
            {
                _first = first;
                _last = last;
                _current = first - 1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_current < _last)
                {
                    _current++;
                    return (true);
                }
                // Current equals last or last+1, set it to last+1
                _current = _last + 1;
                return (false);
            }

            public void Reset()
            {
                _current = _first - 1;
            }

            public int Current
            {
                get
                {
                    if (_current < _first)
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    if (_current > _last)
                        throw new InvalidOperationException("Enumeration already finished");
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        #endregion
    }
}