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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using OpenMI.Standard2;

namespace Oatc.OpenMI.Sdk.Backbone
{

  public static class Argument
  {
    public static IArgument Create(string key, string value, bool isReadOnly, string description)
    {
      ArgumentString argument = new ArgumentString(key, value, value);
      argument.Description = description;
      return argument;
    }
    public static IArgument Create(string key, double value)
    {
      ArgumentDouble argument = new ArgumentDouble(key, value);
      return argument;
    }
  }

  /// <summary>
  /// Argument is a class that contains (key,value) pairs.
  /// <para>This is a trivial implementation of OpenMI.Standard.IArgument, refer there for further details.</para>
  /// </summary>
  [Serializable]
  public abstract class Argument<T> : IArgument
  {
    protected bool _isReadOnly;
    private bool _isOptional;

    private readonly string _id;
    private string _description = string.Empty;
    private string _caption;

    private T _defaultValue;
    private T _value;

    /// <summary>
    /// Empty constructor
    /// </summary>
    protected Argument()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="identifier">Unique Id</param>
    protected Argument(string identifier)
    {
      _id = identifier;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source">Source argument to copy</param>
    protected Argument(IArgument source)
    {
      _id = source.Id;
      _defaultValue = (T) source.DefaultValue;
      _value = (T)source.Value;
      Caption = source.Caption;
      Description = source.Description;
    }

    #region IArgument Members

    public Type ValueType
    {
      get { return typeof(T); }
    }

    public bool IsOptional
    {
      get { return _isOptional; }
      set { _isOptional = value; }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    public T Value
    {
      get { return (_value); } 
      set { _value = value; }
    }

    public T DefaultValue
    {
      get { return _defaultValue; }
      set { _defaultValue = value; }
    }

    object IArgument.Value 
    {
      get { return (_value); } 
      set { _value = (T)value; }
    }

    object IArgument.DefaultValue
    {
      get { return _defaultValue; }
    }

    public IList<object> PossibleValues
    {
      get { return null; }
    }

    public string Caption
    {
      get { return _caption; }
      set { _caption = value; }
    }

    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    #endregion

    ///<summary>
    /// Check if the current instance equals another instance of this class.
    ///</summary>
    ///<param name="obj">The instance to compare the current instance with.</param>
    ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
        return false;
      Argument<T> d = (Argument<T>)obj;
      return (Value.Equals(d.Value) && Caption.Equals(d.Caption));
    }

    ///<summary>
    /// Get Hash Code.
    ///</summary>
    ///<returns>Hash Code for the current instance.</returns>
    public override int GetHashCode()
    {
      int hashCode = base.GetHashCode();
      if (_caption != null) hashCode += _caption.GetHashCode();
      if (_value != null) hashCode += _value.GetHashCode();
      return hashCode;
    }

    public string Id
    {
      get { return _id; }
    }

    public string ValueAsString
    {
      get { return _value.ToString(); }
      set { SetValueFromString(value); }
    }

    abstract protected string GetValueAsString();
    abstract protected void SetValueFromString(string value);


    public static IArgument Create(string key, string value, bool isReadOnly, string description)
    {
      ArgumentString argument = new ArgumentString(key, value, value);
      argument.Description = description;
      return argument;
    }

  }

  
  

  [Serializable]
  public class ArgumentString : Argument<string>
  {
    public ArgumentString(string id)
      : base(id)
    {
      Init(id, "");
    }

    public ArgumentString(string id, string defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
    }

    public ArgumentString(string id, string value, string defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
      Value = value;
    }

    public ArgumentString(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption, string defaultValue)
    {
      Caption = caption;
      Description = "String Argument";
      DefaultValue = defaultValue;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value == null
        ? DefaultValue.ToString()
        : Value.ToString();
    }

    protected override void SetValueFromString(string value)
    {
      Value = value;
    }
  }

  [Serializable]
  public class ArgumentBool : Argument<bool>
  {
    public ArgumentBool(string id, bool defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
    }

