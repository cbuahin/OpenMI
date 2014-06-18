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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    class RainRrCfCompositions
    {
        public const string rainfallMeasurementsId = "Rainfall Measurement";
        public const string rainfallRunoffId = "Rainfall Runoff";
        public const string channelFlowId = "Channel Flow";

        public const string timeExtrapolatorId = "TimeExtrapolator";

        public static ICollection<ITimeSpaceComponent> CreateCompositionA()
        {
            // create the involved components
            IDictionary<string, ITimeSpaceComponent> components = CreateComponentInstances();

            // connect rainfall measurements to rainfall runoff measuring stations
            ConnectItemsWithTimeInterpolator(components[rainfallMeasurementsId], "IsoHyet-1.Rain",
                                             components[rainfallRunoffId], "areaGelderland.rain");
            ConnectItemsWithTimeInterpolator(components[rainfallMeasurementsId], "IsoHyet-2.Rain",
                                             components[rainfallRunoffId], "areaZuidholland.rain");

            // connect rainfall runoff outflows to channel flow lateral discharges
            ConnectItems(components[rainfallRunoffId], "outputLoc-1.runoff", components[channelFlowId],
                         "lateral-1.discharge");
            ConnectItems(components[rainfallRunoffId], "outputLoc-2.runoff", components[channelFlowId],
                         "lateral-2.discharge");

            // validate the composition
            foreach (ITimeSpaceComponent component in components.Values)
            {
                component.Validate();
            }

            // return the composition
            return components.Values;
        }

        public static ICollection<ITimeSpaceComponent> CreateCompositionB(bool useTimeExtrapolator)
        {
            // create the involved components
            IDictionary<string, ITimeSpaceComponent> components = CreateComponentInstances();

            // connect rainfall measurements to rainfall runoff measuring stations
            ConnectItemsWithTimeInterpolator(components[rainfallMeasurementsId], "IsoHyet-1.Rain",
                                             components[rainfallRunoffId], "areaGelderland.rain");
            ConnectItemsWithTimeInterpolator(components[rainfallMeasurementsId], "IsoHyet-2.Rain",
                                             components[rainfallRunoffId], "areaZuidholland.rain");

            // connect rainfall runoff outflows to channel flow lateral discharges
            ConnectItems(components[rainfallRunoffId], "outputLoc-1.runoff", components[channelFlowId],
                         "lateral-1.discharge");
            ConnectItems(components[rainfallRunoffId], "outputLoc-2.runoff", components[channelFlowId],
                         "lateral-2.discharge");

            // connect rainfall runoff outflows to channel flow lateral discharges,
            // either direct, or with a time extrapolator adapted output in between
            if (useTimeExtrapolator)
            {
                ConnectItemsUsingAdaptedOutput(components[channelFlowId], "node-2.waterlevel", components[rainfallRunoffId],
                                               "outputLoc-2.waterlevel", timeExtrapolatorId);
            }
            else
            {
                ConnectItems(components[channelFlowId], "node-2.waterlevel", components[rainfallRunoffId],
                             "outputLoc-2.waterlevel");
            }

            // validate the composition
            foreach (ITimeSpaceComponent component in components.Values)
            {
                component.Validate();
            }

            // return the composition
            return components.Values;
        }

        private static IDictionary<string, ITimeSpaceComponent> CreateComponentInstances()
        {
            // create the involved components (rainfall measurements, rainfall runoff, 1D channel flow

            IDictionary<string, ITimeSpaceComponent> components = new Dictionary<string, ITimeSpaceComponent>();

            ITimeSpaceComponent rainfallMeasurementsInstance = RainRrCfComponents.CreateRainfallMeasurementsInstance(rainfallMeasurementsId);
            ITimeSpaceComponent rainfallRunoffInstance = RainRrCfComponents.CreateRainfallRunoffInstance(rainfallRunoffId);
            ITimeSpaceComponent channelFlowLC = RainRrCfComponents.CreateChannelFlowInstance(channelFlowId);

            components.Add(rainfallMeasurementsId, rainfallMeasurementsInstance);
            components.Add(rainfallRunoffId, rainfallRunoffInstance);
            components.Add(channelFlowId, channelFlowLC);

            return components;
        }

        private static void ConnectItemsWithTimeInterpolator(ITimeSpaceComponent sourceComponentInstance,
                                                             string outputItemId,
                                                             ITimeSpaceComponent targetComponentInstance, string inputItemId)
        {
            ITimeSpaceOutput output = FindOutputItem(sourceComponentInstance, outputItemId);
            ITimeSpaceInput input = FindInputItem(targetComponentInstance, inputItemId);
            IAdaptedOutputFactory derivedOutputFactory = sourceComponentInstance.AdaptedOutputFactories[0];
            IIdentifiable[] derivedOutputIdentifiers = derivedOutputFactory.GetAvailableAdaptedOutputIds(output, input);
            IBaseAdaptedOutput timeInterpolator = derivedOutputFactory.CreateAdaptedOutput(derivedOutputIdentifiers[0], output, input);
        }

        public static void ConnectItemsUsingAdaptedOutput(ITimeSpaceComponent sourceComponentInstance,
                                                          string outputItemId,
                                                          ITimeSpaceComponent targetComponentInstance,
                                                          string inputItemId,
                                                          string derivedOutputName)
        {
            ITimeSpaceOutput output = FindOutputItem(sourceComponentInstance, outputItemId);
            ITimeSpaceInput input = FindInputItem(targetComponentInstance, inputItemId);
            ConnectItemsUsingAdaptedOutput(sourceComponentInstance, output, input, derivedOutputName);
        }

        public static IBaseAdaptedOutput ConnectItemsUsingAdaptedOutput(ITimeSpaceComponent sourceComponentInstance, ITimeSpaceOutput output, ITimeSpaceInput input, string adaptedOutputId)
        {
            IAdaptedOutputFactory adaptedOutputFactory = sourceComponentInstance.AdaptedOutputFactories[0];
            IIdentifiable[] adaptedOutputIds = adaptedOutputFactory.GetAvailableAdaptedOutputIds(output, input);
            IIdentifiable adaptedOutputIdentifier = null;
            foreach (IIdentifiable identifier in adaptedOutputIds)
            {
                if (identifier.Id.StartsWith(adaptedOutputId))
                {
                    adaptedOutputIdentifier = identifier;
                }
            }
            if (adaptedOutputIdentifier == null)
            {
                throw new Exception("AdaptedOutput with name \"" + adaptedOutputId + "\" not found");
            }
            IBaseAdaptedOutput timeExtrapolator = adaptedOutputFactory.CreateAdaptedOutput(adaptedOutputIdentifier,
                                                                                       output, input);
            return timeExtrapolator;
        }

        private static void ConnectItems(ITimeSpaceComponent sourceComponentInstance, string outputItemId,
                                         ITimeSpaceComponent targetComponentInstance, string inputItemId)
        {
            ITimeSpaceOutput output = FindOutputItem(sourceComponentInstance, outputItemId);
            ITimeSpaceInput input = FindInputItem(targetComponentInstance, inputItemId);
            output.AddConsumer(input);
        }

        public static ITimeSpaceComponent FindComponent(IEnumerable<ITimeSpaceComponent> components, string componentId)
        {
            foreach (ITimeSpaceComponent component in components)
            {
                if (component.Id.Equals(componentId))
                {
                    return component;
                }
            }
            throw new Exception("Component \"" + componentId + "\" not found in component list");
        }

        public static ITimeSpaceInput FindInputItem(ITimeSpaceComponent componentInstance, string inputItemId)
        {
            foreach (ITimeSpaceInput inputItem in componentInstance.Inputs)
            {
                if (inputItem.Id.Equals(inputItemId))
                {
                    return inputItem;
                }
            }
            throw new Exception("Input item \"" + inputItemId + "\" not found in component " + componentInstance.Id);
        }

        public static ITimeSpaceOutput FindOutputItem(ITimeSpaceComponent componentInstance, string outputItemId)
        {
            foreach (ITimeSpaceOutput outputItem in componentInstance.Outputs)
            {
                if (outputItem.Id.Equals(outputItemId))
                {
                    return outputItem;
                }
            }
            throw new Exception("Output item \"" + outputItemId + "\" not found in component " + componentInstance.Id);
        }

    }
}