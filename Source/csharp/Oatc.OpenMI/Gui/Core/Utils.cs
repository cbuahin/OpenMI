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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Reflection;

using Microsoft.Win32;

using OpenMI.Standard2;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// Class contains support methods.
	/// </summary>
	public class Utils
	{
        static AssemblyLoader _assemblyLoader = null;

		/// <summary>
		/// Determines whether two dimensions are equal.
		/// </summary>
		/// <param name="dimension1">Dimension one</param>
		/// <param name="dimension2">Dimension two</param>
		/// <returns>Returns <c>true</c> if powers of all dimension bases are same, otherwise returns <c>false</c>.</returns>
		public static bool CompareDimensions(IDimension dimension1, IDimension dimension2)
		{
			for (int i = 0; i < (int)Enum.GetValues(typeof(DimensionBase)).Length; i++)			
				if (dimension1.GetPower((DimensionBase) i) != dimension2.GetPower((DimensionBase) i))				
					return( false );
				
			return( true );
		}


		/// <summary>
		/// Gets <c>FileInfo</c> of file specified by it's (eventually relative) path.
		/// </summary>
		/// <param name="relativeDir">Directory <c>filename</c> is relative to, or <c>null</c> if <c>filename</c> is absolute path or relative path to current directory.</param>
		/// <param name="filename">Relative or absolute path to file.</param>
		/// <returns>Returns <c>FileInfo</c> of file specified.</returns>
		public static FileInfo GetFileInfo( string relativeDir, string filename )
		{
			string oldDirectory=null;
			if( relativeDir!=null )
				oldDirectory = Directory.GetCurrentDirectory();

			try 
			{
				if( relativeDir!=null )
					Directory.SetCurrentDirectory( relativeDir );
				return( new FileInfo(filename) );
			}
			finally
			{
				if( oldDirectory!=null )
					Directory.SetCurrentDirectory( oldDirectory );
			}
		}

		private const string regOprExtension = ".opr";
		private const string regOprDescription = "OmiEd project";
		private const string regOprIdentifier = "OmiEdProject";

		private const string regOmiExtension = ".omi";
		private const string regOmiDescription = "OpenMI model";
		private const string regOmiIdentifier = "OpenMIModel";


		/// <summary>
		/// Registers OPR and OMI file extension in Win32 registry to be opened with specific OmiEd application.
		/// </summary>
		/// <param name="applicationPath">Full path to specific OmiEd application executable.</param>
		public static void RegisterFileExtensions( string applicationPath )
		{
			RegistryKey keyExtension, keyDefaultIcon, keyIdentifier,
				subKeyShell, subKeyOpen, subKeyCommand;
			
			// OPR extension
			keyExtension = Registry.ClassesRoot.CreateSubKey(regOprExtension);
			keyExtension.SetValue(null, regOprIdentifier);
			keyExtension.Close();

			// OPR description
			keyIdentifier = Registry.ClassesRoot.CreateSubKey(regOprIdentifier);
			keyIdentifier.SetValue(null, regOprDescription);				

			// OPR default icon
			keyDefaultIcon = keyIdentifier.CreateSubKey("Defaulticon");
			keyDefaultIcon.SetValue(null, applicationPath + ",0");
			keyDefaultIcon.Close();

			// OPR open shell command
			subKeyShell = keyIdentifier.CreateSubKey("shell");				
			subKeyOpen = subKeyShell.CreateSubKey("open");				
			subKeyCommand = subKeyOpen.CreateSubKey("command");				
			subKeyCommand.SetValue(null, "\"" + applicationPath + "\" /opr \"%1\"");
			subKeyShell.Close();
			subKeyCommand.Close();
			subKeyOpen.Close();
				
			keyIdentifier.Close();


			// OMI extension
			keyExtension = Registry.ClassesRoot.CreateSubKey(regOmiExtension);
			keyExtension.SetValue(null, regOmiIdentifier);
			keyExtension.Close();

			// OMI description
			keyIdentifier = Registry.ClassesRoot.CreateSubKey(regOmiIdentifier);
			keyIdentifier.SetValue(null, regOmiDescription);				

			// OMI default icon
			keyDefaultIcon = keyIdentifier.CreateSubKey("Defaulticon");
			keyDefaultIcon.SetValue(null, applicationPath + ",0");
			keyDefaultIcon.Close();

			// OMI open shell command
			subKeyShell = keyIdentifier.CreateSubKey("shell");				
			subKeyOpen = subKeyShell.CreateSubKey("open");				
			subKeyCommand = subKeyOpen.CreateSubKey("command");				
			subKeyCommand.SetValue(null, "\"" + applicationPath + "\" /omi \"%1\"");
			subKeyShell.Close();
			subKeyCommand.Close();
			subKeyOpen.Close();
				
			keyIdentifier.Close();

			Registry.ClassesRoot.Flush();
		}

		/// <summary>
		/// Determines whether OPR and OMI file extension are registered in Win32 registry
		/// to be opened with specific OmiEd application.
		/// </summary>
		/// <param name="applicationPath">Path to specific OmiEd application executable.</param>
		/// <returns>If OPR and OMI extensions are correctly registered, returns <c>true</c>,
		/// otherwise returns <c>false</c>.</returns>
		public static bool AreFileExtensionsRegistered(string applicationPath)
		{
			RegistryKey keyExtension, keyDefaultIcon, keyIdentifier,
				subKeyShell, subKeyOpen, subKeyCommand;

			// OPR extension
			keyExtension = Registry.ClassesRoot.OpenSubKey(regOprExtension);
			if( keyExtension==null )
				return( false );
			if( (string)keyExtension.GetValue(null) != regOprIdentifier )
				return( false );

			// OPR description
			keyIdentifier = Registry.ClassesRoot.OpenSubKey(regOprIdentifier);
			if( keyIdentifier==null )
				return( false );
			if( (string)keyIdentifier.GetValue(null) != regOprDescription )
				return( false );			

			// OPR default icon
			keyDefaultIcon = keyIdentifier.OpenSubKey("Defaulticon");
			if( keyDefaultIcon==null )
				return( false );
			if( (string)keyDefaultIcon.GetValue(null) != applicationPath + ",0" )
				return( false );

			// OPR open shell command
			subKeyShell = keyIdentifier.OpenSubKey("shell");
			if( subKeyShell==null )
				return( false );
	
			subKeyOpen = subKeyShell.OpenSubKey("open");
			if( subKeyOpen==null )
				return( false );
	
			subKeyCommand = subKeyOpen.OpenSubKey("command");
			if( subKeyCommand==null )
				return( false );
			if( (string)subKeyCommand.GetValue(null) != "\"" + applicationPath + "\" /opr \"%1\"" )
				return( false );


			// OMI extension
			keyExtension = Registry.ClassesRoot.OpenSubKey(regOmiExtension);
			if( keyExtension==null )
				return( false );
			if( (string)keyExtension.GetValue(null) != regOmiIdentifier )
				return( false );

			// OMI description
			keyIdentifier = Registry.ClassesRoot.OpenSubKey(regOmiIdentifier);
			if( keyIdentifier==null )
				return( false );
			if( (string)keyIdentifier.GetValue(null) != regOmiDescription )
				return( false );			

			// OMI default icon
			keyDefaultIcon = keyIdentifier.OpenSubKey("Defaulticon");
			if( keyDefaultIcon==null )
				return( false );
			if( (string)keyDefaultIcon.GetValue(null) != applicationPath + ",0" )
				return( false );

			// OMI open shell command
			subKeyShell = keyIdentifier.OpenSubKey("shell");
			if( subKeyShell==null )
				return( false );
	
			subKeyOpen = subKeyShell.OpenSubKey("open");
			if( subKeyOpen==null )
				return( false );
	
			subKeyCommand = subKeyOpen.OpenSubKey("command");
			if( subKeyCommand==null )
				return( false );
			if( (string)subKeyCommand.GetValue(null) != "\"" + applicationPath + "\" /omi \"%1\"" )
				return( false );

			return( true );
		}


		/// <summary>
		/// Discards any OPR and OMI file extension registration from Win32 registry.
		/// </summary>
		public static void UnregisterFileExtensions( )
		{			
			Registry.ClassesRoot.DeleteSubKeyTree( regOprExtension );
			Registry.ClassesRoot.DeleteSubKeyTree( regOprIdentifier );
			Registry.ClassesRoot.DeleteSubKeyTree( regOmiExtension );
			Registry.ClassesRoot.DeleteSubKeyTree( regOmiIdentifier );
		}

		public static ITimeSpaceComponent OmiDeserializeAndInitialize(FileInfo omiFile)
		{
			if (!omiFile.Exists)
				throw new FileNotFoundException(omiFile.FullName);

			using (StreamReader sr = new StreamReader(omiFile.FullName))
				return OmiDeserializeAndInitialize(sr.ReadToEnd(), omiFile.Directory);
		}

		public static ITimeSpaceComponent OmiDeserializeAndInitialize(string xml, DirectoryInfo omiPath)
		{
			XmlSerializer serializer =
				new XmlSerializer(typeof(LinkableComponentComplexType));

			// TODO: Validate against xsd first?

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;
			settings.IgnoreProcessingInstructions = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;

           // LinkableComponentComplexType comp = new LinkableComponentComplexType();


           // serializer.Serialize(new StreamWriter("C:\\Users\\Caleb\\Desktop\\Projects\\OpenMI\\1.4.0\\Oatc\\src\\csharp\\Examples\\ModelComponents\\SpatialModels\\UnitTest\\test.xml"), comp);
            //try
            //{
				using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
				{
					LinkableComponentComplexType omi = 
						(LinkableComponentComplexType)serializer.Deserialize(reader);
					return OmiDeserializeAndInitialize(omi, omiPath);
				}
            //}
            //catch (Exception e)
            //{
            //    // TODO: Do we want to allow unspecified xmlns?
            //    if (e.InnerException != null
            //        && e.InnerException.Message.Contains("<LinkableComponent xmlns="))
            //        throw new XmlException("xmlns=\"http://www.openmi.org\" missing in <LinkableComponent />", e);

            //    throw;
            //}
		}

		public static ITimeSpaceComponent OmiDeserializeAndInitialize(LinkableComponentComplexType omi, DirectoryInfo relativePath)
		{
            if (_assemblyLoader == null)
                _assemblyLoader = new AssemblyLoader();

            object obj = _assemblyLoader.Load(omi.Assembly, omi.Type, relativePath);

			if (!(obj is ITimeSpaceComponent))
				throw new InvalidCastException("ILinkableComponent");

			ITimeSpaceComponent iLC = (ITimeSpaceComponent)obj;

			Dictionary<string, IArgument> args 
				= new Dictionary<string,IArgument>();

			if (iLC.Arguments != null)
				foreach (IArgument iArg in iLC.Arguments)
					args.Add(iArg.Id, iArg);

			if (omi.Arguments != null)
			{
				foreach (LinkableComponentComplexTypeArgument omiArg in omi.Arguments)
				{
                    if (args.ContainsKey(omiArg.Key))
                    {
                        if (args[omiArg.Key].ValueType == typeof(FileInfo)
                            && !Path.IsPathRooted(omiArg.Value))
                        {
                            omiArg.Value = Path.Combine(relativePath.FullName, omiArg.Value);
                        }
                        else if (args[omiArg.Key].ValueType == typeof(DirectoryInfo)
                            && !Path.IsPathRooted(omiArg.Value))
                        {
                            omiArg.Value = Path.Combine(relativePath.FullName, omiArg.Value);
                        }

                        args[omiArg.Key].ValueAsString = omiArg.Value;
                    }
                    else
                    {// add anyway as string
                        args.Add(omiArg.Key, new ArgumentString(omiArg.Key, omiArg.Value));
                    }
				}
			}

			// Suspect this could come in handy, so added
			// args.Add("OMI_PATH", new ArgumentDirectoryInfo("OMI_PATH", relativePath));

			iLC.Initialize(new List<IArgument>(args.Values));

			return iLC;
		}

		static public string ToString(Exception e)
		{
			StringBuilder sb = new StringBuilder();

			Exception ex = e;

			while (ex != null)
			{
                sb.AppendLine(ex.GetType().ToString());
                sb.AppendLine(ex.Message);
				ex = ex.InnerException;
			}

			if (e.StackTrace != null)
			{
				sb.AppendLine("");
				sb.AppendLine(e.StackTrace);
			}

			return sb.ToString();
		}

		static public string RelativePath(DirectoryInfo relativeTo, string pathIn)
		{
			try
			{
				DirectoryInfo path = new DirectoryInfo(pathIn);

				// Create a list of the path name directories for the relativeTo and pathIn.
				List<string> r = new List<string>(relativeTo.FullName.Split(Path.DirectorySeparatorChar));
				List<string> p = new List<string>(path.FullName.Split(Path.DirectorySeparatorChar));

				// Reverse the lists.
				r.Reverse();
				p.Reverse();

				// Turn the lists into a stack.
				Stack<string> rr = new Stack<string>(r);
				Stack<string> pp = new Stack<string>(p);

				// Reduce the common items in the two lists.
				while (rr.Count > 0 && pp.Count > 0 && rr.Peek().ToLower() == pp.Peek().ToLower())
				{
					rr.Pop();
					pp.Pop();
				}

				// If both are reduced to zero items then both are the same path?
				if (rr.Count == 0 && pp.Count == 0)
					return ".";

				if (pp.Count != 0 && pp.Peek().Contains(":"))
					return pathIn; // Still rooted

				for (int n = 0; n < rr.Count; ++n)
					pp.Push("..");

				StringBuilder sb = new StringBuilder(pp.Pop());

				while (pp.Count > 0)
					sb.AppendFormat("{0}{1}", Path.DirectorySeparatorChar, pp.Pop());

				return sb.ToString();
			}
			catch
			{
			}

			return pathIn;
		}

		public static object Parse(Type type, string parse)
		{
			foreach (MethodInfo mi in type.GetMethods())
			{
				if (mi.Name == "Parse"
					&& mi.IsStatic
					&& mi.GetParameters().Length == 1
					&& mi.GetParameters()[0].ParameterType == typeof(string))
				{
					return mi.Invoke(null, new object[] { parse });
				}
			}
				
			throw new NotImplementedException(
				string.Format("Reflection could not find a {0}.Parse(string) method to invoke",
					type.FullName));
		}

		public static FileInfo GetAbsoluteOmiFile(string omiFile, DirectoryInfo oprPath)
		{
			if (Path.IsPathRooted(omiFile) || oprPath == null)
				return new FileInfo(omiFile);

			return new FileInfo(Path.Combine(oprPath.FullName, omiFile));
		}

		#region Workarround to handle a bug from Microsoft
		//  A bug, see http://dturini.blogspot.com/2004_08_01_dturini_archive.html
		//  or  http://support.microsoft.com/default.aspx?scid=KB;EN-US;q326219#appliesto
		[DllImport("msvcr71.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int _controlfp(int n, int mask);
		const int _RC_NEAR       = 0x00000000;
		const int _PC_53         = 0x00010000;
		const int _EM_INVALID    = 0x00000010;
		const int _EM_UNDERFLOW  = 0x00000002;
		const int _EM_ZERODIVIDE = 0x00000008;
		const int _EM_OVERFLOW   = 0x00000004;
		const int _EM_INEXACT    = 0x00000001;
		const int _EM_DENORMAL   = 0x00080000;
		const int _CW_DEFAULT    = (  _RC_NEAR + _PC_53 
			+ _EM_INVALID 
			+ _EM_ZERODIVIDE
			+ _EM_OVERFLOW 
			+ _EM_UNDERFLOW
			+ _EM_INEXACT
			+ _EM_DENORMAL);
    
		/// <summary>
		/// Resets floating point unit (FPU).
		/// </summary>
		public static void ResetFPU()
		{ 
			_controlfp(_CW_DEFAULT ,0xfffff);
		} 

		#endregion
	}
}
