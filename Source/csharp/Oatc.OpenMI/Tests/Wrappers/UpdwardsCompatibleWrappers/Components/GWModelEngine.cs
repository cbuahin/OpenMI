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
using System.Collections;
using Oatc.UpwardsComp.Backbone;
using Oatc.UpwardsComp.EngineWrapper;
using Oatc.UpwardsComp.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;
using UpwardsComp.Backbone;
using ITime=Oatc.UpwardsComp.Standard.ITime;

namespace Oatc.OpenMI.Wrappers.UpdwardsCompatibleWrappers.UnitTests.Components
{
    /// <summary>
    /// Summary description for GWModelEngine.
    /// </summary>
    public class GWModelEngine  :	IEngine
    {
		
        DateTime  _simulationStart; 
        DateTime  _simulationEnd;   
        double    _timeStepLength;      //[seconds]
        ArrayList _inputExchangeItems;
        ArrayList _outputExchangeItems;
        int        _currentTimeStepNumber;
        int        _numberOfElements;
        double[]   _storage;

        public GWModelEngine()
        {
            _numberOfElements = 4;
            _simulationStart = new DateTime(2005,1,1,0,0,0);
            _simulationEnd   = new DateTime(2005,2,10,0,0,0);
            _timeStepLength      = 3600*24;  //one day

            _inputExchangeItems = new ArrayList();
            _outputExchangeItems = new ArrayList();

            _storage = new double[_numberOfElements];

            for ( int i = 0; i < _numberOfElements; i++)
            {
                _storage[i] = 0;
            }

            _currentTimeStepNumber = 0;
        }

        #region IEngine Members

        public InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
        {
            return (InputExchangeItem) _inputExchangeItems[exchangeItemIndex];
        }

        public ITimeSpan GetTimeHorizon()
        {
            TimeStamp startTime = new TimeStamp(Time.ToModifiedJulianDay(_simulationStart));
            TimeStamp endTime   = new TimeStamp(Time.ToModifiedJulianDay(_simulationEnd));
            Oatc.UpwardsComp.Backbone.TimeSpan timeHorizon = new Oatc.UpwardsComp.Backbone.TimeSpan(startTime, endTime);
            return timeHorizon;
        }

        public string GetModelID()
        {
            return "GWModelEngineModelID";
        }

        public int GetInputExchangeItemCount()
        {
            return _inputExchangeItems.Count;
        }

        public OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
        {
            return (OutputExchangeItem) _outputExchangeItems[exchangeItemIndex];
        }

        public string GetModelDescription()
        {
            return "GWModelEngineModelDescription";
        }

        public int GetOutputExchangeItemCount()
        {
            return _outputExchangeItems.Count;
        }

        #endregion

        #region IRunEngine Members

        public void SetValues(string QuantityID, string ElementSetID, Oatc.UpwardsComp.Standard.IValueSet values)
        {
            for (int i = 0; i < _storage.Length; i++)
            {
                _storage[i] = ((IScalarSet) values).GetScalar(i);
            }
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
            double t = Time.ToModifiedJulianDay(_simulationStart);
            t += _currentTimeStepNumber * _timeStepLength / (24.0*3600.0);
            Oatc.UpwardsComp.Backbone.TimeSpan currentTime = new Oatc.UpwardsComp.Backbone.TimeSpan(new TimeStamp(t - _timeStepLength / (24.0 * 3600.0)), new TimeStamp(t));
            return currentTime;
        }

        public Oatc.UpwardsComp.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            return new ScalarSet(_storage);
        }

        public void Dispose()
        {
        }

        public string GetComponentDescription()
        {
            return "GWModelEngineComponentDescription";
        }

        public ITimeStamp GetEarliestNeededTime()
        {
            return ((ITimeSpan)GetCurrentTime()).Start;
        }

        public void Initialize(Hashtable properties)
        {
            double ox = 2.0;
            double oy = 2.0;
            double dx = 4.0;
            double dy = 4.0;

            // -- Populate Input Exchange Items ---
            Element element0 = new Element("element:0");
            element0.AddVertex(new Vertex(ox      ,oy        ,0));
            element0.AddVertex(new Vertex(ox+dx   ,oy        ,0));
            element0.AddVertex(new Vertex(ox+dx   ,oy+dy     ,0));
            element0.AddVertex(new Vertex(ox      ,oy+dy     ,0));

            Element element1 = new Element("element:1");
            element1.AddVertex(new Vertex(ox + dx ,oy        ,0));
            element1.AddVertex(new Vertex(ox+2*dx ,oy        ,0));
            element1.AddVertex(new Vertex(ox+2*dx ,oy+dy     ,0));
            element1.AddVertex(new Vertex(ox+dx   ,oy+dy     ,0));

            Element element2 = new Element("element:2");
            element2.AddVertex(new Vertex(ox      ,oy+dy       ,0));
            element2.AddVertex(new Vertex(ox+dx   ,oy+dy       ,0));
            element2.AddVertex(new Vertex(ox+dx   ,oy+2*dy     ,0));
            element2.AddVertex(new Vertex(ox      ,oy+2*dy     ,0));

            Element element3 = new Element("element:3");
            element3.AddVertex(new Vertex(ox + dx ,oy+dy       ,0));
            element3.AddVertex(new Vertex(ox+2*dx ,oy+dy       ,0));
            element3.AddVertex(new Vertex(ox+2*dx ,oy+2*dy     ,0));
            element3.AddVertex(new Vertex(ox+dx   ,oy+2*dy     ,0));

            ElementSet elementSet = new ElementSet("RegularGrid","RegularGrid",ElementType.Polygon," ");
            elementSet.AddElement(element0);
            elementSet.AddElement(element1);
            elementSet.AddElement(element2);
            elementSet.AddElement(element3);

            Unit storageUnit = new Unit("Storage", 1.0, 0.0, "Storage");
            storageUnit.Dimension = new Dimension();
            Quantity storageQuantity = new Quantity(storageUnit,"Storage","Storage");
            InputExchangeItem inputExchangeItem = new InputExchangeItem();
            inputExchangeItem.ElementSet = elementSet;
            inputExchangeItem.Quantity   = storageQuantity;
            _inputExchangeItems.Add(inputExchangeItem);

            OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
            outputExchangeItem.ElementSet = elementSet;
            outputExchangeItem.Quantity   = storageQuantity;
            _outputExchangeItems.Add(outputExchangeItem);

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

        public ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            TimeStamp inputStart = new TimeStamp(((ITimeSpan)GetCurrentTime()).End.ModifiedJulianDay);
            TimeStamp inputEnd = new TimeStamp(((ITimeSpan) GetCurrentTime()).End.ModifiedJulianDay + _timeStepLength/(24.0 * 3600.0));
            return new Oatc.UpwardsComp.Backbone.TimeSpan(inputStart, inputEnd);
        }

        #endregion
    }
}