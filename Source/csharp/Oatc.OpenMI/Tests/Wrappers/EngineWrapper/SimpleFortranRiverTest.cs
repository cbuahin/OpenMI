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
﻿using System.Collections.Generic;
using NUnit.Framework;
using Oatc.OpenMI.Examples.SimpleFortranRiver.Wrapper;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Wrappers.EngineWrapper.UnitTest
{
  /// <summary>
  /// The fortran river being tested as in <see cref="RiverModelTest"/>
  /// </summary>
  public class SimpleFortranRiverModelTest : RiverModelTest
  {
    [SetUp]
    public void Setup()
    {
      _riverModelFactory = new FortranRiverModelFactory();
      CreateModelsAndInputItemsForQuery();
    }

    [Ignore("Fortran river does not support spans")]
    public new void GetValues2A()
    {
      base.GetValues2A();
    }

    [Ignore("Fortran river does not support spans")]
    public new void GetValues2BInputAsSpans()
    {
      base.GetValues2BInputAsSpans();
    }

    [Test]
    public new void GetValues2BInputAsStamps()
    {
      // Fortran engine does not support running two instances 
      // at the same time, hence we check it as upper and lower river
      // in two runs

      // Run with fortran river as the upper river
      base.GetValues2BInputAsStamps();
      // Run with fortran river as the lower river
      _riverModelFactory = new FortranRiverModelFactory(2);
      base.GetValues2BInputAsSpans();
    }

    public new void GetValues2CInputAsStamps()
    {
      // Fortran engine does not support running two instances 
      // at the same time, hence we check it as upper and lower river
      // in two runs

      // Run with fortran river as the upper river
      base.GetValues2CInputAsStamps();
      // Run with fortran river as the lower river
      _riverModelFactory = new FortranRiverModelFactory(2);
      base.GetValues2CInputAsStamps();
    }

    [Ignore("Fortran wrapper can not handle two instances due to its global data handling")]
    public new void GetValues2CInputAsStampsStoredInItems()
    {
      base.GetValues2CInputAsStampsStoredInItems();
    }

  }


  /// <summary>
  /// The fortran river being tested as in <see cref="LinkableEnginesTest"/>
  /// </summary>

  [TestFixture]
  public class SimpleFortranRiverLinkingTest : LinkableEnginesTest
  {
    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _riverModelFactory = new FortranRiverModelFactory();
    }
  }
}
