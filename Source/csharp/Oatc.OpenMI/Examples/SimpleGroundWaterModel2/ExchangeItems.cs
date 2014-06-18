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
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using Oatc.OpenMI.Wrappers.EngineWrapper2;
using OpenMI.Standard2.TimeSpace;
using LinkableComponentMW2 = Oatc.OpenMI.Wrappers.EngineWrapper2.LinkableComponent;

namespace Oatc.OpenMI.Examples.SimpleGroundModel2
{
    public class AquiferLevel : ItemOutDoubleArrayBase
    {
		public AquiferLevel(string id, double[] initialValues, double relaxation, IElementSet es, LinkableComponentMW2 component)
            : base(id, new Time(component.TimeExtent.TimeHorizon.StampAsModifiedJulianDay), initialValues, relaxation)
        {
            Quantity q = new Quantity();
            q.Description = "Aquifer level [m]";
            q.Caption = "Aquifer Level";
            q.Unit = new Unit(PredefinedUnits.CubicMeterPerSecond);

            _iDescribable = new Describable(
                string.Format("{0}, {1}", q.Caption, es.Caption),
                string.Format("{0}, {1}", q.Description, es.Description));

            _iValueDefinition = q;
            _iElementSet = es;
            _iComponent = component;
        }
    }

    public class Inflow : ItemInDoubleArrayBase
    {
		public Inflow(string id, double[] defaultValues, double timeTolerance, IElementSet es, LinkableComponentMW2 component)
            : base(id, component.TimeExtent, defaultValues, timeTolerance)
        {
            Quantity q = new Quantity();
            q.Description = "Inflow [m3/s]";
            q.Caption = "Flow";
            q.Unit = new Unit(PredefinedUnits.CubicMeterPerSecond);

            _iDescribable = new Describable(
                string.Format("{0}, {1}", q.Caption, es.Caption),
                string.Format("{0}, {1}", q.Description, es.Description));

            _iValueDefinition = q;
            _iElementSet = es;
            _iComponent = component;
        }
    }

    public class AquiferStorage : ItemOutDoubleArrayBase
    {
		public AquiferStorage(string id, double[] initialValues, double relaxation, IElementSet es, LinkableComponentMW2 component)
            : base(id, new Time(component.TimeExtent.TimeHorizon.StampAsModifiedJulianDay), initialValues, relaxation)
        {
            Quantity q = new Quantity();
            q.Description = "Aquifer Storage [m3]";
            q.Caption = "Aquifer Storage";

            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, 3);
            Unit unit = new Unit("Storage", 1.0, 0.0, "Storage");
            unit.Dimension = dimension;

            q.Unit = unit;

            _iDescribable = new Describable(
                string.Format("{0}, {1}", q.Caption, es.Caption),
                string.Format("{0}, {1}", q.Description, es.Description));

            _iValueDefinition = q;
            _iElementSet = es;
            _iComponent = component;
        }
    }
}
