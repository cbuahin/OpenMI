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
using System.Globalization;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
    /// <summary>
    /// The Time class defines a time instant.
    /// <para>This is a trivial implementation of OpenMI.Standard.Time, refer there for further details.</para>
    /// </summary>
    [Serializable]
    public class Time : ITime, IComparable
    {
        /// <summary>
        /// DateTime of the origin of the Modified Julian
        /// Date system, i.e. 1858-11-17 00:00:00
        /// </summary>
        public static readonly DateTime ModifiedJulianDateZero = new DateTime(1858, 11, 17);

        /// <summary>
        /// Number of ticks at the origin of the Modified Julian
        /// Date system, i.e. 1858-11-17 00:00:00
        /// </summary>
        /// <remarks>
        /// Same as <code>new DateTime(1858, 11, 17).Ticks</code>
        /// </remarks>
        public const long ModifiedJulianDateZeroTicks = 586288800000000000L;


        private double durationInDays;
        private double stampAsModifiedJulianDay;

        static double epsilonForTimeCompare = 1e-8;

        /// <summary>
        /// When comparing time values as modified julian day values,
        /// this is the allowed difference before two times are considered equal.
        /// <para>
        /// The default value of 1e-8 corresponds to approximately 1 millisecond,
        /// 1/(24*60*60*1000) = 1.157e-8
        /// </para>
        /// </summary>
        public static double EpsilonForTimeCompare
        {
            get { return epsilonForTimeCompare; }
            set { epsilonForTimeCompare = value; }
        }

        /// <summary>
        /// Will compare two times as MJD, using epsilonForTimeCompare.
        /// </summary>
        /// <param name="time1AsMJD">First time as MJD</param>
        /// <param name="time2AsMJD">Second time as MJD</param>
        /// <returns>True if time1AsMJD and time2AsMJD are equal.</returns>
        private static bool MjDsAreEqual(double time1AsMJD, double time2AsMJD)
        {
            return time1AsMJD >= time2AsMJD - epsilonForTimeCompare &&
                   time1AsMJD <= time2AsMJD + epsilonForTimeCompare;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Time()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The time stamp to copy</param>
        public Time(ITime source)
        {
            StampAsModifiedJulianDay = source.StampAsModifiedJulianDay;
            DurationInDays = source.DurationInDays;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stampAsModifiedJulianDay">The modified julian day for the time stamp</param>
        public Time(double stampAsModifiedJulianDay)
        {
            this.stampAsModifiedJulianDay = stampAsModifiedJulianDay;
        }

        public Time(double stampAsModifiedJulianDay, double durationInDays)
        {
            this.stampAsModifiedJulianDay = stampAsModifiedJulianDay;
            this.durationInDays = durationInDays;
        }

        public Time(DateTime dateTime)
        {
            double julianDay = ToModifiedJulianDay(dateTime);
            stampAsModifiedJulianDay = julianDay;
        }

        public Time(DateTime start, DateTime end)
        {
            stampAsModifiedJulianDay = ToModifiedJulianDay(start);
            durationInDays = ToModifiedJulianDay(end) - stampAsModifiedJulianDay;
        }

        public Time(ITime start, ITime end)
        {
            StampAsModifiedJulianDay = start.StampAsModifiedJulianDay;
            DurationInDays = end.StampAsModifiedJulianDay - start.StampAsModifiedJulianDay;
        }

        public Time(DateTime start, double durationInDays)
        {
            stampAsModifiedJulianDay = ToModifiedJulianDay(start);
            this.durationInDays = durationInDays;
        }

        #region IComparable Members

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
        ///<param name="obj">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ITime))
            {
                return false;
            }
            return (Equals((ITime)obj));
        }

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
        ///<param name="t">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
        public bool Equals(ITime t)
        {
            if (t == null)
                return false;

            if (!MjDsAreEqual(StampAsModifiedJulianDay, t.StampAsModifiedJulianDay))
                return false;

            if (!MjDsAreEqual(DurationInDays, t.DurationInDays))
                return false;

            return true;
        }

        /// <summary>
        /// Compares two timestamps
        /// </summary>
        /// <param name="obj">The timestamp to compare with</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(object obj)
        {
            if (obj is ITime)
            {
                return(CompareTo((ITime) obj));
            }
            return -1;
        }

        /// <summary>
        /// Compares two timestamps
        /// </summary>
        /// <param name="ts">The timestamp to compare with</param>
        /// <returns>The result of the comparison</returns>
        public int CompareTo(ITime ts)
        {
            int stampDiff = stampAsModifiedJulianDay.CompareTo(ts.StampAsModifiedJulianDay);
            if (stampDiff != 0)
                return (stampDiff);
            return (durationInDays.CompareTo(ts.DurationInDays));
        }

        #endregion

        #region ITime Members

        /// <summary>
        /// Getter and setter for the modified julian day
        /// </summary>
        public double StampAsModifiedJulianDay
        {
            get { return stampAsModifiedJulianDay; }
            set { stampAsModifiedJulianDay = value; }
        }

        public double DurationInDays
        {
            get { return durationInDays; }
            set { durationInDays = value; }
        }

        #endregion

        public void AddDays(double days)
        {
            stampAsModifiedJulianDay += days;
        }

        public void AddSeconds(double seconds)
        {
            stampAsModifiedJulianDay += seconds/24.0/3600.0;
        }

        /// <summary>
        /// Returns the hash code
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return stampAsModifiedJulianDay.GetHashCode()+durationInDays.GetHashCode();
        }

        /// <summary>
        /// Converts the time stamp to a string
        /// </summary>
        /// <returns>String converted time stamp</returns>
        public override String ToString()
        {
            if (DurationInDays != 0 && DurationInDays != double.NaN)
            {
                return (stampAsModifiedJulianDay.ToString(CultureInfo.InvariantCulture) + " - " +
                       (stampAsModifiedJulianDay + DurationInDays).ToString(CultureInfo.InvariantCulture) +
                       " ("+
                       (ToDateTime().ToString("yyyy-MM-dd HH:mm:ss")) + 
                       " - " +
                       (ToDateTime(stampAsModifiedJulianDay + DurationInDays).ToString("yyyy-MM-dd HH:mm:ss")) 
                       +")"
                       );
            }
            return ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Converts the time stamp to DateTime
        /// </summary>
        /// <returns>The DateTime</returns>
        public DateTime ToDateTime()
        {
            return ToDateTime(StampAsModifiedJulianDay);
        }

        /// <summary>
        /// Converts the time stamp to DateTime
        /// </summary>
        /// <returns>The DateTime</returns>
        public DateTime AsDateTime
        {
            get { return ToDateTime(StampAsModifiedJulianDay); }
        }

        public static Time Start(ITime time)
        {
            return (new Time(time.StampAsModifiedJulianDay));
        }

		public static Time End(ITime time)
        {
            return (new Time(time.StampAsModifiedJulianDay + time.DurationInDays));
        }

        /// <summary>
        /// Converts the time stamp to DateTime
        /// </summary>
        /// <returns>The DateTime</returns>
        public static DateTime ToDateTime(double modifiedJulianDay)
        {
            return ModifiedJulianDateZero.AddDays(modifiedJulianDay);
        }

        /// <summary>
        /// Return a date time as a modified julian day.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ToModifiedJulianDay(DateTime dateTime)
        {
            return (dateTime.ToModifiedJulianDay());
        }
    }
}