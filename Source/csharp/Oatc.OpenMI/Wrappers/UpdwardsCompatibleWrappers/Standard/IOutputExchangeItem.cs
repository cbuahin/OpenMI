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

namespace Oatc.UpwardsComp.Standard
{
	/// <summary>
	/// <para>An IOutputExchangeItem describes an output item that can be delivered by a LinkableComponent.
	/// The item describes on which elementset a quantity can be provided.</para>
	/// <para>An output exchange item may provide data operations (interpolation in time, spatial interpolation etc.) that can
	/// be performed on the output exchange item before the values are delivered to the target ILinkableComponent</para>
	/// </summary>

	public interface IOutputExchangeItem : IExchangeItem
	{
		/// <summary>
		/// The number of data operations that can be performed on the output quantity/elemenset.
		/// </summary>
		int DataOperationCount {get;}


		/// <summary>
		/// Get one of the data operations that can be performed on the output quantity/elemenset.
		/// </summary>
		/// <param name="dataOperationIndex">The index for the data operation [0, DataOperationCount-1].</param>
		/// <returns>The data operation for index dataOperationIndex.</returns>
		IDataOperation GetDataOperation (int dataOperationIndex);

	}
}
