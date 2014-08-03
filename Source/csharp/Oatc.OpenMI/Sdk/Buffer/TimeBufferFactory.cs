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
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;
using System.Collections.Generic;
using System.Linq;

namespace Oatc.OpenMI.Sdk.Buffer
{
  public class TimeBufferFactory : IAdaptedOutputFactory
  {
    private readonly string id;
    List<IBaseAdaptedOutput> outputsCreatedSoFar;

    public TimeBufferFactory()
    {
        outputsCreatedSoFar = new List<IBaseAdaptedOutput>();
    }
    public TimeBufferFactory(string id)
    {
      this.id = id;
      outputsCreatedSoFar = new List<IBaseAdaptedOutput>();
    }

    #region Implementation of IDescribable

    public string Caption { get; set; }

    public string Description { get; set; }

    #endregion

    #region Implementation of IIdentifiable

    public string Id
    {
      get
      {
        return id;
      }
    }

    #endregion

    #region Implementation of IAdaptedOutputFactory

    public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
    {
      // The time methods in this factory only works on an ITimeSpaceOutput
      ITimeSpaceOutput tsAdaptee = adaptee as ITimeSpaceOutput;
      if (tsAdaptee == null)
        return (new IIdentifiable[0]);

      IIdentifiable[] ids = new IBaseAdaptedOutput[] 
      {
          new TimeInterpolator(tsAdaptee), 
          new TimeExtrapolator(tsAdaptee), 
          new LinearOperationAdaptedOutput(tsAdaptee) 
      };

      for (int i = 0; i < ids.Length; i++ )
      {
          IBaseAdaptedOutput searchItem = ids[i] as IBaseAdaptedOutput;

          IBaseAdaptedOutput temp = (from n in outputsCreatedSoFar
                                     where searchItem.Id == n.Id && n.Adaptee == searchItem.Adaptee
                                     select n).FirstOrDefault();

          if(temp == null)
          {
              outputsCreatedSoFar.Add(searchItem);
          }
          else
          {
              ids[i] = temp;
          }
      }

      return ids;
    }

    public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputIdentifier, IBaseOutput adaptee, IBaseInput target)
    {
      IBaseAdaptedOutput adaptor = (from n in outputsCreatedSoFar
                                   where n.Id == adaptedOutputIdentifier.Id && n.Adaptee == adaptee
                                   select n).FirstOrDefault();

      if (adaptor == null)
      {
        throw new ArgumentException("Unknown adaptedOutput type - it does not originate from this factory");
      }
      // Connect the adaptor and the adaptee
      if (!adaptee.AdaptedOutputs.Contains(adaptor))
      {
        adaptee.AddAdaptedOutput(adaptor);
      }
      return adaptor;
    }

    #endregion
  }
}
