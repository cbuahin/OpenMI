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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper2;
using OpenMI.Standard2.TimeSpace;
using LinkableComponentMW2 = Oatc.OpenMI.Wrappers.EngineWrapper2.LinkableComponent;

namespace Oatc.OpenMI.Examples.SimpleCSharpRiver2
{
    public class InflowAtNode : ItemInDoubleBase
    {
        public InflowAtNode(string idAndCaption, Point2D point, double defaultValue, double timeTolerance, LinkableComponentMW2 component)
            : base(idAndCaption, component.TimeExtent, defaultValue, timeTolerance)
        {
            Quantity q = new Quantity(
                new Unit(PredefinedUnits.CubicMeterPerSecond),
                "Flow",  // Description
                "Flow"); // Caption
            q.ValueType = typeof(double);

            ElementSet2D es = new ElementSet2D(idAndCaption, ElementType.Point);
            es.AddElement(new Point2D[] { point });

            Initialise(component, q, es);
        }
    }

    public class GroundWaterLevelAtNode : ItemInDoubleBase
    {
        public GroundWaterLevelAtNode(string idAndCaption, Point2D point, double defaultValue, double timeTolerance, LinkableComponentMW2 component)
            : base(idAndCaption, component.TimeExtent, defaultValue, timeTolerance)
        {
            Quantity q = new Quantity(
                new Unit(PredefinedUnits.Meter),
                "Ground Water Level",  // Description
                "Ground Water Level"); // Caption
            q.ValueType = typeof(double);

            ElementSet2D es = new ElementSet2D(idAndCaption, ElementType.Point);
            es.AddElement(new Point2D[] { point });

            Initialise(component, q, es);
        }
    }

    public class FlowAlongBranch : ItemOutDoubleBase
    {
        public FlowAlongBranch(string idAndCaption, List<Point2D[]> polylines, double initialValue, double relaxation, LinkableComponentMW2 component)
            : base(idAndCaption, new Time(component.TimeExtent.TimeHorizon.StampAsModifiedJulianDay), initialValue, relaxation)
        {
            Quantity q = new Quantity(
                new Unit(PredefinedUnits.CubicMeterPerSecond),
                "Flow",  // Description
                "Flow"); // Caption
            q.ValueType = typeof(double);

            ElementSet2D es = new ElementSet2D(idAndCaption, ElementType.PolyLine);
            es.AddRangeElements(polylines);

            Initialise(component, q, es);
        }
    }
}
