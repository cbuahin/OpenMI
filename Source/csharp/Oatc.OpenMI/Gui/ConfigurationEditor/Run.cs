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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading; // TODO remove with Sleep?
using System.IO;

using OpenMI.Standard2;
using Oatc.OpenMI.Gui.Core;
using OpenMI.Standard2.TimeSpace;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	public partial class Run : Form
	{
		enum RunStatus { Runable, Running, Done, Aborted, Failed, }

		RunStatus _runStatus = RunStatus.Runable;
		FileInfo _oprFile;
        BindingList<ProgressStatus> status;

		CompositionRun _runManager;
		List<CompositionRun.State> _logCache;
        int _nOprIndexTrigger = -1;
        DateTime _startTime;
        TimeSpan _timeSpan;

		public Run()
		{
			InitializeComponent();
		}

	    public class Log : DataTable
		{
			public enum EColumns { DateTime = 0, Component, Details}

			Type[] _columnTypes = new Type[] { 
				typeof(DateTime),
				typeof(String),
				typeof(String),
				};

			String[] _columnNames = new String[] { 
				EColumns.DateTime.ToString(),
				EColumns.Component.ToString(),
				EColumns.Details.ToString(),
				};

			List<DataRow> _rows = new List<DataRow>();

			public void Initialise(List<CompositionRun.State> states)
			{
				for (int n = 0; n < Enum.GetNames(typeof(EColumns)).Length; ++n)
					base.Columns.Add(new DataColumn(_columnNames[n], _columnTypes[n]));

				DataRow row;
				LinkableComponentStatusChangeEventArgs status;
				ExchangeItemChangeEventArgs exchange;
				StringBuilder sb;
                int? progress;

				foreach (CompositionRun.State state in states)
				{
					row = NewRow();
					row[(int)EColumns.DateTime] = state.LastEventUpdate;

					if (state.StatusArgs != null)
					{
						status = state.StatusArgs;

                        progress = state.Progress;

						row[(int)EColumns.Component] = string.Format("{0}: {1}",
							state.OprIndex.ToString(),
							status.LinkableComponent.Caption);
						row[(int)EColumns.Details] = progress != null 
                            ? string.Format(
							    "[{0}%], Component Status: {1} => {2}",
                                progress.ToString(),
							    status.OldStatus.ToString(),
							    status.NewStatus.ToString())                                            
                            : string.Format(
							    "Component Status: {0} => {1}",
							    status.OldStatus.ToString(),
							    status.NewStatus.ToString());
					}
					else
					{
						exchange = state.ExchangeArgs;

						row[(int)EColumns.Component] = string.Format("{0}: {1}",
							state.OprIndex.ToString(),
							exchange.ExchangeItem.Component.Caption);

						sb = new StringBuilder(string.Format(
								"{0}: {1}, ",
								exchange.ExchangeItem is ITimeSpaceInput ? "Target" : "Source",
								exchange.ExchangeItem.Caption));

						if (exchange.Message != string.Empty)
							sb.Append(exchange.Message);

						row[(int)EColumns.Details] = sb.ToString();
					}

					Rows.Add(row);
				}
			}

			public DataGridViewCell Cell(EColumns column, int oprIndex, DataGridView grid)
			{
				return grid.Rows[oprIndex].Cells[(int)column];
			}
		}

        void UpdateTitleText(int? progress)
        {
            string text = progress == null
                ? string.Format("Run: {0}", _oprFile.Name)
                : string.Format("Run [{0}%]: {1}", progress.Value.ToString(),  _oprFile.Name);

            if (text != Text)
                Text = text;
        }

		public void Initialise(string oprFile)
		{
			if (oprFile == "")
				Open();

            status = new BindingList<ProgressStatus>();

			Status = RunStatus.Runable;
			_oprFile = new FileInfo(oprFile);
		    
			_runManager = new CompositionRun();
			_logCache = new List<CompositionRun.State>();

			

            List<UIModel> models;
            List<UIConnection> connections;

            Opr.Load(_oprFile, out models, out connections);

			if (models.Count == 0)
				throw new InvalidDataException("No models found in " + _oprFile.FullName);

			// LinkableComponents

            int nOprIndex = -1;

            foreach (UIModel model in models)
			{
                ++nOprIndex;

                status.Add(new ProgressStatus(model.LinkableComponent));
             

                if (model.IsTrigger)
                    _nOprIndexTrigger = nOprIndex;
			}

			

			dataGridViewStatus.DataSource = status;

			btnOk.Text = "Run";

            UpdateTitleText(null);
			Refresh();
		}

		private void btnLog_Click(object sender, EventArgs e)
		{
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (Status == RunStatus.Runable)
				RunComposition();
			else if (Status == RunStatus.Running)
				Abort();
			else
				Reload();
		}

		private void Abort()
		{
			_runManager.Cancel();
		}

		private void Reload()
		{
			FileInfo oprFile = _oprFile;

            AssemblySupport.ReleaseAll();

            progressBarRun.Value = 0;
            progressBarRun.Invalidate();
            UpdateTitleText(null);

			Initialise(oprFile.FullName);
		}

		private void RunComposition()
		{
			Status = RunStatus.Running;

            progressBarRun.Minimum = 0;
            progressBarRun.Maximum = 100;
            progressBarRun.Value = 0;
            UpdateTitleText(0);

            _startTime = DateTime.Now;

			_runManager.AllowCancel = true; // as in UI
			_runManager.RunAsync(_oprFile, RunProgress, RunCompleted);
		}

		RunStatus Status
		{
			get { return _runStatus; }
			set
			{
				_runStatus = value;

                string elapsed = string.Format("{0},{1},{2}.{3}",
                    _timeSpan.Hours, _timeSpan.Minutes,
                    _timeSpan.Seconds, _timeSpan.Milliseconds);

				switch (_runStatus)
				{
					case RunStatus.Aborted:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Aborted [{0}%]\r\nElapsed {1}",
                            progressBarRun.Value, elapsed);
                        break;
					case RunStatus.Failed:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Failed [{0}%]\r\nElapsed {1}",
                            progressBarRun.Value, elapsed);
                        break;
					case RunStatus.Done:
						btnOk.Text = "Reload";
                        labelMessage.Text = string.Format("Run completed\r\nElapsed {0}",
                            elapsed);
                        break;
					case RunStatus.Runable:
						btnOk.Text = "Run";
                        labelMessage.Text = "Loaded";
                        break;
					case RunStatus.Running:
						btnOk.Text = "Abort";
                        // labelMessage.Text updated elsewhere
                        break;
					default:
						btnOk.Text = "Reload";
                        labelMessage.Text = "Requires reload";
                        break;
				}

				Refresh();
			}
		}
		

		void RunCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_runManager.Canceled || e.Cancelled)
			{
				// Note that due to a race condition in 
				// the DoWork event handler, the Cancelled
				// flag may not have been set, even though
				// CancelAsync was called.
				Status = RunStatus.Aborted;
			}
			else if (e.Error != null)
			{
				Status = RunStatus.Failed;
				MessageBox.Show(Utils.ToString(e.Error),
					"Run failed ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
				Status = RunStatus.Done;
		}
    
        delegate void RunProgressDelegate(object sender, ProgressChangedEventArgs e);
		void RunProgress(object sender, ProgressChangedEventArgs e)
		{


				if (e.UserState != null
					&& (e.UserState is CompositionRun.State))
				{
					CompositionRun.State state = (CompositionRun.State)e.UserState;
                    ProgressStatus currStatus = status[state.OprIndex];
                    int? progress = state.Progress;

                    if (progress != null && progress.Value > currStatus.Progress)
                    {
                        IBaseLinkableComponent iLC = state.StatusArgs != null
                            ? state.StatusArgs.LinkableComponent
                            : state.ExchangeArgs.ExchangeItem.Component;



                        currStatus.ComponentStatus = iLC != null ? iLC.Status.ToString() : "";
                        currStatus.LastUpdated = state.LastEventUpdate;


                        currStatus.Progress = progress.Value;

                        if (state.OprIndex == _nOprIndexTrigger && progressBarRun.Value != progress.Value)
                        {

                            progressBarRun.Value = progress.Value;


                            UpdateTitleText(progress);

                            _timeSpan = DateTime.Now - _startTime;

                            string elapsed = string.Format("{0},{1},{2}.{3}",
                                _timeSpan.Hours, _timeSpan.Minutes,
                                _timeSpan.Seconds, _timeSpan.Milliseconds);

                            labelMessage.Text = string.Format("Running [{0}%]\r\nElapsed {1}",
                                progress.Value, elapsed);
                        }

                        dataGridViewStatus.Refresh();
                    }
                 
					_logCache.Add(state);
				}
		}

		public static void RunComposition(string oprFile)
		{
			try
			{
				Run run = new Run();
				run.Initialise(oprFile);
				run.ShowDialog();
			}
			catch (Exception e)
			{
				MessageBox.Show(Utils.ToString(e),
					"Run UI failed ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open();
		}

		private void Open()
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.Filter = "Projects (*.opr)|*.opr|All files|*.*";
				dlg.Multiselect = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Title = "Open project...";

				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;

				_oprFile = new FileInfo(dlg.FileName);

				Reload();
			}
		}

		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RunLog logView = new RunLog(_logCache);
			logView.ShowDialog(this);
		}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _runManager.Cancel();

        }

        public class ProgressStatus : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;
            IBaseLinkableComponent component;
            string  componentStatus;
            DateTime lastUpdated;
            int progress;

            public ProgressStatus(IBaseLinkableComponent component)
            {
                this.component = component;
            }

            public IBaseLinkableComponent Component
            {

                set { component = value; NotifyPropertyChanged(); }
            }


            public string ComponentStatus
            {
                get { return componentStatus;  }
                set
                { 
                    componentStatus = value;
                    NotifyPropertyChanged();
                }
            }

            public string ComponentName
            {
                get { return component.Id; }
                
            }

            public int Progress
            {
                get 
                {
                    return progress; 
                }
                set 
                {
                    progress = value;
                    NotifyPropertyChanged();
                }
            }


            public DateTime LastUpdated
            {
                get { return lastUpdated; }
                set { lastUpdated = value; NotifyPropertyChanged(); }
            }

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
	}
}
