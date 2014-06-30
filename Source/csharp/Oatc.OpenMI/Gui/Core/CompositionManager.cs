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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Reflection;

using log4net;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{

    /// <summary>
    /// Summary description for CompositionManager.
    /// </summary>
    public class CompositionManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompositionManager));

        #region Static members

        #endregion

        #region Internal members

        bool _oprUnsaved;

        Thread _runThread;
        bool _running;
        bool _runInSameThread;

        List<UIModel> _models;
        List<UIConnection> _connections;
        bool[] _listenedEventTypes;
        DateTime _triggerInvokeTime;
        string _logFileName;
        bool _showEventsInListbox;

        private FileInfo _oprFile;

        #endregion

        /// <summary>
        /// Creates a new empty instance of <c>CompositionManager</c> class.
        /// </summary>
        /// <remarks>See <see cref="Initialize">Initialize</see> for more detail.</remarks>
        public CompositionManager()
        {
            Initialize();
        }


        #region Public properties

		public FileInfo FileOpr
		{
			get { return _oprFile; }
		}

        /// <summary>
        /// Gets list of all models (ie. instances of <see cref="UIModel">UIModel</see> class) in composition.
        /// </summary>
        public List<UIModel> Models
        {
            get { return (_models); }
        }


        /// <summary>
        /// Gets list of all connections (ie. instances of <see cref="UIConnection">UIConnection</see> class) in composition.
        /// </summary>
        public List<UIConnection> Connections
        {
            get { return (_connections); }
        }


        /// <summary>
        /// Gets array of <c>bool</c> describing which events should be listened during simulation run.
        /// </summary>
        /// <remarks>Array has <see cref="EventType.NUM_OF_EVENT_TYPES">EventType.NUM_OF_EVENT_TYPES</see>
        /// elements. See <see cref="EventType">EventType</see>, <see cref="Run">Run</see> for more detail.
        /// </remarks>
        public bool[] ListenedEventTypes
        {
            get { return (_listenedEventTypes); }
            /*	set
                {
                    Debug.Assert( value.Length == (int)EventType.NUM_OF_EVENT_TYPES );
                    _listenedEventTypes = value;
                    _shouldBeSaved = true;
                }*/
        }


        /// <summary>
        /// Time when trigger should be invoked.
        /// </summary>
        /// <remarks>See <see cref="EventType">EventType</see> and <see cref="Run">Run</see> for more detail.</remarks>
        public DateTime TriggerInvokeTime
        {
            get { return (_triggerInvokeTime); }
            set
            {
                if (_triggerInvokeTime != value)
                {
                    _triggerInvokeTime = value;
                    _oprUnsaved = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether simulation should be run in same thread. By default it's <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is only recommendation of composition author, you can override
        /// this setting while calling <see cref="Run">Run</see> method. For example
        /// if running from console, simulation is always executed in same thread.
        /// </remarks>
        public bool RunInSameThread
        {
            get { return (_runInSameThread); }
            set
            {
                if (_runInSameThread != value)
                {
                    _oprUnsaved = true;
                    _runInSameThread = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets whether events should be showed in list-box during simulation in UI.
        /// </summary>
        public bool ShowEventsInListbox
        {
            get { return (_showEventsInListbox); }
            set
            {
                if (_showEventsInListbox != value)
                {
                    _showEventsInListbox = value;
                    _oprUnsaved = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets wheather composition was changed and should be saved to OPR file.
        /// </summary>
        /// <remarks>See <see cref="SaveToFile">SaveToFile</see>.</remarks>
        public bool ShouldBeSaved
        {
            get { return (_oprUnsaved); }
            set { _oprUnsaved = value; }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Initializes this composition.
        /// </summary>
        public void Initialize()
        {
            _models = new List<UIModel>();
            _connections = new List<UIConnection>();

            _triggerInvokeTime = new DateTime(1900, 1, 1);

            _showEventsInListbox = true;

            _logFileName = "CompositionRun.log";

			_oprFile = null;
            _oprUnsaved = false;

            _runThread = null;
            _running = false;
            _runInSameThread = false;

			AssemblySupport.ReleaseAll();
        }

		public void ModelAdd(UIModel model)
		{
			foreach (UIModel uiModel in _models)
			{
				if (model.Rect.X == uiModel.Rect.X && model.Rect.Y == uiModel.Rect.Y)
				{
					model.Rect.X = model.Rect.X + model.Rect.Width + 50;
					model.Rect.Y = model.Rect.Y + model.Rect.Height+ 50;
				}
			}

			_models.Add(model);

			_oprUnsaved = true;
		}

        /// <summary>
        /// Removes specified model from composition.
        /// </summary>
        /// <param name="model">Model to be removed.</param>
        /// <remarks>The <c>Dispose</c> method is called on the model.</remarks>
        public void ModelRemove(UIModel model)
        {
            // first remove all links from/to this model
            UIConnection[] copyOfLinks = (UIConnection[])_connections.ToArray();
            {
                foreach (UIConnection uiLink in copyOfLinks)
                    if (uiLink.TargetModel == model || uiLink.SourceModel == model)
                        RemoveConnection(uiLink);

                model.LinkableComponent.Finish();
            }

            _oprUnsaved = true;
            _models.Remove(model); // remove model itself
        }


        /// <summary>
        /// Removes all model from composition.
        /// </summary>
        /// <remarks>See <see cref="RemoveModel">RemoveModel</see> for more detail.</remarks>
        public void RemoveAllModels()
        {
            UIModel[] copyOfModels = _models.ToArray();
            foreach (UIModel model in copyOfModels)
                ModelRemove(model);
        }


        public void AddConnection(UIConnection connection)
        {
			if (connection.SourceModel == connection.TargetModel)
                throw (new Exception("Cannot connect model with itself."));

            // Check whether both models exist
            bool providingFound = false, acceptingFound = false;
            
			foreach (UIModel model in _models)
            {
				if (model == connection.SourceModel)
                    providingFound = true;
				if (model == connection.TargetModel)
                    acceptingFound = true;
            }

            if (!providingFound || !acceptingFound)
                throw (new Exception("Cannot find providing or accepting."));

            // check whether this link isn't already here (if yes, replace)
            foreach (UIConnection uiConnection in _connections)
				if (uiConnection.SourceModel == connection.SourceModel
					&& uiConnection.TargetModel == connection.TargetModel)
					_connections.Remove(uiConnection);

            _connections.Add(connection);

            _oprUnsaved = true;
        }


        /// <summary>
        /// Removes connection between two models.
        /// </summary>
        /// <param name="connection">Connection to be removed.</param>
        public void RemoveConnection(UIConnection connection)
        {
            _connections.Remove(connection);
            _oprUnsaved = true;
        }


        /// <summary>
        /// Gets trigger "model".
        /// </summary>
        /// <returns>Returns trigger, or <c>null</c> if composition has no trigger.</returns>
        public ITimeSpaceComponent GetTrigger()
        {
            foreach (UIModel uiModel in _models)
                if (uiModel.IsTrigger)
                    return uiModel.LinkableComponent;
            return (null);
        }


        public void Save()
        {
			bool saveRelativePaths = true;

			if (_oprFile != null)
				Opr.Save(_oprFile, saveRelativePaths, _models, _connections.ToArray());

            _oprUnsaved = false;
        }

		public void SaveAs(FileInfo oprFile)
		{
			_oprFile = oprFile;
			
			Save();
		}

        public void Open(FileInfo oprFile)
        {			
			Initialize();

			_oprFile = oprFile;

            Opr.Load(_oprFile, out _models, out _connections);
        }

        public void ReOpen()
        {
			FileInfo oprFile = _oprFile;

			Debug.Assert(!_oprUnsaved);

			Initialize();

			Open(oprFile);
        }

        /// <summary>
        /// Calculates time horizon of the simulation,
        /// ie. time between earliest model start and latest model end.
        /// </summary>
        /// <returns>Returns simulation time horizon.</returns>
        public ITime GetSimulationTimehorizon()
        {
            double start = double.MaxValue, end = double.MinValue;

            foreach (UIModel model in _models)
            {
                if (model.IsTrigger)
                    continue;

                foreach(ITimeSpaceInput item in model.LinkableComponent.Inputs)
                {
                    // TODO: probably we should just take it from Times here?
                    if (item.TimeSet != null && item.TimeSet.TimeHorizon != null)
                    {
                        start = Math.Min(start, item.TimeSet.TimeHorizon.StampAsModifiedJulianDay);
                        end = Math.Max(end, item.TimeSet.TimeHorizon.StampAsModifiedJulianDay + item.TimeSet.TimeHorizon.DurationInDays);
                    }
                }
            }

            Debug.Assert(start < end);

            return (new Time(start, end - start));
        }


        /// <summary>
        /// Runs simulation.
        /// </summary>
        /// <param name="runInSameThread">If <c>true</c>, simulation is run in same thread like caller,
        /// ie. method blocks until simulation don't finish. If <c>false</c>, simulation is
        /// run in separate thread and method returns immediately.</param>
        /// <remarks>
        /// Simulation is run the way that trigger invokes the Update of the model it's connected to
        /// at the time specified by <see cref="TriggerInvokeTime">TriggerInvokeTime</see> property.
        /// If you need to use more than one listener you can use <see cref="ProxyListener">ProxyListener</see>
        /// class or <see cref="ProxyMultiThreadListener">ProxyMultiThreadListener</see> if <c>runInSameThread</c> is <c>false</c>.
        /// </remarks>
        public void Run(bool runInSameThread)
        {
            if (_models.Count < 1)
                return;

            // Mutiple model composition 

            if (_running)
                throw (new Exception("Simulation is already running."));

            _running = true;

            try
            {
                foreach (UIModel model in _models)
                {
                    model.LinkableComponent.StatusChanged += LinkableComponent_StatusChanged;
                }

                if (!runInSameThread)
                {
                    _runThread = new Thread(RunThreadFunction);
                    _runThread.Start();
                }
                else
                {
                    RunThreadFunction();
                }
            }
            catch (Exception e)
            {
                log.Error("Error while running models", e);
            }

        }

        void LinkableComponent_ExchangeItemValueChanged(object sender, ExchangeItemChangeEventArgs e)
        {
            throw new NotImplementedException();
        }

        void LinkableComponent_StatusChanged(object sender, LinkableComponentStatusChangeEventArgs e)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Stops the simulation.
        /// </summary>
        /// <remarks>This method has effect only if simulation is run in separate thread
        /// (see <see cref="Run">Run</see> method).
        /// This method calls <see cref="Thread.Abort()">Abort</see> method on the simulation thread.</remarks>
        public void Stop()
        {
            if (_running && _runThread != null)
                _runThread.Abort();
            _runThread = null;
        }


        #endregion

        #region Private methods

        private static ITimeSpaceInput FindExchangeItem(IEnumerable<ITimeSpaceInput> exchangeItems, string exchangeItemID)
        {
            foreach (ITimeSpaceInput inputItem in exchangeItems)
            {
                if (inputItem.Id.Equals(exchangeItemID))
                {
                    return inputItem;
                }
            }
            return null;
        }

        private static ITimeSpaceOutput FindExchangeItem(IEnumerable<ITimeSpaceOutput> exchangeItems, string exchangeItemID)
        {
            foreach (ITimeSpaceOutput outputItem in exchangeItems)
            {
                if (outputItem.Id.Equals(exchangeItemID))
                {
                    return outputItem;
                }
            }
            return null;
        }

        /// <summary>
        /// This method is called in <see cref="Run">Run</see> method.
        /// </summary>
        private void RunThreadFunction()
        {
            ITimeSpaceComponent trigger = GetTrigger();

            Debug.Assert(trigger != null);

            Thread.Sleep(0);

            try
            {
                bool allComponentFinished = false;
                while (!allComponentFinished)
                {
                    foreach (UIModel model in _models)
                    {
                        model.LinkableComponent.Update();
                    }

                    // check if components are finished
                    allComponentFinished = true;
                    foreach (UIModel model in _models)
                    {
                        if (model.LinkableComponent.Status != LinkableComponentStatus.Done)
                        {
                            allComponentFinished = false;
                        }

                        if (model.LinkableComponent.Status == LinkableComponentStatus.Failed)
                        {
                            log.Error("Component has failed: " + model.LinkableComponent);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error while running components", e);
            }
            finally
            {
                _running = false;
            }
        }


        #endregion
    }
}
