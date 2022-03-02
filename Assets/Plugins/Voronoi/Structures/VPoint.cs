using System;
using UnityEngine;

namespace VoronoiLib.Structures
{
    public class VPoint
    {
        public double X { get; }
        public double Y { get; }

        internal VPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Vector2(VPoint v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }
    }
}
