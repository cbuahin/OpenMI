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
using System.Collections.Generic;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
    /// <summary>
    /// 2D element search tree.
    /// <para>
    /// The search tree structure is build up by adding a number of coordinates
    /// to the search tree, usually the coordinates of the nodes in the element.
    /// </para>
    /// <para>
    /// When the search tree structure is build up, the elements can be added
    /// to the search tree. 
    /// </para>
    /// <para>
    /// You can not add additional nodes to a search tree, after the first element
    /// has been added.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the element in the search tree</typeparam>
    public class XYElementSearchTree<T>
    {
        /// <summary>
        /// Head of the tree
        /// </summary>
        private readonly TreeNode _head;

        /// <summary>
        /// Counts the number of unique nodes used for building up the tree
        /// </summary>
        private int _numNodes;

        /// <summary>
        /// Counts the number of elements added to the tree
        /// </summary>
        private int _numElmts;

        /// <summary>
        /// Returns true if elements has already been added to the tree
        /// </summary>
        private bool HasElements { get { return (_numElmts > 0); } }

        /// <summary>
        /// Leaf in the search tree
        /// </summary>
        /// <typeparam name="T">Type of the element in the search tree</typeparam>
        private struct Leaf<TT>
        {
            /// <summary>
            /// Element in the search tree
            /// </summary>
            public TT Element;
            /// <summary>
            /// Extent of element in the search tree
            /// </summary>
            public XYExtent Extent;
        }


        /// <summary>
        /// Create a new search tree that covers the provided <paramref name="extent"/>
        /// </summary>
        /// <param name="extent">Extent that the search tree should cover</param>
        public XYElementSearchTree(XYExtent extent)
        {
            _head = new TreeNode(extent);
        }

        /// <summary>
        /// Add point to the search tree, thereby building the tree.
        /// </summary>
        /// <param name="point">xy point to add</param>
        public void Add(XYPoint point)
        {
            if (HasElements)
                throw new Exception("Can not add nodes when tree has elements");
            bool added = _head.Add(point);
            if (added)
                _numNodes++;
        }


        /// <summary>
        /// Add element to the search tree.
        /// </summary>
        /// <param name="element">Element to add</param>
        /// <param name="extent">Extent that contains the element</param>
        public void AddElement(T element, XYExtent extent)
        {
            Leaf<T> leaf = new Leaf<T>
                             {
                                 Element = element,
                                 Extent = extent,
                             };
            _head.Add(leaf);
            _numElmts++;
        }

        /// <summary>
        /// Find elements with extends that overlaps the provided <paramref name="extent"/>
        /// </summary>
        /// <param name="extent">Extent to look for elements within</param>
        /// <returns>A list of elements with overlapping extents</returns>
        public ICollection<T> FindElements(XYExtent extent)
        {
            List<T> elmts = new List<T>();
            _head.FindElements(extent, elmts);
            return (elmts);
        }

        /// <summary>
        /// Returns the depth of the search tree
        /// </summary>
        public int Depth
        {
            get
            {
                return (_head.Depth(0));
            }
        }

        /// <summary>
        /// Returns the maximum number of elements in one
        /// search tree node
        /// </summary>
        public int MaxElementsInNode
        {
            get
            {
                return (_head.MaxElementsInNode());
            }
        }

        /// <summary>
        /// Returns the total number of nodes in the search tree
        /// </summary>
        public int TreeNodes
        {
            get
            {
                return (_head.Nodes());
            }
        }

        class TreeNode
        {
            public int MaxPointsPerNode = 10;

            private readonly XYExtent _extent;
            private TreeNode[] _children;
            private List<XYPoint> _points = new List<XYPoint>();
            private readonly List<Leaf<T>> _elements = new List<Leaf<T>>();

            public TreeNode(XYExtent extent)
            {
                _extent = extent;
            }

            public bool HasChildren
            {
                get { return (_children != null); }
            }

            public bool Add(XYPoint point)
            {
                bool added = false;
                // Check if inside this domain
                if (!_extent.Contains(point.X, point.Y))
                    return false;

                // If has children, add recursively
                if (HasChildren)
                {
                    foreach (TreeNode child in _children)
                    {
                        added |= child.Add(point);
                    }
                }
                else // it does not have children, add it here
                {
                    // Check if it already exists
                    foreach (XYPoint existingPoint in _points)
                    {
                        if (point.X == existingPoint.X && point.Y == existingPoint.Y)
                            return false; // It did exist, do nothing
                    }
                    // Add point
                    _points.Add(point);
                    added = true;

                    // Check if we should subdivide
                    if (_points.Count > MaxPointsPerNode)
                    {
                        SubDivide();
                    }
                }
                return (added);
            }

            private void SubDivide()
            {
                // Create children
                _children = new TreeNode[4];

                double xMid = 0.5 * (_extent.XMin + _extent.XMax);
                double yMid = 0.5 * (_extent.YMin + _extent.YMax);
                _children[0] = new TreeNode(new XYExtent(xMid, _extent.XMax, yMid, _extent.YMax));
                _children[1] = new TreeNode(new XYExtent(_extent.XMin, xMid, yMid, _extent.YMax));
                _children[2] = new TreeNode(new XYExtent(_extent.XMin, xMid, _extent.YMin, yMid));
                _children[3] = new TreeNode(new XYExtent(xMid, _extent.XMax, _extent.YMin, yMid));

                // Add points of this node to the new children
                foreach (XYPoint point in _points)
                {
                    foreach (TreeNode child in _children)
                    {
                        child.Add(point);
                    }
                }

                // Delete points of this node
                _points.Clear();
                _points = null;
            }

            public void Add(Leaf<T> elmtLeaf)
            {
                // If no overlap, do not add it here
                if (!_extent.Overlaps(elmtLeaf.Extent))
                    return;

                if (HasChildren)
                {
                    foreach (TreeNode child in _children)
                    {
                        child.Add(elmtLeaf);
                    }
                }
                else
                {
                    _elements.Add(elmtLeaf);
                }
            }

            public void FindElements(XYExtent extent, List<T> elmts)
            {
                // If no overlap, just return
                if (!_extent.Overlaps(extent))
                    return;

                // If has children, ask those
                if (HasChildren)
                {
                    foreach (TreeNode child in _children)
                    {
                        child.FindElements(extent, elmts);
                    }
                }
                else // No children, search in elements of this node.
                {
                    foreach (Leaf<T> elmtLeaf in _elements)
                    {
                        if (elmtLeaf.Extent.Overlaps(extent))
                        {
                            // Check if it is already there
                            if (!elmts.Contains(elmtLeaf.Element))
                                elmts.Add(elmtLeaf.Element);
                        }
                    }
                }
            }

            public int Depth(int i)
            {
                int mydepth = i + 1;
                if (!HasChildren)
                    return (mydepth);

                int depth = mydepth;
                foreach (TreeNode child in _children)
                {
                    depth = Math.Max(depth, child.Depth(mydepth));
                }
                return (depth);
            }

            public int Nodes()
            {
                if (!HasChildren)
                    return (1);

                int count = 1;
                foreach (TreeNode child in _children)
                {
                    count += child.Nodes();
                }
                return (count);
            }

            public int MaxElementsInNode()
            {
                if (!HasChildren)
                    return (_elements.Count);

                int count = 0;
                foreach (TreeNode child in _children)
                {
                    count = Math.Max(count, child.MaxElementsInNode());
                }
                return (count);
            }
        }

        /// <summary>
        /// Build a search tree based on an <see cref="IElementSet"/>, containing
        /// element index references.
        /// </summary>
        /// <param name="elmtSet">Element set to build search tree around</param>
        /// <returns>Search tree</returns>
        public static XYElementSearchTree<int> BuildSearchTree(IElementSet elmtSet)
        {
            // Calculate start extent
            XYExtent extent = new XYExtent();
            int elementCount = elmtSet.ElementCount;
            for (int ielmt = 0; ielmt < elementCount; ielmt++)
            {
                int vertixCount = elmtSet.GetVertexCount(ielmt);
                for (int ivert = 0; ivert < vertixCount; ivert++)
                {
                    double x = elmtSet.GetVertexXCoordinate(ielmt, ivert);
                    double y = elmtSet.GetVertexYCoordinate(ielmt, ivert);
                    extent.Include(x, y);
                }
            }
            // Create and build search tree, based on all vertex coordinates
            XYElementSearchTree<int> tree = new XYElementSearchTree<int>(extent);
            for (int ielmt = 0; ielmt < elementCount; ielmt++)
            {
                int vertixCount = elmtSet.GetVertexCount(ielmt);
                for (int ivert = 0; ivert < vertixCount; ivert++)
                {
                    double x = elmtSet.GetVertexXCoordinate(ielmt, ivert);
                    double y = elmtSet.GetVertexYCoordinate(ielmt, ivert);
                    tree.Add(new XYPoint(x, y));
                }
            }

            // Add elements to the search tree
            for (int ielmt = 0; ielmt < elementCount; ielmt++)
            {
                int vertixCount = elmtSet.GetVertexCount(ielmt);
                XYExtent elmtExtent = new XYExtent();
                for (int ivert = 0; ivert < vertixCount; ivert++)
                {
                    double x = elmtSet.GetVertexXCoordinate(ielmt, ivert);
                    double y = elmtSet.GetVertexYCoordinate(ielmt, ivert);
                    elmtExtent.Include(x, y);
                }
                tree.AddElement(ielmt, elmtExtent);
            }
            return (tree);
        }

    }
}