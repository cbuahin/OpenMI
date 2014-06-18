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
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Oatc.OpenMI.Gui.Core;

using NUnit.Framework;

namespace Oatc.OpenMI.Gui
{
	[TestFixture]
	public class UnitTests
	{
        DirectoryInfo _oprFolder = new DirectoryInfo(@"../../data");
        
		[Test]
        public void Test01_SimpleCSharpRiver2_RiverReach1()
		{
            FileInfo opr = FindFile("SimpleCSharpRiver2_RiverReach1.opr");
            FileInfo results = RemoveFile("SimpleCSharpRiver2_RiverReach1.out");
				
			CompositionRun run = new CompositionRun();
			run.Run(opr);

            results.Refresh();

			CheckFlowBalance(results,
				0, // OpenMI
				5754240, // FlowIn
				-5754240, // FlowOut
				0); // Leakage
		}

		[Test]
        public void Test02_SimpleCSharpRiver2_RiverReachs1and2()
		{
            FileInfo opr = FindFile("SimpleCSharpRiver2_RiverReachs1and2.opr");

            FileInfo results1 = RemoveFile("SimpleCSharpRiver2_RiverReach1.out");
            FileInfo results2 = RemoveFile("SimpleCSharpRiver2_RiverReach2.out");

			CompositionRun run = new CompositionRun();
			run.Run(opr);

            results1.Refresh();
            results2.Refresh();

			CheckFlowBalance(results1,
				0, // OpenMI
				5754240, // FlowIn
				-5754240, // FlowOut
				0); // Leakage

			CheckFlowBalance(results2,
				4147200, // OpenMI
				5754240, // FlowIn
				-9901440, // FlowOut
				0); // Leakage
		}

		[Test]
		public void Test03_SimpleCSharpRiver2_Decorators01()
		{
			FileInfo opr = FindFile("SimpleCSharpRiver2_Decorators01.opr");

            FileInfo results1 = RemoveFile("SimpleCSharpRiver2_RiverReach1.out");
            FileInfo results2 = RemoveFile("SimpleCSharpRiver2_RiverReach2.out");

			CompositionRun run = new CompositionRun();
			run.Run(opr);

            results1.Refresh();
            results2.Refresh();

			CheckFlowBalance(results1,
				0, // OpenMI
				5754240, // FlowIn
				-5754240, // FlowOut
				0); // Leakage

			// Includes decorator that doubles values from first

			CheckFlowBalance(results2,
				8294400, // OpenMI
				5754240, // FlowIn
				-14048640, // FlowOut
				0); // Leakage
		}

        [Test]
        public void Test04_SimpleCSharpRiver2_Decorators02()
        {
            FileInfo opr = FindFile("SimpleCSharpRiver2_Decorators02.opr");

            FileInfo results1 = RemoveFile("SimpleCSharpRiver2_RiverReach1.out");
            FileInfo results2 = RemoveFile("SimpleCSharpRiver2_RiverReach2.out");

            CompositionRun run = new CompositionRun();
            run.Run(opr);

            results1.Refresh();
            results2.Refresh();

            CheckFlowBalance(results1,
                0, // OpenMI
                5754240, // FlowIn
                -5754240, // FlowOut
                0); // Leakage

            CheckFlowBalance(results2,
                4147200, // OpenMI
                5754240, // FlowIn
                -9901440, // FlowOut
                0); // Leakage
        }

        [Test]
        public void Test05_SimpleGroundWater2_Corse1()
        {
            FileInfo opr = FindFile("SimpleGroundWater2_Corse1.opr");
            FileInfo results1 = RemoveFile("SimpleGroundWater2_Corse1.out");

            CompositionRun run = new CompositionRun();
            run.Run(opr);

            results1.Refresh();
            CheckStorage(results1, 18e6);
        }

        void CheckFlowBalance(FileInfo results,
			double openMI, double flowIn, double flowOut, double leakage)
		{
			/*
			 * B(n): Flow in branch N
			 * OMI(n): OpenMI flow into node N
			 * Lc: Leakage Coefficient
			 * 
			 * Note: No time component, i.e. delta t arbitary
			 * 
			 * B(n) = (Omi(n) + Bc(n) + B(n-1))(1. - Lc)
			 * 
			double[] expected = new double[] {
				1.1,2.3,3.6,
				2.1,4.3,6.6,
				3.1,6.3,9.6,
				4.1,8.3,12.6,
				5.1,10.3,15.6,
				6.1,12.3,18.6,
			};			 
			 */

			double sum = openMI + flowIn + flowOut + leakage;

			List<double> doubles = LastLineAsDoubles(results);

			Assert.AreEqual(openMI, doubles[0], "OpenMI flows");
			Assert.AreEqual(flowIn, doubles[1], "Boundary condition flows");
			Assert.AreEqual(flowOut, doubles[2], "Branch out flows");
			Assert.AreEqual(leakage, doubles[3], "Leakage flows");
			Assert.AreEqual(sum, doubles[4], "Flow balance");
		}

        void CheckStorage(FileInfo results, double storage)
        {
            List<double> doubles = LastLineAsDoubles(results);

            Assert.AreEqual(storage, doubles[0], "Total Storage [m3]");
        }

		FileInfo FindFile(string localFilename)
		{
			Assert.IsTrue(_oprFolder.Exists,
				"Folder not found " + _oprFolder.FullName);

			FileInfo file = new FileInfo(
				Path.Combine(_oprFolder.FullName, localFilename));

			Assert.IsTrue(file.Exists, "Missing file " + file.FullName);

			return file;
		}

		FileInfo RemoveFile(string localFilename)
		{
			Assert.IsTrue(_oprFolder.Exists,
				"Folder not found " + _oprFolder.FullName);

			FileInfo file = new FileInfo(
				Path.Combine(_oprFolder.FullName, localFilename));

			if (file.Exists)
				file.Delete();

			return file;
		}

		List<double> LastLineAsDoubles(FileInfo results)
		{
			Assert.IsTrue(results.Exists, "File not found " + results.FullName);

			string lastLine = null;

			using (FileStream fs = results.OpenRead())
			{
				string input;

				using (TextReader tr = new StreamReader(fs))
				{
					while ((input = tr.ReadLine()) != null)
						lastLine = input;
				}
			}

			string[] fields = lastLine.Split(',');

			List<double> doubles = new List<double>(fields.Length);

			foreach (string s in fields)
				if (s.Trim() != string.Empty)
					doubles.Add(double.Parse(s));

			return doubles;
		}
	}
}
