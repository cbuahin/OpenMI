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
// file: SimpleRiverEngineDotNetAccess.cs
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
using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;

namespace Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper
{
    /// <summary>
    /// This class will access the <see cref="SimpleRiverEngineDllAccess"/> class 
    /// and for each method in <see cref="SimpleRiverEngineDllAccess"/> class 
    /// change the calling conventions to follow the C# convensions. Methods
    /// that are passing data back through the method argumes are change
    /// to methods that are returning values. In Fortran arrays normally starts
    /// from one, whereas arrays in C# normally starts from zero. This class will
    /// change indices used for arrays between zero based on one based.
    /// 
    /// Each method in the <see cref="SimpleRiverEngineDllAccess"/> class is returning 
    /// a boolean to indicate if something went wrong. Each time this class calls 
    /// methods in the <see cref="SimpleRiverEngineDllAccess"/> class the return 
    /// values is checked and if the values is false the an error messages is 
    /// obtained from the <see cref="SimpleRiverEngineDllAccess"/>  class and a 
    /// System exception is created and thrown.
    /// </summary>
    public class SimpleRiverEngineDotNetAccess
    {
        IntPtr _fortranDllHandle;

        public void AddInflow(int index, double inflow)
        {
            int n = index + 1; // one is added because convenrion in C# is to start from zero whereas Fortran normally starts from 1
            if (!(SimpleRiverEngineDllAccess.AddInflow(ref n, ref inflow)))
            {
                CreateAndThrowException();
            }
        }

        public void Initialize(string filePath, string simFileName)
        {
            string enginedll = @"Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Engine.dll";
            string enginePath = enginedll;

            Trace.TraceInformation("Looking for engineDll: {0}",enginePath);

            if (!File.Exists(enginePath))
            {
                // ADH: Why did this work before?

                string folder = Directory.GetParent(
                    Assembly.GetAssembly(GetType()).Location).FullName;

                enginePath = Path.Combine(folder, enginedll);

                Trace.TraceInformation("Looking for engineDll: {0}", enginePath);

                if (!File.Exists(enginePath))
                    throw new SimpleRiverException("Cannot find dll " + enginedll);
            }

            _fortranDllHandle = Kernel32Wrapper.LoadLibrary(enginePath);

            if (_fortranDllHandle.ToInt32() == 0)
                throw new SimpleRiverException("Failed fortran dll load " + enginedll);

            string curDir = System.IO.Directory.GetCurrentDirectory();

            if (!(SimpleRiverEngineDllAccess.SetSimFileName(simFileName, ((uint)simFileName.Length))))
            {
                CreateAndThrowException();
            }
            if (!(SimpleRiverEngineDllAccess.Initialize(filePath, ((uint)filePath.Length))))
            {
                CreateAndThrowException();
            }
        }

        public void Finish()
        {
            if (!(SimpleRiverEngineDllAccess.Finish()))
            {
                CreateAndThrowException();
            }

            while (Kernel32Wrapper.FreeLibrary(_fortranDllHandle))
            {
            }

            _fortranDllHandle = (IntPtr)0;
        }

        public double GetCurrentTime()
        {
            double time = 0;
            if (!(SimpleRiverEngineDllAccess.GetCurrentTime(ref time)))
            {
                CreateAndThrowException();
            }
            return time;
        }

        public string GetModelDescription()
        {
            StringBuilder description = new StringBuilder("                                                        ");
            if (!(SimpleRiverEngineDllAccess.GetModelDescription(description, (uint)description.Length)))
            {
                CreateAndThrowException();
            }
            return description.ToString().Trim();
        }

        public double GetFlow(int index)
        {
            double flow = 0;
            int n = index + 1; //one is added because the convention in C# is to normally start from zero, whereas for Fortran the convention is to normally start from 1
            if (!(SimpleRiverEngineDllAccess.GetFlow(ref n, ref flow)))
            {
                CreateAndThrowException();
            }
            return flow;
        }

