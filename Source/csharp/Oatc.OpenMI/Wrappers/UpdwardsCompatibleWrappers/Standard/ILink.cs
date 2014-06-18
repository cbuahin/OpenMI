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

using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.UpwardsComp.Standard
{
  /// <summary>
  /// Link interface
  /// </summary>

  public interface ILink
  {
	
    /// <summary>
    /// Identification string
    /// </summary>
    string ID {get;}


    /// <summary>
    /// Additional descriptive information
    /// </summary>
    string Description {get;}


    /// <summary>
    /// Number of data operations
    /// </summary>
    int DataOperationsCount {get;}


    /// <summary>
    /// Get the data operation with index DataOperationIndex.
    /// If this method is invoked with a dataOperationIndex, which is outside the interval
    /// [0,DataOperationCount] an exception must be thrown.
    /// </summary>
    ///  <returns>DataOperation according to the argument: dataOperationCount.</returns>
    IDataOperation GetDataOperation(int dataOperationIndex);

    ITimeSpaceInput Target { get; }
    ITimeSpaceOutput Source { get; }

    /// <summary>
    /// Target quantity
    /// </summary>
		
    IQuantity TargetQuantity {get;}


    /// <summary>
    /// Target elementset
    /// </summary>

    IElementSet TargetElementSet {get;}


    /// <summary>
    /// Source elementset
    /// </summary>

    IElementSet SourceElementSet {get;}


    /// <summary>
    /// Souce linkable component
    /// </summary>

    IBaseLinkableComponent SourceComponent {get;}


    /// <summary>
    /// Source quantity
    /// </summary>

    IQuantity SourceQuantity {get;}


    /// <summary>
    /// Target linkable component
    /// </summary>

    IBaseLinkableComponent TargetComponent {get;}

  }
}