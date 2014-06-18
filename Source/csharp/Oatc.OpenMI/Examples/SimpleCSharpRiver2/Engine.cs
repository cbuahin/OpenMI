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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Oatc.OpenMI.Examples.SimpleCSharpRiver2
{
    public class Engine
    {
        string modelID;
        DateTime simulationStartTime;
        double timeStepLength;
        int numberOfTimeSteps;
        int numberOfNodes;
        double[] nodesXCoords;
        double[] nodesYCoords;
        double[][] boundaryFlow;
        double[] _branchFlows;
        double[] _leakages;
        double[] _groundWaterLevels;
        double[] _riverBedLevels;
        int currentTimeStep;
        double leakageCoefficient;
		FileInfo _outputFile;

		double _totalFlowOmi = 0;
		double _totalFlowBc = 0;
		double _totalFlowLeakage = 0;
		double _totalFlowRiver = 0;

        double[] OpenMIInflows; // OpenMI specific

        public void RunSimulation(string path)
        {
			string fileNetwork = path + "RiverNetwork1.txt";
			string fileBoundaryConditions = path + "RiverBoundaryConditions1.txt";
			string outputFile = path + "River1.out";

            ReadInputFiles(fileNetwork, fileBoundaryConditions);

			init(outputFile);

            for (int i = 0; i < numberOfTimeSteps; i++)
                MakeATimeStep();

            Finish();
        }

        public void ReadInputFiles(string fileNetwork, string fileBoundaryConditions)
        {
			if (!File.Exists(fileNetwork))
				throw new Exception("Missing file: " + fileNetwork);

			if (!File.Exists(fileNetwork))
				throw new Exception("Missing file: " + fileBoundaryConditions);

            // --- Read network file ----

            StreamReader networkFile = new StreamReader(fileNetwork);

            networkFile.ReadLine();  //bypass header lines
            networkFile.ReadLine();  //bypass header lines
            networkFile.ReadLine();  //bypass header lines

			modelID = networkFile.ReadLine();

			networkFile.ReadLine();  //bypass header lines
			networkFile.ReadLine();  //bypass header lines
			
			numberOfNodes = Convert.ToInt32(networkFile.ReadLine());

            nodesXCoords = new double[numberOfNodes];
            nodesYCoords = new double[numberOfNodes];

            networkFile.ReadLine();
            networkFile.ReadLine();

            for (int i = 0; i < numberOfNodes; i++)
            {
                string str = networkFile.ReadLine();
                string[] strs = str.Split(new char[] { ',' });
                nodesXCoords[i] = Convert.ToDouble(strs[0], CultureInfo.InvariantCulture);
                nodesYCoords[i] = Convert.ToDouble(strs[1], CultureInfo.InvariantCulture);
            }

            if (!networkFile.EndOfStream)
                networkFile.ReadLine();  //bypass header lines
            if (!networkFile.EndOfStream)
                networkFile.ReadLine();  //bypass header lines

            for (int n = 0; n < numberOfNodes; ++n)
                if (!networkFile.EndOfStream)
                    _riverBedLevels[n] = double.Parse(networkFile.ReadLine());

            networkFile.Close();

            // --- Read boundary conditions file ---

            StreamReader boundaryConditionsFile = new StreamReader(fileBoundaryConditions);

			boundaryConditionsFile.ReadLine();  //bypass header lines
			boundaryConditionsFile.ReadLine();  //bypass header lines
			boundaryConditionsFile.ReadLine();  //bypass header lines

			leakageCoefficient = Convert.ToDouble(boundaryConditionsFile.ReadLine(), CultureInfo.InvariantCulture);

            boundaryConditionsFile.ReadLine();  // bypass header line
            boundaryConditionsFile.ReadLine();  // bypass header line

			string[] strings = boundaryConditionsFile.ReadLine().Split(new char[] { '-', ' ', ':' });
			int year = Convert.ToInt32(strings[0]);
			int month = Convert.ToInt32(strings[1]);
			int day = Convert.ToInt32(strings[2]);
			int hour = Convert.ToInt32(strings[3]);
			int minute = Convert.ToInt32(strings[4]);
			int second = Convert.ToInt32(strings[5]);
			simulationStartTime = new DateTime(year, month, day, hour, minute, second);

			boundaryConditionsFile.ReadLine();  //bypass header lines
			boundaryConditionsFile.ReadLine();  //bypass header lines

			timeStepLength = Convert.ToDouble(boundaryConditionsFile.ReadLine(), CultureInfo.InvariantCulture);

			boundaryConditionsFile.ReadLine();  //bypass header lines
			boundaryConditionsFile.ReadLine();  //bypass header lines

            numberOfTimeSteps = Convert.ToInt32(boundaryConditionsFile.ReadLine());

            boundaryConditionsFile.ReadLine();  // bypass header line
            boundaryConditionsFile.ReadLine();  // bypass header line

            boundaryFlow = new double[numberOfTimeSteps][];

            for (int i = 0; i < numberOfTimeSteps; i++)
                boundaryFlow[i] = new double[numberOfNodes];

            for (int i = 0; i < numberOfTimeSteps; i++)
            {
                string str1 = boundaryConditionsFile.ReadLine();
                string[] strs1 = str1.Split(new char[] { ',' });

                if (strs1.Length != numberOfNodes)
                    throw new Exception("The number of boundary flow does not equal the number of nodes");

                for (int n = 0; n < strs1.Length; n++)
                    boundaryFlow[i][n] = Convert.ToDouble(strs1[n], CultureInfo.InvariantCulture);
            }

            boundaryConditionsFile.Close();
        }

		public void init(string outputFile)
        {
            currentTimeStep = 0;
            _branchFlows = new double[numberOfNodes - 1];
            _leakages = new double[numberOfNodes - 1];

            _riverBedLevels = new double[numberOfNodes];
            _groundWaterLevels = new double[numberOfNodes];

			_outputFile = new FileInfo(outputFile);

			if (_outputFile.Exists)
				_outputFile.Delete();

            // OpenMI Specific

            OpenMIInflows = new double[numberOfNodes];

            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                _branchFlows[i] = 0.0;
                _leakages[i] = 0.0;
            }

            for (int i = 0; i < numberOfNodes; i++)
            {
                OpenMIInflows[i] = 0.0;
                _riverBedLevels[i] = 0.0;
                _groundWaterLevels[i] = 0.0;
            }
        }

        public void MakeATimeStep()
        {
			StringBuilder sbBranch = new StringBuilder("Section:,");
			StringBuilder sbLeakage = new StringBuilder("Leakage:,");
			StringBuilder sbBcs = new StringBuilder("Boundary Conditions:,");
			StringBuilder sbOpenMI = new StringBuilder("OpenMI:,");

            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                _branchFlows[i] = boundaryFlow[currentTimeStep][i] + OpenMIInflows[i];  //last bit is OpenMI specific

                if (i > 0)
                    _branchFlows[i] += _branchFlows[i - 1];
            }

            for (int i = 0; i < numberOfNodes; i++)
            {
                sbBcs.AppendFormat("{0},", boundaryFlow[currentTimeStep][i].ToString());
				sbOpenMI.AppendFormat("{0},", OpenMIInflows[i].ToString());
			}

            // Reduce flow due to leakage and calculate leakage

            double factor;
   
            for (int i = 0; i < numberOfNodes - 1; i++)
            {
                if (_groundWaterLevels[i] > _riverBedLevels[i]
                    && _groundWaterLevels[i + 1] > _riverBedLevels[i + 1])
                    factor = 0.0;
                else if (_groundWaterLevels[i] > _riverBedLevels[i])
                    factor = (_groundWaterLevels[i] - _riverBedLevels[i])
                        / (_riverBedLevels[i] - _riverBedLevels[i + 1]);
                else if (_groundWaterLevels[i + 1] > _riverBedLevels[i + 1])
                    factor = (_groundWaterLevels[i + 1] - _riverBedLevels[i + 1])
                        / (_riverBedLevels[i + 1] - _riverBedLevels[i]);
                else
                    factor = 1.0;

                _leakages[i] = factor * leakageCoefficient * _branchFlows[i];
                _branchFlows[i] -= _leakages[i];

				sbBranch.AppendFormat("{0},", _branchFlows[i].ToString());
				sbLeakage.AppendFormat("{0},", _leakages[i].ToString());

				_totalFlowOmi += OpenMIInflows[i] * timeStepLength;
				_totalFlowBc += boundaryFlow[currentTimeStep][i] * timeStepLength;
				_totalFlowLeakage -= _leakages[i] * timeStepLength;
            }

			_totalFlowRiver -= _branchFlows[numberOfNodes - 2] * timeStepLength;

            currentTimeStep++;

			using (StreamWriter sw = _outputFile.AppendText())
			{
				sw.WriteLine(
					sbBranch.ToString()
					+ sbLeakage.ToString()
					+ sbBcs.ToString()
					+ sbOpenMI.ToString());
			}

            // OpenMI specific

            for (int i = 0; i < numberOfNodes; i++)
                OpenMIInflows[i] = 0.0;
        }

        public void Finish()
        {
			double sum = _totalFlowOmi + _totalFlowBc
				+ _totalFlowLeakage + _totalFlowRiver;

			using (StreamWriter sw = _outputFile.AppendText())
			{
                sw.WriteLine();
				sw.WriteLine("OpenMI + Inflows + Outflows + Leakage = 0 Litres");

				sw.WriteLine(string.Format("{0},{1},{2},{3},{4},",
					_totalFlowOmi, _totalFlowBc,
					_totalFlowRiver, _totalFlowLeakage,
					sum));
			}
        }

        // --- Methods that are added for the OpenMI Migration.

        public string GetModelID()
        {
            return this.modelID;
        }

        public DateTime GetSimulationStartTime()
        {
            return this.simulationStartTime;
        }

        public int GetNumberOfTimeSteps()
        {
            return this.numberOfTimeSteps;
        }

        public double GetTimeStepLength()
        {
            return this.timeStepLength;
        }

        public int GetCurrentTimeStep()
        {
            return this.currentTimeStep;
        }

        public int GetNumberOfNodes()
        {
            return this.numberOfNodes;
        }

        public double GetFlow(int branchIndex)
        {
            return _branchFlows[branchIndex];
        }

        public void SetExternalNodeInflow(int nodeIndex, double flow)
        {
            OpenMIInflows[nodeIndex] = flow;
        }

        public double GetXCoordinate(int nodeIndex)
        {
            return nodesXCoords[nodeIndex];
        }

        public double GetYCoordiante(int nodeIndex)
        {
            return nodesYCoords[nodeIndex];
        }

        public double GetLeakage(int branchIndex)
        {
            return _leakages[branchIndex];
        }

        public void SetRiverBedLevel(int nodeIndex, double level)
        {
            _riverBedLevels[nodeIndex] = level;
        }

        public double GetRiverBedLevel(int nodeIndex)
        {
            return _riverBedLevels[nodeIndex];
        }

        public void SetGroundWaterLevel(int branchIndex, double level)
        {
            _groundWaterLevels[branchIndex] = level;
        }

        public double GetGroundWaterLevel(int branchIndex)
        {
            return _groundWaterLevels[branchIndex];
        }

        public DateTime GetCurrentTime()
        {
            double simulatedDays = (currentTimeStep * timeStepLength) / (24.0 * 3600);
            return simulationStartTime.AddDays(simulatedDays);          
        }
    }
}