    public ArgumentBool(string id, bool value, bool defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
      Value = value;
    }

    public ArgumentBool(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption, bool defaultValue)
    {
      Caption = caption;
      Description = "Bool Argument";
      DefaultValue = defaultValue;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value.ToString();
    }

    protected override void SetValueFromString(string value)
    {
      Value = bool.Parse(value);
    }
  }

  [Serializable]
  public class ArgumentInt : Argument<int>
  {
    public ArgumentInt(string id, int defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
    }

    public ArgumentInt(string id, int value, int defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
      Value = value;
    }

    public ArgumentInt(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption, int defaultValue)
    {
      Caption = caption;
      Description = "Int Argument";
      DefaultValue = defaultValue;
      Value = defaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value.ToString();
    }

    protected override void SetValueFromString(string value)
    {
      Value = int.Parse(value);
    }
  }

  [Serializable]
  public class ArgumentDouble : Argument<double>
  {
    public ArgumentDouble(string id, double defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
    }

    public ArgumentDouble(string id, double value, double defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
      Value = value;
    }

    public ArgumentDouble(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption, double defaultValue)
    {
      Caption = caption;
      Description = "Double Argument";
      DefaultValue = defaultValue;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value.ToString();
    }

    protected override void SetValueFromString(string value)
    {
      Value = double.Parse(value);
    }
  }

  [Serializable]
  public class ArgumentFileInfo : Argument<FileInfo>
  {
    public ArgumentFileInfo()
    {
      Init("ArgumentFileInfo No ID!");
    }

    public ArgumentFileInfo(string id)
      : base(id)
    {
      Init(id);
    }

    public ArgumentFileInfo(string id, bool readOnly)
      : base(id)
    {
      Init(id);
      _isReadOnly = readOnly;
    }

    public ArgumentFileInfo(string id, FileInfo info)
      : base(id)
    {
      Init(id);
      Value = info;
    }

    public ArgumentFileInfo(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption)
    {
      Caption = caption;
      Description = "FileInfo Argument";
      DefaultValue = null;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value == null
        ? String.Empty
        : (Value).FullName;
    }

    protected override void SetValueFromString(string value)
    {
      Value = new FileInfo(value);
    }
  }

  [Serializable]
  public class ArgumentDirectoryInfo : Argument<DirectoryInfo>
  {
    public ArgumentDirectoryInfo()
    {
      Init("ArgumentDirectoryInfo No ID!");
    }

    public ArgumentDirectoryInfo(string id)
      : base(id)
    {
      Init(id);
    }

    public ArgumentDirectoryInfo(string id, DirectoryInfo info)
      : base(id)
    {
      Init(id);
      Value = info;
    }

    public ArgumentDirectoryInfo(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption)
    {
      Caption = caption;
      Description = "DirectoryInfo Argument";
      DefaultValue = null;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      return Value == null
        ? String.Empty
        : (Value).FullName;
    }

    protected override void SetValueFromString(string value)
    {
      Value = new DirectoryInfo(value);
    }
  }

  [Serializable]
  public class ArgumentDateTime : Argument<DateTime>
  {
    public ArgumentDateTime(string id, DateTime defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
    }

    public ArgumentDateTime(string id, DateTime value, DateTime defaultValue)
      : base(id)
    {
      Init(id, defaultValue);
      Value = value;
    }

    public ArgumentDateTime(IArgument iArg)
      : base(iArg)
    {
    }

    void Init(string caption, DateTime defaultValue)
    {
      Caption = caption;
      Description = "DateTime Argument";
      DefaultValue = defaultValue;
      Value = DefaultValue;
    }

    protected override string GetValueAsString()
    {
      // print as UniversalSortableDateTimePattern
      return Value.ToString("u");
    }

    protected override void SetValueFromString(string value)
    {
      Value = DateTime.ParseExact(value, CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern,
                          CultureInfo.InvariantCulture);
    }
  }
}