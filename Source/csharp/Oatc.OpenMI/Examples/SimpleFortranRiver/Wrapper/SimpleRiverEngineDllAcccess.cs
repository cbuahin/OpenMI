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
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper 
// purpose: Example-implementation river model wrapper
// file: SimpleRiverEngineDllAccess.cs
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
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion
using System.Runtime.InteropServices;
using System.Text;

namespace Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper
{
    /// <summary>
    /// Class providing access to the engine dll through a win32 API.
    /// The class makes a one-to-one conversion of all exported functions 
    /// in the engine core API to public .NET methods
    /// </summary>
    public abstract class SimpleRiverEngineDllAccess
    {
        public const string engineDllFilePath = @"Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Engine.dll";

        [DllImport(engineDllFilePath,
            EntryPoint = "ADDINFLOW",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddInflow(ref int index, ref double inflow);

        [DllImport(engineDllFilePath,
            EntryPoint = "INITIALIZE",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Initialize(string filePath, uint length);

        [DllImport(engineDllFilePath,
            EntryPoint = "SETSIMFILENAME",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetSimFileName(string simFileName, uint length);

        [DllImport(engineDllFilePath,
            EntryPoint = "FINISH",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Finish();

        [DllImport(engineDllFilePath,
            EntryPoint = "GETCURRENTTIME",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetCurrentTime(ref double time);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETMODELDESCR",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetModelDescription([MarshalAs(UnmanagedType.LPStr)] StringBuilder description, uint length);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETSIMULATIONSTARTDATE",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSimulationStartDate([MarshalAs(UnmanagedType.LPStr)] StringBuilder simulationStartDate, uint length);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETFLOW",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetFlow(ref int index, ref double flow);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETMODELID",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetModelID([MarshalAs(UnmanagedType.LPStr)] StringBuilder id, uint length);

        /// <summary>
        /// Get the input time, seconds since start of simulation
        /// </summary>
        /// <param name="inputTime"></param>
        /// <returns></returns>
        [DllImport(engineDllFilePath,
            EntryPoint = "GETINPUTTIME",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetInputTime(ref double inputTime);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETMESSAGE",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMessage(ref int index, [MarshalAs(UnmanagedType.LPStr)] StringBuilder message, uint length);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETNUMBEROFNODES",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetNumberOfNodes(ref int numberOfNodes);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETXCOORDINATE",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetXCoordinate(ref int index, ref double xCoordinate);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETYCOORDINATE",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetYCoordinate(ref int index, ref double yCoordinate);

        [DllImport(engineDllFilePath,
            EntryPoint = "NUMBEROFMESSAGES",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumberOfMessages();

        [DllImport(engineDllFilePath,
            EntryPoint = "PERFORMTIMESTEP",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PerformTimeStep();

        /// <summary>
        /// Return the time step length in seconds
        /// </summary>
        /// <param name="timeStepLength"></param>
        /// <returns></returns>
        [DllImport(engineDllFilePath,
            EntryPoint = "GETTIMESTEPLENGTH",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetTimeStepLength(ref double timeStepLength);

        /// <summary>
        /// Set the time step length in seconds
        /// </summary>
        /// <param name="timeStepLength"></param>
        /// <returns></returns>
        [DllImport(engineDllFilePath,
            EntryPoint = "SETTIMESTEPLENGTH",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetTimeStepLength(ref double timeStepLength);

        [DllImport(engineDllFilePath,
            EntryPoint = "GETNTIMESTEPS",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetNumberOfTimeSteps(ref int numberOfTimeSteps);

    }
}