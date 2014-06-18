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
using Oatc.OpenMI.Sdk.Backbone;

namespace org.OpenMI.Utilities.AdvancedControl
{
	/// <summary>
	/// This component returns the sum of squared
	/// differences between two inputs and can be
	/// used for calibrating models
	/// </summary>
	public class SumSquaredDifference:LinkableComponent
	{
		public override void Prepare()
		{
		}

		private double _beginTime;
		private double _endTime;
		private double _timeStep;

		public override string ComponentDescription
		{
			get
			{
				return "Sum of Squared Differences";
			}
		}

		public override string ComponentID
		{
			get
			{
				return "Sum of Squared Differences";
			}
		}
		public override string ModelID
		{
			get
			{
				return "Sum of Squared Differences";
			}
		}
		public override string ModelDescription
		{
			get
			{
				return "Sum of Squared Differences";
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

		public override void Finish()
		{
		}


		public override void Initialize(IArgument[] properties)
		{
			foreach (IArgument argument in properties)
			{
				if (argument.Key.Equals("BeginTime")) 
				{
					_beginTime = Double.Parse(argument.Value);
				}
				if (argument.Key.Equals("EndTime"))
				{
					_endTime = Double.Parse(argument.Value);
				}
				if (argument.Key.Equals("TimeStep"))
				{
					_timeStep = Double.Parse(argument.Value);
				}
			}
		}

		public override IValueSet GetValues(ITime time, string LinkID)
		{
			ILink[] links = GetAcceptingLinks();
			if (links.Length<2)
				throw new Exception("Sum of squared differences should have 2 inputs");

			double _time = _beginTime;
			double sum = 0.0;

			while (_time<_endTime) 
			{
				TimeStamp timeStamp = new TimeStamp(_time);
				IValueSet values = links[0].SourceComponent.GetValues(timeStamp,links[0].ID);
				IValueSet reference = links[1].SourceComponent.GetValues(timeStamp,links[1].ID);

				if (values is ScalarSet && reference is ScalarSet && values.Count==reference.Count) 
				{
					IScalarSet scalarSet = (IScalarSet)values;
					IScalarSet referenceSet = (IScalarSet)reference;

					for (int i=0;i<scalarSet.Count;i++) {
						double diff = scalarSet.GetScalar(i)-referenceSet.GetScalar(i);
						sum += diff*diff;
						}
				}

				_time += _timeStep;
			}
			return new ScalarSet(new double[] {sum});
		}

		public override IInputExchangeItem GetInputExchangeItem(int inputExchangeItemIndex)
		{
			inputExchangeItemIndex++;
			string ID = inputExchangeItemIndex.ToString();
			Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Quantity","Quantity");
			ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
				new SpatialReference());
			Element element = new Element(ID);
			elementSet.AddElement(element);
			InputExchangeItem exchangeItem = new InputExchangeItem();
			exchangeItem.ElementSet = elementSet;
			exchangeItem.Quantity = quantity;
			return exchangeItem;
			
		}

		public override int InputExchangeItemCount
		{
			get
			{
				return 2;
			}
		}

		public override IOutputExchangeItem GetOutputExchangeItem(int outputExchangeItemIndex)
		{
			string ID = "1";
			Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Cost","Cost");
			ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
				new SpatialReference());
			Element element = new Element(ID);
			elementSet.AddElement(element);
			OutputExchangeItem exchangeItem = new OutputExchangeItem();
			exchangeItem.ElementSet = elementSet;
			exchangeItem.Quantity = quantity;
			return exchangeItem;
		}

		public override int OutputExchangeItemCount
		{
			get
			{
				return 1;
			}
		}
	}
}
