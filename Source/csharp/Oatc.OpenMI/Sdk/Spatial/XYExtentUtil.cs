namespace Oatc.OpenMI.Sdk.Spatial
{
    /// <summary>
    /// Utility class for creating extends from different geometric figures.
    /// </summary>
    public static class XYExtentUtil
    {
        public static XYExtent GetExtent(XYPoint point)
        {
            return new XYExtent
                       {
                           XMin = point.X,
                           XMax = point.X,
                           YMin = point.Y,
                           YMax = point.Y,
                       };
        }

        public static XYExtent GetExtent(XYPoint point, double epsilon)
        {
            return new XYExtent
            {
                XMin = point.X-epsilon,
                XMax = point.X+epsilon,
                YMin = point.Y-epsilon,
                YMax = point.Y+epsilon,
            };
        }

        public static XYExtent GetExtent(XYLine line)
        {
            XYExtent res = new XYExtent();
            if (line.P1.X < line.P2.X)
            {
                res.XMin = line.P1.X;
                res.XMax = line.P2.X;
            }
            else
            {
                res.XMin = line.P2.X;
                res.XMax = line.P1.X;
            }
            if (line.P1.Y < line.P2.Y)
            {
                res.YMin = line.P1.Y;
                res.YMax = line.P2.Y;
            }
            else
            {
                res.YMin = line.P2.Y;
                res.YMax = line.P1.Y;
            }
            return (res);
        }
        public static XYExtent GetExtent(XYPolyline polyline)
        {
            XYExtent res = new XYExtent();
            foreach (XYPoint point in polyline.Points)
            {
                res.Include(point.X, point.Y);
            }
            return (res);
        }
        public static XYExtent GetExtent(XYPolygon polygon)
        {
            XYExtent res = new XYExtent();
            foreach (XYPoint point in polygon.Points)
            {
                res.Include(point.X, point.Y);
            }
            return (res);
        }
    }
}