c ///////////////////////////////////////////////////////////
c //
c // namespace:  
c // purpose:  Simple Fortran river model, used as example in the OpenMI guidelines
c // file: SimpleRiverMain.f
c //
c ///////////////////////////////////////////////////////////
c //
c //    Copyright (C) 2005 OpenMI Association
c //
c //    Permission is hereby granted, free of charge, to any person obtaining a
c //    copy of this software and associated documentation files (the "Software"), 
c //    to deal in the Software without restriction, including without limitation 
c //    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
c //    and/or sell copies of the Software, and to permit persons to whom the 
c //    Software is furnished to do so, subject to the following conditions:
c //
c //    The above copyright notice and this permission notice shall be included in 
c //    all copies or substantial portions of the Software.
c //    
c //    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
c //    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
c //    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
c //    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
c //    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
c //    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
c //    DEALINGS IN THE SOFTWARE.
c //
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
c //  Created on:      06 April 2005
c //  Version:         1.0.0 
c //
c //  Modification history:
c //  
c //
c ///////////////////////////////////////////////////////////


c ----------------------------------------------------------------------
c --- PROGRAM: SimpleRiverFortranEngine.
c --- PURPOSE: Calling SimpleRiverFortranEngineDll.
c ----------------------------------------------------------------------

      program SimpleRiverFortranEngine
      implicit none
c --- Functions
      logical RunSimulation,GetMessage
      integer NumberOfMessages
      logical GetXcoordinate
c --- Local
      logical lok
      integer i,n
      character(256) message
      real*8 kurt

c --- Run the simulation
      write(*,'(a,/,a)')'Calling RunSimulation...',' '
      lok=RunSimulation()
      lok=GetXCoordinate(1,kurt) 
c --- Any messages ?
      n=NumberOfMessages()
      do i=1,n
        if(GetMessage(i,Message))write(*,'(a)')trim(message)
      enddo

c --- Termination      
      if(lok)then
        write(*,'(/,a)')'Normal termination.'
      else
        write(*,'(/,a)')'Abnormal termination.'
      endif

      end program SimpleRiverFortranEngine
