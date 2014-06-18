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
#region Copyright

///////////////////////////////////////////////////////////
//
// namespace: org.OpenMI.Utilities.Wrapper.UnitTest 
// purpose: UnitTest for the org.OpenMI.Utilities.Wrapper package
// file: RiverModelEngine.cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper.UnitTest
{
  /// <summary>
  /// Summary description for RiverModelEngine.
  /// </summary>
  public class RiverModelEngine : IEngine, IManageState
  {
    private readonly ILinkableComponent myLinkableComponent;
    private readonly double[] _flow; //[liter pr second]
    private readonly List<IInputItem> _inputExchangeItems;
    private readonly List<IOutputItem> _outputExchangeItems;
    private readonly double[] _leakage; //[liter pr second]
    private readonly int _numberOfNodes;
    private readonly double _runoff; //[liter pr second]
    private readonly DateTime _simulationEnd;
    private readonly DateTime _simulationStart;

    private readonly double[] _storage; //[liters]
    private readonly double[] _xCoordinate; // x-coordinates for the nodes
    private readonly double[] _yCoordinate; // y-coordinates for the nodes
    private int _currentTimeStepNumber;
    public bool _disposeMethodWasInvoked;
    public bool _finishMethodWasInvoked;
    public bool _initializeMethodWasInvoked;
    private string _modelID;
    private int _stateIdCreator;
    public ArrayList _states;
    private double _timeStepLength; //[seconds]


    public RiverModelEngine(ILinkableComponent myLinkableComponent)
    {
      this.myLinkableComponent = myLinkableComponent;
      _modelID = "TestRiverModel Model Caption";
      _xCoordinate = new double[] { 3, 5, 8, 8 };
      _yCoordinate = new double[] { 9, 7, 7, 3 };

      _numberOfNodes = _xCoordinate.Length;

      _simulationStart = new DateTime(2005, 1, 1, 0, 0, 0);
      _simulationEnd = new DateTime(2005, 2, 10, 0, 0, 0);
      _timeStepLength = 3600 * 24; //one day

      _inputExchangeItems = new List<IInputItem>();
      _outputExchangeItems = new List<IOutputItem>();

      _storage = new double[_numberOfNodes];
      for (int i = 0; i < _numberOfNodes; i++)
      {
        _storage[i] = 0;
      }
      _flow = new double[_numberOfNodes - 1];
      _leakage = new double[_numberOfNodes - 1];
      _runoff = 10;
      _currentTimeStepNumber = 0;

      _initializeMethodWasInvoked = false;
      _finishMethodWasInvoked = false;
      _disposeMethodWasInvoked = false;

      _states = new ArrayList();
      _stateIdCreator = 0;
    }

    #region IEngine Members

    public IInputItem GetInputExchangeItem(int exchangeItemIndex)
    {
      return _inputExchangeItems[exchangeItemIndex];
    }

    public ITime GetTimeHorizon()
    {
      return new Time(_simulationStart, _simulationEnd);
    }

    public string GetModelID()
    {
      return _modelID;
    }

    public int GetInputExchangeItemCount()
    {
      return _inputExchangeItems.Count;
    }

    public IOutputItem GetOutputExchangeItem(int exchangeItemIndex)
    {
      return _outputExchangeItems[exchangeItemIndex];
    }

    public string GetModelDescription()
    {
      return "Test River model - Model description";
    }

    public int GetOutputExchangeItemCount()
    {
      return _outputExchangeItems.Count;
    }

    public string GetComponentID()
    {
      return "Test River Model Component Caption";
    }

    public void Finish()
    {
      _finishMethodWasInvoked = true;
    }

    public ITime GetCurrentTime()
    {
      double duration = _currentTimeStepNumber * _timeStepLength / (24.0 * 3600.0);
      return new Time(_simulationStart, duration);
    }

    public double[] GetValues(string QuantityID, string ElementSetID)
    {
      char[] separator = new char[] { ':' };
      double[] x;

      if (ElementSetID.Split(separator)[0] == "Branch")
      {
        x = new double[1];
        int branchIndex = Convert.ToInt32(ElementSetID.Split(separator)[1]);
        if (QuantityID == "Flow")
        {
          x[0] = _flow[branchIndex];
        }
        else if (QuantityID == "Leakage")
        {
          x[0] = _leakage[branchIndex];
        }
        else
        {
          throw new Exception("Quantity Caption not recognized in GetValues method");
        }
      }
      else if (ElementSetID.Split(separator)[0] == "WholeRiver")
      {
        x = new double[_leakage.Length];
        for (int i = 0; i < _leakage.Length; i++)
        {
          x[i] = _leakage[i];
        }
      }
      else
      {
        throw new Exception("Failed to recognize ElementSetID in method GetValues");
      }

      return x;
    }

    public void Dispose()
    {
      _disposeMethodWasInvoked = true;
    }

    public string GetComponentDescription()
    {
      return "Test River model component description";
    }

    public void Initialize(Hashtable properties)
    {
      if (properties.ContainsKey("ModelID"))
      {
        _modelID = (string)properties["ModelID"];
      }

      if (properties.ContainsKey("TimeStepLength"))
      {
        _timeStepLength = Convert.ToDouble((string)properties["TimeStepLength"]);
      }

      // -- create a flow quanitity --
      Dimension flowDimension = new Dimension();
      flowDimension.SetPower(DimensionBase.Length, 3);
      flowDimension.SetPower(DimensionBase.Time, -1);
      
      Unit literPrSecUnit = new Unit("LiterPrSecond", 0.001, 0, "Liters pr Second");
      
      Quantity flowQuantity = new Quantity(literPrSecUnit, "Flow", "Flow");
      Quantity leakageQuantity = new Quantity(literPrSecUnit, "Leakage", "Leakage");

      // -- create and populate elementset to represente the whole river network --
      ElementSet fullRiverElementSet =
        new ElementSet("WholeRiver", "WholeRiver", ElementType.PolyLine);
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        Element element = new Element();
        element.Id = "Branch:" + i;
        element.AddVertex(new Coordinate(_xCoordinate[i], _yCoordinate[i], -999));
        element.AddVertex(new Coordinate(_xCoordinate[i + 1], _yCoordinate[i + 1], -999));
        fullRiverElementSet.AddElement(element);
      }

      // --- Populate input exchange item for flow to the whole georeferenced river ---
      InputItem wholeRiverInputExchangeItem = new InputItem("WholeRiver.Flow", flowQuantity, fullRiverElementSet);
      wholeRiverInputExchangeItem.Component = myLinkableComponent;
      _inputExchangeItems.Add(wholeRiverInputExchangeItem);

      // --- populate input exchange items for flow to individual nodes ---
      for (int i = 0; i < _numberOfNodes; i++)
      {
        Element element = new Element();
        element.Id = "Node:" + i;
        ElementSet elementSet =
          new ElementSet("Individual nodes", "Node:" + i, ElementType.IdBased);
        elementSet.AddElement(element);

        InputItem inputExchangeItem = new InputItem("Individual nodes.Flow", flowQuantity, elementSet);
        inputExchangeItem.Component = myLinkableComponent;
        _inputExchangeItems.Add(inputExchangeItem);
      }

      // --- Populate output exchange items for flow in river branches ---
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        Element element = new Element();
        element.Id = "Branch:" + i;
        ElementSet elementSet =
          new ElementSet("Individual nodes", "Branch:" + i, ElementType.IdBased);
        elementSet.AddElement(element);

        OutputItem outputExchangeItem = new OutputItem("",flowQuantity, elementSet);
          outputExchangeItem.Component = myLinkableComponent;
        _outputExchangeItems.Add(outputExchangeItem);
      }

      // --- polulate output exchange items for leakage for individual branches --
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        Element element = new Element();
        element.Id = "Branch:" + i;
        ElementSet elementSet =
          new ElementSet("Individual nodes", "Branch:" + i, ElementType.IdBased);
        elementSet.AddElement(element);

        BufferedOutputItem outputExchangeItem = new BufferedOutputItem("Individual nodes.Leakage", leakageQuantity, elementSet, myLinkableComponent);
        _outputExchangeItems.Add(outputExchangeItem);
      }

      // --- Populate output exchange item for leakage from the whole georeferenced river ---
      BufferedOutputItem wholeRiverOutputExchangeItem = new BufferedOutputItem("WholeRiver.Leakage", leakageQuantity, fullRiverElementSet, myLinkableComponent);
      _outputExchangeItems.Add(wholeRiverOutputExchangeItem);

      // --- populate with initial state variables ---
      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        _flow[i] = 7;
      }

      _currentTimeStepNumber = 1;
      _initializeMethodWasInvoked = true;
    }

    public bool PerformTimeStep()
    {
      for (int i = 0; i < _numberOfNodes; i++)
      {
        _storage[i] += _runoff * _timeStepLength;
      }

      for (int i = 0; i < _numberOfNodes - 1; i++)
      {
        _flow[i] = 0.5 * _storage[i] / _timeStepLength;
        _leakage[i] = _flow[i];
        _storage[i + 1] += 0.5 * _storage[i];
      }

      for (int i = 0; i < _numberOfNodes; i++)
      {
        _storage[i] = 0;
      }

      _currentTimeStepNumber++;

      // -- debug output writing ----
//        string outstring = "TsNo:" + _currentTimeStepNumber + " ";
//        ITime currentTime = GetCurrentTime();
//        outstring += Time.ToDateTime(currentTime.StampAsModifiedJulianDay) + " - ";
//        outstring += Time.ToDateTime(currentTime.StampAsModifiedJulianDay + currentTime.DurationInDays) + " - ";
//        for (int n = 0; n < _numberOfNodes -1; n++)
//        {
//            outstring += " F" + n + ": " + _flow[n];
//        }
//        Console.WriteLine(outstring);


      // ----------------------------

      return true;
    }

    public double GetMissingValueDefinition()
    {
      return -999;
    }

    public void SetValues(string QuantityID, string ElementSetID, double[] values)
    {
      char[] separator = new char[] { ':' };

      if (ElementSetID == "WholeRiver")
      {
        if (values.Length != _numberOfNodes - 1)
        {
          throw new Exception("Illegal number of values in ValueSet in argument to SetValues method");
        }
        for (int i = 1; i < _numberOfNodes; i++)
        {
          _storage[i] += values[i - 1] * _timeStepLength;
        }
      }
      else if (ElementSetID.Split(separator)[0] == "Node")
      {
        if (values.Length != 1)
        {
          throw new Exception("illigal number of values in ValueSet in argument to SetValues method");
        }
        int nodeIndex = Convert.ToInt32(ElementSetID.Split(separator)[1]);
        _storage[nodeIndex] += values[0] * _timeStepLength;
      }
      else
      {
        throw new Exception("Failed to recognize ElementSetID in method SetValues");
      }
    }

    #endregion

    #region IManageState Members

    public IIdentifiable KeepCurrentState()
    {
      _stateIdCreator++;

      string stateID = "state:" + _stateIdCreator;

      RiverModelState state = new RiverModelState(stateID, _currentTimeStepNumber);
      _states.Add(state);

      return state;
    }

    public void RestoreState(IIdentifiable stateID)
    {
      int index = -999;

      for (int i = 0; i < _states.Count; i++)
      {
        if (((RiverModelState)_states[i]).StateId.Equals(stateID))
        {
          index = i;
        }
      }

      if (index < 0)
      {
        throw new Exception("Failed to find stateID in RestoreState method");
      }
      _currentTimeStepNumber = ((RiverModelState)_states[index]).TimeStepNumber;
    }

    public void ClearState(IIdentifiable stateID)
    {
      int index = -999;

      for (int i = 0; i < _states.Count; i++)
      {
        if (((RiverModelState)_states[i]).StateId.Equals(stateID))
        {
          index = i;
        }
      }

      if (index < 0)
      {
        throw new Exception("Failed to find stateID in RemoveState method");
      }
      _states.RemoveAt(index);
    }

    #endregion
  }
}