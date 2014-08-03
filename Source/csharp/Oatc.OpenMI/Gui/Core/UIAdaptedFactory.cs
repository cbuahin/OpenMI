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
using System.Collections;
using System.Reflection;
using System.IO;
using System.Linq;
using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
    public class UIAdaptedFactory : IAdaptedOutputFactory
    {
        string _id = string.Empty;
        string _modelCaption = string.Empty;
        FileInfo _assembly = null;
        Type _type = null;
        IAdaptedOutputFactory _factory = null;
        bool _is3rdParty = true;


        public UIAdaptedFactory() { }

        public void InitialiseAs3rdParty(Type type, ITimeSpaceComponent source)
        {
            FileInfo assembly = new FileInfo(System.Reflection.Assembly.GetAssembly(type).Location);

            InitialiseAs3rdParty(assembly, type.FullName);

            _modelCaption = source.Caption;
        }

        public void InitialiseAs3rdParty(FileInfo assembly, string type)
        {
            _assembly = assembly;
            _is3rdParty = true;

            // 3rd party factory

            if (!_assembly.Exists)
                throw new ArgumentException("Cannot find adapter factory " + _assembly.FullName);

            Assembly ass = System.Reflection.Assembly.LoadFile(_assembly.FullName);

            if (ass == null)
                throw new ArgumentException("Cannot load adapter factory " + _assembly.FullName);

            _type = ass.GetType(type, true, true);

            _factory = (IAdaptedOutputFactory)Activator.CreateInstance(_type);

            if (_factory == null)
                throw new ArgumentException("Cannot create adapter factory "
                    + _assembly.FullName + ": " + _type);
        }

        public void InitialiseAsNative(string id, IBaseLinkableComponent source)
        {
            _id = id;
            _modelCaption = source.Caption;
            _is3rdParty = false;

            foreach (IAdaptedOutputFactory factory in source.AdaptedOutputFactories)
            {
                if (factory.Id == id)
                {
                    _type = factory.GetType();
                    _assembly = new FileInfo(_type.Assembly.Location);
                    _factory = factory;
                    return;
                }
            }

            throw new ArgumentException(string.Format(
                "Cannot find adapter factory \"{0}\" in model \"{1}\"", id, source.Caption));
        }

        public void Initialise(oprConnectionDecoratorFactory opr, IBaseLinkableComponent source, DirectoryInfo oprPath)
        {
            if (opr.id != string.Empty || opr.assembly == "SourceComponent")
            {
                InitialiseAsNative(opr.id, source);
            }
            else // 3rd party
            {
                FileInfo assembly;

                if (!Path.IsPathRooted(opr.assembly))
                    assembly = new FileInfo(
                        Path.Combine(oprPath.FullName, opr.assembly));
                else
                    assembly = new FileInfo(opr.assembly);

                InitialiseAs3rdParty(assembly, opr.type);
            }
        }

        public string ModelCaption
        {
            get { return _modelCaption; }
        }

        public oprConnectionDecoratorFactory Persist(DirectoryInfo oprPath)
        {
            oprConnectionDecoratorFactory factory = new oprConnectionDecoratorFactory();

            factory.type = _type.FullName;

            if (_is3rdParty)
            {
                factory.id = string.Empty;
                factory.assembly = oprPath == null  ? _assembly.FullName: Utils.RelativePath(oprPath, _assembly.FullName);
            }
            else
            {
                factory.id = _id;
                factory.assembly = null;
            }

            return factory;
        }

        public override string ToString()
        {
            return _modelCaption == string.Empty ? string.Format("{0} ({1})", _factory.Caption, _factory.GetType().ToString()) : string.Format("\"{0}\", {1}", _modelCaption, _factory.Caption);
        }

        public FileInfo Assembly
        {
            get { return _assembly; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public bool Is3rdParty
        {
            get { return _is3rdParty; }
        }

        public IAdaptedOutputFactory Factory
        {
            get { return _factory; }
        }

        public UIAdaptedOutputItem NewAdaptedUIOutputItem(string decoratorId, UIOutputItem parent, ITimeSpaceInput target)
        {
        
            IIdentifiable[] ids = _factory.GetAvailableAdaptedOutputIds(parent.TimeSpaceOutput, target);

            IIdentifiable found = (from n in ids
                                  where n.Id == decoratorId select n).FirstOrDefault();

            IBaseAdaptedOutput iAdapted = _factory.CreateAdaptedOutput(found, parent.TimeSpaceOutput, target);

            UIAdaptedOutputItem item = new UIAdaptedOutputItem(this, found, (ITimeSpaceAdaptedOutput)iAdapted, parent);

            parent.AddAdaptedOutput(item.TimeSpaceAdaptedOutput);

            return item;
        }

        #region IAdaptedOutputFactory Members

        public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
        {
            return _factory.GetAvailableAdaptedOutputIds(adaptee, target);
        }

        public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
        {
            return _factory.CreateAdaptedOutput(adaptedOutputId, adaptee, target);
        }

        #endregion

        #region IDescribable Members

        public string Caption
        {
            get { return _factory.Caption; }
            set { _factory.Caption = value; }
        }

        public string Description
        {
            get { return _factory.Description; }
            set { _factory.Description = value; }
        }

        #endregion

        #region IIdentifiable Members

        public string Id
        {
            get { return _id; }
        }

        #endregion
    }
}
