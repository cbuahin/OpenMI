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
#region Copyright
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2005 HarmonIT Consortium
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      December 1st 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion

using System;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;


namespace Oatc.OpenMI.Examples.SimpleGroundModel2
{
	/// <summary>
	/// Implementation of the IElementSet interface. The class
	/// is tailored to create ElementSet for the Mike She model. This element set is
	/// the 2D regular finite volume grid in Mike She.
	/// </summary>
	public class Grid : IElementSet
	{
		GridInfo _gridInfo;
        Describable _describable = new Describable("Regular Grid", "Regular Grid");

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="gridInfo">DHI.OpenMI.MikeShe.GridInfo Object</param>
		public Grid(GridInfo gridInfo)
		{
			_gridInfo = gridInfo;
            _describable.Description = _gridInfo.ToString();
	    }

		/// <summary>
		/// ElementSet version number. For Mike She the elementset remeains the same for 
		/// the entire simulation, thus this method always return 0.
		/// </summary>
		public int Version
		{
			get
			{
				return 0;
			}
		}

   
		/// <summary>
		/// Return elementID
		/// </summary>
		/// <param name="elementIndex"></param>
		/// <returns>Index in the ElementSet</returns>
		public IIdentifiable GetElementId(int elementIndex)
		{
			return(new Identifier(_gridInfo.CellCenterXCoords[elementIndex].ToString()+"_"+_gridInfo.CellCenterYCoords[elementIndex].ToString()));
		}

		/// <summary>
		/// ElementType. Always ElementType.XYPolygon
		/// </summary>
		public ElementType ElementType
		{
			get { return ElementType.Polygon; }
		}

		/// <summary>
		/// Returns x-coordinates for a requested vertex in the ElementSet
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <param name="vertexIndex">Vertex index</param>
		/// <returns>x-coordinate</returns>
		public double GetVertexXCoordinate(int elementIndex, int vertexIndex)
		{
            double angle = Math.PI * _gridInfo.GridAngle / 180.0;
            double x = GetNonRotatedXCoordinate(elementIndex,vertexIndex);

			if (_gridInfo.GridAngle > 0)
            {
                double x0 = this._gridInfo.CellCenterXCoords[0] - this._gridInfo.CellSize * 0.5;
                double y0 = this._gridInfo.CellCenterYCoords[0] - this._gridInfo.CellSize * 0.5;
                double y = GetNonRotatedYCoordinate(elementIndex,vertexIndex);

                double alpha;
                
                if ((x - x0) == 0)
                {
                    alpha = Math.PI / 2.0;
                }
                else 
                {
                    alpha = Math.Atan((y-y0)/(x-x0));
                }

                x = Math.Sqrt((x - x0)*(x - x0) + (y - y0)*(y-y0)) * Math.Cos(angle + alpha) + x0;

            }

            return x;
		}
		
		/// <summary>
		/// Returns the y-coordinate for the requested vertex in the ElementSet
		/// </summary>
		/// <param name="elementIndex">element index</param>
		/// <param name="vertexIndex">vertex index</param>
		/// <returns>y-coordinate</returns>
		public double GetVertexYCoordinate(int elementIndex, int vertexIndex)
		{
            double angle = Math.PI * _gridInfo.GridAngle / 180.0;
            double y = GetNonRotatedYCoordinate(elementIndex,vertexIndex);

            if (_gridInfo.GridAngle > 0)
            {
                double y0 = this._gridInfo.CellCenterYCoords[0] - this._gridInfo.CellSize * 0.5;
                double x0 = this._gridInfo.CellCenterXCoords[0] - this._gridInfo.CellSize * 0.5;
                double x = GetNonRotatedXCoordinate(elementIndex,vertexIndex);

                double alpha;
                
                if ((x - x0) == 0)
                {
                    alpha = Math.PI / 2.0;
                }
                else 
                {
                   alpha = Math.Atan((y-y0)/(x-x0));
                }

                y = Math.Sqrt((x - x0)*(x - x0) + (y - y0)*(y-y0)) * Math.Sin(angle + alpha) + y0;
            }

            return y;
		}

       

