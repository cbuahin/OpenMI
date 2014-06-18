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
using OpenMI.Standard;
using System.Collections;
using Oatc.OpenMI.Sdk.Backbone;

namespace org.OpenMI.Utilities.AdvancedControl
{
	/// <summary>
	/// The IterationController controls iterations between
	/// linkable components
	/// </summary>
	public class IterationController : Controller
	{
		public override void Prepare()
		{
		}

		Buffer buffer = new Buffer();

		double relaxation = 0.25;
		int maxIter = 25;
		double eps=1e-6;

		public override string ComponentDescription
		{
			get
			{
				return "Iteration Controller";
			}
		}

		public override string ComponentID
		{
			get
			{
				return "Iteration Controller";
			}
		}
		public override string ModelID
		{
			get
			{
				return "Iteration Controller";
			}
		}
		public override string ModelDescription
		{
			get
			{
				return "Iteration Controller";
			}
		}

		public override ITimeStamp EarliestInputTime
		{
			get
			{
				return new TimeStamp(0.0);
			}
		}

		public override ITimeSpan TimeHorizon
		{
			get
			{
				return new Oatc.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(0.0),new TimeStamp(1e6));
			}
		}

		public override void Finish()
		{
		}

		public override EventType GetPublishedEventType(int providedEventTypeIndex)
		{
			return new EventType ();
		}

		public override int GetPublishedEventTypeCount()
		{
			return 0;
		}

		public override string Validate()
		{
			return "";
		}

		public override void Initialize(IArgument[] properties)
		{
			for (int i=0;i<20;i++) 
			{
				string ID = (i+1).ToString();
				Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Quantity","Quantity");
				ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
					new SpatialReference());
				Element element = new Element(ID);
				elementSet.AddElement(element);
				InputExchangeItem exchangeItem = new InputExchangeItem();
				exchangeItem.ElementSet = elementSet;
				exchangeItem.Quantity = quantity;
				AddInputExchangeItem(exchangeItem);
			}

			for (int i=0;i<20;i++) 
			{
				string ID = (i+1).ToString();
				Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Quantity","Quantity");
				ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
					new SpatialReference());
				Element element = new Element(ID);
				elementSet.AddElement(element);
				OutputExchangeItem exchangeItem = new OutputExchangeItem();
				exchangeItem.ElementSet = elementSet;
				exchangeItem.Quantity = quantity;
				AddOutputExchangeItem(exchangeItem);
			}
		}

		public override IValueSet GetValues(ITime time, string LinkID)
		{
			if (!isComputing) 
			{
				isComputing = true;

				KeepAllStates();

				int iterCount;

				for (iterCount=0;iterCount<maxIter;iterCount++) 
				{
					RestoreAllStates();

					ILink[] links = GetAcceptingLinks();

					double change=0.0;

					for (int i=0;i<links.Length;i++) 
					{
						ILink link = links[i];
						IValueSet valueSet = link.SourceComponent.GetValues(time,link.ID);
						string targetID = link.TargetElementSet.ID;
						if (valueSet!=null&&valueSet.Count>0) 
						{
							IValueSet bufferSet = buffer.Get(targetID);
							if (bufferSet==null) 
							{
								buffer.Add(targetID,valueSet);
								change = 10;
							}
							else 
							{
								double[] values = new double[bufferSet.Count];
								for (int j=0;j<bufferSet.Count;j++)
								{
									values[j] = ((IScalarSet)bufferSet).GetScalar(j);
								}
								double[] newValues = new double[valueSet.Count];
								for (int j=0;j<valueSet.Count;j++) 
								{
									newValues[j] = ((IScalarSet)valueSet).GetScalar(j);
								}
								for (int j=0;j<values.Length;j++) 
								{
									double diff = values[j]-newValues[j];
									change += diff*diff;
									values[j] = relaxation*values[j]+(1-relaxation)*newValues[j];
								}
								buffer.Add(targetID,new ScalarSet(values));
							}
						}
					}

					if (change<eps)
						break;
				}

				isComputing = false;
			} 

			ILink outputLink = GetLink(LinkID);
			return buffer.Get(outputLink.SourceElementSet.ID);
		}
	}

	class BufferElement
	{
		string _ID;
		IValueSet _valueSet;

		public BufferElement(string ID, IValueSet valueSet)
		{
			_ID = ID;
			_valueSet = valueSet;
		}

		public string ID
		{
			get 
			{
				return _ID;
			}
			set 
			{
				_ID = value;
			}
		}

		public IValueSet ValueSet
		{
			get
			{
				return _valueSet;
			}
			set
			{
				_valueSet = value;
			}
		}
	}

	public class Buffer
	{
		ArrayList list = new ArrayList();

		public void Clear() 
		{
			list.Clear();
		}

		public IValueSet Get (string ID)
		{
			foreach (BufferElement element in list) 
			{
				if (element.ID.Equals(ID))
					return element.ValueSet;
			}
			return null;
		}

		public void Add (string ID, IValueSet valueSet)
		{
			foreach (BufferElement element in list) 
			{
				if (element.ID.Equals(ID)) 
				{
					element.ValueSet =valueSet;
					return;
				}
			}
			list.Add(new BufferElement(ID,valueSet));
			return;
		}
	}
}



