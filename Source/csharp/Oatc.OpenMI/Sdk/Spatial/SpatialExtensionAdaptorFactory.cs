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
  /// An <see cref="IAdaptedOutputFactory"/> that creates <see cref="IBaseAdaptedOutput"/>
  /// which wraps the spatial definition (in case of a spatial extension) as <see cref="IElementSet"/>
  /// </summary>
  public class SpatialExtensionAdaptorFactory : IAdaptedOutputFactory
  {
    public string Caption { get; set; }
    public string Description { get; set; }
    public string Id { get; private set; }

    public SpatialExtensionAdaptorFactory()
    {
      Id = "SpatialExtensionAdaptorFactory";
      Caption = "SpatialExtension AdaptorFactory";
      Description = "Creates adaptors wrapping spatial extension into IElementSet";
    }

    public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
    {
      ITimeSpaceOutput tadaptee = adaptee as ITimeSpaceOutput;
      
      // Not a time-space exchange item, no available adaptors
      if (tadaptee == null)
        return (new IIdentifiable[0]);
      
      // For each of the spatial extension classes, return its wrapper class

      // MultiPoint must come before ILineString (because ILineString is currently also an IMultiPoint)
      if (tadaptee.SpatialDefinition is IMultiPoint)
        return new IIdentifiable[] { new SpatialExtensionElementSetAdaptor(tadaptee, new MultiPointWrapper(tadaptee.SpatialDefinition as IMultiPoint), "MultiPointWrapper") };
      
      if (tadaptee.SpatialDefinition is ILineString)
        return new IIdentifiable[] { new SpatialExtensionElementSetAdaptor(tadaptee, new LineStringWrapper(tadaptee.SpatialDefinition as ILineString), "LineStringWrapper") };
      
      if (tadaptee.SpatialDefinition is ISpatial2DGrid)
        return new IIdentifiable[] { new SpatialExtensionElementSetAdaptor(tadaptee, new Spatial2DGridWrapper(tadaptee.SpatialDefinition as ISpatial2DGrid), "Spatial2DGridWrapper") };
      
      if (tadaptee.SpatialDefinition is ISpatialMesh)
        return new IIdentifiable[] { new SpatialExtensionElementSetAdaptor(tadaptee, new SpatialMeshWrapper(tadaptee.SpatialDefinition as ISpatialMesh), "SpatialMeshWrapper") };

      // Spatial definition is not known, return an empty set.
      return (new IIdentifiable[0]);

    }

    public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
    {
      ITimeSpaceAdaptedOutput adaptedOutput = adaptedOutputId as ITimeSpaceAdaptedOutput;
      if (adaptedOutput == null)
        throw new ArgumentException("Adapted output id does no come from this factory","adaptedOutputId");
      // Connect adaptor and adaptee
      if (!adaptee.AdaptedOutputs.Contains(adaptedOutput))
      {
        adaptee.AddAdaptedOutput(adaptedOutput);
      }
      return (adaptedOutput);
    }
  }
}
