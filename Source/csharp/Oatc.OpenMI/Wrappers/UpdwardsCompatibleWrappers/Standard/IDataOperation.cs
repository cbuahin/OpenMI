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

namespace Oatc.UpwardsComp.Standard
{

	/// <summary>
	/// DataOperation interface
	/// </summary>

	public interface IDataOperation
	{
		/// <summary>
        /// This method should not be a part of the standard since it is not required 
        /// to be invoked by any outside component. However, in order to avoid changing 
        /// the standard it will remain in the IDataOperation interface. 
        /// It is recommended simply to make an empty implementation of this method.
		/// </summary>
		void Initialize(IArgument[] properties);

		/// <summary>
        /// <para>Identification string for the data operation.</para>
        /// 
        /// <para>Two or more data operations provided by one OutputExchangeItem may not have the same ID.</para> 
		/// 
        /// <para>EXAMPLE:</para>
        /// <para>"Mean value", "Max value", "Spatially averaged", "Accumulated", "linear conversion"</para>
        /// </summary>
		string ID {get;}


		/// <summary>
		/// Number of arguments for this data operation
		/// </summary>
		int ArgumentCount {get;}


		/// <summary>
        /// <para>Gets the argument object (instance of class implementing IArgument) as
        /// identified by the argumentIndex parameter.</para>
        /// </summary>
        /// 
        /// <param name="argumentIndex">
        /// <para>The index-number of the requested DataOperation(indexing starts from zero)</para>
        /// <para>This method must accept values of argumentIndex in the interval [0, ArgumentCount - 1].
        /// If the argumentIndex is outside this interval an exception must be thrown.</para>.</param>
        /// 
        /// <returns>The Argument as identified by argumentIndex.</returns>
		IArgument GetArgument(int argumentIndex);

		/// <summary>
        /// Validates a specific combination of InputExchangeItem, OutputExchangeItem and a 
        /// selection of DataOperations. If this combination is valid true should be 
        /// returned otherwise false should be returned.
		/// </summary>
        /// 
		/// <param name="inputExchangeItem">The input exchange item.</param>
        /// 
		/// <param name="outputExchangeItem">The output exchange item.</param>
        /// 
		/// <param name="selectedDataOperations">The already selected data operations.</param>
        /// 
        /// <returns>True if the combination of InputExchangeItem, OutputExchangeItem, and the array 
        /// of dataOperations provided in the methods argument is valid, otherwise false.</returns>
		bool IsValid(IInputExchangeItem inputExchangeItem,IOutputExchangeItem outputExchangeItem,
			IDataOperation[] selectedDataOperations);
	}
}
