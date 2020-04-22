using System;
using System.Collections.Generic;
using ProductivityMonitor.Service.Data.Models;

namespace ProductivityMonitor.Service.Data
{
	public class TimeGrouping
	{
		public DateTime TimePoint { get; set; }
		public ApplicationRecord Application { get; set; }
		public List<KeyboardInputRecord> KeyboardInputs { get; set; }
		public List<MouseInputRecord> MouseInputs { get; set; }
	}
}
