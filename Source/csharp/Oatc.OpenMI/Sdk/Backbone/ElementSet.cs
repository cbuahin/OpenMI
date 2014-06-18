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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary>
    /// The ElementSet class describes a collection of spatial elements.
    /// <para>This is a trivial implementation of OpenMI.Standard.IElementSet, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class ElementSet : IElementSet
    {
        private readonly ArrayList elements = new ArrayList();
        private string _caption = string.Empty;
        private string _description = string.Empty;
        private ElementType _elementType;
        private string _spatialReferenceWKT = string.Empty;
        bool _hasZ = false;
        bool _hasM = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public ElementSet(string caption)
        {
            _caption = caption;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The element set to copy</param>
        public ElementSet(IElementSet source)
        {
            _description = source.Description;
            _caption = source.Caption;
            _elementType = source.ElementType;
            _spatialReferenceWKT = source.SpatialReferenceSystemWkt;

            for (int i = 0; i < source.ElementCount; i++)
            {
                Element element = new Element(source.GetElementId(i).Id);
                for (int j = 0; j < source.GetVertexCount(i); j++)
                {
                    Coordinate vertex = new Coordinate(source.GetVertexXCoordinate(i, j),
                                                       source.GetVertexYCoordinate(i, j),
                                                       source.GetVertexZCoordinate(i, j));
                    element.AddVertex(vertex);
                }
                elements.Add(element);
            }
        }

        public ElementSet(string description, string caption, ElementType elementType)
            : this(description, caption, elementType, "")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Description</param>
        /// <param name="caption">Caption</param>
        /// <param name="elementType">Element type</param>
        /// <param name="spatialReference">Spatial reference</param>
        public ElementSet(string description, string caption, ElementType elementType, string spatialReference)
        {
            _description = description;
            _caption = caption;
            _elementType = elementType;
            _spatialReferenceWKT = spatialReference;
        }

        /// <summary>
        /// Getter and setter functions for the element list
        /// </summary>
        public virtual Element[] Elements
        {
            get
            {
                Element[] elements = new Element[this.elements.Count];
                for (int i = 0; i < this.elements.Count; i++)
                {
                    elements[i] = (Element) this.elements[i];
                }
                return elements;
            }
            set
            {
                elements.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    elements.Add(value[i]);
                }
            }
        }

        #region IElementSet Members

        /// <summary>
        /// ElementSet version
        /// </summary>
        public virtual int Version
        {
            get { return 0; }
        }

        /// <summary>
        /// Setter and getter for the element set description
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Setter and getter for the element set Caption
        /// </summary>
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        /// <summary>
        /// Setter and getter for the element type
        /// </summary>
        public virtual ElementType ElementType
        {
            get { return _elementType; }
            set { _elementType = value; }
        }

        public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            return element.GetVertex(vertexIndex).X;
        }

        public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            return element.GetVertex(vertexIndex).Y;
        }

        public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            return element.GetVertex(vertexIndex).Z;
        }

        public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
        {
          Element element = (Element)elements[elementIndex];
          return element.GetVertex(vertexIndex).M;
        }

        public bool HasZ
        {
            get { return _hasZ; }
            set { _hasZ = value; }
        }

        public bool HasM
        {
            get { return _hasM; }
            set { _hasM = value; }
        }

        /// <summary>
        /// Returns the x coordinate for a vertex
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <param name="vertexIndex">The vertex index</param>
        /// <returns>The x coordinate</returns>
        public virtual double GetXCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            Coordinate vertex = element.GetVertex(vertexIndex);
            return vertex.X;
        }

        /// <summary>
        /// Returns the y coordinate for a vertex
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <param name="vertexIndex">The vertex index</param>
        /// <returns>The y coordinate</returns>
        public virtual double GetYCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            Coordinate vertex = element.GetVertex(vertexIndex);
            return vertex.Y;
        }

        /// <summary>
        /// Returns the z coordinate for a vertex
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <param name="vertexIndex">The vertex index</param>
        /// <returns>The z coordinate</returns>
        public virtual double GetZCoordinate(int elementIndex, int vertexIndex)
        {
            Element element = (Element) elements[elementIndex];
            Coordinate vertex = element.GetVertex(vertexIndex);
            return vertex.Z;
        }

        /// <summary>
        /// Returns the number of elements
        /// </summary>
        public virtual int ElementCount
        {
            get { return elements.Count; }
        }

        /// <summary>
        /// Returns the number of vertices for an element
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <returns>The number of vertices for this element</returns>
        public virtual int GetVertexCount(int elementIndex)
        {
            Element element = (Element) elements[elementIndex];
            return element.VertexCount;
        }

        /// <summary>
        /// Getter and setter for the spatial reference
        /// </summary>
        public virtual string SpatialReferenceSystemWkt
        {
            get { return _spatialReferenceWKT; }
            set { _spatialReferenceWKT = value; }
        }

        /// <summary>
        /// Returns the element index for a given element Caption
        /// </summary>
        /// <param name="elementId">The element Caption</param>
        /// <returns>The element index</returns>
        public virtual int GetElementIndex(IIdentifiable elementId)
        {
            if (elementId is ElementIdentifier)
            {
                return ((ElementIdentifier)elementId).Index;
            }

            for (var i = 0; i < elements.Count; i++)
            {
                var element = (Element)elements[i];

                if (element.Id.Equals(elementId.Id))
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException("Element " + elementId.Id + " not found.");
        }

        /// <summary>
        /// Returns an element Caption for an element
        /// </summary>
        /// <param name="index">The element index</param>
        /// <returns>The element Caption</returns>
        public virtual IIdentifiable GetElementId(int index)
        {
            return (new ElementIdentifier(index, (Element)elements[index]));
        }



        /// <summary>
        /// Returns the list of face vertex indices for a given element and face
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <param name="faceIndex">The face index</param>
        /// <returns>List of face vertex indices</returns>
        public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
        {
            return ((Element) elements[elementIndex]).GetFaceVertexIndices(faceIndex);
        }

        /// <summary>
        /// Returns the face count for a given element
        /// </summary>
        /// <param name="elementIndex">The element index</param>
        /// <returns>The face count for the given element</returns>
        public int GetFaceCount(int elementIndex)
        {
            return ((Element) elements[elementIndex]).FaceCount;
        }

        #endregion

        /// <summary>
        /// Adds an element
        /// </summary>
        /// <param name="element">The element to add</param>
        public virtual void AddElement(Element element)
        {
            elements.Add(element);
        }

        /// <summary>
        /// Gets an element
        /// </summary>
        /// <param name="index">The element index</param>
        /// <returns>The element</returns>
        public virtual Element GetElement(int index)
        {
            return (Element) elements[index];
        }

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
        ///<param name="obj">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            ElementSet s = (ElementSet) obj;
            if (Description == null && s.Description != null)
                return false;
            if (Description != null && !Description.Equals(s.Description))
                return false;
            if (!Caption.Equals(s.Caption))
                return false;
            if (SpatialReferenceSystemWkt == null && s.SpatialReferenceSystemWkt != null)
                return false;
            if (SpatialReferenceSystemWkt != null && !SpatialReferenceSystemWkt.Equals(s.SpatialReferenceSystemWkt))
                return false;
            if (!ElementType.Equals(s.ElementType))
                return false;
            if (ElementCount != s.ElementCount)
                return false;
            for (int i = 0; i < ElementCount; i++)
                if (!GetElement(i).Equals(s.GetElement(i)))
                    return false;
            return true;
        }

        ///<summary>
        /// Get Hash Code.
        ///</summary>
        ///<returns>Hash Code for the current instance.</returns>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            if (_caption != null) hashCode += _caption.GetHashCode();
            hashCode += _elementType.GetHashCode();
            return hashCode;
        }

        ///<summary>
        /// String representation of the 
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            return Caption;
        }

        /// <summary>
        /// Helper class for Id'ing elements
        /// </summary>
        class ElementIdentifier : IIdentifiable
        {
            public readonly int Index;
            private readonly Element _element;

            public ElementIdentifier(int index, Element element)
            {
                Index = index;
                _element = element;
            }

            public string Id
            {
                get { return (_element.Id); }
            }

            public string Caption { get; set; }
            public string Description { get; set; }
        }

    }
}