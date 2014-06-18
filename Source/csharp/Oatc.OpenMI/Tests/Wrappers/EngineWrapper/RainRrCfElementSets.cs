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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
    internal class RainRrCfElementSets
    {
        public static IDictionary<string, IElementSet> CreateRainfallMeasurementsElementSets()
        {
            IDictionary<string, IElementSet> elementSets = new Dictionary<string, IElementSet>();

            String areaId = "IsoHyet-1";
            ElementSet elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            Element element = new Element(areaId);
            element.AddVertex(new Coordinate(75, 25, 0));
            element.AddVertex(new Coordinate(125, 25, 0));
            element.AddVertex(new Coordinate(125, 125, 0));
            element.AddVertex(new Coordinate(75, 125, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            areaId = "IsoHyet-2";
            elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            element = new Element(areaId);
            element.AddVertex(new Coordinate(125, 25, 0));
            element.AddVertex(new Coordinate(175, 25, 0));
            element.AddVertex(new Coordinate(175, 175, 0));
            element.AddVertex(new Coordinate(125, 175, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            return elementSets;
        }

        public static IDictionary<string, IElementSet> CreateRainfallRunoffElementSets()
        {
            IDictionary<string, IElementSet> elementSets = new Dictionary<string, IElementSet>();

            String areaId = "areaGelderland";
            ElementSet elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            Element element = new Element(areaId);
            element.AddVertex(new Coordinate(150, 50, 0));
            element.AddVertex(new Coordinate(250, 50, 0));
            element.AddVertex(new Coordinate(250, 150, 0));
            element.AddVertex(new Coordinate(150, 150, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            areaId = "areaUtrecht";
            elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            element = new Element(areaId);
            element.AddVertex(new Coordinate(100, 50, 0));
            element.AddVertex(new Coordinate(150, 50, 0));
            element.AddVertex(new Coordinate(150, 100, 0));
            element.AddVertex(new Coordinate(100, 100, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            areaId = "areaZuidholland";
            elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            element = new Element(areaId);
            element.AddVertex(new Coordinate(0, 50, 0));
            element.AddVertex(new Coordinate(100, 50, 0));
            element.AddVertex(new Coordinate(100, 100, 0));
            element.AddVertex(new Coordinate(0, 100, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            areaId = "areaBrabant";
            elementSet = new ElementSet(areaId, areaId, ElementType.Polygon, "RijksdriehoekCoordinaten");
            element = new Element(areaId);
            element.AddVertex(new Coordinate(50, 0, 0));
            element.AddVertex(new Coordinate(200, 0, 0));
            element.AddVertex(new Coordinate(200, 50, 0));
            element.AddVertex(new Coordinate(50, 50, 0));
            elementSet.AddElement(element);
            elementSets.Add(areaId, elementSet);

            return elementSets;
        }
    }
}