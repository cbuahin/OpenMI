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
#region Copyright
///////////////////////////////////////////////////////////
//
// namespace: org.OpenMI.Utilities.Spatial 
// purpose: Utility for spatial data-operations within the LinkableComponents wrappers
// file: DataOperation.cs
//
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original author: Peter Sinding & Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      6 April 2006
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion

using System;
using Oatc.UpwardsComp.Standard;

namespace Oatc.UpwardsComp.Spatial
{
	/// <summary>
	/// Summary description for DataOperation.
	/// </summary>
	[Serializable]
	public class DataOperation : Oatc.UpwardsComp.Backbone.DataOperation
	{
    //=====================================================================
    // DataOperation() : void
    //=====================================================================
    /// <summary>
    /// Redecalration of the overloaded constructor from the base class. 
    /// The redeclaration is probably needed since constructors can not be 
    /// marked as virtual.
    /// </summary>
    public DataOperation()
    {
		}

    //=====================================================================
    // DataOperation(string ID) : void
    //=====================================================================
    /// <summary>
    /// Redecalration of the overloaded constructor from the base class. 
    /// The redeclaration is probably needed since constructors can not be 
    /// marked as virtual.
    /// </summary>
   	public DataOperation(string ID) : base(ID)
		{
		}

    //=====================================================================
    // IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem, 
    //         IDataOperation[] SelectedDataOperations): bool
    //=====================================================================
    /// <summary>
    /// For a given combination of inputExchangeItem, outputExchangeItem and list of dataOperation 
    /// it is decided if the dataOperations constitutes a valid set seen from a spatial settings 
    /// point of view. 
    /// </summary>
    /// <param name="inputExchangeItem">The input exchange item</param>
    /// <param name="outputExchangeItem">The output exchange item</param>
    /// <param name="SelectedDataOperations">List of selected dataOperations</param>
    public override bool IsValid(IInputExchangeItem inputExchangeItem, IOutputExchangeItem outputExchangeItem, IDataOperation[] SelectedDataOperations)
    {
      bool returnValue = true;
      bool methodAvaileble = false;
      ElementMapper elementMapper = new ElementMapper();

      foreach (string idString in elementMapper.GetIDsForAvailableDataOperations(outputExchangeItem.ElementSet.ElementType, inputExchangeItem.ElementSet.ElementType))
      {
        if (ID == idString)
        {
          methodAvaileble = true;
        }
      }
      if (!methodAvaileble)
      {
        return false;
      }

      // --- check that only one SpatialMapping dataoperation is selected. ---
      int numberOfSelectedSpatialMappingDataOperations = 0;
      foreach (IDataOperation dataOperation in SelectedDataOperations)
      {
        for (int i = 0; i < dataOperation.ArgumentCount; i++)
        {
          if (dataOperation.GetArgument(i).Id == "Type")
          {
            if (dataOperation.GetArgument(i).Value == "SpatialMapping")
            {
              numberOfSelectedSpatialMappingDataOperations++;  //this counting is done to check if the same dataOpertion is added twise

              if (dataOperation.ID != ID) //the selected dataoperation must be this dataOperation
              {
                returnValue = false; //the selected dataoperation must be this 
              }
            }
          }
        }
      }

      if (numberOfSelectedSpatialMappingDataOperations > 1)
      {
        returnValue = false;
      }

      return returnValue;
    }
  }
}
