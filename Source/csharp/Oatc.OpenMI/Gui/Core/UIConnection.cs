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
using System.Windows.Forms;
using System.Text;

using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Gui.Core
{
    public class Link
    {
        UIInputItem _target;
        List<UIOutputItem> _sources;

        public Link()
        {
            _sources = new List<UIOutputItem>();
        }

        public Link(UIOutputItem source, UIInputItem target)
        {
            _sources = new List<UIOutputItem>();
            _target = target;
            _sources.Add(source);
        }

        public Link(List<UIOutputItem >sources , UIInputItem target)
        {
            _sources = new List<UIOutputItem>(sources);
            _target = target;
           
        }

        public UIOutputItem Source
        {
            get 
            {
                if(_sources.Count > 0)
                {
                    return _sources[0];
                }
                else
                return null; 
            }
            set 
            {
                _sources.Remove(value);
                _sources.Add(value); 
            }
        }

        public List<UIOutputItem> Sources
        {
            get { return _sources; }
            set
            {
                _sources = value;
            }
        }

        public UIInputItem Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public override string ToString()
        {
            string toString = "";

            if (_sources.Count > 0 ||  _target != null)
            {
                if(IsMultiInputLink)
                {
                    for(int i = 0 ; i <_sources.Count ; i++)
                    {
                        if(i == 0)
                        {
                            toString = "[ " +GetOutputItemString(_sources[i]) + " ]";
                        }
                        else
                        {
                            toString = toString + " , [ " + GetOutputItemString(_sources[i]) + " ]";
                        }
                    }
                }
                else
                {
                    toString = GetOutputItemString(Source);
                }
               
                toString = "{ " +toString + " } => { " + _target + " }";
            }
            else
            {
                return "<Source not set> => <Target not set>";

            }
            return toString;
        }

        string GetOutputItemString(UIOutputItem item)
        {
            string toString = item.ToString();
            UIOutputItem parent = item.Parent;

            while (parent != null)
            {
                toString = parent + " => " + toString;
                parent = parent.Parent;
            }

            return toString;
        }

        public bool IsMultiInputLink
        {
            get
            {
            return _sources.Count > 1;
            }
        }

        bool CheckOutputItemEqual(UIOutputItem item1 , UIOutputItem item2)
        {
            if(item1 != null && item2 != null && item1 == item2 || item1.ExchangeItem == item2.ExchangeItem)
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if(obj is Link)
            {
                Link link = (Link)obj;

                if ( link.Sources.Count == _sources.Count)
                {
                    int max = _sources.Count;

                    for (int i = 0; i < max; i++)
                    {
                        if(!CheckOutputItemEqual(_sources[i], link.Sources[i]))
                        {
                            return false;
                        }
                    }
                     
                    return true;
                }

            }

            return false;
        }

        public List<UIOutputItem> FindNextChildrenInChain(UIOutputItem output)
        {
            List<UIOutputItem> items = new List<UIOutputItem>();

            foreach(UIOutputItem source in Sources)
            {
                UIOutputItem parent = source.Parent;
                
                bool found = false;

                while(parent != null)
                {
                    if (parent == output || parent.ExchangeItem == output.ExchangeItem)
                    {
                        found = true;
                       break;
                    }

                    parent = parent.Parent;
                }
                
                if(found)
                {
                   
                    items.Add(parent);
                }
            }

            return items;
        }

        public UIOutputItem FindInChain(ITimeSpaceOutput output)
        {
            foreach (UIOutputItem source in Sources)
            {

                UIOutputItem parent = source;

                while (parent != null)
                {
                    if (parent.ExchangeItem == output)
                    {
                        return parent;
                    }

                    parent = parent.Parent;
                }
         
            }

            return null;
        }

        public static List<UIOutputItem> ParentChain(UIOutputItem item)
        {
            List<UIOutputItem> parentChain = new List<UIOutputItem>();

            UIOutputItem parent = item;

            while(parent != null)
            {
                parentChain.Add(parent);
                parent = parent.Parent;
            }

            parentChain.Reverse();

            return parentChain;
        }
    }

	/// <summary>
	/// This class describes connection between two OpenMI models in one direction,
	/// which consists of many links in same direction.
	/// </summary>
	public class UIConnection
	{

		UIModel _sourceModel = null;
		UIModel _targetModel = null;
		Point[] _trianglePoints = new Point[0];
        List<Link> _links = new List<Link>();
        static Pen _sLinePen = new Pen(Color.Blue, 2);

        public UIConnection(UIModel sourceModel, UIModel targetModel)
		{
            _sourceModel = sourceModel;
            _targetModel = targetModel;

			_trianglePoints = new Point[3];		
        }

		public UIModel SourceModel
		{
			get { return(_sourceModel); }
		}

		public UIModel TargetModel
		{
			get { return(_targetModel); }
		}

        public List<Link> Links
        {
            get { return _links; }
            set { _links = value; }
        }
	
		/// <summary>
		/// Draw connection (i.e. line with triangle) to specific graphics object.
		/// </summary>
		/// <param name="windowPosition">Position of window described by graphics object in composition area.</param>
		/// <param name="g">Graphics where connection should be drawn.</param>		
		public void Draw(bool selected, Point windowPosition, Graphics g)
		{
			float startX = _sourceModel.GetMidPoint().X;
			float startY = _sourceModel.GetMidPoint().Y ;
			float endX   = _targetModel.GetMidPoint().X;
			float endY   = _targetModel.GetMidPoint().Y;

			// calculate triangle point in area points and store them internally
			_trianglePoints = GetTrianglePoints( startX, startY, endX, endY );

			// recalculate trinagle points so they correspond to window and can be draw
			Point[] windowTrianglePoints = new Point[3];
			for( int i=0; i<3; i++ )
			{
				windowTrianglePoints[i].X = _trianglePoints[i].X - windowPosition.X;
				windowTrianglePoints[i].Y = _trianglePoints[i].Y - windowPosition.Y;
			}

			// modify start and end so they correspond to window
			startX -= windowPosition.X;
			startY -= windowPosition.Y;
			endX -= windowPosition.X;
			endY -= windowPosition.Y;
		
			g.DrawLine(_sLinePen, startX, startY, endX, endY);
			
			// we draw the triangle only the link is at least 10 pixels
			if( Math.Abs(startX-endX) + Math.Abs(startY-endY) > 10 )
			{
                g.FillPolygon(selected ? Brushes.Red : Brushes.Blue, windowTrianglePoints, System.Drawing.Drawing2D.FillMode.Alternate);
				g.DrawPolygon(selected ? Pens.Red : Pens.Blue, windowTrianglePoints );
			}
		}
	
		private static Point[] GetTrianglePoints(float startX, float startY, float endX, float endY)
		{
			Point[] trianglePoints = new Point[3];

			const float size = 10; // size of triangles

			float midX   = (endX + startX) / 2;
			float midY   = (endY + startY) / 2;			

			float length = (float) Math.Sqrt(Math.Pow((startX-midX),2) + Math.Pow((startY-midY),2));
		
			float pX = midX + size *(startX - midX)/length;
			float pY = midY + size *(startY - midY)/length;

			float vX = midX - pX;
			float vY = midY - pY;

			float t1X = pX - vY;
			float t1Y = pY + vX;

			float t2X = pX + vY;
			float t2Y = pY - vX;

			trianglePoints[0] = new Point((int) midX,(int) midY);
			trianglePoints[1] = new Point((int) t1X,(int) t1Y);
			trianglePoints[2] = new Point((int) t2X,(int) t2Y);

			return( trianglePoints );
		}


		/// <summary>
		/// Determines, whether point is on connection line, i.e. in the triangle.
		/// </summary>
		/// <param name="point">Point</param>
		/// <returns>Returns <c>true</c> if point is inside the triangle, otherwise returns <c>false</c>.</returns>
		public bool IsOnConnectionLine( Point point )
		{
			bool isOnConnectionLine = true;
			int m;

			for (int i = 0; i < 3; i++)
			{
				m = i + 1;
				if (m == 3)
					m = 0;			

				if(0 < (point.X - _trianglePoints[i].X)*(_trianglePoints[m].Y-_trianglePoints[i].Y) - ( _trianglePoints[m].X - _trianglePoints[i].X )*(point.Y-_trianglePoints[i].Y))
					isOnConnectionLine = false;				
			}

			return isOnConnectionLine;				
		}		
	}
}
