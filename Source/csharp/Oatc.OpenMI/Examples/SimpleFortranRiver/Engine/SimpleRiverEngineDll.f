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

c --- <summary>
c --- Simple River is a model engine used to test the org.OpenMI.Utilities.Wrapper implementations
c ---
c --- </summary>


!DEC$ DEFINE VF66
c ----------------------------------------------------------------------
      BLOCK DATA SimpleRiverEngineDll0
c ----------------------------------------------------------------------
      include 'SimpleRiverEngine.i'

      data FilePath / ' ' /

      data numberOfNodes,numberOfTimeSteps,CurrentTimeStepNumber
     +  /              0,                0,                    0 /

      data NoMessages,iuIn,iuOut
     +  /           0,  10,   11 /

      end

c ----------------------------------------------------------------------
      logical function RunSimulation()
c ----------------------------------------------------------------------

!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::RunSimulation
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_RUNSIMULATION"::RunSimulation
!DEC$ ENDIF


      implicit none
      include 'SimpleRiverEngine.i'
c --- Functions
      logical Initialize,PerformTimeStep,Finish
c --- Local
      logical lok
      integer i

      lok=.false.

      if(.not.Initialize('C:\HarmonIT\SourceCode\DotNet\Examples\'//
     +               'ModelComponents\SimpleRiver\UnitTest\Data\Rhine')
     +  )goto 9999

      do i=1,numberOfTimeSteps
        if(.not.PerformTimeStep())goto 9999
      enddo

      if(.not.Finish())goto 9999

      lok=.true.
 9999 RunSimulation=lok
      return
      end function RunSimulation

c ----------------------------------------------------------------------
      logical function ReadInputFiles()
c ----------------------------------------------------------------------
      implicit none
      include 'SimpleRiverEngine.i'
c --- Functions
      character(80) I4ToC
c --- Local
      logical lok
      integer irc,i,j
      character(Len_Texts) itemtext
      character(Len_FileNames) bndFileName
      character(Len_FileNames) ntwFileName
      character(Len_FileNames) fname
      character(256) line

      lok=.false.

c --- Read simulation file ---

      fname=trim(FilePath)//'\'//trim(SimFileName)

      open(iuIn,file=fname,status='old',iostat=irc)
      if(irc.ne.0)goto 910

c --- Bypass header line + empty line + Setup ID header line. Read Setup ID.
      itemtext='header line'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      itemtext='Setup ID'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)SetupID
c --- Bypass empty line + Start Time header. Read start date.
      itemtext='Simulation start date'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)SimulationStartDate
c --- Bypass empty line + Time Step header. Read Time Step Length
      itemtext='Time Step Length'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)timeStepLength
c --- Bypass empty line + header. Read initial flow
      itemtext='Initial flow'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)initialFlow
c --- Bypass empty line + header. Read global runoff
      itemtext='Global runoff'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)runoff
c --- Bypass empty line + header. Read leakFactor
      itemtext='Leakage factor'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)leakFactor
c --- Bypass empty line + header. Read ntw filename
      itemtext='ntw FileName'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)ntwFileName
c --- Bypass empty line + header. Read bnd filename
      itemtext='bnd FileName'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)bndFileName

      close(iuIn)

c --- Read network file --

      fname=trim(FilePath)//'\'//trim(ntwFileName)

      open(iuIn,file=fname,status='old',iostat=irc)
      if(irc.ne.0)goto 912

c --- Bypass header line + empty line + Number nodes header line. Read no. of nodes.
      itemtext='Number of nodes'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)numberOfNodes
