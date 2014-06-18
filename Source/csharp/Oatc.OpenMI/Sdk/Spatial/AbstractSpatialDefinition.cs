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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  /// <summary>
  /// Default implementation of the spatial base interface <see cref="ISpatialDefinition"/>
  /// </summary>
  public abstract class AbstractSpatialDefinition
  {
    public string Caption { get; set; }
    public string Description { get; set; }
    public string SpatialReferenceSystemWkt { get; set; }
    public virtual int Version { get; set; }
    //public abstract int ElementCount { get; }
  }

  /// <summary>
  /// Default implementation of a class that wraps an <see cref="ISpatialDefinition"/>
  /// </summary>
  public abstract class AbstractSpatialDefinitionWrapper : ISpatialDefinition, IIdentifiable
  {
    private string _id;
    protected abstract ISpatialDefinition SpatialDefinition { get; }

    protected AbstractSpatialDefinitionWrapper(string id)
    {
      _id = id;
    }

    public string Caption
    {
      get { return (SpatialDefinition.Caption); }
      set { SpatialDefinition.Caption = value; }
    }

    public string Description
    {
      get { return (SpatialDefinition.Description); }
      set { SpatialDefinition.Description = value; }
    }

    public string SpatialReferenceSystemWkt
    {
      get { return (SpatialDefinition.SpatialReferenceSystemWkt); }
    }

    public int ElementCount
    {
      get { return (SpatialDefinition.ElementCount); }
    }

    public int Version
    {
      get { return (SpatialDefinition.Version); }


    }

    public string Id
    {
      get { return (_id); }
    }
  }
}
