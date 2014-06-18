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
using System.Collections.Generic;
using log4net;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper;
using Oatc.UpwardsComp.Backbone;
using Oatc.UpwardsComp.Standard;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;
using ExchangeItem=Oatc.OpenMI.Sdk.Backbone.ExchangeItem;
using ITime=OpenMI.Standard2.TimeSpace.ITime;

namespace Oatc.UpwardsComp.EngineWrapper
{
    public class LinkableEngine1_4<T> : LinkableGetSetEngine where T : class, IEngine, new()
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(LinkableEngine1_4<T>));

        /// <summary>
        /// Reference to the engine. Must be assigned in the derived class
        /// </summary>
        protected T _engineApiAccess;

        /// <summary>
        /// True if the Initialize method was invoked
        /// </summary>
        private bool _initializeWasInvoked;

        /// <summary>
        /// True if the Prepare method was invoked
        /// </summary>
        internal bool _prepareWasInvoked;

        /// <summary>
        /// used when comparing time in the IsLater method (see property TimeEpsilon)
        /// </summary>
        private double _timeEpsilon; // used when comparing time in the IsLater method (see property TimeEpsilon)

        private TimeSet _timeExtend;

        /// <summary>
        /// COnstructor
        /// </summary>
        public LinkableEngine1_4()
        {
            _initializeWasInvoked = false;
            _prepareWasInvoked = false;
            _timeEpsilon = 1.0 / (1000.0 * 3600.0 * 24.0); // 1/1000 second
            _engineApiAccess = new T();
            Id = string.Empty; // not set yet
            Description = "Engine not yet initialized";
        }


        public T Engine
        {
            get { return (_engineApiAccess); }
        }

        /// <summary>
        /// This _timeEpsilon variable is used when comparing the current time in the engine with
        /// the time specified in the parameters for the GetValue method. 
        /// if ( requestedTime > engineTime + _timeEpsilon) then PerformTimestep()..
        /// The default values for _timeEpsilon is 1/1000 second
        /// </summary>
        // TODO: Utilize or remove!
        public double TimeEpsilon
        {
            get { return _timeEpsilon; }
            set { _timeEpsilon = value; }
        }

        internal IEngine EngineApiAccess
        {
            get { return _engineApiAccess; }
        }

        protected override string[] OnValidate()
        {
            if (!_initializeWasInvoked)
            {
                throw new Exception(
                  "Validate method in the LinkableEngine cannot be invoked before the Initialize method has been invoked");
            }
            return new string[0]; // TODO: implement validation.
        }

        public override void Initialize()
        {
            Status = LinkableComponentStatus.Initializing;

            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < Arguments.Count; i++)
            {
                hashtable.Add(Arguments[i].Caption, Arguments[i].Value);
            }

            if (_engineApiAccess == null)
            {
                throw new Exception("Failed to assign the engine");
            }

            _engineApiAccess.Initialize(hashtable);

            //foreach (InputItem inputItem in inputItems)
            //{
            //    TimeSet timeset = new TimeSet();
            //    timeset.Times.Add(TimeHelper.ConvertTime(_engineApiAccess.GetCurrentTime()));
            //    inputItem.TimeSet = timeset;
            //    // TODO inputItem.TimeSet = TimeSet for input item;
            //}
            //foreach (OutputItem outputItem in outputItems)
            //{
            //    TimeSet timeset = new TimeSet();
            //    timeset.Times.Add(TimeHelper.ConvertTime(_engineApiAccess.GetCurrentTime()));
            //    outputItem.TimeSet = timeset;
            //}

            Id = _engineApiAccess.GetModelID();
            Description = _engineApiAccess.GetModelDescription();

            for (int i = 0; i < _engineApiAccess.GetOutputExchangeItemCount(); i++)
            {
                OutputExchangeItem output = _engineApiAccess.GetOutputExchangeItem(i);
                IQuantity quantity = output.Quantity;
                IElementSet elementSet = output.ElementSet();
                String outputItemId = string.IsNullOrEmpty(output.Id) ? elementSet.Caption + ":" + quantity.Caption : output.Id;
                EngineOutputItem outputItem = new EngineEOutputItem(outputItemId, quantity, elementSet, this);
                outputItem.TimeSet.SetSingleTime(TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon().Start));
                outputItem.TimeSet.SetTimeHorizon(TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon()));
                EngineOutputItems.Add(outputItem);
            }

            for (int i = 0; i < _engineApiAccess.GetInputExchangeItemCount(); i++)
            {
                InputExchangeItem input = _engineApiAccess.GetInputExchangeItem(i);
                IQuantity quantity = input.Quantity;
                IElementSet elementSet = input.ElementSet();
                String inputItemId = string.IsNullOrEmpty(input.Id) ? elementSet.Caption + ":" + quantity.Caption : input.Id;
                EngineInputItem inputItem = new EngineEInputItem(inputItemId, quantity, elementSet, this);
                inputItem.TimeSet.SetSingleTime(TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon().Start));
                inputItem.TimeSet.SetTimeHorizon(TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon()));
                EngineInputItems.Add(inputItem);
            }


            Status = LinkableComponentStatus.Initialized;
            _initializeWasInvoked = true;
        }

        protected override void OnPrepare()
        {
        }

        public void Dispose()
        {
            _engineApiAccess.Dispose();
        }

        public override void Finish()
        {
            _engineApiAccess.Finish();
        }

        ///// <summary>
        ///// The linkable component that gets the update call should be the only linkable component in a composition that
        ///// has no output items. For that reason the composition will be able to automatically detect which model is the 
        ///// startup model. If there are more models without output items in a composition the composition is invalid.
        ///// </summary>
        ///// <param name="requiredOutputItems">Output Items to do the update for</param>
        //public LinkableComponentStatus OnUpdate(IOutput[] requiredOutputItems)
        //{
        //    if (Status == LinkableComponentStatus.Updating)
        //    {
        //        // Update call that was invoked by bidirectional link
        //        return Status;
        //    }

        //    Status = LinkableComponentStatus.Updating;

        //    foreach (IInput inputItem in Inputs)
        //    {
        //        if (inputItem.Provider != null)
        //        {
        //            // TODO inputItem.TimeSet = TimeSet for input item; get from engine
        //            while (!inputItem.Provider.IsAvailable)
        //            {
        //                inputItem.Provider.Component.Update();
        //            }
        //            // get and store input
        //            IList providedValues = inputItem.Provider.Values;
        //            LogIncomingValues(inputItem, providedValues);
        //            inputItem.Values = providedValues;
        //        }
        //    }

        //    // compute output / store in output / increment time stored in output
        //    foreach (OutputItem outputItem in outputItems)
        //    {
        //        // TODO get values from Engine
        //        // TODO outputItem.TimeSet = TimeSet for output item;
        //    }

        //    // check if more time stamps need to be done
        //    _currentTime = ((ITimeStamp)_engineApiAccess.GetCurrentTime()).ModifiedJulianDay;
        //    if (_currentTime >= _engineApiAccess.GetTimeHorizon().End.ModifiedJulianDay)
        //    {
        //        return LinkableComponentStatus.Done;
        //    }
        //    return LinkableComponentStatus.Updated;
        //}

        protected void LogIncomingValues(EngineInputItem item, IList values)
        {
            string message = Caption + " " + item.Caption + " values:";
            foreach (double value in values)
            {
                message += " " + value;
            }
            message += " <= " + item.Provider.Caption + " from " + item.Provider.Component.Caption +
                       ")";
            log.Info(message);
        }

        public override bool DefaultForStoringValuesInExchangeItem
        {
            get { return false; }
        }

        protected override ITime StartTime
        {
            get { return (TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon().Start)); }
        }

        protected override ITime EndTime
        {
            get { return (TimeHelper.ConvertTime(_engineApiAccess.GetTimeHorizon().End)); }
        }

        public override ITime GetCurrentTime(bool asStamp)
        {
            if (asStamp)
                return CurrentTime;
            throw new NotSupportedException();
        }

        public override ITime CurrentTime
        {
            get
            {
                ITime time;
                Oatc.UpwardsComp.Standard.ITime time1_4 = _engineApiAccess.GetCurrentTime();
                // A 1.4 component may return a time interval where the start time is the current time
                if (time1_4 is ITimeSpan)
                    time = TimeHelper.ConvertTime(((Oatc.UpwardsComp.Standard.ITimeSpan)time1_4).Start);
                else
                    time = TimeHelper.ConvertTime(time1_4);
                return (time);
            }
        }

        public override ITime GetInputTime(bool asStamp)
        {
            if (!asStamp)
                throw new NotSupportedException();
            ITime time;
            Oatc.UpwardsComp.Standard.ITime time1_4 = _engineApiAccess.GetCurrentTime();
            // A 1.4 component may return a time interval where the start time is the current time
            if (time1_4 is Oatc.UpwardsComp.Standard.ITimeSpan)
                time = TimeHelper.ConvertTime(((Oatc.UpwardsComp.Standard.ITimeSpan)time1_4).End);
            else
                time = TimeHelper.ConvertTime(time1_4);
            return (time);
        }

        public override ITimeSpaceValueSet GetEngineValues(ExchangeItem exchangeItem)
        {
            IScalarSet scalarSet = (IScalarSet)_engineApiAccess.GetValues(exchangeItem.ValueDefinition.Caption, exchangeItem.SpatialDefinition.Caption);
            List<double> values = new List<double>(scalarSet.Count);
            for (int i = 0; i < scalarSet.Count; i++)
            {
                values.Add(scalarSet.GetScalar(i));
            }
            return (new ValueSet(new List<IList>{values}));
        }

        public override void SetEngineValues(EngineInputItem inputItem, ITimeSpaceValueSet values)
        {
            int elementCount = ValueSet.GetElementCount(values);
            double[] avalues = new double[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                avalues[i] = (double)values.GetValue(0,i);
            }
            ScalarSet scalarSet = new ScalarSet(avalues);
            _engineApiAccess.SetValues(inputItem.ValueDefinition.Caption,inputItem.SpatialDefinition.Caption,scalarSet);
        }

        protected override void PerformTimestep(ICollection<EngineOutputItem> requiredOutputItems)
        {
            _engineApiAccess.PerformTimeStep();

            //ITime time = CurrentTime;

            //foreach (EngineOutputItem engineOutputItem in requiredOutputItems)
            //{
            //    engineOutputItem.SetSingleTime(time);
            //}
        }

   }
}