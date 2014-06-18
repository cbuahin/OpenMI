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

using System.Windows.Forms;
using log4net.Appender;
using log4net.Core;

namespace Oatc.OpenMI.Gui.Core
{
    /// <summary>
	/// Listener used to show simulation progress in <see cref="ListView">ListView</see> control.
	/// </summary>
    public class ListViewLogAppender : AppenderSkeleton
	{
		private static ListView _listView;

        public static ListView ListView
        {
            get { return _listView; }
            set { _listView = value; }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if(_listView == null)
            {
                return;
            }

            if(_listView.InvokeRequired)
            {
                _listView.BeginInvoke(new AddMessageToListViewDelegate(AddMessageToListView), new object[] { loggingEvent.RenderedMessage });			
            }

        }

        delegate void AddMessageToListViewDelegate(string message);

        private void AddMessageToListView(string message)
        {
            _listView.BeginUpdate();

            _listView.EndUpdate();
        }
	}
}
