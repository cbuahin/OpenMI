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
using System.Collections;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Oatc.OpenMI.Gui.Core
{
    public class AssemblyLoader
    {
        List<string> _potentialAssemblyFolders = null;
        List<string> _potentialAssemblyExtensions = null;
        FileInfo _assemblyFile;

        /* TODO ADH
         * allow user to define dll paths from omi arguments
         */

        public AssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += FindAssemblies;
        }

        public object Load(string assemblyPath, string typename, DirectoryInfo relativePath)
        {
            string file = assemblyPath;

            if (!Path.IsPathRooted(file))
                file = Path.Combine(relativePath.FullName, assemblyPath);

            _assemblyFile = new FileInfo(file);

            if (!_assemblyFile.Exists)
                throw new FileNotFoundException(_assemblyFile.FullName);

            Assembly ass = System.Reflection.Assembly.LoadFile(_assemblyFile.FullName);

            if (ass == null)
                throw new ArgumentException("Cannot load assembly " + _assemblyFile.FullName);

            try
            {
                Type[] types = ass.GetTypes();
            }
            catch (ReflectionTypeLoadException etl)
            {
                if (etl.LoaderExceptions != null)
                {
                    StringBuilder sb = new StringBuilder("\r\nReflectionTypeLoadException.LoaderExceptions");

                    foreach (Exception e in etl.LoaderExceptions)
                        sb.AppendLine("\r\n\t\t" + e.Message);

                    Trace.TraceError(sb.ToString());

                    throw new InvalidOperationException(sb.ToString(), etl);
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
            }

            Type type = ass.GetType(typename, true, true);

            object obj = Activator.CreateInstance(type);

            if (obj == null)
                throw new ArgumentException("Cannot create instance " + type.FullName);

            return obj;
        }

        Assembly FindAssemblies(object sender, ResolveEventArgs args)
        {
            // If a linkable component loads additional assemblies OmiEd might
            // have issues resolving them
            // This will help by guessing that the additional assembly might be
            // in the same folder as the linkable component assembly and trying that.
            // Not guanteed to work but definatley does help in some cases.

            string name = new AssemblyName(args.Name).Name;

            Trace.TraceInformation(string.Format("FindAssemblies({0})", name));

            if (_potentialAssemblyExtensions == null)
            {
                _potentialAssemblyExtensions = new List<string>();
                _potentialAssemblyExtensions.Add(".dll");
                _potentialAssemblyExtensions.Add(".exe");
            }

            if (_potentialAssemblyFolders == null)
                _potentialAssemblyFolders = new List<string>();

            string file;
            Assembly assembly;
            FileInfo fi;

            foreach (string extension in _potentialAssemblyExtensions)
            {
                file = name + extension;

                if (_assemblyFile != null)
                {
                    fi = new FileInfo(Path.Combine(_assemblyFile.DirectoryName, file));

                    if (fi.Exists)
                    {
                        assembly = Assembly.LoadFrom(fi.FullName);

                        if (assembly != null)
                        {
                            Trace.TraceInformation(
                                string.Format("FindAssemblies({0}) =\r\n\t{1}\r\n\t{2}",
                                name, assembly.FullName, assembly.Location));
                            return assembly;
                        }
                    }
                }

                foreach (string path in _potentialAssemblyFolders)
                {
                    fi = new FileInfo(Path.Combine(path, file));

                    if (!fi.Exists)
                        continue;

                    assembly = Assembly.LoadFrom(fi.FullName);

                    if (assembly != null)
                    {
                        Trace.TraceInformation(
                            string.Format("FindAssemblies({0}) =\r\n\t{1}\r\n\t{2}",
                            name, assembly.FullName, assembly.Location));
                        return assembly;
                    }
                }
            }

            Trace.TraceError(string.Format(
                "FindAssemblies({0}) = Not Found", name));

            return null;
        }
    }

	/// <summary>
	/// This class is globaly used to manage assemblies.
	/// </summary>
	public class AssemblySupport
	{
		/// <summary>
		/// One element of internal list of assemblies
		/// </summary>
		private class AssemblyItem
		{
			public Assembly assembly;
			public string fullName;
		}

		/// <summary>
		/// Internal list of assemblies.
		/// </summary>
		private static ArrayList _assemblies;

		/// <summary>
		/// Loads specific assembly into internal list of assemblies.
		/// </summary>
		/// <param name="directory">Directory <c>filename</c> is relative to, or <c>null</c> if <c>filename</c> is absolute or relative to current directory.</param>
		/// <param name="filename">Relative or absolute path to assembly.</param>
		/// <remarks>See <see cref="Utils.GetFileInfo">Utils.GetFileInfo</see> for more info about how
		/// specified file is searched. If file isn't found, method tries to
		/// load assembly from global assembly cache (GAC).</remarks>
		public static void LoadAssembly( string directory, string filename )
		{
			Assembly assembly;
			
			FileInfo assemblyFileInfo = Utils.GetFileInfo( directory, filename );

			// if assemby file exists, try to load it
			if( assemblyFileInfo.Exists )
			{
				assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);
			}
			else
			{
				// if file doesn't exist, try to load assembly from GAC
				try
				{					
					assembly = Assembly.Load(filename);
				}
				catch( Exception e )
				{					
					throw( new Exception("Assembly cannot be loaded (CurrentDirectory='"+Directory.GetCurrentDirectory()+"', Name='"+filename+"')", e) );
				}
			}
		
			// add assembly to list of assemblies only if not already present
			foreach( AssemblyItem assemblyItem in _assemblies )
				if( 0==String.Compare(assemblyItem.fullName, assembly.FullName, true) )
					return;
			
			AssemblyItem newItem = new AssemblyItem();
			newItem.assembly = assembly;
			newItem.fullName = assembly.FullName;
			_assemblies.Add( newItem );			
		}


		/// <summary>
		/// Creates new instance of type contained in one previously loaded assembly, or from application context if 
		/// not found.
		/// </summary>
		/// <param name="typeName">Name of the type</param>
		/// <returns>Returns new instance of specified type.</returns>
		/// <remarks>New instance is created with default parameterless constructor,
		/// if such constructor doesn't exists an exception is thrown.</remarks>
		public static object GetNewInstance( string typeName )
		{
			object result;
			Type type = null;
			
			foreach( AssemblyItem assemblyItem in _assemblies )
			{				
				type = assemblyItem.assembly.GetType( typeName, false );
				if( type!=null )
					break;
			}

			if( type==null )
				type = Type.GetType( typeName, false );
			
			if( type==null )
				throw( new Exception("Class type "+typeName+" not found neither in loaded assemblies nor in application context.") );

			// construct new item with default constructor
			ConstructorInfo constructorInfo = type.GetConstructor( Type.EmptyTypes );
			if( constructorInfo==null )
				throw( new Exception("Requested class type has no default parameterless constructor.") );

			result = constructorInfo.Invoke(null);
			return( result );
		}


		/// <summary>
		/// Releases all assemblies from internal list.
		/// </summary>
		public static void ReleaseAll()
		{
			_assemblies = new ArrayList();
			GC.Collect();			
		}

		/// <summary>
		/// Initializes internal list of assemblies.
		/// </summary>
		static AssemblySupport()
		{
			_assemblies = new ArrayList();
		}
	}
}
