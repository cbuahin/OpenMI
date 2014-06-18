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
// file: XYPoint.cs
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

namespace Oatc.UpwardsComp.Spatial
{
	/// <summary>
	/// XYPoint is simply a x and a y coordinate.
	/// </summary>
	public class XYPoint
	{
		private double _x;
		private double _y;

		//=====================================================================
		// XYPoint(): void
		//===================================================================== 
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <returns>None</returns>
		public XYPoint()
		{
			_x = -9999;
			_y = -9999;
		}

		//=====================================================================
		// XYPoint(): void
		//===================================================================== 
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <returns>None</returns>
		public XYPoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		//=====================================================================
		// XYPoint(): void
		//===================================================================== 
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <returns>None</returns>
		public XYPoint(XYPoint xypoint)
		{
			_x = xypoint.X;
			_y = xypoint.Y;
		}

		//=====================================================================
		// public X : double
		//===================================================================== 
		/// <summary>
		/// Read/Write property describing the x-coordinate of the point.
		/// </summary>
		public double X
		{
			get { return _x; }
			set { _x = value; }
		}

		//=====================================================================
		// public Y : double
		//===================================================================== 
		/// <summary>
		/// Read/Write property describing the y-coordinate of the point.
		/// </summary>
		public double Y
		{
			get { return _y; }
			set { _y = value; }
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
				return false;
			else
				return ((XYPoint) obj).X == X && ((XYPoint) obj).Y == Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return 0;
		}
	}
}