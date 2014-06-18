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
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Wrapper
{
    public abstract class LinkableEngine : LinkableComponent
    {
        protected List<IInputItem> inputItems = new List<IInputItem>();
        protected List<IOutputItem> outputItems = new List<IOutputItem>();

        /// <summary>
        /// Reference to the engine. Must be assigned in the derived class
        /// </summary>
        protected IEngine engineApiAccess;

        /// <summary>
        /// True if the Initialize method was invoked
        /// </summary>
        private bool initializeWasInvoked;

        /// <summary>
        /// True if the Prepare method was invoked
        /// </summary>
        internal bool prepareWasInvoked;

        protected LinkableEngine()
        {
            initializeWasInvoked = false;
            prepareWasInvoked = false;

            TimeEpsilon = 1.0 / (1000.0 * 3600.0 * 24.0); // 1/1000 second

           decoratorFactory = new DefaultDecoratorFactory();

        }

        private double _timeEpsilon;

        /// <summary>
        /// This timeEpsilon variable is used when comparing the current time in the engine with
        /// the time specified in the parameters for the GetValue method. 
        /// if ( requestedTime > engineTime + timeEpsilon) then PerformTimestep()..
        /// The default values for timeEpsilon is double.Epsilon = 4.94065645841247E-324
        /// The default value may be too small for some engines, in which case the timeEpsilon can
        /// be changed the class that you have inherited from LinkableRunEngine og LinkableEngine.
        /// </summary>
        public double TimeEpsilon
        {
          get { return _timeEpsilon; }
          set { _timeEpsilon = value; }
        }

        /// <summary>
        /// Set reference to the engine
        /// </summary>
        protected abstract void SetEngineApiAccess();

        public override void Initialize(IArgument[] arguments)
        {
            Status = LinkableComponentStatus.Initializing;

            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < arguments.Length; i++)
            {
                hashtable.Add(arguments[i].Caption, arguments[i].Value);
            }

            SetEngineApiAccess();
            if (engineApiAccess == null)
            {
                throw new Exception("Failed to assign the engine");
            }

            engineApiAccess.Initialize(hashtable);

            for (int i = 0; i < engineApiAccess.GetInputExchangeItemCount(); i++)
            {
                inputItems.Add(engineApiAccess.GetInputExchangeItem(i));
            }

            for (int i = 0; i < engineApiAccess.GetOutputExchangeItemCount(); i++)
            {
                outputItems.Add(engineApiAccess.GetOutputExchangeItem(i));
            }

            initializeWasInvoked = true;

            Status = LinkableComponentStatus.Initialized;
        }

        public override IList<IInputItem> InputItems
        {
            get { return inputItems; }
        }

        public override IList<IOutputItem> OutputItems
        {
            get { return outputItems; }
        }

        public override string[] Validate()
        {
            if (!initializeWasInvoked)
            {
                throw new Exception(
                    "Validate method in the LinkableEngine cannot be invoked before the Initialize method has been invoked");
            }

            return new string[0]; // TODO: implement validation.
        }

        protected virtual void OnFirstGetValuesCall()
        {
            // Remove ouput items without consumers since these are not used
            for (int i = outputItems.Count - 1; i >= 0; i--)
            {
                if (outputItems[i].Consumers.Count == 0)
                {
                    outputItems.RemoveAt(i);
                }
            }

            // Remove input items without providers since these are not used.
            // TODO: how to distingish input items for which the values will be set.
            for (int i = inputItems.Count - 1; i >= 0; i--)
            {
                if (inputItems[i].Provider == null)
                {
                    inputItems.RemoveAt(i);
                }
            }
        }

        public virtual void Dispose()
        {
            engineApiAccess.Dispose();
        }

        public override void Finish()
        {
            engineApiAccess.Finish();
        }

        public override void Update(params IOutputItem[] requiredOutputItems)
        {
            Status = LinkableComponentStatus.Updating;
            ITime targetTime = null; // todo: determine last required time stamp from consumers
            computeUntil(targetTime);
            Status = LinkableComponentStatus.Updated;
        }

        /// <summary>
        /// The linkable component that gets the update call should be the only linkable component in a composition that
        /// has no output items. For that reason the composition will be able to automatically detect which model is the 
        /// startup model. If there are more models without output items in a composition the composition is invalid.
        /// </summary>
        /// <param name="targetTime"></param>
        private void computeUntil(ITime targetTime)
        {
            // Retrieve all input

            ITime time = engineApiAccess.GetCurrentTime();

            while (LastBufferedOutputTime == null || IsLater(time, LastBufferedOutputTime))
            {

                //            while (currentTime.StampAsModifiedJulianDay < targetTime.StampAsModifiedJulianDay)
                //            {
                foreach (InputItem inputItem in inputItems)
                {
                    if (inputItem.Provider != null)
                    {
                        double[] inputValues = inputItem.Values as double[];
                        engineApiAccess.SetValues(inputItem.ValueDefinition.Caption, inputItem.ElementSet.Caption, inputValues);
                    }
                }

                engineApiAccess.PerformTimeStep();

                time = engineApiAccess.GetCurrentTime();

                // Produce all output
                foreach (IOutputItem outputItem in outputItems)
                {
                    if (outputItem.Consumers.Count > 0)
                    {
                        if (outputItem is BufferedOutputItem)
                        {
                            double[] outputValues =
                                engineApiAccess.GetValues(outputItem.ValueDefinition.Caption, outputItem.ElementSet.Caption);
                            ((BufferedOutputItem)outputItem).SetValues(targetTime, outputValues);
                        }
                    }
                }
            }
        }

        public ITime LastBufferedOutputTime
        {
            get
            {
                ITime time = null;
                foreach (IOutputItem outputItem in outputItems)
                {
                    if (outputItem is BufferedOutputItem)
                    {
                        ITime bufferTime = ((BufferedOutputItem) outputItem).LastBufferedTime;
                        if ( (time == null) || (bufferTime != null && !IsLater(bufferTime, time)))
                        {
                            time = bufferTime;
                        }
                    }
                }

                return time;
            }
        }

        /// <summary>
        /// Will compare two times. If the first argument t1, is later than the second argument t2
        /// the method will return true. Otherwise false will be returned. t1 and t2 can be of types
        /// ITimeSpan or ITimeStamp.
        /// </summary>
        /// <param name="t1">First time</param>
        /// <param name="t2">Second time</param>
        /// <returns>isLater</returns>
        protected static bool IsLater(ITime t1, ITime t2)
        {
            return t1.StampAsModifiedJulianDay > t2.StampAsModifiedJulianDay;
        }
    }

    internal class DefaultDecoratorFactory : IExchangeItemDecoratorFactory
    {
        string id = "Oatc.DefaultDecoratorFactory";
        string caption = "Oatc.DefaultDecoratorFactory";
        string description = "Default decorator factory, provides time decorators, SI-converion and Element Mapping";

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public IIdentifiable[] GetAvailableOutputDecorators(IOutputItem decoratedItem, IInputItem targetItem)
        {
            throw new NotImplementedException();
        }

        public IOutputItemDecorator CreateOutputItemDecorator(IIdentifiable decoratorIdentifier, IOutputItem decoratedItem, IInputItem targetItem)
        {
            throw new NotImplementedException();
        }

        public IDescribable GetDecoratorDescription(IIdentifiable id)
        {
            throw new NotImplementedException();
        }
    }
}