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
using System.Diagnostics;
using Oatc.OpenMI.Sdk.Backbone;


namespace org.OpenMI.Utilities.AdvancedControl
{
	/// <summary>
	/// The optimization controller can optimize an
	/// objective function
	/// </summary>
	public class OptimizationController : Controller
	{
		Random random;

		public OptimizationController()
		{
			random = new Random();
		}

		public OptimizationController(int RandomSeed)
		{
			random = new Random(RandomSeed);
		}

		public override void Prepare()
		{
		}

		ArrayList parameters = new ArrayList();
		Buffer buffer = new Buffer();
		int evaluationCount=0;

		public override string ComponentDescription
		{
			get
			{
				return "Optimization Controller";
			}
		}

		public override string ComponentID
		{
			get
			{
				return "Optimization Controller";
			}
		}
		public override string ModelID
		{
			get
			{
				return "Optimization Controller";
			}
		}
		public override string ModelDescription
		{
			get
			{
				return "Optimization Controller";
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


		void AddParameter (ParameterDescriptor parameter)
		{
			parameters.Add(parameter);
		}

		ArrayList GetParameters() 
		{
			return parameters;
		}

		public override void Initialize(IArgument[] properties)
		{
			foreach (IArgument argument in properties)
			{
				if (argument.Key.Equals("Parameter")) 
				{
					string paramstring = argument.Value;
					char[] delimiter = new char[1];
					delimiter[0] = ',';
					string[] substring = paramstring.Split(delimiter);

					string ID = substring[0];
					double minimum = Double.Parse(substring[1]);
					double maximum = Double.Parse(substring[2]);
					double currentValue = Double.Parse(substring[3]);
					ParameterDescriptor descriptor = new ParameterDescriptor(ID,minimum,maximum,currentValue);
					AddParameter(descriptor);
					Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Quantity","Quantity");
					ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
						new SpatialReference());
					OutputExchangeItem exchangeItem = new OutputExchangeItem();
					exchangeItem.ElementSet = elementSet;
					exchangeItem.Quantity = quantity;
					AddOutputExchangeItem(exchangeItem);
				}
			}

			for (int i=0;i<1;i++) 
			{
				string ID = (i+1).ToString();
				Quantity quantity = new Quantity(new Unit("Unit",1.0,0.0,"Unit"),"Cost","Cost");
				ElementSet elementSet = new ElementSet(ID,ID,ElementType.IDBased,
					new SpatialReference());
				Element element = new Element(ID);
				elementSet.AddElement(element);
				InputExchangeItem exchangeItem = new InputExchangeItem();
				exchangeItem.ElementSet = elementSet;
				exchangeItem.Quantity = quantity;
				AddInputExchangeItem(exchangeItem);
			}
		}

		public double EvaluateCostFunction(ITime time,Solution solution)
		{
			evaluationCount++;
			for (int i=0;i<solution._values.Length;i++)
				((ParameterDescriptor)parameters[i]).CurrentValue = 
					solution._values[i];

			foreach (ParameterDescriptor parameter in parameters) 
			{
				buffer.Add(parameter.ID,new ScalarSet(new double[] {parameter.CurrentValue}));
			}

			ILink[] acceptingLinks = this.GetAcceptingLinks();
			double result = 0;
			for (int i=0;i<acceptingLinks.Length;i++) 
			{
				IValueSet valueSet = 
					acceptingLinks[i].SourceComponent.GetValues(time,acceptingLinks[i].ID);
				if (valueSet is IScalarSet) 
				{
					IScalarSet scalarSet = (IScalarSet) valueSet;
					for (int j=0;j<scalarSet.Count;j++) 
					{
						double val = scalarSet.GetScalar(j);
						result = val;
					}
				}
			}
			return result;
		}


		public override IValueSet GetValues(ITime time, string LinkID)
		{
			if (!isComputing) 
			{
				isComputing = true;

				int populationSize = 20;
				int nParam = parameters.Count;
			
				ArrayList solutions = new ArrayList();

				for (int i=0;i<populationSize;i++)
				{
					double[] minimum = new double[nParam];
					double[] maximum = new double[nParam];
					double[] values = new double[nParam];

					for (int j=0;j<nParam;j++) 
					{
						ParameterDescriptor descriptor =
							(ParameterDescriptor)parameters[j];
						minimum[j] = descriptor.Minimum;
						maximum[j] = descriptor.Maximum;
						values[j] = descriptor.CurrentValue;
					}
					Solution solution = new Solution(values,minimum,maximum,0.0,random);
					solution.randomize();
					solution._cost = EvaluateCostFunction(time,solution);
					solutions.Add(solution);
				}

				while (true) 
				{
					if (evaluationCount%1000==0) 
					{
						int bestIndex=0;
						for (int i=0;i<solutions.Count;i++)
						{
							if (((Solution)solutions[i])._cost<((Solution)solutions[bestIndex])._cost)
								bestIndex=i;
						}
						Debug.WriteLine("Best solution after "+evaluationCount+" evaluations:");
						Debug.WriteLine("(Cost="+((Solution)solutions[bestIndex])._cost+")");
						for (int i=0;i<parameters.Count;i++) 
						{
							Debug.WriteLine(((ParameterDescriptor)parameters[i]).ID+"="+((Solution)solutions[bestIndex])._values[i]);
						}
					}

					if (evaluationCount==10000)
						break;

					ArrayList tournament = new ArrayList();

					for (int i=0;i<4;i++) 
					{
						Solution solution;
						do 
						{
							solution = (Solution)solutions[random.Next(solutions.Count)];
						} while (tournament.Contains(solution));
						tournament.Add(solution);
					}

					tournament.Sort();

					solutions.Remove(tournament[2]);
					solutions.Remove(tournament[3]);

					Solution child1 = new Solution((Solution)tournament[0],(Solution)tournament[1],
						random);
					Solution child2 = new Solution((Solution)tournament[0],(Solution)tournament[1],
						random);
					child1._cost = EvaluateCostFunction(time,child1);
					child2._cost = EvaluateCostFunction(time,child2);
					solutions.Add(child1);
					solutions.Add(child2);
				}

				double[] result = new double[nParam];
				for (int i=0;i<nParam;i++)
					result[i] = ((Solution) solutions[0])._values[i];
				return new ScalarSet(result);
			} 
			else 
			{
				ILink outputLink = GetLink(LinkID);
				return buffer.Get(outputLink.SourceElementSet.ID);
			}
		}
	}

