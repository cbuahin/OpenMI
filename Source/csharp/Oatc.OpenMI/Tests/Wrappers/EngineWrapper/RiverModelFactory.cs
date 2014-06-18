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
using System.Linq;
using System.Text;
using Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper;
using Oatc.OpenMI.Examples.SimpleSCharpRiver;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest1;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  /// <summary>
  /// Factory interface for creating the different river models
  /// </summary>
  public interface IRiverModelFactory
  {
    LinkableEngine CreateRiverModel();
    List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model);
  }

  /// <summary>
  /// Factory for creating the <see cref="RiverModelLC"/>
  /// </summary>
  public class RiverModelFactory : IRiverModelFactory
  {
    public LinkableEngine CreateRiverModel()
    {
      return (new RiverModelLC());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      return (new List<IArgument>());
    }
  }

  /// <summary>
  /// Factory for creating the <see cref="RiverModelDelegateLC"/>
  /// </summary>
  public class RiverModelDelegateFactory : IRiverModelFactory
  {
    public LinkableEngine CreateRiverModel()
    {
      return (new RiverModelDelegateLC());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      return (new List<IArgument>());
    }
  }

  /// <summary>
  /// Factory for creating the <see cref="RiverModelInterfaceLC"/>
  /// </summary>
  public class RiverModelInterfaceFactory : IRiverModelFactory
  {
    public LinkableEngine CreateRiverModel()
    {
      return (new RiverModelInterfaceLC());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      return (new List<IArgument>());
    }
  }

  /// <summary>
  /// Factory for creating the fortran river model,
  /// <see cref="SimpleRiverEngineWrapper"/>
  /// </summary>
  public class FortranRiverModelFactory : IRiverModelFactory
  {
    RiverModelFactory _netFactory = new RiverModelFactory();

    private int _simpleRiverCounter;
    private int _simpleFortranRiverNumber = 1;

    public FortranRiverModelFactory()
    {
    }

    public FortranRiverModelFactory(int simpleFortranRiverNumber)
    {
      _simpleFortranRiverNumber = simpleFortranRiverNumber;
    }


    public LinkableEngine CreateRiverModel()
    {
      // Only create one of the fortran rivers, use the other river for the remainder
      _simpleRiverCounter++;
      if (_simpleRiverCounter == _simpleFortranRiverNumber)
        return (new SimpleRiverEngineWrapper());
      return (_netFactory.CreateRiverModel());
    }

    public List<IArgument> CreateRiverModelArguments(ITimeSpaceComponent model)
    {
      if (model is SimpleRiverEngineWrapper)
      {
        var arguments = new List<IArgument>();
        arguments.Add(Argument.Create("FilePath", @"..\..\..\..\..\Examples\SimpleFortranRiver\Data", false, "FilePath"));
        arguments.Add(Argument.Create("SimFileName", @"SimpleRiverUT.sim", false, "FilePath"));
        return (arguments);
      }
      return (_netFactory.CreateRiverModelArguments(model));
    }
  }
  

}
