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
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
	public class CompositionRun
	{
		BackgroundWorker _worker;
		FileInfo _oprfile;
		bool _allowCancel = false;
		bool _reportProgress = false;
		bool _canceled = false;
		bool _monitorComponentEvents = true;
		bool _monitorExchangeEvents = true;

		RunStatus _status = RunStatus.available;
		List<IBaseLinkableComponent> _components;

		public enum RunStatus { available = 0, running, completed_ok, cancelled, exception_thrown, }

		public RunStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public bool ReportProgress
		{
			get { return _reportProgress; }
			set { _reportProgress = value; }
		}
        
		public bool AllowCancel
		{
			get { return _allowCancel; }
			set { _allowCancel = value; }
		}

		public bool MonitorComponentEvents
		{
			get { return _monitorComponentEvents; }
			set { _monitorComponentEvents = value; }
		}

		public bool MonitorExchangeEvents
		{
			get { return _monitorExchangeEvents; }
			set { _monitorExchangeEvents = value; }
		}

		public delegate void RunCompleted(object sender, RunWorkerCompletedEventArgs e);
		public delegate void RunProgress(object sender, ProgressChangedEventArgs e);

		public void RunAsync(FileInfo oprfile, RunProgress progress, RunCompleted completed)
		{
			if (_status == RunStatus.running)
				throw new Exception("Already running, cannot start additional run");

			_oprfile = oprfile;

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(DoRun);

			_reportProgress = progress != null;

			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(completed);
			worker.ProgressChanged += new ProgressChangedEventHandler(progress);
			worker.WorkerReportsProgress = _reportProgress;
			worker.WorkerSupportsCancellation = _allowCancel;
			worker.RunWorkerAsync();

			_status = RunStatus.running;
		}

		public void Run(FileInfo oprfile)
		{
			try
			{
				if (_status == RunStatus.running)
					throw new Exception("Already running, cannot start additional run");

				_oprfile = oprfile;

				_status = RunStatus.running;

				DoRun(null, null);

				_status = RunStatus.completed_ok;
			}
			catch
			{
				_status = RunStatus.exception_thrown;
				throw;
			}
		}

		public void Cancel()
		{
			if (_worker != null && _status == RunStatus.running)
				_worker.CancelAsync();
		}

		public bool Canceled
		{
			set { _canceled = value; }
			get 
			{
				if (_worker != null && _worker.CancellationPending)
				{
					_canceled = true;

					foreach (ITimeSpaceComponent iLC in _components)
						iLC.Finish();
				}
					
				return _canceled; 
			}
		}

		void DoRun(object sender, DoWorkEventArgs e)
		{
			Canceled = false;

			// Get the BackgroundWorker that raised this event.
			_worker = sender != null ? sender as BackgroundWorker : null;

			if (_worker != null && _worker.WorkerReportsProgress)
				_worker.ReportProgress(0);
			//_worker.ReportProgress(0, new State(State.Status.Preparation));

			if (!_oprfile.Exists)
				throw new FileNotFoundException(_oprfile.FullName);

			_components = new List<IBaseLinkableComponent>();
			ITimeSpaceComponent triggered = null;

			List<UIModel> models;
            List<UIConnection> connections;

            Opr.Load(_oprfile, out models, out connections);

            if (models == null || models.Count == 0)
				throw new InvalidDataException("No models found in " + _oprfile.FullName);

			if (Canceled)
				return;

			Dictionary<UIModel, ITimeSpaceComponent> modelcomponents
				= new Dictionary<UIModel, ITimeSpaceComponent>();

			foreach (UIModel model in models)
			{
				model.LinkableComponent.StatusChanged += new EventHandler<LinkableComponentStatusChangeEventArgs>(LinkableComponent_StatusChanged);

				_components.Add(model.LinkableComponent);

				modelcomponents.Add(model, model.LinkableComponent);

				if (model.IsTrigger)
					triggered = model.LinkableComponent;
			}

			ITimeSpaceComponent provider, acceptor;

			foreach (UIConnection connection in connections)
			{
				provider = modelcomponents[connection.SourceModel];
				acceptor = modelcomponents[connection.TargetModel];

				foreach (UIConnection.Link pair in connection.Links)
				{
					pair.Source.AddConsumer(pair.Target);
					pair.Target.Provider = pair.Source;
				}
			}

            //prepare

            foreach (ITimeSpaceComponent iComponent in _components)
            {
                iComponent.Prepare();
            }

			if (triggered == null)
				throw new InvalidOperationException("No trigger specified");

			if (Canceled)
				return;

			// See comment on WaitingForFinish additional state
			while (triggered.Status != LinkableComponentStatus.Finishing
					&& triggered.Status != LinkableComponentStatus.Failed)
			{
				if (Canceled)
					return;

				//Thread.Sleep(1000); // TODO: Remove
				triggered.Update();
			}

			foreach (ITimeSpaceComponent iComponent in _components)
			{
				iComponent.Finish();

				if (iComponent is IDisposable)
					((IDisposable)iComponent).Dispose();
			}
		}

		public class State
		{
			int _oprIndex = -1;
			LinkableComponentStatusChangeEventArgs _statusArgs = null;
			ExchangeItemChangeEventArgs _exchangeArgs = null;
			DateTime _lastEventUpdate;

			public State(int oprIndex, LinkableComponentStatusChangeEventArgs args)
			{
				_oprIndex = oprIndex;
				_statusArgs = args;
				_lastEventUpdate = DateTime.Now;
			}

			public State(int oprIndex, ExchangeItemChangeEventArgs args)
			{
				_oprIndex = oprIndex;
				_exchangeArgs = args;
				_lastEventUpdate = DateTime.Now;
            }	

			public LinkableComponentStatusChangeEventArgs StatusArgs
			{
				get { return _statusArgs; }
			}

			public ExchangeItemChangeEventArgs ExchangeArgs
			{
				get { return _exchangeArgs; }
			}

			public int OprIndex
			{
				get { return _oprIndex; }
			}

			public DateTime LastEventUpdate
			{
				get { return _lastEventUpdate; }
			}

            public int? Progress
            {
                get
                {

                    if (_statusArgs == null || _statusArgs.LinkableComponent == null)
                        return null;

                    IBaseLinkableComponent iLC = _statusArgs.LinkableComponent;

                    if (iLC.TimeExtent() == null
                        || iLC.TimeExtent().TimeHorizon == null)
                        return null;

                    double start = iLC.TimeExtent().TimeHorizon.StampAsModifiedJulianDay;
                    double end = start + iLC.TimeExtent().TimeHorizon.DurationInDays;

                    double last = start;
                    ITime iTime = null;

                    // Try the TimeExtent (if implemented), this will provide values for Stand alone runs
                    // ie that have no output exchange items 
                    IList<ITime> times = iLC.TimeExtent().Times ;

                    lock (times)
                    {
                        if (times != null && times.Count > 0)
                        {
                            iTime = iLC.TimeExtent().Times[iLC.TimeExtent().Times.Count - 1];

                            if (iTime.StampAsModifiedJulianDay > last)
                            {
                                last = iTime.StampAsModifiedJulianDay;
                            }
                        }
                    }

                    if (iTime == null)
                    {
                        foreach (ITimeSpaceOutput item in iLC.Outputs)
                        {
                            if (item.TimeSet != null && item.TimeSet.Times != null && item.TimeSet.Times.Count > 0)
                            {
                                iTime = item.TimeSet.Times[item.TimeSet.Times.Count - 1];

                                if (iTime.StampAsModifiedJulianDay > last)
                                {
                                    last = iTime.StampAsModifiedJulianDay;
                                }
                            }
                        }
                    }
        
                    double p = 100.0 * (last - start) / (end - start);

                    return (int)p;
                }
            }
		}

		void LinkableComponent_StatusChanged(object sender, LinkableComponentStatusChangeEventArgs e)
		{
			if (!_monitorComponentEvents)
				return;

			if (_worker != null && _worker.WorkerReportsProgress)
				_worker.ReportProgress(0,
					new State(_components.IndexOf((ITimeSpaceComponent)sender), e));
		}

		void LinkableComponent_ExchangeItemValueChanged(object sender, ExchangeItemChangeEventArgs e)
		{
			if (!_monitorExchangeEvents)
				return;

			if (_worker != null && _worker.WorkerReportsProgress)
				_worker.ReportProgress(0,
					new State(_components.IndexOf(((ITimeSpaceExchangeItem)sender).Component), e));
		}
	}
}
