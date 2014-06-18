using System;

namespace Oatc.OpenMI.Sdk.Spatial
{
    /// <summary>
    /// Extent is a rectangle in xy-space.
    /// </summary>
    public class XYExtent
    {
        /// <summary>
        /// Minimum x coordinate of extent
        /// </summary>
        public double XMin;
        /// <summary>
        /// Maximum x coordinate of extent
        /// </summary>
        public double XMax;
        /// <summary>
        /// Minimum y coordinate of extent
        /// </summary>
        public double YMin;
        /// <summary>
        /// Maximum y coordinate of extent
        /// </summary>
        public double YMax;

        /// <summary>
        /// Default constructor, creates an empty extent
        /// </summary>
        public XYExtent()
        {
            XMin = Double.MaxValue;
            XMax = Double.MinValue;
            YMin = Double.MaxValue;
            YMax = Double.MinValue;
        }

        /// <summary>
        /// Constructor that ininitalizes the extent to a certain extent.
        /// </summary>
        /// <param name="xmin">Minimum x coordinate</param>
        /// <param name="xmax">Maximum x coordinate</param>
        /// <param name="ymin">Minimum y coordinate</param>
        /// <param name="ymax">Maximum y coordinate</param>
        public XYExtent(double xmin, double xmax, double ymin, double ymax)
        {
            XMin = xmin;
            XMax = xmax;
            YMin = ymin;
            YMax = ymax;
        }

        /// <summary>
        /// Make this extent include <paramref name="other"/>. This will
        /// grow this extent, if the <paramref name="other"/> point is outside
        /// this extent.
        /// </summary>
        /// <param name="other">Other extent to include</param>
        public void Include(XYExtent other)
        {
            if (other.XMin < XMin)
                XMin = other.XMin;
            if (other.XMax > XMax)
                XMax = other.XMax;
            if (other.YMin < YMin)
                YMin = other.YMin;
            if (other.YMax > YMax)
                YMax = other.YMax;
        }

        /// <summary>
        /// Make this extent include the xy-point. This will
        /// grow this extent, if the xy-point is outside
        /// this extent.
        /// </summary>
        /// <param name="x">x coordinate of point to include</param>
        /// <param name="y">y coordinate of point to include</param>
        public void Include(double x, double y)
        {
            if (x < XMin)
                XMin = x;
            if (x > XMax)
                XMax = x;
            if (y < YMin)
                YMin = y;
            if (y > YMax)
                YMax = y;
        }

        /// <summary>
        /// Checks if this extent contains the xy-point
        /// </summary>
        /// <param name="x">x coordinate of point to include</param>
        /// <param name="y">y coordinate of point to include</param>
        /// <returns>True if xy-point is inside (or on boundary of) this extent.</returns>
        public bool Contains(double x, double y)
        {
            return (
                       XMin <= x && x <= XMax &&
                       YMin <= y && y <= YMax
                   );
        }

        /// <summary>
        /// Checks if this extent overlaps the other extent
        /// </summary>
        /// <param name="other">Extent to check overlap with</param>
        /// <returns>True if the two extends overlaps</returns>
        public bool Overlaps(XYExtent other)
        {
            return
                (
                    XMin <= other.XMax && XMax >= other.XMin &&
                    YMin <= other.YMax && YMax >= other.YMin
                );
        }
    }
}