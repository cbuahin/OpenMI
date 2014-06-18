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
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{

	/// <summary>
	/// Dummy exchange item used by trigger's link.
	/// </summary>
	public class TriggerExchangeItem: Input
	{
	    /// <summary>
		/// Creates new instance of <see cref="TriggerExchangeItem">TriggerExchangeItem</see>
		/// </summary>
		public TriggerExchangeItem(IBaseLinkableComponent trigger) :
            base("Trigger")
		{
            Caption = "Trigger";
            ValueDefinition = new Quantity("Trigger");
            SpatialDefinition = new ElementSet("Trigger");
            Component = trigger;
		}
	}


	/// <summary>
	/// Linkable component which can hold only one input link. It's used to fire the simulation.
	/// </summary>
	public class Trigger : IBaseLinkableComponent
	{
        // Can only be one in composition, so can make static and use before instantiated
        static string _id = "9483873B-DD83-4591-BD00-D5A4725A180B";

	    readonly ITime _timeHorizon;
	    readonly Time _earliestInputTime;
        private readonly List<IBaseInput> inputItems = new List<IBaseInput>();

	    /// <summary>
		/// Creates a new instance of <see cref="Trigger">Trigger</see> class.
		/// </summary>
		public Trigger()
		{
            inputItems.Add(new TriggerExchangeItem(this));

            _timeHorizon = new Time(new DateTime(1800, 1, 1), new DateTime(2200, 1, 1));
            _earliestInputTime = new Time(_timeHorizon.StampAsModifiedJulianDay + _timeHorizon.DurationInDays);
		}

		/// <summary>
		/// Default implementation.
		/// </summary>
		public void Dispose()
		{			
		}

	    public ITimeSpaceAdaptedOutput CreateOutputDecorator(ITimeSpaceOutput sourceItem, ITimeSpaceInput target)
	    {
	        throw new NotImplementedException();
	    }

	    public ITimeSet TimeExtent
	    {
            get { return null; }
	    }

    public void Initialize()
    {
    }

	    /// <summary>
		/// Default implementation.
		/// </summary>
		public void Prepare()
		{			
		}

	    public void Update(ITime timeStamp)
	    {
	        throw new NotImplementedException();
	    }

		/// <summary>
		/// Gets description of trigger.
		/// </summary>
		public string ComponentDescription
		{
			get
			{
				return( "Component implementing trigger model." );
			}
		}

	    public IList<IArgument> Arguments
	    {
	        get { throw new System.NotImplementedException(); }
	    }

	    public LinkableComponentStatus Status
	    {
	        get { throw new System.NotImplementedException(); }
	    }

	    public IList<IBaseInput> Inputs
	    {
	        get { return inputItems; }
	    }

        public IList<IBaseOutput> Outputs
	    {
            get { return new List<IBaseOutput>(); }
	    }

	    public event EventHandler<LinkableComponentStatusChangeEventArgs> StatusChanged;

	    /// <summary>
		/// Preforms validation of the <see cref="Trigger">Trigger</see> model.
		/// </summary>
		/// <returns>Returns empty string.</returns>
		public string[] Validate()
		{
	        return new string[]{};
		}

        void IBaseLinkableComponent.Update(params IBaseOutput[] requiredOutput)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
		/// Default implementation.
		/// </summary>
		public void Finish()
		{			
		}

	    public List<IAdaptedOutputFactory> AdaptedOutputFactories
	    {
            get
            {
                throw new NotImplementedException();
            }
	    }

        public bool CascadingUpdateCallsDisabled { get; set; }

        /// <summary>
		/// Gets earliest time when next input is needed, typically the trigger invoke time.
		/// </summary>
		public ITime EarliestInputTime
		{
			get
			{
				return( _earliestInputTime );
			}
		}

	    public string Caption
	    {
	        get { return "Trigger"; }
	        set { throw new System.NotImplementedException(); }
	    }

	    public string Description
	    {
	        get { return "Used to specify which output exchange item controls the pull deriven solution"; }
	        set { throw new System.NotImplementedException(); }
	    }

	    public string Id
	    {
            get { return _id; }
	    }

        public static string ID
        {
            get { return _id; }
        }

        #region ILinkableComponent Members


        string[] IBaseLinkableComponent.Validate()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

