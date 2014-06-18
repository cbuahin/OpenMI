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
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary> 
    /// The exchange item is a combination of a quantity/quality and an element set.
    /// <para>This is a trivial implementation of OpenMI.Standard.IExchangeItem, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class Output : ExchangeItem, ITimeSpaceOutput
    {
        protected List<ITimeSpaceInput> _consumers;
        protected ITimeSpaceValueSet _values;
        protected List<IBaseAdaptedOutput> _adaptedOutputs;

        public Output(string id)
            : base(id)
        {
        }

        public Output(string id, IValueDefinition valueDefinition, IElementSet elementSet) :
            base(id, valueDefinition, elementSet)
        {
        }

        public virtual IList<IBaseInput> Consumers
        {
            get
            {
                if (_consumers == null)
                {
                    _consumers = new List<ITimeSpaceInput>();
                }
                return new ListWrapper<ITimeSpaceInput, IBaseInput>(_consumers.AsReadOnly());
            }
        }

        public virtual void AddConsumer(IBaseInput consumer)
        {
            ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
            if (timeSpaceConsumer == null)
                throw new ArgumentException("Must be a ITimeSpaceInput - may need to add adaptor");

            // Create list of consumers
            if (_consumers == null)
            {
                _consumers = new List<ITimeSpaceInput>();
            }

            // consumer should not be already added
            if (_consumers.Contains(timeSpaceConsumer))
            {
                throw new Exception("consumer \"" + consumer.Caption +
                    "\" has already been added to \"" + Caption);
            }

            _consumers.Add(timeSpaceConsumer);


            if (consumer.Provider != this)
            {
                consumer.Provider = this;
            }
            //// TODO (JG): EXPERIMENTAL CODE
            //if (consumer is IBaseMultiInput)
            //{
            //    ((IBaseMultiInput)consumer).AddProvider(this);
            //}
            //else
            //{
            //    if (consumer.Provider != this)
            //    {
            //        consumer.Provider = this;
            //    }
            //}
        }

        public virtual void RemoveConsumer(IBaseInput consumer)
        {
            ITimeSpaceInput timeSpaceConsumer = consumer as ITimeSpaceInput;
            if (timeSpaceConsumer == null || _consumers == null || !_consumers.Contains(timeSpaceConsumer))
            {
                throw new Exception("consumer \"" + consumer.Caption +
                    "\" can not be removed from \"" + Caption +
                    "\", because it was not added");
            }
            _consumers.Remove(timeSpaceConsumer);
            consumer.Provider = null;
        }

        public IList<IBaseAdaptedOutput> AdaptedOutputs
        {
            get
            {
                if (_adaptedOutputs == null)
                {
                    _adaptedOutputs = new List<IBaseAdaptedOutput>();
                }
                return _adaptedOutputs.AsReadOnly();
            }
        }

        public virtual void AddAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            if (_adaptedOutputs == null)
            {
                _adaptedOutputs = new List<IBaseAdaptedOutput>();
            }
            _adaptedOutputs.Add(adaptedOutput);
        }

        public virtual void RemoveAdaptedOutput(IBaseAdaptedOutput adaptedOutput)
        {
            if (_adaptedOutputs == null)
            {
                return;
            }
            _adaptedOutputs.Remove(adaptedOutput);
        }

        IBaseValueSet IBaseOutput.Values
        {
            get { return (Values); }
        }

        IBaseValueSet IBaseOutput.GetValues(IBaseExchangeItem querySpecifier)
        {
            return GetValues(querySpecifier);
        }

        public virtual ITimeSpaceValueSet Values
        {
            get
            {
                if (_values == null)
                {
                    throw new Exception("No values set");
                }
                return _values;  // TODO: How to export "AsReadOnly()" ???
            }
            set { _values = value; }
        }

        public virtual ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
            ITimeSpaceExchangeItem timeSpaceQuery = querySpecifier as ITimeSpaceExchangeItem;
            if (timeSpaceQuery == null)
            {
                throw new ArgumentException("Must be an ITimeSpaceExchangeItem - an adaptor may be required");
            }
            if (!ExchangeItemHelper.OutputAndInputElementSetsFit(this, timeSpaceQuery))
            {
                throw new Exception("ElementSet of output item \"" + Id +
                                    "\" does not fit the ElementSet of requesting item \"" + querySpecifier.Id);
            }

            if (!ExchangeItemHelper.OutputAndInputTimeSetsFit(this, timeSpaceQuery))
            {
                throw new Exception("TimeSet of output item \"" + Id +
                                    "\" does not fit the TimeSet of requesting item \"" + querySpecifier.Id);
            }

            return Values;
        }
    }
}