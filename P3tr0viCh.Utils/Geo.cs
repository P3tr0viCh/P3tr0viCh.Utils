using System;

namespace P3tr0viCh.Utils
{
    public static class Geo
    {
        public const int EARTH_RADIUS = 6372795;

        public static double RadToDeg(double radians)
        {
            return (180 / Math.PI) * radians;
        }

        public static double DegToRad(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static double Haversine(double lat1, double lng1, double lat2, double lng2)
        {
            if (lat1 == lat2 && lng1 == lng2)
            {
                return 0.0;
            }

            lat1 = DegToRad(lat1);
            lng1 = DegToRad(lng1);
            lat2 = DegToRad(lat2);
            lng2 = DegToRad(lng2);

            double deltaLat = lat2 - lat1;
            double deltaLng = lng2 - lng1;

            double sinDeltaLat = Math.Sin(deltaLat / 2.0);
            double sinDeltaLng = Math.Sin(deltaLng / 2.0);

            double A = sinDeltaLat * sinDeltaLat +
                Math.Cos(lat1) * Math.Cos(lat2) * sinDeltaLng * sinDeltaLng;

            double C = 2.0 * Math.Asin(Math.Min(1, Math.Sqrt(A)));

            return C * EARTH_RADIUS;
        }
    }
}