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
// file: GWModelEngine.cs
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
  /// Summary description for GWModelEngine.
  /// </summary>
  public class GWModelEngine : IEngine
  {
    private readonly ILinkableComponent myLinkableComponent;
    private readonly List<IInputItem> _inputExchangeItems;
    private readonly List<IOutputItem> _outputExchangeItems;
    private readonly int _numberOfElements;
    private readonly DateTime _simulationEnd;
    private readonly DateTime _simulationStart;
    private readonly double[] _storage;
    private readonly double _timeStepLength; //[seconds]
    private int _currentTimeStepNumber;

    public GWModelEngine(ILinkableComponent myLinkableComponent)
    {
      this.myLinkableComponent = myLinkableComponent;
      _numberOfElements = 4;
      _simulationStart = new DateTime(2005, 1, 1, 0, 0, 0);
      _simulationEnd = new DateTime(2005, 2, 10, 0, 0, 0);
      _timeStepLength = 3600 * 24; //one day

      _inputExchangeItems = new List<IInputItem>();
      _outputExchangeItems = new List<IOutputItem>();

      _storage = new double[_numberOfElements];

      for (int i = 0; i < _numberOfElements; i++)
      {
        _storage[i] = 0;
      }

      _currentTimeStepNumber = 0;
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
      return "GWModelEngineModelID";
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
      return "GWModelEngineModelDescription";
    }

    public int GetOutputExchangeItemCount()
    {
      return _outputExchangeItems.Count;
    }

    public string GetComponentID()
    {
      return "GWModelEngineComponentID";
    }

    public void Finish()
    {
    }

    public ITime GetCurrentTime()
    {
      double duration = _currentTimeStepNumber * _timeStepLength / (24.0 * 3600.0);
      return new Time(_simulationStart, duration);
    }

    public double[] GetValues(string QuantityID, string ElementSetID)
    {
      if (ElementSetID.Equals("RegularGrid"))
        return _storage;

        double[] res = new double[1];
        res[0] = _storage[0];
        return (res);
    }

    public void SetValues(string QuantityID, string ElementSetID, double[] values)
    {
      if (ElementSetID.Equals("RegularGrid"))
      {
        for (int i = 0; i < _storage.Length; i++)
        {
          _storage[i] = values[i];
        }
      }
      else //if (ElementSetID.Equals("FirstElement"))
      {
        _storage[0] = values[0];
      }
    }


    public void Dispose()
    {
    }

    public string GetComponentDescription()
    {
      return "GWModelEngineComponentDescription";
    }

    public void Initialize(Hashtable properties)
    {
      const double ox = 2.0;
      const double oy = 2.0;
      const double dx = 4.0;
      const double dy = 4.0;

      // -- Populate Input Exchange Items ---

      // Element set for an Polygon based item
      Element element0 = new Element("element:0");
      element0.AddVertex(new Coordinate(ox, oy, 0));
      element0.AddVertex(new Coordinate(ox + dx, oy, 0));
      element0.AddVertex(new Coordinate(ox + dx, oy + dy, 0));
      element0.AddVertex(new Coordinate(ox, oy + dy, 0));

      Element element1 = new Element("element:1");
      element1.AddVertex(new Coordinate(ox + dx, oy, 0));
      element1.AddVertex(new Coordinate(ox + 2 * dx, oy, 0));
      element1.AddVertex(new Coordinate(ox + 2 * dx, oy + dy, 0));
      element1.AddVertex(new Coordinate(ox + dx, oy + dy, 0));

      Element element2 = new Element("element:2");
      element2.AddVertex(new Coordinate(ox, oy + dy, 0));
      element2.AddVertex(new Coordinate(ox + dx, oy + dy, 0));
      element2.AddVertex(new Coordinate(ox + dx, oy + 2 * dy, 0));
      element2.AddVertex(new Coordinate(ox, oy + 2 * dy, 0));

      Element element3 = new Element("element:3");
      element3.AddVertex(new Coordinate(ox + dx, oy + dy, 0));
      element3.AddVertex(new Coordinate(ox + 2 * dx, oy + dy, 0));
      element3.AddVertex(new Coordinate(ox + 2 * dx, oy + 2 * dy, 0));
      element3.AddVertex(new Coordinate(ox + dx, oy + 2 * dy, 0));

      ElementSet regularGridSet =
        new ElementSet("RegularGrid", "RegularGrid", ElementType.Polygon);
      regularGridSet.AddElement(element0);
      regularGridSet.AddElement(element1);
      regularGridSet.AddElement(element2);
      regularGridSet.AddElement(element3);

      // Element set for an ID based item
      ElementSet idSet =
        new ElementSet("FirstElement", "FirstElement", ElementType.IdBased);
      idSet.AddElement(element0);   // is an IdBased set required to have elements?

      Quantity storageQuantity =
        new Quantity(new Unit("Storage", 1.0, 0.0, "Storage"), "Storage", "Storage");

      InputItem regularGridInput = new InputItem("grid.Storage", storageQuantity, regularGridSet);
      regularGridInput.Component = myLinkableComponent;
      InputItem idInput = new InputItem("FirstElement.Storage", storageQuantity, idSet);
      idInput.Component = myLinkableComponent;
      _inputExchangeItems.Add(regularGridInput);
      _inputExchangeItems.Add(idInput);

      OutputItem regularGridOutput = new OutputItem("grid.Storage", storageQuantity, regularGridSet);
      regularGridOutput.Component = myLinkableComponent;
      OutputItem idOutput = new OutputItem("FirstElement.Storage", storageQuantity, idSet);
      idOutput.Component = myLinkableComponent;
      _outputExchangeItems.Add(regularGridOutput);
      _outputExchangeItems.Add(idOutput);
    }

    public bool PerformTimeStep()
    {
      _currentTimeStepNumber++;
      return true;
    }

    public double GetMissingValueDefinition()
    {
      return -999.99;
    }

    #endregion
  }
}