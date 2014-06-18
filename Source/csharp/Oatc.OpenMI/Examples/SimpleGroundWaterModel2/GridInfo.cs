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
using System.Diagnostics;

namespace Oatc.OpenMI.Examples.SimpleGroundModel2
{
	/// <summary>
	/// Basic information about the MikeShe finite volume grid (only 2D)
	/// </summary>
	public class GridInfo
	{
		private int      _numberOfCells;		// Number of celles in the finite volume grid including boundary cells
		private double[] _cellCenterXCoords;    // X-coordinates for all grid cells, starting from buttom left and going up column by column
		private double[] _cellCenterYCoords;    // Y-coordinates for all grid cells, starting from buttom left and going up column by column
		private double   _cellSize;             // The size of each individual kvadratic grid cell (all cells are the same size) [meters]
        private double   _GridAngle;            // rotation angle of the grid in deg.
        int _nx;
        int _ny;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="numberOfCells">Number of celles in the finite volume grid including boundary cells</param>
		/// <param name="cellCenterXCoords">X-coordinates for all grid cells, starting from buttom left and going up column by column</param>
		/// <param name="cellCenterYCoords">Y-coordinates for all grid cells, starting from buttom left and going up column by column</param>
		/// <param name="cellSize">The size of each individual kvadratic grid cell (all cells are the same size) [meters]</param>
        public GridInfo(int numberOfCells, double[] cellCenterXCoords, double[] cellCenterYCoords, double cellSize)
		{
			_cellCenterXCoords = cellCenterXCoords;
			_cellCenterYCoords = cellCenterYCoords;
			_cellSize          = cellSize;
			_numberOfCells     = numberOfCells;
            _GridAngle         = 0;
            _nx = cellCenterXCoords.Length;
            _ny = cellCenterYCoords.Length;

            Debug.Assert(_nx * _ny == _numberOfCells);
		}

        public override string ToString()
        {
            return string.Format("Cells {0}, Size {1}, Angle {2}",
                _numberOfCells, _cellSize, _GridAngle);
        }

        public GridInfo( double origX, double origY, double cellSize, int nx, int ny, double gridAngle)
        {
            _numberOfCells = nx * ny;
            _cellCenterXCoords = new double[nx * ny];
            _cellCenterYCoords = new double[ny * ny];
            _cellSize = cellSize;
            _GridAngle = gridAngle;
            _nx = nx;
            _ny = ny;

            int index = 0;
            for (int i = 0; i < nx; i++)
            {
                for (int n = 0; n < ny; n++)
                {
                    _cellCenterXCoords[index] = origX + i * cellSize + 0.5 * cellSize;
                    index++;
                }
            }

            index = 0;
            for (int i = 0; i < nx; i++)
            {
                for (int n = 0; n < ny; n++)
                {
                    _cellCenterYCoords[index] = origY + n * cellSize + 0.5 * cellSize;
                    index++;
                }
            }
        }

		/// <summary>
		/// Number of celles in the finite volume grid including boundary cells
		/// </summary>
		public int NumberOfCells
		{
			get
			{
				return _numberOfCells;
			}
		}

        public int NX { get { return _nx; } }
        public int NY { get { return _ny; } }

		/// <summary>
		/// The size of each individual kvadratic grid cell (all cells are the same size) [meters]
		/// </summary>
		public double CellSize
		{
			get
			{
				return _cellSize;
			}
		}

		/// <summary>
		/// X-coordinates for all grid cells, starting from buttom left and going up column by column
		/// </summary>
		public double[] CellCenterXCoords
		{
			get
			{
				return _cellCenterXCoords;
			}
		}

		/// <summary>
		/// Y-coordinates for all grid cells, starting from buttom left and going up column by column
		/// </summary>
		public double[] CellCenterYCoords
		{
			get
			{
				return _cellCenterYCoords;
			}
		}

        public double GridAngle
        {
            get
            {
                return _GridAngle;
            }
        }
               
                     

	}
}
