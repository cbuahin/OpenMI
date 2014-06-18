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
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    class RainRrCfComponents
    {
        public static ITimeSpaceComponent CreateRainfallMeasurementsInstance(string instanceId)
        {
            RainRrCfMeasurementsDatabase rainfallMeasurementsDatabaseInstance = new RainRrCfMeasurementsDatabase();
            List<IArgument> arguments = new List<IArgument>();
            AddArgument(arguments, "id", instanceId);
            rainfallMeasurementsDatabaseInstance.Initialize(arguments);
            return rainfallMeasurementsDatabaseInstance;
        }

        public static ITimeSpaceComponent CreateRainfallRunoffInstance(string instanceId)
        {
            RainRrCfTestComponent rainfallRunoffInstance = new RainRrCfTestComponent();
            List<IArgument> arguments = new List<IArgument>();
            AddArgument(arguments, "id", instanceId);

            AddArgument(arguments, "quantity", "rain;mm/day");
            AddArgument(arguments, "quantity", "runoff;m3/s");
            AddArgument(arguments, "quantity", "waterlevel;m");

            // rainfall catchment area's (polygons) will be created by component
            AddArgument(arguments, "elementSet", "catchment-polygons");
            AddArgument(arguments, "inputItem", "areaGelderland.rain");
            AddArgument(arguments, "inputItem", "areaUtrecht.rain");
            AddArgument(arguments, "inputItem", "areaZuidholland.rain");
            AddArgument(arguments, "inputItem", "areaBrabant.rain");

            AddArgument(arguments, "elementSet", "outputLoc-1");
            AddArgument(arguments, "elementSet", "outputLoc-2");
            AddArgument(arguments, "elementSet", "outputLoc-3");
            AddArgument(arguments, "outputItem", "outputLoc-1.runoff");
            AddArgument(arguments, "outputItem", "outputLoc-2.runoff");
            AddArgument(arguments, "outputItem", "outputLoc-3.runoff");
            AddArgument(arguments, "inputItem", "outputLoc-1.waterlevel");
            AddArgument(arguments, "inputItem", "outputLoc-2.waterlevel");
            AddArgument(arguments, "inputItem", "outputLoc-3.waterlevel");

            rainfallRunoffInstance.Initialize(arguments);
            return rainfallRunoffInstance;
        }

        public static ITimeSpaceComponent CreateChannelFlowInstance(string instanceId)
        {
            RainRrCfTestComponent channelFlowInstance = new RainRrCfTestComponent();
            List<IArgument> arguments = new List<IArgument>();
            AddArgument(arguments, "id", instanceId);

            AddArgument(arguments, "quantity", "waterlevel;m");
            AddArgument(arguments, "quantity", "discharge;m3/s");

            AddArgument(arguments, "elementSet", "lateral-1");
            AddArgument(arguments, "elementSet", "lateral-2");
            AddArgument(arguments, "elementSet", "lateral-3");
            AddArgument(arguments, "elementSet", "lateral-4");
            AddArgument(arguments, "elementSet", "node-1");
            AddArgument(arguments, "elementSet", "node-2");
            AddArgument(arguments, "elementSet", "node-3");
            AddArgument(arguments, "elementSet", "node-4");
            AddArgument(arguments, "elementSet", "node-5");

            AddArgument(arguments, "inputItem", "lateral-1.discharge");
            AddArgument(arguments, "inputItem", "lateral-2.discharge");
            AddArgument(arguments, "inputItem", "lateral-3.discharge");
            AddArgument(arguments, "inputItem", "lateral-4.discharge");

            AddArgument(arguments, "outputItem", "node-1.discharge");
            AddArgument(arguments, "outputItem", "node-1.waterlevel");
            AddArgument(arguments, "outputItem", "node-2.discharge");
            AddArgument(arguments, "outputItem", "node-2.waterlevel");
            AddArgument(arguments, "outputItem", "node-3.discharge");
            AddArgument(arguments, "outputItem", "node-3.waterlevel");
            AddArgument(arguments, "outputItem", "node-4.discharge");
            AddArgument(arguments, "outputItem", "node-4.waterlevel");
            AddArgument(arguments, "outputItem", "node-5.discharge");
            AddArgument(arguments, "outputItem", "node-5.waterlevel");

            channelFlowInstance.Initialize(arguments);
            return channelFlowInstance;
        }

        private static void AddArgument(ICollection<IArgument> arguments, string key, string value)
        {
            arguments.Add(Argument.Create(key, value, true, ""));
        }

    }
}