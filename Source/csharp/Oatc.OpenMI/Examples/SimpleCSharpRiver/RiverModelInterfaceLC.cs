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
using System.Windows.Forms.VisualStyles;
using Oatc.OpenMI.Examples.SimpleSCharpRiver;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest1
{
  public class RiverModelInterfaceLC : RiverModelLC //, OpenMI.Standard2.IManageState
  {

    protected override EngineOutputItem CreateOutputFlowInRiver(Quantity leakageQuantity, IElementSet fullRiverElementSet)
    {
      EngineIOutputItem wholeRiverFlowOutputExchangeItem = new EngineIOutputItem("WholeRiver:Flow", leakageQuantity, fullRiverElementSet, this);
      wholeRiverFlowOutputExchangeItem.ValueGetter = new VectorValueGetSetter<double>(_flow);
      return wholeRiverFlowOutputExchangeItem;
    }

    protected override EngineOutputItem CreateOutputFlowInBranch(int i, string id, Quantity flowQuantity, ElementSet elementSet)
    {
      EngineIOutputItem outputExchangeItem = new EngineIOutputItem(id + ":Flow", flowQuantity, elementSet, this);
      outputExchangeItem.ValueGetter = new ValueToVectorGetSetter<double>(_flow, i);
      return outputExchangeItem;
    }

    protected override EngineOutputItem CreateOutputLeakageInBranch(int i, string id, Quantity leakageQuantity, ElementSet elementSet)
    {
      EngineIOutputItem outputExchangeItem = new EngineIOutputItem(id + ":Leakage", leakageQuantity, elementSet, this);
      outputExchangeItem.ValueGetter = new ValueToVectorGetSetter<double>(_leakage, i);
      return outputExchangeItem;
    }
    
    protected override EngineOutputItem CreateOuputLeakageInRiver(Quantity leakageQuantity, IElementSet fullRiverElementSet)
    {
      EngineIOutputItem wholeRiverLeakageOutputExchangeItem = new EngineIOutputItem("WholeRiver:Leakage", leakageQuantity, fullRiverElementSet, this);
      wholeRiverLeakageOutputExchangeItem.ValueGetter = new VectorValueGetSetter<double>(_leakage);
      return wholeRiverLeakageOutputExchangeItem;
    }


  }
}
