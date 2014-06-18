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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary>
    /// The exchange item is a combination of a quantity/quality and an element set.
    /// <para>This is a trivial implementation of OpenMI.Standard.IExchangeItem, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public abstract class ExchangeItem : ITimeSpaceExchangeItem
    {
        #region Private/protected fields

        // identification / description
        protected readonly string _id;
        protected string _caption = string.Empty;
        protected string _description = string.Empty;

        // exchange item definition
        protected IValueDefinition _valueDefinition;
        protected ISpatialDefinition _spatialDefinition;
        protected ITimeSet _timeSet;

        // component
        protected ITimeSpaceComponent _component;

        #endregion

        #region Constructors

        protected ExchangeItem(string id)
        {
            _id = _caption = _description = id;
        }

        protected ExchangeItem(string id, IValueDefinition valueDefinition, ISpatialDefinition spatialDefinition)
          : this(id)
        {
            _valueDefinition = valueDefinition;
            _spatialDefinition = spatialDefinition;
        }

        #endregion

        #region IExchangeItem Members

        /// <summary>
        /// Getter and setter for the value definition (quantity/quality)
        /// See <see cref="ITimeSpaceExchangeItem.ValueDefinition">IExchangeItem.ValueDefinition</see>.
        /// </summary>
        public virtual IValueDefinition ValueDefinition
        {
            get { return _valueDefinition; }
            set { _valueDefinition = value; }
        }

        /// <summary>
        /// Getter for the spatial axis.
        /// </summary>
        public virtual ISpatialDefinition SpatialDefinition
        {
            get { return _spatialDefinition; }
            set { _spatialDefinition = value; }
        }

        public virtual IBaseLinkableComponent Component
        {
            get { return _component; }
            set {
                if (value == null)
                  _component = null;
                else
                {
                    _component = value as ITimeSpaceComponent;
                    if (_component == null)
                    {
                       throw new ArgumentException("Component must be an ITimeSpaceComponent");
                    }
                }

            }
        }

        ITimeSet ITimeSpaceExchangeItem.TimeSet
        {
          get { return TimeSet; }
        }

        public virtual ITimeSet TimeSet
        {
            get { return _timeSet; }
            set { _timeSet = value; } // TODO: update number of values
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Id
        {
            get { return _id; }
        }

        // TODO: Event must be triggede when item is changed. Not implemented!
        public event EventHandler<ExchangeItemChangeEventArgs> ItemChanged;

        #endregion

        public override string ToString()
        {
            return _id;
        }


        #region Public Convenience Functions

        protected int ComputeIndex(int timeIndex, int elementIndex)
        {
            if (SpatialDefinition != null && elementIndex >= 0 && elementIndex >= SpatialDefinition.ElementCount)
            {
                throw new ArgumentException("elementIndex (=" + elementIndex + ") >= #elements (=" +
                                            _spatialDefinition.ElementCount);
            }
            if (TimeSet != null && timeIndex >= 0 && timeIndex >= TimeSet.Times.Count)
            {
                throw new ArgumentException("timeIndex (=" + timeIndex + ") >= #times (=" +
                                            TimeSet.Times.Count);
            }
            if (timeIndex >= 0)
            {
                if (TimeSet != null)
                {
                    if (_spatialDefinition != null)
                    {
                        int elementCount = _spatialDefinition.ElementCount;
                        if (elementIndex >= 0 && elementIndex < elementCount)
                        {
                            return timeIndex*elementCount + elementIndex;
                        }
                        throw new ArgumentException("elementIndex (=" + elementIndex + ") >= #elements (=" +
                                                    _spatialDefinition.ElementCount);
                    }
                    return timeIndex;
                }
                throw new ArgumentException("timeIndex >= 0, while there is not TimeSet available");
            }
            if (elementIndex >= 0)
            {
                if (elementIndex >= _spatialDefinition.ElementCount)
                {
                    throw new ArgumentException("elementIndex (=" + elementIndex + ") >= #elements (=" +
                                                _spatialDefinition.ElementCount);
                }
                return elementIndex;
            }
            throw new ArgumentException("invalid time(Start|End)index, element(Start|End)index combination");
        }

        #endregion

    }
}