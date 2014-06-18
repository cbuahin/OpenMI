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
// namespace: org.OpenMI.Utilities.Spatial 
// purpose: Utility for spatial data-operations within the LinkableComponents wrappers
// file: ElementSetChecker.cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
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
//  Original author: Peter Sinding & Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion

using OpenMI.Standard2.TimeSpace;

namespace Oatc.UpwardsComp.Spatial
{
	/// <summary>
	/// Summary description for ElementSetChecker.
	/// </summary>
	public class ElementSetChecker
	{

    //===============================================================================================
    //CheckElementSet(IElementSet elementSet): void static
    //===============================================================================================
    /// <summary>
    /// Static method that validates an object with an IElementSet interface. The method 
    /// raises an Exception in case IElementSet does not describe a valid ElementSet.
    /// The checks made are:
    ///   <p>ElementType: Check</p>
    ///   <p>XYPoint:     Only one vertex in each element.</p>
    ///   <p>XYPolyline:  At least two vertices in each element.</p>
    ///   <p>             All line segments in each element has length > 0</p>
    ///   <p>XYPolygon:   At least three vertices in each element.</p>
    ///   <p>             Area of each element is larger than 0</p>
    ///   <p>             All line segments in each element has length > 0</p>
    ///   <p>             No line segments within an element crosses.</p>
    /// </summary>
    /// 
    /// <param name="elementSet">Object that implement the IElementSet interface</param>
    /// 
    /// <returns>
    /// The method has no return value.
    /// </returns>
    public static void CheckElementSet(IElementSet elementSet)
		{
      try
      {
        if(elementSet.ElementType == ElementType.Point)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {              
              if(elementSet.GetVertexCount(i) != 1)
              {
                throw new System.Exception("Number of vertices in point element is different from 1.");
              }
            }
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementId(i),e);
            }
          }
        }
        else if(elementSet.ElementType == ElementType.PolyLine)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {
              XYPolyline xypolyline = new XYPolyline();
              for (int j = 0; j < elementSet.GetVertexCount(i); j++)
              {
                XYPoint xypoint = new XYPoint(elementSet.GetVertexXCoordinate(i,j),elementSet.GetVertexYCoordinate(i,j));
                xypolyline.Points.Add(xypoint);
              }
              xypolyline.Validate();
            }
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementId(i),e);
            }
          }
        }
        else if(elementSet.ElementType == ElementType.Polygon)
        {
          for (int i = 0; i < elementSet.ElementCount; i++)
          {
            try
            {
              XYPolygon xypolygon = new XYPolygon();
              for (int j = 0; j < elementSet.GetVertexCount(i); j++)
              {
                XYPoint xypoint = new XYPoint(elementSet.GetVertexXCoordinate(i,j),elementSet.GetVertexYCoordinate(i,j));
                xypolygon.Points.Add(xypoint);
              }
              xypolygon.Validate();
            }         
            catch (System.Exception e)
            {
              throw new System.Exception("ElementID = "+elementSet.GetElementId(i),e);
            }
          }
        }
      }
      catch (System.Exception e)
      {
        throw new System.Exception("ElementSet with ID = "+elementSet.Caption +" is invalid",e);
      }
		}
	}
}
