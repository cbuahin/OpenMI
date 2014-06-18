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
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.UpwardsComp.Standard;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace UpwardsComp.Backbone
{
	/// <summary>
	/// The link is used to describe the data transfer between
	/// linkable components.
    /// <para>This is a trivial implementation of OpenMI.Standard.ILink, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class Link : ILink
	{	
		private ArrayList _dataOperations = new ArrayList();
		private string _description = "";
		private string _id = "";

	  private ITimeSpaceInput _target;
	  private ITimeSpaceOutput _source;


		/// <summary>
		/// Constructor
		/// </summary>
		public Link()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">The link to copy</param>
		public Link(ILink source)
		{
			Source = source.Source;
			Target = source.Target;
			Description = source.Description;
			ID = source.ID;
			for (int i=0;i<source.DataOperationsCount;i++)
				AddDataOperation(source.GetDataOperation(i));
		}

	  public Link(ITimeSpaceOutput output, ITimeSpaceInput input, string linkId)
	  {
	    _source = output;
	    _target = input;
	    _id = linkId;
	  }


	  /// <summary>
		/// Getter and setter for the link ID
		/// </summary>
		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Getter and setter for the link description
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Getter and setter for the source component
		/// </summary>
		public IBaseLinkableComponent SourceComponent
		{
			get
			{
				return _source.Component;
			}
		}

		/// <summary>
		/// Getter and setter for the source quantity
		/// </summary>
		public IQuantity SourceQuantity
		{
			get
			{
				return (IQuantity)_source.ValueDefinition;
			}
		}

		/// <summary>
		/// Getter and setter for the source element set
		/// </summary>
		public IElementSet SourceElementSet
		{
			get
			{
				return _source.ElementSet();
			}
		}

		/// <summary>
		/// Getter and setter for the target component
		/// </summary>
		public IBaseLinkableComponent TargetComponent
		{
			get
			{
				return _target.Component;
			}
		}

	  public ITimeSpaceOutput Source
	  {
	    get { return(_source); }
      set { _source = value;}
	  }

    public ITimeSpaceInput Target
    {
      get { return (_target); }
      set { _target = value; }
    }

	  /// <summary>
		/// Getter and setter for the target quantity
		/// </summary>
		public IQuantity TargetQuantity
		{
			get
			{
				return (IQuantity)_target.ValueDefinition;
			}
		}

		/// <summary>
		/// Getter and setter for the target element set
		/// </summary>
		public IElementSet TargetElementSet
		{
			get
			{
				return _target.ElementSet();
			}
		}

		/// <summary>
		/// The number of data operations
		/// </summary>
		public int DataOperationsCount
		{
			get
			{
				return _dataOperations.Count;
			}
		}

		/// <summary>
		/// Adds a data operation
		/// </summary>
		/// <param name="dataOperation">The data operation</param>
		public void AddDataOperation (IDataOperation dataOperation)
		{
			if ( ! (dataOperation is ICloneable ) )
			{
				// Data Operation can not be cloned, issue warning
        // TODO: how to do this
			  throw new System.NotImplementedException();
        //Event warning = new Event(EventType.Warning);
        //warning.Description = "DataOperation " + dataOperation.ID + " can not be cloned yet!";
        //warning.Sender = _sourceComponent;
        //_sourceComponent.SendEvent(warning);

				_dataOperations.Add(dataOperation);
			}
			else
			{
				_dataOperations.Add(((ICloneable)dataOperation).Clone());
			}
		}


		/// <summary>
		/// Gets a data operation
		/// </summary>
		/// <param name="DataOperationIndex">The index of the data operation</param>
		/// <returns>The data operation</returns>
		public IDataOperation GetDataOperation(int DataOperationIndex)
		{
			return (IDataOperation) _dataOperations[DataOperationIndex];
		}

	  ///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Link link = (Link) obj;
			return (ID.Equals(link.ID));
		}

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_source != null) hashCode += _source.GetHashCode();
			if (_target != null) hashCode += _target.GetHashCode();
			return hashCode;
		}
	}
}
 
