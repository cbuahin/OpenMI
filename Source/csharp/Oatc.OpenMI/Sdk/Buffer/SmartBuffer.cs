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
using System.Diagnostics;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Buffer
{
  /// <summary>
  /// The SmartBuffer class provides buffering functionality that will store values needed for a
  /// particular output item in memory and functionality that will interpolate, extrapolate and aggregate 
  /// values from these values.
  /// </summary>
  /// <remarks>
  /// The content of the SmartBuffer is lists of corresponding times and ValueSets,
  /// where times can be TimeStamps or TimeSpans and the ValueSets are double[].
  /// <para>
  /// It works on a <see cref="ValueSetArray{T}"/> of type double.
  /// </para>
  /// <para>
  /// SmartBuffer objects may not contain mixtures of TimeSpans and TimeStamps.
  /// The number of Times (TimeSpans or TimeStamps) must equal the number of ValueSets 
  /// ( double[]s) in the SmartBuffer.
  /// </para>
  /// <para>
  /// When time spans are put in the buffer, it is assumed that there are not "holes"
  /// between the spans, i.e. a time setup of:
  /// <code>
  ///     |----|----|   |----|----|
  ///     a    b    c   d    e    f
  /// </code>
  /// where data is missing between c and d is not allowed (a value of 0 is assumed in 
  /// such an interval)
  /// </para>
  /// </remarks>
  [Serializable]
  public class SmartBuffer
  {
    private const int _defaultBufferSizeMessageFrequency = 1000;

    //readonly List<ITime> _times = new List<ITime>();
    readonly TimeSet _times = new TimeSet();
    //readonly List<double[]> values = new List<double[]>();
    readonly ValueSetArray<double> _values = new ValueSetArray<double>();
    double _relaxationFactor;  //Used for the extrapolation algorithm see also RelaxationFactor property
    bool _doExtendedDataVerification;

    private int _lastBufferSizeMessageCounter = 0;
    private int _bufferSizeMessageFrequency = _defaultBufferSizeMessageFrequency;

    /// <summary>
    /// Create an empty smart-buffer.
    /// </summary>
    public SmartBuffer()
    {
      DoExtrapolate = true;
      Create();
    }

    /// <summary>
    /// Create a new SmartBuffer with values and times copied from another SmartBuffer
    /// </summary>
    /// <param name="smartBuffer">The SmartBuffer to copy</param>
    public SmartBuffer(SmartBuffer smartBuffer)
    {
      DoExtrapolate = true;
      Create();

      if (smartBuffer.TimesCount > 0)
      {
        for (int i = 0; i < smartBuffer.TimesCount; i++)
        {
          AddValues(new Time(smartBuffer.GetTimeAt(i)), GetCopy(smartBuffer.GetValuesAt(i)));
        }
      }
    }

    public TimeSet TimeSet { get { return (_times); } }
    public ValueSetArray<double> ValueSet { get { return (_values); } }

    public int BufferSizeMessageFrequency
    {
      set
      {
        _bufferSizeMessageFrequency = value;
      }
    }

    private static double[] GetCopy(ICollection array)
    {
      double[] copyArray = new double[array.Count];
      array.CopyTo(copyArray, 0);
      return copyArray;
    }

    private void Create()
    {
      _doExtendedDataVerification = true;
      _relaxationFactor = 1.0;
    }

    /// <summary>
    /// Flag indicating whether the buffer should extrapolate, if
    /// the requested time is outside the spans available in the buffer.
    /// Default is true.
    /// <para>
    /// If false and a requested time is outside the available spans, 
    /// an exception is thrown.
    /// </para>
    /// </summary>
    public bool DoExtrapolate { get; set; }

    /// <summary>
    ///	Add corresponding values for time and values to the SmartBuffer.
    /// </summary>
    /// <param name="time"> Description of the time parameter</param>
    /// <param name="valueSet">Description of the values parameter</param>
    /// <remarks>
    /// The AddValues method will internally make a copy of the added times and values. The reason for
    /// doing this is that the times and values arguments are references, and the correspondign values 
    /// could be changed by the owner of the classes
    /// </remarks>
    public void AddValues(ITime time, double[] valueSet)
    {
      double[] x = GetCopy(valueSet);
      AddValuesToBuffer(time, x);
    }

    public void AddValues(ITime time, IList values)
    {
      if (values == null || values.Count == 0)
        return;
      if (!(values[0] is double))
        throw new Exception("Buffer only handles doubles");

      double[] x = GetCopy(values);
      AddValuesToBuffer(time, x);
    }

    /// <summary>
    /// Add values to the buffer, check that time is increasing, not
    /// overlapping (spans), and has durations matching the TimeSet.
    /// </summary>
    private void AddValuesToBuffer(ITime time, double[] values)
    {
      // Make the first value in the buffer decide whether it has durations or not.
      if (_times.Count == 0)
        _times.HasDurations = (time.DurationInDays > 0);

      // Check if we can add the values
      if (_times.HasDurations)
      {
        if (time.DurationInDays <= 0)
          throw new Exception("Time without duration added to time set with durations.");
        
        // Order of times: The end time is allowed to overlap with at most 
        // Time.EpsilonForTimeCompare with the new time.
        if (_times.Times.Count > 0 &&
            time.StampAsModifiedJulianDay + Time.EpsilonForTimeCompare <
            _times[_times.Times.Count - 1].EndStampAsModifiedJulianDay())
          throw new Exception("Overlapping times/out of order times in time bufferer not allowed.");
      }
      else
      {
        if (time.DurationInDays > 0)
          throw new Exception("Time with duration added to time set without durations.");
        
        // Order of times: There must at least be a difference of Time.EpsilonForTimeCompare between two stamps
        if (_times.Count > 0 &&
            time.StampAsModifiedJulianDay - Time.EpsilonForTimeCompare <
            _times[_times.Count - 1].StampAsModifiedJulianDay)
          throw new Exception("Overlapping times/out of order times in time bufferer not allowed.");
      }

      _times.Add(new Time(time));  // save a copy of time
      _values.Values2DArray.Add(values);

      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
      CheckBufferGrowth();
    }

    private void CheckBufferGrowth()
    {
      if (_times.Count > _lastBufferSizeMessageCounter)
      {
        if (_times.Count % _bufferSizeMessageFrequency == 0)
        {
          Trace.WriteLine("Buffer size has increased to ");
          _lastBufferSizeMessageCounter = _times.Count;
        }
      }
    }

    /// <summary>
    /// RelaxationFactor. The relaxation factor must be in the interval [0; 1]. The relaxation
    /// parameter is used when doing extrapolation. A value of 1 results in nearest extrapolation
    /// whereas a value 0 results in linear extrapolation.
    /// </summary>
    public double RelaxationFactor
    {
      get
      {
        return _relaxationFactor;
      }
      set
      {
        _relaxationFactor = value;
        if (_relaxationFactor < 0 || _relaxationFactor > 1)
        {
          throw new Exception("ReleaxationFactor is out of range");
        }
      }
    }

    /// <summary>
    /// Returns the timeStep´th ITime.
    /// </summary>
    /// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th ITime</returns>
    public ITime GetTimeAt(int timeStep)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
      return _times[timeStep];
    }

    //===============================================================================================
    // GetValuesAt(int timeStep) : double[]
    //===============================================================================================
    /// <summary>
    /// Returns the timeStep´th double[]
    /// </summary>
    /// <param name="timeStep">time step index</param>
    /// <returns>The timeStep´th double[]</returns>
    public double[] GetValuesAt(int timeStep)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }
      return _values[timeStep];
    }

    /// <summary>
    /// Returns the ValueSet that corresponds to requestTime. The ValueSet may be found by 
    /// interpolation, extrapolation and/or aggregation.
    /// </summary>
    /// <param name="requestedTime">time for which the value is requested</param>
    /// <returns>valueSet that corresponds to requestTime</returns>
    public double[] GetValues(ITime requestedTime)
    {
      if (_doExtendedDataVerification)
      {
        CheckBuffer();
      }

      if (!DoExtrapolate)
      {
        if (requestedTime.End().StampAsModifiedJulianDay > _times[_times.Count - 1].End().StampAsModifiedJulianDay + Time.EpsilonForTimeCompare ||
            requestedTime.Start().StampAsModifiedJulianDay < _times[0].Start().StampAsModifiedJulianDay - Time.EpsilonForTimeCompare)
        {
          throw new Exception("Extrapolation not allowed for this buffer");
        }
      }

      double[] returnValues = null;
      if (_values.Values2DArray.Count != 0)
      {
        if (_times[0].DurationInDays > 0 && requestedTime.DurationInDays > 0)
        {
          returnValues = MapFromTimeSpansToTimeSpan(requestedTime);
        }
        else if (_times[0].DurationInDays > 0 && requestedTime.DurationInDays == 0)
        {
          returnValues = MapFromTimeSpansToTimeStamp(requestedTime);
        }
        else if (_times[0].DurationInDays == 0 && requestedTime.DurationInDays > 0)
        {
          returnValues = MapFromTimeStampsToTimeSpan(requestedTime);
        }
        else // time stamps
        {
          returnValues = MapFromTimeStampsToTimeStamp(requestedTime);
        }
      }
      //            Console.WriteLine(((requestedTime.DurationInDays > 0) ? "Span" : "Stamp") + " from " +
      //                ((times[0].DurationInDays > 0) ? "Span" : "Stamp") + " in Buffer: " + requestedTime + ": "
      //                + returnValueSet[0]);
      return returnValues;
    }

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeSpans.
    /// </summary>
    /// <param name="requestedTime">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private double[] MapFromTimeSpansToTimeSpan(ITime requestedTime)
    {
      try
      {
        int m = _values[0].Length;
        double[] xr = new double[m];                                                        // Values to return
        double trb = requestedTime.StampAsModifiedJulianDay;                                // Begin time in requester time interval
        double tre = requestedTime.StampAsModifiedJulianDay + requestedTime.DurationInDays; // End time in requester time interval
        double trl = tre - trb; // length of requested time interval

        if (_times.Count == 0)
          throw new Exception("No times in buffer");

        double tbb0 = _times[0].StampAsModifiedJulianDay;

        // In the following the current abbreviations are used:
        // B: Buffer time
        // R: Requested time
        // I: Time included by this part of the code

        if (DoExtrapolate)
        {
          //--------------------------------------------------------------------------
          // B                tbb0|-------tbe0|tbb1------|tbe1
          // R+I  trb|---|tre               
          //--------------------------------------------------------------------------
          if (tre <= tbb0)
          {
            if (_times.Count >= 2 && _relaxationFactor != 1)
            {
              // Linear interpolation
              double tbe0 = _times[0].EndStampAsModifiedJulianDay();
              double tbe1 = _times[1].EndStampAsModifiedJulianDay();
              //double tbb1 = ((ITimeSpan) times[1]).Start.StampAsModifiedJulianDay;

              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i);
                double sbi1 = BufferHelper.GetVal(_values[1], i);
                xr[i] = sbi0 - (1 - _relaxationFactor) * (sbi1 - sbi0) * (tbe0 + tbb0 - tre - trb) / (tbe1 - tbb0);
              }
            }
            else
            {
              // Nearest value interpolation
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i);
                xr[i] = sbi0;
              }
            }
            // We are done now, just return xr
            return xr;
          }

          //--------------------------------------------------------------------------
          // B          tbb0|--------|---------|--------|
          // R     trb|----------------|tre              
          // I        |-----|
          //---------------------------------------------------------------------------
          if (trb < tbb0) // && tre > tbb0
          {
            double tbe0 = _times[0].EndStampAsModifiedJulianDay();
            if (_times.Count >= 2 && _relaxationFactor != 1)
            {
              double tbe1 = _times[1].EndStampAsModifiedJulianDay();
              // Linear interpolation, use tbb0 as "endpoint" of interval
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i);
                double sbi1 = BufferHelper.GetVal(_values[1], i);
                xr[i] += ((tbb0 - trb) / trl) * (sbi0 - (1 - _relaxationFactor) * (sbi1 - sbi0) * (tbe0 - trb) / (tbe1 - tbb0));
              }
            }
            else
            {
              // Nearest value interpolation
              for (int i = 0; i < m; i++)
              {
                double sbi0 = BufferHelper.GetVal(_values[0], i);
                xr[i] += sbi0 * (tbb0 - trb) / trl;
              }
            }
          }


          double tbeN0 = _times[_times.Count - 1].End().StampAsModifiedJulianDay;


          //--------------------------------------------------------------------------
          // B     tbb0|---?----|-------|tbeN0
          // R+I                                trb|---|tre
          //--------------------------------------------------------------------------
          if (tbeN0 < trb)
          {
            if (_times.Count >= 2 && _relaxationFactor != 1)
            {
              // Linear interpolation
              double tbeN1 = _times[_times.Count - 2].EndStampAsModifiedJulianDay();
              double tbbN1 = _times[_times.Count - 2].StampAsModifiedJulianDay;
              //double tbbN_1 = ((ITimeSpan) times[times.Count-1]).Start.StampAsModifiedJulianDay;

              for (int i = 0; i < m; i++)
              {
                double sbiN0 = BufferHelper.GetVal(_values[_times.Count - 1], i);
                double sbiN1 = BufferHelper.GetVal(_values[_times.Count - 2], i);
                xr[i] = sbiN0 + (1 - _relaxationFactor) * (sbiN0 - sbiN1) * (trb + tre - tbeN0 - tbeN1) / (tbeN0 - tbbN1);
              }
            }
            else
            {
              // Nearest value interpolation
              for (int i = 0; i < m; i++)
              {
                double sbiN0 = BufferHelper.GetVal(_values[_times.Count - 1], i);
                xr[i] = sbiN0;
              }
            }
            // We are done now, just return xr
            return xr;
          }

          //--------------------------------------------------------------------------
          // B     |---?----|-------|tbeN0
          // R               trb|-------|tre
          // I                      |---|
          //--------------------------------------------------------------------------
          if (tbeN0 < tre)
          {

            if (_times.Count >= 2 && _relaxationFactor != 1)
            {
              //double tbeN_2 = ((ITimeSpan) times[times.Count-2]).End.StampAsModifiedJulianDay;
              double tbeN1 = _times[_times.Count - 2].EndStampAsModifiedJulianDay();
              double tbbN1 = _times[_times.Count - 2].StampAsModifiedJulianDay;
              for (int i = 0; i < m; i++)
              {
                double sbiN0 = BufferHelper.GetVal(_values[_times.Count - 1], i);
                double sbiN1 = BufferHelper.GetVal(_values[_times.Count - 2], i);
                xr[i] += ((tre - tbeN0) / (tre - trb)) *
                         (sbiN0 + (1 - _relaxationFactor) * (sbiN0 - sbiN1) * (tre - tbeN1) / (tbeN0 - tbbN1));
              }
            }
            else
            {
              for (int i = 0; i < m; i++)
              {
                double sbiN0 = BufferHelper.GetVal(_values[_times.Count - 1], i);
                xr[i] += sbiN0 * ((tre - tbeN0) / (tre - trb));
              }
            }
          }
        }

        int nstart = 0;
        int nend = _times.Count - 1;
        // Narrow down the number of spans to investigate, based on the requested span.
        if (nend > 10)
        {
          // Assuming 4 spans in the buffer, use end-time in GetIntrval
          // will provide the right indices
          // spans                 |-0-|-1-|-2-|-3-|
          // endStamp intervals      0 | 1 | 2 | 3 | 4

          double fraction;
          nstart = _times.Times.GetInterval(trb, out fraction, time => time.EndStampAsModifiedJulianDay());
          nend = _times.Times.GetInterval(tre, out fraction, time => time.End().StampAsModifiedJulianDay);
          nend = Math.Min(nend, _times.Count - 1);
        }

        for (int n = nstart; n <= nend; n++)
        {
          double tbbn = _times[n].StampAsModifiedJulianDay;
          double tben = _times[n].EndStampAsModifiedJulianDay();

          //---------------------------------------------------------------------------
          // B:       tbbn|--------------------------|tben
          // R:     trb|-------------------------------------|tre
          // I:           |--------------------------|
          //---------------------------------------------------------------------------
          if (trb <= tbbn && tre >= tben)
          {
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n], i);
              xr[i] += sbin*(tben - tbbn)/(tre - trb);
            }
          }

            //---------------------------------------------------------------------------
          // B:      tbbn|-----------------------|tben
          // R+I:          trb|--------------|tre
          // --------------------------------------------------------------------------
          else if (tbbn <= trb && tre <= tben) //cover all
          {
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              xr[i] += BufferHelper.GetVal(_values[n], i);
            }
          }

            //---------------------------------------------------------------------------
          // B:     tbbn|-----------------|tben
          // R:                 trb|--------------|tre
          // I:                    |------|
          // --------------------------------------------------------------------------
          else if (tbbn < trb && trb < tben && tre > tben)
          {
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n], i);
              xr[i] += sbin*(tben - trb)/(tre - trb);
            }
          }

            //---------------------------------------------------------------------------
          // B:           tbbn|-----------------|tben
          // R:      trb|--------------|tre
          // I:               |--------|
          // --------------------------------------------------------------------------
          else if (trb < tbbn && tre > tbbn && tre < tben)
          {
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n], i);
              xr[i] += sbin*(tre - tbbn)/(tre - trb);
            }
          }
        }

        return xr;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeSpan Failed", e);
      }
    }

    /// <summary>
    /// A ValueSet corresponding to a TimeSpan is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeStamps.
    /// </summary>
    /// <param name="requestedTime">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private double[] MapFromTimeStampsToTimeSpan(ITime requestedTime)
    {
      try
      {
        int m = _values[0].Length;
        //int        N  = times.Count;								   	      // Number of time steps in buffer
        double[] xr = new double[m];                                      // Values to return
        double trb = requestedTime.StampAsModifiedJulianDay;   // Begin time in requester time interval
        double tre = requestedTime.StampAsModifiedJulianDay + requestedTime.DurationInDays;    // End time in requester time interval

        //---------------------------------------------------------------------------
        // This handles values within the time horizon of the buffer, i.e.
        // there must be at least two values in the buffer.

        int nstart = 1;
        int nend = _times.Count - 1;
        // Narrow down the number of intervals to investigate, based on the requested span.
        if (nend > 4)
        {
          double fraction;
          nstart = _times.Times.GetInterval(trb, out fraction, time => time.StampAsModifiedJulianDay);
          nend = _times.Times.GetInterval(tre, out fraction, time => time.StampAsModifiedJulianDay);
          nstart = Math.Max(nstart, 1);
          nend = Math.Min(nend, _times.Count - 1);
        }
        
        // n corresponds to the n'th interval between the time stamps.
        for (int n = nstart; n <= nend; n++)
        {
          double tbn = _times[n-1].StampAsModifiedJulianDay;
          double tbnp1 = _times[n].StampAsModifiedJulianDay;

          //---------------------------------------------------------------------------
          //    B:        tbn|--------------------------|tbnp1
          //    R:     trb|-------------------------------------|tre
          //    I:           |--------------------------|
          // --------------------------------------------------------------------------
          if (trb <= tbn && tre >= tbnp1)
          {
            double factor = (tbnp1 - tbn)/(tre - trb);
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n-1], i);
              double sbinp1 = BufferHelper.GetVal(_values[n], i);
              xr[i] += 0.5*(sbin + sbinp1)*factor;
            }
          }

          //---------------------------------------------------------------------------
          // B:       tbn|-----------------------|tbnp1
          // R+I:         trb|--------------|tre
          // --------------------------------------------------------------------------
          else if (tbn <= trb && tre <= tbnp1) //cover all
          {
            double fraction = ((tre + trb)/2 - tbn)/(tbnp1 - tbn);
            for (int i = 0; i < m; i++) // for all elements
            {
              double sbin = BufferHelper.GetVal(_values[n-1], i);
              double sbinp1 = BufferHelper.GetVal(_values[n], i);
              xr[i] += sbin + (sbinp1 - sbin)*fraction;
            }
          }

          //---------------------------------------------------------------------------
          // B:      tbn|-----------------|tbnp1
          // R:                 trb|--------------|tre
          // I:                    |------|
          // --------------------------------------------------------------------------
          else if (tbn < trb && trb < tbnp1 && tre > tbnp1)
          {
            double fraction = ((tbnp1 - trb)/2)/(tbnp1 - tbn);
            double factor = (tbnp1 - trb)/(tre - trb);
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n-1], i);
              double sbinp1 = BufferHelper.GetVal(_values[n], i);
              xr[i] += (sbinp1 - (sbinp1 - sbin)*fraction)*factor;
            }
          }

          //---------------------------------------------------------------------------
          // B:            tbn|-----------------|tbnp1
          // R:      trb|--------------|tre
          // I:               |--------|
          // --------------------------------------------------------------------------
          else if (trb < tbn && tre > tbn && tre < tbnp1)
          {
            double fraction = ((tre - tbn)/2)/(tbnp1 - tbn);
            double factor = (tre - tbn)/(tre - trb);
            for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            {
              double sbin = BufferHelper.GetVal(_values[n-1], i);
              double sbinp1 = BufferHelper.GetVal(_values[n], i);
              xr[i] += (sbin + (sbinp1 - sbin)*fraction)*factor;
            }
          }
        }


        //---------------------------------------------------------------------------
        // This handles values outside the time horizon of the buffer.

        //--------------------------------------------------------------------------
        // In case of only one value in the buffer, regardless of its position relative to R
        //    |     or     |     or   |               B
        //        |----------------|                  R
        //---------------------------------------------------------------------------
        if (_times.Count == 1)
        {
          // TODO: Test if extrapolation is ok.
          for (int i = 0; i < m; i++)
          {
            double sbi0 = BufferHelper.GetVal(_values[0], i);
            xr[i] = sbi0;
          }
        }
        else
        {
          // At least two stamp values in the buffer, so we can do extrapolation
          //--------------------------------------------------------------------------
          //  B:       tb0|-----tb1|---------|--------|
          //  R: trb|----------------|tre
          //  I:    |-----|
          //---------------------------------------------------------------------------
          double tb0 = _times[0].StampAsModifiedJulianDay;
          double tb1 = _times[1].StampAsModifiedJulianDay;
          double tbN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay;
          double tbN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay;

          if (trb < tb0 && tre > tb0)
          {
            double fraction = (1 - _relaxationFactor)*0.5*(tb0 - trb)/(tb1 - tb0);
            double factor = ((tb0 - trb)/(tre - trb));
            for (int i = 0; i < m; i++)
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i);
              double sbi1 = BufferHelper.GetVal(_values[1], i);
              xr[i] += factor*(sbi0 - fraction*(sbi1 - sbi0));
            }
          }
          //-------------------------------------------------------------------------------------
          // B     |--------|---------|tbn_2---|tbn_1
          // R                        trb|----------------|tre
          // I                                 |----------| 
          //-------------------------------------------------------------------------------------
          if (tre > tbN_1 && trb < tbN_1)
          {
            double factor = ((tre - tbN_1)/(tre - trb));
            double fraction = (1 - _relaxationFactor)*0.5*(tre - tbN_1)/(tbN_1 - tbN_2);
            for (int i = 0; i < m; i++)
            {
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i);
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i);
              xr[i] += factor*(sbiN_1 + fraction*(sbiN_1 - sbiN_2));
            }
          }
          //-------------------------------------------------------------------------------------
          // B   :   |--------|---------|tbn_2---|tbn_1
          // R+I :                                   trb|----------------|tre
          //-------------------------------------------------------------------------------------
          if (trb >= tbN_1)
          {
            double fraction = (1 - _relaxationFactor)*(0.5*(trb + tre) - tbN_1)/(tbN_1 - tbN_2);
            for (int i = 0; i < m; i++)
            {
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i);
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i);

              xr[i] = sbiN_1 + (sbiN_1 - sbiN_2)*fraction;
            }
          }
          //-------------------------------------------------------------------------------------
          // B   :                  tb0|-----tb1|---------|--------| B
          // R+I :   trb|-------|tre
          //-------------------------------------------------------------------------------------
          if (tre <= tb0)
          {
            double fraction = (1 - _relaxationFactor)/(tb1 - tb0)*(tb0 - 0.5*(trb + tre));
            for (int i = 0; i < m; i++)
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i);
              double sbi1 = BufferHelper.GetVal(_values[1], i);
              xr[i] = sbi0 - (sbi1 - sbi0) * fraction;
            }
          }
        }
        //-------------------------------------------------------------------------------------

        return xr;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeSpan Failed", e);
      }
    }


    /// <summary>
    /// A ValueSet corresponding to a TimeStamp is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeStamps.
    /// </summary>
    /// <param name="requestedTimeStamp">TimeStamp for which the values are requested</param>
    /// <returns>ValueSet that corresponds to the requested time stamp</returns>
    private double[] MapFromTimeStampsToTimeStamp(ITime requestedTimeStamp)
    {
      try
      {
        int m = (_values[0]).Length;
        double[] xr = new double[m];                             // Values to return
        double tr = requestedTimeStamp.StampAsModifiedJulianDay;		     // Requested TimeStamp


        if (_times.Count == 1)
        {
          //---------------------------------------------------------------------------
          //    Buffered TimesStamps: |          >tb0<  
          //    Requested TimeStamp:  |    >tr<
          // or Requested TimeStamp:  |          >tr<
          // or Requested TimeStamp:  |                >tr<
          //                           -----------------------------------------> t
          // --------------------------------------------------------------------------
          if (tr > (_times[0].StampAsModifiedJulianDay + Time.EpsilonForTimeCompare) && !DoExtrapolate)
          {
            throw new Exception("Extrapolation not allowed");
          }
          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              xr[i] = BufferHelper.GetVal(_values[0], i);
            }
          }
        }
        else if (tr <= _times[0].StampAsModifiedJulianDay)
        {
          //---------------------------------------------------------------------------
          //  Buffered TimesStamps: |          >tb0<   >tb1<   >tb2<  >tbN<
          //  Requested TimeStamp:  |    >tr<
          //                         -----------------------------------------> t
          // --------------------------------------------------------------------------
          double tb0 = _times[0].StampAsModifiedJulianDay;
          double tb1 = _times[1].StampAsModifiedJulianDay;

          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i);
              double sbi1 = BufferHelper.GetVal(_values[1], i);
              xr[i] = ((sbi0 - sbi1) / (tb0 - tb1)) * (tr - tb0) * (1 - _relaxationFactor) + sbi0;
            }
          }
        }
        else if (tr > _times[_times.Count - 1].StampAsModifiedJulianDay)
        {
          //---------------------------------------------------------------------------
          //  Buffered TimesStamps: |    >tb0<   >tb1<   >tb2<  >tbN_2<  >tbN_1<
          //  Requested TimeStamp:  |                                             >tr<
          //                         ---------------------------------------------------> t
          // --------------------------------------------------------------------------
          double tbN_2 = _times[_times.Count - 2].StampAsModifiedJulianDay;
          double tbN_1 = _times[_times.Count - 1].StampAsModifiedJulianDay;

          {
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [N-1]
            {
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i);
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i);

              xr[i] = ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (tr - tbN_1) * (1 - _relaxationFactor) + sbiN_1;
            }
          }
        }
        else
        {
          //---------------------------------------------------------------------------
          //  Available TimesStamps: |    >tb0<   >tb1<  >tbna<       >tnb<   >tbN_1<  >tbN_2<
          //  Requested TimeStamp:   |                          >tr<
          //                         -------------------------------------------------> t
          // --------------------------------------------------------------------------
          double fraction;
          int interval = _times.Times.GetInterval(tr, out fraction, time => time.StampAsModifiedJulianDay);

          double[] valueSetA = _values[interval - 1];
          double[] valueSetB = _values[interval];

          for (int i = 0; i < m; i++) // For each element value
          {
            double sbinA = BufferHelper.GetVal(valueSetA, i);
            double sbinB = BufferHelper.GetVal(valueSetB, i);
            xr[i] = sbinA + fraction * (sbinB - sbinA);
          }
        }
        //----------------------------------------------------------------------------------------------
        return xr;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeStampsToTimeStamp Failed", e);
      }
    }

    /// <summary>
    /// A Value for a time stamp is calculated using interpolation or
    /// extrapolation in corresponding lists of ValueSets and TimeSpans.
    /// </summary>
    /// <param name="requestedTimeStamp">Time for which the ValueSet is requested</param>
    /// <returns>ValueSet that corresponds to requestedTime</returns>
    private double[] MapFromTimeSpansToTimeStamp(ITime requestedTimeStamp)
    {
      try
      {
        int m = _values[0].Length;
        double[] xr = new double[m];                             // Values to return
        double tr = requestedTimeStamp.StampAsModifiedJulianDay; 	     // Requested TimeStamp

        if (_times.Count == 1)
        {
          //---------------------------------------------------------------------------
          //    Buffered TimesSpans:  |         |------|  
          //    Requested TimeStamp:  |  >tr<
          // or Requested TimeStamp:  |           >tr<
          // or Requested TimeStamp:  |                   >tr<
          //                           -----------------------------------------> t
          // --------------------------------------------------------------------------

          // Check if tr is inside span
          if (!DoExtrapolate)
          {
            ITime time = _times.Times[0];
            if (time.StampAsModifiedJulianDay - Time.EpsilonForTimeCompare > tr ||
                tr > time.EndStampAsModifiedJulianDay() + Time.EpsilonForTimeCompare)
              throw new Exception("Extrapolation not allowed");
          }

          for (int i = 0; i < m; i++) // For each element
          {
            xr[i] = BufferHelper.GetVal(_values[0], i);
          }
        }
        //---------------------------------------------------------------------------
        //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
        //  Requested TimeStamp:  |    >tr<
        //                         -----------------------------------------> t
        // --------------------------------------------------------------------------
        else if (tr <= _times[0].StampAsModifiedJulianDay)
        {
          // Check if we are allowed to extrapolate
          if (!DoExtrapolate)
          {
            if (tr < _times[0].StampAsModifiedJulianDay - Time.EpsilonForTimeCompare)
              throw new Exception("Extrapolation not allowed");
            // Very close to the first point, just provide that value
            for (int i = 0; i < m; i++) // For each element
            {
              xr[i] = BufferHelper.GetVal(_values[0], i);
            }
          }
          else
          {
            // Extrapolate from the first two values
            double tbb0 = _times[0].StampAsModifiedJulianDay;
            double tbb1 = _times[1].StampAsModifiedJulianDay;
            double fraction = (tr - tbb0)/(tbb0 - tbb1)*(1 - _relaxationFactor);
            for (int i = 0; i < m; i++) //For each Vector in buffered VectorSet [0]
            {
              double sbi0 = BufferHelper.GetVal(_values[0], i);
              double sbi1 = BufferHelper.GetVal(_values[1], i);
              xr[i] = (sbi0 - sbi1)*fraction + sbi0;
            }
          }
        }

        //---------------------------------------------------------------------------
        //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
        //  Requested TimeStamp:  |                                             >tr<
        //                         ---------------------------------------------------> t
        // --------------------------------------------------------------------------
        else if (tr >= _times[_times.Count - 1].EndStampAsModifiedJulianDay())
        {
          // Check if we are allowed to extrapolate
          if (!DoExtrapolate)
          {
            if (tr > _times[0].StampAsModifiedJulianDay + Time.EpsilonForTimeCompare)
              throw new Exception("Extrapolation not allowed");
            // Very close to the last point, just provide that value
            for (int i = 0; i < m; i++) // For each element
            {
              xr[i] = BufferHelper.GetVal(_values[_times.Count - 1], i);
            }
          }
          else
          {
            // Extrapolate from the last two values
            double tbeN_2 = _times[_times.Count - 2].EndStampAsModifiedJulianDay();
            double tbeN_1 = _times[_times.Count - 1].EndStampAsModifiedJulianDay();
            double fraction = (tr - tbeN_1)/(tbeN_1 - tbeN_2)*(1 - _relaxationFactor);
            for (int i = 0; i < m; i++) // For each element
            {
              double sbiN_2 = BufferHelper.GetVal(_values[_times.Count - 2], i);
              double sbiN_1 = BufferHelper.GetVal(_values[_times.Count - 1], i);

              xr[i] = (sbiN_1 - sbiN_2)*fraction + sbiN_1;
            }
          }
        }

        //---------------------------------------------------------------------------
        //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
        //  Requested TimeStamp:   |                          >tr<
        //                         -------------------------------------------------> t
        // --------------------------------------------------------------------------
        else
        {

          // Using end-time in GetIntrval will provide the right indices
          // Example: assuming 4 spans in the buffer, 
          // spans                 |-0-|-1-|-2-|-3-|
          // endStamp intervals      0 | 1 | 2 | 3 | 4
          double fraction;
          int interval = _times.Times.GetInterval(tr, out fraction, time => time.EndStampAsModifiedJulianDay());

          for (int i = 0; i < m; i++) // For each element
          {
            xr[i] = BufferHelper.GetVal(_values[interval], i);
          }
        }

        //----------------------------------------------------------------------------------------------

        return xr;
      }
      catch (Exception e)
      {
        throw new Exception("MapFromTimeSpansToTimeStamp Failed", e);
      }
    }

    /// <summary>
    /// Number of time streps in the buffer.
    /// </summary>
    public int TimesCount
    {
      get
      {
        return _times.Count;
      }
    }

    /// <summary>
    /// Read only property for the number of values in each of the valuesets contained in the buffer.
    /// </summary>
    public int ValuesCount
    {
      get
      {
        return (_values[0]).Length;
      }
    }

    /// <summary>
    /// Checks weather the contents of the buffer is valid.
    /// </summary>
    public void CheckBuffer()
    {
      if (_times.Count != _values.Values2DArray.Count)
      {
        throw new Exception("Different numbers of values and times in buffer");
      }

      if (_times.Count == 0)
      {
        throw new Exception("Buffer is empty");
      }
      _lastBufferSizeMessageCounter = 0;
    }

    /// <summary>
    /// Read/Write property flag that indicates wheather or not to perform extended data
    /// checking.
    /// </summary>
    public bool DoExtendedDataVerification
    {
      get
      {
        return _doExtendedDataVerification;
      }
      set
      {
        _doExtendedDataVerification = value;
      }
    }


    /// <summary>
    /// Clear all times and values in the buffer at or later than the specified time
    /// If the specified time is type ITimeSpan the Start time is used.
    /// </summary>
    /// <param name="time"></param>
    /// <remarks>
    /// The time to clear after is using an epsilon of <see cref="Time.EpsilonForTimeCompare"/>
    /// in order to avoid rounding errors. Times within this epsilon value are also removed.
    /// </remarks>
    public void ClearAfter(ITime time)
    {
      ITime clearTime = new Time(time.StampAsModifiedJulianDay - Time.EpsilonForTimeCompare);
      for (int i = 0; i < _times.Count; i++)
      {
        if (clearTime.StampAsModifiedJulianDay <= _times[i].StampAsModifiedJulianDay)
        {
          // clear after current time
          int numberOfValuesToRemove = _times.Count - i;
          _times.RemoveRange(i, numberOfValuesToRemove);
          _values.Values2DArray.RemoveRange(i, numberOfValuesToRemove);
          return;
        }
      }
      _lastBufferSizeMessageCounter = _times.Count;
    }

    /// <summary>
    /// Clear all records in the buffer assocaited to time that is earlier that the
    /// time specified in the argument list. However, one record associated to time 
    /// before the time in the argument list is left in the buffer.
    /// The criteria when comparing TimeSpans is that they may not overlap in order
    /// to be regarded as before each other.
    /// (see also Oatc.OpenMI.Sdk.Buffer.Support.IsBefore(ITime ta, ITime tb)
    /// </summary>
    /// <param name="time">time before which the records are removed</param>
    /// <remarks>
    /// The time to clear beforeis using an epsilon of <see cref="Time.EpsilonForTimeCompare"/>
    /// in order to avoid rounding errors. Times within the epsilon value are kept in the buffer.
    /// </remarks>
    public void ClearBefore(ITime time)
    {
      ITime clearTime = new Time(time.StampAsModifiedJulianDay - Time.EpsilonForTimeCompare);
      for (int i = _times.Count - 1; i >= 0; i--)
      {
        if (BufferHelper.IsBefore(_times[i], clearTime))
        {
          // clear before current time
          _times.RemoveRange(0, i);
          _values.Values2DArray.RemoveRange(0, i);
          return;
        }
      }
      _lastBufferSizeMessageCounter = _times.Count;
    }

    public IList<IList<double>> GetAllValues()
    {
      IList<IList<double>> valuesList = new List<IList<double>>();
      int nTimes = _times.Count;
      for (int t = 0; t < nTimes; t++)
      {
        valuesList[t] = new List<double>();
        double[] timeStepValues = _values[t];
        for (int j = 0; j < timeStepValues.Length; j++)
        {
          valuesList[t].Add(timeStepValues[j]);
        }
      }
      return valuesList;
    }

    public void SetOrAddValues(ITime time, IList values)
    {
      int index = _times.Times.BinarySearch(time);
      if (index < 0)
      {
        AddValues(time, values);
      }
      else
      {
        double[] copy = GetCopy(values);
        this._values[index] = copy;
      }
    }
  }
}