		/// <summary>
		/// This elementset is only defined in two dimensions. Calling this method will 
		/// give an exception
		/// </summary>
		/// <param name="elementIndex">ElementSet index</param>
		/// <param name="vertexIndex">Vertex index</param>
		/// <returns>Always throws an exception</returns>
		public double GetVertexZCoordinate(int elementIndex, int vertexIndex)
		{
			throw new System.Exception("Z coordinate is not defined for an XYPolygon");
		}

		/// <summary>
		/// ElementCount. The number of elements in the ElementSet
		/// </summary>
		public int ElementCount
		{
			get
			{
				return _gridInfo.NumberOfCells;
			}
		}

		/// <summary>
		/// Return the number of vertex in the requested element in the ElementSet.
		/// In this implementation this method always return 4.
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <returns>returns always 4</returns>
		public int GetVertexCount(int elementIndex)
		{
			return 4;
		}
        
		/// <summary>
		/// Returns the index number of an element in the ElementSet
		/// </summary>
		/// <param name="elementID">Element ID</param>
		/// <returns>index</returns>
		public int GetElementIndex(IIdentifiable elementID)
		{
			string xID = "-999";
			string yID = "-999";
			char [] spilt = new char[]{'_'};
			int ctr = 0;
			foreach(string subString in elementID.Id.Split(spilt))
			{
				if(ctr == 0) xID = subString;
				if(ctr == 1) yID = subString;
				ctr ++;
			}

			return (GetElementIndex(double.Parse(xID),double.Parse(yID)));
		}

		/// <summary>
		/// Returns the ElementIndex for a specified location (x,y)
		/// </summary>
		/// <param name="x">x-coordinate</param>
		/// <param name="y">y-coordinate</param>
		/// <returns>Element index</returns>
		private int GetElementIndex(double x,double y)
		{
			for (int iref = 0; iref<_gridInfo.NumberOfCells; iref++)
			{
				if((Math.Abs(_gridInfo.CellCenterXCoords[iref]-x)<double.Epsilon) && (Math.Abs(_gridInfo.CellCenterYCoords[iref]-y) < double.Epsilon))
				{
					return iref;
				}
			}

			return -999;
		}


		/// <summary>
		/// This ElementSet i two-dimensional, so this method will always return null
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <param name="faceIndex">face index</param>
		/// <returns>always returns null</returns>
		public int[] GetFaceVertexIndices(int elementIndex, int faceIndex)
		{
		      return null;
		}

		/// <summary>
		/// This elementSet is two-dimensional, so this method always returns zero
		/// </summary>
		/// <param name="elementIndex">Element index</param>
		/// <returns>always return zero</returns>
		public int GetFaceCount(int elementIndex)
		{
			return 0;
		}

        private double GetNonRotatedXCoordinate(int elementIndex, int vertexIndex)
        {
            if(vertexIndex==0 || vertexIndex==3 )
            {
                return (_gridInfo.CellCenterXCoords[elementIndex]-_gridInfo.CellSize/2);
            }
            else if(vertexIndex==1 || vertexIndex==2)
            {
                return (_gridInfo.CellCenterXCoords[elementIndex]+_gridInfo.CellSize/2);
            }
            else
            {
                throw new System.Exception("Vertex index outside range [0;4]. Vertex index was:"+vertexIndex.ToString());
            }
        }
		
        
        private double GetNonRotatedYCoordinate(int elementIndex, int vertexIndex)
        {
            if(vertexIndex==0 || vertexIndex==1 )
            {
                return (_gridInfo.CellCenterYCoords[elementIndex]-_gridInfo.CellSize/2);
            }
            else if(vertexIndex==2 || vertexIndex==3)
            {
                return (_gridInfo.CellCenterYCoords[elementIndex]+_gridInfo.CellSize/2);
            }
            else
            {
                throw new System.Exception("Vertex index outside range [0;4]. Vertex index was:"+vertexIndex.ToString());
            }
        }


        #region IElementSet Members

        public string SpatialReferenceSystemWkt
        {
            get { return ""; }
        }

        public bool HasZ
        {
            get { return false; }
        }

        public bool HasM
        {
            get { return false; }
        }

        public double GetVertexMCoordinate(int elementIndex, int vertexIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _describable.Caption; }
            set { _describable.Caption = value; }
        }

        string IDescribable.Description
        {
            get { return _describable.Description; }
            set { _describable.Description = value; }
        }

        #endregion
    }
}