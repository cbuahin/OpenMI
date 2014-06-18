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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Examples.SimpleSCharpRiver
{
  public class RiverModelDelegateLC : RiverModelLC //, OpenMI.Standard2.IManageState
  {

    protected override EngineInputItem CreateInputInflowToOneNode(int i, string id, Quantity flowQuantity, IElementSet elementSet)
    {
      EngineDInputItem inputExchangeItem = new EngineDInputItem(id + ":Flow", flowQuantity, elementSet, this);
      int nodeIndex = i;
      inputExchangeItem.ValueSetter = delegate(ITimeSpaceValueSet values)
      {
        _inflowStorage[nodeIndex] += ((double)values.GetValue(0, 0)) *
                               _timeStepLengthInSeconds;
      };
      return inputExchangeItem;
    }

    protected override EngineInputItem CreateInputInflowToRiver(Quantity flowQuantity, IElementSet fullRiverElementSet)
    {
      EngineDInputItem wholeRiverFlowInputItem = new EngineDInputItem("WholeRiver:Flow", flowQuantity, fullRiverElementSet, this);
      wholeRiverFlowInputItem.ValueSetter = AddInflow;
      return wholeRiverFlowInputItem;
    }

    protected override EngineOutputItem CreateOutputFlowInBranch(int branchIndex, string id, Quantity flowQuantity, ElementSet elementSet)
    {
      EngineDOutputItem outputExchangeItem = new EngineDOutputItem(id + ":Flow", flowQuantity, elementSet, this);
      outputExchangeItem.ValueGetter = delegate()
      {
        IList res = new List<double>(1) { _flow[branchIndex] };
        return new ValueSet(new List<IList> { res });
      };
      return outputExchangeItem;
    }


    protected override EngineOutputItem CreateOutputFlowInRiver(Quantity flowQuantity, IElementSet fullRiverElementSet)
    {
      EngineDOutputItem wholeRiverFlowOutputExchangeItem = new EngineDOutputItem("WholeRiver:Flow", flowQuantity, fullRiverElementSet, this);
      wholeRiverFlowOutputExchangeItem.ValueGetter = GetFlowValues;
      return wholeRiverFlowOutputExchangeItem;
    }

    protected override EngineOutputItem CreateOutputLeakageInBranch(int i, string id, Quantity leakageQuantity, ElementSet elementSet)
    {
      EngineDOutputItem outputExchangeItem = new EngineDOutputItem(id + ":Leakage", leakageQuantity, elementSet, this);
      int branchIndex = i;
      outputExchangeItem.ValueGetter = delegate()
                                         {
                                           IList res = new List<double>(1) { _leakage[branchIndex] };
                                           return new ValueSet(new List<IList> { res });
                                         };
      return outputExchangeItem;
    }

    #region helper methods for exchange items

    private void AddInflow(ITimeSpaceValueSet values)
    {
      // values are numberOfNodes-1 long (input for each branch.
      // Put inflow at "upstream" node/storage
      IList elementValues = values.GetElementValuesForTime(0);
      for (int i = 0; i < _inflowStorage.Length - 1; i++)
      {
        _inflowStorage[i] += ((double)elementValues[i]) * _timeStepLengthInSeconds;
      }
    }

    private ITimeSpaceValueSet GetFlowValues()
    {
      IList values = new List<double>();
      for (int i = 0; i < _flow.Length; i++)
      {
        values.Add(_flow[i]);
      }
      return new ValueSet(new List<IList> { values });
    }

    #endregion
  }
}