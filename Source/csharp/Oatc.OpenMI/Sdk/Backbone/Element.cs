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
using OpenMI.Standard2;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary>
    /// The Element class contains a spatial element.
    /// <para>This is a trivial implementation for use with Oatc.OpenMI.Sdk.Backbone.ElementSet, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class Element : IIdentifiable
    {
        private readonly List<Coordinate> vertices = new List<Coordinate>();
        private readonly List<int[]> faceVertexIndices = new List<int[]>();
        private string id = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public Element()
        {
        }

        /// <summary>
        /// Setter and getter functions for the vertex coordinates array
        /// </summary>
        public virtual Coordinate[] Vertices
        {
            get { return vertices.ToArray(); }
            set
            {
                vertices.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    vertices.Add(value[i]);
                }
            }
        }

        /// <summary>
        /// Returns the vertex count
        /// </summary>
        public virtual int VertexCount
        {
            get { return vertices.Count; }
        }

        /// <summary>
        /// Getter and setter functions for the element Caption
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The element to copy</param>
        public Element(Element source)
        {
            Id = source.Id;
            for (int i = 0; i < source.Vertices.Length; i++)
            {
                vertices.Add(source.Vertices[i]);
            }
        }

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="id">The Element Caption</param>
        public Element(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Gets a vertex
        /// </summary>
        /// <param name="Index">The vertex index</param>
        /// <returns>The vertex</returns>
        public virtual Coordinate GetVertex(int Index)
        {
            return vertices[Index];
        }

        /// <summary>
        /// Adds a vertex
        /// </summary>
        /// <param name="vertex">The vertex to add</param>
        public virtual void AddVertex(Coordinate vertex)
        {
            vertices.Add(vertex);
        }

        /// <summary>
        /// Returns the number of faces
        /// </summary>
        public int FaceCount
        {
            get { return faceVertexIndices.Count; }
        }

        /// <summary>
        /// Adds a face
        /// </summary>
        /// <param name="vertexIndices">The vertex indices for the face</param>
        public void AddFace(int[] vertexIndices)
        {
            faceVertexIndices.Add(vertexIndices);
        }

        /// <summary>
        /// Returns the face vertex indices for a face
        /// </summary>
        /// <param name="faceIndex">The face index</param>
        /// <returns>The face vertex indices</returns>
        public int[] GetFaceVertexIndices(int faceIndex)
        {
            return faceVertexIndices[faceIndex];
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
            Element e = (Element) obj;
            if (!Id.Equals(e.Id))
                return false;
            if (VertexCount != e.VertexCount)
                return false;
            for (int i = 0; i < VertexCount; i++)
                if (!GetVertex(i).Equals(e.GetVertex(i)))
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
            if (id != null) hashCode += id.GetHashCode();
            if (vertices != null) hashCode += vertices.GetHashCode();
            return hashCode;
        }

        ///<summary>
        /// String representation of the element
        ///</summary>
        ///<returns></returns>
        public override string ToString()
        {
            if (id != null)
            {
                return id;
            }
            else
            {
                return "georef'd element";
            }
        }

        public string Caption { get; set; }
        
        public string Description { get; set; }
    }
}