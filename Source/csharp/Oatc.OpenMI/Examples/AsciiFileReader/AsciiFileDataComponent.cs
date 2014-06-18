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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Buffer;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
﻿using Oatc.OpenMI.Sdk.Spatial;
﻿using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.AsciiFileReader
{
    public class AsciiFileDataComponent : LinkableComponent
    {
        private List<ITimeSpaceInput> _inputs = new List<ITimeSpaceInput>();
        private List<ITimeSpaceOutput> _outputs = new List<ITimeSpaceOutput>();

        public AsciiFileDataComponent()
        {
            Arguments = new List<IArgument>();
            for (int i = 0; i < 10; i++)
            {
                Arguments.Add(new ArgumentString(string.Format("File{0}",i)));
            }
        }

        public override IList<IBaseInput> Inputs
        {
            get { return (new ListWrapper<ITimeSpaceInput, IBaseInput>(_inputs)); }
        }

        public override IList<IBaseOutput> Outputs
        {
            get { return new ListWrapper<ITimeSpaceOutput, IBaseOutput>(_outputs); }
        }

        public override List<IAdaptedOutputFactory> AdaptedOutputFactories
        {
            get { throw new NotImplementedException(); }
        }

        public override void Initialize()
        {
            foreach (IArgument argument in Arguments)
            {
                if (argument != null && !string.IsNullOrEmpty(argument.ValueAsString))
                    ReadFile(argument.ValueAsString);
            }
        }

        public override string[] Validate()
        {
            return (null);
        }

        public override void Prepare()
        {
        }

        public override void Update(params IBaseOutput[] requiredOutput)
        {
            throw new NotSupportedException("Data provider can not update its data");
        }

        public override void Finish()
        {
            return;
        }

        private void ReadFile(string inputFile)
        {
            TimeBufferer output = new TimeInterpolator();

            TimeSet timeSet = output.TTimeSet;
            IElementSet elementSet;

            StreamReader reader = new StreamReader(inputFile);

            // Read quantity and imply unit
            string line = GetNextLine(reader).Trim(' ', '"');
            Quantity quantity = new Quantity(line);
            if (line.Equals("Flow", StringComparison.InvariantCultureIgnoreCase))
                quantity.Unit = new Unit(PredefinedUnits.CubicMeterPerSecond);
            else if (line.Equals("WaterLevel", StringComparison.InvariantCultureIgnoreCase))
                quantity.Unit = new Unit(PredefinedUnits.Meter);
            else
                quantity.Unit = new Unit("Unspecified unit");
            output.ValueDefinition = quantity;

            // Read elementset
            line = GetNextLine(reader);
            string[] elements = line.Split(';');
            string elementTypeString = elements[0].Trim('"');
            if (elementTypeString.Equals("IdBased", StringComparison.InvariantCultureIgnoreCase))
            {
                var idelementSet = new ElementSet(inputFile + "-" + quantity.Caption, inputFile, ElementType.IdBased, "");
                for (int i = 1; i < elements.Length; i++)
                {
                    idelementSet.AddElement(new Element(elements[i].Trim('"')));
                }
                elementSet = idelementSet;
            }

            else if (elementTypeString.Equals("Points", StringComparison.InvariantCultureIgnoreCase))
            {
                var pelementSet = new ElementSet(inputFile + "-" + quantity.Caption, inputFile, ElementType.Point, "");
                for (int i = 1; i < elements.Length; i++)
                {
                    string[] coordinates = elements[i].Trim('"', '(', ')').Split(',');
                    if (coordinates.Length < 2)
                        throw new InvalidDataException("Invalid file format: only one coordinate for point: " + inputFile);
                    Element element = new Element();
                    double x = Double.Parse(coordinates[0], NumberFormatInfo.InvariantInfo);
                    double y = Double.Parse(coordinates[1], NumberFormatInfo.InvariantInfo);
                    Coordinate elmtCoor = new Coordinate(x, y);
                    element.Vertices = new Coordinate[] { elmtCoor };
                    pelementSet.AddElement(element);
                }
                elementSet = pelementSet;
            }
            else if (elementTypeString.Equals("RegularGrid", StringComparison.InvariantCultureIgnoreCase))
            {
                Spatial2DRegularGrid grid = new Spatial2DRegularGrid();
                grid.IsNodeBased = false;
                string[] parts = elements[1].Trim('"', '(', ')').Split(',');
                grid.X0 = Double.Parse(parts[0], NumberFormatInfo.InvariantInfo);
                grid.Y0 = Double.Parse(parts[1], NumberFormatInfo.InvariantInfo);
                grid.Dx = Double.Parse(parts[2], NumberFormatInfo.InvariantInfo);
                grid.Dy = Double.Parse(parts[3], NumberFormatInfo.InvariantInfo);
                grid.XCount = Int32.Parse(parts[4], NumberFormatInfo.InvariantInfo);
                grid.YCount = Int32.Parse(parts[5], NumberFormatInfo.InvariantInfo);
                grid.Orientation = (parts.Length < 5) ? 0 : Int32.Parse(parts[6], NumberFormatInfo.InvariantInfo);
                
                elementSet = new Spatial2DGridWrapper(grid);
            }
            else
            {
                throw new InvalidDataException("Invalid file format: Element type not reckognized: " + inputFile);
            }
            output.SpatialDefinition = elementSet;

            // Read times and values. First element is time, following are element values.
            ITimeSpaceValueSet valueSet = output.Values;
            while ((line = GetNextLine(reader)) != null)
            {
                string[] fileValues = line.Split(';');
                if (fileValues.Length - 1 != elementSet.ElementCount)
                    throw new InvalidDataException("Number of data does not match number of elements on line: \n" + line);

                DateTime timestamp = DateTime.ParseExact(fileValues[0].Trim('"'), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                Time time = new Time(timestamp);

                double[] locationValues = new double[fileValues.Length - 1];
                for (int i = 1; i < fileValues.Length; i++)
                {
                    locationValues[i - 1] = Double.Parse(fileValues[i].Trim('"'), NumberFormatInfo.InvariantInfo);
                }
                timeSet.Times.Add(time);
                valueSet.Values2D.Add(locationValues);
            }

            reader.Close();

            timeSet.SetTimeHorizonFromTimes();
            
            _outputs.Add(output);
        }

        /// <summary>
        /// Returns the next line from the file not being a comment line.
        /// </summary>
        private static string GetNextLine(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                if (!line.StartsWith("//"))
                    return (line);
            return (null);
        }


    }
}