c --- Check no. of nodes
      if(numberOfNodes.lt.2)then
        call SetMessage('ERROR  : Invalid no. of nodes : '//
     +    trim(I4ToC(numberOfNodes))//' - must be > 1.')
        goto 9999
      elseif(numberOfNodes.gt.MaxNoNodes)then
        call SetMessage(
     +    'ERROR  : Insufficient dimension for no. of nodes : '//
     +    trim(I4ToC(MaxNoNodes)))
        call SetMessage(
     +    '         - required : '//trim(I4ToC(numberOfNodes)))
        goto 9999
      endif
c --- Bypass empty line + header line. Read Node X- and Y-coordinates
      itemtext='Node X- & Y-coordinates'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      do i=1,numberOfNodes
        read(iuIn,*,end=911,err=912)xCoordinates(i),yCoordinates(i)
      enddo

      close(iuIn)

c --- Read boundary file --

      fname=trim(FilePath)//'\'//trim(bndFileName)

      open(iuIn,file=fname,status='old',iostat=irc)
      if(irc.ne.0)goto 912

c --- Bypass header line + empty line + Number timesteps header line. Read no. of time steps.
      itemtext='Number of time steps'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,*    ,end=911,err=912)numberOfTimeSteps
c --- Check no. of time steps
      if(numberOfTimeSteps.lt.1)then
        call SetMessage('ERROR  : Invalid no. of Time Steps : '//
     +    trim(I4ToC(numberOfTimeSteps)))
        goto 9999
      elseif(numberOfTimeSteps.gt.MaxNoTimeSteps)then
        call SetMessage(
     +    'ERROR  : Insufficient dimension for no. of Time Steps : '//
     +    trim(I4ToC(MaxNoTimeSteps)))
        call SetMessage(
     +    '         - required : '//trim(I4ToC(numberOfTimeSteps)))
        goto 9999
      endif

c --- Bypass empty line + header line. Read inflows(all nodes,all time steps)
      itemtext='Inflows to nodes, all time steps'
      read(iuIn,'(a)',end=911,err=912)line
      read(iuIn,'(a)',end=911,err=912)line
      do j=1,numberOfTimeSteps
        read(iuIn,*,end=911,err=912)
     +    (boundaryInflows(i,j),i=1,numberOfNodes)
      enddo

      close(iuIn)

      lok=.true.
      goto 9999

c --- Label: Could not open file.
  910   call SetMessage('ERROR  : Could not open input file')
        call SetMessage('File   : '//trim(fname))
      goto 9999
c --- Label: End-of-file
  911   call SetMessage('ERROR  : End of input file')
        call SetMessage('File   : '//trim(fname))
        call SetMessage('Item   : '//trim(itemtext))
      goto 9999  
c --- Label: Input error
  912   call SetMessage('ERROR  : Input error')
        call SetMessage('File   : '//trim(fname))
        call SetMessage('Item   : '//trim(itemtext))

c --- Return (true or false)
 9999 ReadInputFiles=lok
      return
      end function ReadInputFiles


c ----------------------------------------------------------------------
      logical function InitializeOutputFiles()
c ----------------------------------------------------------------------
      implicit none
      include 'SimpleRiverEngine.i'
c --- Local
      logical lok
      integer irc,i
      character(Len_FileNames) fname

      lok=.false.

      fname=trim(FilePath)//'\SimpleRiver.out'

      open(iuOut,file=fname,iostat=irc)
      if(irc.ne.0)goto 910

      write(iuOut,'(a)',err=912)'Simple River output'
      write(iuOut,'(a)',err=912)'Setup ID: '//trim(SetupID)
      write(iuOut,'(a)',err=912)' '
      write(iuOut,'(a)',err=912)'Flow in branches [m3/sec.]'
      write(iuOut,'(a,10(a,i3),/,(12x,10(a,i3)))',err=912)
     +  '        Time',('   Branch',i,i=1,NumberOfNodes-1)

      lok=.true.
      goto 9999

c --- Label: Could not open file.
  910   call SetMessage('ERROR  : Could not open output file')
        call SetMessage('File   : '//trim(fname))
      goto 9999
c --- Label: Output error
  912   call SetMessage('ERROR  : Output error')
        call SetMessage('File   : '//trim(fname))
        call SetMessage('Item   : Header lines')

c --- Return (true or false)
 9999 InitializeOutputFiles=lok
      return
      end function InitializeOutputFiles


c ----------------------------------------------------------------------
      logical function PerformTimeStep()
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::PerformTimeStep
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_PERFORMTIMESTEP"::PerformTimeStep
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Functions
      character(80) I4ToC
c --- Local
      logical lok
      integer i,irc
      real*8  currentTime
      character(Len_FileNames) fname

      lok=.false.

c --- add water from boundaries ---
      do i=1,numberOfNodes
        storages(i) = storages(i)
     +    + boundaryInflows(i,CurrentTimeStepNumber) * timeStepLength
     +    + runoff * timeStepLength
      enddo

c --- Calculate ---
      do i=2,numberOfNodes
        flows(i-1) = (1-leakFactor) * storages(i-1)/timeStepLength
c        leaks(i-1) = leakFactor * storages(i-1)/timeStepLength
        storages(i) = storages(i) + (1-leakFactor) * storages(i-1)
c        flows(i-1) = storages(i-1)/timeStepLength
c        storages(i) = storages(i) + storages(i-1)
        storages(i-1) = 0
      enddo

c --- increment time step ---
      CurrentTimeStepNumber=CurrentTimeStepNumber+1

c --- Write output for time step ---
      currentTime = CurrentTimeStepNumber * timeStepLength
      write(iuOut,'(11f12.2,/,(12x,10f12.2))',err=912)
     +  currentTime,(flows(i),i=1,numberOfNodes-1)

      lok=.true.
      goto 9999

c --- Label: Output error
  912   inquire(iuOut,NAME=fname,iostat=irc)
        call SetMessage('ERROR  : Output error in time step no. '//
     +    trim(I4ToC(CurrentTimeStepNumber)))
        call SetMessage('File   : '//trim(fname))

 9999 PerformTimeStep=lok
      return
      end function PerformTimeStep

c ----------------------------------------------------------------------
      logical function Finish()
c ----------------------------------------------------------------------

!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::Finish
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_FINISH"::Finish
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
      logical lopen

      inquire(iuOut,Opened=lopen)
      if(lopen)close(iuOut)

      Finish=.true.
      return
      end function Finish

c ----------------------------------------------------------------------
      logical function AddInflow(index,inflow)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::AddInflow
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_ADDINFLOW"::AddInflow
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer index
      real*8  inflow

      storages(index)=storages(index)+inflow*timeStepLength

      AddInflow=.true.
      return
      end function AddInflow

c ----------------------------------------------------------------------
      logical function GetCurrentTime(Time)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetCurrentTime
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETCURRENTTIME"::GetCurrentTime
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      real*8 Time

      Time=timeStepLength*(CurrentTimeStepNumber-1)

      GetCurrentTime=.true.
      return
      end function GetCurrentTime

c ----------------------------------------------------------------------
      logical function GetTimeStepLength(dT)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetTimeStepLength
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETTIMESTEPLENGTH"::GetTimeStepLength
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      real*8 dT

      dT=timeStepLength

      GetTimeStepLength=.true.
      return
      end function GetTimeStepLength

c ----------------------------------------------------------------------
      logical function SetTimeStepLength(dT)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::SetTimeStepLength
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_SETTIMESTEPLENGTH"::SetTimeStepLength
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      real*8 dT

      timeStepLength=dT

      SetTimeStepLength=.true.
      return
      end function SetTimeStepLength

c ----------------------------------------------------------------------
      logical function GetNTimeSteps(nSteps)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetNTimeSteps
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETNTIMESTEPS"::GetNTimeSteps
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      integer nSteps

      nSteps=NumberOfTimeSteps

      GetNTimeSteps=.true.
      return
      end function GetNTimeSteps

c ----------------------------------------------------------------------
      logical function GetFlow(index,Flow)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetFlow
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETFLOW"::GetFlow
!DEC$ ENDIF
      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer index
c --- Output
      real*8 Flow

      Flow=flows(index)

      GetFlow=.true.
      return
      end function GetFlow

c ----------------------------------------------------------------------
      logical function GetModelDescr(Description)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetModelDescr
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETMODELDESCR"::GetModelDescr
!DEC$ ENDIF
      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      character(*) Description

      Description='Simple River model for: '//trim(SetupID)

      GetModelDescr=.true.
      return
      end function GetModelDescr

c ----------------------------------------------------------------------
      logical function GetModelID(ID)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetModelID
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETMODELID"::GetModelID
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      character(*) ID

      ID=SetupID

      GetModelID=.true.
      return
      end function GetModelID

c ----------------------------------------------------------------------
      logical function GetEarliestNeededTime(Time)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetEarliestNeededTime
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETEARLIESTNEEDEDTIME"::
     + GetEarliestNeededTime
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      real*8 Time

      Time=timeStepLength*CurrentTimeStepNumber

      GetEarliestNeededTime=.true.
      return
      end function GetEarliestNeededTime

c ----------------------------------------------------------------------
      logical function SetSimFileName(simFileName_in)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::SetSimFileName
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_SETSIMFILENAME"::SetSimFileName
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      character(*) simFileName_in
c --- Functions
c --- Local
      logical lok

      lok=.false.

      simFileName = simFileName_in

      lok=.true.
 9999 SetSimFileName=lok
      end function SetSimFileName

c ----------------------------------------------------------------------
      logical function Initialize(filePath_in)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::Initialize
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_INITIALIZE"::Initialize
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      character(*) filePath_in
c --- Functions
      logical ReadInputFiles,InitializeOutputFiles
c --- Local
      logical lok
      integer i

      lok=.false.

      runoff = 0.0
      leakFactor = 0.0

      filePath = filePath_in

      if(.not.ReadInputFiles())goto 9999

      do i=1,numberOfNodes
        storages(i)=0.
      enddo

      do i=1,numberOfNodes-1
        flows(i)=initialFlow
      enddo

c --- NOTE: Difference between Fortran(starts with 1) and C(starts with 0)
      CurrentTimeStepNumber = 1

      if(.not.InitializeOutputFiles())goto 9999

      lok=.true.
 9999 Initialize=lok
      return
      end function Initialize

c ----------------------------------------------------------------------
      logical function GetInputTime(Time)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetInputTime
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETINPUTTIME"::GetInputTime
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      real*8 Time

      Time=timeStepLength*CurrentTimeStepNumber

      GetInputTime=.true.
      return
      end function GetInputTime

c ----------------------------------------------------------------------
      logical function GetYCoordinate(nodeIndex,YCoor)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetYCoordinate
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETYCOORDINATE"::GetYCoordinate
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer nodeIndex
c --- Output
      real*8  YCoor

      YCoor=yCoordinates(nodeIndex)

      GetYCoordinate=.true.
      return
      end function GetYCoordinate

c ----------------------------------------------------------------------
      logical function GetXCoordinate(nodeIndex,XCoor)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetXCoordinate
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETXCOORDINATE"::GetXCoordinate
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer nodeIndex
c --- Output
      real*8  XCoor

      XCoor=XCoordinates(nodeIndex)

      GetXCoordinate=.true.
      return
      end function GetXCoordinate

c ----------------------------------------------------------------------
      logical function GetNumberOfNodes(n)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetNumberOfNodes
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETNUMBEROFNODES"::GetNumberOfNodes
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Output
      integer n

      n=numberOfNodes

      GetNumberOfNodes=.true.
      return
      end function GetNumberOfNodes

c ----------------------------------------------------------------------
      logical function GetSimulationStartDate(SDate)
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetSimulationStartDate
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETSIMULATIONSTARTDATE"::
     +  GetSimulationStartDate
!DEC$ ENDIF


      implicit none                                          
      include 'SimpleRiverEngine.i'
c --- Output
      character(*) SDate

      SDate=SimulationStartDate

      GetSimulationStartDate=.true.
      return
      end function GetSimulationStartDate

c ----------------------------------------------------------------------
      integer function NumberOfMessages()
c ----------------------------------------------------------------------
c     Returns the no. of error messages (i.e., lines) stored in the Messages stack.
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::NumberOfMessages
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_NUMBEROFMESSAGES"::NumberOfMessages
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'

      NumberOfMessages=NoMessages

      return
      end function NumberOfMessages

c ----------------------------------------------------------------------
      logical function GetMessage(No,Message)
c ----------------------------------------------------------------------
c     NOTE: Last written message = Highest number !
c ----------------------------------------------------------------------
!DEC$ IFDEFINED (VF66)
!dec$ attributes dllexport::GetMessage
!DEC$ ELSE
!dec$ attributes dllexport,alias:"_GETMESSAGE"::GetMessage
!DEC$ ENDIF

      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer No
c --- Output
      character(*) Message

      if(No.ge.1.and.No.le.NoMessages)then
        Message=Messages(No)
        GetMessage=.true.
      else
        Message=' '
        GetMessage=.false.
      endif

      return
      end function GetMessage

c ----------------------------------------------------------------------
      subroutine SetMessage(Message)
c ----------------------------------------------------------------------
c --- Messages are stored in a stack: Last written = Highest number.
c --- First written is discarded if the stack size is exceeded.
c ----------------------------------------------------------------------
      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      character(*) Message
c --- Local
      integer i

      if(NoMessages.eq.0)then
        do i=1,MaxNoMessages
          Messages(i)=' '
        enddo
      endif

      NoMessages=NoMessages+1

      if(NoMessages.gt.MaxNoMessages)then
        NoMessages=MaxNoMessages
        do i=1,NoMessages-1
          Messages(i)=Messages(i+1)
        enddo
      endif

      Messages(NoMessages)=Message

      return
      end subroutine SetMessage

c ----------------------------------------------------------------------
      character(80) function I4ToC(I4Val)
c ----------------------------------------------------------------------
c     Writes integer value to a text string (function value).
c     NOTE Entry Points for R4,R8,L4.
c ----------------------------------------------------------------------
      implicit none
      include 'SimpleRiverEngine.i'
c --- Input
      integer I4Val
      real*4  R4Val
      real*8  R8Val
      logical L4Val
c --- Entry points
      character(80) R4ToC,R8ToC,L4ToC
c --- Local
      character(80) ValString

c --- I4 -> character
      ValString=' '
      write(ValString,*)I4Val
      I4ToC=trim(adjustl(ValString))
      return

c --- R4 -> character
      ENTRY R4ToC(R4Val)
      ValString=' '
      write(ValString,*)R4Val
      I4ToC=trim(adjustl(ValString))
      return

c --- R8 -> character
      ENTRY R8ToC(R8Val)
      ValString=' '
      write(ValString,*)R8Val
      R8ToC=trim(adjustl(ValString))
      return

c --- Logical -> character
      ENTRY L4ToC(L4Val)
      ValString=' '
      write(ValString,*)L4Val
      L4ToC=trim(adjustl(ValString))
      return

      end function