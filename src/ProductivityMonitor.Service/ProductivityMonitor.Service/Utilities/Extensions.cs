using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityMonitor.Service.Utilities
{
	public static class Extensions
	{
		public static DateTime RoundToSeconds(this DateTime dateTime) =>
			new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour,
				dateTime.Minute, dateTime.Second);

		public static bool
			SecondsEqual(this DateTime dateTimeA, DateTime dateTimeB) =>
			(int) (dateTimeA - dateTimeB).TotalSeconds == 0;
	}
}
