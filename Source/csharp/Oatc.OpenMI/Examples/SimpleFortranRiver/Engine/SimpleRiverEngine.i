c ///////////////////////////////////////////////////////////
c //
c // namespace:  
c // purpose: Simple Fortran river model, used as example in the OpenMI guidelines
c // file: SimpleRiverEngine.i
c //
c ///////////////////////////////////////////////////////////
c //
c //    Copyright (C) 2005 OpenMI Association
c //
c //    This library is free software; you can redistribute it and/or
c //    modify it under the terms of the GNU Lesser General Public
c //    License as published by the Free Software Foundation; either
c //    version 2.1 of the License, or (at your option) any later version.
c //
c //    This library is distributed in the hope that it will be useful,
c //    but WITHOUT ANY WARRANTY; without even the implied warranty of
c //    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
c //    Lesser General Public License for more details.
c //
c //    You should have received a copy of the GNU Lesser General Public
c //    License along with this library; if not, write to the Free Software
c //    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
c //    or look at URL www.gnu.org/licenses/lgpl.html
c //
c //    Contact info: 
c //      URL: www.openmi.org
c //	Email: sourcecode@openmi.org
c //	Discussion forum available at www.sourceforge.net
c //
c //      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
c //
c ///////////////////////////////////////////////////////////
c //
c //  Original author: Thomas Clausen, DHI - Water & Environment, Horsholm, Denmark
c //  Created on:      6 April 2005
c //  Version:         1.0.0 
c //
c //  Modification history:
c //  
c //
c ///////////////////////////////////////////////////////////


c --- Parameters

      integer
     +  MaxNoNodes,
     +  MaxNoTimesteps,
     +  MaxNoMessages,
     +  Len_FileNames,
     +  Len_Texts

      parameter(
     +  MaxNoNodes     = 100,
     +  MaxNoTimesteps = 1000,
     +  MaxNoMessages  =  10,
     +  Len_FileNames  = 256,
     +  Len_Texts      = 128)


c --- Simple integers

      integer
     +  numberOfNodes,
     +  numberOfTimeSteps,
     +  CurrentTimeStepNumber,
     +  NoMessages,
     +  iuIn,iuOut

      common /SimpInt/
     +  numberOfNodes,
     +  numberOfTimeSteps,
     +  CurrentTimeStepNumber,
     +  NoMessages,
     +  iuIn,iuOut


c --- Simple R8

      real*8
     +  timeStepLength,
     +  initialFlow,
     +  runoff,
     +  leakFactor

      common /SimpR8/
     +  timeStepLength,
     +  initialFlow,
     +  runoff,
     +  leakFactor


c --- Simple text strings

      character(Len_FileNames)
     +  filePath,
     +  simFileName
      character(128)
     +  SetupID,
     +  SimulationStartDate

      common /SimpChar/
     +  filePath,
     +  simFileName,
     +  SetupID,
     +  SimulationStartDate

c --- R8 arrays

      real*8
     +  flows(MaxNoNodes),
     +  storages(MaxNoNodes),
     +  boundaryInflows(MaxNoNodes,MaxNoTimesteps),
     +  xCoordinates(MaxNoNodes),
     +  yCoordinates(MaxNoNodes)

      common /ArrR8/
     +  flows,
     +  storages,
     +  boundaryInflows,
     +  xCoordinates,
     +  yCoordinates

c --- Character arrays

c --- Some error messages contain a file name
      character(Len_FileNames)
     +  Messages(MaxNoMessages)
     
      common /ArrChar/
     +  Messages