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
using System.Diagnostics;
using System.IO;

namespace Oatc.OpenMI.Examples.SimpleGroundModel2
{
    class Engine
    {
	    DateTime _simulationStart;
        DateTime _simulationEnd;
        DateTime _currentTime;
        double _timeStep = 24.0 * 60.0 * 60.0; // [s]
        double _cellArea; // [m2]
        FileInfo _outputFile;

        GridInfo _gridInfo;

        double[] _storage;
        double[] _levels;
        double[] _inflows;

        public Engine(DateTime simulationStart, DateTime simulationEnd, GridInfo gridInfo, double groundWaterInitialLevel, FileInfo outputFile)
		{
            _simulationStart = simulationStart;
            _simulationEnd = simulationEnd;
            _currentTime = _simulationStart;

            _outputFile = outputFile;

            if (_outputFile.Exists)
                _outputFile.Delete();

            _gridInfo = gridInfo;

            int nElements = _gridInfo.NumberOfCells;

            _storage = new double[nElements];
            _levels = new double[nElements];
            _inflows = new double[nElements];

            _cellArea = _gridInfo.CellSize * _gridInfo.CellSize; // [m2]

            for (int i = 0; i < nElements; i++)
            {
                _storage[i] = groundWaterInitialLevel * _cellArea;
                _levels[i] = groundWaterInitialLevel;
                _inflows[i] = 0;
            }
        }

        public DateTime SimulationStart
        {
            get { return _simulationStart; }
        }

        public DateTime SimulationEnd
        {
            get { return _simulationEnd; }
        }

        public DateTime CurrentTime
        {
            get { return _currentTime; }
        }

        public void DoTimeStep()
        {
            for (int n = 0; n < _storage.Length; ++n)
            {
                _storage[n] += _inflows[n] * _timeStep; // [m3 = m3/s * s]
                _levels[n] = _storage[n] / _cellArea; // [m = m3 / m2]
                _inflows[n] = 0;
            }

            _currentTime = _currentTime.AddDays(1.0); // One day
        }

        public void SetInflows(double[] inflows)
        {
            Debug.Assert(_inflows.Length == inflows.Length);

            for (int n = 0; n < _inflows.Length; ++n)
                _inflows[n] += inflows[n];
        }

        public double[] GetAquiferStorage()
        {
            return _storage;
        }

        public double[] GetAquiferLevels()
        {
            return _levels;
        }

        public void Finish()
        {
            WriteOutput();
        }

        void WriteOutput()
        {
            using (StreamWriter sw = _outputFile.AppendText())
            {
                sw.WriteLine();
                sw.WriteLine(string.Format("Current Time: {0} {1}",
                    _currentTime.ToLongDateString(), _currentTime.ToLongTimeString()));
                sw.WriteLine();

                StringBuilder sb;
                double totalStorage = 0;

                for (int nR = 0; nR < _gridInfo.NY; ++nR)
                {
                    sb = new StringBuilder();

                    for (int nC = 0; nC < _gridInfo.NX; ++nC)
                    {
                        sb.Append(_levels[nC + _gridInfo.NX * nR].ToString());
                        sb.Append(",");
                    }

                    sw.WriteLine(sb.ToString());
                }

                sw.WriteLine();
                sw.WriteLine();

                for (int nR = 0; nR < _gridInfo.NY; ++nR)
                {
                    sb = new StringBuilder();

                    for (int nC = 0; nC < _gridInfo.NX; ++nC)
                    {
                        sb.Append(_storage[nC + _gridInfo.NX * nR].ToString());
                        sb.Append(",");

                        totalStorage += _storage[nC + _gridInfo.NX * nR];
                    }

                    sw.WriteLine(sb.ToString());
                }

                sw.WriteLine();
                sw.WriteLine(string.Format("Total Storage [m3]\t\n{0}", totalStorage.ToString()));
            }
        }
    }
}
