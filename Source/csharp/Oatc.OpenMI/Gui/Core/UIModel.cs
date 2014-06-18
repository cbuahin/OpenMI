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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class UIModel
	{
        // Have a unique caption, even when Linkable Component is unavailable 
        static int SNextUIModelIndex = 0;
        int _nextUIModelIndex = SNextUIModelIndex;

		private string _omiFilename;
	
		private readonly Font _font;
		bool _isTrigger = false;

		/// <summary>
		/// <c>true</c> if user is moving the model rectangle on the screen
		/// </summary>
		private bool _isMoving;

		private ITimeSpaceComponent _linkableComponent;

        static Pen _sPenBlue = new Pen(Color.Blue, 1);
        static Pen _sPenRed = new Pen(Color.Red, 2);
		
		/// <summary>
		/// Creates a new instance of <see cref="UIModel">UIModel</see> class.
		/// </summary>
		public UIModel()
		{
            ++SNextUIModelIndex;

			_isMoving = false;

			//  Workarround to handle a bug from Microsoft
			//  A bug, see http://dturini.blogspot.com/2004_08_01_dturini_archive.html
			//  or  http://support.microsoft.com/default.aspx?scid=KB;EN-US;q326219#appliesto
			try
			{
				_font = new Font("Arial", 11);
			}
			catch (ArithmeticException)
			{
				Utils.ResetFPU(); 
				_font = new Font("Arial", 11);
			} 

			Rect = new Rectangle(30,30,100,3*_font.Height);	
        }

        public bool IsTrigger
        {
			set { _isTrigger = value; }
            get { return _isTrigger; }
        }

		/// <summary>
		/// Gets or sets path to OMI file representing this model.
		/// </summary>
		/// <remarks>Setting of this property has only sense in case this model is trigger, see
		/// <see cref="NewTrigger">NewTrigger</see> method.</remarks>
		public string OmiFilename
		{
			get { return(_omiFilename); }
			set { _omiFilename = value; }
		}

		/// <summary>
		/// Gets Caption of this model.
		/// </summary>
		/// <remarks>ID is equivalent to <see cref="ITimeSpaceComponent.ComponentIdentifer">ILinkableComponent.ModelID</see>.
		/// It must be unique in the composition.
		/// </remarks>
		public string InstanceCaption
		{
			get 
            { 
                return _linkableComponent == null
                    ? _nextUIModelIndex.ToString()
                    : _linkableComponent.Caption; 
            }
		}

		private void MoveModel( Point offset )
		{
			Rect.Offset( offset );
		}

		/// <summary>
		/// Draws this model's rectangle into specified <see cref="Graphics">Graphics</see> object.
		/// </summary>
		/// <param name="displacement">Displacement of composition box in whole composition area.</param>
		/// <param name="g"><see cref="Graphics">Graphics</see> where rectangle should be drawn.</param>
		public void Draw(bool selected, Point displacement, Graphics g)
		{
			Rectangle rectToDraw = Rect;
			rectToDraw.X -= displacement.X;
			rectToDraw.Y -= displacement.Y;

			Region fillRegion = new Region(rectToDraw);	
		
			g.FillRegion( GetFillBrush(), fillRegion );
			g.DrawRectangle(selected ? _sPenRed : _sPenBlue, rectToDraw);
            g.DrawString(InstanceCaption, _font, Brushes.Black, rectToDraw);
		}

		private Brush GetFillBrush()
		{
            if (IsTrigger)
			{
				// trigger has different color
				if( _isMoving )
					return( new SolidBrush(Color.SteelBlue) );
				else
					return( new SolidBrush(Color.SkyBlue) );
			}			

			if( _isMoving )
				return( new SolidBrush(Color.Goldenrod) );
			else
				return( new SolidBrush(Color.Yellow) );
		}

		/// <summary>
		/// Gets middle point of model's rectangle.
		/// </summary>
		/// <returns>Returns middle point of model's rectangle.</returns>
		public Point GetMidPoint()
		{
			return new Point( Rect.X + Rect.Width/2, Rect.Y + Rect.Height/2 );
		}

		/// <summary>
		/// Determines whether point is in model's rectangle.
		/// </summary>
		/// <param name="point">Point</param>
		/// <returns>Returns <c>true</c> if the point is in model's rectangle, otherwise returns <c>false</c>.</returns>
		public bool IsPointInside( Point point )
		{
			return( Rect.X < point.X
				&& (Rect.X + Rect.Width) > point.X
				&& Rect.Y < point.Y
				&& (Rect.Y + Rect.Height) > point.Y );
		}


		/// <summary>
		/// Gets or sets whether model's rectangle is currently moving.
		/// </summary>
		/// <remarks>
		/// It's useful for example to draw moving rectangles with different color.
		/// </remarks>
		public bool IsMoving
		{
			get	{ return _isMoving; }
			set { _isMoving = value; }
		}

		/// <summary>
		/// Model's rectangle.
		/// </summary>
		public Rectangle Rect;	

		/// <summary>
		/// Linkable component corresponding to this model.
		/// </summary>
		public ITimeSpaceComponent LinkableComponent
		{
			get
			{
				return _linkableComponent;
			}
			set
			{
				_linkableComponent = value;
			}
		}

		public void OmiDeserializeAndInitialize(FileInfo omiFile)
		{
			_linkableComponent = Utils.OmiDeserializeAndInitialize(omiFile);

			_omiFilename = omiFile.FullName;
		}
	}
}