	public class Solution : IComparable
	{
		public double[] _values;
		public static double[] _minimum;
		public static double[] _maximum;
		public double _cost;
		public Random random;



		public Solution(double[] values,double[] minimum,double[] maximum,double cost,
			Random random)
		{
			_values = values;
			_minimum = minimum;
			_maximum = maximum;
			_cost = cost;
			this.random = random;
		}

		public Solution(Solution parent1,Solution parent2,Random random)
		{
			this.random = random;
			int length = parent1._values.Length;
			_values = new double[length];

			// intermediate recombination
			for (int i=0;i<length;i++) 
			{
				double a = random.NextDouble()*1.5-0.25;
				_values[i] = a*parent1._values[i]+(1-a)*parent2._values[i];
			}
			mutate();
		}

		public void randomize()
		{
			for (int i=0;i<_values.Length;i++) 
			{
				_values[i] = random.NextDouble()*(_maximum[i]-_minimum[i])+_minimum[i];
			}
		}

		void mutate()
		{

			// breeder GA mutation

			for (int i = 0;i<_values.Length;i++) 
			{
				if (random.NextDouble()<1.0/_values.Length) 
				{
					double s = 2.0*(random.NextDouble()-0.5);
					double r = 0.1*(_maximum[i]-_minimum[i]);
					double k = 4;
					double u = random.NextDouble();
					double a = Math.Pow(2.0,-k*u);

					_values[i] = _values[i]+s*r*a;
				}
			}
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if(obj is Solution) 
			{
				Solution solution = (Solution) obj;
				return _cost.CompareTo(solution._cost);
			}
			throw new ArgumentException("object is not a Solution");   
		}
		#endregion
	}
}
