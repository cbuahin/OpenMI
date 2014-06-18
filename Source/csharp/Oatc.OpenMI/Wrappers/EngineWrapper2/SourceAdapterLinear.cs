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
using System.Collections.Generic;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters
{
    class Linear : AddapterBase
    {
        static Identifier _identifer = new Identifier(
            "Oatc.OpenMI.Wrappers.EngineWrapper2.SourceAdapters.Linear");
        static Describable _describable = new Describable(
            "y = Ax + B",
			"OATC ModelWrapper2 source adapter Linear (double): y = Ax + B");

        double _a = 1.0;
        double _b = 0.0;
        bool _initialisedFromArgs = false;

        public Linear(ITimeSpaceOutput itemOut, ITimeSpaceInput itemIn)
            : base(_identifer, _describable, itemOut, itemIn)
        {
            IArgument arg = new ArgumentDouble("A", 1.0)
                              {
                                Caption = "A", 
                                Description = "A [double] in y = A*x + B"
                              };

          _arguments.Add(arg);

            arg = new ArgumentDouble("B", 0.0)
                    {
                      Caption = "B", 
                      Description = "B [double] in y = A*x + B"
                    };

          _arguments.Add(arg);
        }

        public override string GetCaption()
        {
            return string.Format("y = {0}*x + {1}", _a.ToString(), _b.ToString());
        }

        void InitialiseFromArgs()
        {
            foreach (IArgument iArg in _arguments)
            {
                if (iArg.Id == "A")
                    _a = (double)iArg.Value;
                else if (iArg.Id == "B")
                    _b = (double)iArg.Value;
            }

            _initialisedFromArgs = true;
        }

        public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
        {
            // Note (SH): shouldn't a copy be made here?
			// Reply (ADH): In EngineWrapper2 I effectivly clone via the Values attribute
			// Suggest that standard should require GetValues to return
			// cloned IValueSet.
			// Maybe, more simply, IVAlueSet should implement IClonable
            ITimeSpaceValueSet values = base.GetValues(querySpecifier);

            if (values == null)
                return values;

            if (!_initialisedFromArgs)
                InitialiseFromArgs();

            for (int m = 0; m < values.Values2D.Count; ++m)
                for (int n = 0; n < values.Values2D[m].Count; ++n)
                    if (values.Values2D[m][n] is double)
                        values.Values2D[m][n] = _a * (double)values.Values2D[m][n] + _b;

            return values;
        }

        /// <summary>
        /// TODO Add as Static to standard?
        /// </summary>
        public static IIdentifiable Identifier
        {
            get { return _identifer; }
        }

        /// <summary>
        /// TODO Add as Static to standard?
        /// </summary>
        public static IDescribable Describable
        {
            get { return _describable; }
        }
    }

}