        public string GetModelID()
        {
            StringBuilder id = new StringBuilder("                                                        ");
            if (!(SimpleRiverEngineDllAccess.GetModelID(id, (uint)id.Length)))
            {
                CreateAndThrowException();
            }
            return id.ToString().Trim();
        }

        /// <summary>
        /// Return the input time as seconds since start of simulation
        /// </summary>
        /// <returns></returns>
        public double GetInputTime()
        {
            double time = 0;
            if (!(SimpleRiverEngineDllAccess.GetInputTime(ref time)))
            {
                CreateAndThrowException();
            }
            return time;
        }

        public int GetNumberOfNodes()
        {
            int numberOfNodes = 0;
            if (!(SimpleRiverEngineDllAccess.GetNumberOfNodes(ref numberOfNodes)))
            {
                CreateAndThrowException();
            }
            return numberOfNodes;
        }

        public string GetSimulationStartDate()
        {
            StringBuilder simulationStartDate = new StringBuilder("                                                        ");
            if (!(SimpleRiverEngineDllAccess.GetSimulationStartDate(simulationStartDate, (uint)simulationStartDate.Length)))
            {
                CreateAndThrowException();
            }
            return simulationStartDate.ToString().Trim();
        }

        public double GetXCoordinate(int nodeIndex)
        {
            double xCoordinate = 0;
            int index = nodeIndex + 1; //Fortran starts arrays from 1 and C# starts from 0
            if (!(SimpleRiverEngineDllAccess.GetXCoordinate(ref index, ref xCoordinate)))
            {
                CreateAndThrowException();
            }
            return xCoordinate;
        }

        public double GetYCoordinate(int nodeIndex)
        {
            double yCoordinate = 0;
            int index = nodeIndex + 1; //Fortran starts arrays from 1 and C# starts from 0
            if (!(SimpleRiverEngineDllAccess.GetYCoordinate(ref index, ref yCoordinate)))
            {
                CreateAndThrowException();
            }
            return yCoordinate;
        }

        public void PerformTimeStep()
        {
            if (!(SimpleRiverEngineDllAccess.PerformTimeStep()))
            {
                // note that for some more advanced models the return value from PerformTimeStep can be used 
                // to make the SmartWrapper redo the time step. One example could be for situations where a model
                // has adoptive time steps. For the SimpleRiver model a return value that is false indicates an error
                CreateAndThrowException();
            }
        }

        /// <summary>
        /// Return the time step length in seconds
        /// </summary>
        /// <returns></returns>
        public double GetTimeStepLength()
        {
            double timeStepLength = -999;

            if (!(SimpleRiverEngineDllAccess.GetTimeStepLength(ref timeStepLength)))
            {
                CreateAndThrowException();
            }

            return timeStepLength;
        }

        /// <summary>
        /// Set the time step length in seconds
        /// </summary>
        public void SetTimeStepLength(double timeStepLength)
        {
            if (!(SimpleRiverEngineDllAccess.SetTimeStepLength(ref timeStepLength)))
            {
                CreateAndThrowException();
            }
        }

        public int GetNumberOfTimeSteps()
        {
            int numberOfTimeSteps = 0;
            if (!(SimpleRiverEngineDllAccess.GetNumberOfTimeSteps(ref numberOfTimeSteps)))
            {
                CreateAndThrowException();
            }
            return numberOfTimeSteps;
        }


        private static void CreateAndThrowException()
        {
            int numberOfMessages = SimpleRiverEngineDllAccess.GetNumberOfMessages();
            string message = "Error Message from SimpleRiver Fortran Core ";

            for (int i = 0; i < numberOfMessages; i++)
            {
                int n = i;
                StringBuilder messageFromCore = new StringBuilder("                                                                                                                               ");
                SimpleRiverEngineDllAccess.GetMessage(ref n, messageFromCore, (uint)messageFromCore.Length);
                message += "; ";
                message += messageFromCore.ToString().Trim();
            }
            throw new Exception(message);
        }
    }

    public class SimpleRiverException : Exception
    {
        public SimpleRiverException() { }
        public SimpleRiverException(string s) : base(s) { }
        public SimpleRiverException(string s, Exception inner) : base(s, inner) { }
    }

}