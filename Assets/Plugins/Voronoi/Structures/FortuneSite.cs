using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiLib.Structures
{
    public class FortuneSite
    {
        public double X { get; }
        public double Y { get; }

        public List<VEdge> Cell { get; private set; }

        public List<FortuneSite> Neighbors { get; private set; }

        public FortuneSite(double x, double y)
        {
            X = x;
            Y = y;
            Cell = new List<VEdge>();
            Neighbors = new List<FortuneSite>();
        }

        public static explicit operator Vector2(FortuneSite v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }
    }
}
