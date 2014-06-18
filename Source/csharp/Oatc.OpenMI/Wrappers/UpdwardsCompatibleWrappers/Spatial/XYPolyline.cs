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
// file: XYPolyline.cs
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

using System;
using System.Collections;

namespace Oatc.UpwardsComp.Spatial
{
	/// <summary>
	/// XYPolyline is a collection of points (at least 2) connected with straigth lines.
	/// Polylines are typically used for presentation of 1D data, river networks e.t.c.
	/// </summary>

  public class XYPolyline
  {
    private readonly ArrayList _points;


    //=====================================================================
    // XYPolyline(): void
    //===================================================================== 
    /// <summary>
    /// Constructor.
    /// </summary>
    public XYPolyline()
	  {
		  _points = new ArrayList();
	  }

    //=====================================================================
    // XYPolyline(XYPolyline xyPolyline): void
    //===================================================================== 
    /// <summary>
    /// Constructor. Copies the contents of the xyPolyline parameter.
    /// </summary>
    /// <param name="xyPolyline">Polyline to copy.</param>
    /// <returns>None</returns>
    public XYPolyline(XYPolyline xyPolyline)
	  {
		  _points = new ArrayList();

		  foreach (XYPoint xypoint in xyPolyline.Points)
		  {
			  _points.Add(new XYPoint(xypoint.X, xypoint.Y));
		  }

	  }

	  
    //=====================================================================
    // Points : ArrayList
    //===================================================================== 
    /// <summary>
    /// Read only property holding the list of points.
    /// </summary>
    public ArrayList Points
	  {
		  get
		  {
			  return _points;
		  }
	  }

    //=====================================================================
    // GetX(int index) : double
    //=====================================================================
    /// <summary>
    /// Retrieves the x-coordinate of the index´th line point.
    /// </summary>
    /// <param name="index">Index number of the point.</param>
    /// <returns>X-coordinate of the index´th point in the polyline.</returns>
    public double GetX(int index)
	  {
		  return ((XYPoint) _points[index]).X;
	  }

    //=====================================================================
    // GetY(int index) : double
    //=====================================================================
    /// <summary>
    /// Retrieves the y-coordinate of the index´th line point.
    /// </summary>
    /// <param name="index">Index number of the point.</param>
    /// <returns>Y-coordinate of the index´th point in the polyline.</returns>
    public double GetY(int index)
	  {
	    return ((XYPoint) _points[index]).Y;
	  }
    
    //=====================================================================
    // GetLine(int lineNumber) : XYLine
    //=====================================================================
    /// <summary>
    /// Retrieves the lineNumber´th line segment of the polyline. The index 
    /// list is zero based.
    /// </summary>
    /// <param name="lineNumber">Index number of the line to retrieve.</param>
    /// <returns>The lineNumber´th line segment of the polyline.</returns>
    public XYLine GetLine(int lineNumber)
    {
 		  return new XYLine((XYPoint)_points[lineNumber], (XYPoint)_points[lineNumber+1]);
    }

  
    //=====================================================================
    // GetLength() : double
    //=====================================================================
    /// <summary>
    /// Calculates the length of the polyline.
    /// </summary>
    /// <returns>Length of the polyline.</returns>
    public double GetLength()
    {
		  double length = 0;	
		  for (int i = 0; i < _points.Count - 1; i++)
		  {
			  length += GetLine(i).GetLength();
  		}	
      return length;  
    }

    //=====================================================================
    // Equals(Object obj) : Bool
    //===================================================================== 
    /// <summary>
    /// Compares the object type and the coordinates of the object and the 
    /// object passed as parameter.
    /// </summary>
    /// <returns>True if object type is XYPolyline and the coordinates are 
    /// equal to to the coordinates of the current object. False otherwise.</returns>
    public override bool Equals(Object obj) 
    {
      if (obj == null || GetType() != obj.GetType()) 
      {
        return false;
      }
      XYPolyline e = (XYPolyline) obj;
      if (_points.Count!=e.Points.Count)
      {  
        return false;
      }
      for (int i=0;i<_points.Count;i++)
      {
        if (!_points[i].Equals(e.Points[i]))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return 0;
    }

    //=====================================================================
    // Validate() : void
    //=====================================================================
    /// <summary>
    /// The validate method check if the XYPolyline is valid. The checks 
    /// made are: 
    ///   - is number of points >= 2
    ///   - is the length of all line segments positiv
    /// Exception is raised if the constraints are not met.
    /// </summary>
    public void Validate()
    {
      if(_points.Count < 2)
      {
        throw new Exception("Number of vertices in polyline element is less than 2.");
      }
      for (int j = 0; j < _points.Count-1; j++)
      {
        if (GetLine(j).GetLength() == 0)
        {
          throw new Exception("Length of line segment no: "+
            j+" (0-based) of XYPolyline is zero.");
        }
      }

    }
  }
}
