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
// namespace: Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest 
// purpose: Unit-testing the package Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper
// file: SimpleRiverEngineDotNetAccessTest.cs
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
using NUnit.Framework;
using Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper;

namespace Oatc.OpenMI.Examples.ModelComponents.SimpleRiver.Wrapper.UnitTest
{
    /// <summary>
    /// Test of access to the fortran dll from .NET
    /// </summary>
    [TestFixture]
    public class SimpleRiverEngineDotNetAccessTest
    {
        SimpleRiverEngineDotNetAccess _simpleRiverEngineDotNetAccess;
        string _filePath;
        string _simFileName;

        [SetUp]
        public void Init()
        {
            _simpleRiverEngineDotNetAccess = new SimpleRiverEngineDotNetAccess();
            _filePath = @"..\..\..\..\Data";
            _simFileName = @"SimpleRiver.sim";
            _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
        }

        [TearDown]
        public void ClearUp()
        {

        }

        [Test]
        public void AddInflow()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                _simpleRiverEngineDotNetAccess.AddInflow(1, 4.0);
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(7.6, _simpleRiverEngineDotNetAccess.GetFlow(2));
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(6.6, _simpleRiverEngineDotNetAccess.GetFlow(2));

                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }




        [Test]
        public void GetCurrentTime()
        {
            try
            {
                double dt = 172800;
                double currentTime = 0;
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(currentTime, _simpleRiverEngineDotNetAccess.GetCurrentTime());
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                currentTime += dt;
                Assert.AreEqual(currentTime, _simpleRiverEngineDotNetAccess.GetCurrentTime());
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                currentTime += dt;
                Assert.AreEqual(currentTime, _simpleRiverEngineDotNetAccess.GetCurrentTime());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void GetModelDescription()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual("Simple River model for: The river Rhine", _simpleRiverEngineDotNetAccess.GetModelDescription());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }

        }

        [Test]
        public void GetTimeStepLength()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(172800.0, _simpleRiverEngineDotNetAccess.GetTimeStepLength());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void SetTimeStepLength()
        {
            _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
            Assert.AreEqual(172800.0, _simpleRiverEngineDotNetAccess.GetTimeStepLength());
            _simpleRiverEngineDotNetAccess.SetTimeStepLength(3600);
            Assert.AreEqual(3600.0, _simpleRiverEngineDotNetAccess.GetTimeStepLength());
            _simpleRiverEngineDotNetAccess.Finish();
        }

        [Test]
        public void GetNumberOfTimeSteps()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(6, _simpleRiverEngineDotNetAccess.GetNumberOfTimeSteps());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void GetFlow()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(0, _simpleRiverEngineDotNetAccess.GetFlow(0));
                Assert.AreEqual(0, _simpleRiverEngineDotNetAccess.GetFlow(1));
                Assert.AreEqual(0, _simpleRiverEngineDotNetAccess.GetFlow(2));
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void GetModelID()
        {
            _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
            Assert.AreEqual("The river Rhine", _simpleRiverEngineDotNetAccess.GetModelID());
            _simpleRiverEngineDotNetAccess.Finish();
        }

        [Test]
        public void GetInputTime()
        {
            try
            {
                double dt = 172800;
                double inputTime = dt;
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(inputTime, _simpleRiverEngineDotNetAccess.GetInputTime());
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                inputTime += dt;
                Assert.AreEqual(inputTime, _simpleRiverEngineDotNetAccess.GetInputTime());
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                inputTime += dt;
                Assert.AreEqual(inputTime, _simpleRiverEngineDotNetAccess.GetInputTime());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }

        }

        [Test]
        public void GetNumberOfNodes()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(4, _simpleRiverEngineDotNetAccess.GetNumberOfNodes());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }

        }

        [Test]
        public void GetSimulationStartDate()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual("2004-10-07 16:38:32", _simpleRiverEngineDotNetAccess.GetSimulationStartDate());
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }

        }

        [Test]
        public void GetXCoordinate()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(1000.0, _simpleRiverEngineDotNetAccess.GetXCoordinate(0));
                Assert.AreEqual(7000.0, _simpleRiverEngineDotNetAccess.GetXCoordinate(1));
                Assert.AreEqual(9000.0, _simpleRiverEngineDotNetAccess.GetXCoordinate(2));
                Assert.AreEqual(14000.0, _simpleRiverEngineDotNetAccess.GetXCoordinate(3));
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void GetYCoordinate()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                Assert.AreEqual(9000.0, _simpleRiverEngineDotNetAccess.GetYCoordinate(0));
                Assert.AreEqual(6000.0, _simpleRiverEngineDotNetAccess.GetYCoordinate(1));
                Assert.AreEqual(3000.0, _simpleRiverEngineDotNetAccess.GetYCoordinate(2));
                Assert.AreEqual(1000.0, _simpleRiverEngineDotNetAccess.GetYCoordinate(3));
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }
        }

        [Test]
        public void PerformTimeStep()
        {
            try
            {
                _simpleRiverEngineDotNetAccess.Initialize(_filePath, _simFileName);
                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(1.1, _simpleRiverEngineDotNetAccess.GetFlow(0));
                Assert.AreEqual(2.3, _simpleRiverEngineDotNetAccess.GetFlow(1));
                Assert.AreEqual(3.6, _simpleRiverEngineDotNetAccess.GetFlow(2));

                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(2.1, _simpleRiverEngineDotNetAccess.GetFlow(0));
                Assert.AreEqual(4.3, _simpleRiverEngineDotNetAccess.GetFlow(1));
                Assert.AreEqual(6.6, _simpleRiverEngineDotNetAccess.GetFlow(2));

                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(3.1, _simpleRiverEngineDotNetAccess.GetFlow(0));
                Assert.AreEqual(6.3, _simpleRiverEngineDotNetAccess.GetFlow(1));
                Assert.AreEqual(9.6, _simpleRiverEngineDotNetAccess.GetFlow(2));

                _simpleRiverEngineDotNetAccess.PerformTimeStep();
                Assert.AreEqual(4.1, _simpleRiverEngineDotNetAccess.GetFlow(0));
                Assert.AreEqual(8.3, _simpleRiverEngineDotNetAccess.GetFlow(1));
                Assert.AreEqual(12.6, _simpleRiverEngineDotNetAccess.GetFlow(2));
                _simpleRiverEngineDotNetAccess.Finish();
            }
            catch (Exception e)
            {
                WriteException(e.Message);
                throw (e);
            }

        }

        [Test]
        //[ExpectedException(typeof(Exception))]
        public void CreateExpectedException()
        {
            try
            {
                string filePath = "Wrong file path";
                _simpleRiverEngineDotNetAccess.Initialize(filePath, "wrong file name");
                _simpleRiverEngineDotNetAccess.Finish();
                Assert.Fail("Should not reach here");
            }
            catch (Exception e)
            {
                char[] deliminator = { ';' };
                WriteException(e.Message);
                Assert.AreEqual(" ERROR  : Could not open input file", (e.Message.Split(deliminator))[2]);
            }
        }

        private void WriteException(string message)
        {
            char[] deliminator = { ';' };
            for (int i = 0; i < message.Split(deliminator).Length; i++)
            {
                Console.WriteLine((message.Split(deliminator)[i]));
            }
        }
    }
}
