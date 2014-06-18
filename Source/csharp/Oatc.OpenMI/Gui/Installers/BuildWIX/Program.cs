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
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace BuildWIX
{
	struct Shortcut
	{
		public readonly string Name;
		public readonly string Description;
		public readonly FileInfo File;
		public readonly string RegistryKey;

		public Shortcut(string name, string description, FileInfo file, string registryKey)
		{
			Name = name;
			Description = description;
			File = file;
			RegistryKey = registryKey;
		}
	};

	struct Action
	{
		public readonly string Description;
		public readonly FileInfo TargetFile;
		public readonly FileInfo IconFile;
		public readonly string Extension;
		public readonly string Command;

		public Action(string description, FileInfo targetFile, FileInfo iconFile,
			string extension, string command)
		{
			Description = description;
			TargetFile = targetFile;
			IconFile = iconFile;
			Extension = extension;
			Command = command;
		}
	};


    class Program
    {
        static void Main(string[] args)
        {
            // User values

			Installer installer = new Installer(
				"OATC OpenMI Editor 2.0 Alpha X",
				"OpenMI Association Technical Committee",
				"2.0.0.0");

			DirectoryInfo dirInstaller = new DirectoryInfo(@"..\..\..\");
			DirectoryInfo dirBin = new DirectoryInfo(Path.Combine(
				dirInstaller.FullName, @"..\bin"));
			DirectoryInfo dirHelp = new DirectoryInfo(Path.Combine(
				dirInstaller.FullName, @"..\Help"));
			DirectoryInfo dirExamples = new DirectoryInfo(Path.Combine(
				dirInstaller.FullName, @"..\Help\examples"));
			DirectoryInfo dirExampleModels = new DirectoryInfo(Path.Combine(
				dirInstaller.FullName, @"..\Help\examples\models"));
			DirectoryInfo dirBitmaps = new DirectoryInfo(Path.Combine(
				dirInstaller.FullName, @".\Bitmaps"));

			FileInfo[] programFilesFiles = new FileInfo[] {
				new FileInfo(Path.Combine(dirBin.FullName,
					@"OmiEd.ico")), // ico files should be first in this array
				new FileInfo(Path.Combine(dirBin.FullName, 
					@"Oatc_OpenMI_ConfigurationEditor.exe")),
				new FileInfo(Path.Combine(dirBin.FullName,
					@"Oatc_OpenMI_Gui_Controls.dll")),
				new FileInfo(Path.Combine(dirBin.FullName,
					@"Oatc_OpenMI_Gui_Core.dll")),
				new FileInfo(Path.Combine(dirBin.FullName,
					@"Oatc.OpenMI.Sdk.dll")),
				new FileInfo(Path.Combine(dirBin.FullName,
					@"OpenMI.Standard2.dll")),
				new FileInfo(Path.Combine(dirBin.FullName,
					@"log4net.dll")),
				new FileInfo(Path.Combine(dirHelp.FullName,
					@"OpenMIEditorHelp_2_0.pdf")),
				new FileInfo(Path.Combine(dirHelp.FullName,
					@"OpenMIEditorHelp_2_0.chm")),
			};

			FileInfo[] desktopFolderFiles = new FileInfo[] {
				new FileInfo(Path.Combine(dirExamples.FullName, 
					@"SimpleCSharpRiver2_RiverReach1.opr")),
				new FileInfo(Path.Combine(dirExamples.FullName,
					@"SimpleCSharpRiver2_RiverReachs1and2.opr")), 
				new FileInfo(Path.Combine(dirExamples.FullName,
					@"SimpleCSharpRiver2_Decorators01.opr")),
				new FileInfo(Path.Combine(dirExamples.FullName,
					@"SimpleCSharpRiver2_Decorators02.opr")),
			};

			FileInfo[] desktopFolderSubFiles = new FileInfo[] {
				new FileInfo(Path.Combine(dirExampleModels.FullName, 
					@"Oatc.OpenMI.Sdk.dll")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"Oatc_OpenMI_Examples_SimpleCSharpRiver2.exe")), 
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"Oatc_OpenMI_Sdk_ModelWrapper2.dll")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"OpenMI.Standard2.dll")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"SimpleCSharpRiver2_BoundaryConditions1.txt")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"SimpleCSharpRiver2_RiverReach1.omi")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"SimpleCSharpRiver2_RiverReach1.txt")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"SimpleCSharpRiver2_RiverReach2.omi")),
				new FileInfo(Path.Combine(dirExampleModels.FullName,
					@"SimpleCSharpRiver2_RiverReach2.txt")),
			};

			Shortcut[] shortcuts = new Shortcut[] {
				new Shortcut(
					"Editor", 
					"OATC OpenMI Editor",
					new FileInfo(Path.Combine(dirBin.FullName,
						@"Oatc_OpenMI_ConfigurationEditor.exe")),
					@"Software\OpenMI\OATC_Editor"),
				new Shortcut(
					"Help (PDF)", 
					"Help (PDF)",
					new FileInfo(Path.Combine(dirHelp.FullName,
						@"OpenMIEditorHelp_2_0.pdf")),
					@"Software\OpenMI\OATC_Editor"),
				new Shortcut(
					"Help (CHM)", 
					"Help (CHM)",
					new FileInfo(Path.Combine(dirHelp.FullName,
						@"OpenMIEditorHelp_2_0.chm")),
					@"Software\OpenMI\OATC_Editor"),
			};

			Action[] actions = new Action[] {
				new Action(
					"OpenMI OPR composition file",
						new FileInfo(Path.Combine(dirBin.FullName,
					@"Oatc_OpenMI_ConfigurationEditor.exe")),
					new FileInfo(Path.Combine(dirBin.FullName,
						@"OmiEd.ico")),
					"opr",
					"-opr \"%1\""),
					/*
				new Action(
					"OpenMI OMI Model file",
						new FileInfo(Path.Combine(dirBin.FullName,
					@"Oatc_OpenMI_ConfigurationEditor.exe")),
					new FileInfo(Path.Combine(dirBin.FullName,
						@"OmiEd.ico")),
					"omi",
					"-omi \"%1\""),
					 */
			};

            // Create WIX XML file

			installer.AddProgramFilesFiles("Editor", programFilesFiles);
			installer.AddDesktopFolderFiles("Examples", desktopFolderFiles);
			installer.AddDesktopFolderSubFiles("models", desktopFolderSubFiles);

			installer.AddShortcuts(shortcuts);
			installer.AddActions(actions);

			installer.AddLicenceFile(new FileInfo(Path.Combine(
                dirInstaller.FullName, @"License.rtf")));

			installer.AddBitmapFiles(
				new FileInfo(Path.Combine(dirBitmaps.FullName, @"bannrbmp.bmp")),
				new FileInfo(Path.Combine(dirBitmaps.FullName, @"dlgbmp.bmp")));

			installer.WriteWix(new FileInfo(@"../../Editor.wxs"));
               
            Console.WriteLine("Press return to exit");
            Console.ReadLine();
        }

		class Installer
		{	
			String _productName;
			string _manufacturer;
			string _version;
			FileInfo[] _programFilesFiles;
			FileInfo[] _desktopFolderFiles;
			Dictionary<string, FileInfo[]> _desktopFolderSubFiles;
			FileInfo _licenceFile;
			FileInfo _bannrbmp;
			FileInfo _dlgbmp;
			Shortcut[] _shortcuts;
			Dictionary<string, Action> _actions;
			int _uniqueInt = 0;
			Dictionary<string, string> _filenameFileId
				= new Dictionary<string, string>();
			Dictionary<string, string> _filenameComponentId
				= new Dictionary<string, string>();

			List<XmlElement> _programFilesComponents 
				= new List<XmlElement>();
			List<XmlElement> _desktopFilesComponents 
				= new List<XmlElement>();

			string _programFilesFeatureName;
			string _desktopFilesFeatureName;

			string _xmlns = @"http://schemas.microsoft.com/wix/2006/wi";

			XmlDocument _doc;
			XmlElement _wix;
			XmlElement _product;

			public Installer(String productName, string manufacturer, string version)
			{
				_productName = productName;
				_manufacturer = manufacturer;
				_version = version;
			}

			public void AddProgramFilesFiles(string featureName, FileInfo[] files)
			{
				_programFilesFeatureName = featureName;
				_programFilesFiles = files;

				foreach (FileInfo f in _programFilesFiles)
					if (!f.Exists)
						throw new FileNotFoundException(f.FullName);
			}

			public void AddDesktopFolderFiles(string featureName, FileInfo[] files)
			{
				_desktopFilesFeatureName = featureName;

				_desktopFolderFiles = files;

				foreach (FileInfo f in _desktopFolderFiles)
					if (!f.Exists)
						throw new FileNotFoundException(f.FullName);
			}

			public void AddDesktopFolderSubFiles(string subFolderName, FileInfo[] files)
			{
				if (_desktopFolderSubFiles == null)
					_desktopFolderSubFiles = new Dictionary<string, FileInfo[]>(1);

				_desktopFolderSubFiles.Add(subFolderName, files);

				foreach (FileInfo f in _desktopFolderSubFiles[subFolderName])
					if (!f.Exists)
						throw new FileNotFoundException(f.FullName);
			}

			public void AddShortcuts(Shortcut[] shortcuts)
			{
				_shortcuts = shortcuts;
			}

			public void AddActions(Action[] actions)
			{
				_actions = new Dictionary<string, Action>(actions.Length);

				foreach (Action action in actions)
					_actions.Add(action.TargetFile.FullName, action);
			}

			public void AddLicenceFile(FileInfo file)
			{
				_licenceFile = file;
			}

			public void AddBitmapFiles(FileInfo bannrbmp, FileInfo dlgbmp)
			{
				_bannrbmp = bannrbmp;
				_dlgbmp = dlgbmp;
			}

			public void WriteWix(FileInfo xmlFile)
			{
				_doc = new XmlDocument();
				_wix = _doc.CreateElement("Wix", _xmlns);
				_product = Product();

				_doc.AppendChild(_wix);
				_wix.AppendChild(_product);

				_product.AppendChild(Package());
				_product.AppendChild(Media());

				XmlElement dirTarget = Directory("TARGETDIR", "SourceDir", _product);
				XmlElement dirProgFiles = Directory("ProgramFilesFolder", null, dirTarget);
				XmlElement dirInstall = Directory("PROGRAMFILELOCATION", _productName, dirProgFiles);

				_programFilesComponents.AddRange(
					AddFileComponents(_programFilesFiles, dirInstall));

				if (_desktopFolderFiles != null 
					&& _desktopFolderFiles.Length > 0)
				{
					XmlElement dirDesktop = Directory("DesktopFolder", null, dirTarget);
					XmlElement dirDesktopFolder = Directory("DESKTOPLOCATION", _productName, dirDesktop);
					
					_desktopFilesComponents.AddRange(
						AddFileComponents2(_desktopFolderFiles, dirDesktopFolder));

					if (_desktopFolderSubFiles != null)
					{
						foreach (KeyValuePair<string, FileInfo[]> kv in _desktopFolderSubFiles)
						{
							XmlElement subFolder = Directory(UniqueId(), kv.Key, dirDesktopFolder);
							_desktopFilesComponents.AddRange(
								AddFileComponents2(kv.Value, subFolder));
						}
					}
				}

				XmlElement dirMenuPrograms = Directory("ProgramMenuFolder", "PMFolder", dirTarget);
				XmlElement dirMenuProgramsDir = Directory("ApplicationProgramsFolder", _productName, dirMenuPrograms);

				XmlElement dirRefMenuProgramsDir = DirectoryRef("ApplicationProgramsFolder");

				_product.AppendChild(dirRefMenuProgramsDir);

				XmlElement featureProgramFiles = Feature("FeatureProgramFiles",
					_programFilesFeatureName, "PROGRAMFILELOCATION");
				
				foreach (Shortcut shortcut in _shortcuts)
				{
					XmlElement shortcutComponent = Component(UniqueId());
					XmlElement cut = Shortcut(shortcut,
						"ApplicationProgramsFolder",
						"PROGRAMFILELOCATION");

					shortcutComponent.AppendChild(cut);
					shortcutComponent.AppendChild(RemoveFolder());
					shortcutComponent.AppendChild(RegistryValueHKCU(shortcut.RegistryKey));

					dirRefMenuProgramsDir.AppendChild(shortcutComponent);

					featureProgramFiles.AppendChild(ComponentRef(shortcutComponent.Attributes["Id"].Value));
				}

				foreach (XmlElement c in _programFilesComponents)
					featureProgramFiles.AppendChild(ComponentRef(
						c.Attributes["Id"].Value));
				
				_product.AppendChild(featureProgramFiles);

				if (_desktopFilesComponents != null && _programFilesFeatureName.Length > 0)
				{
					XmlElement featureDesktopFiles = Feature("FeatureDesktopFiles",
						_desktopFilesFeatureName, "DESKTOPLOCATION");
					
					foreach (XmlElement c in _desktopFilesComponents)
						featureDesktopFiles.AppendChild(ComponentRef(
							c.Attributes["Id"].Value));

					_product.AppendChild(featureDesktopFiles);
				}

				if (_licenceFile != null && _licenceFile.Exists)
					_product.AppendChild(WixVariable(
						"WixUILicenseRtf", _licenceFile.FullName));

				if (_bannrbmp != null && _bannrbmp.Exists)
					_product.AppendChild(WixVariable(
						"WixUIBannerBmp", _bannrbmp.FullName));

				if (_dlgbmp != null && _dlgbmp.Exists)
					_product.AppendChild(WixVariable(
						"WixUIDialogBmp", _dlgbmp.FullName));

				_product.AppendChild(ConditionNetFramwork());

				_product.AppendChild(UIRef("WixUI_FeatureTree"));
				_product.AppendChild(UIRef("WixUI_ErrorProgressText"));

				// Write XML file

				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.NewLineOnAttributes = true;

				Console.WriteLine("Creating " + xmlFile.FullName);

				using (XmlWriter xwriter = XmlWriter.Create(xmlFile.FullName, settings))
				{
					_doc.WriteTo(xwriter);
				}
			}

			static bool PathChar(char c)
			{
				return c == '.'
					|| c == ':'
					|| c == Path.DirectorySeparatorChar;		
			}

			string ComponentId(FileInfo file)
			{
				string id = UniqueId();
				_filenameComponentId.Add(file.FullName, id);
				return id;
			}

			string FileId(FileInfo file)
			{
				string id = UniqueId();
				_filenameFileId.Add(file.FullName, id);
				return id;
			}

			XmlElement Feature(string Id, string title, string configurableDirectory)
			{
				XmlElement xml = _doc.CreateElement("Feature", _xmlns);
				xml.SetAttribute("Id", Id);
				xml.SetAttribute("Title", title);
				xml.SetAttribute("Level", "1");
				xml.SetAttribute("Display", "expand");
				xml.SetAttribute("ConfigurableDirectory", configurableDirectory);
				return xml;
			}

			XmlElement RemoveFolder()
			{
				XmlElement xml = _doc.CreateElement("RemoveFolder", _xmlns);
				xml.SetAttribute("Id", UniqueId());
				xml.SetAttribute("On", "uninstall");
				return xml;
			}

			XmlElement RegistryValueHKCU(string key)
			{
				XmlElement value = _doc.CreateElement("RegistryValue", _xmlns);
				value.SetAttribute("Root", "HKCU");
				value.SetAttribute("Key", key);
				value.SetAttribute("Name", "installed");
				value.SetAttribute("Type", "integer");
				value.SetAttribute("Value", "1");
				value.SetAttribute("KeyPath", "yes");
				return value;
			}

			XmlElement DirectoryRef(string id)
			{
				XmlElement directory = _doc.CreateElement("DirectoryRef", _xmlns);
				directory.SetAttribute("Id", id);
				return directory;
			}

			XmlElement ComponentRef(string id)
			{
				XmlElement c = _doc.CreateElement("ComponentRef", _xmlns);
				c.SetAttribute("Id", id);
				return c;
			}

			XmlElement Directory(string id, string name, XmlElement parent)
			{
				XmlElement directory = _doc.CreateElement("Directory", _xmlns);
				directory.SetAttribute("Id", id);
				if (name != null)
					directory.SetAttribute("Name", name);
				parent.AppendChild(directory);
				return directory;
			}

			XmlElement Product()
			{
				XmlElement product = _doc.CreateElement("Product", _xmlns);
				product.SetAttribute("Id", Guid.NewGuid().ToString());
				product.SetAttribute("Name", _productName);
				product.SetAttribute("Language", "1033");
				product.SetAttribute("Version", _version);
				product.SetAttribute("Manufacturer", _manufacturer);
				product.SetAttribute("UpgradeCode", Guid.NewGuid().ToString());
				return product;
			}

			XmlElement Package()
			{
				XmlElement package = _doc.CreateElement("Package", _xmlns);
				package.SetAttribute("Description", _productName);
				package.SetAttribute("Comments", _manufacturer);
				package.SetAttribute("InstallerVersion", "200");
				package.SetAttribute("Compressed", "yes");
				return package;
			}

			XmlElement Media()
			{
				XmlElement media = _doc.CreateElement("Media", _xmlns);
				media.SetAttribute("Id", "1");
				media.SetAttribute("Cabinet", "Product.cab");
				media.SetAttribute("EmbedCab", "yes");
				return media;
			}

			XmlElement FileComponent(FileInfo file)
			{
				XmlElement xfile = _doc.CreateElement("File", _xmlns);
				xfile.SetAttribute("Id", FileId(file));
				xfile.SetAttribute("Name", file.Name);
				xfile.SetAttribute("Source", file.FullName);

				XmlElement component = Component(ComponentId(file));
				component.AppendChild(xfile);
				return component;
			}
	
			XmlElement Component(string id)
			{
				XmlElement component = _doc.CreateElement("Component", _xmlns);
				component.SetAttribute("Id", id);
				component.SetAttribute("Guid", Guid.NewGuid().ToString());
				component.SetAttribute("DiskId", "1");
				return component;
			}

			string UniqueId()
			{
				return "i" + ++_uniqueInt;
			}

			XmlElement Shortcut(Shortcut shortcut, string directory, string workingDirectory)
			{
				XmlElement s = _doc.CreateElement("Shortcut", _xmlns);
				s.SetAttribute("Id", UniqueId());
				s.SetAttribute("Name", shortcut.Name);
				s.SetAttribute("Description", shortcut.Description);
				s.SetAttribute("Target", string.Format("[{0}][!{1}]",
					workingDirectory,
					_filenameFileId[shortcut.File.FullName]));					
				s.SetAttribute("Directory", directory);
				s.SetAttribute("WorkingDirectory", workingDirectory);
				return s;
			}

			XmlElement[] AddFileComponents(FileInfo[] files, XmlElement dir)
			{
				XmlElement[] components = new XmlElement[files.Length];

				for (int n = 0; n < files.Length; ++n)
				{
					components[n] = FileComponent(files[n]);

					if (_actions.ContainsKey(files[n].FullName))
						components[n].AppendChild(ProgId(_actions[files[n].FullName]));

					if (n == 0)
					{
						components[n].AppendChild(
							RegistryDirectoryKeyPathHKCU(@"Software\OpenMI\OATC_Editor"));

						List<KeyValuePair<string, string>> values
							= new List<KeyValuePair<string,string>>();
						values.Add(new KeyValuePair<string, string>(
							"Install_Location", "[PROGRAMFILELOCATION]"));
						values.Add(new KeyValuePair<string, string>(
							"Examples_Location", "[DESKTOPLOCATION]"));

						components[n].AppendChild(
							RegistryValueHKCU(@"Software\OpenMI\OATC_Editor", values));
					}

					dir.AppendChild(components[n]);
				}

				return components;
			}

			XmlElement[] AddFileComponents2(FileInfo[] files, XmlElement dir)
			{
				XmlElement[] components = new XmlElement[files.Length];

				for (int n = 0; n < files.Length; ++n)
				{
					components[n] = FileComponent(files[n]);

					components[n].AppendChild(
						RegistryDirectoryKeyPathHKCU(@"Software\OpenMI\OATC_Editor"));

					components[n].AppendChild(
						RemoveFolder());

					if (_actions.ContainsKey(files[n].FullName))
						components[n].AppendChild(ProgId(_actions[files[n].FullName]));

					dir.AppendChild(components[n]);
				}

				return components;
			}

			XmlElement RegistryDirectoryKeyPathHKCU(string key)
			{
				XmlElement k = _doc.CreateElement("RegistryKey", _xmlns);
				k.SetAttribute("Id", UniqueId());
				k.SetAttribute("Root", "HKCU");
				k.SetAttribute("Key", key);
				k.SetAttribute("Action", "createAndRemoveOnUninstall");

				XmlElement v = _doc.CreateElement("RegistryValue", _xmlns);
				v.SetAttribute("Name", UniqueId());
				v.SetAttribute("Value", "1");
				v.SetAttribute("Type", "integer");
				v.SetAttribute("KeyPath", "yes");

				k.AppendChild(v);

				return k;
			}

			XmlElement RegistryValueHKCU(string key, List<KeyValuePair<string, string>> values)
			{
				XmlElement k = _doc.CreateElement("RegistryKey", _xmlns);
				k.SetAttribute("Id", UniqueId());
				k.SetAttribute("Root", "HKCU");
				k.SetAttribute("Key", key);
				k.SetAttribute("Action", "createAndRemoveOnUninstall");

				XmlElement v;

				foreach (KeyValuePair<string, string> kv in values)
				{
					v = _doc.CreateElement("RegistryValue", _xmlns);
					v.SetAttribute("Id", UniqueId());
					v.SetAttribute("Name", kv.Key);
					v.SetAttribute("Value", kv.Value);
					v.SetAttribute("Type", "string");

					k.AppendChild(v);
				}

				return k;
			}
 
			XmlElement ProgId(Action action)
			{
				XmlElement progId = _doc.CreateElement("ProgId", _xmlns);
				progId.SetAttribute("Id", UniqueId());
				progId.SetAttribute("Description", action.Description);
				if (action.IconFile != null)
					progId.SetAttribute("Icon", _filenameFileId[action.IconFile.FullName]);

				XmlElement extension = _doc.CreateElement("Extension", _xmlns);
				extension.SetAttribute("Id", action.Extension);
				extension.SetAttribute("ContentType", "application/xyz");
				progId.AppendChild(extension);

				XmlElement verb = _doc.CreateElement("Verb", _xmlns);
				verb.SetAttribute("Id", "open");
				verb.SetAttribute("Command", "Open");
				verb.SetAttribute("TargetFile", _filenameFileId[action.TargetFile.FullName]);
				verb.SetAttribute("Argument", action.Command);
				extension.AppendChild(verb);
				return progId;
			}

			XmlElement WixVariable(string id, string value)
			{
				XmlElement var = _doc.CreateElement("WixVariable", _xmlns);
				var.SetAttribute("Id", id);
				var.SetAttribute("Value", value);
				return var;
			}

			XmlElement UIRef(string id)
			{
				XmlElement uiref = _doc.CreateElement("UIRef", _xmlns);
				uiref.SetAttribute("Id", id);
				return uiref;
			}

			XmlElement ConditionNetFramwork()
			{
				XmlElement condition = _doc.CreateElement("Condition", _xmlns);
				condition.SetAttribute("Message",
					"This setup requires the .NET Framework 2.0 or higher.");

				XmlCDataSection cdata = _doc.CreateCDataSection(
					"MsiNetAssemblySupport >= \"2.0.50727\"");
				condition.AppendChild(cdata);

				return condition;
			}
		}
    }
}
