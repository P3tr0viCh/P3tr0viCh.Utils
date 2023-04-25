using System;

namespace P3tr0viCh.Utils
{
    public static class Osm
    {
        public static int LatToTileY(double lat, int z)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(Geo.DegToRad(lat)) + 1 / Math.Cos(Geo.DegToRad(lat))) / Math.PI) / 2 * (1 << z));
        }

        public static int LngToTileX(double lng, int z)
        {
            return (int)Math.Floor((lng + 180.0) / 360.0 * (1 << z));
        }

        public static double TileYToLat(int y, int z)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (double)(1 << z);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }

        public static double TileXToLng(int x, int z)
        {
            return x / (double)(1 << z) * 360.0 - 180;
        }
    }
}