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
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  /// <summary>
  /// Helper class, providing functionality used by many of the tests
  /// </summary>
  public static class UTHelper
  {
    public static ITimeSpaceInput FindInputItem(IBaseLinkableComponent componentInstance, string inputItemId)
    {
      foreach (ITimeSpaceInput inputItem in componentInstance.Inputs)
      {
        if (inputItem.Id.Equals(inputItemId))
        {
          return inputItem;
        }
      }
      throw new Exception("Input item \"" + inputItemId + "\" not found in component " + componentInstance.Id);
    }

    public static ITimeSpaceOutput FindOutputItem(IBaseLinkableComponent componentInstance, string outputItemId)
    {
      foreach (ITimeSpaceOutput outputItem in componentInstance.Outputs)
      {
        if (outputItem.Id.Equals(outputItemId))
        {
          return outputItem;
        }
      }
      throw new Exception("Output item \"" + outputItemId + "\" not found in component " + componentInstance.Id);
    }
  }
}