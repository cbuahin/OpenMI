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
using System.Globalization;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;

using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
    public static class Opr
    {
        public static void Save(FileInfo file, bool saveRelativePaths, List<UIModel> models, UIConnection[] connections)
        {
            opr opr = Serialize(models, connections, saveRelativePaths ? file.Directory : null);

            if (saveRelativePaths && opr.models != null)
                foreach (oprModel model in opr.models)
                    model.omi = Utils.RelativePath(file.Directory, model.omi);

            XmlSerializer serializer = new XmlSerializer(typeof(opr));

            using (TextWriter tw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(tw, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");

                    serializer.Serialize(xmlWriter, opr, namespaces);
                }

                using (FileStream fs = new FileStream(file.FullName, FileMode.Create))
                {
                    byte[] info = new UTF8Encoding(false).GetBytes(tw.ToString());
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        static oprModel[] Models(UIModel[] models)
        {
            oprModel[] oprModels
                = new oprModel[models.Length];

            for (int n = 0; n < models.Length; ++n)
            {
                oprModels[n] = new oprModel();
                oprModels[n].omi = models[n].OmiFilename;
                oprModels[n].rect_x = models[n].Rect.X.ToString();
                oprModels[n].rect_y = models[n].Rect.Y.ToString();
                oprModels[n].rect_width = models[n].Rect.Width.ToString();
                oprModels[n].rect_height = models[n].Rect.Height.ToString();

                if (models[n].IsTrigger)
                    oprModels[n].is_trigger = true;
            }

            return oprModels;
        }

        static oprConnection[] Connections(List<UIModel> models, UIConnection[] connections, DirectoryInfo oprPath)
        {
            oprConnection[] oprConnections = new oprConnection[connections.Length];

            for (int n = 0; n < connections.Length; ++n)
            {
               UIConnection uiconn = connections[n];
               oprConnection conn  = new oprConnection();
               conn.source_model_indexSpecified = true;
               conn.target_model_indexSpecified = true;
               conn.source_model_index = models.IndexOf(uiconn.SourceModel);
               conn.target_model_index = models.IndexOf(uiconn.TargetModel);
               
               List<UIAdaptedOutputItem> allAdaptersInConnection;

               conn.decorators = Decorators(uiconn, oprPath, out allAdaptersInConnection);
               conn.links = Links(uiconn, allAdaptersInConnection);
               oprConnections[n] = conn;

            }

            return oprConnections;
        }

        static oprConnectionDecorator[] Decorators(UIConnection connection, DirectoryInfo oprPath, out List<UIAdaptedOutputItem> allAdaptersInConnection)
        {
            allAdaptersInConnection = new List<UIAdaptedOutputItem>();

            UIOutputItem adapter = null;

            foreach (Link link in connection.Links)
            {
                foreach (UIOutputItem source in link.Sources)
                {
                    adapter = source;

                    while (adapter != null && adapter is UIAdaptedOutputItem)
                    {
                        if (!allAdaptersInConnection.Contains((UIAdaptedOutputItem)adapter))
                            allAdaptersInConnection.Add((UIAdaptedOutputItem)adapter);

                        adapter = adapter.Parent;
                    }
                }
            }

            oprConnectionDecorator[] oprDecorators = new oprConnectionDecorator[allAdaptersInConnection.Count];

            for (int n = 0; n < allAdaptersInConnection.Count; ++n)
            {
                oprDecorators[n] = new oprConnectionDecorator();
                oprDecorators[n].item_id = allAdaptersInConnection[n].Id;
                oprDecorators[n].factory = allAdaptersInConnection[n].Factory.Persist(oprPath);

                IList<IArgument> ars = ((ITimeSpaceAdaptedOutput)allAdaptersInConnection[n].TimeSpaceOutput).Arguments;
              
                oprDecorators[n].arguments = DecoratorArgs(new List<IArgument>(ars).ToArray());
            }

            return oprDecorators;
        }

        static oprConnectionDecoratorArgument[] DecoratorArgs(IArgument[] arguments)
        {
            oprConnectionDecoratorArgument[] args = new oprConnectionDecoratorArgument[arguments.Length];

            for (int n = 0; n < arguments.Length; ++n)
            {
                args[n] = new oprConnectionDecoratorArgument();
                args[n].id = arguments[n].Id;
                args[n].value = arguments[n].ValueAsString;
            }

            return args;
        }

        static oprConnectionLink[] Links(UIConnection connection, List<UIAdaptedOutputItem> allAdaptersInConnection)
        {
            oprConnectionLink[] links = new oprConnectionLink[connection.Links.Count];

			UIOutputItem source;

            for (int n = 0; n < connection.Links.Count; ++n)
            {
                Link link = connection.Links[n];
                source = link.Source;

                oprConnectionLink oprLink = new oprConnectionLink()
                {
                    target_item_id = link.Target.Id
                };


                oprSourceItem source_item = new oprSourceItem()
                {
                      source_item_id = source.Id,
                      source_item_is_adaptedoutput = source is UIAdaptedOutputItem,
                      source_item_adpatedoutput_index = source is UIAdaptedOutputItem ? allAdaptersInConnection.IndexOf(source as UIAdaptedOutputItem) : -1,
                };

                oprLink.source_item_id = source_item;

                source = source.Parent;

                while(source != null)
                {
                    oprSourceItem tempItem = new oprSourceItem()
                    {
                        source_item_id = source.Id,
                        source_item_is_adaptedoutput = source is UIAdaptedOutputItem,
                        source_item_adpatedoutput_index = source is UIAdaptedOutputItem ? allAdaptersInConnection.IndexOf(source as UIAdaptedOutputItem) : -1,
                    };

                    source_item.parent_item_id = tempItem;
                    source_item = tempItem;
                    source = source.Parent;
                }

                oprSourceItem[] source_items = new oprSourceItem[link.Sources.Count];

                for(int i = 0; i < source_items.Length ; i++)
                {
                    source = link.Sources[i];

                    source_item = new oprSourceItem()
                    {
                        source_item_id = source.Id,
                        source_item_is_adaptedoutput = source is UIAdaptedOutputItem,
                        source_item_adpatedoutput_index = source is UIAdaptedOutputItem ? allAdaptersInConnection.IndexOf(source as UIAdaptedOutputItem) : -1,
                    };

                    source = source.Parent;

                    source_items[i] = source_item;


                    while (source != null)
                    {
                        oprSourceItem tempItem = new oprSourceItem()
                        {
                            source_item_id = source.Id,
                            source_item_is_adaptedoutput = source is UIAdaptedOutputItem,
                            source_item_adpatedoutput_index = source is UIAdaptedOutputItem ? allAdaptersInConnection.IndexOf(source as UIAdaptedOutputItem) : -1,
                        };

                        source_item.parent_item_id = tempItem;
                        source_item = tempItem;
                        source = source.Parent;
                    }

                }
                oprLink.source_item_ids = source_items;   
                links[n] = oprLink;
            }

            return links;
        }

        static List<oprConnectionLinkDecorated> Decorated(Link link, List<UIAdaptedOutputItem> allAdaptersInConnection)
        {
            List<oprConnectionLinkDecorated> oprs
                = new List<oprConnectionLinkDecorated>();

			List<UIOutputItem> outputs = new List<UIOutputItem>(1);
			outputs.Add(link.Source);

            while (outputs[outputs.Count - 1].Parent != null && outputs[outputs.Count - 1].Parent != outputs[outputs.Count - 1])
            {
                outputs.Add(outputs[outputs.Count - 1].Parent);
            }

			outputs.Reverse();

			oprConnectionLinkDecorated decorator;

			foreach (UIOutputItem source in outputs)
			{
				if (source.TimeSpaceOutput is ITimeSpaceAdaptedOutput)
				{
					decorator = new oprConnectionLinkDecorated();
                    decorator.index = allAdaptersInConnection.IndexOf((UIAdaptedOutputItem)source);
					decorator.indexSpecified = true;
					oprs.Add(decorator);
				}
			}

            return oprs;
        }

        public static void Load(FileInfo oprFile, out List<UIModel> models, out List<UIConnection> connections)
        {
            if (!oprFile.Exists)
                throw new FileNotFoundException(oprFile.FullName);

            object obj;

            using (StreamReader xml = new StreamReader(oprFile.FullName))
            {
                // TODO: Validate against xsd first?

                string sXML = xml.ReadToEnd();

                using (StringReader sr = new StringReader(sXML))
                {
                    XmlSerializer serializer = sXML.Contains("guiComposition")
                        ? new XmlSerializer(typeof(guiComposition))
                        : new XmlSerializer(typeof(opr));

                    obj = serializer.Deserialize(sr);
                }
            }

            if (obj is guiComposition)
                DeserializeV1((guiComposition)obj, oprFile.Directory, out models, out connections);
            else if (obj is opr)
                DeserializeV2((opr)obj, oprFile.Directory, out models, out connections);
            else
                throw new NotImplementedException("OPR Read: Unreconised XML root element");
        }

        static opr Serialize(List<UIModel> models, UIConnection[] connections, DirectoryInfo oprPath)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            opr opr = new opr();

            try
            {
                opr.version = "2.0";
                opr.models = Models(models.ToArray());
                opr.connections = Connections(models, connections, oprPath);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }

            return opr;
        }

        static void DeserializeV2(opr opr, DirectoryInfo oprPath, out List<UIModel> models, out List<UIConnection> connections)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("");

                models = DeserializeModelsV2(opr, oprPath);
                connections = DeserializeConnectionsV2(opr, models, oprPath);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        static void DeserializeV1(guiComposition opr, DirectoryInfo oprPath, out List<UIModel> models, out List<UIConnection> connections)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("");

                models = DeserializeModelsV1(opr, oprPath);
                connections = DeserializeConnectionsV1(opr, models);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        static List<UIModel> DeserializeModelsV2(opr opr, DirectoryInfo oprPath)
        {
            List<UIModel> models = new List<UIModel>();

            if (opr.models == null)
                return models;

            UIModel uiModel;
            FileInfo omiFile = null;

            foreach (oprModel model in opr.models)
            {
                try
                {
                    omiFile = Utils.GetAbsoluteOmiFile(model.omi, oprPath);

                    uiModel = new UIModel();
                    uiModel.OmiDeserializeAndInitialize(omiFile);

                    uiModel.Rect.X = Int32.Parse(model.rect_x);
                    uiModel.Rect.Y = Int32.Parse(model.rect_y);
                    uiModel.Rect.Width = Int32.Parse(model.rect_width);
                    uiModel.Rect.Height = Int32.Parse(model.rect_height);
                    uiModel.IsTrigger = model.is_trigger;

                    models.Add(uiModel);
                }
                catch (Exception e)
                {
                    throw new Exception("Cannot instatiate model " + omiFile == null ? model.omi : omiFile.FullName, e);
                }
            }

            return models;
        }

        static List<UIModel> DeserializeModelsV1(guiComposition opr, DirectoryInfo oprPath)
        {
            throw new NotImplementedException();
#if DEPRECATED
                if (opr.models != null)
                {
                    UIModel uiModel;
                    FileInfo omiFile;

                    foreach (guiCompositionModel oprModel in opr.models)
                    {
                        try
                        {
                            omiFile = Utils.GetAbsoluteOmiFile(
                                oprModel.omi, oprPath);

                            uiModel = new UIModel();
                            uiModel.OmiDeserializeAndInitialize(omiFile);

                            uiModel.Rect.X = Int32.Parse(oprModel.rect_x);
                            uiModel.Rect.Y = Int32.Parse(oprModel.rect_y);
                            uiModel.Rect.Width = Int32.Parse(oprModel.rect_width);
                            uiModel.Rect.Height = Int32.Parse(oprModel.rect_height);

                            ModelAdd(uiModel);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Cannot add model " + oprModel.omi, e);
                        }
                    }

                    // TODO ADH Connections
                }
#endif
        }

        static void SetArguments(oprConnectionDecoratorArgument[] arguments, ITimeSpaceAdaptedOutput iDecorator)
        {
            if (arguments == null || arguments.Length < 1)
                return;

            Dictionary<string, IArgument> args 
                = new Dictionary<string, IArgument>(iDecorator.Arguments.Count);

            foreach (IArgument arg in iDecorator.Arguments)
                args.Add(arg.Id, arg);

            foreach (oprConnectionDecoratorArgument arg in arguments)
                if (args.ContainsKey(arg.id))
                    args[arg.id].ValueAsString = arg.value;
        }

        // TODO Wrap Arguments so that exception thrown to indicate invalid value
        // can be caught and presented to the user.
        // e.g. valueA now = fred, valueB can only be ethal or freda
        // 

        // Arguments list members (count as well as values) can change depending on
        // values of other arguments.
        // So say, 1 argument firstly which is file name to another list of
        // arguments to then display.

        // OpenWEB
        // model file change throws exception saying reload required
        // pipistrelle reconises exception_openweb type and reloads automatically

        static List<UIConnection> DeserializeConnectionsV2(opr opr, List<UIModel> models, DirectoryInfo oprPath)
        {
            List<UIConnection> connections = new List<UIConnection>();

            if (opr.connections == null)
                return connections;

            Dictionary<string, UIAdaptedOutputItem> adaptedOutputs = new Dictionary<string, UIAdaptedOutputItem>();

            UIInputItem target;

            UIModel uiSourceModel, uiTargetModel;
            UIConnection uiConnection;

            Dictionary<string, UIOutputItem> sources = new Dictionary<string, UIOutputItem>();
            Dictionary<string, UIInputItem> targets = new Dictionary<string, UIInputItem>();
            List<Link> links = new List<Link>();


            foreach (oprConnection connection in opr.connections)
            {
                if (!connection.source_model_indexSpecified || !connection.target_model_indexSpecified)
                {
                    throw new NotImplementedException("Incompletly specified connections");
                }

                uiSourceModel = models[connection.source_model_index];
                uiTargetModel = models[connection.target_model_index];

                uiConnection = new UIConnection(uiSourceModel, uiTargetModel);
                connections.Add(uiConnection);

                sources.Clear();
                targets.Clear();

                foreach (ITimeSpaceOutput item in uiSourceModel.LinkableComponent.Outputs)
                    sources.Add(item.Id, new UIOutputItem(item));

                foreach (ITimeSpaceInput item in uiTargetModel.LinkableComponent.Inputs)
                    targets.Add(item.Id, new UIInputItem(item));

                # region delete later
                //nDecorators =  connection.decorators != null ? connection.decorators.Length : 0;

                //ids = new string[nDecorators];
                //oprFactories = new oprConnectionDecoratorFactory[nDecorators];
                //oprArguments = new oprConnectionDecoratorArgument[nDecorators][];

                //for (int n = 0; n < nDecorators; ++n)
                //{
                //    ids[n] = connection.decorators[n].item_id;
                //    oprFactories[n] = connection.decorators[n].factory;
                //    oprArguments[n] = connection.decorators[n].arguments;
                //}

                #endregion

                links = new List<Link>();

                foreach (oprConnectionLink oprLink in connection.links)
                {
                   
                    target = targets[oprLink.target_item_id];

                    oprSourceItem srcItem = oprLink.source_item_id;
                    List<UIOutputItem> sourcesm = new List<UIOutputItem>();
                    UIOutputItem source = BuildOutputItemChain(uiSourceModel.LinkableComponent, srcItem, connection.decorators, sources, adaptedOutputs, oprPath, target);


                    foreach(oprSourceItem item in oprLink.source_item_ids)
                    {
                        UIOutputItem tempSrc = BuildOutputItemChain(uiSourceModel.LinkableComponent, item, connection.decorators, sources, adaptedOutputs, oprPath, target);
                        sourcesm.Add(tempSrc);
                    }



                    // Just telling target about most decorated source,
					// not the intermediates (& visa versa).
					// Probably the safest option re ensuring UI works
					// in maximum number of cases. 
					// Thats what the UIOuputItem.Parent attribute is for!
					// TODO ADH: However, might want
					// to make explicit in Standard?

                    if (target.ExchangeItem is IBaseMultiInput)
                    {
                        IBaseMultiInput mtarget = (IBaseMultiInput)target.ExchangeItem;
                        
                        foreach(UIOutputItem tempout in sourcesm)
                        {
                            mtarget.AddProvider(tempout);
                            tempout.AddConsumer(mtarget);
                        }
                    }
                    else
                    {
                        target.Provider = source;
                        source.AddConsumer(target);
                    }

                    Link tempLink = new Link(sourcesm, target);
                    //tempLink.Source = source;

					links.Add(tempLink);
                }

                uiConnection.Links = links;
            }

            return connections;
        }

        static UIOutputItem BuildOutputItemChain(IBaseLinkableComponent sourceComp, oprSourceItem sourceItem, oprConnectionDecorator[] decorators,
            Dictionary<string, UIOutputItem> sources, Dictionary<string, UIAdaptedOutputItem> existingAdaptedOutputs, DirectoryInfo oprPath , UIInputItem target)
        {
            UIOutputItem src = null;

            if(sourceItem.source_item_is_adaptedoutput)
            {
                UIOutputItem parent = null;
                oprSourceItem parentItem =sourceItem.parent_item_id;

                Debug.Assert(parentItem != null);

                if (existingAdaptedOutputs.ContainsKey(sourceItem.source_item_id))
                {
                    src = existingAdaptedOutputs[sourceItem.source_item_id];
                }
                else
                {
                    if (parentItem.source_item_is_adaptedoutput)
                    {
                        if (existingAdaptedOutputs.ContainsKey(parentItem.source_item_id))
                        {
                            parent = existingAdaptedOutputs[parentItem.source_item_id];
                        }
                        else
                        {
                            parent = BuildOutputItemChain(sourceComp, parentItem, decorators, sources, existingAdaptedOutputs, oprPath, target);

                        }
                    }
                    else
                    {
                        parent = sources[parentItem.source_item_id];
                    }

                    #region delete later

                    //if (srcItem.source_item_is_adaptedoutput)
                    //{
                    //    Debug.Assert(srcItem.source_item_adpatedoutput_index >= 0);

                    //    factory = new UIAdaptedFactory();

                    //    factory.Initialise(oprFactories[srcItem.source_item_adpatedoutput_index], uiSourceModel.LinkableComponent, oprPath);

                    //    adapted = factory.NewAdaptedUIOutputItem(ids[srcItem.source_item_adpatedoutput_index], source, target);

                    //    SetArguments(oprArguments[srcItem.source_item_adpatedoutput_index], (ITimeSpaceAdaptedOutput)adapted.Output);


                    //    source = adapted;

                    //}
                    //else
                    //{
                    //    source = sources[srcItem.source_item_id];
                    //}

                    //UIOutputItem currentSource = source;
                    //UIOutputItem parentSource = null;

                    //while(srcItem.parent_item_id != null)
                    //{
                    //    oprSourceItem parentItem = srcItem.parent_item_id;

                    //    if(parentItem.source_item_is_adaptedoutput)
                    //    {
                    //        Debug.Assert(parentItem.source_item_adpatedoutput_index >= 0);

                    //        factory = new UIAdaptedFactory();
                    //        factory.Initialise(oprFactories[parentItem.source_item_adpatedoutput_index], uiSourceModel.LinkableComponent, oprPath);
                    //        parentSource = factory.NewAdaptedUIOutputItem(ids[parentItem.source_item_adpatedoutput_index], source, target);
                    //        SetArguments(oprArguments[parentItem.source_item_adpatedoutput_index], (ITimeSpaceAdaptedOutput)parentSource.Output);


                    //    }
                    //    else
                    //    {
                    //        parentSource = sources[parentItem.source_item_id];
                    //    }

                    //    currentSource.Parent = parentSource;
                    //    srcItem = parentItem;
                    //}

                    #endregion

                    UIAdaptedFactory factory = new UIAdaptedFactory();
                    oprConnectionDecorator decorator = decorators[sourceItem.source_item_adpatedoutput_index];
                    factory.Initialise(decorator.factory, sourceComp, oprPath);
                    src = factory.NewAdaptedUIOutputItem(decorator.item_id, parent, target);
                    SetArguments(decorator.arguments, (ITimeSpaceAdaptedOutput)src.TimeSpaceOutput);
                    src.Parent = parent;
                    existingAdaptedOutputs.Add(src.Id, src as UIAdaptedOutputItem);
                }
            }
            else
            {
                src = sources[sourceItem.source_item_id];
            }

            return src;
        }

        static List<UIConnection> DeserializeConnectionsV1(guiComposition opr, List<UIModel> models)
        {
            throw new NotImplementedException();
        }
    }
}
