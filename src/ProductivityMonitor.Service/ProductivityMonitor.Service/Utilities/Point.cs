using System;
using System.Runtime.InteropServices;

namespace ProductivityMonitor.Service.Utilities
{

	[StructLayout(LayoutKind.Sequential)]
	public struct Point
	{
		public int X;
		public int Y;
	}

	public static class PointExtensions
	{
		public static int DistanceFromSquared(this Point pointA, Point pointB)
		{
			var xDistance = pointA.X - pointB.X;
			var yDistance = pointA.Y - pointB.Y;
			return xDistance * xDistance + yDistance * yDistance;
		}

		public static int DistanceFrom(this Point pointA, Point pointB) =>
			(int) Math.Round(Math.Sqrt(pointA.DistanceFromSquared(pointB)));
	}
}
