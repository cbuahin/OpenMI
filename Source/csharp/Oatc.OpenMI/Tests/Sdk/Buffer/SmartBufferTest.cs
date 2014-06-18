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
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Sdk.Buffer.UnitTest
{
  /// <summary>
  /// The SmartBufferTest class serves as teting of all public methods in the
  /// Oatc.OpenMI.Sdk.Buffer.SmartBuffer class. The SmartBufferTest class 
  /// is used with the NUnit software.
  /// </summary>
  [TestFixture]
  public class SmartBufferTest
  {
    private static void WriteException(Exception e)
    {
      Console.WriteLine(" ");
      Console.WriteLine("------- System.Exception ----------------------------- ");
      Console.WriteLine("Catched in....: org.OpenMITest.Utilities.Wrapper.GetValues_River_Trigger_IDBased()");
      Console.WriteLine("Message.......: " + e.Message);
      Console.WriteLine("Stact trace...: " + e.StackTrace);
      Console.WriteLine("TargetSite....: " + e.TargetSite.Name);
      Console.WriteLine("Source........: " + e.Source);
      Console.WriteLine(" ");
    }

    [Test]
    public void AddValues_01()
    {
      try
      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.DoExtendedDataVerification = true;

        double[] values = new double[] { 0, 1, 2 };
        Time Time = new Time(1);

        smartBuffer.AddValues(Time, values);

        Time.StampAsModifiedJulianDay = 2;
        values[0] = 10;
        values[1] = 11;
        values[2] = 12;

        smartBuffer.AddValues(Time, values);

        smartBuffer.AddValues(new Time(3), new double[] { 110, 111, 112 });
        smartBuffer.AddValues(new Time(4), new double[] { 1110, 1111, 1112 });

        Assert.AreEqual(0, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(1, smartBuffer.GetValuesAt(0)[1]);
        Assert.AreEqual(2, smartBuffer.GetValuesAt(0)[2]);

        Assert.AreEqual(10, smartBuffer.GetValuesAt(1)[0]);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(1)[1]);
        Assert.AreEqual(12, smartBuffer.GetValuesAt(1)[2]);

        Assert.AreEqual(110, smartBuffer.GetValuesAt(2)[0]);
        Assert.AreEqual(111, smartBuffer.GetValuesAt(2)[1]);
        Assert.AreEqual(112, smartBuffer.GetValuesAt(2)[2]);

        Assert.AreEqual(1110, smartBuffer.GetValuesAt(3)[0]);
        Assert.AreEqual(1111, smartBuffer.GetValuesAt(3)[1]);
        Assert.AreEqual(1112, smartBuffer.GetValuesAt(3)[2]);

        Assert.AreEqual(4, smartBuffer.TimesCount);
        Assert.AreEqual(3, smartBuffer.ValuesCount);
      }
      catch (Exception e)
      {
        WriteException(e);
        throw (e);
      }
    }

    [Test]
    public void ClearAfter()
    {
      Time time = new Time();

      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.DoExtendedDataVerification = true;

        // --Populate the SmartBuffer --
        smartBuffer.AddValues(new Time(10), new double[] { 11, 21 });
        smartBuffer.AddValues(new Time(13), new double[] { 12, 22 });
        smartBuffer.AddValues(new Time(16), new double[] { 13, 23 });
        smartBuffer.AddValues(new Time(20), new double[] { 14, 24 });
        smartBuffer.AddValues(new Time(27), new double[] { 15, 25 });
        smartBuffer.AddValues(new Time(30), new double[] { 16, 26 });
        smartBuffer.AddValues(new Time(48), new double[] { 17, 27 });


        time.StampAsModifiedJulianDay = 50; // this time is after the last time in buffer
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(7, smartBuffer.TimesCount); //nothing removed
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(27, smartBuffer.GetValuesAt(6)[1]);

        time.StampAsModifiedJulianDay = 30;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(5, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(25, smartBuffer.GetValuesAt(4)[1]);

        time.StampAsModifiedJulianDay = 16.5;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(3, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(23, smartBuffer.GetValuesAt(2)[1]);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        smartBuffer.AddValues(new Time(10), new double[] { 11, 21 });
        smartBuffer.AddValues(new Time(13), new double[] { 12, 22 });
        smartBuffer.AddValues(new Time(16), new double[] { 13, 23 });
        smartBuffer.AddValues(new Time(20), new double[] { 14, 24 });
        smartBuffer.AddValues(new Time(27), new double[] { 15, 25 });
        smartBuffer.AddValues(new Time(30), new double[] { 16, 26 });
        smartBuffer.AddValues(new Time(48), new double[] { 17, 27 });

        time = new Time(50, 100);

        time.StampAsModifiedJulianDay = 50; // this time is after the last time in buffer
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(7, smartBuffer.TimesCount); //nothing removed
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(27, smartBuffer.GetValuesAt(6)[1]);

        time.StampAsModifiedJulianDay = 30;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(5, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(25, smartBuffer.GetValuesAt(4)[1]);

        time.StampAsModifiedJulianDay = 16.5;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(3, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(23, smartBuffer.GetValuesAt(2)[1]);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

      }
      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.AddValues(new Time(10, 3), new double[] {11, 21});
        smartBuffer.AddValues(new Time(13, 3), new double[] {12, 22});
        smartBuffer.AddValues(new Time(16, 4), new double[] {13, 23});
        smartBuffer.AddValues(new Time(20, 7), new double[] {14, 24});
        smartBuffer.AddValues(new Time(27, 3), new double[] {15, 25});
        smartBuffer.AddValues(new Time(30, 18), new double[] {16, 26});
        smartBuffer.AddValues(new Time(48, 17), new double[] {17, 27});


        time.StampAsModifiedJulianDay = 50; // this time is after the last time in buffer
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(7, smartBuffer.TimesCount); //nothing removed
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(27, smartBuffer.GetValuesAt(6)[1]);

        time.StampAsModifiedJulianDay = 30;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(5, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(25, smartBuffer.GetValuesAt(4)[1]);

        time.StampAsModifiedJulianDay = 16.5;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(3, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(23, smartBuffer.GetValuesAt(2)[1]);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        smartBuffer.AddValues(new Time(10, 3), new double[] {11, 21});
        smartBuffer.AddValues(new Time(13, 3), new double[] {12, 22});
        smartBuffer.AddValues(new Time(16, 4), new double[] {13, 23});
        smartBuffer.AddValues(new Time(20, 7), new double[] {14, 24});
        smartBuffer.AddValues(new Time(27, 3), new double[] {15, 25});
        smartBuffer.AddValues(new Time(30, 18), new double[] {16, 26});
        smartBuffer.AddValues(new Time(48, 17), new double[] {17, 27});

        time.StampAsModifiedJulianDay = 50; // this time is after the last time in buffer
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(7, smartBuffer.TimesCount); //nothing removed
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(27, smartBuffer.GetValuesAt(6)[1]);

        time.StampAsModifiedJulianDay = 30;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(5, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(25, smartBuffer.GetValuesAt(4)[1]);

        time.StampAsModifiedJulianDay = 16.5;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(3, smartBuffer.TimesCount);
        Assert.AreEqual(11, smartBuffer.GetValuesAt(0)[0]);
        Assert.AreEqual(23, smartBuffer.GetValuesAt(2)[1]);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);

        time.StampAsModifiedJulianDay = 9;
        smartBuffer.ClearAfter(time);
        Assert.AreEqual(0, smartBuffer.TimesCount);
      }
    }

    [Test]
    public void ClearBefore()
    {
      SmartBuffer timeBuffer = new SmartBuffer();
      timeBuffer.AddValues(new Time(1), new double[] { 1.1, 2.1, 3.1 });
      timeBuffer.AddValues(new Time(3), new double[] { 4.1, 5.1, 6.1 });
      timeBuffer.AddValues(new Time(4), new double[] { 7.1, 8.1, 9.1 });
      timeBuffer.AddValues(new Time(5), new double[] { 10.1, 11.1, 12.1 });

      timeBuffer.ClearBefore(new Time(0.5));
      Assert.AreEqual(4, timeBuffer.TimesCount);
      timeBuffer.CheckBuffer();

      // Must keep also first value, until time 3
      timeBuffer.ClearBefore(new Time(1));
      Assert.AreEqual(4, timeBuffer.TimesCount);
      timeBuffer.CheckBuffer();

      timeBuffer.ClearBefore(new Time(1.1));
      Assert.AreEqual(4, timeBuffer.TimesCount);
      Assert.AreEqual(1.1, timeBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(1, (timeBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      timeBuffer.CheckBuffer();

      // Check for rounding errors - should only remove 1
      timeBuffer.ClearBefore(new Time(4));
      Assert.AreEqual(3, timeBuffer.TimesCount);
      timeBuffer.CheckBuffer();

      // Check for rounding errors - should not remove any further values
      timeBuffer.ClearBefore(new Time(4 + 1e-10));
      Assert.AreEqual(3, timeBuffer.TimesCount);
      timeBuffer.CheckBuffer();

      // Removing one more value
      timeBuffer.ClearBefore(new Time(4.1));
      Assert.AreEqual(2, timeBuffer.TimesCount);
      Assert.AreEqual(7.1, timeBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(4, (timeBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      timeBuffer.CheckBuffer();

      timeBuffer.ClearBefore(new Time(5.1));
      Assert.AreEqual(1, timeBuffer.TimesCount);
      Assert.AreEqual(10.1, timeBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(5, (timeBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      timeBuffer.CheckBuffer();


      /// The buffer is a bit conservative when removing span data - always keeping
      /// one buffer back in time, even though that is never going to be used.

      
      SmartBuffer spanBuffer = new SmartBuffer();
      spanBuffer.AddValues(new Time(1, 2), new double[] { 1.1, 2.1, 3.1 });
      spanBuffer.AddValues(new Time(3, 2), new double[] { 4.1, 5.1, 6.1 });
      spanBuffer.AddValues(new Time(5, 2), new double[] { 7.1, 8.1, 9.1 });
      spanBuffer.AddValues(new Time(7, 2), new double[] { 10.1, 11.1, 12.1 });

      spanBuffer.ClearBefore(new Time(0.5));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      spanBuffer.ClearBefore(new Time(1.0));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      spanBuffer.ClearBefore(new Time(2.0));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      spanBuffer.ClearBefore(new Time(3.0));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      // this could remove the first interval, though it does not
      spanBuffer.ClearBefore(new Time(4.0));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      Assert.AreEqual(1.1, spanBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(1, (spanBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      spanBuffer.CheckBuffer();


      // Check for rounding errors - should not remove anything
      spanBuffer.ClearBefore(new Time(5.0));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      // Check for rounding errors - should not remove anything
      spanBuffer.ClearBefore(new Time(5.0 + 1e-10));
      Assert.AreEqual(4, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      // Check for rounding errors - now one item should be removed
      spanBuffer.ClearBefore(new Time(5.1));
      Assert.AreEqual(3, spanBuffer.TimesCount);
      spanBuffer.CheckBuffer();

      spanBuffer.ClearBefore(new Time(7.0));
      Assert.AreEqual(3, spanBuffer.TimesCount);
      Assert.AreEqual(4.1, spanBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(3, (spanBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      spanBuffer.CheckBuffer();

      spanBuffer.ClearBefore(new Time(10.0));
      Assert.AreEqual(1, spanBuffer.TimesCount);
      Assert.AreEqual(10.1, spanBuffer.GetValuesAt(0)[0]);
      Assert.AreEqual(7, (spanBuffer.GetTimeAt(0)).StampAsModifiedJulianDay);
      spanBuffer.CheckBuffer();
    }

    [Test]
    public void GetValues_TimeSpansToTimeSpans()
    {
      // Test with one value in buffer
      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.AddValues(new Time(8, 4), new double[] { 1 });
        smartBuffer.RelaxationFactor = 1.0;

        for (int i = 0; i < 2; i++)
        {
          // Same result with linear interpolation, due to only one value in buffer
          if (i == 1)
            smartBuffer.RelaxationFactor = 0;

          smartBuffer.DoExtrapolate = true;

          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(0, 4))[0]);
          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(6, 4))[0]);
          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(10, 4))[0]);
          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(14, 4))[0]);
          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(6, 8))[0]);
          Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(9, 2))[0]);

          smartBuffer.DoExtrapolate = false;

          // If a value of zero is assumed outside the spans
          //Assert.AreEqual(0.0, smartBuffer.GetValues(new Time(0, 4))[0]);
          //Assert.AreEqual(0.5, smartBuffer.GetValues(new Time(6, 4))[0]);
          //Assert.AreEqual(0.5, smartBuffer.GetValues(new Time(10, 4))[0]);
          //Assert.AreEqual(0.0, smartBuffer.GetValues(new Time(14, 4))[0]);
          //Assert.AreEqual(0.5, smartBuffer.GetValues(new Time(6, 8))[0]);
          //Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(9, 2))[0]);
        }
      }

      // Testing extrapolation - equidistant spans
      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.AddValues(new Time(10, 2), new double[] { 11 });
        smartBuffer.AddValues(new Time(12, 2), new double[] { 13 });

        smartBuffer.RelaxationFactor = 0;
        smartBuffer.DoExtrapolate = true;

        Assert.AreEqual(7.5, smartBuffer.GetValues(new Time(7, 1))[0]);
        Assert.AreEqual(8.5, smartBuffer.GetValues(new Time(8, 1))[0]);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1-1e-8))[0], 1e-6);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1))[0]);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1+1e-8))[0], 1e-6);

        Assert.AreEqual(16.5, smartBuffer.GetValues(new Time(16, 1))[0]);
        Assert.AreEqual(15.5, smartBuffer.GetValues(new Time(15, 1))[0]);
        Assert.AreEqual(14.5, smartBuffer.GetValues(new Time(14 + 1e-8, 1))[0], 1e-6);
        Assert.AreEqual(14.5, smartBuffer.GetValues(new Time(14, 1))[0]);
        Assert.AreEqual(14.5, smartBuffer.GetValues(new Time(14 - 1e-8, 1 + 1e-8))[0], 1e-6);

        double rf = 0.3;
        double ri = 1-rf;
        smartBuffer.RelaxationFactor = rf;
        Assert.AreEqual(7.5*ri+11*rf, smartBuffer.GetValues(new Time(7, 1))[0]);
        Assert.AreEqual(8.5*ri+11*rf, smartBuffer.GetValues(new Time(8, 1))[0]);
        Assert.AreEqual(9.5*ri+11*rf, smartBuffer.GetValues(new Time(9, 1 - 1e-8))[0], 1e-6);
        Assert.AreEqual(9.5*ri+11*rf, smartBuffer.GetValues(new Time(9, 1))[0]);
        Assert.AreEqual(9.5*ri+11*rf, smartBuffer.GetValues(new Time(9, 1 + 1e-8))[0], 1e-6);

        Assert.AreEqual(16.5*ri+13*rf, smartBuffer.GetValues(new Time(16, 1))[0]);
        Assert.AreEqual(15.5*ri+13*rf, smartBuffer.GetValues(new Time(15, 1))[0]);
        Assert.AreEqual(14.5*ri+13*rf, smartBuffer.GetValues(new Time(14 + 1e-8, 1))[0], 1e-6);
        Assert.AreEqual(14.5*ri+13*rf, smartBuffer.GetValues(new Time(14, 1))[0], 1e-14);
        Assert.AreEqual(14.5*ri+13*rf, smartBuffer.GetValues(new Time(14 - 1e-8, 1 + 1e-8))[0], 1e-6);
      }

      // Testing extrapolation - non-equidistant spans
      {
        SmartBuffer smartBuffer = new SmartBuffer();
        smartBuffer.AddValues(new Time(10, 2), new double[] { 11 });
        smartBuffer.AddValues(new Time(12, 4), new double[] { 14 });

        smartBuffer.RelaxationFactor = 0;
        smartBuffer.DoExtrapolate = true;

        Assert.AreEqual(7.5, smartBuffer.GetValues(new Time(7, 1))[0]);
        Assert.AreEqual(8.5, smartBuffer.GetValues(new Time(8, 1))[0]);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1-1e-8))[0], 1e-6);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1))[0]);
        Assert.AreEqual(9.5, smartBuffer.GetValues(new Time(9, 1+1e-8))[0], 1e-6);

        Assert.AreEqual(18.5, smartBuffer.GetValues(new Time(18, 1))[0]);
        Assert.AreEqual(17.5, smartBuffer.GetValues(new Time(17, 1))[0]);
        Assert.AreEqual(16.5, smartBuffer.GetValues(new Time(16 + 1e-8, 1))[0], 1e-6);
        Assert.AreEqual(16.5, smartBuffer.GetValues(new Time(16, 1))[0], 1e-6);
        Assert.AreEqual(16.5, smartBuffer.GetValues(new Time(16 - 1e-8, 1+1e-8))[0], 1e-6);

        smartBuffer.RelaxationFactor = 0.5;
        Assert.AreEqual((7.5+11)*0.5, smartBuffer.GetValues(new Time(7, 1))[0]);
        Assert.AreEqual((8.5+11)*0.5, smartBuffer.GetValues(new Time(8, 1))[0]);
        Assert.AreEqual((9.5+11)*0.5, smartBuffer.GetValues(new Time(9, 1 - 1e-8))[0], 1e-6);
        Assert.AreEqual((9.5+11)*0.5, smartBuffer.GetValues(new Time(9, 1))[0]);
        Assert.AreEqual((9.5+11)*0.5, smartBuffer.GetValues(new Time(9, 1 + 1e-8))[0], 1e-6);

        Assert.AreEqual((18.5+14)*0.5, smartBuffer.GetValues(new Time(18, 1))[0]);
        Assert.AreEqual((17.5+14)*0.5, smartBuffer.GetValues(new Time(17, 1))[0]);
        Assert.AreEqual((16.5+14)*0.5, smartBuffer.GetValues(new Time(16 + 1e-8, 1))[0], 1e-6);
        Assert.AreEqual((16.5+14)*0.5, smartBuffer.GetValues(new Time(16, 1))[0], 1e-6);
        Assert.AreEqual((16.5+14)*0.5, smartBuffer.GetValues(new Time(16 - 1e-8, 1 + 1e-8))[0], 1e-6);
      }

      // Test with several values in buffer
      {
        SmartBuffer smartBuffer = new SmartBuffer();

        smartBuffer.AddValues(new Time(10, 2), new double[] { 1 });
        smartBuffer.AddValues(new Time(12, 2), new double[] { 2 });
        smartBuffer.AddValues(new Time(14, 2), new double[] { 3 });
        smartBuffer.AddValues(new Time(16, 6), new double[] { 4 });
        smartBuffer.AddValues(new Time(22, 8), new double[] { 5 });
        smartBuffer.AddValues(new Time(30, 2), new double[] { 6 });

        smartBuffer.RelaxationFactor = 1.0;
        smartBuffer.DoExtrapolate = true;

        Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(7, 2))[0]);
        Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(9, 2))[0]);
        Assert.AreEqual(1.25, smartBuffer.GetValues(new Time(9, 4))[0]);
        Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(10, 2))[0]);
        Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(11, 1))[0]);
        Assert.AreEqual(2.5, smartBuffer.GetValues(new Time(12, 4))[0]);
        Assert.AreEqual(2.5, smartBuffer.GetValues(new Time(13, 2))[0]);
        Assert.AreEqual(5.5, smartBuffer.GetValues(new Time(28, 4))[0]);
        Assert.AreEqual(5.75, smartBuffer.GetValues(new Time(28, 8))[0]);
        Assert.AreEqual(6, smartBuffer.GetValues(new Time(31, 2))[0]);
        Assert.AreEqual(6, smartBuffer.GetValues(new Time(33, 2))[0]);

        smartBuffer.DoExtrapolate = false;
        
        // If a value of zero is assumed outside the spans
        //Assert.AreEqual(0.0, smartBuffer.GetValues(new Time(7, 2))[0]);
        //Assert.AreEqual(0.5, smartBuffer.GetValues(new Time(9, 2))[0]);
        //Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(9, 4))[0]);
        //Assert.AreEqual(1.0, smartBuffer.GetValues(new Time(10, 2))[0]);
        //Assert.AreEqual(2.75, smartBuffer.GetValues(new Time(28, 8))[0]);
        //Assert.AreEqual(3, smartBuffer.GetValues(new Time(31, 2))[0]);
        //Assert.AreEqual(0, smartBuffer.GetValues(new Time(33, 2))[0]);

        smartBuffer.RelaxationFactor = 0;
        smartBuffer.DoExtrapolate = true;
        Assert.AreEqual(-0.75, smartBuffer.GetValues(new Time(7, 1))[0]);
        Assert.AreEqual(-0.25, smartBuffer.GetValues(new Time(8, 1))[0]);
        Assert.AreEqual(-0.5, smartBuffer.GetValues(new Time(7, 2))[0]);
        Assert.AreEqual(0.25, smartBuffer.GetValues(new Time(9, 1))[0]);
        Assert.AreEqual(0.25, smartBuffer.GetValues(new Time(9, 1.000000001))[0], 1e-6);
        Assert.AreEqual((0.25+1)*0.5, smartBuffer.GetValues(new Time(9, 2))[0]);
        Assert.AreEqual(0.25*0.25+1*0.5+2*0.25, smartBuffer.GetValues(new Time(9, 4))[0]);

        Assert.AreEqual(6.7, smartBuffer.GetValues(new Time(34, 1))[0]);
        Assert.AreEqual(6.5, smartBuffer.GetValues(new Time(33, 1))[0]);
        Assert.AreEqual(6.3, smartBuffer.GetValues(new Time(32, 1))[0]);
        Assert.AreEqual(6.3, smartBuffer.GetValues(new Time(32-1e-8, 1+1e-8))[0], 1e-7);

        Assert.AreEqual(0.5*6 + 0.5*6.3, smartBuffer.GetValues(new Time(31, 2))[0]);
        Assert.AreEqual(0.25*5 + 0.25*6 + 0.5*6.6, smartBuffer.GetValues(new Time(28, 8))[0]);

        smartBuffer.RelaxationFactor = 0.5;
        Assert.AreEqual((0.25+1)*0.25+0.5, smartBuffer.GetValues(new Time(9, 2))[0]);
        Assert.AreEqual(0.5*(0.25*0.25+1*0.5+2*0.25 + 1.25), smartBuffer.GetValues(new Time(9, 4))[0]);

        Assert.AreEqual(0.25*6 + 0.25*6.3 + 3, smartBuffer.GetValues(new Time(31, 2))[0]);
        Assert.AreEqual(0.5*(0.25*5 + 0.25*6 + 0.5*6.6 + 5.75), smartBuffer.GetValues(new Time(28, 8))[0]);
      }

    }

    [Test]
    public void GetValues_TimeSpansToTimeStamp_01()
    {
      //-------------------------------------------------------------------------------------------------
      // Testing Getvalues when buffer contains Times and the requested value corresponds to a
      // Time. Three different relaxation factors are used.
      //-------------------------------------------------------------------------------------------------
      {
        SmartBuffer smartBuffer = new SmartBuffer();

        smartBuffer.AddValues(new Time(10, 2), new double[] { 11 });
        smartBuffer.AddValues(new Time(12, 2), new double[] { 13 });

        smartBuffer.RelaxationFactor = 0.0;
        // TODO: Smartbuffer implementation is not updated for the new extrapolation definition
        Assert.Fail("Smartbuffer implementation is not updated for the new extrapolation definition");
        Assert.AreEqual(4, smartBuffer.GetValues(new Time(4))[0]);
        Assert.AreEqual(9, smartBuffer.GetValues(new Time(9))[0]);
        Assert.AreEqual(9.9, smartBuffer.GetValues(new Time(9.9))[0]);
        Assert.AreEqual(9.99, smartBuffer.GetValues(new Time(9.99))[0]);
        Assert.AreEqual(10, smartBuffer.GetValues(new Time(10))[0]);
        Assert.AreEqual(10, smartBuffer.GetValues(new Time(10.1))[0]);
      
      }
      {
        SmartBuffer smartBuffer = new SmartBuffer();

        smartBuffer.AddValues(new Time(10, 3), new double[] {5, 11.5, 5});
        smartBuffer.AddValues(new Time(13, 3), new double[] {7, 14.5, 5});
        smartBuffer.AddValues(new Time(16, 4), new double[] {9, 18, 5});
        smartBuffer.AddValues(new Time(20, 7), new double[] {2, 23.5, 5});
        smartBuffer.AddValues(new Time(27, 3), new double[] {-5, 28.5, 5});
        smartBuffer.AddValues(new Time(30, 18), new double[] {7, 39, 5});

        smartBuffer.RelaxationFactor = 0.0;
        Assert.AreEqual(new double[] {1, 4, 5}, smartBuffer.GetValues(new Time(4)));
        Assert.AreEqual(new double[] {3, 7, 5}, smartBuffer.GetValues(new Time(7)));
        Assert.AreEqual(new double[] {5, 12.5, 5}, smartBuffer.GetValues(new Time(10)));
        Assert.AreEqual(new double[] {5, 12.5, 5}, smartBuffer.GetValues(new Time(11)));
        Assert.AreEqual(new double[] {5, 12.5, 5}, smartBuffer.GetValues(new Time(12)));
        Assert.AreEqual(new double[] {7, 15.5, 5}, smartBuffer.GetValues(new Time(13)));
        Assert.AreEqual(new double[] {7, 15.5, 5}, smartBuffer.GetValues(new Time(14)));
        Assert.AreEqual(new double[] {7, 15.5, 5}, smartBuffer.GetValues(new Time(15)));
        Assert.AreEqual(new double[] {9, 18, 5}, smartBuffer.GetValues(new Time(16)));
        Assert.AreEqual(new double[] {9, 18, 5}, smartBuffer.GetValues(new Time(17)));
        Assert.AreEqual(new double[] {2, 23.5, 5}, smartBuffer.GetValues(new Time(20)));
        Assert.AreEqual(new double[] {2, 23.5, 5}, smartBuffer.GetValues(new Time(21)));
        Assert.AreEqual(new double[] {2, 23.5, 5}, smartBuffer.GetValues(new Time(23)));
        Assert.AreEqual(new double[] {-5, 28.5, 5}, smartBuffer.GetValues(new Time(27)));
        Assert.AreEqual(new double[] {-5, 28.5, 5}, smartBuffer.GetValues(new Time(28)));
        Assert.AreEqual(new double[] {-5, 28.5, 5}, smartBuffer.GetValues(new Time(29)));
        Assert.AreEqual(new double[] {7, 39, 5}, smartBuffer.GetValues(new Time(30)));
        Assert.AreEqual(new double[] {7, 39, 5}, smartBuffer.GetValues(new Time(33)));
        Assert.AreEqual(new double[] {7, 39, 5}, smartBuffer.GetValues(new Time(36)));
        Assert.AreEqual(new double[] {7, 39, 5}, smartBuffer.GetValues(new Time(42)));
        Assert.AreEqual(new double[] {7, 39, 5}, smartBuffer.GetValues(new Time(48)));
        Assert.AreEqual(new double[] {11, 54, 5}, smartBuffer.GetValues(new Time(54)));
        Assert.AreEqual(new double[] {15, 60, 5}, smartBuffer.GetValues(new Time(60)));

        smartBuffer.RelaxationFactor = 1.0;

        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(4)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(7)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(10)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(11)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(12)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(13)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(14)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(15)));
        Assert.AreEqual(new double[] {9, 10, 5}, smartBuffer.GetValues(new Time(16)));
        Assert.AreEqual(new double[] {9, 10, 5}, smartBuffer.GetValues(new Time(17)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(20)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(21)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(23)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(27)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(28)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(29)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(30)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(33)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(36)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(42)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(48)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(54)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(60)));

        const double a = 0.7;
        smartBuffer.RelaxationFactor = a;

        Assert.AreEqual(new double[] {5 - (1 - a)*(5 - 1), 12 - (1 - a)*(12 - 14), 5},
                        smartBuffer.GetValues(new Time(4)));
        Assert.AreEqual(new double[] {5 - (1 - a)*(5 - 3), 12 - (1 - a)*(12 - 13), 5},
                        smartBuffer.GetValues(new Time(7)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(10)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(11)));
        Assert.AreEqual(new double[] {5, 12, 5}, smartBuffer.GetValues(new Time(12)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(13)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(14)));
        Assert.AreEqual(new double[] {7, 11, 5}, smartBuffer.GetValues(new Time(15)));
        Assert.AreEqual(new double[] {9, 10, 5}, smartBuffer.GetValues(new Time(16)));
        Assert.AreEqual(new double[] {9, 10, 5}, smartBuffer.GetValues(new Time(17)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(20)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(21)));
        Assert.AreEqual(new double[] {2, 7, 5}, smartBuffer.GetValues(new Time(23)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(27)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(28)));
        Assert.AreEqual(new double[] {-5, 6, 5}, smartBuffer.GetValues(new Time(29)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(30)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(33)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(36)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(42)));
        Assert.AreEqual(new double[] {7, 3, 5}, smartBuffer.GetValues(new Time(48)));
        Assert.AreEqual(new double[] {7 + (1 - a)*(11 - 7), 3 + (1 - a)*(2 - 3), 5},
                        smartBuffer.GetValues(new Time(54)));
        Assert.AreEqual(new double[] {7 + (1 - a)*(15 - 7), 3 + (1 - a)*(1 - 3), 5},
                        smartBuffer.GetValues(new Time(60)));
      }
    }

    [Test]
    public void GetValues_TimeStampsToTimeSpan_01()
    {
      SmartBuffer smartBuffer = new SmartBuffer();
      smartBuffer.AddValues(new Time(2), new double[] { 2 });
      smartBuffer.AddValues(new Time(4), new double[] { 4 });
      smartBuffer.AddValues(new Time(7), new double[] { 4 });
      smartBuffer.AddValues(new Time(11), new double[] { 6 });
      smartBuffer.AddValues(new Time(13), new double[] { 4 });
      smartBuffer.AddValues(new Time(15), new double[] { 3 });

      smartBuffer.RelaxationFactor = 1.0;

      Assert.AreEqual(3.25, smartBuffer.GetValues(new Time(3, 0.5))[0], 0.0000000001);
      Assert.AreEqual(24.5 / 6, smartBuffer.GetValues(new Time(3, 6))[0], 0.0000000001);
      Assert.AreEqual(49.25 / 11, smartBuffer.GetValues(new Time(3, 11))[0], 0.0000000001);
      Assert.AreEqual(13.0 / 4.0, smartBuffer.GetValues(new Time(13, 4))[0]);

      smartBuffer.RelaxationFactor = 0.0;
      Assert.AreEqual(3, smartBuffer.GetValues(new Time(13, 4))[0]);

      smartBuffer.RelaxationFactor = 1.0;
      Assert.AreEqual(3, smartBuffer.GetValues(new Time(1, 4))[0]);
    }

    [Test] // testing the Initialise method
    public void GetValues_TimeStampsToTimeStamp_01()
    {
      SmartBuffer smartBuffer = new SmartBuffer();
      smartBuffer.AddValues(new Time(1), new double[] { 1, 2, 3 });
      smartBuffer.AddValues(new Time(3), new double[] { 3, 4, 5 });

      // Extrapolation (piecwise-constant)
      Assert.AreEqual(new double[] { 1, 2, 3 }, smartBuffer.GetValues(new Time(0)));

      // "Hit" first Time
      Assert.AreEqual(new double[] { 1, 2, 3 }, smartBuffer.GetValues(new Time(1)));

      // Interpolation
      Assert.AreEqual(new double[] { 2, 3, 4 }, smartBuffer.GetValues(new Time(2)));

      // "Hit" last Time
      Assert.AreEqual(new double[] { 3, 4, 5 }, smartBuffer.GetValues(new Time(3)));

      // Extrapolation (piecwise-constant)
      Assert.AreEqual(new double[] { 3, 4, 5 }, smartBuffer.GetValues(new Time(4)));

      // Extrapolation
      // Assert.AreEqual(new double[] { (3-1)/(3-1)*(1-a)+3, (4-2)/(3-1)*(1-a)+4, (5-3)/(3-1)*(1-a)+5 },smartBuffer.GetValues(new Time(4)));
    }

    [Test] // testing the Initialise method
    public void GetValues_TimeStampsToTimeStamp_02()
    {
      // GetValues_TimesToTime_02()method is teting situation where the Times in
      // the buffer is of type ITime and the requested values is associated to a ITime
      //
      // Three different relaxation factors are used


      // See drawing at the following link.
      //		http://projects.dhi.dk/harmonIT/WP6/SourceCodeDocumentation/Oatc.OpenMI.Sdk.Buffer.TestCode.htm


      SmartBuffer smartBuffer = new SmartBuffer();


      // --Populate the SmartBuffer --
      smartBuffer.AddValues(new Time(10), new double[] { 11, 10, 6 });
      smartBuffer.AddValues(new Time(13), new double[] { 5, 13, 6 });
      smartBuffer.AddValues(new Time(16), new double[] { 2, 16, 6 });
      smartBuffer.AddValues(new Time(20), new double[] { 2, 20, 6 });
      smartBuffer.AddValues(new Time(27), new double[] { 2, 27, 6 });
      smartBuffer.AddValues(new Time(30), new double[] { -4, 30, 6 });
      smartBuffer.AddValues(new Time(48), new double[] { 8, 48, 6 });

      smartBuffer.RelaxationFactor = 0.0;

      Assert.AreEqual(new double[] { 13, 9, 6 }, smartBuffer.GetValues(new Time(9)));
      Assert.AreEqual(new double[] { 11, 10, 6 }, smartBuffer.GetValues(new Time(10)));
      Assert.AreEqual(new double[] { 9, 11, 6 }, smartBuffer.GetValues(new Time(11)));
      Assert.AreEqual(new double[] { 7, 12, 6 }, smartBuffer.GetValues(new Time(12)));
      Assert.AreEqual(new double[] { 5, 13, 6 }, smartBuffer.GetValues(new Time(13)));
      Assert.AreEqual(new double[] { 4, 14, 6 }, smartBuffer.GetValues(new Time(14)));
      Assert.AreEqual(new double[] { 3, 15, 6 }, smartBuffer.GetValues(new Time(15)));
      Assert.AreEqual(new double[] { 2, 16, 6 }, smartBuffer.GetValues(new Time(16)));
      Assert.AreEqual(new double[] { 2, 17, 6 }, smartBuffer.GetValues(new Time(17)));
      Assert.AreEqual(new double[] { 2, 20, 6 }, smartBuffer.GetValues(new Time(20)));
      Assert.AreEqual(new double[] { 2, 21, 6 }, smartBuffer.GetValues(new Time(21)));
      Assert.AreEqual(new double[] { 2, 23, 6 }, smartBuffer.GetValues(new Time(23)));
      Assert.AreEqual(new double[] { 2, 27, 6 }, smartBuffer.GetValues(new Time(27)));
      Assert.AreEqual(new double[] { 0, 28, 6 }, smartBuffer.GetValues(new Time(28)));
      Assert.AreEqual(new double[] { -2, 29, 6 }, smartBuffer.GetValues(new Time(29)));
      Assert.AreEqual(new double[] { -4, 30, 6 }, smartBuffer.GetValues(new Time(30)));
      Assert.AreEqual(new double[] { -2, 33, 6 }, smartBuffer.GetValues(new Time(33)));
      Assert.AreEqual(new double[] { 0, 36, 6 }, smartBuffer.GetValues(new Time(36)));
      Assert.AreEqual(new double[] { 4, 42, 6 }, smartBuffer.GetValues(new Time(42)));
      Assert.AreEqual(new double[] { 8, 48, 6 }, smartBuffer.GetValues(new Time(48)));
      Assert.AreEqual(new double[] { 12, 54, 6 }, smartBuffer.GetValues(new Time(54)));

      smartBuffer.RelaxationFactor = 1.0;

      Assert.AreEqual(new double[] { 11, 10, 6 }, smartBuffer.GetValues(new Time(9)));
      Assert.AreEqual(new double[] { 11, 10, 6 }, smartBuffer.GetValues(new Time(10)));
      Assert.AreEqual(new double[] { 9, 11, 6 }, smartBuffer.GetValues(new Time(11)));
      Assert.AreEqual(new double[] { 7, 12, 6 }, smartBuffer.GetValues(new Time(12)));
      Assert.AreEqual(new double[] { 5, 13, 6 }, smartBuffer.GetValues(new Time(13)));
      Assert.AreEqual(new double[] { 4, 14, 6 }, smartBuffer.GetValues(new Time(14)));
      Assert.AreEqual(new double[] { 3, 15, 6 }, smartBuffer.GetValues(new Time(15)));
      Assert.AreEqual(new double[] { 2, 16, 6 }, smartBuffer.GetValues(new Time(16)));
      Assert.AreEqual(new double[] { 2, 17, 6 }, smartBuffer.GetValues(new Time(17)));
      Assert.AreEqual(new double[] { 2, 20, 6 }, smartBuffer.GetValues(new Time(20)));
      Assert.AreEqual(new double[] { 2, 21, 6 }, smartBuffer.GetValues(new Time(21)));
      Assert.AreEqual(new double[] { 2, 23, 6 }, smartBuffer.GetValues(new Time(23)));
      Assert.AreEqual(new double[] { 2, 27, 6 }, smartBuffer.GetValues(new Time(27)));
      Assert.AreEqual(new double[] { 0, 28, 6 }, smartBuffer.GetValues(new Time(28)));
      Assert.AreEqual(new double[] { -2, 29, 6 }, smartBuffer.GetValues(new Time(29)));
      Assert.AreEqual(new double[] { -4, 30, 6 }, smartBuffer.GetValues(new Time(30)));
      Assert.AreEqual(new double[] { -2, 33, 6 }, smartBuffer.GetValues(new Time(33)));
      Assert.AreEqual(new double[] { 0, 36, 6 }, smartBuffer.GetValues(new Time(36)));
      Assert.AreEqual(new double[] { 4, 42, 6 }, smartBuffer.GetValues(new Time(42)));
      Assert.AreEqual(new double[] { 8, 48, 6 }, smartBuffer.GetValues(new Time(48)));
      Assert.AreEqual(new double[] { 8, 48, 6 }, smartBuffer.GetValues(new Time(54)));

      const double a = 0.7;
      smartBuffer.RelaxationFactor = a;

      Assert.AreEqual(new double[] { 11 + (1 - a) * 2, 10 - (1 - a) * 1, 6 }, smartBuffer.GetValues(new Time(9)));
      Assert.AreEqual(new double[] { 11, 10, 6 }, smartBuffer.GetValues(new Time(10)));
      Assert.AreEqual(new double[] { 9, 11, 6 }, smartBuffer.GetValues(new Time(11)));
      Assert.AreEqual(new double[] { 7, 12, 6 }, smartBuffer.GetValues(new Time(12)));
      Assert.AreEqual(new double[] { 5, 13, 6 }, smartBuffer.GetValues(new Time(13)));
      Assert.AreEqual(new double[] { 4, 14, 6 }, smartBuffer.GetValues(new Time(14)));
      Assert.AreEqual(new double[] { 3, 15, 6 }, smartBuffer.GetValues(new Time(15)));
      Assert.AreEqual(new double[] { 2, 16, 6 }, smartBuffer.GetValues(new Time(16)));
      Assert.AreEqual(new double[] { 2, 17, 6 }, smartBuffer.GetValues(new Time(17)));
      Assert.AreEqual(new double[] { 2, 20, 6 }, smartBuffer.GetValues(new Time(20)));
      Assert.AreEqual(new double[] { 2, 21, 6 }, smartBuffer.GetValues(new Time(21)));
      Assert.AreEqual(new double[] { 2, 23, 6 }, smartBuffer.GetValues(new Time(23)));
      Assert.AreEqual(new double[] { 2, 27, 6 }, smartBuffer.GetValues(new Time(27)));
      Assert.AreEqual(new double[] { 0, 28, 6 }, smartBuffer.GetValues(new Time(28)));
      Assert.AreEqual(new double[] { -2, 29, 6 }, smartBuffer.GetValues(new Time(29)));
      Assert.AreEqual(new double[] { -4, 30, 6 }, smartBuffer.GetValues(new Time(30)));
      Assert.AreEqual(new double[] { -2, 33, 6 }, smartBuffer.GetValues(new Time(33)));
      Assert.AreEqual(new double[] { 0, 36, 6 }, smartBuffer.GetValues(new Time(36)));
      Assert.AreEqual(new double[] { 4, 42, 6 }, smartBuffer.GetValues(new Time(42)));
      Assert.AreEqual(new double[] { 8, 48, 6 }, smartBuffer.GetValues(new Time(48)));
      Assert.AreEqual(new double[] { 8 + (1 - a) * 4, 48 + (1 - a) * 6, 6 }, smartBuffer.GetValues(new Time(54)));
    }


    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void GetValues_TimeStampsToTimeStamp_05()
    {
      //-------------------------------------------------------------------------------------------------
      // Only two ValueSets in buffer
      //-------------------------------------------------------------------------------------------------

      SmartBuffer smartBuffer = new SmartBuffer();

      smartBuffer.RelaxationFactor = 0;

      smartBuffer.AddValues(new Time(10), new double[] { 11, 10, 6 });
      smartBuffer.AddValues(new Time(13), new double[] { 5, 13, 6 });

      Assert.AreEqual(new double[] { 13, 9, 6 }, smartBuffer.GetValues(new Time(9)));
      Assert.AreEqual(new double[] { 11, 10, 6 }, smartBuffer.GetValues(new Time(10)));
      Assert.AreEqual(new double[] { 9, 11, 6 }, smartBuffer.GetValues(new Time(11)));
      Assert.AreEqual(new double[] { 7, 12, 6 }, smartBuffer.GetValues(new Time(12)));
      Assert.AreEqual(new double[] { 5, 13, 6 }, smartBuffer.GetValues(new Time(13)));
      Assert.AreEqual(new double[] { 3, 14, 6 }, smartBuffer.GetValues(new Time(14)));
    }

    [Test]
    public void GetValues_TimeStampsToTimeStamp_06()
    {
      //-------------------------------------------------------------------------------------------------
      // Only one ValueSets in buffer
      //-------------------------------------------------------------------------------------------------

      SmartBuffer smartBuffer = new SmartBuffer();

      smartBuffer.RelaxationFactor = 0;

      smartBuffer.AddValues(new Time(10), new double[] { 11, 2, 6 });

      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(9)));
      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(10)));
      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(11)));
      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(12)));
      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(13)));
      Assert.AreEqual(new double[] { 11, 2, 6 }, smartBuffer.GetValues(new Time(14)));
    }

    [Test]
    public void SmartBuffer()
    {
      // Testing the overloaded constructor SmartBuffer.SmartBuffer(SmartBuffer buffer)
      // Note: this test does not include testing for buffers containing VectorSets

      SmartBuffer smartBufferA = new SmartBuffer();
      smartBufferA.AddValues(new Time(1), new double[] { 1, 2, 3 });
      smartBufferA.AddValues(new Time(3), new double[] { 3, 4, 5 });
      smartBufferA.AddValues(new Time(6), new double[] { 6, 7, 8 });

      SmartBuffer buffer1 = new SmartBuffer(smartBufferA);
      Assert.AreEqual(smartBufferA.TimesCount, buffer1.TimesCount);
      for (int i = 0; i < smartBufferA.TimesCount; i++)
      {
        Assert.AreEqual(smartBufferA.GetTimeAt(i).StampAsModifiedJulianDay,
                        buffer1.GetTimeAt(i).StampAsModifiedJulianDay);
        for (int n = 0; n < smartBufferA.ValuesCount; n++)
        {
          Assert.AreEqual(smartBufferA.GetValuesAt(i)[n], buffer1.GetValuesAt(i)[n]);
        }
      }

      SmartBuffer smartBufferB = new SmartBuffer();
      smartBufferB.AddValues(new Time(1, 1), new double[] { 11, 12, 13 });
      smartBufferB.AddValues(new Time(2, 1), new double[] { 13, 14, 15 });
      smartBufferB.AddValues(new Time(3, 2), new double[] { 16, 17, 18 });

      SmartBuffer buffer2 = new SmartBuffer(smartBufferB);
      Assert.AreEqual(smartBufferB.TimesCount, buffer2.TimesCount);
      for (int i = 0; i < smartBufferB.TimesCount; i++)
      {
        Assert.AreEqual(smartBufferB.GetTimeAt(i).StampAsModifiedJulianDay,
                        buffer2.GetTimeAt(i).StampAsModifiedJulianDay);
        Assert.AreEqual(smartBufferB.GetTimeAt(i).DurationInDays, buffer2.GetTimeAt(i).DurationInDays);

        for (int n = 0; n < smartBufferB.ValuesCount; n++)
        {
          Assert.AreEqual(smartBufferB.GetValuesAt(i)[n], buffer2.GetValuesAt(i)[n]);
        }
      }
    }
  }
}