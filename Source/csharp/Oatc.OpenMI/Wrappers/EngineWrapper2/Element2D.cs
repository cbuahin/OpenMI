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
using System.Text;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2
{
    public struct Point2D
    {
        public readonly double X;
        public readonly double Y;

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class ElementSet2D : IElementSet, ICloneable
    {
        string _caption;
        string _description = null;
        string _spatialReferenceSystemWkt = string.Empty;
        int _version = 0;
        ElementType _elementType;
        List<Point2D[]> _points = new List<Point2D[]>();

        public ElementSet2D(string caption, ElementType elementType)
        {
            _caption = caption;
            _elementType = elementType;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", _caption, _description);
        }

        public void AddElement(Point2D[] elementPoints)
        {
            _points.Add(elementPoints);
        }

        public void AddRangeElements(List<Point2D[]> elements)
        {
            _points.AddRange(elements);
        }

        #region IElementSet Members

        public string SpatialReferenceSystemWkt
        {
            get { return _spatialReferenceSystemWkt; }
            set { _spatialReferenceSystemWkt = value; }
        }

        public ElementType ElementType
        {
            get { return _elementType; }
        }

        public int ElementCount
        {
            get { return _points.Count; }
        }

        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public int GetElementIndex(IIdentifiable elementId)
        {
            int elementIndex;

            if (!int.TryParse(elementId.Id, out elementIndex))
                throw new ArgumentOutOfRangeException(elementId.Id);

            return elementIndex;
        }

        public IIdentifiable GetElementId(int elementIndex)
        {
            if (elementIndex < 0 || elementIndex >= ElementCount)
                throw new ArgumentOutOfRangeException(elementIndex.ToString());

            return new Identifier(elementIndex.ToString());
        }

        public int GetVertexCount(int elementIndex)
        {
            if (elementIndex < 0 || elementIndex >= ElementCount)
                throw new ArgumentOutOfRangeException(elementIndex.ToString());

            return _points[elementIndex].Length;
        }

        public int GetFaceCount(int elementIndex)
        {
            // TODO (ADH): Redundant?
            return 0;
        }

        public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
        {
            // TODO (ADH): Redundant?

            if (faceIndex < 0 || GetFaceCount(elementIndex) >= faceIndex)
                throw new ArgumentOutOfRangeException(faceIndex.ToString());

            int[] indices = new int[GetVertexCount(elementIndex)];

            for (int n = 0; n < indices.Length; ++n)
                indices[n] = n;

            return indices;
        }

        public bool HasZ
        {
            get { return false; }
        }

        public bool HasM
        {
            get { return false; }
        }

        public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
        {
            if (elementIndex < 0 || elementIndex >= ElementCount)
                throw new ArgumentOutOfRangeException(elementIndex.ToString());
            if (vertexIndex < 0 || vertexIndex >= _points[elementIndex].Length)
                throw new ArgumentOutOfRangeException(vertexIndex.ToString());

            return _points[elementIndex][vertexIndex].X;
        }

        public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
        {
            if (elementIndex < 0 || elementIndex >= ElementCount)
                throw new ArgumentOutOfRangeException(elementIndex.ToString());
            if (vertexIndex < 0 || vertexIndex >= _points[elementIndex].Length)
                throw new ArgumentOutOfRangeException(vertexIndex.ToString());

            return _points[elementIndex][vertexIndex].Y;
        }

        public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
        {
            throw new InvalidOperationException();
        }

        public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                    return string.Format("{0}[{1}] ({2},{3})", 
                        _elementType, _points.Count, _spatialReferenceSystemWkt, _version);

                return _description;
            }
            set { _description = value; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            ElementSet2D es = (ElementSet2D)MemberwiseClone();
            es._points = new List<Point2D[]>(_points.Count);
            es._points.AddRange(_points);
            return es;
        }

        #endregion
    }
}
